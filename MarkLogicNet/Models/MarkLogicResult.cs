using System.Collections.Generic;

namespace MarkLogicNet.Models
{
    public class MarkLogicResult
    {
        public string? Content { get; set; }
        public string? ContentType { get; set; }
        public Dictionary<string, string> DataType { get; set; } = new Dictionary<string, string>();
    }
}
