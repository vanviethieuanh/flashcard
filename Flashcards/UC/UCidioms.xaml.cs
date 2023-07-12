using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using Flashcards.Class;
using System.Text;

namespace Flashcards.UC
{
    /// <summary>
    /// Interaction logic for UCidioms.xaml
    /// </summary>
    public partial class UCidioms : UserControl
    {
        private Idiom idiom;
        public Idiom Idiom { get => idiom; set
            {
                idiom = value;
                
                DataContext = idiom;
                StringBuilder sb = new StringBuilder();
                foreach (var s in idiom.Meaning)
                {
                    sb.Append("    ").Append(s.Trim());
                    sb.AppendLine();
                }
                t.Text = sb.ToString();
            }
        }
        private string topic;
        public string Topic { get => topic;
            set
            {
                topic = value;
                Idioms = new List<Idiom>();
                List<string> link = Idiom.GetIdioms(topic);
                foreach (var item in link)
                {
                  
                    Idioms.Add(Idiom.GetIdiom(item));
                }
                for (int i = 0; i < Idioms.Count; i++)
                {
                    list_Idioms.Items.Add(new ListViewItem());
                }
                list_Idioms.SelectedIndex = 0;
            }
        }

        List<Idiom> idioms;
        public List<Idiom> Idioms { get => idioms; set => idioms = value; }

        public UCidioms()
        {
            InitializeComponent();
        }

        private void btn_Next_Click(object sender, RoutedEventArgs e)
        {
            if (list_Idioms.SelectedIndex == idioms.Count - 1)
                list_Idioms.SelectedIndex = 0;
            else if (list_Idioms.SelectedIndex < idioms.Count)
                list_Idioms.SelectedIndex += 1;
        }

        private void btn_Previous_Click(object sender, RoutedEventArgs e)
        {
            if (list_Idioms.SelectedIndex > 0)
                list_Idioms.SelectedIndex -= 1;
            else list_Idioms.SelectedIndex = idioms.Count - 1; 
            
        }

        private void list_Idioms_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            Idiom = Idioms[list_Idioms.SelectedIndex];
        }
    }
}
