using Dapper;
using Microsoft.Data.SqlClient;
using System;
using System.Collections.Generic;
using System.Data;
using System.Threading.Tasks;

namespace BlazorPurchaseOrders.Data {
    public class SupplierService : ISupplierService {
        // Database connection
        private readonly SqlConnectionConfiguration _configuration;
        public SupplierService(SqlConnectionConfiguration configuration) {
            _configuration = configuration;
        }
        // Add (create) a Supplier table row (SQL Insert)
        // This only works if you're already created the stored procedure.
        public async Task<bool> SupplierInsert(Supplier supplier) {
            using (var conn = new SqlConnection(_configuration.Value)) {
                var parameters = new DynamicParameters();
                parameters.Add("SupplierName", supplier.SupplierName, DbType.String);
                parameters.Add("SupplierAddress1", supplier.SupplierAddress1, DbType.String);
                parameters.Add("SupplierAddress2", supplier.SupplierAddress2, DbType.String);
                parameters.Add("SupplierAddress3", supplier.SupplierAddress3, DbType.String);
                parameters.Add("SupplierPostCode", supplier.SupplierPostCode, DbType.String);
                parameters.Add("SupplierEmail", supplier.SupplierEmail, DbType.String);
                parameters.Add("SupplierIsArchived", supplier.SupplierIsArchived, DbType.Boolean);

                // Stored procedure method
                await conn.ExecuteAsync("spSupplier_Insert", parameters, commandType: CommandType.StoredProcedure);
            }
            return true;
        }
        // Get a list of supplier rows (SQL Select)
        // This only works if you're already created the stored procedure.
        public async Task<IEnumerable<Supplier>> SupplierList() {
            IEnumerable<Supplier> suppliers;
            using (var conn = new SqlConnection(_configuration.Value)) {
                suppliers = await conn.QueryAsync<Supplier>("spSupplier_List", commandType: CommandType.StoredProcedure);
            }
            return suppliers;
        }

        // Get one supplier based on its SupplierID (SQL Select)
        // This only works if you're already created the stored procedure.
        public async Task<Supplier> Supplier_GetOne(int @SupplierID) {
            Supplier supplier = new Supplier();
            var parameters = new DynamicParameters();
            parameters.Add("@SupplierID", SupplierID, DbType.Int32);
            using (var conn = new SqlConnection(_configuration.Value)) {
                supplier = await conn.QueryFirstOrDefaultAsync<Supplier>("spSupplier_GetOne", parameters, commandType: CommandType.StoredProcedure);
            }
            return supplier;
        }
        // Update one Supplier row based on its SupplierID (SQL Update)
        // This only works if you're already created the stored procedure.
        public async Task<bool> SupplierUpdate(Supplier supplier) {
            using (var conn = new SqlConnection(_configuration.Value)) {
                var parameters = new DynamicParameters();
                parameters.Add("SupplierID", supplier.SupplierID, DbType.Int32);

                parameters.Add("SupplierName", supplier.SupplierName, DbType.String);
                parameters.Add("SupplierAddress1", supplier.SupplierAddress1, DbType.String);
                parameters.Add("SupplierAddress2", supplier.SupplierAddress2, DbType.String);
                parameters.Add("SupplierAddress3", supplier.SupplierAddress3, DbType.String);
                parameters.Add("SupplierPostCode", supplier.SupplierPostCode, DbType.String);
                parameters.Add("SupplierEmail", supplier.SupplierEmail, DbType.String);
                parameters.Add("SupplierIsArchived", supplier.SupplierIsArchived, DbType.Boolean);

                await conn.ExecuteAsync("spSupplier_Update", parameters, commandType: CommandType.StoredProcedure);
            }
            return true;
        }
    }
}