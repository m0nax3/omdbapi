using System;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace OMDbSharp
{
    /// <summary>
    /// Client for http://www.omdbapi.com
    /// </summary>
    public class OmdbApiClient
    {
        private readonly string _apiKey;
        private const string OmdbUrl = "http://www.omdbapi.com/?"; // Base OMDb API URL
        private readonly bool _rottenTomatoesRatings = false;
        private readonly IApiWebClient _apiWebClient;
        private static readonly JsonSerializer Serializer = new JsonSerializer();

        public OmdbApiClient(bool rottenTomatoesRatings, string apiKey) 
            : this(rottenTomatoesRatings, (IApiWebClient)null)
        {
            _apiKey = apiKey;
        }

        public OmdbApiClient(bool rottenTomatoesRatings, IApiWebClient apiWebClient)
        {
            this._rottenTomatoesRatings = rottenTomatoesRatings;

            if (apiWebClient == null)
                apiWebClient = new ApiWebClient();

            _apiWebClient = apiWebClient;
        }

        private T Request<T>(string query)
        {
            var requestUrl = OmdbUrl + query + "&tomatoes=" + _rottenTomatoesRatings;

            if (!string.IsNullOrWhiteSpace(_apiKey))
                requestUrl += "&apikey=" + _apiKey;

            return GetResponse<T>(requestUrl);
        }

        private T GetResponse<T>(string requestUrl)
        {
            if (requestUrl == null) throw new ArgumentNullException(nameof(requestUrl));

            var json = GetRawJsonResponce(requestUrl);

            JToken result;
            if (TryGetResult(json, out result))
            {
                var reader = result.CreateReader();
                return Serializer.Deserialize<T>(reader);
            }

            return default(T);
        }
        private string GetRawJsonResponce(string url)
        {
            return _apiWebClient.DownloadString(url);
        }

        private bool TryGetResult(string json, out JToken result)
        {
            result = null;

            try
            {
                if (string.IsNullOrWhiteSpace(json))
                    return false;

                var jObject = JObject.Parse(json);

                if (jObject.HasValues)
                {
                    JToken ok;
                    if (jObject.TryGetValue("Response", out ok))
                    {
                        result = jObject;
                        return true;
                    }
                }
            }
            catch
            {
                return false;
            }
            return false;
        }


        public Item GetItemByTitle(string title)
        {
            Item item =  Request<Item>("t=" + title);
            return item;
        }

        public Item GetItemByID(string id)
        {
            ThrowIfInvalidId(id);
            Item item =  Request<Item>("i=" + id);
            return item;
        }

        public ItemList GetItemList(string query)
        {
            ItemList itemList =  Request<ItemList>("s=" + query);
            return itemList;
        }

        public Season GetSeriesSeason(string id, int season)
        {
            ThrowIfInvalidId(id);
            Season seriesSeason =  Request<Season>("i=" + id + "&Season=" + season);
            return seriesSeason;
        }

        public SeasonDetails GetSeriesSeasonDetails(string id, int season)
        {
            ThrowIfInvalidId(id);
            SeasonDetails seasonDetails =  Request<SeasonDetails>("i=" + id + "&Season=" + season + "&detail=full");
            return seasonDetails;
        }

        public Episode GetSeriesEpisode(string id, int season, int episode)
        {
            ThrowIfInvalidId(id);
            Episode seriesEpisode =  Request<Episode>("i=" + id + "&Season=" + season + "&Episode=" + episode);
            return seriesEpisode;
        }

        public EpisodeDetails GetSeriesEpisodeDetails(string id, int season, int episode)
        {
            ThrowIfInvalidId(id);
            EpisodeDetails episodeDetails =  Request<EpisodeDetails>("i=" + id + "&Season=" + season + "&Episode=" + episode + "&detail=full");
            return episodeDetails;
        }

        private static void ThrowIfInvalidId(string id)
        {
            if (id == null) throw new ArgumentNullException(nameof(id));
            if (!id.StartsWith("tt")) throw new ArgumentException("Wrong id, must begin with 'tt'");
        }
    }

    public interface IApiWebClient
    {
        string DownloadString(string url);
    }

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
    }
}