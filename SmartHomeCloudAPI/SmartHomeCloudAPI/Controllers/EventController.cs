﻿using Microsoft.WindowsAzure.Storage.Table;
using SmartHomeCloudAPI.Data;
using SmartHomeCloudAPI.Models;
using Swashbuckle.Swagger.Annotations;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;

namespace SmartHomeCloudAPI.Controllers
{
    public class EventController : ApiController
    {

        private static EventDataConnector dataConnector = new EventDataConnector();



        [HttpGet]     
        public HttpResponseMessage Get([FromUri]EventFilter filter, string NextPartitionKey = null, string NextRowKey = null)
        {
            TableContinuationToken token = null;
            if (!String.IsNullOrEmpty(NextPartitionKey) && !String.IsNullOrEmpty(NextRowKey))
            {
                token = new TableContinuationToken();
                token.NextRowKey = NextRowKey;
                token.NextPartitionKey = NextPartitionKey;
                token.TargetLocation = Microsoft.WindowsAzure.Storage.StorageLocation.Primary;
            }
            var values = dataConnector.GetEvents(token,filter);
            var response = Request.CreateResponse(HttpStatusCode.OK, values);

            //add continuation token to header
            if (values.ContinuationToken != null && !String.IsNullOrEmpty(values.ContinuationToken.NextPartitionKey) && !String.IsNullOrEmpty(values.ContinuationToken.NextRowKey))
            {
                response.Headers.Add("x-ms-continuation-NextPartitionKey", values.ContinuationToken.NextPartitionKey);
                response.Headers.Add("x-ms-continuation-NextRowKey", values.ContinuationToken.NextRowKey);

            }
            return response;
        }
    }
}
