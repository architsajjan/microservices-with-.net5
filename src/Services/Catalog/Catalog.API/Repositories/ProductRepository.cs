using Catalog.API.Data;
using Catalog.API.Entities;
using Microsoft.Extensions.Logging;
using MongoDB.Driver;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading.Tasks;

namespace Catalog.API.Repositories
{
    public class ProductRepository : IProductRepository
    {
        #region Properties and Constructors
        private readonly ICatalogContext _context;
        private readonly ILogger<ProductRepository> _logger;
        private Stopwatch stopwatch = new Stopwatch();

        public ProductRepository(ICatalogContext context, ILogger<ProductRepository> logger)
        {
            _context = CheckForNulls(context, nameof(ICatalogContext));
            _logger = CheckForNulls(logger, nameof(ProductRepository));
        }
        #endregion

        public async Task CreateProduct(Product product)
        {
            try
            { 
                InitializeStopWatch();
                await _context.Products.InsertOneAsync(product);
                LogDatabaseExecutionRuntime(nameof(CreateProduct));
            }
            catch (Exception e)
            {
                LogDatabaseExecutionRuntime(nameof(CreateProduct), e.InnerException?.Message ?? e.Message);
            }
        }

        public async Task<bool> DeleteProduct(string id)
        {
            try
            { 
                InitializeStopWatch();
                var updateResult = await _context.Products.DeleteOneAsync(
                    filter: item => item.Id.Equals(id));
                LogDatabaseExecutionRuntime(nameof(DeleteProduct));

                return updateResult.IsAcknowledged && updateResult.DeletedCount.Equals(1);
            }
            catch (Exception e)
            {
                LogDatabaseExecutionRuntime(nameof(DeleteProduct), e.InnerException?.Message ?? e.Message);
                return false;
            }
        }

        public async Task<Product> GetProduct(string id)
        {
            try
            {
                InitializeStopWatch();
                var product = await _context.Products.Find(product => product.Id.Equals(id)).FirstOrDefaultAsync();
                LogDatabaseExecutionRuntime(nameof(GetProduct));

                return product;
            }
            catch (Exception e)
            {
                LogDatabaseExecutionRuntime(nameof(GetProduct), e.InnerException?.Message ?? e.Message);
                return new Product();
            }
        }

        public async Task<IEnumerable<Product>> GetProductByCategory(string categoryName)
        {
            FilterDefinition<Product> filter = Builders<Product>.Filter.Eq(product => product.Category, categoryName);

            try
            {
                InitializeStopWatch();
                var products = await _context.Products.Find(filter).ToListAsync();
                LogDatabaseExecutionRuntime(nameof(GetProductByCategory));

                return products;
            }
            catch (Exception e)
            {
                LogDatabaseExecutionRuntime(nameof(GetProductByCategory), e.InnerException?.Message ?? e.Message);
                return new List<Product>();
            }
        }

        public async Task<IEnumerable<Product>> GetProductByName(string name)
        {
            FilterDefinition<Product> filter = Builders<Product>.Filter.Eq(product => product.ProductName, name);

            try
            {
                InitializeStopWatch();
                var products = await _context.Products.Find(filter).ToListAsync();
                LogDatabaseExecutionRuntime(nameof(GetProductByName));

                return products;
            }
            catch (Exception e)
            {
                LogDatabaseExecutionRuntime(nameof(GetProductByName), e.InnerException?.Message ?? e.Message);
                return new List<Product>();
            }
        }

        public async Task<IEnumerable<Product>> GetProducts()
        {
            try
            {
                InitializeStopWatch();
                var products = await _context.Products.Find(product => true).ToListAsync();
                LogDatabaseExecutionRuntime(nameof(GetProducts));

                return products;
            }
            catch (Exception e)
            {
                LogDatabaseExecutionRuntime(nameof(GetProducts), e.InnerException?.Message ?? e.Message);
                return new List<Product>();
            }
        }

        public async Task<bool> UpdateProduct(Product product)
        {
            try
            {
                InitializeStopWatch();

                var updateResult = await _context.Products.ReplaceOneAsync(
                filter: item => item.Id.Equals(product.Id),
                replacement: product);

                LogDatabaseExecutionRuntime(nameof(UpdateProduct));
                return updateResult.IsAcknowledged && updateResult.ModifiedCount.Equals(1);
            }
            catch (Exception e)
            {
                LogDatabaseExecutionRuntime(nameof(UpdateProduct), e.InnerException?.Message ?? e.Message);
                return false;
            }
        }

        #region Private Methods
        private void LogDatabaseExecutionRuntime(string name, string additionalInfo = "")
        {
            stopwatch.Stop();
            _logger.LogDebug($"<<{name}>> ran for <<{stopwatch.ElapsedMilliseconds}>> ms at {DateTime.Now}.");
            if (!string.IsNullOrWhiteSpace(additionalInfo))
            {
                _logger.LogDebug($"Additional Info : {additionalInfo}");
            }
        }

        private void InitializeStopWatch()
        {
            stopwatch = Stopwatch.StartNew();
        }

        private dynamic CheckForNulls(dynamic value, string typeName) => value ?? throw new ArgumentException($"{nameof(value)} at {typeName} on {DateTime.Now}");
        #endregion
    }
}
