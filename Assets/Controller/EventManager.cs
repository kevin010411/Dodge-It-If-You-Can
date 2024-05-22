using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EventManager : MonoBehaviour
{
    public static EventManager Instance;

    // Start is called before the first frame update
    void Awake()
    {
        Instance = this;
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void TriggerEventSeries(EventSeries TargetEventSeries)
    {
        StartCoroutine(HandleEventTransition(TargetEventSeries));
    }

    private IEnumerator HandleEventTransition(EventSeries TargetEventSeries)
    {
        foreach (var _Event in TargetEventSeries.EventList)
        {
            _Event.TriggerEvent();

            if (_Event.ConditionalEnd)
            {
                yield return new WaitUntil(() => _Event.IsEndConditionReached());
            }

            if (_Event.ForceSkipOneFrame)
            {
                yield return null;
            }

            if(_Event.EndDelaySeconds > 0f)
            {
                yield return new WaitForSeconds(_Event.EndDelaySeconds);
            }

            Debug.Log("Passed End Conditions.");
        }
    }

    public void TriggerSingleEvent(BaseEvent TargetEvent)
    {
        TargetEvent.TriggerEvent();
    }
}
