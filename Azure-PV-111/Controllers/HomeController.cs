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

        public ViewResult Maps()
        {
            return View();
        }
        private static String filesPath => (System.IO.Directory.Exists(@"C:\home\site\"))
                    ? @"C:\home\site\Files\" : "./Files/";

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
            ViewData["data"] = DataMiddleware.GetData();
            return View();
        }

        public async Task<ViewResult> Db()
        {
            return View();
        }

        public ViewResult Files()
        {
            String filename;
            if (System.IO.Directory.Exists(@"C:\home\site\"))
            {
                filename = @"C:\home\site\";
            }
            else
            {
                filename = "./";
            }
            filename += "Files/";

            String[] files = System.IO.Directory.GetFiles(filename);

            for (int i = 0; i < files.Length; i++)
            {
                files[i] = Path.GetFileName(files[i]);
            }
            ViewData["files"] = files;

            /*
         if (System.IO.Directory.Exists(filename))
         {
             ViewData["dir-exists"] = "Exists";
         }
         else
         {
             ViewData["dir-exists"] = "Not Exists ";
             try
             {
                 System.IO.Directory.CreateDirectory(filename);
                 ViewData["dir-exists"] += "Created";
             }
             catch (Exception ex)
             {
                 ViewData["dir-exists"] += ex.Message;
             }
         }

         filename += "file.txt";

         if (System.IO.File.Exists(filename))
         {
             ViewData["exists"] = "Exists. " +
                System.IO.File.ReadAllText(filename);
         }
         else
         {
             ViewData["exists"] = "Not Exists. ";
             try
             {
                 System.IO.File.WriteAllText(filename, "Test Line 2");
                 ViewData["exists"] += "Created";
             }
             catch (Exception ex)
             {
                 ViewData["exists"] += ex.Message;
             }
         }
         */
            if (HttpContext.Session.Keys.Contains("file-message"))
            {
                ViewData["file-message"] = HttpContext.Session.GetString("file-message");
                HttpContext.Session.Remove("file-message");
            }
            return View();
        }


        public IActionResult FileDownloader(String filename)
        {
            String file = filesPath + filename;
            if (System.IO.File.Exists(file))
            {
                return File(
                        System.IO.File.ReadAllBytes(file),
                        System.Net.Mime.MediaTypeNames.Application.Octet,
                        filename
                    );
            }
            return NotFound();
        }
        public IActionResult FileDeleter(String filename)
        {
            String file = filesPath + filename;
            if (System.IO.File.Exists(file))
            {
                System.IO.File.Delete(file);
                HttpContext.Session.SetString("file-message", "Successfuly deleted");
                return RedirectToAction(nameof(Files));
            }
            return NotFound();
        }
        [HttpPost]
        public RedirectToActionResult FileUploader(IFormFile uploaded)
        {
            if (uploaded != null && uploaded.Length > 0)
            {
                using Stream stream = System.IO.File.OpenWrite(filesPath + uploaded.FileName);
                uploaded.CopyTo(stream);
                HttpContext.Session.SetString("file-message", "Successfuly updated");
            }
            else HttpContext.Session.SetString("file-message", "Error while updating");

            return RedirectToAction(nameof(Files));
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
            DataMiddleware.Add("privacy visited");
            String filename = "./privacy.txt";
            if (System.IO.File.Exists(filename))
            {
                System.IO.File.AppendAllText(
                    filename,
                    $"\n{DateTime.Now}");
            }
            else
            {
                System.IO.File.WriteAllText(
                                    filename,
                                    $"{DateTime.Now}");
            }
            List<DateTime> dates = new();
            foreach (String line in System.IO.File.ReadAllLines(filename))
            {
                dates.Add(DateTime.Parse(line));
            }
            ViewData["dates"] = dates;
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}