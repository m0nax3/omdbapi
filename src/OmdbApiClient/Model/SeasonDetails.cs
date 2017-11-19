using System.Collections.Generic;

namespace OmdbApi.Model
{
    public class SeasonDetails
    {
        public string Title { get; set; }
        public string seriesID { get; set; }
        public string Season { get; set; }
        public string totalSeasons { get; set; }
        public List<EpisodeDetails> Episodes { get; set; }
        public string Response { get; set; }
    }
}