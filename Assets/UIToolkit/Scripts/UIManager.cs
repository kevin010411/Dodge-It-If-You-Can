using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using Unity.VisualScripting;
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;
using SFB;
using UnityEngine.SceneManagement;
using EditorScene;
using System.ComponentModel;

public class UIManager : MonoBehaviour
{
    private GameObject StageController;
    private UIDocument UI;
    private VisualElement rootElement;
    private SoundManager soundManager;
    private StageManager stageManager;
	private Slider MusicSlider;
    private ObjectInfoWindow objectInfoWindow;
    private Button PauseButton;

	private void _FindStageController()
    {
		StageController = GameObject.Find("/StageController");
		if (StageController == null)
			Debug.LogWarning("沒找到StageController");
		soundManager = StageController.GetComponent<SoundManager>();
		stageManager = StageController.GetComponent<StageManager>();
	}
    private void _SetPlayButton()
    {
        Button StartButton = rootElement.Q<Button>("StartButton");
        StartButton.RegisterCallback<ClickEvent>((button) =>
        {
            enabled = true;
            soundManager.PlayMusic();
        });
    }
    private void _SetPauseButton()
    {
        PauseButton = rootElement.Q<Button>("PauseButton");
        PauseButton.RegisterCallback<ClickEvent>((button) =>
        {
            ClickPasueButton();
		});
    }
    private void _SetSettingPannel()
    {
        Button SettingButton = rootElement.Q<Button>("SettingButton");
        VisualElement SettingPannel = rootElement.Q<VisualElement>("SettingView");
        VisualElement FootPannel = rootElement.Q<VisualElement>("FootComponent");
        SettingButton.RegisterCallback<ClickEvent>((button) =>
        {
            if (SettingPannel.style.left == new StyleLength(Length.Percent(0)))
            {
                SettingPannel.style.left = new(Length.Percent(-100));
                FootPannel.style.opacity = 1;
            }
            else
            {
                SettingPannel.style.left = new(Length.Percent(0));
				soundManager.PauseMusic();
                FootPannel.style.opacity = 0;
                CloseObjectInfoWindow();
			}
		});

        Button SetMusicButton = rootElement.Q<Button>("SetMusicButton");
        SetMusicButton.RegisterCallback<ClickEvent>(button =>
        {
            string[] folderPaths = StandaloneFileBrowser.OpenFilePanel("Select Folder", "", "mp3", false);
            if (folderPaths != null && folderPaths.Length > 0)
            {
                string selectedFolderPath = folderPaths[0];
                Debug.Log("Selected Folder: " + selectedFolderPath);
                TextField musicPathFild = rootElement.Q<TextField>("MusicPath");
                musicPathFild.value = selectedFolderPath;
			}
            else
            {
                Debug.Log("No folder selected.");
            }
        });

        TextField stageDesribe = rootElement.Q<TextField>("SetDescriptionRow");
		stageDesribe.SetVerticalScrollerVisibility(ScrollerVisibility.AlwaysVisible);

        Button SaveButton = rootElement.Q<Button>("SaveButton");
        TextField MusicPath = rootElement.Q<TextField>("MusicPath");
        TextField SetLifeRow = rootElement.Q<TextField>("SetLifeRow");
        TextField SetDescriptionRow = rootElement.Q<TextField>("SetDescriptionRow");
        TextField StageName = rootElement.Q<TextField>("StageName");

		StageManager stageManager = GameObject.Find("StageController").GetComponent<StageManager>();
		SaveButton.RegisterCallback<ClickEvent>(evt =>
        {
			//TODO 檢測是否符合標準
			StageDescription Data = new StageDescription(MusicPath.value, int.Parse(SetLifeRow.value),
														StageName.value, SetDescriptionRow.value);
            stageManager.SaveStageInfo("Desert", StageName.value);
			// TODO 偵測StageName,SubStageName,fileName或要求用戶輸入
			JsonController.SaveStageDescription(Data,"Desert", StageName.value);
        });

        Button BackHomeButton = rootElement.Q<Button>("BackHomeButton");
        BackHomeButton.RegisterCallback<ClickEvent>(evt =>
        {
            // TODO 檢查是否需要更新
            StageDescription Data = new StageDescription(MusicPath.value, int.Parse(SetLifeRow.value),
                                                        StageName.value, SetDescriptionRow.value);
            if(JsonController.isNeedSaveStageDescription(Data, "Desert", "Test"))
            {
                CheckLeaveWindow checkLeaveWindow = new CheckLeaveWindow();
                rootElement.Add(checkLeaveWindow);
                checkLeaveWindow.Confirmed += _GoMainMenu;
                checkLeaveWindow.Canceled += () => { rootElement.Remove(checkLeaveWindow); };
				return;
            }
			_GoMainMenu();
		});
	}
    private void _GoMainMenu()
    {
		SceneManager.LoadScene("MainMenu");
	}

	private void _SetPlayerEnableButton()
    {
        Toggle toggle = rootElement.Q<Toggle>("TogglePlayerEnable");
        GameObject player = GameObject.Find("Player");
		toggle.RegisterCallback<ChangeEvent<bool>>((button) =>
        {
			player.SetActive(toggle.value);
        });
    }
    private void _SetMusicSlider()
    {
        Label StartTimeLabel = rootElement.Q<Label>("nowStartTime");
		MusicSlider = rootElement.Q<Slider>("MusicSlider");
		MusicSlider.lowValue = 0;
		MusicSlider.highValue = 1;
		MusicSlider.RegisterValueChangedCallback((slide) =>
		{
			float nowStartTime = slide.newValue * soundManager.TrackLength;
			TimeSpan time = new TimeSpan(0, 0, Convert.ToInt32(nowStartTime));
			StartTimeLabel.text = $"現在時間:{time.Minutes:D2}:{time.Seconds:D2}";
		});
		MusicSlider.RegisterCallback<MouseCaptureEvent>((obj) =>
		{
			enabled = false;
			soundManager.PauseMusic();
		});
		MusicSlider.RegisterCallback<MouseCaptureOutEvent>((obj) =>
		{
			float nowStartTime = MusicSlider.value * soundManager.TrackLength;
			soundManager.SetStartTime(nowStartTime);
		});
	}

    private VisualElement _CreateStageListView(string rootDir)
    {
        Foldout foldout = new Foldout();
        foldout.text = rootDir.Substring(rootDir.LastIndexOf('/') + 1);
        int child = 0;
        foreach(string childDir in Directory.GetDirectories(rootDir))
        {
			VisualElement list = _CreateStageListView(childDir);
            if(list != null)
				child++;
			foldout.Add(list);
		}
        if(child>0)
			return foldout;
		string[] files = Directory.GetFiles(rootDir, "StageChart.json");
        if(files.Length == 1)
        {
            string FileDir = files[0];
			Button button = new Button();
			string fileName = rootDir.Substring(rootDir.LastIndexOf('\\') + 1);
			button.text = fileName;
			button.name = fileName;
			button.style.backgroundColor = new Color(0, 0, 0, 0);
			button.RegisterCallback<ClickEvent>((mouse) => {
				_SetStageDescription(JsonController.LoadStageDescription(rootDir + "/StageDescription.json"));
				stageManager.LoadStageInfo(FileDir);
			});
            return button;
		}
        return null;
    }

    private void _SetStageChartList()
    {
        ScrollView container = rootElement.Q<ScrollView>("StageChartList");
        string StageDir = Application.dataPath + "/Resources/StageChart";
        string DesertStageDir = Application.dataPath + "/Resources/Desert";
        List<String> jsonFile = new List<string>();
        jsonFile.AddRange(Directory.GetFiles(StageDir,"test*.json"));
        foreach (string file in jsonFile)
        {
            Button button = new Button();
            string fileName = file.Substring(file.LastIndexOf('\\') + 1);
            button.text = fileName;
            button.name = fileName;
            button.style.backgroundColor = new Color(0, 0, 0, 0);
            button.RegisterCallback<ClickEvent>((mouse) =>
            {
                //Debug.Log(fileName);
                stageManager.LoadStageInfo(file);
            });
            container.hierarchy.Add(button);
        }
		container.Add(_CreateStageListView(DesertStageDir));
	}
	
    private void _SetStageDescription(StageDescription data)
    {
		TextField MusicPath = rootElement.Q<TextField>("MusicPath");
		TextField SetLifeRow = rootElement.Q<TextField>("SetLifeRow");
		TextField SetDescriptionRow = rootElement.Q<TextField>("SetDescriptionRow");
		TextField StageName = rootElement.Q<TextField>("StageName");
        MusicPath.value = data.MusicName;
        SetLifeRow.value = data.TotalLife.ToString();
        SetDescriptionRow.value = data.Description;
        StageName.value = data.Name;
	}

    public ObjectInfoWindow CreateObjectInfoWindow()
    {
        if(objectInfoWindow!=null)
        {
            objectInfoWindow.RemoveFromHierarchy();
            objectInfoWindow = null;
        }
		objectInfoWindow = new ObjectInfoWindow();
        rootElement.Insert(rootElement.childCount - 2,objectInfoWindow);
        return objectInfoWindow;
    }

    public void CloseObjectInfoWindow()
    {
		if (objectInfoWindow != null)
			objectInfoWindow.RemoveFromHierarchy();
	}

    private void _SetEditorTabGroup()
    {
        EditorTabGroup tabGroup = rootElement.Q<EditorTabGroup>("EditorTab");
        tabGroup.SetStageManager(stageManager);
	}

	public void ClickPasueButton()
    {
		//和PasueButton做一樣的是，希望後面可以改成真的點擊
		enabled = false;
		soundManager.PauseMusic();
	}

	void Start()
    {
        _FindStageController();
		UI = GetComponent<UIDocument>();
        rootElement = UI.rootVisualElement;
		
        
        _SetPlayButton();
        _SetPauseButton();
        _SetSettingPannel();
		_SetPlayerEnableButton();
		_SetMusicSlider();
        _SetStageChartList();
        _SetEditorTabGroup();
	}
	// Update is called once per frame
	void Update()
    {
        MusicSlider.value = (soundManager.TrackProgress / soundManager.TrackLength);

	}
}
