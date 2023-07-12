using System.Windows;
using System.Windows.Input;

namespace Flashcards.Windows
{
    /// <summary>
    /// Interaction logic for Prompter.xaml
    /// </summary>
    public partial class Prompter : Window
    {
        public enum SelectionMode
        {
            OK, Close, Cancel
        };

        private SelectionMode _userSelection;

        public SelectionMode UserSelection { get => _userSelection; set => _userSelection = value; }

        public Prompter()
        {
            InitializeComponent();
        }

        private void btn_Ok_Click(object sender, RoutedEventArgs e)
        {
            UserSelection = SelectionMode.OK;
            Close();
        }

        private void btn_Cancel_Click(object sender, RoutedEventArgs e)
        {
            UserSelection = SelectionMode.Cancel;
            Close();
        }

        private void btn_close_Click(object sender, RoutedEventArgs e)
        {
            UserSelection = SelectionMode.Close;
            Close();
        }

        public static SelectionMode Show(string Message)
        {
            Prompter dialog = new Prompter();
            dialog.txt_message.Text = Message;

            dialog.ShowDialog();
            return dialog.UserSelection;
        }

        public static SelectionMode Show(string Message, string Header)
        {
            Prompter dialog = new Prompter();
            dialog.txt_message.Text = Message;
            dialog.lbl_header.Content = Header;

            dialog.ShowDialog();
            return dialog.UserSelection;
        }

        private void Grid_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if (e.ChangedButton == MouseButton.Left)
                this.DragMove();
        }
    }
}
