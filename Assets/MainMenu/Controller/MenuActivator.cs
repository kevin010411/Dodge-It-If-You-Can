using JetBrains.Annotations;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Playables;
using UnityEngine.Timeline;

public class MenuActivator : MonoBehaviour
{
    public PlayableDirector IntroCutscene;
    public GameObject[] ToBeDeactivated;
    public GameObject[] ToBeActivated;
    // Start is called before the first frame update
    void Awake()
    {
        foreach (GameObject Obj in ToBeActivated) { Obj.SetActive(false); }
        StartCoroutine(HandleActivatingTiming((float)IntroCutscene.duration));
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public IEnumerator HandleActivatingTiming(float Duration)
    {
        yield return new WaitForSeconds(Duration);
        foreach (GameObject Obj in ToBeActivated) { Obj.SetActive(true); }
        foreach (GameObject Obj in ToBeDeactivated) { Obj.SetActive(false); }
    } 
}
