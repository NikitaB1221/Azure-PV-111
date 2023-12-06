using Azure_PV_111.Models.Home.Db;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.Cosmos;

namespace Azure_PV_111.Controllers
{
    [Route("api/db")]
    [ApiController]
    public class DbController : ControllerBase
    {
        private readonly IConfiguration _configuration;
        private static Container? _container;

        public DbController(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        [HttpPost]
        public async Task<object> AddProducer(ProducerFormModel formModel)
        {
            Container dbContainer = await GetDbContainer();
            ProducerDataModel data = new()
            {
                Id = Guid.NewGuid(),
                Name = formModel.Name,
            };
            ItemResponse<ProducerDataModel> response =
                await dbContainer.CreateItemAsync<ProducerDataModel>(
                    data,
                    new PartitionKey(data.PartitionKey)
                );
            return response.StatusCode;
        }

        private async Task<Container> GetDbContainer()
        {
            if (_container != null)
            {
                return _container;

            }
            else
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

                Container _container = await database
                    .CreateContainerIfNotExistsAsync(containerId, "/partitionKey");
                return _container;
                /*
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
                */
            }
        }

        [HttpGet]
        public async Task<IEnumerable<object>> GetItemsAsync(String type)
        {
            Container dbContainer = await GetDbContainer();
            QueryDefinition query = new($"SELECT * FROM c WHERE c.type='{type}'");
            FeedIterator<ProducerDataModel> feedIterator = dbContainer.GetItemQueryIterator<ProducerDataModel>(query);
            List<ProducerDataModel> res = new List<ProducerDataModel>();
            while (feedIterator.HasMoreResults)
            {
                FeedResponse<ProducerDataModel> response = await feedIterator.ReadNextAsync();
                foreach (ProducerDataModel item in response)
                {
                    res.Add(item);
                }
            }
            return res;
        }

        [HttpDelete]
        public async Task<object> DeleteProducer( String producerId )
        {
            Container dbContainer = await GetDbContainer();
            ItemResponse<ProducerDataModel> response = await 
                dbContainer.DeleteItemAsync<ProducerDataModel>(producerId, 
                new PartitionKey(ProducerDataModel.DataType));
            return new { status = response.StatusCode };
        }

        [HttpPut]
        public async Task<object> UpdateProducers(String producerId, string newNum)
        {
            return new { producerId, newNum };
        }
    }
}
