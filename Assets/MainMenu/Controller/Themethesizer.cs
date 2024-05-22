using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DevUtils;
using UnityEditor.PackageManager;
using UnityEngine.Audio;

public class Themethesizer : MonoBehaviour
{
    [Header("Required")] public AudioSource AudioSource;
    [SerializeField] private bool AutoAttachSource = false;
    [SerializeField] private bool FadeIn = false;
    [SerializeField] [Range(0f, 1f)] [Header("Volume Before Fading")] private float Before;
    [SerializeField] [Range(0f, 1f)] [Header("Volume After Fading")] private float After;
    [SerializeField] private float FadeInDuration;
    void Awake()
    {
        if (AutoAttachSource)
        {
            this.AudioSource = GetComponent<AudioSource>();
        }

        if (FadeIn)
        {
            ApplySmoothFade(Before, After, FadeInDuration);
        }
    }

    void Update()
    {
        
    }

    public void ApplySmoothFade(float FromVolume, float ToVolume, float Duration)
    {
        if(FromVolume < 0f || FromVolume > 1f)
        {
            ErrorManagement.Err("OutOfRange", this.name, $"{FromVolume} is not between [0 ~ 1]");
        } else if (ToVolume < 0f || ToVolume > 1f)
        {
            ErrorManagement.Err("OutOfRange", this.name, $"{ToVolume} is not between [0 ~ 1]");
        }

        StartCoroutine(HandleSmoothFadeEffect(FromVolume, ToVolume, Duration));
    }

    private IEnumerator HandleSmoothFadeEffect(float FromVolume, float ToVolume, float Duration)
    {
        float Timer = 0f;

        while(Timer < Duration)
        {
            Timer += Time.deltaTime;
            this.AudioSource.volume = Mathf.Lerp(FromVolume, ToVolume, Timer / Duration);
            yield return null;
        }
        this.AudioSource.volume = ToVolume;
    }
}
