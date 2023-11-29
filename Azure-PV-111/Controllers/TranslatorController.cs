using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Text.Json;
using System.Text;

namespace Azure_PV_111.Controllers
{
    [Route("api/translate")]
    [ApiController]
    public class TranslatorController : ControllerBase
    {
        private readonly IConfiguration _configuration;
        private readonly ILogger<TranslatorController> _logger;

        public TranslatorController(IConfiguration configuration, ILogger<TranslatorController> logger)
        {
            _configuration = configuration;
            _logger = logger;
        }

        [HttpGet]
        public async Task<object> GetAsync([FromQuery]String text, [FromQuery]String from, [FromQuery]String to)
        {


            String? endpoint = _configuration.GetSection("Translator").GetSection("Endpoint").Value;
            String? key = _configuration.GetSection("Translator").GetSection("Key").Value;
            String? location = _configuration.GetSection("Translator").GetSection("Location").Value;
            if (endpoint != null && key != null && location != null)
            {
                endpoint += $"/translate?api-version=3.0&to={to}";
                if (from != "auto")
                {
                    endpoint += $"&from={from}";
                }
                _logger.LogInformation("GetAsync request: {endpoint}", endpoint);
                object[] body = new object[] { new { Text = text } };
                var requestBody = JsonSerializer.Serialize(body);



                using var client = new HttpClient();
                using var request = new HttpRequestMessage();

                // Build the request.
                request.Method = HttpMethod.Post;
                request.RequestUri = new Uri(endpoint);
                request.Content = new StringContent(requestBody, Encoding.UTF8, "application/json");
                request.Headers.Add("Ocp-Apim-Subscription-Key", key);
                // location required if you're using a multi-service or regional (not global) resource.
                request.Headers.Add("Ocp-Apim-Subscription-Region", location);



                // Send the request and get response.
                HttpResponseMessage response = await client.SendAsync(request).ConfigureAwait(false);
                // Read response as a string.
                string result = await response.Content.ReadAsStringAsync();
                return result;

            }
            else return new { Status = "Error" };
        }

        [HttpPost]
        public async Task<object> PostAsync([FromQuery] String text, [FromQuery] String from, [FromQuery] String fromScript, [FromQuery] String toScript)
        {

            String? endpoint = _configuration.GetSection("Translator").GetSection("Endpoint").Value;
            String? key = _configuration.GetSection("Translator").GetSection("Key").Value;
            String? location = _configuration.GetSection("Translator").GetSection("Location").Value;
            
            if (endpoint != null && key != null && location != null)
            {
                endpoint += $"/transliterate?api-version=3.0&language={from}&fromScript={fromScript}&toScript={toScript}";
                _logger.LogInformation("PostAsync request: {endpoint}", endpoint);
                object[] body = new object[] { new { Text = text } };
                var requestBody = JsonSerializer.Serialize(body);



                using var client = new HttpClient();
                using var request = new HttpRequestMessage();

                // Build the request.
                request.Method = HttpMethod.Post;
                request.RequestUri = new Uri(endpoint);
                request.Content = new StringContent(requestBody, Encoding.UTF8, "application/json");
                request.Headers.Add("Ocp-Apim-Subscription-Key", key);
                // location required if you're using a multi-service or regional (not global) resource.
                request.Headers.Add("Ocp-Apim-Subscription-Region", location);



                // Send the request and get response.
                HttpResponseMessage response = await client.SendAsync(request).ConfigureAwait(false);
                // Read response as a string.
                string result = await response.Content.ReadAsStringAsync();
                return result;

            }
            else return new { Status = "Error" };
        }
    }
}
