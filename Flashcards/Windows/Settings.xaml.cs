using Flashcards.Class;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace Flashcards.Windows
{
    /// <summary>
    /// Interaction logic for Settings.xaml
    /// </summary>
    public partial class Settings : Window
    {
        bool fist = true;

        SettingsData data;

        public SettingsData Data { get => data; set => data = value; }

        public Settings(SettingsData setData)
        {
            InitializeComponent();
            PreviewKeyDown += Settings_PreviewKeyDown;

            Data = setData;

            if (setData.Idioms.Count > 0)
                radio_idioms.IsChecked = true;
            if (setData.Quotes.Count > 0)
                radio_quotation.IsChecked = true;

            fist = false;
        }

        private void Settings_PreviewKeyDown(object sender, KeyEventArgs e)
        {
            Close();
        }

        private void grid_Movebar_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if (e.ChangedButton == MouseButton.Left)
                DragMove();
        }

        private void radio_idioms_Checked(object sender, RoutedEventArgs e)
        {
            if (!fist)
            {
                SimpleSelector ss = new SimpleSelector();
                if (ss.Show(Topics.Idioms, "What is your favourite topic", SelectionMode.Multiple))
                {
                    data.Idioms.Clear();
                    foreach (int i in ss.SelectedIndices)
                    {
                        data.Idioms.Add(Topics.Idioms[i]);
                    }
                }
            }
        }

        private void radio_quotation_Checked(object sender, RoutedEventArgs e)
        {
            if (!fist)
            {
                SimpleSelector ss = new SimpleSelector();
                if (ss.Show(Topics.PopularQuotes, "What is your favourite topic", SelectionMode.Multiple, Topics.Quotes))
                {
                    data.Quotes.Clear();
                    if (ss.IsMoreChecked)
                    {
                        if (!ss.SelectedIndices.Contains(-1))
                        {
                            foreach (int i in ss.SelectedIndices)
                            {
                                data.Quotes.Add(Topics.Quotes[i]);
                            }
                        }
                        else {
                            radio_quotation.IsChecked = false;
                        }
                        
                    }
                    else
                    {
                        if (!ss.SelectedIndices.Contains(-1))
                        {
                            foreach (int i in ss.SelectedIndices)
                            {
                                data.Quotes.Add(Topics.PopularQuotes[i]);
                            }
                        }
                        else
                        {
                            radio_quotation.IsChecked = false;
                        }
                    }
                }
            }
        }

        private void btn_Close_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }

        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            if (radio_idioms.IsChecked == false)
                data.Idioms = new List<string>();
            if (radio_quotation.IsChecked == false)
                data.Quotes = new List<string>();

            SettingsData.Save(data);
        }
    }
}
