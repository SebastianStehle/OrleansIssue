
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Orleans;
using Orleans.Hosting;

class Filter : IIncomingGrainCallFilter
{
    public Task Invoke(IIncomingGrainCallContext context)
    {
        return Task.CompletedTask;
    }
}

public interface IMyGrain : IGrainWithGuidKey
{
    Task Foo();
}

public class MyGrain : Grain, IMyGrain
{
    public Task Foo()
    {
        return Task.CompletedTask;
    }
}

public class HostedService : IHostedService
{
    private readonly IGrainFactory factory;

    public HostedService(IGrainFactory factory)
    {
        this.factory = factory;
    }

    public Task StartAsync(CancellationToken cancellationToken)
    {
        var grain = factory.GetGrain<IMyGrain>(Guid.NewGuid());

        return grain.Foo();
    }

    public Task StopAsync(CancellationToken cancellationToken)
    {
        throw new NotImplementedException();
    }
}

public static class Program
{
    public static void Main()
    {
        Host.CreateDefaultBuilder()
            .UseOrleans(builder =>
            {
                builder.UseLocalhostClustering();
                builder.AddIncomingGrainCallFilter<Filter>();
            })
            .ConfigureServices(services =>
            {
                services.AddSingleton<IHostedService, HostedService>();
            })
            .Build()
            .Run();
    }
}
