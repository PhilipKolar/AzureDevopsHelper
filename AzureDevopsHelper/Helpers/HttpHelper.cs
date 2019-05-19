using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using HttpMethod = AzureDevopsHelper.Enums.HttpMethod;

namespace AzureDevopsHelper.Helpers
{
    public class HttpHelper
    {
        private readonly ConfigContainer _config;

        public HttpHelper(ConfigContainer config)
        {
            _config = config;
        }

        public async Task<string> GetResponseAsync(string url, HttpMethod method = HttpMethod.Get, string body = null)
        {
            try
            {
                using (HttpClient client = new HttpClient())
                {
                    client.DefaultRequestHeaders.Accept.Add(
                        new MediaTypeWithQualityHeaderValue("application/json"));

                    client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Basic",
                        Convert.ToBase64String(
                            System.Text.ASCIIEncoding.ASCII.GetBytes(
                                string.Format("{0}:{1}", "", _config.PersonalAccessToken))));

                    switch (method)
                    {
                        case HttpMethod.Get:
                            using (HttpResponseMessage response = client.GetAsync(url).Result)
                            {
                                response.EnsureSuccessStatusCode();
                                return await response.Content.ReadAsStringAsync();
                            }
                        case HttpMethod.Post:
                            using (HttpResponseMessage response = client.PostAsync(url, new StringContent(body, Encoding.UTF8, "application/json")).Result)
                            {
                                response.EnsureSuccessStatusCode();
                                return await response.Content.ReadAsStringAsync();
                            }
                        default:
                            throw new NotSupportedException($"Unknown httpMethod: {method}");
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
                throw;
            }
        }
    }
}
