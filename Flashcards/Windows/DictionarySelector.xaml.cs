using Flashcards.Class;
using System.Windows;
using System.Windows.Input;

namespace Flashcards.Windows
{
    /// <summary>
    /// Interaction logic for DictionarySelector.xaml
    /// </summary>
    public partial class DictionarySelector : Window
    {
        private bool _isSelect;
        private DictionaryInfo _selectedItem;
        private int _selectedIndex;

        public DictionaryInfo SelectedItem { get => _selectedItem; set => _selectedItem = value; }
        public int SelectedIndex { get => _selectedIndex; set => _selectedIndex = value; }
        public bool IsSelect { get => _isSelect; set => _isSelect = value; }

        private ListDictionary cloud;
        public DictionarySelector(ListDictionary list)
        {
            InitializeComponent();
            list_dictionary.ItemsSource = list.ListDictionaryInfo;
            cloud = list;
        }

        protected void HandleDoubleClick(object sender, MouseButtonEventArgs e)
        {
            //set result
            SelectedIndex = list_dictionary.SelectedIndex;
            SelectedItem = cloud.ListDictionaryInfo[SelectedIndex];
            IsSelect = true;

            Close();
        }

        private void btn_cancel_Click(object sender, RoutedEventArgs e)
        {
            IsSelect = false;
            Close();
        }

        private void Rectangle_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if (e.ChangedButton == MouseButton.Left)
                this.DragMove();
        }
    }
}
