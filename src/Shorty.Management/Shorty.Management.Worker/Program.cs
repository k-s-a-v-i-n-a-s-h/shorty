using Shorty.Management.Worker.Extensions;

var builder = Host.CreateApplicationBuilder(args);

var workerMode = Environment.GetEnvironmentVariable("WorkerMode")?.Trim().ToLowerInvariant();
_ = workerMode switch
{
    "relay" => builder.Services.AddRelayServices(builder.Configuration),
    "dispatch" => builder.Services.AddDispatchServices(builder.Configuration),
    "cleanup" => builder.Services.AddCleanupServices(builder.Configuration),
    _ => throw new ArgumentException($"Invalid worker mode: '{workerMode}'")
};

var host = builder.Build();
host.Run();
