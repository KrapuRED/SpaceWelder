using System;
using System.Collections.Generic;

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
    private List<Action<T>> _listeners = new();
    public void AddListener(Action<T> listener) => _listeners.Add(listener);
    public void RemoveListener(Action<T> listener) => _listeners.Remove(listener);

    public void Invoke(T arg)
    {
        for (int i = _listeners.Count - 1; i >= 0; i--)
        {
            var listener = _listeners[i];

            // Check if the target object is still alive
            if (listener.Target is UnityEngine.Object unityObj && unityObj == null)
            {
                _listeners.RemoveAt(i); // auto-clean dead listeners
                continue;
            }

            listener.Invoke(arg);
        }
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
    public static readonly CustomEvents<int> OnChangeScene = new CustomEvents<int>();

    public static readonly CustomEvents<string> OnHullBeenReapir = new();

    public static readonly CustomEvents<UpgradeType, float> OnUpgradeRobot = new();
    public static readonly CustomEvents<int> OnApplyExtraBoomArm = new();
    public static readonly CustomEvents<float> OnApplySpeedUpgrade = new();
    public static readonly CustomEvents<float> OnApplyWeldingUpgrade = new();

    //=============================== UI ================================================//
    public static readonly CustomEvents<float> OnUpdateHealthRobotUI = new();
    public static readonly CustomEvents<float, float> OnProgressDestinationUI = new();
    public static readonly CustomEvents<float> OnProgressTimeDestinationUI = new();
    public static readonly CustomEvents<float> OnShipEffciencyUI = new();
    public static readonly CustomEvents<string> OnMissionControlDialogue = new();

    //=============================== Panel ================================================//
    public static readonly CustomEvents OnShowPanel = new();
}
