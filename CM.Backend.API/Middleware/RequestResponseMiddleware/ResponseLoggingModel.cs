using System.Collections;
using System.Collections.Generic;
using Baseline;
using CM.Backend.Persistence.Model;

namespace CM.Backend.API.Middleware
{
    public class ResponseLoggingModel
    {
        public int StatusCode { get; set; }
        public string ReasonPhrase { get; set; }
        public string Date { get; set; }
        public string Server { get; set; }
        public string ContentType { get; set; }
    }
}