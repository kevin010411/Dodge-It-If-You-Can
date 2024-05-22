using JetBrains.Annotations;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Playables;
using UnityEngine.Timeline;
using UnityEngine.UIElements;
using UnityEngine.XR;

public class MenuActivator : MonoBehaviour
{
    [SerializeField] private PlayableDirector IntroCutscene;
    [SerializeField] private GameObject[] ToBeDeactivated;
    [SerializeField] private GameObject[] ToBeActivated;
    [SerializeField] private AudioSource ThemePlayerSource;
    [SerializeField] private GameObject IntroAnimationSkipHint;
    [SerializeField] private float SkipHintShowDuration;
    private bool FinishIntro = false;
    private bool IsSkipHintLiving = true;
    private float Counter = 0f;
    // Start is called before the first frame update
    void Awake()
    {
        IntroAnimationSkipHint.SetActive(false);
        SetToBeActivated(false);
        StartCoroutine(HandleActivatingTiming((float)IntroCutscene.duration));
    }

    // Update is called once per frame
    void Update()
    {
        if(IsMouseMoving() && IsSkipHintLiving)
        {
            IntroAnimationSkipHint.SetActive(true);
            Counter = SkipHintShowDuration;
        } else if (IsSkipHintLiving)
        {
            Counter = Counter - Time.deltaTime < 0 ? 0f : Counter - Time.deltaTime;
            if(Counter <= 0f)
            {
                IntroAnimationSkipHint.SetActive(false);
            }
        }
    }

    private bool IsMouseMoving()
    {
        return Input.GetAxis("Mouse X") != 0 || Input.GetAxis("Mouse Y") != 0;
    }

    private IEnumerator HandleActivatingTiming(float Duration)
    {
        StartCoroutine(WaitForPlayableFinishPlaying(Duration));
        StartCoroutine(DetectIsSkipButtonPressed());

        yield return new WaitUntil(() => FinishIntro);

        IsSkipHintLiving = false;
        SetToBeActivated(true);
        SetToBeDeactivated(false);
    } 

    private IEnumerator WaitForPlayableFinishPlaying(float Duration)
    {
        yield return new WaitForSeconds(Duration);
        FinishIntro = true;
    }

    private IEnumerator DetectIsSkipButtonPressed()
    {
        yield return new WaitUntil(() => Input.GetKeyDown(KeyCode.Space));
        FinishIntro = true;

        if(IntroCutscene.time < IntroCutscene.duration)
        {
            IntroCutscene.Stop();
            GlitchController.Instance.SetAnalogParameters(Vector4.zero);
            GlitchController.Instance.SetDigitalIntensity(0);
            ThemePlayerSource.time = (float)IntroCutscene.duration;
        }
    }

    private void SetToBeActivated(bool IsTrue)
    {
        foreach(GameObject Obj in ToBeActivated) { Obj.SetActive(IsTrue); }
    }

    private void SetToBeDeactivated(bool IsTrue)
    {
        foreach(GameObject Obj in ToBeDeactivated) { Obj.SetActive(IsTrue); }
    }
}
