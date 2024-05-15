using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class EventSeries
{
    public List<BaseEvent> EventList;

    public EventSeries() { EventList = new List<BaseEvent>(); }

    public void LinearTrigger()
    {
        EventManager.Instance.TriggerEventSeries(this);
    }

    public void PushBack(BaseEvent TargetEvent)
    {
        EventList.Add(TargetEvent);
    }
}
