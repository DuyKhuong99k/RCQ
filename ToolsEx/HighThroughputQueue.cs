namespace ToolsEx;

using System.Collections.Concurrent;

public class HighThroughputQueue<T>
{
    private readonly ConcurrentQueue<T> _queue = new();

    public void Enqueue(T item)
    {
        _queue.Enqueue(item);
    }

    public List<T> GetSnapshot()
    {
        return _queue.ToList();
    }

    public bool TryDequeue(out T item)
    {
        return _queue.TryDequeue(out item);
    }

    public int Count => _queue.Count;
}