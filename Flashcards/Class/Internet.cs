using System.Net;

namespace Flashcards.Class
{
    public static class Internet
    {
        public static bool CheckInternetConection()
        {
            try
            {
                using (var client = new WebClient())
                {
                    using (var stream = client.OpenRead("http://www.google.com"))
                    {
                        return true;
                    }
                }
            }
            catch
            {
                return false;
            }

        }
    }
}
