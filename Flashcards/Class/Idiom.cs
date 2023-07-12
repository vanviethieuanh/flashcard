using System.Collections.Generic;
using System.Net;
using System.Text.RegularExpressions;

namespace Flashcards.Class
{
    public class Idiom
    {
        private string idi;
        private List<string> meaning = new List<string>();
        private List<string> ex = new List<string>();

        public List<string> Meaning { get => meaning; set => meaning = value; }
        public List<string> Ex { get => ex; set => ex = value; }
        public string Idi { get => idi; set => idi = value; }

        public static List<string> GetIdioms(string topic)
        {
            topic = topic.ToLower();
            List<string> result = new List<string>();

            Regex regex = new Regex(@"<dt><a href=""(?<link>.*?)"">(?<idiom>.*?)</a></dt>", RegexOptions.IgnoreCase | RegexOptions.CultureInvariant | RegexOptions.Multiline | RegexOptions.Singleline);
            MatchCollection matchCollection = regex.Matches((new WebClient()).DownloadString(string.Format(@"http://www.theidioms.com/{0}/",topic)));
            foreach (Match match in matchCollection)
            {
                result.Add(match.Groups["link"].ToString());
            }

            return result;
        }

        public static Idiom GetDailyIdiom()
        {
            string result = "";

            Regex regex = new Regex(@"<p class=""daily""><strong><a href=""(?<link>.*?)"">(?<idiom>.*?)</a></strong><br>(?<des>.*?)<a class=""read-more"" href=""(?<link>.*?)"">Read on</a></p>", 
                RegexOptions.IgnoreCase | RegexOptions.CultureInvariant | RegexOptions.Multiline | RegexOptions.Singleline);
            MatchCollection matchCollection = regex.Matches((new WebClient()).DownloadString("http://www.theidioms.com"));
            foreach (Match match in matchCollection)
            {
                result = match.Groups["link"].ToString();
            }
            return GetIdiom(result);
        }

        public static Idiom GetIdiom(string link)
        {
            Idiom result = new Idiom();

            Regex regex_meaning = new Regex(@"<li>(?<meaning>.*?)</li>");
            //Regex regex_exs = new Regex(@"<ol>(?<exs>.*?)</ol>");
            //Regex regex_ex = new Regex(@"<li>(?<ex>.*?)</li>");

            Regex re_meaning1 = new Regex(@"<br><strong>Meaning</strong>(?<meaning>.*?)<br>|<strong>Meanings:</strong><br>(?<meanings>.*?)<br>(?<meanings>.*?)</p>");
            //Regex re_ex1 = new Regex(@"<strong>Example</strong>(?<ex>.*?)</p>");

            Regex regex = new Regex(@"<div class=""blocks main-block"">(.*?)</div>", RegexOptions.IgnoreCase | RegexOptions.CultureInvariant | RegexOptions.Multiline | RegexOptions.Singleline);
            string html = regex.Match((new WebClient()).DownloadString(link)).Groups[1].ToString();

            Regex regex_header = new Regex(@"<h1 class=""page-heading"">(?<header>.*?)</h1>");
            result.Idi = regex_header.Match(html).Groups["header"].ToString();
            result.Idi = StringProcessing.Encode(result.Idi);
            result.Idi = Regex.Replace(result.Idi, "&#8217;","'");

            Regex regex_meanings = new Regex(@"<ul>(?<meanings>.*?)</ul>");
            string meanings = regex_meanings.Match(html).Groups["meanings"].ToString();
            if (meanings.Length > 0)
            {
                foreach (Match m in regex_meaning.Matches(meanings))
                {
                    string meaning = m.Groups["meaning"].ToString();
                    meaning = Regex.Replace(meaning, "&#8217;", "'");
                    result.Meaning.Add(StringProcessing.Encode(meaning));
                }

                #region Get Regex
                //string exs = regex_exs.Match(html).Groups["exs"].ToString();
                //foreach (Match m in regex_ex.Matches(exs))
                //{
                //    string ex = m.Groups["ex"].ToString();
                //    ex = Regex.Replace(ex, "&#8217", "'");
                //    result.ex.Add(Regex.Replace(ex, "<strong>|</strong>", ""));
                //}
                #endregion
            }
            else {

                Match meaning = re_meaning1.Match(html);
                if (meaning.Groups[1].ToString() != "" || meaning.Groups[2].ToString() != "")
                {
                    if (meaning.Groups["meaning"].Success)
                    {
                        string m = meaning.Groups[1].ToString();
                        m = Regex.Replace(m, "&#8217;", "'");
                        result.Meaning.Add(StringProcessing.Encode(m.Remove(0, 2)));
                    }
                    if (meaning.Groups["meanings"].Success)
                    {
                        string m = "";
                        foreach (Capture c in meaning.Groups["meanings"].Captures)
                        {
                            m += c.ToString();
                        }
                        string[] arrS = Regex.Split(m, "&#8211;|;");
                        for (int i = 0; i < arrS.Length - 1; i++)
                        {
                            arrS[i] = Regex.Replace(arrS[i], "<br>", "");
                        }
                        result.Meaning.AddRange(arrS);
                    }
                }

                #region Get Example
                //string ex = re_ex1.Match(html).Groups["ex"].ToString();
                //if (ex != "")
                //{
                //    ex = Regex.Replace(ex, "&#8217;", "'");
                //    ex = Regex.Replace(ex, "<strong>|</strong>", "");
                //    result.Ex.Add(ex.Remove(0, 2));
                //}
                #endregion
            }
            return result;
        }
    }
}
