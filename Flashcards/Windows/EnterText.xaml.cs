using Flashcards.Class;
using System.Text;
using System.Windows;

namespace Flashcards.Windows
{
    /// <summary>
    /// Interaction logic for EnterText.xaml
    /// </summary>
    public partial class EnterText : Window
    {
        private bool _ok;
        private string _header;
        private string _placeHolder;
        private string _result;
        private ListDictionary _listDictionary;

        public bool Ok { get => _ok; set => _ok = value; }
        public string Header
        {
            get => _header; set
            {
                _header = value;
                lbl_Header.Content = Header;
            }
        }
        public string PlaceHolder
        {
            get => _placeHolder; set
            {
                _placeHolder = value;
                textbox.Tag = PlaceHolder;
            }
        }
        public string Result { get => _result; set => _result = value; }
        public ListDictionary ListDictionary { get => _listDictionary; set => _listDictionary = value; }

        public EnterText()
        {
            InitializeComponent();
            textbox.Focus();
            this.DataContext = DataContext;
        }

        private void btn_Ok_Click(object sender, RoutedEventArgs e)
        {
            if (textbox.Text.Contains(@"\") || textbox.Text.Contains(@":") || textbox.Text.Contains(@"/") || textbox.Text.Contains(@"*")
                  || textbox.Text.Contains(@"?") || textbox.Text.Contains('"'.ToString()) || textbox.Text.Contains(@"<") || textbox.Text.Contains(@">")
                  || textbox.Text.Contains(@"|"))
            {
                StringBuilder sb = new StringBuilder();
                sb.AppendLine("This text can't contain any of the following characters:");
                sb.AppendLine(string.Format(@" \ / : * ? {0} < > |", '"'.ToString()));
                tooltip.Content = sb.ToString();
                tooltip.IsOpen = true;

                textbox.SelectionStart = textbox.Text.Length - 1;
                textbox.SelectionLength = 0;
                return;
            }

            if (!string.IsNullOrEmpty(textbox.Text) && !string.IsNullOrWhiteSpace(textbox.Text))
            {
                Ok = true;
                Result = textbox.Text;
                Close();
            }
            else
            {
                tooltip.Content = string.Format(@"This text can't be null, empty or whitespaces");
                tooltip.IsOpen = true;
                return;
            }
        }

        private void btn_Cancel_Click(object sender, RoutedEventArgs e)
        {
            Ok = false;
            Close();
        }

        private void Rectangle_MouseDown(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            if (e.ChangedButton == System.Windows.Input.MouseButton.Left)
                this.DragMove();
        }

        private void textbox_TextChanged(object sender, System.Windows.Controls.TextChangedEventArgs e)
        {
            if (textbox.Text.Contains(@"\") || textbox.Text.Contains(@":") || textbox.Text.Contains(@"/") || textbox.Text.Contains(@"*")
                   || textbox.Text.Contains(@"?") || textbox.Text.Contains('"'.ToString()) || textbox.Text.Contains(@"<") || textbox.Text.Contains(@">")
                   || textbox.Text.Contains(@"|"))
            {
                StringBuilder sb = new StringBuilder();
                sb.AppendLine("This text can't contain any of the following characters:");
                sb.AppendLine(string.Format(@" \ / : * ? {0} < > |", '"'.ToString()));
                tooltip.Content = sb.ToString();
                tooltip.IsOpen = true;

                textbox.Text = textbox.Text.Substring(0, textbox.Text.Length - 1);
                if (textbox.Text.Length > 0)
                {
                    textbox.SelectionStart = textbox.Text.Length;
                    textbox.SelectionLength = 0;
                }
            }
        }
    }
}
