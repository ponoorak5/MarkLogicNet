using MarkLogicClient.Models;
using System.Collections.Generic;
using System.IO;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using EnumsNET;
using MarkLogicClient.Models.Configuration;
using System;

namespace MarkLogicClient
{
    public class MarkLogicClient
    {
        private readonly HttpClient _httpClient;
        private readonly MarkLogicClientConfiguration _configuration;


        public MarkLogicClient(HttpClient httpClient, MarkLogicClientConfiguration configuration)
        {
            _httpClient = httpClient;
            _configuration = configuration;
        }

        public async Task<MarkLogicResponse> ExecuteXqueryScript(string xQuery)
        {
            return await ExecuteScript(xQuery, QueryLanguage.XQuery);
        }

        public async Task<MarkLogicResponse> ExecuteJsScript(string jsScript)
        {
            return await ExecuteScript(jsScript, QueryLanguage.JavaScript);
        }

        public async Task<MarkLogicResponse> ExecuteScript(string script, QueryLanguage queryLanguage, CancellationToken cancellationToken = default)
        {
            using var request = new HttpRequestMessage();
            request.Method = HttpMethod.Post;
            request.Content = new FormUrlEncodedContent(new List<KeyValuePair<string?, string>>()
            {
                new KeyValuePair<string?, string>(queryLanguage.AsString(EnumFormat.Description), script)
            });

            _httpClient.BaseAddress = _configuration.GetEvalUri();
            _httpClient.DefaultRequestHeaders.Clear();
            _httpClient.DefaultRequestHeaders.Add("Authorization", _configuration.GetCredentials());
            _httpClient.Timeout = TimeSpan.FromMilliseconds(_configuration.Timeout);
            
            var response = await _httpClient.SendAsync(request, cancellationToken);
            var ms = await response.Content.ReadAsStreamAsync();

            var result = new MarkLogicResponse()
            {
                StatusCode = response.StatusCode
            };

            if (result.HasError)
            {
                using var tr = new StreamReader(ms);
                var errorMessage = await tr.ReadToEndAsync();
                result.ErrorMessage = errorMessage;
                result.Stream = null;

                return result;
            }

            result.Stream = ms;
            return result;
        }
    }
}