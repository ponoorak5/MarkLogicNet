using CommandDotNet;
using CommandDotNet.Tokens;
using MarkLogicNet.Models;
using MarkLogicNet.Models.Configuration;
using MarkLogicNetCmd.Providers;

namespace MarkLogicNetCmd.Commands
{
    public class Commands
    {
        private string? _username;
        private Password? _password;
        private Uri? _host;
        private int _timeout;
        private QueryLanguage _language;
        private FileInfo? _outputFile;
        private FileInfo? _inputFile;

        private readonly MarkLogicService _markLogicService;

        public Commands(MarkLogicService markLogicService)
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
            [Option(longName:"language", AssignToExecutableSubcommands = true)] QueryLanguage language = QueryLanguage.JavaScript,
            [Option(longName:"InputFile", AssignToExecutableSubcommands = true)] FileInfo? inputFile = null,
            [Option(longName:"OutputFile", AssignToExecutableSubcommands = true)] FileInfo? outputFile = null
        )
        {
            _username = username;
            _password = password;
            _host = url;
            _timeout = timeout;
            _language = language;
            _outputFile = outputFile;
            _inputFile = inputFile;

            var pipeline = ctx.InvocationPipeline;

            if (ctx.DependencyResolver?.Resolve(typeof(MarkLogicClientConfiguration)) is MarkLogicClientConfiguration cfg)
            {

                if (cfg.Url != _host)
                {
                    cfg.Url = _host;
                }

                if (cfg.Login != _username)
                {
                    cfg.Login = _username;
                }

                if (cfg.Password != _password?.GetPassword())
                {
                    cfg.Password = _password?.GetPassword();
                }
                
                cfg.Timeout = _timeout;
            }

            var result = next();

            // post-execution logic here

            return result;
        }

        [Command(Description = "provide input from pipe")]
        // ReSharper disable once UnusedMember.Global
        public async Task<int> Eval(IConsole console, CancellationToken ct)
        {
            string content;
            if (_inputFile != null)
            {
                content = await File.ReadAllTextAsync(_inputFile.FullName, ct);
            }
            else
            {
                content = await console.In.ReadToEndAsync(ct);
            }

            var stream = _outputFile != null ? new FileStream(_outputFile.FullName, FileMode.Create, FileAccess.Write) : Console.OpenStandardOutput();

            if (await _markLogicService.Execute(stream, content, _language, ct))
            {
                return 0;
            }

            return 1;
        }
    }
}