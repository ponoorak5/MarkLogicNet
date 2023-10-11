using System.IO;
using System.Net;

namespace MarkLogicClient.Models
{
    public class MarkLogicResponse
    {
        public bool HasError => StatusCode != HttpStatusCode.OK;
        public HttpStatusCode StatusCode { get; set;  }
        public Stream? Stream { get; set; }
        public string? ErrorMessage { get; set; }
    }
}
