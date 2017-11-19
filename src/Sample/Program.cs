using System;
using OmdbApi;

namespace Sample
{
    class Program
    {
        static void Main(string[] args)
        {
            //http://www.omdbapi.com/apikey.aspx

            var api = new OmdbApiClient("");

            var goodAnswer = api.GetItemByID("tt0111161");

            Console.WriteLine(goodAnswer.Title);

            try
            {
                var incorrectimdbId = api.GetItemByID("tt111161");
            }
            catch (OmdbAPiException e)
            {
                Console.WriteLine(e);
            }

            Console.ReadKey();
        }
    }
}
