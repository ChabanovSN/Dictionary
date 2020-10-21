using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;

namespace Dictionary
{
    public class MainWindow: Form
    {
        private Label FromLabel;
        private Label ToLabel;
        private MaskedTextBox FromTextBox;
        private MaskedTextBox ToTextBox;
        private Button CreateDictionry;
        private ListBox ListOdDictionary;
        private Dictionary<string, int> MapOfDictionary;
        public MainWindow()
        {
           
            Init();
        }

        private void Init() {
            Text = "Список словарей";
            BackColor = Color.BurlyWood;
            StartPosition = FormStartPosition.CenterScreen;
            var currentSize = Font.SizeInPoints;
            currentSize += 3;
            Font = new Font(Font.Name, currentSize,
                 Font.Style);
            FromLabel = new Label {
                Location = new Point(10, 30),
                Size = new Size(160, 20),
                Text = "С какого языка"

            };
            FromTextBox = new MaskedTextBox
            {
                Location = new Point(10, 50),
                Size = new Size(160, 20),
             
              
            };
            ToLabel = new Label
            {
                Location = new Point(180, 30),
                Size = new Size(140, 20),
                Text = "На какой язык"

            };
            ToTextBox = new MaskedTextBox
              {
                  Location = new Point(180, 50),
                  Size = new Size(140, 20),
                 

              };
            CreateDictionry = new Button {
          
                Location = new Point(330,35),
                Size = new Size(100, 40),
                    Text= " Новый\nсловарь"       

        };
            CreateDictionry.Click += CreateDictionry_Click;
           ListOdDictionary = new ListBox
            {
                Size = new Size(430, 200),
                Location = new Point(10, 80)
            };
            ListOdDictionary.Click += ListOdDictionary_Click;
            this.Controls.AddRange(new Control[] {FromLabel,ToLabel, FromTextBox, ToTextBox, CreateDictionry, ListOdDictionary });
      
                ListOdDictionary.MultiColumn = false;          
                ListOdDictionary.SelectionMode = SelectionMode.One;
            ListOdDictionary.DrawMode = DrawMode.OwnerDrawFixed;
            ListOdDictionary.DrawItem += ListOdDictionary_DrawItem;
            GetListOfDictionary();



            this.ClientSize = new Size(450,300);
        }

        private void GetListOfDictionary() {
            MapOfDictionary = DBHilper.GetDictionary();
            ListOdDictionary.Items.Clear();
            ListOdDictionary.BeginUpdate();

            foreach (var item in MapOfDictionary)
            {
                ListOdDictionary.Items.Add(item.Key);
            }
            ListOdDictionary.EndUpdate();
        }

        private void CreateDictionry_Click(object sender, EventArgs e)
        {
            if (FromTextBox.Text.Length > 0 && ToTextBox.Text.Length > 0)
            {
                string fromName = FromTextBox.Text.Trim().ToLower();
                string toName = ToTextBox.Text.Trim().ToLower();
                var fromNameDig = FromTextBox.Text.Trim().ToLower().Select( c => ((int) c).ToString()).ToArray();
                var toNameDig = ToTextBox.Text.Trim().ToLower().Select(c => ((int)c).ToString()).ToArray();
                string fromTable = "from" + string.Join("", fromNameDig);
                string toTable = "to" + string.Join("", toNameDig);
                string midTable = "mid" + fromTable + toTable;
               
                DBHilper.CreateNewDictionary(fromName, toName, fromTable, toTable, midTable);
                GetListOfDictionary();
              
            }
        }

        void ListOdDictionary_Click(object sender, EventArgs e)
        {
            int id = MapOfDictionary[ListOdDictionary.SelectedItem.ToString().Trim()];
            TranslationWindow translation = new TranslationWindow(id);
            translation.Show();
            Hide();
         // MessageBox.Show(id.ToString());
        }
        private void ListOdDictionary_DrawItem(object sender, DrawItemEventArgs e)
        {
            e.DrawBackground();
            if (e.Index >= 0)
            {
                var box = (ListBox)sender;
                var fore = box.ForeColor;
              //  if ((e.State & DrawItemState.Selected) == DrawItemState.Selected) fore = SystemColors.HighlightText;
                TextRenderer.DrawText(e.Graphics, box.Items[e.Index].ToString(),
                    box.Font, e.Bounds, fore);
            }
            e.DrawFocusRectangle();
        }
    }
}
