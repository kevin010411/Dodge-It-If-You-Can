using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEditor;

public class SceneLoadingController : MonoBehaviour
{
    // Global Instance
    public static SceneLoadingController Instance;

    // public members
    public bool PlayOnStart;
    public bool DontLoadScene;
    public bool TriggerByParameter;
    public bool TransitionSFX;

    // SerializedFields
    [SerializeField] private Animator TargetTransition;
    [SerializeField] private string TargetSceneToLoad;
    [SerializeField] private int OptionalBuilderIndex;
    [SerializeField] private string TriggerParameter;
    [SerializeField] private string TriggerStateName;
    [SerializeField] private float Duration;
    [SerializeField] private string SFXName;
    // Start is called before the first frame update
    void Start()
    {
        if (PlayOnStart && DontLoadScene)
        {
            if (TransitionSFX)
            {
                PlaySFXEvent TransitionSFXEvent = new PlaySFXEvent();
                TransitionSFXEvent.EventName = "TransitionSFXEvent";
                TransitionSFXEvent.SFXName = SFXName;
                TransitionSFXEvent.TriggerEvent();
            }

            if (TriggerByParameter)
            {
                PlayTransitionByTriggerParameter();
            } 
        } else if (PlayOnStart)
        {
            if (TransitionSFX)
            {
                PlaySFXEvent TransitionSFXEvent = new PlaySFXEvent();
                TransitionSFXEvent.EventName = "TransitionSFXEvent";
                TransitionSFXEvent.SFXName = SFXName;
                TransitionSFXEvent.TriggerEvent();
            }

            if (TriggerByParameter)
            {
                LoadScene(TargetSceneToLoad, TargetTransition, Duration, TriggerParameter);
            }
        }

        Instance = this;
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    /// <summary>
    /// Conditional handling transition, can specify the animator trigger.
    /// </summary>
    /// <param name="SceneName"></param>
    /// <param name="Transition"></param>
    /// <param name="Duration"></param>
    /// <param name="Trigger"></param>
    public void LoadScene(string SceneName, Animator Transition, float Duration, string Trigger)
    {
        StartCoroutine(HandleTransition(SceneName, Transition, Duration, Trigger));
    }

    /// <summary>
    /// Conditional handling transition, besides from specifying animator trigger parameter,
    /// also can play sound effect within, But a SFXManagerEntity must exist in scene.
    /// </summary>
    /// <param name="SceneName"></param>
    /// <param name="Transition"></param>
    /// <param name="Duration"></param>
    /// <param name="Trigger"></param>
    /// <param name="SFXName"></param>
    public void LoadScene(
        string SceneName, Animator Transition, float Duration, string Trigger, string SFXName)
    {
        PlaySFXEvent TransitionSFXEventAPIVersion = new PlaySFXEvent();
        TransitionSFXEventAPIVersion.EventName = "TransitionSFXEventAPIVersion";
        TransitionSFXEventAPIVersion.SFXName = SFXName;
        TransitionSFXEventAPIVersion.TriggerEvent();

        StartCoroutine(HandleTransition(SceneName, Transition, Duration, Trigger));
    }

    /// <summary>
    /// Purely play the transition by setting the trigger, suitable in the situation of 
    /// switching from a action-less state to a state with action.
    /// </summary>
    /// <param name="SceneName"></param>
    /// <param name="Duration"></param>
    /// <returns></returns>
    private void PlayTransitionByTriggerParameter()
    {
        TargetTransition.SetTrigger(TriggerParameter);
    }

    IEnumerator HandleTransition(string SceneName, Animator Transition, float Duration, string Trigger)
    {
        Transition.SetTrigger(Trigger);
        yield return new WaitForSeconds(Duration);
        SceneManager.LoadScene(SceneName);
    }

}
