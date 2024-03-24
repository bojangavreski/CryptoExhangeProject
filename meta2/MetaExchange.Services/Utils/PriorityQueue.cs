public class PriorityQueue<T>
{
    private readonly Comparison<T> _comparison;
    private readonly List<T> _heap = new List<T>();

    public int Count => _heap.Count;

    public PriorityQueue(Comparison<T> comparison)
    {
        _comparison = comparison;
    }

    public void Enqueue(T item)
    {
        _heap.Add(item);
        int i = _heap.Count - 1;
        while (i > 0)
        {
            int parent = (i - 1) / 2;
            if (_comparison(_heap[parent], _heap[i]) <= 0)
                break;
            Swap(i, parent);
            i = parent;
        }
    }

    public T Dequeue()
    {
        if (_heap.Count == 0)
            throw new InvalidOperationException("Priority queue is empty");

        T root = _heap[0];
        int lastIndex = _heap.Count - 1;
        _heap[0] = _heap[lastIndex];
        _heap.RemoveAt(lastIndex);

        int current = 0;
        while (true)
        {
            int child = current * 2 + 1;
            if (child >= _heap.Count)
                break;

            if (child + 1 < _heap.Count && _comparison(_heap[child], _heap[child + 1]) > 0)
                child++;

            if (_comparison(_heap[current], _heap[child]) <= 0)
                break;

            Swap(current, child);
            current = child;
        }

        return root;
    }

    private void Swap(int i, int j)
    {
        T temp = _heap[i];
        _heap[i] = _heap[j];
        _heap[j] = temp;
    }
}