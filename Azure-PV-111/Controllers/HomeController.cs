using Azure_PV_111.Middleware;
using Azure_PV_111.Models;
using Azure_PV_111.Models.Home.Db;
using Azure_PV_111.Models.Home.ImageSearch;
using Azure_PV_111.Models.Home.Search;
using Azure_PV_111.Models.Home.SpellCheck;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.Cosmos;
using System.Diagnostics;
using System.Reflection;
using System.Text.Json;
using System.Text.Json.Nodes;
using System.Text.Json.Serialization;

namespace Azure_PV_111.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly IConfiguration _configuration;

        public HomeController(ILogger<HomeController> logger, IConfiguration configuration)
        {
            _logger = logger;
            _configuration = configuration;
        }

        public IActionResult Index()
        {
            ViewData["config"] = _configuration.GetSection("Search").GetSection("Endpoint").Value;
            return View();
        }

        public ViewResult Translator()
        {
            using HttpClient client = new HttpClient();
            ViewData["Langs"] = JsonSerializer.Deserialize<JsonNode>(
                client.GetStringAsync("https://api.cognitive.microsofttranslator.com/languages?api-version=3.0")
                .Result);
            return View();
        }

        public ViewResult Data()
        {
            ViewData["data"] = DataMiddleware.Data;
            return View();
        }

        public async Task<ViewResult> Db()
        {
            String? endpoint = _configuration.GetSection("CosmosDb").GetSection("Endpoint").Value;
            String? key = _configuration.GetSection("CosmosDb").GetSection("Key").Value;
            String? databaseId = _configuration.GetSection("CosmosDb").GetSection("DatabaseId").Value;
            String? containerId = _configuration.GetSection("CosmosDb").GetSection("ContainerId").Value;

            CosmosClient cosmosClient = new(
                endpoint, key, 
                new CosmosClientOptions()
                {
                    ApplicationName = "Azure_PV111"
                });
            
            Database database = await cosmosClient
                .CreateDatabaseIfNotExistsAsync(databaseId);
            
            Container container = await database
                .CreateContainerIfNotExistsAsync(containerId, "/partitionKey");

            int rand = new Random().Next(100);

            Models.Home.Db.Test data = new Models.Home.Db.Test()
            {
                Id = Guid.NewGuid().ToString(),
                PartitionKey = rand.ToString(),
                Data = $"Random {rand}"
            };
            
            ItemResponse<Test> response = await container
                .CreateItemAsync<Test>(data, new PartitionKey(data.PartitionKey));
            
            ViewData["code"] = response.StatusCode;
            
            return View();
        }

        public IActionResult Search(String? search, int? page)
        {
            HomeSearchViewModel model = new HomeSearchViewModel();

            if (!String.IsNullOrEmpty(search))
            {
                page ??= 1;
                int count = 20;
                int offset = (page.Value - 1) * count;

                model.offset = offset;
                model.page = page.Value;

                String? endpoint = _configuration.GetSection("Search").GetSection("Endpoint").Value;
                String? key = _configuration.GetSection("Search").GetSection("Key").Value;
                String? location = _configuration.GetSection("Search").GetSection("Location").Value;
                if (endpoint != null && key != null && location != null)
                {
                    endpoint += $"v7.0/search?mkt=en-En&count={count}&offset={offset}&q=" + Uri.EscapeDataString(search);
                    using HttpClient client = new HttpClient();
                    client.DefaultRequestHeaders.Add("Ocp-Apim-Subscription-Key", key);
                    String content = client.GetStringAsync(endpoint).Result;

                    model.WebSearchResponse =
                        JsonSerializer.Deserialize<WebSearchResponse>(content);
                }
                else
                {
                    model.ErrorMessage = "Config load error";
                }
            }

            return View(model);
        }

        public ViewResult ImageSearch(String? search)
        {
            HomeImageSearchViewModel model = new();
            if (!String.IsNullOrEmpty(search))  // є пошуковий запит
            {
                String? endpoint = _configuration.GetSection("Search").GetSection("Endpoint").Value;
                String? key = _configuration.GetSection("Search").GetSection("Key").Value;
                String? location = _configuration.GetSection("Search").GetSection("Location").Value;
                if (endpoint != null && key != null && location != null)
                {
                    endpoint += $"v7.0/images/search?mkt=uk-UA&count={10}&offset={0}&q=" + Uri.EscapeDataString(search);
                    using HttpClient httpClient = new();
                    httpClient.DefaultRequestHeaders.Add("Ocp-Apim-Subscription-Key", key);
                    String content = httpClient.GetStringAsync(endpoint).Result;

                    model.SearchResponse =
                    JsonSerializer.Deserialize<ImageSearchResponse>(content);

                    model.ErrorMessage = content;

                }
                else
                {
                    model.ErrorMessage = "Config load error";
                }
            }
            return View(model);
        }

        public IActionResult NewsSearch(String? search, int? page)
        {
            //HomeImageSearchViewModel model = new();

            if (!String.IsNullOrEmpty(search))
            {
                page ??= 1;
                int count = 10;
                int offset = (page.Value - 1) * count;

                //model.offset = offset;
                //model.page = page.Value;

                String? endpoint = _configuration.GetSection("Search").GetSection("EndPoint").Value;
                String? key = _configuration.GetSection("Search").GetSection("Key").Value;
                String? location = _configuration.GetSection("Search").GetSection("Location").Value;

                if (endpoint != null && key != null && location != null)
                {
                    endpoint += $"v7.0/news/search?textDecorations=true&textFormat=HTML&q=" + Uri.EscapeDataString(search);
                    using HttpClient httpClient = new();
                    httpClient.DefaultRequestHeaders.Add("Ocp-Apim-Subscription-Key", key);
                    String content = httpClient.GetStringAsync(endpoint).Result;
                    ViewData["content"] = content;
                    ViewData["Search"] = JsonSerializer.Deserialize<JsonNode>(content);

                    //model.SearchResponse = JsonSerializer.Deserialize<ImageSearchResponse>(content);

                }
                else
                {
                    //model.ErrorMessage = "Config load error";
                }
            }
            return View();
        }

        public ViewResult SpellCheck(String? phrase)
        {
            HomeSpellCheckViewModel model = new HomeSpellCheckViewModel();
            if (!String.IsNullOrEmpty(phrase))
            {
                String? endpoint = _configuration.GetSection("Search").GetSection("Endpoint").Value;
                String? key = _configuration.GetSection("Search").GetSection("Key").Value;
                String? location = _configuration.GetSection("Search").GetSection("Location").Value;
                if (endpoint != null && key != null && location != null)
                {
                    endpoint += $"v7.0/spellcheck?mkt=en-us&mode=proof";
                    using HttpClient httpClient = new();
                    httpClient.DefaultRequestHeaders.Add("Ocp-Apim-Subscription-Key", key);
                    var postContent = new StringContent(
                    $"text={phrase}",
                    System.Text.Encoding.UTF8,
                    "application/x-www-form-urlencoded");

                    String content =
                    httpClient
                    .PostAsync(endpoint, postContent)
                    .Result
                    .Content
                    .ReadAsStringAsync()
                    .Result;
                    model.SpellCheckResponse = JsonSerializer.Deserialize<SpellCheckResponse>(content);
                }
                else
                {
                    model.ErrorMessage = "Config Error";
                }
            }
            return View(model);
        }

        public IActionResult Privacy()
        {
            DataMiddleware.Data.Add("privacy visited");
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}