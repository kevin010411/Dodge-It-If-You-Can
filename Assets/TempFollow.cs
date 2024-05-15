using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class TempFollow : MonoBehaviour
{

    public Transform Player;

    // Start is called before the first frame update
    void Start()
    {
        DialogEvent dialogEvent = new DialogEvent();
        dialogEvent.UITemplateName = "DialogTemplate";
        dialogEvent.DialogText = "Well, you finally came to desert.";
        dialogEvent.EndDelaySeconds = 0f;
        EventSeries Evs = new EventSeries();
        Evs.PushBack(dialogEvent);
        DialogEvent ev2 = new DialogEvent();
        ev2.UITemplateName = "DialogTemplate";
        ev2.DialogText = "Are you ready for the challenge?";
        Evs.PushBack(ev2);

        Evs.LinearTrigger();
    }

    // Update is called once per frame
    void Update()
    {
        transform.position = new Vector3(Player.position.x, transform.position.y, transform.position.z);
    }
}
