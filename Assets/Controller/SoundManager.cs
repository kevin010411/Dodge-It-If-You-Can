using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using UnityEngine;
using UnityEngine.Audio;

public class SoundManager : MonoBehaviour
{

    [HideInInspector]
    public float TrackLength;//���ܦ�public�BHideInSpector
    public Track LevelTrack;

    [SerializeField]
    public float TrackProgress = 0;

    [Range(0f, 1f)]
    public float PlayingStartTime = 0;

    // Start is called before the first frame update
    void Awake()
    {
        // Configuration of AudioSource
        LevelTrack.LevelTrackSource = gameObject.AddComponent<AudioSource>();
        LevelTrack.LevelTrackSource.clip = LevelTrack.LevelTrackClip;
        LevelTrack.LevelTrackSource.volume = LevelTrack.TrackVolume;
        // Get track length
        TrackLength = LevelTrack.LevelTrackClip.length;
        // Play
        LevelTrack.LevelTrackSource.time = TrackLength * PlayingStartTime;
        LevelTrack.LevelTrackSource.Play();
    }

    // Update is called once per frame
    void Update()
    {

        // Updating time progress each frame
        TrackProgress = LevelTrack.LevelTrackSource.time;
        // Immediately changeable volume
        LevelTrack.LevelTrackSource.volume = LevelTrack.TrackVolume;

    }

    /*
     * �U���O�ŸԶ��s�W��function
     */
	public void PauseMusic()
	{
		LevelTrack.LevelTrackSource.Pause();
	}
	public void PlayMusic()
	{
		LevelTrack.LevelTrackSource.Play();
	}
}
