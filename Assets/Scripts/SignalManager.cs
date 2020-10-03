using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class SignalManager : MonoBehaviour
{
    public static SignalManager Inst;

    private Dictionary<Type, Action<Signal>> listeners = new Dictionary<Type, Action<Signal>>();

    void Awake()
    {
        Inst = this;
    }

    public void AddListener<T>(Action<Signal> callback) where T : Signal
    {
        if(listeners.ContainsKey(typeof(T)))
            listeners[typeof(T)] += callback;
        else
            listeners.Add(typeof(T), callback);
    }

    public void RemoveListener<T>(Action<Signal> callback) where T : Signal
    {
        listeners[typeof(T)] -= callback;
        if(listeners[typeof(T)] == null)
            listeners.Remove(typeof(T));
    }

    public void FireSignal(Signal signal)
    {
        if(listeners.ContainsKey(signal.GetType()))
            listeners[signal.GetType()](signal);
    }
}

public abstract class Signal {}
public class SignalTest : Signal 
{
    public string TestString;

    public SignalTest(string testString)
    {
        this.TestString = testString;
    }
}
