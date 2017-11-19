# OmdbApi

A C# API wrapper for <a href="http://omdbapi.com/" target="_blank">omdbapi.com</a>, a <s>free</s> web service to obtain movie information.
Written for netstandard2.0 / net core 2

Example:

<pre>                 
var api = new OmdbApiClient("you_key");
var goodAnswer = api.GetItemByID("tt0111161");
</pre>

Support few other methods like search, get episodes, seasons etc
