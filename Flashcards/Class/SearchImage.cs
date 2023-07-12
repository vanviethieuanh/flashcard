using System.Net;
using System.Text.RegularExpressions;

namespace Flashcards.Class
{
    public static class SearchImage
    {
        public static string[] GoogleSrearchImages(string word, int limit)
        {

            WebClient web = new WebClient();
            string[] aUrls = new string[limit];
            string htmlUrl = string.Format("https://www.google.com/search?q={0}&tbm=isch&gws_rd=ssl",word);
            int n = 0;
            string html = web.DownloadString(htmlUrl);
            string pattern = @"<img.*?src=""(?<url>.*?)"".*?>";
            Regex rx = new Regex(pattern);
            foreach (Match m in rx.Matches(html))
            {
                aUrls[n] = (m.Groups["url"].Value);
                n++;
                if (n > limit - 2)
                { break; }
            }
            return aUrls;
        }
    }
}
