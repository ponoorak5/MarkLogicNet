using System.IO;
using System.Net;

namespace MarkLogicNet.Models
{
    public class MarkLogicResponse
    {
        public bool HasError => StatusCode != HttpStatusCode.OK;
        public HttpStatusCode StatusCode { get; set;  }
        public Stream? Stream { get; set; }
        public string? ErrorMessage { get; set; }
    }
}
