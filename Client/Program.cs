using System;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace NServiceBusIssue
{
    public class Program
    {
        private static readonly HttpClient _httpClient = new HttpClient();

        public static void Main(string[] args)
        {
            MainAsync(args).ConfigureAwait(false).GetAwaiter().GetResult();
        }

        private static async Task MainAsync(string[] args)
        {
            var url = "http://localhost:24512/api";
            var concurrency = 100;

            var jobs = new Task[concurrency];

            Console.WriteLine($"Concurrency: {concurrency}");

            for (var i = 0; i < concurrency; i++)
            {
                jobs[i] = StartRequest(url);
            }

            await Task.WhenAll(jobs).ConfigureAwait(false);
            Console.WriteLine("All done! press enter to quit");
            Console.Read();
        }

        private static async Task StartRequest(string url)
        {
            var request = new HttpRequestMessage
            {
                Method = HttpMethod.Post,
                RequestUri = new Uri(url),
                Content = new StringContent("", Encoding.UTF8)
            };

            var response = await _httpClient.SendAsync(request).ConfigureAwait(false);
            if (!response.IsSuccessStatusCode)
            {
                Console.WriteLine($"Request to {url} failed: {response.StatusCode}");
                Console.WriteLine(await response.Content.ReadAsStringAsync().ConfigureAwait(false));
            }
        }
    }
}
