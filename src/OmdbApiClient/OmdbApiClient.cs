using System;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using OmdbApi.Model;
using OmdbApi.WebClient;

namespace OmdbApi
{
    /// <inheritdoc />
    /// <summary>
    ///     Client for http://www.omdbapi.com
    /// </summary>
    public class OmdbApiClient : IDisposable
    {
        private const string OmdbUrl = "http://www.omdbapi.com/?"; // Base OMDb API URL

        private static readonly JsonSerializer Serializer = new JsonSerializer();
        private readonly string _apiKey;
        private readonly IApiWebClient _apiWebClient;

        #region Constructors

        public OmdbApiClient() : this(null, null)
        {
        }

        public OmdbApiClient(IApiWebClient apiWebClient) : this(apiWebClient, null)
        {
        }

        public OmdbApiClient(string apiKey) : this(null, apiKey)
        {
        }

        public OmdbApiClient(IApiWebClient apiWebClient, string apiKey)
        {
            _apiWebClient = apiWebClient;
            _apiKey = apiKey;

            if (apiWebClient == null)
                _apiWebClient = new ApiWebClient();
        }

        #endregion

        #region API

        public OmdbItem GetItemByTitle(string title)
        {
            return GetItemByTitle(title, null, false);
        }

        public OmdbItem GetItemByTitle(string title, int? year)
        {
            return GetItemByTitle(title, year, false);
        }

        public OmdbItem GetItemByTitle(string title, bool fullPlot)
        {
            return GetItemByTitle(title, null, fullPlot);
        }

        public OmdbItem GetItemByTitle(string title, int? year, bool fullPlot)
        {
            if (string.IsNullOrWhiteSpace(title)) throw new ArgumentNullException(nameof(title));
            ;
            var query = "t=" + title;

            if (year.HasValue)
                query += "&y=" + year;

            query = Plot(fullPlot, query);

            return Request<OmdbItem>(query);
        }

        public OmdbItem GetItemByID(string id)
        {
            return GetItemByID(id, false);
        }

        public OmdbItem GetItemByID(string id, bool fullPlot)
        {
            var query = Plot(fullPlot, AddId(id));
            return Request<OmdbItem>(query);
        }

        public OmdbInfoList GetItemList(string query)
        {
            if (string.IsNullOrWhiteSpace(query)) throw new ArgumentNullException(nameof(query));
            return Request<OmdbInfoList>("s=" + query);
        }

        public Season GetSeriesSeason(string id, int season)
        {
            return Request<Season>($"{AddId(id)}{'&'}{"Season=" + season}");
        }

        public SeasonDetails GetSeriesSeasonDetails(string id, int season)
        {
            return Request<SeasonDetails>($"{AddId(id)}{'&'}{"Season=" + season}&detail=full");
        }


        public Episode GetSeriesEpisode(string id, int season, int episode)
        {
            return Request<Episode>($"{AddId(id)}{'&'}{"Season=" + season}{'&'}{"Episode=" + episode}");
        }

        public EpisodeDetails GetSeriesEpisodeDetails(string id, int season, int episode)
        {
            return Request<EpisodeDetails>($"{AddId(id)}{'&'}{"Season=" + season}{'&'}{"Episode=" + episode}&detail=full");
        }

        #endregion

        #region Internal

        private string AddId(string id)
        {
            ThrowIfInvalidId(id);
            return "i=" + id;
        }

        private static void ThrowIfInvalidId(string id)
        {
            if (id == null) throw new ArgumentNullException(nameof(id));
            if (!id.StartsWith("tt")) throw new ArgumentException("Wrong id, must begin with 'tt'");
        }

        private static string Plot(bool fullPlot, string query)
        {
            if (fullPlot)
                query += "&plot = full";
            return query;
        }

        private T Request<T>(string query) where T : class
        {
            if (query == null) throw new ArgumentNullException(nameof(query));

            if (query.EndsWith("&"))
                query = query.Substring(0, query.Length - 1);

            var requestUrl = OmdbUrl + query;

            if (!string.IsNullOrWhiteSpace(_apiKey))
                requestUrl += "&apikey=" + _apiKey;

            return GetResponse<T>(requestUrl);
        }

        protected virtual T GetResponse<T>(string requestUrl) where T : class
        {
            if (requestUrl == null) throw new ArgumentNullException(nameof(requestUrl));

            var json = _apiWebClient.DownloadString(requestUrl);

            if (TryGetResult(json, out var result))
            {
                var reader = result.CreateReader();
                return Serializer.Deserialize<T>(reader);
            }

            return default(T);
        }

        protected virtual bool TryGetResult(string json, out JToken result)
        {
            result = null;

            if (string.IsNullOrWhiteSpace(json))
                return false;

            var jObject = JObject.Parse(json);

            if (jObject.HasValues)
            {
                if (jObject.TryGetValue("Response", out var token))
                {
                    if (token.Value<bool>() == false)
                    {
                        if (jObject.TryGetValue("Error", out var tokenError))
                        {
                            if (tokenError.ToString() == "Error getting data.")
                                return false;
                            throw new OmdbAPiException(tokenError.ToString());
                        }
                        else
                        {
                            throw new OmdbAPiException("Unexpected API answer, 'Response' tag is 'False' but no error specified");
                        }
                    }

                    result = jObject;

                    return true;
                }
                else
                {
                    throw new OmdbAPiException("No response tag in API answer");
                }
            }

            return false;
        }


        public void Dispose()
        {
            _apiWebClient.Dispose();
        }

        #endregion
    }

    [Serializable]
    public class OmdbAPiException : Exception
    {
        public OmdbAPiException() { }
        public OmdbAPiException(string message) : base(message) { }
        public OmdbAPiException(string message, Exception inner) : base(message, inner) { }
        protected OmdbAPiException(
          System.Runtime.Serialization.SerializationInfo info,
          System.Runtime.Serialization.StreamingContext context) : base(info, context) { }
    }
}