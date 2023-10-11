using MarkLogicClient.Models;
using MarkLogicClient.Utilities;

namespace MarkLogicConsole.Providers
{
    public class ServiceCall
    {
        private readonly MarkLogicClient.MarkLogicClient _markLogicClient;

        public ServiceCall(MarkLogicClient.MarkLogicClient markLogicClient)
        {
            _markLogicClient = markLogicClient;
        }

        public async Task<bool> Execute(string script, QueryLanguage language, CancellationToken token)
        {
            var result = await _markLogicClient.ExecuteScript(script, language, token);

            if (result.HasError)
            {
                Console.WriteLine(result.ErrorMessage);
                return false;
            }

            using var reader = new MarkLogicStreamReader(result.Stream ?? throw new InvalidOperationException());
            while (await reader.ReadLineAsync() is { } res)
            {
                Console.WriteLine(res.Content);
            }

            return true;
        }

        public async Task<bool> ExecuteFile(string scriptFileName, QueryLanguage language, CancellationToken token)
        {
            var content = await File.ReadAllTextAsync(scriptFileName, token);
            return await Execute(content, language, token);
        }
    }
}