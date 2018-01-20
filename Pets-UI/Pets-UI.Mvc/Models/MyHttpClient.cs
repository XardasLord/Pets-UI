using System.Net.Http;

namespace Pets_UI.Mvc.Models
{
    public class MyHttpClient
    {
        private static HttpClient _instance = null;
        private static readonly object obj = new object();

        private MyHttpClient()
        {
        }

        public static HttpClient GetInstance(HttpClientHandler handler = null)
        {
            if (_instance == null || handler != null)
            {
                lock (obj)
                {
                    _instance = new HttpClient(handler);
                }
            }

            return _instance;
        }
    }
}