using xNet;

namespace Flashcards.Class
{
    public static class HTML
    {
        public static string GetHTMLForTranslate(string word)
        {
            HttpRequest httpClient = new HttpRequest();
            httpClient.Cookies = new CookieDictionary();

            string address = string.Format("http://dictionary.cambridge.org/search/english-vietnamese/direct/?q={0}", word);
            
            return httpClient.Get(address, null).ToString();
        }

        public static string GetHTMLForSearch(string word)
        {
            HttpRequest httpClient = new HttpRequest();
            httpClient.Cookies = new CookieDictionary();

            string address = string.Format("http://dictionary.cambridge.org/dictionary/english/{0}", word);

            return httpClient.Get(address, null).ToString();
        }

        public static string GetHTMLForQuotes(string topic)
        {
            HttpRequest httpClient = new HttpRequest();
            httpClient.Cookies = new CookieDictionary();

            string address = string.Format("https://www.brainyquote.com/quotes/topics/topic_{0}.html", topic);

            return httpClient.Get(address, null).ToString();
        }
    }
}
