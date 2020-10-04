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
public class GoSignal : Signal {}
public class ButtonPressedSignal : Signal 
{
    public InputButton InputButton;

    public ButtonPressedSignal (InputButton inputButton)
    {
        this.InputButton = inputButton;
    }
}

public class ButtonReleasedSignal : Signal
{
    public InputButton InputButton;

    public ButtonReleasedSignal (InputButton inputButton)
    {
        this.InputButton = inputButton;
    }
}

public class MouseMovedSignal : Signal
{
    public float DeltaX;
    public float DeltaY;

    public MouseMovedSignal (float deltaX, float deltaY)
    {
        this.DeltaX = deltaX;
        this.DeltaY = deltaY;
    }
}

public class JumpStartedSignal : Signal {}
public class PlayerLandedSignal : Signal {}
public class TutorialCompleteSignal : Signal {}

public class FlyInFinishedSignal : Signal {}
public class FlailedToDeathSignal : Signal {}
public class PlayerCollidedWithPlanetSignal : Signal {}
public class IceCollectedSignal : Signal {}
