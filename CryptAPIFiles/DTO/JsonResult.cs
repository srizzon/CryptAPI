using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Web;

namespace CryptAPI.DTO
{
    public class JsonResult
    {
        public string message { get; set; }
        public bool error { get; set; }
        public HttpStatusCode status { get; set; }
    }
}