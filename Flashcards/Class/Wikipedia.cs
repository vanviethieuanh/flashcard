using System.Collections.Generic;
using System.Net;
using System.Text.RegularExpressions;

namespace Flashcards.Class
{
    public static class Wikipedia
    {
        public static List<Section> GetWiki(this string keyword)
        {
            string address = string.Format("https://en.wikipedia.org/wiki/Special:Search?search={0}",
               keyword.ToURL());

            WebClient wc = new WebClient();
            string html = wc.DownloadString(address);
            List<Section> ls = new List<Section>();

            MatchCollection matches = Regex.Matches(html, @"<p>(?<content>.*?)</p>|<h2>(?<header>.*?)</h2>|<h3>(?<Sheader>.*?)</h3>");
            Section s = new Section();
            Content c = new Content();
            foreach (Match match in matches)
            {
                if (match.Groups["header"].Success && match.Groups["header"].ToString().Length > 6)
                {
                    s.Contents.Add(c);
                    c = new Content();
                    ls.Add(s);
                    s = new Section();
                    string header = Regex.Replace(match.Groups["header"].ToString(), "<.*?>|</.*?>", "");
                    header = header.Remove(header.Length - 6, 6);
                    s.Header = StringProcessing.Encode(header.Encode().EncodeTransform());
                }
                if (match.Groups["content"].Success)
                {
                    string cont = match.Groups["content"].ToString();
                    cont = Regex.Replace(cont, "<.*?>|</.*?>", "");
                    c.Cont.Append("   ").Append(cont.Trim().EncodeTransform().Encode()).AppendLine();
                }
                if (match.Groups["Sheader"].Success)
                {
                    s.Contents.Add(c);
                    c = new Content();
                    string header = match.Groups["Sheader"].ToString();
                    header = Regex.Replace(header, "<.*?>|</.*?>", "");
                    header = header.Remove(header.Length - 6, 6);
                    c.Header = header.EncodeTransform().Encode();
                }
            }
            return ls;
        }
    }
}
