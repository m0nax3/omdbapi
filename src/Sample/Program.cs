using System;
using System.Diagnostics;
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

            Debug.Assert(goodAnswer != null);

            Console.WriteLine("OK:" + goodAnswer.Title);

            try
            {
                var incorrectimdbId = api.GetItemByID("tt111161");
            }
            catch (OmdbAPiException e)
            {
                Console.WriteLine(e.Message);
            }

            var noData = api.GetItemByID("tt9999999");

            Debug.Assert(noData == null);

            Console.ReadKey();
        }
    }
}
