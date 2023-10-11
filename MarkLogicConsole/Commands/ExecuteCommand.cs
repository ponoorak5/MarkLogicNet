using CommandDotNet;
using MarkLogicClient.Models;
using MarkLogicClient.Models.Configuration;
using MarkLogicConsole.Providers;

namespace MarkLogicConsole.Commands
{
    public class ExecuteCommand
    {
        private string? _username;
        private Password? _password;
        private Uri? _host;
        private readonly ServiceCall _serviceCall;
        private int _timeout;
        private QueryLanguage _language;

        // Constructors can take any resolvable parameters. IConsole, CommandContext,
        // and CancellationToken when UseCancellationHandlers is configured.
        public ExecuteCommand(ServiceCall serviceCall)
        {
            _serviceCall = serviceCall;
        }

        // Method name does not matter but must include an InterceptorExecutionDelegate parameter.
        // All other parameters listed here are optional.
        public Task<int> Interceptor(
            InterceptorExecutionDelegate next,
            CommandContext ctx,
            [Option('u', "username")] string? username,
            [Option('p', "password")] Password? password,
            [Option('s', "server")] Uri? url,
            [Option('t', "timeout", Description = "timeout for response in ms.")] int timeout = 30000,
            [Option('q', AssignToExecutableSubcommands = true)] QueryLanguage language = QueryLanguage.JavaScript
        )
        {
            _username = username;
            _password = password;
            _host = url;
            _timeout = timeout;
            _language = language;

            // access to AppConfig, AppSettings and other services
            var settings = ctx.AppConfig.AppSettings;
            // access to parse results, including remaining and separated arguments
            var parseResult = ctx.ParseResult;
            // access to target command method and its hosting object,
            // and all interceptor methods in the path to the target command.
            var pipeline = ctx.InvocationPipeline;

            var cfg = ctx.DependencyResolver?.Resolve(typeof(MarkLogicClientConfiguration)) as MarkLogicClientConfiguration;
            cfg.Url = _host;
            cfg.Login = _username;
            cfg.Password = _password?.GetPassword();
            cfg.Timeout = _timeout;
            // pre-execution logic here

            // next() will execute the TargetCommand and all
            // remaining interceptors in the ctx.InvocationPipeline
            var result = next();

            // post-execution logic here

            return result;
        }

        public async Task<int> Execute(IConsole console, CancellationToken ct)
        {
            var s = await console.In.ReadToEndAsync(ct);
            if (await _serviceCall.Execute(s, _language, ct))
            {
                return 0;
            }

            return 1;
        }

        public async Task<int> File(IConsole console, CancellationToken ct, string inputfile)
        {
            if (await _serviceCall.ExecuteFile(inputfile, _language, ct))
            {
                return 0;
            }

            return 1;
        }
    }
}