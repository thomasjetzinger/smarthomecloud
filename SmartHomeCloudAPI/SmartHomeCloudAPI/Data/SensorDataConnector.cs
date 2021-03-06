﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Auth;
using Microsoft.WindowsAzure.Storage.Table;
using System.Configuration;
using SmartHomeCloudAPI.Models;

namespace SmartHomeCloudAPI.Data
{


    public class SensorDataConnector
    {
        private CloudTable table;

        public SensorDataConnector()
        {
            CloudStorageAccount storageAccount = CloudStorageAccount.Parse(
                ConfigurationManager.AppSettings["StorageConnectionString"]);
            // Create the table client.
           
             table = storageAccount.CreateCloudTableClient().GetTableReference("SensorData");
        }

   
        public TableQuerySegment<SensorValueEntity> GetAllEntries(TableContinuationToken token)
        { 
            // Construct the query operation for all customer entities where PartitionKey="Smith".
            TableQuery<SensorValueEntity> query = new TableQuery<SensorValueEntity>().Where(TableQuery.GenerateFilterCondition("PartitionKey", QueryComparisons.Equal, "2298a348-e2f9-4438-ab23-82a3930662ab")).Take(100);
            return table.ExecuteQuerySegmented(query,token);
           
        }

        public TableQuerySegment<SensorValueEntity> GetEntries(TableContinuationToken token, SensorValueFilter filter)
        {
            // Construct the query operation for all customer entities where PartitionKey="Smith".
            TableQuery<SensorValueEntity> query = new TableQuery<SensorValueEntity>().Where(filter.GetFilterString()).Take(100);
            return table.ExecuteQuerySegmented(query, token);

        }

        public long ClearTable()
        {
            long deletionCount = 0;
            // Construct the query operation for all customer entities where PartitionKey="Smith".
            var list = new List<string>();
            list.Add("PartitionKey");
            list.Add("RowKey");
            TableQuery<SensorValueEntity> query = new TableQuery<SensorValueEntity>().Select(list).Take(100);
            var results = table.ExecuteQuery(query);
            
            if (results.Count() < 1)
                return deletionCount;
            foreach(var resultGroup in results.GroupBy(a => a.PartitionKey))
            {
                TableBatchOperation batchOperation = new TableBatchOperation();
                foreach (var result in resultGroup)
                {
                    batchOperation.Delete(result);
                    deletionCount++;
                }
                table.ExecuteBatch(batchOperation);
            }
            
            return deletionCount;
        }
    }
}