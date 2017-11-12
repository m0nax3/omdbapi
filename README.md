# OmdbApi

A C# API wrapper for <a href="http://omdbapi.com/" target="_blank">omdbapi.com</a>, a free web service to obtain movie information.
Written for netstandard2.0 / net core 2

Example:

<pre>            OmdbApiClient c = new OmdbApiClient(false, "mykey");*//or no key for free access
            var apiAnswer = c.GetItemByID("tt1396484");
</pre>

Support few other methods like search, get episodes, seasons etc