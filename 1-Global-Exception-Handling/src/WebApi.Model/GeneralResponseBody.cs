using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WebApi.Model
{
    public class GeneralErrorResponseBody
    {
        public string? InnerException { get; set; }
        public string Message { get; set; }
        public string RuntimeExceptionType { get; set; }
        public int StatusCode { get; set; }
        public string? TargetSite { get; set; }

    }
}
