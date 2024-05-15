using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioVisualizer : MonoBehaviour
{
    public AudioSource VisualizingSource;
    private float PositionPortion;
    private float CameraHalfWidth;
    private float CameraHalfHeight;

    // must provide
    public int RectAmount;
    public int CaptureAmount;
    public float ScaleXOffset;
    public float ScaleMultiplier;
    public GameObject RectPrefab;
    public Camera TargetCamera;

    private float[] AudioSpectrumSamples;
    private GameObject[] VisualizedRects;
    // Start is called before the first frame update
    void Start()
    {
        if (VisualizingSource == null)
        {
            VisualizingSource = GetComponent<AudioSource>();
        }
        AudioSpectrumSamples = new float[RectAmount];
        VisualizedRects = new GameObject[CaptureAmount];

        CameraHalfHeight = TargetCamera.orthographicSize;
        CameraHalfWidth = TargetCamera.orthographicSize * TargetCamera.aspect;

        PositionPortion = CameraHalfWidth * 2 / (float)CaptureAmount;
        float CurrentPosition = -1f * CameraHalfWidth + (PositionPortion / 2);
        Debug.Log($"PositionPortion={PositionPortion}");

        for(int i = 0; i < VisualizedRects.Length; i++)
        {
            VisualizedRects[i] = Instantiate(RectPrefab);
            VisualizedRects[i].transform.position = new Vector3(CurrentPosition, -1f * CameraHalfHeight, 0f);
            VisualizedRects[i].transform.localScale = new Vector3(PositionPortion, 1f);
            CurrentPosition += PositionPortion;
        }
    }

    // Update is called once per frame
    void Update()
    {
        VisualizingSource.GetSpectrumData(AudioSpectrumSamples, 0, FFTWindow.Blackman);

        for(int i = 0; i < VisualizedRects.Length; i++)
        {
            if (VisualizedRects[i] != null)
            {
                VisualizedRects[i].transform.localScale = 
                    new Vector3(PositionPortion - ScaleXOffset, ScaleMultiplier * AudioSpectrumSamples[i] + 1f);
            }
        }
    }
}
