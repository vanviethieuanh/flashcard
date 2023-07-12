using System.Windows.Controls;
using System.Windows;
using Flashcards.Windows;
using Flashcards.Class;

namespace Flashcards.UC
{
    /// <summary>
    /// Interaction logic for UCContent.xaml
    /// </summary>
    public partial class UCContent : UserControl
    {
        private Content cont;
        public Content Cont { get => cont;
            set
            {
                cont = value;
                if (!string.IsNullOrEmpty(cont.Header))
                    txt_header.Visibility = Visibility.Visible;
                txt_header.Text = cont.Header;
                txt_header.ToolTip = cont.Header.TranslateString("EN","VI").EncodeTransform();
                txt_cont.Text = cont.Cont.ToString();
            }
        }

        public UCContent()
        {
            InitializeComponent();
        }

        private void MenuItem_Click(object sender, RoutedEventArgs e)
        {
            if (txt_cont.SelectionLength < 160)
            {
                if (txt_cont.SelectionLength > 1)
                {
                    WTranslation trans = new WTranslation();
                    trans.Show(txt_cont.SelectedText.TranslateString("EN", "VI").EncodeTransform());
                }
            }
            else {
                Prompter.Show("Less than 160 characters");
            }
           
        }
    }
}
