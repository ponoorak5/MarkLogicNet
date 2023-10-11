using CommandDotNet;
using MarkLogicNet.Models;
using MarkLogicNet.Models.Configuration;
using MarkLogicNetCmd.Providers;

namespace MarkLogicNetCmd.Commands
{
    public class ExecuteCommand
    {
        private string? _username;
        private Password? _password;
        private Uri? _host;
        private int _timeout;
        private QueryLanguage _language;

        private readonly MarkLogicService _markLogicService;

        public ExecuteCommand(MarkLogicService markLogicService)
        {
            _markLogicService = markLogicService;
        }

        public Task<int> Interceptor(
            InterceptorExecutionDelegate next,
            CommandContext ctx,
            [Option('u', "username")] string? username,
            [Option('p', "password")] Password? password,
            [Option('s', "server")] Uri? url,
            [Option('t', "timeout", Description = "timeout (in ms) waiting")] int timeout = 30000,
            [Option(longName:"language", AssignToExecutableSubcommands = true)] QueryLanguage language = QueryLanguage.JavaScript
        )
        {
            _username = username;
            _password = password;
            _host = url;
            _timeout = timeout;
            _language = language;
            var pipeline = ctx.InvocationPipeline;

            var cfg = ctx.DependencyResolver?.Resolve(typeof(MarkLogicClientConfiguration)) as MarkLogicClientConfiguration;
            cfg.Url = _host;
            cfg.Login = _username;
            cfg.Password = _password?.GetPassword();
            cfg.Timeout = _timeout;
            var result = next();

            // post-execution logic here

            return result;
        }

        [Command(Description = "Default command, provide input from pipe")]
        public async Task<int> Execute(IConsole console, CancellationToken ct)
        {
            var s = await console.In.ReadToEndAsync(ct);
            if (await _markLogicService.Execute(s, _language, ct))
            {
                return 0;
            }

            return 1;
        }

        [Command(Description = "Provide input from file")]
        public async Task<int> File(IConsole console, CancellationToken ct, [Option(longName:"InputFile")]string inputfile)
        {
            if (await _markLogicService.ExecuteFile(inputfile, _language, ct))
            {
                return 0;
            }

            return 1;
        }
    }
}