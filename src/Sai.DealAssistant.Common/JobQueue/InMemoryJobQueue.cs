using System.Collections.Concurrent;

namespace Sai.DealAssistant.Common.Queue;

public class InMemoryJobQueue<T> : IJobQueue<T>
{
    private readonly ConcurrentQueue<T> _queue = new();

    public void Enqueue(T job)
    {
        _queue.Enqueue(job);
    }

    public bool TryDequeue(out T job)
    {
        return _queue.TryDequeue(out job);
    }
}
