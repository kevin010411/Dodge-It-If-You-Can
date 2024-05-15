using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public abstract class BaseEvent
{
    public string EventName;
    public bool ConditionalEnd = false;
    public bool ForceSkipOneFrame = false;
    public float EndDelaySeconds = 0f;
    public abstract void TriggerEvent();
    public abstract bool IsEndConditionReached();
}
