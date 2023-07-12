using System.Windows;
using System.Windows.Controls;
using Flashcards.Class;
using System.Windows.Media.Animation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Media;

namespace Flashcards.UC
{
    /// <summary>
    /// Interaction logic for UCques.xaml
    /// </summary>
    public partial class UCques : UserControl
    {
        public Question.result Result;
        int selectedIndex;

        private DoubleAnimation anime_time = new DoubleAnimation()
        {
            To = 1,
            From = 0,
            Duration = Constant.timeforaQues            
        };

        private Question question;
        public Question Question
        {
            get => question;
            set {
                Random r = new Random();
                question = value;
                DataContext = Question;
                List<string> ans = Question.Answers.ToList();
                foreach (Button button in grid_answer.Children)
                {
                    int index = r.Next(0, ans.Count - 1);
                    button.Content = ans[index];
                    ans.RemoveAt(index);
                }
                scaler_timeMark.BeginAnimation(ScaleTransform.ScaleXProperty, anime_time);
            } 
        }

        public int SelectedIndex { get => selectedIndex; set => selectedIndex = value; }

        public UCques()
        {
            InitializeComponent();
            anime_time.Completed += Anime_time_Completed;
        }

        private void Anime_time_Completed(object sender, EventArgs e)
        {
            if (rect_timeMark.ActualWidth == border_time.ActualWidth)
            {
                Result = Question.result.TimeOver;
                TimeOver(this, e);
            }
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            Button thisBtn = sender as Button;
            if (thisBtn.Content.ToString() == Question.Answers[0])
                Result = Question.result.Correct;
            else 
            {
                Result = Question.result.Incorrect;
                SelectedIndex = Question.Answers.IndexOf(thisBtn.Content.ToString());
            }
            Selected(this, e);
        }

        public event EventHandler TimeOver;
        public event EventHandler Selected;
    }
}
