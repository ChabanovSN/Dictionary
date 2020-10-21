using System;
using System.Text;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;

namespace Dictionary
{  public struct Table {
         public int id;
         public string fromName;
         public string toName;
         public string fromTable;
         public string toTable;
         public string midTable;
        public override string ToString()
        {
            return $"{id} {fromName} {toName} {fromTable} {toTable} {midTable}";
        }
    }
   
    public class TranslationWindow : Form
    {
        private Table table;


        private Label AddTranslationLabel;
        private Label WordLabel;
        private MaskedTextBox Word;
        private MaskedTextBox AddTranslationText;
        private Button FindWord;
        private Button AddTranslation;
        private Button DeleteWord;
        private ListBox Translation;
        private Button DownloudBtn;
        private Button GetPathBtn;
        private List<string> ListWords;
        FolderBrowserDialog openFD;
        private string pathToLoad = ".";
        private Label PathLabel;
        private Button ReturnBtn;
        public TranslationWindow(int id)
        {  
            openFD = new FolderBrowserDialog();
            table = DBHilper.GetTable(id);
            Init();
            Word.Select();
        }

        void AddWord_Click(object sender, EventArgs e)
        {  
           
            if (AddTranslationText.Text.Length > 0 && Word.Text.Length > 0)
            {
                string word = Word.Text.Trim().ToLower();
                string translation = AddTranslationText.Text.Trim().ToLower();
                if (Translation.Items.Count == 0) // если слова нет то сначала записать слово, а потом перевод
                {
                    DBHilper.InsertInFROMTable(table.fromTable, word);
                }
                    DBHilper.Insert(table.fromTable, table.toTable, table.midTable, word,translation);
               
            }


            FindWord_Click(null, null);
        }


        void FindWord_Click(object sender, EventArgs e)
        {
            if (Word.Text.Length < 1) return;

                ListWords = DBHilper.GetListTranslaters(table.fromTable, table.toTable,
                                                    table.midTable, Word.Text.Trim());
            if (ListWords.Count == 0)
            {
                MessageBox.Show("Пусто");
                Translation.Items.Clear();
                return;
            }
            else
            {
                Translation.Items.Clear();
                Translation.BeginUpdate();

                foreach (var item in ListWords)
                {
                    Translation.Items.Add(UppercaseFirst(item));
                }
                Translation.EndUpdate();
            }
        }

        void DeleteWord_Click(object sender, EventArgs e)
        {
            if (DeleteWord.Text == "Удалить перевод")
            {

                if (Translation.Items.Count == 1)
                {
                    MessageBox.Show("Последнее слово!");

                }
                if (Word.Text.Length > 0 && Translation.SelectedItem != null)
                {
                    string word = Word.Text.Trim().ToLower();
                    string translation = Translation.SelectedItem.ToString().ToLower();
                    Translation.Items.Clear();
                    DBHilper.Delete(table.fromTable, table.toTable, table.midTable, word, translation);
                    FindWord_Click(null, null);
                }
            }
            else
            {
                string word = Word.Text.Trim().ToLower();             
                Translation.Items.Clear();
                DBHilper.Delete(table.fromTable, table.toTable, table.midTable, word);
                Word.Text = "";
                AddTranslationText.Text = "";
                FindWord_Click(null, null);
            }

        }

        private void FindPath_Click(object sender, EventArgs e)
        {

            if (openFD.ShowDialog() == DialogResult.OK)
            {
                pathToLoad = openFD.SelectedPath;
                PathLabel.Text = pathToLoad;
            }
           
        }
        private void DownloudBtn_Click(object sender, EventArgs e)
        {
            if (Word.Text.Length > 0 && Translation.Items.Count>0)
            {
                StringBuilder builder = new StringBuilder();
                builder.Append(Word.Text+ Environment.NewLine);
                int k = 1;
                foreach (var item in Translation.Items)
                {
                    builder.Append($" {k++}) {item.ToString()} {Environment.NewLine}");
                }         
                FileHelper.CreateFile(pathToLoad, Word.Text.Trim(), builder.ToString());
            }
            else {
                MessageBox.Show("Выгружать нечего");
            }

        }

        void ReturnBtn_Click(object sender, EventArgs e)
        {
            MainWindow ifrm = (Dictionary.MainWindow)Application.OpenForms[0];
            if (ifrm is MainWindow frm)
            {            
                frm.Show();
                this.Close();
            }
        }


        private string UppercaseFirst(string s)
        {

            if (string.IsNullOrEmpty(s))
            {
                return string.Empty;
            }
            char[] a = s.ToCharArray();
            a[0] = char.ToUpper(a[0]);
            return new string(a);
        }
        private void Init()
        {
            BackColor = Color.BurlyWood;
            StartPosition = FormStartPosition.CenterScreen;
            var currentSize = Font.SizeInPoints;
            currentSize += 3;
            Font = new Font(Font.Name, currentSize,
                 Font.Style);
            Text = UppercaseFirst(table.fromName) + " - " + UppercaseFirst(table.toName);


            PathLabel = new Label {
                Location = new Point(10, 5),
                Size = new Size(405, 20),
                Text = "Расположение файла выгрузки"
            };
            GetPathBtn = new Button {
                Location = new Point(10, 25),
                Size = new Size(280, 25),
                Text ="Найти место для выгрузки"

            };
            GetPathBtn.Click += FindPath_Click;

            DownloudBtn = new Button
            {
                Location = new Point(300, 25),
                Size = new Size(110, 25),
                Text = "Выгрузить"

            };
            DownloudBtn.Click += DownloudBtn_Click;
            AddTranslationLabel = new Label {
                Location = new Point(10, 65),
                Size = new Size(400, 25),
                Text = "Добавить вариант перевода"
            };
            AddTranslationText = new MaskedTextBox
            {
                Location = new Point(10, 90),
                Size = new Size(400, 25),
                Width = 400,
                AsciiOnly = false

            };
            FindWord = new Button
            {
                Location = new Point(10, 135),
                Size = new Size(95, 25),
                Text = "Найти"

            };
            FindWord.Click += FindWord_Click;
            AddTranslation = new Button
            {
                Location = new Point(110, 135),
                Size = new Size(95, 25),
                Text = "Добавить"


            };
            AddTranslation.Click += AddWord_Click;
            DeleteWord = new Button
            {
                Location = new Point(210, 135),
                Size = new Size(200, 25),

            };
            DeleteWord.Click += DeleteWord_Click;
            WordLabel = new Label {
                Location = new Point(10, 170),
                Size = new Size(400, 20),
                Text = "Искать слово"
            };
            Word = new MaskedTextBox
            {
                Location = new Point(10, 190),
                Size = new Size(400, 25),
                Width = 400,
                AsciiOnly = false,
              

        };
           
            Word.GotFocus += (s, a) => DeleteWord.Text = "Удалить слово";

        
  
            Translation = new ListBox {
                Location = new Point(10, 230),
                Size = new Size(400, 180),
                ScrollAlwaysVisible = true
            };
            Translation.GotFocus += (s, a) => DeleteWord.Text = "Удалить перевод";

            ReturnBtn = new Button {
                Location = new Point(10, 410),
                Size = new Size(100, 25),
                Text = "Вернуться" 
            };
            ReturnBtn.Click +=ReturnBtn_Click;
            Controls.AddRange(new Control[] { AddTranslationLabel, PathLabel,DownloudBtn, GetPathBtn,AddTranslationLabel,WordLabel, Word, FindWord, 
                              AddTranslation, DeleteWord, Translation,AddTranslationText,  ReturnBtn});
         this.ClientSize = new Size(420, 440);
    }

       
    }


}
