using System;
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


        private Label FromName;
        private Label ToName;
        private MaskedTextBox Word;
        private MaskedTextBox AddTranslationText;
        private Button FindWord;
        private Button AddTranslation;
        private Button DeleteWord;
        private ListBox Translation;
        private List<string> ListWords;

        public TranslationWindow(int id)
        {
            table = DBHilper.GetTable(id);
            Init();
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
                MessageBox.Show("Empty");
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
            if (Translation.Items.Count == 1) {
                MessageBox.Show("The last word!");
               
            }
            string word = Word.Text.Trim().ToLower();
            string translation = Translation.SelectedItem.ToString().ToLower();
            Translation.Items.Clear();
            DBHilper.Delete(table.fromTable, table.toTable, table.midTable, word, translation);
            FindWord_Click(null, null);


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

            StartPosition = FormStartPosition.CenterScreen;
            var currentSize = Font.SizeInPoints;
            currentSize += 3;
            Font = new Font(Font.Name, currentSize,
                 Font.Style);
            Text = UppercaseFirst(table.fromName) + " - " + UppercaseFirst(table.toName);

            FromName = new Label {

            };
            ToName = new Label {

            };
            Word = new MaskedTextBox
            {
                Location = new Point(10, 50),
                Size = new Size(400, 25),
                Width = 400,
                AsciiOnly = false

            };
            AddTranslationText = new MaskedTextBox
            {
                Location = new Point(10, 25),
                Size = new Size(400, 25),
                Width = 400,
                AsciiOnly = false

            };
            FindWord = new Button
            {
                Location = new Point(10, 80),
                Size = new Size(80, 25),
                Text = "Find"

            };
            FindWord.Click +=FindWord_Click;
            AddTranslation = new Button
            {
                Location = new Point(100, 80),
                Size = new Size(80, 25),
                Text = "Add"


            };
            AddTranslation.Click += AddWord_Click;
            DeleteWord = new Button
            {
                Location = new Point(190, 80),
                Size = new Size(80, 25),
                Text = "Delete"
            };
            DeleteWord.Click += DeleteWord_Click;
            Translation = new ListBox {
                Location = new Point(10, 110),
                Size = new Size(400, 180),
                ScrollAlwaysVisible = true
            };

            Controls.AddRange(new Control[] {FromName,ToName, Word, FindWord, 
                AddTranslation, DeleteWord, Translation,AddTranslationText});
         this.ClientSize = new Size(420, 350);
    }
    }


}
