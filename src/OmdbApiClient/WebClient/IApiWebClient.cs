using System;

namespace OmdbApi.WebClient
{
    public interface IApiWebClient : IDisposable
    {
        string DownloadString(string url);
    }
}