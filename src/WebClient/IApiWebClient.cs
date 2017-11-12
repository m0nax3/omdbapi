namespace OMDbSharp
{
    public interface IApiWebClient
    {
        string DownloadString(string url);
    }
}