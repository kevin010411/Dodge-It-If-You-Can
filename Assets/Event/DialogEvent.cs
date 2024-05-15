using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DialogEvent : BaseEvent
{
    public string UITemplateName;
    public string DialogText;
    public override void TriggerEvent()
    {
        ConditionalEnd = true;
        ForceSkipOneFrame = true;

        StageUIManager.Instance.HandleEvent(this); 
    }

    public override bool IsEndConditionReached()
    {
        return Input.GetKeyDown(KeyCode.Space);
    }
}
