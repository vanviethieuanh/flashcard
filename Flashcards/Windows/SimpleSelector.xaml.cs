using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using Flashcards.Class.Extension;

namespace Flashcards.Windows
{
    /// <summary>
    /// Interaction logic for SimpleSelector.xaml
    /// </summary>
    public partial class SimpleSelector : Window
    {
        bool isOk;
        List<string> itemsSource;
        List<string> moreItems;
        SelectionMode selectionMode;
        List<int> selectedIndices;
        int selectedIndex;
        string selectedItem;
        string header;
        bool isMoreChecked;

        public SelectionMode SelectionMode { get => selectionMode;
            set {
                selectionMode = value;
                list.SelectionMode = selectionMode;
            }
        }

        public bool IsOk { get => isOk; }

        public List<int> SelectedIndices { get => selectedIndices; }
        public int SelectedIndex { get => selectedIndex; }
        public string SelectedItem { get => selectedItem; }

        public List<string> ItemsSource
        {
            get => itemsSource; set
            {
                itemsSource = value;
                list.ItemsSource = ItemsSource;
            }
        }
        public string Header { get => header; set => header = value; }
        public List<string> MoreItems { get => moreItems; set => moreItems = value; }
        public bool IsMoreChecked { get => isMoreChecked; set => isMoreChecked = value; }

        public SimpleSelector()
        {
            InitializeComponent();
        }

        private void grid_Move_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if (e.ChangedButton == MouseButton.Left)
                DragMove();
        }

        private void btn_cancel_Click(object sender, RoutedEventArgs e)
        {
            isOk = false;
            Close();
        }

        private void btn_OK_Click(object sender, RoutedEventArgs e)
        {
            if (list.SelectedIndex != -1)
            {
                if (SelectionMode == SelectionMode.Multiple)
                    selectedIndices = list.SelectedIndices();
                if (SelectionMode == SelectionMode.Single)
                {
                    selectedIndex = list.SelectedIndex;
                    selectedItem = list.SelectedItem.ToString();
                    selectedIndices[0] = selectedIndex;
                }
                isOk = true;
                Close();
            }
            else {
                Prompter.Show("You must select before click OK","ERROR!");
            }
        }

        public bool Show(List<string> ItemSource, string header)
        {
            Header = header;
            ItemsSource = ItemSource;
            SelectionMode = SelectionMode.Single;
            ShowDialog();
            return IsOk;
        }
        public bool Show(List<string> ItemSource, string header, SelectionMode selectionmode)
        {
            Header = header;
            ItemsSource = ItemSource;
            SelectionMode = selectionmode;
            ShowDialog();
            return IsOk;
        }

        public bool Show(List<string> ItemSource, string header, SelectionMode selectionmode,List<string> more)
        {
            MoreItems = more;
            Header = header;
            ItemsSource = ItemSource;
            SelectionMode = selectionmode;
            btn_More.Visibility = Visibility.Visible;
            ShowDialog();
            return IsOk;
        }

        private void btn_More_Click(object sender, RoutedEventArgs e)
        {
            list.ItemsSource = MoreItems;
            list.Items.Refresh();
            btn_More.Visibility = Visibility.Collapsed;
            IsMoreChecked = true;
        }
    }
}
