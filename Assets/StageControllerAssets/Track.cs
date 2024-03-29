using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;

[System.Serializable]
public class Track
{
    [SerializeField]
    public string TrackIdentifier;

    public AudioClip LevelTrackClip;

    [HideInInspector]
    public AudioSource LevelTrackSource;

    [Range(0f, 1f)]
    public float TrackVolume;

    // further attributes can be added.
}
