using System.Collections.Generic;
using System.Text;
using System.Windows.Controls;
using System.Linq;
using System.Text.RegularExpressions;
using System;

namespace Flashcards.Class
{
    public static class StringProcessing
    {
        public static string FirstCharacter(this string word)
        {
            word = word[0].ToString().ToUpper();
            return word;
        }
        
        public static string StandardizedStringForCambridge(TextBox t)
        {
            string s = t.Text;
            StringBuilder result = new StringBuilder(s.ToString());
            if (s == "")
            {

            }
            int n = s.Length;
            for (int i = 0; i < n; i++)
            {
                if (char.IsWhiteSpace(s[i]))
                    result[i] = '-';
            }
            return result.ToString();
        }

        public static IEnumerable<IGrouping<string, string>> GroupWord(List<string> input)
        {
            input.RemoveAll(s => s == "" | string.IsNullOrWhiteSpace(s));
            return input.GroupBy(x => x.ElementAt(0).ToString().ToUpper());
        }

        public static IEnumerable<string> RemoveDuplication(List<string> input)
        {
            input.RemoveAll(s => s == "" | string.IsNullOrWhiteSpace(s));
            return input.GroupBy(word => word).Select(group => group.Key);
        }

        public static string Encode(this string s)
        {
            byte[] bytes = Encoding.Default.GetBytes(s);
            string result = Encoding.UTF8.GetString(bytes);

            return result;
        }

        public static string EncodeTransform(this string input)
        {
            Regex RxCode = new Regex(@"&#([0-9]+);");
            string lineNew = RxCode.Replace(
                input,
                delegate (Match match) {
                    return "" + (char)Convert.ToInt32(match.Groups[1].Value);
                }
            );
            return lineNew;
        }

        public static string ToURL(this string s)
        {
            return System.Web.HttpUtility.UrlEncode(s);
        }
    }
}
