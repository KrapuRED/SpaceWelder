using System;

#region Custom Events
public class CustomEvents
{
    private event Action _action = delegate { };

    public void Invoke()
    {
        _action?.Invoke();
    }

    public void AddListener(Action listener)
    {
        _action += listener;
    }
    public void RemoveListener(Action listener)
    {
        _action += listener;
    }
}

public class CustomEvents<T>
{
    private event Action<T> _action = delegate { };
    public void Invoke(T arg)
    {
        _action?.Invoke(arg);
    }

    public void AddListener(Action<T> listener)
    {
        _action += listener;
    }
    public void RemoveListener(Action<T> listener)
    {
        _action += listener;
    }
}

public class CustomEvents<T1, T2>
{
    private event Action<T1, T2> _action = delegate { };
    public void Invoke(T1 arg1, T2 arg2)
    {
        _action?.Invoke(arg1, arg2);
    }

    public void AddListener(Action<T1, T2> listener)
    {
        _action += listener;
    }
    public void RemoveListener(Action<T1, T2> listener)
    {
        _action += listener;
    }
}

public class CustomEvents<T1, T2, T3>
{
    private event Action<T1, T2, T3> _action = delegate { };
    public void Invoke(T1 arg1, T2 arg2, T3 arg3)
    {
        _action?.Invoke(arg1, arg2, arg3);
    }

    public void AddListener(Action<T1, T2, T3> listener)
    {
        _action += listener;
    }
    public void RemoveListener(Action<T1, T2, T3> listener)
    {
        _action += listener;
    }
}
#endregion

public class GlobalEvents 
{
    public static readonly CustomEvents<string> OnHullBeenReapir = new();

    //=============================== UI ================================================//
    public static readonly CustomEvents<float> OnUpdateHealthRobotUI = new();
    public static readonly CustomEvents<float, float> OnProgressDestinationUI = new();
    public static readonly CustomEvents<float> OnProgressTimeDestinationUI = new();
    public static readonly CustomEvents<float> OnShipEffciencyUI = new();
    public static readonly CustomEvents<string> OnMissionControlDialogue = new();
}
