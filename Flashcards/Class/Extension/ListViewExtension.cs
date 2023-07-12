using System.Collections.Generic;
using System.Windows.Controls;

namespace Flashcards.Class.Extension
{
    public static class ListViewExtension
    {
        public static List<int> SelectedIndices(this ListView lv)
        {
            List<int> result = new List<int>();

            foreach (var item in lv.SelectedItems)
                result.Add(lv.Items.IndexOf(item));

            return result;
        }
    }
}
