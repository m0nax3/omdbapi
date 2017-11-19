using System.Collections.Generic;

namespace OmdbApi.Model
{
    public class Season
    {
        public string Title { get; set; }

        public string SeasonNumber { get; set; }

        public List<SeasonEpisode> Episodes { get; set; }

        public string Response { get; set; }
    }
}