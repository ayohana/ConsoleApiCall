using System;
using System.Threading.Tasks;
using RestSharp;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.Collections.Generic;

namespace ConsoleApiCall
{
    public class Article
    {
        public string Section { get; set; }
        public string Title { get; set; }
        public string Abstract { get; set; }
        public string Url { get; set; }
        public string Byline { get; set; }
    }
  class Program
  {
    static void Main()
    {
      var apiCallTask = ApiHelper.ApiCall(EnvironmentVariables.ApiKey);
      var result = apiCallTask.Result;

      // Here, we turn the giant string stored as result into JSON data.
      JObject jsonResponse = JsonConvert.DeserializeObject<JObject>(result);
      // converts the JSON-formatted string result into a JObject. The type JObject comes from the Newtonsoft.Json.Linq library and is a .NET object we can treat as JSON.
      
      // Console.WriteLine(jsonResponse["results"]);
      // Now we have access to the data stored in the "results" key. All we have to do is call jsonResponse["results"]. If we run the program again, the response will be nicely formatted and will include everything stored in the "results" key.
      // Notice that each key in the JSON response is in lower snake-case, not the PascalCase commonly used in C#. When retrieving data from a JSON response, always make sure to use the name of the JSON key with the exact casing it appears in the response object.

      List<Article> articleList = JsonConvert.DeserializeObject<List<Article>>(jsonResponse["results"].ToString());
      // We use the DeserializeObject() method to create a list of Articles. The method will automatically grab any JSON keys in our response that match the names of the properties in our class. In order for this to work, the property name has to match the JSON key. This means that the Section property for our message class needs to be named Section. We could not rename it to something like Category because the information is named "section" in the JSON data.

        foreach (Article article in articleList)
        {
            Console.WriteLine($"Section: {article.Section}");
            Console.WriteLine($"Title: {article.Title}");
            Console.WriteLine($"Abstract: {article.Abstract}");
            Console.WriteLine($"Url: {article.Url}");
            Console.WriteLine($"Byline: {article.Byline}");
        }

        // Because the JSON keys must match the object property names, we'll sometimes need to go deeper than depicted in the example above before turning API response data into a C# object. For instance, if the response information we'd like to deserialize is contained in an object that's part of an array which is nested under a specific JSON key, we'd need to programmatically isolate that specific data whose keys match our object properties before converting it into an object. We can't just pass in the whole big response. Fortunately, we can always use Postman to take a closer look at how JSON key-value pairs are nested.
    }
  }

  class ApiHelper
  {
    // We want our API calls to run asynchronously so that the application is responsive and free to run other tasks while the HTTP request/response loop executes. In order to achieve this, we add the async keyword to our method declaration.
    public static async Task<string> ApiCall(string apiKey)
    {
      RestClient client = new RestClient("https://api.nytimes.com/svc/topstories/v2");
      // Note that we use the base URL https://api.nytimes.com/svc/topstories/v2 from the Top Stories API. We instantiate a RestSharp RestClient object and store the connection in a variable called client.

      RestRequest request = new RestRequest($"home.json?api-key={apiKey}", Method.GET);
      // Next, we create a RestRequest object. This is our actual request. We include the path to the endpoint we are looking for (home.json) along with our API key. We also specify that we will be using a GET Http method.
      // Note that we utilize C#'s string interpolation to place the apiKey variable into the RestRequest by placing a $ before a string and then placing any interpolated values in curly braces. This is really similar to how we did template literals in JavaScript.

      var response = await client.ExecuteTaskAsync(request);
      // Then we use the await keyword to specify that we need to receive a result before we attempt to define response. We call the RestClient's ExecuteTaskAsync method and pass in our request object.

      return response.Content;
      // Finally, we return the Content property of the response variable, which is a string representation of the response content.
    }
  }
}
