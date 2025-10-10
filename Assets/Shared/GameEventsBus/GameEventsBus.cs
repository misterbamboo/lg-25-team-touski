using System;
using System.Collections.Generic;
using UnityEngine;

public class GameEventsBus
{
    private static GameEventsBus _instance;
    public static GameEventsBus Instance
    {
        get
        {
            if (_instance == null)
            {
                _instance = new GameEventsBus();
            }
            return _instance;
        }
    }

    private Dictionary<Type, List<Delegate>> eventSubscribers = new Dictionary<Type, List<Delegate>>();

    // Subscribe to an event
    public void Subscribe<T>(Action<T> callback) where T : IGameEvent
    {
        Type eventType = typeof(T);

        if (!eventSubscribers.ContainsKey(eventType))
        {
            eventSubscribers[eventType] = new List<Delegate>();
        }

        eventSubscribers[eventType].Add(callback);
    }

    // Unsubscribe from an event
    public void Unsubscribe<T>(Action<T> callback) where T : IGameEvent
    {
        Type eventType = typeof(T);

        if (eventSubscribers.ContainsKey(eventType))
        {
            eventSubscribers[eventType].Remove(callback);
        }
    }

    // Publish an event
    public void Publish<T>(T eventData) where T : IGameEvent
    {
        Type eventType = typeof(T);

        if (eventSubscribers.ContainsKey(eventType))
        {
            foreach (Delegate callback in eventSubscribers[eventType])
            {
                (callback as Action<T>)?.Invoke(eventData);
            }
        }
    }

    // Clear all subscribers
    public void Clear()
    {
        eventSubscribers.Clear();
    }
}

// Base interface for all game events
public interface IGameEvent { }
