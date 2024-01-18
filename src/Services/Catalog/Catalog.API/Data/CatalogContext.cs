using Catalog.API.Entities;
using Microsoft.Extensions.Configuration;
using MongoDB.Driver;

namespace Catalog.API.Data
{
    public class CatalogContext : ICatalogContext
    {
        public IMongoCollection<Product> Products { get; set; }

        public CatalogContext(IConfiguration configuration)
        {
            var client = new MongoClient(GetConfigurationByName(configuration, nameof(DataBaseItems.ConnectionString)));
            var database = client.GetDatabase(GetConfigurationByName(configuration, nameof(DataBaseItems.DatabaseName)));

            Products = database.GetCollection<Product>(GetConfigurationByName(configuration, nameof(DataBaseItems.CollectionName)));
            CatalogContextSeed.SeedData(Products);
        }


        #region Private Methods
        private string GetConfigurationByName(IConfiguration configuration, string name) => configuration.GetValue<string>($"DatabaseSettings:{name}");

        #endregion
    }
}
