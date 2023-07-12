using System.Windows;
using System.Windows.Input;
using Flashcards.Class;

namespace Flashcards.Windows
{
    /// <summary>
    /// Interaction logic for WTranslation.xaml
    /// </summary>
    public partial class WTranslation : Window
    {
        public WTranslation()
        {
            InitializeComponent();
        }

        public void Show(string content)
        {
            var mouse = MousePosition.GetMousePosition();
            Left = mouse.X + ActualWidth;
            Top = mouse.Y + ActualHeight;
            txt_Content.Text = content;
            Show();
        }

        private void btn_Close_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }

        private void Grid_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if (e.ChangedButton == MouseButton.Left)
                DragMove();
        }

        private void ContentControl_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            Close();
        }
    }
}
