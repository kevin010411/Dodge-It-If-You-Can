using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UIElements;

public class UIManager : MonoBehaviour
{
    private GameObject StageController;
    private UIDocument UI;

    private VisualElement rootElement;
    private Slider MusicSlider;
    private Label StartTimeLabel;
    private Button StartButton;
    private Button PauseButton;
    // Start is called before the first frame update
    void Start()
    {
        StageController = GameObject.Find("/StageController");
        if (StageController == null)
            Debug.LogWarning("沒找到StageController");
        SoundManager soundManager = StageController.GetComponent<SoundManager>();
        StageManager stageManager = StageController.GetComponent<StageManager>();


		UI = GetComponent<UIDocument>();
        rootElement = UI.rootVisualElement;
		MusicSlider = rootElement.Q<Slider>("MusicSlider");
        StartTimeLabel = rootElement.Q<Label>("nowStartTime");
        StartButton = rootElement.Q<Button>("StartButton");
        StartButton.RegisterCallback<ClickEvent>((button) =>
        {
            soundManager.PlayMusic();
        });
        PauseButton = rootElement.Q<Button>("PauseButton");
        PauseButton.RegisterCallback<ClickEvent>((button) =>
        {
            Debug.Log(button.ToString());
            soundManager.PauseMusic();
        });
		MusicSlider.lowValue = 0;
        MusicSlider.highValue = 1;
        MusicSlider.RegisterValueChangedCallback((slide) =>
        {
            TimeSpan time = new TimeSpan(0,0,Convert.ToInt32(slide.newValue * soundManager.TrackLength));
            StartTimeLabel.text = $"現在時間:{time.Minutes:D2}:{time.Seconds:D2}";
        });
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
