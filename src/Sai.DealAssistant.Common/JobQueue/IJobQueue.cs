namespace Sai.DealAssistant.Common.Queue;

public interface IJobQueue<T>
{
    void Enqueue(T job);
    bool TryDequeue(out T job);
}
