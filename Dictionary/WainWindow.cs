using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;

namespace Dictionary
{
    public class WainWindow: Form
    {
        private MaskedTextBox FromTextBox;
        private MaskedTextBox ToTextBox;
        private Button CreateDictionry;
        private ListBox ListOdDictionary;
        private Dictionary<string, int> MapOfDictionary;
        public WainWindow()
        {
           
            Init();
        }

        private void Init() {

            StartPosition = FormStartPosition.CenterScreen;
            var currentSize = Font.SizeInPoints;
            currentSize += 3;
            Font = new Font(Font.Name, currentSize,
                 Font.Style);

            FromTextBox = new MaskedTextBox
            {
                Location = new Point(10, 10),
                Size = new Size(160, 25),
              
            };
              ToTextBox = new MaskedTextBox
              {
                  Location = new Point(180, 10),
                  Size = new Size(140, 25),

              };
            CreateDictionry = new Button {
          
                Location = new Point(330, 10),
                Size = new Size(80, 25),
                    Text= "Create"       

        };
            CreateDictionry.Click += CreateDictionry_Click;
           ListOdDictionary = new ListBox
            {
                Size = new Size(400, 200),
                Location = new Point(10, 80)
            };
            ListOdDictionary.Click += ListOdDictionary_Click;
            this.Controls.AddRange(new Control[] { FromTextBox, ToTextBox, CreateDictionry, ListOdDictionary });
      
                ListOdDictionary.MultiColumn = false;          
                ListOdDictionary.SelectionMode = SelectionMode.One;
            GetListOfDictionary();
            this.ClientSize = new Size(420, 250);
        }

        private void GetListOfDictionary() {
            MapOfDictionary = DBHilper.GetDictionary();
            ListOdDictionary.Items.Clear();
            ListOdDictionary.BeginUpdate();

            foreach (var item in MapOfDictionary)
            {
                ListOdDictionary.Items.Add(item.Key.PadLeft(50 - item.Key.Length / 2));
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
                Console.WriteLine($" {fromName} , {toName}, {fromTable}, {toTable}, {midTable}");
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

    }
}
