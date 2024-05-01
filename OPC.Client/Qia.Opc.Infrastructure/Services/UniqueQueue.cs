namespace QIA.Opc.Infrastructure.Services;

public class UniqueQueue<T>
{
    private readonly Queue<T> _queue = new();
    private readonly HashSet<T> _hashSet = new();
    private const int ELEMENTS_TO_KEEP = 10;
    // Adds an item to the queue if it is not already present.
    public bool Enqueue(T item)
    {
        if (_hashSet.Add(item))
        {
            _queue.Enqueue(item);
            if (_queue.Count > ELEMENTS_TO_KEEP)
            {
                Dequeue();
            }
            return true;
        }
        return false;
    }

    // Removes and returns the item at the beginning of the queue.
    public T Dequeue()
    {
        if (_queue.Count == 0)
        {
            throw new InvalidOperationException("The queue is empty.");
        }

        T item = _queue.Dequeue();
        _hashSet.Remove(item);
        return item;
    }

    // Gets the number of elements contained in the queue.
    public int Count => _queue.Count;

    // Checks if the queue contains the specified item.
    public bool Contains(T item) => _hashSet.Contains(item);

    // Returns the item at the beginning of the queue without removing it.
    public T Peek()
    {
        if (_queue.Count == 0)
        {
            throw new InvalidOperationException("The queue is empty.");
        }

        return _queue.Peek();
    }
}
