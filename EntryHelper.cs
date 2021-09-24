using System;
using System.Threading.Tasks;
using System.Collections.Generic;
using System.Net;
using Microsoft.Azure.Cosmos;
using Microsoft.Extensions.Logging;

namespace Company.Function
{
    public class EntryHelper
    {
        private static readonly string EndpointUri = Environment.GetEnvironmentVariable("EndpointUri");
        private static readonly string PrimaryKey = Environment.GetEnvironmentVariable("PrimaryKey");
        private CosmosClient cosmosClient;
        private Database database;
        private Container container;
        private string databaseId = "Entry";
        private string containerId = "Items";
        private ILogger log;

        private const string _partitionKey = "Entry";

        public EntryHelper(ILogger log)
        {
            this.log = log;
            this.cosmosClient = new CosmosClient(EndpointUri, PrimaryKey, new CosmosClientOptions() { ApplicationName = "CosmosDBDotnetQuickstart" });
            this.database =  this.cosmosClient.GetDatabase(databaseId);
            this.container = this.database.GetContainer(containerId);
            //CreateDatabaseAsync().Wait();
            //CreateContainerAsync().Wait();
        }

        /// <summary>
        /// Create the database if it does not exist
        /// </summary>
        private async Task CreateDatabaseAsync()
        {
            this.database = await this.cosmosClient.CreateDatabaseIfNotExistsAsync(databaseId);
            log.LogInformation("Created Database: {0}\n", this.database.Id);
        }

        /// <summary>
        /// Create the container if it does not exist. 
        /// Specifiy "/partitionKey" as the partition key path since we're storing family information, to ensure good distribution of requests and storage.
        /// </summary>
        /// <returns></returns>
        private async Task CreateContainerAsync()
        {
            this.container = await this.database.CreateContainerIfNotExistsAsync(containerId, "/partitionKey");
            log.LogInformation("Created Container: {0}\n", this.container.Id);
        }

        /// <summary>
        /// Scale the throughput provisioned on an existing Container.
        /// You can scale the throughput (RU/s) of your container up and down to meet the needs of the workload. Learn more: https://aka.ms/cosmos-request-units
        /// </summary>
        /// <returns></returns>
        private async Task ScaleContainerAsync()
        {
            // Read the current throughput
            try
            {
                int? throughput = await this.container.ReadThroughputAsync();
                if (throughput.HasValue)
                {
                    log.LogInformation("Current provisioned throughput : {0}\n", throughput.Value);
                    int newThroughput = throughput.Value + 100;
                    // Update throughput
                    await this.container.ReplaceThroughputAsync(newThroughput);
                    log.LogInformation("New provisioned throughput : {0}\n", newThroughput);
                }
            }
            catch (CosmosException cosmosException) when (cosmosException.StatusCode == HttpStatusCode.BadRequest)
            {
                log.LogInformation("Cannot read container throuthput.");
                log.LogInformation(cosmosException.ResponseBody);
            }
            
        }

        // <AddItemsToContainerAsync>
        /// <summary>
        /// Add Family items to the container
        /// </summary>
        public async Task AddItemsToContainerAsync(Entry item)
        {
            item.Id = Guid.NewGuid().ToString();
            item.PartitionKey = _partitionKey;
             
            try
            {
                ItemResponse<Entry> entryResponse = await this.container.ReadItemAsync<Entry>(item.Id, new PartitionKey(item.PartitionKey));
                log.LogInformation("Item in database with id: {0} already exists\n", entryResponse.Resource.Id);
            }
            catch(CosmosException ex) when (ex.StatusCode == HttpStatusCode.NotFound)
            {
                ItemResponse<Entry> entryResponse = await this.container.CreateItemAsync<Entry>(item, new PartitionKey(item.PartitionKey));
                log.LogInformation("Created item in database with id: {0} Operation consumed {1} RUs.\n", entryResponse.Resource.Id, entryResponse.RequestCharge);
            }
        }
        // </AddItemsToContainerAsync>


        // <QueryItemsAsync>
        /// <summary>
        /// Run a query (using Azure Cosmos DB SQL syntax) against the container
        /// Including the partition key value of lastName in the WHERE filter results in a more efficient query
        /// </summary>
        public async Task<List<Entry>> QueryItemsAsync(string who)
        {
            var sqlQueryText = "SELECT * FROM c WHERE c.who = '" + who + "'";

            log.LogInformation("Running query: {0}\n", sqlQueryText);

            QueryDefinition queryDefinition = new QueryDefinition(sqlQueryText);
            FeedIterator<Entry> queryResultSetIterator = this.container.GetItemQueryIterator<Entry>(queryDefinition);

            List<Entry> entries = new List<Entry>();

            while (queryResultSetIterator.HasMoreResults)
            {
                FeedResponse<Entry> currentResultSet = await queryResultSetIterator.ReadNextAsync();
                foreach (Entry entry in currentResultSet)
                {
                    entries.Add(entry);
                    log.LogInformation("\tRead {0}\n", entry);
                }
            }

            return entries;
        }
        // </QueryItemsAsync>

        /// <summary>
        /// Replace an item in the container
        /// </summary>
        public async Task ReplaceEntryItemAsync(string id,Entry item)
        {
            ItemResponse<Entry> entryResponse = await this.container.ReadItemAsync<Entry>(id, new PartitionKey(_partitionKey));
            var itemBody = entryResponse.Resource;
            itemBody.Who = item.Who;
            itemBody.When = item.When;
            itemBody.What = item.What;
            entryResponse = await this.container.ReplaceItemAsync<Entry>(itemBody, itemBody.Id, new PartitionKey(itemBody.PartitionKey));
            log.LogInformation("Updated Entry [{0},{1}].\n \tBody is now: {2}\n", itemBody.PartitionKey, itemBody.Id, entryResponse.Resource);
        }

        /// <summary>
        /// Delete an item in the container
        /// </summary>
        public async Task DeleteEntryItemAsync(string id)
        {
            ItemResponse<Entry> wakefieldFamilyResponse = await this.container.DeleteItemAsync<Entry>(id,new PartitionKey(_partitionKey));
            log.LogInformation("Deleted Entry [{0},{1}]\n", _partitionKey, id);
        }
    }
}
