using CommandDotNet;
using CommandDotNet.Diagnostics;
using CommandDotNet.IoC.MicrosoftDependencyInjection;
using MarkLogicNet.Extensions;
using MarkLogicNetCmd.Commands;
using MarkLogicNetCmd.Providers;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

Debugger.AttachIfDebugDirective(args);

var builder = Host.CreateApplicationBuilder(args);
builder.Services.AddLogging(options => options
    .ClearProviders()
    .AddConfiguration(builder.Configuration.GetSection("Logging")));

builder.Services.AddTransient<MarkLogicService>();
builder.Services.AddMarkLogicClient();
builder.Services.AddSingleton<ExecuteCommand>();

var host = builder.Build();

return new AppRunner<ExecuteCommand>()
    .UseMicrosoftDependencyInjection(host.Services)
    .UseCancellationHandlers()
    .UseDebugDirective()
    .Run(args);