using System.Collections.Concurrent;
using UnityEngine;

public interface IPoolable
{
    ObjectPoolComponent Pool { get; set; }
}

public class ObjectPool<T>
{
    ConcurrentBag<T> pool = new ConcurrentBag<T>();

    public T Take()
    {
        pool.TryTake(out var item);
        return item;
    }

    public void Add(T item)
    {
        pool.Add(item);
    }
}
