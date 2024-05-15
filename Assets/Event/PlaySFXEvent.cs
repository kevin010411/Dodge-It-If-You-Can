using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class PlaySFXEvent : BaseEvent
{
    public string SFXName;
    public override void TriggerEvent()
    {
        if (ConditionalEnd)
        {
            EndDelaySeconds = SFXManager.Instance.GetSFXLength(SFXName);
        }
        SFXManager.Instance.HandleEvent(this);
    }
    public override bool IsEndConditionReached()
    {
        return true;
    }
}
