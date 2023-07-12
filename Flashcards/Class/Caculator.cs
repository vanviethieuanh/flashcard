using System;
using System.Collections.Generic;
using System.Linq;

namespace Flashcards.Class
{
    public static class Caculator
    {
        public static List<double> convertTOpercent(List<double> list)
        {
            List<double> result = new List<double>();
            double max = list.Max();
            if (max == 0)
            {
                return list;
            }
            else
            {
                for (int i = 0; i < list.Count; i++)
                {
                    double a = (list[i] * 100) / max;
                    result.Add(a);
                }
                return result;
            }
        }

        public static void SortDecending(this int[] array)
        {
            Array.Sort(array,
                           new Comparison<int>(
                                   (i1, i2) => i2.CompareTo(i1)
                           ));
        }
    }
}
