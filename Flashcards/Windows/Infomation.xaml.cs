using System.Windows;
using System.Windows.Input;

namespace Flashcards.Windows
{
    /// <summary>
    /// Interaction logic for Infomation.xaml
    /// </summary>
    public partial class Infomation : Window
    {
        public Infomation()
        {
            InitializeComponent();
            this.PreviewKeyDown += Infomation_PreviewKeyDown;
        }

        private void Infomation_PreviewKeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Escape)
                Close();
        }

        private void Grid_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if (e.ChangedButton == MouseButton.Left)
                this.DragMove();
        }
    }
}
