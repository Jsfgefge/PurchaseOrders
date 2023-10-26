// This is the service for the Product class.
using Dapper;
using Microsoft.Data.SqlClient;
using System;
using System.Collections.Generic;
using System.Data;
using System.Threading.Tasks;

namespace BlazorPurchaseOrders.Data {
    public class ProductService : IProductService {
        // Database connection
        private readonly SqlConnectionConfiguration _configuration;
        public ProductService(SqlConnectionConfiguration configuration) {
            _configuration = configuration;
        }
        // Add (create) a Product table row (SQL Insert)
        // This only works if you're already created the stored procedure.
        public async Task<bool> ProductInsert(Product product) {
            using (var conn = new SqlConnection(_configuration.Value)) {
                var parameters = new DynamicParameters();
                parameters.Add("ProductCode", product.ProductCode, DbType.String);
                parameters.Add("ProductDescription", product.ProductDescription, DbType.String);
                parameters.Add("ProductUnitPrice", product.ProductUnitPrice, DbType.Decimal);
                parameters.Add("ProductSupplierID", product.ProductSupplierID, DbType.Int32);
                parameters.Add("ProductIsArchived", product.ProductIsArchived, DbType.Boolean);

                // Stored procedure method
                await conn.ExecuteAsync("spProduct_Insert", parameters, commandType: CommandType.StoredProcedure);
            }
            return true;
        }
        // Get a list of product rows (SQL Select)
        // This only works if you're already created the stored procedure.
        public async Task<IEnumerable<Product>> ProductList() {
            IEnumerable<Product> products;
            using (var conn = new SqlConnection(_configuration.Value)) {
                products = await conn.QueryAsync<Product>("spProduct_List", commandType: CommandType.StoredProcedure);
            }
            return products;
        }

        // Get one product based on its ProductID (SQL Select)
        // This only works if you're already created the stored procedure.
        public async Task<Product> Product_GetOne(int @ProductID) {
            Product product = new Product();
            var parameters = new DynamicParameters();
            parameters.Add("@ProductID", ProductID, DbType.Int32);
            using (var conn = new SqlConnection(_configuration.Value)) {
                product = await conn.QueryFirstOrDefaultAsync<Product>("spProduct_GetOne", parameters, commandType: CommandType.StoredProcedure);
            }
            return product;
        }
        // Update one Product row based on its ProductID (SQL Update)
        // This only works if you're already created the stored procedure.
        public async Task<bool> ProductUpdate(Product product) {
            using (var conn = new SqlConnection(_configuration.Value)) {
                var parameters = new DynamicParameters();
                parameters.Add("ProductID", product.ProductID, DbType.Int32);

                parameters.Add("ProductCode", product.ProductCode, DbType.String);
                parameters.Add("ProductDescription", product.ProductDescription, DbType.String);
                parameters.Add("ProductUnitPrice", product.ProductUnitPrice, DbType.Decimal);
                parameters.Add("ProductSupplierID", product.ProductSupplierID, DbType.Int32);
                parameters.Add("ProductIsArchived", product.ProductIsArchived, DbType.Boolean);

                await conn.ExecuteAsync("spProduct_Update", parameters, commandType: CommandType.StoredProcedure);
            }
            return true;
        }
    }
}