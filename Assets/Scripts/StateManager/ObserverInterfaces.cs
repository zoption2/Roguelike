using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IObservable
{
    public void AddObserver(IObserver observer);
    public void RemoveObserver(IObserver observer);
}

public interface IObserver
{
    public void Notify();
}
