using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using DevUtils;
using System.Linq;
using Unity.VisualScripting;
using System.Globalization;

public class SFXManager : MonoBehaviour
{
    public static SFXManager Instance;

    private Dictionary<string, AudioSource> LaunchPad;

    public bool RunInEventMode;

    [Header("Must Be Filled")]
    [SerializeField] private string CustomSFXFolderPath;
    [Header("This Field Has Higher Priority")]
    [SerializeField] private TextAsset SFXChartInJson;
    [Header("Launchpad Analysis (ReadOnly)")]
    [SerializeField] private List<string> RegisteredSoundEffects;

    [SerializeField] private string CustomFilePath;
    [SerializeField] private SFXChart MainSFXChart;
    [SerializeField] private int CommandPointer;
    [SerializeField] private float CurrentTimePoint;
    // Awake is called before Start
    void Awake()
    {
        CommandPointer = 0;
        CurrentTimePoint = 0f;
        LaunchPad = new Dictionary<string, AudioSource>();
        RegisteredSoundEffects = new List<string>();
        MainSFXChart = new SFXChart();

        if(CustomSFXFolderPath == string.Empty)
        {
            Debug.LogError("Necessary parameter lost");
        }

        if(CustomFilePath == string.Empty && SFXChartInJson == null)
        {
            Debug.Log("SFXManager will run in EventMode");
            Instance = this;
            RunInEventMode = true;
            return;
        }
        else if(SFXChartInJson == null) {
            SFXChartInJson = Resources.Load<TextAsset>(CustomFilePath);

            if(SFXChartInJson == null)
            {
                Debug.Log("Given path doesn't direct to a json file.");
                RunInEventMode = true;
                return;
            }
        }

        MainSFXChart = JsonControllerV2.ToSFXChart(SFXChartInJson);
        MainSFXChart.SFXLaunchCommands
            = MainSFXChart.SFXLaunchCommands.OrderBy(Command => Command.LaunchTime).ToList();

        foreach (SFXLaunchCommand _Command in MainSFXChart.SFXLaunchCommands)
        {
            AudioClip _AudioClip = Resources.Load<AudioClip>($"{CustomSFXFolderPath}/{_Command.SFXName}");

            if (_AudioClip == null)
            {
                Debug.LogError($"Error! {_Command.SFXName} is not a correct AudioClip filename");
            }
            else
            {
                AudioSource _Source = gameObject.AddComponent<AudioSource>();
                _Source.clip = _AudioClip;

                string _RealName = __RemoveUnecessarySuffix(_AudioClip.name);
                LaunchPad.Add(_RealName, _Source);
                RegisteredSoundEffects.Add(_RealName);
            }
        }

        Debug.Log($"count = {MainSFXChart.SFXLaunchCommands.Count}");
        Debug.Log($"Object Inspection {MainSFXChart}");

        Instance = this;
    }

    // Update is called once per frame
    void Update()
    {
        if (!RunInEventMode)
        {
            while (CommandPointer < MainSFXChart.SFXLaunchCommands.Count
                && CurrentTimePoint >= MainSFXChart.SFXLaunchCommands[CommandPointer].LaunchTime)
            {
                LaunchPad[MainSFXChart.SFXLaunchCommands[CommandPointer].SFXName].Play();
                CommandPointer++;
            }
        }
    }

    void ReceiveCurrentTimePoint(float TrackProgress)
    {
        CurrentTimePoint = TrackProgress;
    }

    /// <summary>
    /// This function erases unecessary suffix after a whitespace of a "name" attribute
    /// of AudioClip. Which makes the processed string become a use-able filename
    /// string for my Launchpad dictionary name.
    /// </summary>
    /// <param name="ObjectNameAttribute"></param>
    /// <returns></returns>
    private string __RemoveUnecessarySuffix(string ObjectNameAttribute)
    {
        string _ProcessedString = "";
        foreach (char _Character in ObjectNameAttribute)
        {
            if(_Character == ' ')
            {
                break;
            } else _ProcessedString += _Character;
        }
        return _ProcessedString;
    }

    // SFX Manager - API 
    public bool DynamicLoadSFX(string SFXName)
    {
        AudioClip TargetSFX = Resources.Load<AudioClip>($"{CustomSFXFolderPath}/{SFXName}");
        if(TargetSFX == null)
        {
            Debug.Log($"{SFXName} is not an audio file.");
            return false;
        }
        AudioSource TargetSource = gameObject.AddComponent<AudioSource>();

        TargetSource.clip = TargetSFX;
        string ClipRealName = __RemoveUnecessarySuffix(TargetSFX.name);

        LaunchPad.Add(ClipRealName, TargetSource);
        RegisteredSoundEffects.Add(ClipRealName);
        return true;
    }

    public bool IsAudioAvailable(string SFXName)
    {
        return LaunchPad.ContainsKey(SFXName);
    }

    public bool PlaySFX(string SFXName)
    {
        if (!IsAudioAvailable(SFXName) && !DynamicLoadSFX(SFXName))
        {
            return false;
        }

        if (IsSFXPlaying(SFXName))
        {
            StartCoroutine(PlaySFXParllelly(LaunchPad[SFXName].clip));
        }
        else
        {
            LaunchPad[SFXName].Play();
        }
        return true;
    }

    private IEnumerator PlaySFXParllelly(AudioClip TargetAudio)
    {
        AudioSource _AudioSource = gameObject.AddComponent<AudioSource>();
        _AudioSource.clip = TargetAudio;
        _AudioSource.Play();

        yield return new WaitWhile(() => _AudioSource.isPlaying);
        Destroy(_AudioSource);
    }

    public bool IsSFXPlaying(string SFXName)
    {
        return LaunchPad[SFXName].isPlaying;
    }

    public float GetSFXLength(string SFXName)
    {
        if (!IsAudioAvailable(SFXName) && !DynamicLoadSFX(SFXName)) 
        {
            return 0f;
        }
        return LaunchPad[SFXName].clip.length;
    }

    // Event Handler (Request Receiver)
    public void HandleEvent(PlaySFXEvent TargetEvent)
    {
        if (!PlaySFX(TargetEvent.SFXName))
        {
            Debug.LogError($"[SFXManager]: unable to handle event({TargetEvent.EventName})");
            return;
        }
        Debug.Log($"({TargetEvent.EventName}) successfully handled!");
    }
}
