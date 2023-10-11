using System;
using Microsoft.Extensions.Configuration;

namespace MarkLogicNet.Models.Configuration
{
    public class MarkLogicClientConfiguration
    {
        public MarkLogicClientConfiguration() { }
        public MarkLogicClientConfiguration(IConfiguration configuration)
        {
            configuration.Bind("MarkLogicClientConfiguration", this);
        }
        public Uri? Url { get; set; }
        public string? Login { get; set; }
        public string? Password { get; set; }
        public int Timeout { get; set; } = 30000;

        public string GetCredentials()
        {

            var encoded = Convert.ToBase64String(System.Text.Encoding.GetEncoding("ISO-8859-1").GetBytes(
                $"{Login}:{Password}"));
            return $"Basic {encoded}";
        }

        public Uri GetEvalUri()
        {
            if (Url == null)
            {
                throw new ApplicationException("Missing Url for MarkLogic server");
            }
            return new Uri(Url, "/LATEST/eval");
        }
    }
}
