using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using System.Windows.Media.Effects;
using System.IO;
using Microsoft.Win32;
using System.Windows.Media.Animation;
using System.Collections.ObjectModel;
using Microsoft.WindowsAPICodePack.Dialogs;
using Flashcards.Windows;
using Flashcards.Class;
using Flashcards.Class.Extension;
using Flashcards.UC;

namespace Flashcards
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {

        #region Declare

        enum MainWindowsMode { Extending, Curtailing }

        SettingsData SettingsData;

        private MainWindowsMode windowsMode = MainWindowsMode.Extending;

        private string DescriptionsCurrentWord_Search;
        private string fist_Image_source;

        private static CurrentWord currentWord_search = new CurrentWord();
        private static CurrentWord currentWord_dictionary = new CurrentWord();

        private int IndexcurrentDictionaryInfo;
        private ObservableCollection<WordOfDictionary> currentDictionary;

        DoubleAnimation animeX;
        DoubleAnimation animeY;

        List<Word> searching_word;
        
        private ListDictionary _listDictionaryInfo;
        public ListDictionary ListDictionaryInfo { get => _listDictionaryInfo; set => _listDictionaryInfo = value; }

        private PersonalizeData UserCustom;

        #endregion
        public MainWindow()
        {
            InitializeComponent();

            IsEnabled = false;

            #region registry open with window
            RegistryKey regkey = Registry.CurrentUser.CreateSubKey("Software\\Flashcards");
            RegistryKey regstart = Registry.CurrentUser.CreateSubKey("SOFTWARE\\Microsoft\\Windows\\CurrentVersion\\Run");
            string keyvalue = "1";

            try
            {
                regkey.SetValue("Index", keyvalue);
                regstart.SetValue("Flashcards", System.Environment.CurrentDirectory + "\\Flashcard1.0.0.exe");
            }
            catch
            {
            }
            #endregion
            
            if (!Internet.CheckInternetConection())
            {
                Prompter.Show("Lỗi", "Hãy Kiểm tra kế nối Internet của bạn");
            }

            if (!Directory.Exists(Link.PathOfDictionaryFolder))
            {
                Directory.CreateDirectory(Link.PathOfDictionaryFolder);
            }

            IsEnabled = true;
            //load dictionary info
            try
            {
                ListDictionaryInfo = ListDictionary.Load();
            }
            catch
            {
                ListDictionaryInfo = new ListDictionary() { ListDictionaryInfo = new List<DictionaryInfo>() };
            }

            try
            {
                UserCustom = PersonalizeData.Load();
                ResetUI();
                
            }
            catch {
                UserCustom = new PersonalizeData() {
                    Blur = 0,
                    BackgoundPath = "",
                    IndexMainColor = 2
                };
            }

            try
            {
                SettingsData = SettingsData.Load();
                bool done0 = false;
                Random r = new Random();
                int learn = r.Next(0,2);
                switch (learn)
                {
                    case 0:
                        if (SettingsData.Quotes.Count > 0)
                        {
                            Quotes_search.Quotes = Quote.GetTopicQuote(SettingsData.Quotes[r.Next(0, SettingsData.Quotes.Count)]);
                            Quotes_search.Visibility = Visibility.Visible;
                            done0 = true;
                            break;
                        }
                        else {
                            done0 = true;
                            goto case 1;
                        }
                        
                    case 1:
                        if (SettingsData.Idioms.Count > 0)
                        {
                            idioms_search.Topic = SettingsData.Idioms[r.Next(0,SettingsData.Idioms.Count)];
                            idioms_search.Visibility = Visibility.Visible;
                            break;
                        }
                        if (SettingsData.Idioms.Count < 0 && !done0)
                        {
                           goto case 0;
                        }
                        else { break; }
                }
                   
            }
            catch {
                SettingsData = new SettingsData();
            }
        }
        #region Method

        public void ResetUI()
        {
            Application.Current.Resources["Main_color.light"] = MainColors.ListColor[UserCustom.IndexMainColor].Light;
            Application.Current.Resources["Main_color.dark"] = MainColors.ListColor[UserCustom.IndexMainColor].Dark;
            try
            {
                if (File.Exists(UserCustom.BackgoundPath))
                    Application.Current.Resources["BgImage"] = new BitmapImage(new Uri(UserCustom.BackgoundPath));
            }
            catch { }
            img_BackGround.Effect = new BlurEffect() { Radius = UserCustom.Blur };
        }

        public void RefreshListWord()
        {
            list_WordInDictionary.ItemsSource = currentDictionary;
            list_WordInDictionary.Items.Refresh();
        }

        public void RefreshListDictionary()
        {
            list_dictionary_Dictionary.ItemsSource = ListDictionaryInfo.ListDictionaryInfo;
            list_dictionary_Dictionary.Items.Refresh();
        }

        public bool WordFilter(object item)
        {
            if (string.IsNullOrEmpty(txb_Dicitonary_Word_Filter.Text))
                return true;
            else
                return ((item as WordOfDictionary).WordDic.IndexOf(txb_Dicitonary_Word_Filter.Text, StringComparison.OrdinalIgnoreCase) >= 0);
        }

        public void CreateLineChart(List<Tuple<string, List<double>>> Value)
        {
            canvasDots.Children.Clear();
            canvasChartLines.Children.Clear();

            var VerticalValue = new List<double>();

            for (int i = 0; i < Value[0].Item2.Count; i++)
            {
                VerticalValue.Add(0);
            }

            int longestString = 0;

            foreach (var dictionary in Value)
            {
                for (int i = 0; i < dictionary.Item2.Count; i++)
                {
                    VerticalValue[i] += dictionary.Item2[i];
                    if (longestString < dictionary.Item1.Length)
                        longestString = dictionary.Item1.Length;
                }
            }

            List<double> _VerticalValue = Caculator.convertTOpercent(VerticalValue);
            double distance = ((canvasDots.ActualWidth) / (_VerticalValue.Count - 1)) - (22.5 / _VerticalValue.Count);
           

            for (int i = 0; i < _VerticalValue.Count; i++)
            {
                System.Text.StringBuilder sb = new System.Text.StringBuilder();
                if (_VerticalValue.Count == 4)
                {
                    if (i == 3)
                    {
                        sb.Append("Last week");
                    }
                    else {
                        sb.AppendFormat("{0} weeks ago", 3 - i);
                    }
                }
                else {
                    if (i == 11)
                    {
                        sb.Append("Last month");
                    }
                    else {
                        sb.AppendFormat("{0} months ago", 11 - i);
                    }
                }

                sb.AppendLine();
                sb.Append(new string('-',longestString*2));
                sb.AppendLine();

                foreach (var dictionary in Value)
                {
                    if (dictionary.Item2[i] > 1)
                    {
                        sb.AppendFormat("{0}: {1} words", dictionary.Item1, dictionary.Item2[i]);
                    }
                    if (dictionary.Item2[i] == 1)
                    {
                        sb.AppendFormat("{0}: {1} word", dictionary.Item1, dictionary.Item2[i]);
                    }
                    sb.AppendLine();
                }

                
                if (VerticalValue[i] > 0)
                {
                    sb.Append(new string('-', longestString * 2));
                    sb.AppendLine();
                    sb.AppendFormat("Total: {0} words", VerticalValue[i]);
                }
                else {
                    sb = new System.Text.StringBuilder(sb.ToString().Trim());
                    sb.AppendLine();
                    sb.Append("No word");
                }

                Ellipse e = new Ellipse();
                e.ToolTip = sb;
                canvasDots.Children.Add(e);
                Canvas.SetBottom(e, (_VerticalValue[i] * (canvasDots.ActualHeight - 15)) / 100);
                Canvas.SetLeft(e, i * distance);
          
                if (i > 0)
                {
                    Line line = new Line();
                    double X1 = (i - 1) * distance + 7.5;
                    double Y1 = ((_VerticalValue[i - 1] * (canvasChartLines.ActualHeight - 15)) / 100) + 7.5;
                    double X2 = i * distance + 7.5;
                    double Y2 = ((_VerticalValue[i] * (canvasChartLines.ActualHeight - 15)) / 100) + 7.5;

                    line.X1 = X1;
                    line.Y1 = Y1;
                    line.X2 = X1;
                    line.Y2 = Y1;

                    animeX = new DoubleAnimation();
                    animeX.To = X2;
                    animeX.Duration = TimeSpan.FromSeconds(Constant.TimeCreateChart);
                    animeX.BeginTime = TimeSpan.FromSeconds(i * Constant.TimeCreateChart);

                    animeY = new DoubleAnimation();
                    animeY.To = Y2;
                    animeY.Duration = TimeSpan.FromSeconds(Constant.TimeCreateChart);
                    animeY.BeginTime = TimeSpan.FromSeconds(i * Constant.TimeCreateChart);

                    canvasChartLines.Children.Add(line);

                    line.BeginAnimation(Line.X2Property, animeX);
                    line.BeginAnimation(Line.Y2Property, animeY);
                }
            }
        }

        public void Group(ListView lv, string Decription)
        {
            CollectionView view = (CollectionView)CollectionViewSource.GetDefaultView(lv.ItemsSource);
            PropertyGroupDescription groupDescription = new PropertyGroupDescription(Decription);
            view.GroupDescriptions.Add(groupDescription);
        }
        
        private void ChangeColor(Color color)
        {
            tabcontrol.BorderBrush = new SolidColorBrush(color);
        }
        
        private void btnSearch_Click(object sender, RoutedEventArgs e)
        {
            if (!string.IsNullOrEmpty(txbSearch.Text) && !string.IsNullOrWhiteSpace(txbSearch.Text))
            {
                try
                {
                    currentWord_search.ThisWord = txbSearch.Text;
                    string html_currentWord = HTML.GetHTMLForSearch(currentWord_search.ThisWord);

                    grid_search_Result.Visibility = Visibility.Visible;
                    grid_search_result_img.Visibility = Visibility.Visible;
                    Quotes_search.Visibility = Visibility.Hidden;
                    grid_search_result_translation.Visibility = Visibility.Hidden;

                    string keyword = StringProcessing.StandardizedStringForCambridge(txbSearch);

                    DescriptionsCurrentWord_Search = Translator.Description(html_currentWord);
                    Search_desciptions.Text = DescriptionsCurrentWord_Search;

                    string[] image_Source = SearchImage.GoogleSrearchImages(keyword, 25);
                    fist_Image_source = image_Source[0];
                    foreach (string url in image_Source)
                    {
                        if (!string.IsNullOrEmpty(url))
                        {
                            Image a = new Image();
                            BitmapImage BitImg = new BitmapImage(new Uri(
                                url, UriKind.RelativeOrAbsolute));
                            a.Source = BitImg;
                            a.Height = (grid_search_result_img.ActualHeight - 74) / 4;
                            stack.Children.Add(a);
                        }
                    }

                    currentWord_search.LinkSpeak = Speak.GetlinkSpeak(html_currentWord);
                    currentWord_search.Pron = Speak.Getpron(html_currentWord);
                    currentWord_search.SourceImage = image_Source[0];
                }
                catch
                {

                }
            }
            else
            {
                return;
            }
        }
        #endregion
        #region Top_Bar
        private void btn_Close_Click(object sender, RoutedEventArgs e)
        {
            Close();
            Application.Current.Shutdown();
        }

        private void btn_Minimize_Click(object sender, RoutedEventArgs e)
        {
            WindowState = WindowState.Minimized;
        }
        #endregion

        #region Event(not all)

        #region tabitem
        //dictionary
        private void TabItem_Selected(object sender, RoutedEventArgs e)
        {
            list_dictionary_Dictionary.ItemsSource = ListDictionaryInfo.ListDictionaryInfo;
        }
        #endregion

        private void btn_Info_Click(object sender, RoutedEventArgs e)
        {
            Infomation infoWindow = new Infomation();
            infoWindow.ShowDialog();
        }

        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            try
            {
                ListDictionary.Save(Link.PathtoListDictionary, ListDictionaryInfo);
            }
            catch { }
            Application.Current.Shutdown();
        }

        private void Window_IsEnabledChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            if (IsEnabled)
                Opacity = 1;
            else
                Opacity = 0.5;
        }
        private void Window_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            if (WindowState == WindowState.Maximized)
            {
                full.IsChecked = true;
                Boss.Margin = new Thickness(0, 0, 0, 0);
                ResizeMode = ResizeMode.CanResize;
            }
            else
            {
                PreviousSize = RenderSize;
                full.IsChecked = false;
                Boss.Margin = new Thickness(15, 15, 15, 15);
                ResizeMode = ResizeMode.CanResizeWithGrip;
            }

            if (Top == 0)
                Boss.Margin = new Thickness { Top = 0 };
            if (Left == 0)
                Boss.Margin = new Thickness { Left = 0 };
            if (ActualWidth + Left == SystemParameters.PrimaryScreenWidth)
                Boss.Margin = new Thickness { Right = 0 };
            if (ActualHeight + Top == SystemParameters.PrimaryScreenHeight)
                Boss.Margin = new Thickness { Bottom = 0 };

            if (ActualWidth < Constant.WidthChangeStyle)
            {
                if (windowsMode == MainWindowsMode.Extending)
                {
                    tabcontrol.Style = FindResource("Small_tabcontrol") as Style;
                    grid_tabitem_container.ColumnDefinitions[0].Width = new GridLength(65);
                    tabitem_Dictionary.Width = 65;
                    tabitem_Search.Width = 65;
                    tabitem_Progress.Width = 65;
                    tabitem_Export_and_Import.Width = 65;
                    windowsMode = MainWindowsMode.Curtailing;
                }
            }
            else
            {
                if (windowsMode == MainWindowsMode.Curtailing)
                {
                    tabcontrol.Style = FindResource("Large_tabcontrol") as Style;
                    grid_tabitem_container.ColumnDefinitions[0].Width = new GridLength(200);
                    tabitem_Dictionary.Width = 200;
                    tabitem_Search.Width = 200;
                    tabitem_Progress.Width = 200;
                    tabitem_Export_and_Import.Width = 200;
                    windowsMode = MainWindowsMode.Extending;
                }
            }
        }

        private void Grid_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if (WindowState == WindowState.Maximized)
            {
                var Position = MousePosition.GetMousePosition();
                Left = Position.X;
                Top = Position.Y - ActualHeight;
                WindowState = WindowState.Normal;
                Width = MinWidth;
                Height = MinHeight;
            }
            else
            {
                if (e.ClickCount == 2)
                    WindowState = WindowState.Maximized;
            }
            if (e.ChangedButton == MouseButton.Left)
            {
                DragMove();
            }
        }


        public Size PreviousSize;
        private void full_Click(object sender, RoutedEventArgs e)
        {

            //edit hear
            if (WindowState != WindowState.Maximized)
            {
                WindowState = WindowState.Maximized;
                Boss.Margin = new Thickness(0, 0, 0, 0);
                full.IsChecked = true;
            }
            else
            {
                WindowState = WindowState.Normal;
                RenderSize = PreviousSize;
                full.IsChecked = false;
            }
        }

        /// <summary>
        /// show setting window
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btn_Settings_Click(object sender, RoutedEventArgs e)
        {
            Settings s = new Settings(SettingsData);
            s.ShowDialog();
            SettingsData = s.Data;
        }
        
        private void btn_search_result_add_Click(object sender, RoutedEventArgs e)
        {
            DictionarySelector selector = new DictionarySelector(ListDictionaryInfo);
            WordData addword = currentWord_search;
            
            selector.ShowDialog();
            if (selector.IsSelect)
            {
                WordData.Add(selector.SelectedItem, addword);
                Word.Save(searching_word.ToArray());

                grid_search_Result.Visibility = Visibility.Hidden;
            }

            currentWord_search = new CurrentWord();
            RefreshListDictionary();
        }

        private void btn_search_result_home_Click(object sender, RoutedEventArgs e)
        {
            grid_search_Result.Visibility = Visibility.Hidden;
            Quotes_search.Visibility = Visibility.Visible;
        }

        private void btn_search_result_us_Click(object sender, RoutedEventArgs e)
        {
            Speak.StartSpeak(currentWord_search.LinkSpeak, Speak.pronSpeakMode.US, Speaker);
        }

        private void btn_search_result_uk_Click(object sender, RoutedEventArgs e)
        {
            Speak.StartSpeak(currentWord_search.LinkSpeak, Speak.pronSpeakMode.UK, Speaker);
        }

        private void grid_search_result_img_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            stack.Children.Clear();

            try
            {
                searching_word = Translator.Translation(currentWord_search.ThisWord);
                w_search_result_trans.Word = searching_word[0];
                
                stk_search_relatedword.Children.Clear();
                for (int i = 1; i < searching_word.Count;i++)
                {
                    UCword word_Display = new UCword() { Word = searching_word[i]};
                    stk_search_relatedword.Children.Add(word_Display);
                    currentWord_search.RelatedWord += Word.ConvertToRelated(searching_word[i]);
                }
                if (stk_search_relatedword.Children.Count > 0)
                    border_search_relatedword_header.Visibility = Visibility.Visible;
                else
                    border_search_relatedword_header.Visibility = Visibility.Hidden;


                lbl_search_result_pronunciations_uk.Text = currentWord_search.Pron[0];
                lbl_search_result_pronunciations_us.Text = currentWord_search.Pron[1];
                
                try { img_search_result.Source = new BitmapImage(new Uri(currentWord_search.SourceImage)); }
                catch { }

                grid_search_result_img.Visibility = Visibility.Hidden;
                grid_search_result_translation.Visibility = Visibility.Visible;
            }
            catch
            {
                Prompter.Show("Thông báo:", "Làm ơn hãy kiểm tra từ bạn nhập");
            }

            List<byte> arr = new List<byte>();

            string a = "0021";
            foreach (char c in a)
            {
                arr.Add(byte.Parse(c.ToString()));
            }

            byte[] result = arr.ToArray();
        }

        private void mit_dictionary_delWord_Click(object sender, RoutedEventArgs e)
        {
            DictionaryInfo _currentDicitonary = ListDictionaryInfo.ListDictionaryInfo[IndexcurrentDictionaryInfo];
            if (btn_dictionary_select.IsChecked == false)
            {
                //change data
                WordData.Delete(ref _currentDicitonary, list_WordInDictionary.SelectedIndex);

                //change ui
                currentDictionary.RemoveAt(list_WordInDictionary.SelectedIndex);
            }
            else
            {
                WordData.Delete(ref _currentDicitonary, list_WordInDictionary.SelectedIndices().ToArray());

                //change ui
                int[] selectedIndices = list_WordInDictionary.SelectedIndices().ToArray();
                selectedIndices.SortDecending();
                foreach (int i in selectedIndices)
                {
                    currentDictionary.RemoveAt(i);
                }

            }
            RefreshListWord();
            ListDictionaryInfo.ListDictionaryInfo[IndexcurrentDictionaryInfo] = _currentDicitonary;
            RefreshListDictionary();
        }

        private void mit_dictionary_moveWord_Click(object sender, RoutedEventArgs e)
        {
            if (list_WordInDictionary.SelectedIndex != -1)
            {
                DictionarySelector selector = new DictionarySelector(ListDictionaryInfo);
                selector.ShowDialog();

                if (selector.IsSelect)
                {
                    DictionaryInfo target = selector.SelectedItem;
                    int targetIndex = selector.SelectedIndex;

                    ListDictionary streamlist = ListDictionaryInfo;

                    WordData.Move(ref streamlist, IndexcurrentDictionaryInfo, targetIndex, list_WordInDictionary.SelectedIndices().ToArray());

                    ListDictionaryInfo = streamlist;

                    //change ui
                    int[] selectedIndices = list_WordInDictionary.SelectedIndices().ToArray();
                    selectedIndices.SortDecending();
                    foreach (int i in selectedIndices)
                    {
                        currentDictionary.RemoveAt(i);
                    }
                    RefreshListWord();
                }
                RefreshListDictionary();
            }
        }

        private void nit_dictionary_rename_Click(object sender, RoutedEventArgs e)
        {

            if (list_dictionary_Dictionary.SelectedIndex != -1)
            {
                EnterText name = new EnterText();
                name.Header = "Rename";
                name.PlaceHolder = "Enter name here!";
                name.ShowDialog();
                if (name.Ok)
                {
                    if (ListDictionary.ExistedDictionary(name.Result,ListDictionaryInfo))
                    {
                        Prompter.SelectionMode selectionmode = Prompter.Show("This dictionary's name already existed. If you continue, we will replace that!" +
                            " Click OK button if you want to continue",
                             "Warning!");
                        if (selectionmode == Prompter.SelectionMode.OK)
                        {
                            DictionaryInfo existedDictionary = ListDictionaryInfo.ListDictionaryInfo.Where(p => p.NameOfDictionary == name.Result).FirstOrDefault();

                            ListDictionary streamlist = ListDictionaryInfo;

                            ListDictionaryInfo.ListDictionaryInfo.Remove(existedDictionary);
                            File.Delete(Link.PathOfDictionary(existedDictionary));

                            ListDictionary.Rename(ref streamlist, list_dictionary_Dictionary.SelectedIndex, name.Result);

                            ListDictionaryInfo = streamlist;
                            RefreshListDictionary();
                        }
                    }
                    else
                    {
                        ListDictionary streamlist = ListDictionaryInfo;
                        ListDictionary.Rename(ref streamlist, list_dictionary_Dictionary.SelectedIndex, name.Result);
                        ListDictionaryInfo = streamlist;
                        RefreshListDictionary();
                    }
                }
            }

            else
            {
                // choose dictionary
                int indexTarget;

                DictionarySelector selector = new DictionarySelector(ListDictionaryInfo);
                selector.ShowDialog();
                if (selector.IsSelect)
                {
                    indexTarget = selector.SelectedIndex;

                    EnterText name = new EnterText();
                    name.Header = "Rename";
                    name.PlaceHolder = "Enter name here!";
                    name.ShowDialog();

                    if (name.Ok)
                    {
                        if (ListDictionary.ExistedDictionary(name.Result,ListDictionaryInfo))
                        {
                            Prompter.SelectionMode selectionmode = Prompter.Show("This dictionary's name already existed. If you continue, we will replace that!" +
                                " Click OK button if you want to continue",
                                "Warning!");
                            if (selectionmode == Prompter.SelectionMode.OK)
                            {
                                DictionaryInfo existedDictionary = ListDictionaryInfo.ListDictionaryInfo.Where(p => p.NameOfDictionary == name.Result).FirstOrDefault();

                                ListDictionary streamlist = ListDictionaryInfo;

                                ListDictionaryInfo.ListDictionaryInfo.Remove(existedDictionary);
                                File.Delete(Link.PathOfDictionary(existedDictionary));

                                ListDictionary.Rename(ref streamlist, indexTarget, name.Result);

                                ListDictionaryInfo = streamlist;
                                RefreshListDictionary();
                            }
                        }

                        else
                        {
                            ListDictionary streamlist = ListDictionaryInfo;
                            ListDictionary.Rename(ref streamlist, indexTarget, name.Result);
                            ListDictionaryInfo = streamlist;
                            RefreshListDictionary();
                        }
                    }
                }
            }
        }

        private void mit_dictionary_deleteDic_Click(object sender, RoutedEventArgs e)
        {
            if (list_dictionary_Dictionary.SelectedIndex == -1)
            {
                DictionarySelector selector = new DictionarySelector(ListDictionaryInfo);
                selector.ShowDialog();

                if (selector.IsSelect)
                {
                    int deleteIndex = selector.SelectedIndex;

                    ListDictionary streamlist = ListDictionaryInfo;

                    ListDictionary.DeleteDic(ref streamlist, deleteIndex);

                    ListDictionaryInfo = streamlist;
                }
                RefreshListDictionary();
            }
            else
            {
                ListDictionary streamlist = ListDictionaryInfo;
                ListDictionary.DeleteDic(ref streamlist, list_dictionary_Dictionary.SelectedIndex);
                ListDictionaryInfo = streamlist;
                RefreshListDictionary();
            }
        }

        private void it_select_Click(object sender, RoutedEventArgs e)
        {
            btn_dictionary_select.IsChecked = true;
        }

        private void txb_Dicitonary_Word_Filter_TextChanged(object sender, TextChangedEventArgs e)
        {
            CollectionViewSource.GetDefaultView(list_WordInDictionary.ItemsSource).Refresh();
        }

        private void btn_dictionary_createDic_Click(object sender, RoutedEventArgs e)
        {
            string nameofdictionary;
            ListDictionary visual = ListDictionaryInfo;

            EnterText name = new EnterText();
            name.Header = "Create a dictionary";
            name.PlaceHolder = "Enter dictionary's name";
            name.ShowDialog();
            if (name.Ok)
            {
                if (ListDictionary.ExistedDictionary(name.Result,ListDictionaryInfo))
                {
                    Prompter.SelectionMode selection = Prompter.Show("This dictionary's name already existed. If you go ahead we will replace the old one." +
                         " Click OK if you really want to go ahead!", "Warning!");
                    if (selection == Prompter.SelectionMode.OK)
                    {
                        DictionaryInfo existedDictionary = ListDictionaryInfo.ListDictionaryInfo.Where(p => p.NameOfDictionary == name.Result).FirstOrDefault();
                        ListDictionaryInfo.ListDictionaryInfo.Remove(existedDictionary);

                        nameofdictionary = name.Result;
                        ListDictionary.CreateDictionary(ref visual, nameofdictionary);
                    }
                }
                else
                {
                    nameofdictionary = name.Result;
                    ListDictionary.CreateDictionary(ref visual, nameofdictionary);
                }
            }

            ListDictionaryInfo = visual;
            list_dictionary_Dictionary.ItemsSource = ListDictionaryInfo.ListDictionaryInfo;
            list_dictionary_Dictionary.Items.Refresh();
        }

        private void tobtn_dictionary_back_Click(object sender, RoutedEventArgs e)
        {
            btn_dictionary_select.IsChecked = false;
        }

        private void ListDicitonary_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            if (list_dictionary_Dictionary.SelectedIndex != -1)
            {
                IndexcurrentDictionaryInfo = list_dictionary_Dictionary.SelectedIndex;
                DictionaryInfo currentdictionaryInfo = ListDictionaryInfo.ListDictionaryInfo[IndexcurrentDictionaryInfo];

                currentDictionary = WordData.Load(currentdictionaryInfo);

                list_WordInDictionary.ItemsSource = currentDictionary;
                Group(list_WordInDictionary, "FistCharacter");

                CollectionView view = (CollectionView)CollectionViewSource.GetDefaultView(list_WordInDictionary.ItemsSource);
                view.Filter = WordFilter;

                tobtn_dictionary_back.IsEnabled = true;
                lbl_dictionary_namedictionary.Content = currentdictionaryInfo.NameOfDictionary;
            }
        }
        #region Disable right-click selection

        protected void HandleDoubleClick(object sender, MouseButtonEventArgs e)
        {
            if (btn_dictionary_select.IsChecked == true)
            {
                return;
            }
            try
            {
                btn_dictionary_select.IsChecked = false;

                WordData selectedWord = WordData.Query(ListDictionaryInfo.ListDictionaryInfo[IndexcurrentDictionaryInfo],
                               currentDictionary[list_WordInDictionary.SelectedIndex].WordDic
                    );

                if (currentDictionary[list_WordInDictionary.SelectedIndex].PracticeDay == "Not seen")
                {
                    WordData.TurnOffNotSeen(ListDictionaryInfo.ListDictionaryInfo[IndexcurrentDictionaryInfo], selectedWord.ThisWord);
                    currentDictionary[list_WordInDictionary.SelectedIndex].PracticeDay = 0.ToString();
                    RefreshListWord();
                }

                List<string> queryWord = new List<string>() { selectedWord.ThisWord };
                queryWord.AddRange(selectedWord.RelatedWord.Split(','));

                List<Word> displayWord = Word.Load(queryWord.ToArray());

                w_dictionary_trans.Word = displayWord[0];
                displayWord.RemoveAt(0);

                int countRelated = 0;
                stk_dictionary_relatedwords.Children.Clear();
                foreach (var word in displayWord)
                {
                    UCword wDisplayer = new UCword();
                    wDisplayer.Word = word;
                    stk_dictionary_relatedwords.Children.Add(wDisplayer);
                    countRelated++;
                }

                scv_dictionary_result_trans.Visibility = Visibility.Visible;
                stk_dictionary_related.Visibility = Visibility.Hidden;
                scrollviewer_dictionary.ScrollToBottom();

                if (countRelated > 0)
                    stk_dictionary_related.Visibility = Visibility.Visible;
            }
            catch {
                Prompter.Show("Error code:fs001", "Error");
            }
            
            
        }

        #endregion

        #endregion
        
        private void btn_share_export_Click(object sender, RoutedEventArgs e)
        {
            DictionarySelector selector = new DictionarySelector(ListDictionaryInfo);
            selector.ShowDialog();
            if (selector.IsSelect)
            {
                tobtn_share_export_back.IsEnabled = true;
                btn_share_export_name.Content = selector.SelectedItem.NameOfDictionary;
                txt_share_export_words.Content = selector.SelectedItem.NumberOfWord;
            }
        }

        private void btn_share_export_list_Click(object sender, RoutedEventArgs e)
        {
            //253355781
        }

        private void btn_share_export_data_Click(object sender, RoutedEventArgs e)
        {
            var dialog = new SaveFileDialog();
            dialog.Filter = "Dictionary|*.xml";
            dialog.OverwritePrompt = true;
            dialog.FileName = btn_share_export_name.Content.ToString();
            dialog.DefaultExt = ".xml";

            if (dialog.ShowDialog() == true)
            {
                string pathfolder = string.Format(@"{0}\{1}",System.IO.Path.GetDirectoryName(dialog.FileName), System.IO.Path.GetFileNameWithoutExtension(dialog.SafeFileName));
                Directory.CreateDirectory(pathfolder);

                ListDictionary.ExportToXMLData(new DictionaryInfo()
                {
                    NameOfDictionary = btn_share_export_name.Content.ToString()
                },
                pathfolder, dialog.SafeFileName);
            }
        }

        string DictionaryFolder;

        private void btn_share_import_data_Click(object sender, RoutedEventArgs e)
        {
            var dialog = new CommonOpenFileDialog();
            
            dialog.Title = "Select Dictionary folder";
            dialog.Multiselect = false;
            dialog.IsFolderPicker = true;

            if (dialog.ShowDialog() == CommonFileDialogResult.Ok)
            {
                DictionaryFolder = dialog.FileName;
                btn_share_import_name.Content = System.IO.Path.GetFileNameWithoutExtension(DictionaryFolder);
                tobtn_share_import_back.IsEnabled = true;
            }
        }
        //sf.xml
        private void btn_share_import_name_Click(object sender, RoutedEventArgs e)
        {
            EnterText rename = new EnterText();
            rename.PlaceHolder = "Enter new name ...";
            rename.Header = "Rename";
            rename.ShowDialog();
            if (rename.Ok)
            {
                btn_share_import_name.Content = rename.Result;
            }
        }

        private void btn_share_import_Click(object sender, RoutedEventArgs e)
        {
            string Name = btn_share_import_name.Content.ToString();
            if (ListDictionary.ExistedDictionary(Name,ListDictionaryInfo))
            {
                Prompter.SelectionMode selection = Prompter.Show("This dictionary already existed. If you go ahead we will replace the old one." +
                         " Click OK if you really want to go ahead!", "Warning!");
                if (selection == Prompter.SelectionMode.OK)
                {
                    ListDictionaryInfo.ListDictionaryInfo.RemoveAll(p => p.NameOfDictionary == Name);
                    ListDictionaryInfo.ListDictionaryInfo.Add(WordData.Import(Name,DictionaryFolder));
                    Word.ImportLibrary(DictionaryFolder);
                }
                else
                {
                    return;
                }
            }
            else {
                ListDictionaryInfo.ListDictionaryInfo.Add(WordData.Import(Name,DictionaryFolder));
                Word.ImportLibrary(DictionaryFolder);
            }

            RefreshListDictionary();
            tobtn_share_import_back.IsEnabled = false;
        }

        private void progress_cmb_timeremaining_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (progress_cmb_timeremaining.SelectedIndex == 1)
            {
                CreateLineChart(ListDictionary.GetMonthProgress());
            }
            else {
                CreateLineChart(ListDictionary.GetYearProgress());
            }
            
        }

        List<Question> Ques;
        int IndexCurrentQues = 0;
        private void btn_practice_start_Click(object sender, RoutedEventArgs e)
        {
            btn_practice_start.Visibility = Visibility.Hidden;
            ques_practice.Visibility = Visibility.Visible;
            grid_practice_summary.Visibility = Visibility.Hidden;

            Ques = Question.Generate(Constant.LimitQuestion);
            ques_practice.Question = Ques[IndexCurrentQues];

            ques_practice.TimeOver += Ques_practice_TimeOver;
            ques_practice.Selected += Ques_practice_Selected;
        }

        private void Ques_practice_Selected(object sender, EventArgs e)
        {
            Ques[IndexCurrentQues].UserSummary = ques_practice.Result;
            if (ques_practice.Result == Question.result.Incorrect)
                Ques[IndexCurrentQues].IncorrectIndex = ques_practice.SelectedIndex;
            
            if (!CheckEndPractice())
            {
                IndexCurrentQues++;
                ques_practice.Question = Ques[IndexCurrentQues];
            }
        }

        private void Ques_practice_TimeOver(object sender, EventArgs e)
        {
            if (ques_practice.Visibility == Visibility.Visible)
            {
                Ques[IndexCurrentQues].UserSummary = ques_practice.Result;
                if (!CheckEndPractice())
                {
                    IndexCurrentQues++;
                    ques_practice.Question = Ques[IndexCurrentQues];
                }
            }
        }

        private void btn_practice_skip_Click(object sender, RoutedEventArgs e)
        {
            if (ques_practice.Visibility == Visibility.Visible)
            {
                Ques[IndexCurrentQues].UserSummary = Question.result.Skip;
                if (!CheckEndPractice())
                {
                    IndexCurrentQues++;
                    ques_practice.Question = Ques[IndexCurrentQues];
                }
            }
            
        }

        public bool CheckEndPractice()
        {
            if (IndexCurrentQues == Constant.LimitQuestion - 1)
            {
                

                btn_practice_start.Visibility = Visibility.Hidden;
                ques_practice.Visibility = Visibility.Hidden;
                grid_practice_summary.Visibility = Visibility.Visible;

                stk_practice_correct.Children.Clear();
                stk_practice_incorrect.Children.Clear();
                stk_practice_SkipOrTimeOver.Children.Clear();

                int correct = 0;
                int i = 0;
                int skipOrOverTime = 0;

                foreach (var q in Ques)
                {
                    switch (q.UserSummary)
                    {
                        case Question.result.Correct:
                            correct++;
                            stk_practice_correct.Children.Add(new UCcorrect(q));
                            break;
                        case Question.result.Incorrect:
                            i++;
                            stk_practice_incorrect.Children.Add(new UCincorrect(q));
                            break;
                        default:
                            skipOrOverTime++;
                            stk_practice_SkipOrTimeOver.Children.Add(new UCcorrect(q));
                            break;
                    }
                }

                lbl_practice_Correct.Content = (correct.ToString());
                lbl_practice_Incorrect.Content = (i.ToString());
                lbl_practice_SkipOrTimeOver.Content = (skipOrOverTime.ToString());

                IndexCurrentQues = 0;
                Ques = new List<Question>();

                return true;
            }
            return false;
        }

        private void grid_import_Drop(object sender, DragEventArgs e)
        {
            string[] files = (string[])e.Data.GetData(DataFormats.FileDrop);
            if (files.Length == 1)
            {
                DictionaryFolder = files[0];
                btn_share_import_name.Content = System.IO.Path.GetFileNameWithoutExtension(DictionaryFolder);
                tobtn_share_import_back.IsEnabled = true;
            }
        }

        private void btn_Personalize_Click(object sender, RoutedEventArgs e)
        {
            Personalize p = new Personalize(UserCustom);
            p.ChangeBlur += P_ChangeBlur;
            p.ShowDialog();
            if (p.IsSaved)
                UserCustom = p.UserCustom;
            else ResetUI();
        }

        private void P_ChangeBlur(object sender, EventArgs e)
        {
            img_BackGround.Effect = new BlurEffect() { Radius = double.Parse(sender.ToString())};
        }

        private void btn_wiki_search_Click(object sender, RoutedEventArgs e)
        {
            stk_wiki_search.VerticalAlignment = VerticalAlignment.Top;
            img_wiki.Visibility = Visibility.Collapsed;
            grid_wiki_content.Visibility = Visibility.Visible;
            stk_wiki_content.Children.Clear();

            List<Section> Swiki = txt_wiki_search.Text.GetWiki();

            foreach (Section s in Swiki)
            {
                if (!Section.IsNullSection(s))
                {
                    UCSection uc = new UCSection();
                    uc.Sec = s;

                    stk_wiki_content.Children.Add(uc);
                }
            }
        }

        private void btn_text_translate_Click(object sender, RoutedEventArgs e)
        {
            string input = txt_text_input.Text;
            if (!string.IsNullOrEmpty(input))
            {
                string output = input.TranslateString("EN", "VI");
                txt_text_output.Text = output.EncodeTransform();
                if (txt_text_output.Text.Length < 100)
                    txt_text_output.FontSize = 25;
                if (txt_text_output.Text.Length > 100)
                    txt_text_output.FontSize = 20;
            }
        }

        private void txt_text_input_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (txt_text_input.Text.Length < 100)
                txt_text_input.FontSize = 25;
            if (txt_text_input.Text.Length > 100)
                txt_text_input.FontSize = 20;
            if (txt_text_input.Text.Length >= 160)
            {
                Prompter.Show("Text must less than 160 characters");
                txt_text_input.Text = "";
            }
               
        }
    }
}
