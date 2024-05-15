using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class TestKeyDownEvent : BaseEvent
{
    public override void TriggerEvent()
    {
        Debug.Log("Event Triggered");
    }
    public override bool IsEndConditionReached()
    {
        return Input.GetKey(KeyCode.Space);
    }
}
