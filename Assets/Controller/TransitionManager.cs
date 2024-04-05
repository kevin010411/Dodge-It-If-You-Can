using System.Collections;
using System.Collections.Generic;
using Unity.Mathematics;
using UnityEngine;

public class TransitionManager : MonoBehaviour
{

    public bool EventPlayerDead = false;
    public bool EventTrackFinished = false;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {

        if (EventPlayerDead)
        {
            Debug.Log("Player has died");
            Application.Quit();
        } else if (EventTrackFinished)
        {
            Debug.Log("Track has finished");
        }

    }

    /// <summary>
    /// Broadcast TriggerPlayerDead will invoke procedures to handle the event
    /// which player is dead.
    /// </summary>
    public void TriggerPlayerDead()
    {
        Debug.Log("Player dead? ==> " + true);
        EventPlayerDead = true;
    }

    /// <summary>
    /// Broadcast TriggerTrackFinished will invoke procedures to handle the 
    /// event which the level track has finished playing.
    /// </summary>
    public void TriggerTrackFinished()
    {
        Debug.Log("Track finished? ==> " + true);
        EventTrackFinished = true;
    }
}
