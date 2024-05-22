using System;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.UIElements;
using SFB;
using UnityEngine.SceneManagement;
using EditorScene;

public class UIManager : MonoBehaviour
{
    private GameObject StageController;
    private UIDocument UI;
    private VisualElement rootElement;
    private SoundManager soundManager;
    private StageManager stageManager;
	private Slider MusicSlider;
    private ObjectInfoWindow objectInfoWindow;
	private GameObject player;
	private EditorCamera editorCamera;
	private void _FindStageController()
    {
		StageController = GameObject.Find("/StageController");
		if (StageController == null)
			Debug.LogWarning("沒找到StageController");
		soundManager = StageController.GetComponent<SoundManager>();
		stageManager = StageController.GetComponent<StageManager>();
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
			ExtensionFilter[] extensions = new[]
			{
				 new ExtensionFilter("Sound File","mp3"),
			};
            string[] folderPaths = StandaloneFileBrowser.OpenFilePanel("Select Folder", "", extensions, false);
            if (folderPaths != null && folderPaths.Length > 0)
            {
                string selectedFolderPath = folderPaths[0];
                //Debug.Log("Selected Folder: " + selectedFolderPath);
                TextField musicPathFild = rootElement.Q<TextField>("MusicPath");
                musicPathFild.value = selectedFolderPath;
				soundManager.ChangeTrack(selectedFolderPath);
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
        IntegerField SetLifeRow = rootElement.Q<IntegerField>("SetLifeRow");
        TextField SetDescriptionRow = rootElement.Q<TextField>("SetDescriptionRow");
        TextField StageName = rootElement.Q<TextField>("StageName");

		StageManager stageManager = GameObject.Find("StageController").GetComponent<StageManager>();
		SaveButton.RegisterCallback<ClickEvent>(evt =>
        {
            //TODO 檢測是否符合標準
			StageDescription Data = new StageDescription(MusicPath.value, SetLifeRow.value,
														StageName.value, SetDescriptionRow.value);
            stageManager.SaveStageInfo("Custom", StageName.value);
			// TODO 偵測StageName,SubStageName,fileName或要求用戶輸入
			JsonController.SaveStageDescription(Data, "Custom", StageName.value);
        });

        Button BackHomeButton = rootElement.Q<Button>("BackHomeButton");
        BackHomeButton.RegisterCallback<ClickEvent>(evt =>
        {
            // TODO 檢查是否需要更新
            StageDescription Data = new StageDescription(MusicPath.value, SetLifeRow.value,
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
    
    private VisualElement _CreateStageListView(string rootDir)
    {
        if(Path.GetExtension(rootDir) == ".json")
        {
			Button button = new Button();
			string fileName = Path.GetFileNameWithoutExtension(rootDir);
			string PathName = Path.GetDirectoryName(rootDir);
            string MusicPath = Path.Join(PathName, "StageMusic.mp3");
			if (fileName == "StageChart") 
            {
                fileName = Path.GetFileName(PathName);
			}
			button.text = fileName;
			button.name = fileName;
			button.style.backgroundColor = new Color(0, 0, 0, 0);
			button.RegisterCallback<ClickEvent>((mouse) => {
				_SetStageDescription(JsonController.LoadStageDescription(Path.Join(rootDir,"StageDescription.json")));
				stageManager.LoadStageInfo(rootDir);
                if (Directory.Exists(MusicPath))
                    soundManager.ChangeTrack(MusicPath);
			});
			return button;
		}
		Foldout foldout = new Foldout();
		foldout.text = Path.GetFileName(rootDir);
		foreach (string childDir in Directory.GetDirectories(rootDir))
        {
			VisualElement list = _CreateStageListView(childDir);
			foldout.Add(list);
		}
		string[] files = Directory.GetFiles(rootDir, "StageChart.json");
        if (files.Length == 1)
            return _CreateStageListView(files[0]);

		foreach (string file in files)
            foldout.Add(_CreateStageListView(file));
        if (foldout.childCount>0)
			return foldout;

		return null;
    }

    private void _SetStageChartList()
    {
		ScrollView container = rootElement.Q<ScrollView>("StageChartList");
		container.RegisterCallback<WheelEvent>((scroll) =>
        {
			float scrollAmount = Input.GetAxis("Mouse ScrollWheel")*500;
			if (Input.GetKey(KeyCode.LeftShift))
                container.scrollOffset -= new Vector2(scrollAmount, 0f);
            else
                container.scrollOffset -= new Vector2(0f, scrollAmount);
		});
        container.horizontalScrollerVisibility = ScrollerVisibility.Auto;
        string StageDir = Application.dataPath + "/Resources/StageChart";
        List<string> jsonFile = new List<string>();
        jsonFile.AddRange(Directory.GetFiles(StageDir,"test*.json"));
		container.Add(_CreateStageListView(StageDir));
		foreach (string file in jsonFile)
			container.Add(_CreateStageListView(file));
	}
	
    private void _SetStageDescription(StageDescription data)
    {
		TextField MusicPath = rootElement.Q<TextField>("MusicPath");
		IntegerField SetLifeRow = rootElement.Q<IntegerField>("SetLifeRow");
		TextField SetDescriptionRow = rootElement.Q<TextField>("SetDescriptionRow");
		TextField StageName = rootElement.Q<TextField>("StageName");
        MusicPath.value = data.MusicName;
        SetLifeRow.value = data.TotalLife;
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

	public void ClickPasueButton()
    {
		//和PasueButton做一樣的是，希望後面可以改成真的點擊
		player.SetActive(false);
		enabled = false;
		soundManager.PauseMusic();
	}

    private void _UpdatePlayerState()
    {
		player.SetActive(isPlayerEnable);
        if(isPlayerEnable)
        {
            player.transform.position = new Vector2(0,-3);
        }
	}

	private void _SetPlayButton()
	{
		Button StartButton = rootElement.Q<Button>("StartButton");
		StartButton.RegisterCallback<ClickEvent>((button) =>
		{
			enabled = true;
			_UpdatePlayerState();
			soundManager.PlayMusic();
		});
	}
	private void _SetPauseButton()
	{
		Button PauseButton = rootElement.Q<Button>("PauseButton");
		PauseButton.RegisterCallback<ClickEvent>((button) =>
		{
			ClickPasueButton();
		});
	}
	public bool IsRemoveMode = false;
	private void _SetRemoveModeButton()
	{
		Button RemoveModeButton = rootElement.Q<Button>("ToggleRemoveMode");
		RemoveModeButton.RegisterCallback<ClickEvent>((button) =>
		{
			if (RemoveModeButton.ClassListContains("RemoveModeEnable"))
			{
				RemoveModeButton.RemoveFromClassList("RemoveModeEnable");
				RemoveModeButton.AddToClassList("RemoveModeDisable");
				CursorManager.SetDefaltCursor();
				IsRemoveMode = false;
			}
			else
			{
				RemoveModeButton.RemoveFromClassList("RemoveModeDisable");
				RemoveModeButton.AddToClassList("RemoveModeEnable");
				CursorManager.SetRemoveCursor();
				IsRemoveMode = true;
			}
		});
	}
	private bool isPlayerEnable = true;
	private void _SetPlayerEnableButton()
	{
		Button button = rootElement.Q<Button>("TogglePlayer");
		button.AddToClassList("PlayerEnable");
		button.RegisterCallback<ClickEvent>((evt) =>
		{
			isPlayerEnable = !isPlayerEnable;
			if (enabled)
				_UpdatePlayerState();
			if (isPlayerEnable)
			{
				button.RemoveFromClassList("PlayerDisable");
				button.AddToClassList("PlayerEnable");
			}
			else
			{
				button.RemoveFromClassList("PlayerEnable");
				button.AddToClassList("PlayerDisable");
			}
		});
	}

	private void _SetFuntionButtonGroup()
    {
		_SetPlayButton();
		_SetPauseButton();
		_SetRemoveModeButton();
		_SetPlayerEnableButton();
	}

    private void _SetFootContainer()
    {
		_SetFuntionButtonGroup();
		_SetMusicSlider();
		_SetEditorTabGroup();
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
			StartTimeLabel.text = $"Time:{time.Minutes:D2}:{time.Seconds:D2}";
		});
		MusicSlider.RegisterCallback<MouseCaptureEvent>((obj) =>
		{
			ClickPasueButton();
		});
		MusicSlider.RegisterCallback<MouseCaptureOutEvent>((obj) =>
		{
			float nowStartTime = MusicSlider.value * soundManager.TrackLength;
			soundManager.SetStartTime(nowStartTime);
		});
	}

	private void _SetEditorTabGroup()
	{
		EditorTabGroup tabGroup = rootElement.Q<EditorTabGroup>("EditorTab");
		tabGroup.SetStageManager(stageManager);
	}

	void Start()
    {
        _FindStageController();
		UI = GetComponent<UIDocument>();
		editorCamera = GameObject.Find("Main Camera").GetComponent<EditorCamera>();
		rootElement = UI.rootVisualElement;
        player = GameObject.Find("Player");
        player.SetActive(false);

        _SetFootContainer();
		_SetSettingPannel();
        _SetStageChartList();
        //預設狀態是暫停
        ClickPasueButton();
	}
	// Update is called once per frame
	void Update()
    {
        MusicSlider.value = (soundManager.TrackProgress / soundManager.TrackLength);
	}
}
