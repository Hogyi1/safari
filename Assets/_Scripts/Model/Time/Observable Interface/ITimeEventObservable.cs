using System.Collections.Generic;
using UnityEngine;

public interface ITimeEventObservable
{
    public void AddObserver(ITimeEventObserver observer);
    public void RemoveObserver(ITimeEventObserver observer);
    public void NotifyObservers(TimeEvent timeEvent);
}
