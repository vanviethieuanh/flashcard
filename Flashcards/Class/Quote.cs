using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;

namespace Flashcards.Class
{
    public class Quote
    {
        private string qt;
        private string an;

        public string Qt { get => qt; set => qt = value; }
        public string An { get => an; set => an = value; }

        public static List<Quote> GetTopicQuote(string topic)
        {
            topic = topic.ToLower();
            List<Quote> result = new List<Quote>();

            Encoding enc = new UTF8Encoding();

            string html;
            Regex re_Quote = new Regex(@"""qt"":""(?<qt>.*?)""|""an"":""(?<an>.*?)""");

            html = HTML.GetHTMLForQuotes(topic);

            Regex regex_json = new Regex(@"<script type=""text/javascript"">(?<quote>.*?)</script>", RegexOptions.IgnoreCase | RegexOptions.CultureInvariant | RegexOptions.Multiline | RegexOptions.Singleline);
            MatchCollection matchCollection = regex_json.Matches(html);
            Match logest = matchCollection[0];
            foreach (Match m in matchCollection)
            {
                if (logest.ToString().Length < m.ToString().Length)
                    logest = m;
            }

            Regex regex_splitData = new Regex(@"{(?<data>.*?)}", RegexOptions.IgnoreCase | RegexOptions.CultureInvariant | RegexOptions.Multiline | RegexOptions.Singleline);
            MatchCollection matchCollection_data = regex_splitData.Matches(logest.Groups["quote"].ToString());
            foreach (Match match in matchCollection_data)
            {
                MatchCollection matchCollection_quote = re_Quote.Matches(match.Groups["data"].ToString());
                for (int i = 0; i < matchCollection_quote.Count; i += 2)
                {
                    Quote q = new Quote();

                    Match m1 = matchCollection_quote[i];
                    Match m2 = matchCollection_quote[i + 1];
                    if (m1.Groups["qt"].Success)
                    {
                        q.Qt = m1.Groups["qt"].ToString();
                        q.An = m2.Groups["an"].ToString();
                    }
                    else
                    {
                        q.Qt = m2.Groups["qt"].ToString();
                        q.An = m1.Groups["an"].ToString();
                    }

                    q.Qt = Regex.Replace(q.Qt, @"\\u0027", "'");
                    q.Qt = Regex.Replace(q.Qt, @"\\u0022", @"""");
                    q.Qt = Regex.Replace(q.Qt, @"\\u0021", "!");
                    q.Qt = Regex.Replace(q.Qt, @"\\u003B", ";");

                    result.Add(q);
                }
            }
            return result;
        }
    }
}
