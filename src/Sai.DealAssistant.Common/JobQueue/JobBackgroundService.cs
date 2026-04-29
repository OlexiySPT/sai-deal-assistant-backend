using MediatR;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Sai.DealAssistant.Common.JobQueue;
using Sai.DealAssistant.Common.Queue;

namespace Sai.DealAssistant.Application.DealAutomation;

public class JobBackgroundService : BackgroundService
{
    private readonly IJobQueue<IJobQueueCommand> _queue;
    private readonly IServiceProvider _serviceProvider;

    public JobBackgroundService(IJobQueue<IJobQueueCommand> queue, IServiceProvider serviceProvider)
    {
        _queue = queue;
        _serviceProvider = serviceProvider;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        while (!stoppingToken.IsCancellationRequested)
        {
            if (_queue.TryDequeue(out var job))
            {
                using var scope = _serviceProvider.CreateScope();
                var mediator = scope.ServiceProvider.GetRequiredService<IMediator>();
                await mediator.Send(job, stoppingToken);
            }
            else
            {
                await Task.Delay(1000, stoppingToken); // Polling delay
            }
        }
    }
}