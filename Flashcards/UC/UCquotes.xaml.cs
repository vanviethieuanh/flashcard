using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using Flashcards.Class;

namespace Flashcards.UC
{
    /// <summary>
    /// Interaction logic for UCquotes.xaml
    /// </summary>
    public partial class UCquotes : UserControl
    {
        int indexCurrentQuote;
        List<Quote> quotes;

        public int IndexCurrentQuote { get => indexCurrentQuote;}
        public List<Quote> Quotes { get => quotes;
            set {
                quotes = value;
                DataContext = Quotes[0];
                indexCurrentQuote = 0;
                foreach (var item in Quotes)
                {
                    list_Quote.Items.Add(new ListViewItem());
                }
                list_Quote.SelectedIndex = 0;
            }
        }

        public UCquotes()
        {
            InitializeComponent();
        }

        private void btn_Previous_Click(object sender, RoutedEventArgs e)
        {
            if (indexCurrentQuote > 0)
            {
                indexCurrentQuote--;
                list_Quote.SelectedIndex = IndexCurrentQuote;
                DataContext = Quotes[IndexCurrentQuote];
            }
        }

        private void btn_Next_Click(object sender, RoutedEventArgs e)
        {
            if (indexCurrentQuote < Quotes.Count - 1)
            {
                indexCurrentQuote++;
                list_Quote.SelectedIndex = IndexCurrentQuote;
                DataContext = Quotes[IndexCurrentQuote];
            }
        }

        private void list_Quote_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            indexCurrentQuote = list_Quote.SelectedIndex;
            DataContext = Quotes[IndexCurrentQuote];
        }
    }
}
