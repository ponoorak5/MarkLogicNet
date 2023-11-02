using CommandDotNet;
using MarkLogicNet;
using MarkLogicNet.Models;

namespace MarkLogicNetCmd.Providers
{
    public class MarkLogicService
    {
        private readonly MarkLogicClient _markLogicClient;

        public MarkLogicService(MarkLogicClient markLogicClient)
        {
            _markLogicClient = markLogicClient;
        }

        public async Task<bool> Execute(Stream outputStream, string script, QueryLanguage language, CancellationToken token)
        {
            var result = await _markLogicClient.ExecuteScript(script, language, token);

            await using var writer = new StreamWriter(outputStream);
            writer.AutoFlush = true;

            if (result.HasError)
            {
                await writer.WriteLineAsync(result.ErrorMessage);
                return false;
            }

            using var reader = new MarkLogicStreamReader(result.Stream ?? throw new InvalidOperationException());
            while (await reader.ReadLineAsync() is { } res)
            {
                await writer.WriteLineAsync(res.Content);
            }

            return true;
        }

        public async Task<bool> ExecuteFile(Stream stream, string scriptFileName, QueryLanguage language, CancellationToken token)
        {
            var content = await File.ReadAllTextAsync(scriptFileName, token);
            return await Execute(stream, content, language, token);
        }
    }
}