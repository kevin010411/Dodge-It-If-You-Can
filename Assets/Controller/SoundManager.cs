using DevUtils;
using System;
using System.Collections;
using System.IO;
using UnityEngine;
using UnityEngine.Audio;
using NLayer;

public class SoundManager : MonoBehaviour
{
    [SerializeField] private AudioClip LevelTrack;
    [SerializeField] private AudioSource AudioSource;
    [SerializeField] private bool Pause = false;
    [SerializeField][Range(0f, 1f)] private float MusicVolume; 
    [SerializeField][Range(0f, 1f)] private float PlayingStartTime = 0;
    //[HideInInspector] 
    public float TrackLength;
    [HideInInspector] public float TrackProgress;

    // Start is called before the first frame update
    void Awake()
    {
        if(AudioSource == null)
        {
            this.AudioSource = gameObject.AddComponent<AudioSource>();
        }
        AudioSource.clip = LevelTrack;
        TrackLength = AudioSource.clip.length;
        TrackProgress = TrackLength * PlayingStartTime;
        AudioSource.time = TrackProgress;
        AudioSource.Play();
    }

    // Update is called once per frame
    void Update()
    {
        if (Pause) { 
            StartCoroutine(HandlePauseEvent()); 
        }
        else
        {
            TrackProgress = AudioSource.time;
        }

        SendMessage(FunctionNames.ReceiveCurrentTimePoint, TrackProgress);
        this.AudioSource.volume = MusicVolume;
	}

	public void PauseMusic()
	{
        Pause = true;
	}

	public void PlayMusic()
	{
        Pause = false;
	}

    private IEnumerator HandlePauseEvent()
    {
        AudioSource.Pause();
        yield return new WaitWhile(() => Pause);
        AudioSource.UnPause();
    }

    public void SetStartTime(float Time, bool DoResetStage = true)
    {
        this.AudioSource.time = Time;
	}
    
    public void ChangeTrack(string AudioPath)
    {
        Pause = true;
        var TargetFileStream = File.OpenRead(AudioPath);
        AudioClip AudioClipToChange = FromMp3Stream(TargetFileStream);
        LevelTrack = AudioClipToChange;
        this.AudioSource.clip = AudioClipToChange;
		TrackLength = AudioSource.clip.length;
		this.AudioSource.Play();
	}

    private AudioClip FromMp3Stream(Stream Mp3Stream)
    {
        var Mp3Reader = new MpegFile(Mp3Stream);
        var Samples = new float[Mp3Reader.Length / sizeof(float)];
        Mp3Reader.ReadSamples(Samples, 0, Samples.Length);

        AudioClip NewlyCreatedAudioClip =
            AudioClip.Create(
                "LoadedMp3",
                Samples.Length / Mp3Reader.Channels,
                Mp3Reader.Channels,
                Mp3Reader.SampleRate,
                false);

        NewlyCreatedAudioClip.SetData(Samples, 0);
        return NewlyCreatedAudioClip;
    }
}
