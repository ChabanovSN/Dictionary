using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;

namespace Dictionary
{
    public class WainWindow: Form
    {
        private ListBox ListOdDictionary;
        private Dictionary<string, int> MapOfDictionary;
        public WainWindow()
        {
            MapOfDictionary = DBHilper.GetDictionary();
            Init();
        }

        private void Init() {

            StartPosition = FormStartPosition.CenterScreen;
            var currentSize = Font.SizeInPoints;
            currentSize += 3;
            Font = new Font(Font.Name, currentSize,
                 Font.Style);
            ListOdDictionary = new ListBox
            {
                Size = new Size(400, 200),
                Location = new Point(10, 10)
            };
            ListOdDictionary.Click += ListOdDictionary_Click;
            this.Controls.Add(ListOdDictionary);
      
                ListOdDictionary.MultiColumn = false;          
                ListOdDictionary.SelectionMode = SelectionMode.One;
                ListOdDictionary.BeginUpdate();
       
            foreach (var item in MapOfDictionary)
            {
                ListOdDictionary.Items.Add(item.Key.PadLeft(50-item.Key.Length/2));
            }
                ListOdDictionary.EndUpdate();
            this.ClientSize = new Size(420, 250);
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
