﻿using Dapper;
using Microsoft.AspNetCore.Server.IIS.Core;
using Microsoft.Data.SqlClient;
using Microsoft.Identity.Client;
using Microsoft.Win32.SafeHandles;
using System;
using System.Collections.Generic;
using System.Data;
using System.Reflection.Metadata.Ecma335;
using System.Threading.Tasks;

namespace BlazorPurchaseOrders.Data{
    public class POHeaderService : IPOHeaderService {
        private readonly SqlConnectionConfiguration _configuration;
        public POHeaderService(SqlConnectionConfiguration configuration) {
            _configuration = configuration;
        }
        //Add (create) a POHeader table row (SQL Insert)
        //This Only works if you're alreader created the store procedure.
        public async Task<bool> POHeaderInsert(POHeader poheader) {
            using (var conn = new SqlConnection(_configuration.Value)) {
                var parameters = new DynamicParameters();
                parameters.Add("POHeaderOrderNumber", poheader.POHeaderOrderNumber, DbType.Int32);
                parameters.Add("POHeaderOrderDAte", poheader.POHeaderOrderDate, DbType.Date);
                parameters.Add("PoHeaderSupplierID", poheader.POHeaderID, DbType.Int32);
                parameters.Add("POHeaderSupplierAddress1", poheader.POHeaderSupplierAddress1, DbType.String);
                parameters.Add("POHeaderSupplierAddress2", poheader.POHeaderSupplierAddress2, DbType.String);
                parameters.Add("POHeaderSupplierAddress3", poheader.POHeaderSupplierAddress3, DbType.String);
                parameters.Add("POHeaderSupplierPostCode", poheader.POHeaderSupplierPostCode, DbType.String);
                parameters.Add("POHeaderSupplierEmail", poheader.POHeaderSupplierEmail, DbType.String);
                parameters.Add("POHeaderRequestedBy", poheader.POHeaderRequestedBy, DbType.String);
                parameters.Add("POHeaderIsArquived", poheader.POHeaderIsArchived, DbType.Boolean);

                await conn.ExecuteAsync("spPOHeader_Insert", parameters, commandType: CommandType.StoredProcedure);

            }
            return true;
        }
        //Get a list of poheader rows (SQL Select)
        //This Only works if you're already created the stored procedure
        public async Task<IEnumerable<POHeader>> POHeaderList() {
            IEnumerable<POHeader> poheaders;
            using (var conn = new SqlConnection(_configuration.Value)) {
                poheaders = await conn.QueryAsync<POHeader>("spPOHeader_List", commandType: CommandType.StoredProcedure);
            }
            return poheaders;
        }
        public async Task<POHeader> POHeader_GetOne(int @POHeaderID) {
            POHeader poheader = new POHeader();
            var parameters = new DynamicParameters();
            parameters.Add("@POHeaderID", POHeaderID, DbType.Int32);
            using(var conn = new SqlConnection(_configuration.Value)) {
                poheader = await conn.QueryFirstOrDefaultAsync<POHeader>("spPOHeader_GetOne", parameters, commandType: CommandType.StoredProcedure);
            }
            return poheader;
        }
        //Update one POHeader row based on its POHeaderID (SQL Update)
        public async Task<bool> POHeaderUpdate(POHeader poheader) {
            using(var conn = new SqlConnection(_configuration.Value)) {
                var parameters = new DynamicParameters();
                parameters.Add("POHeaderID", poheader.POHeaderID, DbType.Int32);
                parameters.Add("POHeaderOrderNumber", poheader.POHeaderOrderNumber, DbType.Int32);
                parameters.Add("POHeaderOrderDate", poheader.POHeaderOrderDate, DbType.Int32);
                parameters.Add("POHeaderSupplierID", poheader.POHeaderSupplierID, DbType.Int32);
                parameters.Add("POHeaderSupplierAddress1", poheader.POHeaderSupplierAddress1, DbType.Int32);
                parameters.Add("POHeaderSupplierAddress2", poheader.POHeaderSupplierAddress2, DbType.Int32);
                parameters.Add("POHeaderSupplierAddress3", poheader.POHeaderSupplierAddress3, DbType.Int32);
                parameters.Add("POHeaderSupplierPostCode", poheader.POHeaderSupplierPostCode, DbType.String);
                parameters.Add("POHeaderSupplierEmail", poheader.POHeaderSupplierEmail, DbType.String);
                parameters.Add("POHeaderRequestedBy", poheader.POHeaderRequestedBy, DbType.String);
                parameters.Add("POHeaderIsArquived", poheader.POHeaderIsArchived, DbType.Boolean);

                await conn.ExecuteAsync("spPOHeader_Update", parameters, commandType: CommandType.StoredProcedure);
            }
            return true;
        }
    }
}