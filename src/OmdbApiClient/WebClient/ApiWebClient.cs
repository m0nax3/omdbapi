using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;

namespace OmdbApi.WebClient
{
    internal class ApiWebClient : IApiWebClient
    {
        readonly HttpClient _client;

        public ApiWebClient()
        {
            _client = new HttpClient(new HttpClientHandler(){AutomaticDecompression = DecompressionMethods.GZip | DecompressionMethods.Deflate});

            _client.DefaultRequestHeaders.Accept.Clear();
            _client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
            _client.DefaultRequestHeaders.UserAgent.ParseAdd("Mozilla/5.0 (Bot;admin@torrent-sensor.org)");
            _client.DefaultRequestHeaders.AcceptEncoding.ParseAdd("gzip, deflate");
        }

        public string DownloadString(string url)
        {
            var tsk = _client.GetStringAsync(url);
            tsk.Wait();
            return tsk.Result;
        }

        public void Dispose()
        {
            _client?.Dispose();
        }
    }
}