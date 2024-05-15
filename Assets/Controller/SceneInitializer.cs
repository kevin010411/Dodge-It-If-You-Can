using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SceneInitializer : MonoBehaviour
{
    public Animator Transition;
    public float Duration;
    // Start is called before the first frame update
    void Start()
    {
        Transition.SetTrigger("TriggerOpen");
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
