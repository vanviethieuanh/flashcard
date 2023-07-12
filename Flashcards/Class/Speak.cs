using System.Text.RegularExpressions;
using System.Windows.Controls;

namespace Flashcards.Class
{
    public static class Speak
    {

        public enum pronSpeakMode
        {
            UK, US
        }

        /// <summary>
        /// get pronunciation of a word from html \\ st element is uk,nd element is us
        /// </summary>
        /// <param name="html"></param>
        /// <returns></returns>
        public static string[] Getpron(string html)
        {
            string[] result = new string[2];

            Regex reUS = new Regex(@"<span pron-region=""US"".+?<span class=""ipa"">(?<pron>.+?)</span></span>", RegexOptions.IgnoreCase | RegexOptions.CultureInvariant | RegexOptions.Multiline | RegexOptions.Singleline);
            Regex reUK = new Regex(@"<span pron-region=""US"".+?<span class=""ipa"">(?<pron>.+?)</span></span>", RegexOptions.IgnoreCase | RegexOptions.CultureInvariant | RegexOptions.Multiline | RegexOptions.Singleline);

            Match matchUK = reUK.Match(html);
            Match matchUS = reUS.Match(html);

            result[0] = matchUK.Groups["pron"].ToString();
            result[0] = Regex.Replace(result[0], @"<(.+?)>|/", "");
            result[0] = result[0].Replace(".", string.Empty);

            result[1] = matchUS.Groups["pron"].ToString();
            result[1] = Regex.Replace(result[1], @"<(.+?)>|/", "");
            result[1] = result[1].Replace(".", string.Empty);

            return result;
        }

        /// <summary>
        /// get link sound speak of a word from html \\ st element is uk,nd element is us
        /// </summary>
        /// <param name="word">the word that you wanna get link</param>
        /// <returns></returns>
        public static string[] GetlinkSpeak(string html)
        {
            string[] result = new string[2];

            bool exitsUS = false;
            bool exitsUK = false;

            Regex re = new Regex(@"data-src-mp3=""(?<pron>.+?)""");

            foreach (Match m in re.Matches(html))
            {
                string link = (m.Groups["pron"].ToString());
                if (link.Contains("uk_pron") && !exitsUK)
                {
                    result[0] = link;
                    exitsUK = true;
                }
                if (link.Contains("us_pron") && !exitsUS)
                {
                    result[1] = link;
                    exitsUS = true;
                }
            }
            return result;
        }

        /// <summary>
        /// start speak
        /// </summary>
        /// <param name="link">the link get from linkSpeak method</param>
        /// <param name="mode">us or uk</param>
        /// <param name="media">media element which speaker</param>
        public static void StartSpeak(string[] link, pronSpeakMode mode, MediaElement media)
        {
            if (mode == pronSpeakMode.UK)
                media.Source = new System.Uri(link[0]);

            if (mode == pronSpeakMode.US)
                media.Source = new System.Uri(link[1]);

            media.Play();
        }
    }
}
