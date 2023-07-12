using System.Windows;
using Flashcards.Class;
using System.Windows.Media.Imaging;
using System;
using System.Windows.Media.Effects;

namespace Flashcards.Windows
{
    /// <summary>
    /// Interaction logic for Personalize.xaml
    /// </summary>
    public partial class Personalize : Window
    {
        public event EventHandler ChangeBlur;
        private string path = "";
        private bool isSetData = false;
        private PersonalizeData userCustom;
        private bool isSaved;

        public PersonalizeData UserCustom { get => userCustom; set => userCustom = value; }
        public bool IsSaved { get => isSaved; set => isSaved = value; }

        public Personalize(PersonalizeData data)
        {
            InitializeComponent();
            PreviewKeyDown += Personalize_PreviewKeyDown;
            list_colors.ItemsSource = MainColors.ListColor;
            list_colors.SelectedIndex = data.IndexMainColor;
            if (data.BackgoundPath != "")
            {
                ckb_AllowTheme.IsChecked = true;
                path = data.BackgoundPath;
            }
            isSetData = true;
            sld_Blur.Value = data.Blur;
        }

        private void Personalize_PreviewKeyDown(object sender, System.Windows.Input.KeyEventArgs e)
        {
            if (e.Key == System.Windows.Input.Key.Escape)
            {
                Close();
                IsSaved = false;
            }
        }

        private void list_colors_SelectionChanged(object sender, System.Windows.Controls.SelectionChangedEventArgs e)
        {
            Application.Current.Resources["Main_color.light"] = MainColors.ListColor[list_colors.SelectedIndex].Light;
            Application.Current.Resources["Main_color.dark"] = MainColors.ListColor[list_colors.SelectedIndex].Dark;
        }

        private void Grid_Drop(object sender, DragEventArgs e)
        {
            if (e.Data.GetDataPresent(DataFormats.FileDrop))
            {
                string[] files = (string[])e.Data.GetData(DataFormats.FileDrop);
                if (files.Length == 1)
                {
                    Application.Current.Resources["BgImage"] = new BitmapImage(new Uri(files[0]));
                    path = files[0];
                }
            }
        }

        private void ckb_AllowTheme_Unchecked(object sender, RoutedEventArgs e)
        {
            Application.Current.Resources["BgImage"] = Application.Current.Resources["BgImage_Cloud"];
        }

        private void sld_Blur_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            if(!isSetData)
                ChangeBlur(sld_Blur.Value, e);
            else isSetData = false;

        }

        private void btn_Save_Click(object sender, RoutedEventArgs e)
        {
            if (ckb_AllowTheme.IsChecked == true)
            {
                PersonalizeData.Save(path, list_colors.SelectedIndex, (byte)sld_Blur.Value);
                UserCustom = new PersonalizeData()
                {
                    Blur = (byte)sld_Blur.Value,
                    BackgoundPath = path,
                    IndexMainColor = (byte)list_colors.SelectedIndex
                };
            }
            else
            {
                PersonalizeData.Save("", list_colors.SelectedIndex, (byte)sld_Blur.Value);
                UserCustom = new PersonalizeData()
                {
                    Blur = (byte)sld_Blur.Value,
                    BackgoundPath = "",
                    IndexMainColor = (byte)list_colors.SelectedIndex
                };
            }
            IsSaved = true;
            Close();
        }

        private void grid_move_MouseDown(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            if (e.ChangedButton == System.Windows.Input.MouseButton.Left)
                DragMove();
        }

        private void btn_Close_Click(object sender, RoutedEventArgs e)
        {
            Close();
            IsSaved = false;
        }
    }
}
