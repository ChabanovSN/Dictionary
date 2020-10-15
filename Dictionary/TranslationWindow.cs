using System;
using System.Collections.Generic;
using System.Drawing;
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
        private TextBox Word;
        private TextBox AddWordText;
        private Button FindWord;
        private Button AddWord;
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
            if (AddWordText.Text.Length>0 && Word.Text.Length>0)
            DBHilper.Insert(table.fromTable, table.toTable,table.midTable, Word.Text.Trim(), 
                                                                           AddWordText.Text.Trim());
            FindWord_Click(null, null);
        }


        void FindWord_Click(object sender, EventArgs e)
        {
            if (Word.Text.Length < 1) return;

                ListWords = DBHilper.GetListTranslaters(table.fromTable, table.toTable,
                                                    table.midTable, Word.Text.Trim());
            if (ListWords.Count < 1)
            {
                MessageBox.Show("Empty");
                return;
            }
            Translation.Items.Clear();
            Translation.BeginUpdate();

            foreach (var item in ListWords)
            {
                Translation.Items.Add(item);
            }
            Translation.EndUpdate();
        }


        private void Init()
        {

            StartPosition = FormStartPosition.CenterScreen;
            var currentSize = Font.SizeInPoints;
            currentSize += 3;
            Font = new Font(Font.Name, currentSize,
                 Font.Style);
            Text = table.fromName + " - " + table.toName;

            FromName = new Label {

            };
            ToName = new Label {

            };
            Word = new TextBox {
                Location = new Point(10, 50),
                Size = new Size(400, 25),
                Width = 400
            
            };
            AddWordText = new TextBox
            {
                Location = new Point(10, 25),
                Size = new Size(400, 25),
                Width = 400

            };
            FindWord = new Button
            {
                Location = new Point(10, 80),
                Size = new Size(80, 25),
                Text = "Find"

            };
            FindWord.Click +=FindWord_Click;
            AddWord = new Button
            {
                Location = new Point(100, 80),
                Size = new Size(80, 25),
                Text = "Add"


            };
            AddWord.Click += AddWord_Click;
            DeleteWord = new Button
            {
                Location = new Point(190, 80),
                Size = new Size(80, 25),
                Text = "Delete"
            };
            Translation = new ListBox {
                Location = new Point(10, 110),
                Size = new Size(400, 180),
                ScrollAlwaysVisible = true
            };

            Controls.AddRange(new Control[] {FromName,ToName, Word, FindWord, 
                AddWord, DeleteWord, Translation,AddWordText});
         this.ClientSize = new Size(420, 350);
    }
    }


}
