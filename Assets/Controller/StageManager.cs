using DevUtils;
using System;
using System.IO;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using UnityEditor;
using UnityEngine;


public class StageManager:MonoBehaviour
{
	public StageInfo StageData;
	private List<GameObject> aliveGenerator;
	private UIManager EditorUIManager;
	public double timeStamp = 0;
	public bool isEditMode;
	private int index = 0;
	public string DefaultDataDir;
	
	public void Start()
	{
		StageData = new StageInfo();
		aliveGenerator = new List<GameObject>();
		if(DefaultDataDir != "" )
			LoadStageInfo($"{Application.dataPath}/Resources/{DefaultDataDir}");
		try
		{
			EditorUIManager = GameObject.Find("/EditorUI").GetComponent<UIManager>();
			Debug.Log("Editor Mode On");
		}
		catch { EditorUIManager = null; }
		//StageData.CreateTempData();
		//JsonContorller.SaveStageInfo(StageData, DataName);
	}
	public void SaveStageInfo(string StageName,string SubStageName,string fileName= "StageChart")
	{
		JsonController.SaveStageInfo(StageData, StageName, SubStageName, fileName);
	}
	public void ReceiveCurrentTimePoint(double timeStamp)
	{
		if(!enabled) { return; }
		this.timeStamp = timeStamp;
		Operation(timeStamp);
	}
	
	public void ResetStageStatus()
	{
		index = 0;
		CleanStage();
	}

	public void Operation(double timeStamp)
	{
		TimedSpawnAndDestroyGenerator(timeStamp);
		PassTimeToGenerator(timeStamp);
		CheckGeneratorVisibility();
		UpdateAllBulletPos(timeStamp);
	}
	
	private void TimedSpawnAndDestroyGenerator(double timeStamp)
	{
		DestroyExpiredGenerator(timeStamp);
		SpawnAtTimeGenerator(timeStamp);
	}
	
	/// <summary>
	/// 檢查Generator是否應該要被刪除.
	/// Time Complexity O(n):n是現有Generator數量
	/// </summary>
	/// <param name="timeStamp">現在時間戳</param>
	private void DestroyExpiredGenerator(double timeStamp)
	{
		List<GameObject> removeList = new List<GameObject>();
		foreach (GameObject generator in aliveGenerator)
			if (generator.GetComponent<BulletGenerator>().start > timeStamp ||
				generator.GetComponent<BulletGenerator>().end <= timeStamp)
				removeList.Add(generator);
		foreach (GameObject generator in removeList)
		{
			aliveGenerator.Remove(generator);
			Destroy(generator);
		}
	}
	
	/// <summary>
	/// 檢查Generator是否應該要被產生.
	/// Time Complexity O(1)
	/// </summary>
	/// <param name="timeStamp">現在時間戳</param>
	private void SpawnAtTimeGenerator(double timeStamp)
	{
		if(StageData.Data.Count <= index)
			return;
		while (index < StageData.Data.Count && StageData.Data[index].generatorInfo.start <= timeStamp)
		{
			GameObject Generator = new GameObject($"Generator-{index}");
			Generator.tag = "generator";
			Generator.AddComponent<BoxCollider2D>().isTrigger = true;
			try
			{
				Type type = Type.GetType(StageData.Data[index].generatorInfo.ClassName); ;
				BulletGenerator bulletGenerator = (BulletGenerator)Generator.AddComponent(type);
				int nowIndex = index;
				bulletGenerator.UpdateInfoEvent.AddListener((info) =>UpdataStageData(nowIndex, info));
				bulletGenerator.InitData(StageData.Data[index]);
				bulletGenerator.SetUIManager(EditorUIManager);
				aliveGenerator.Add(Generator);
			}
			catch (Exception e)
			{
				Debug.LogError(e.Message);
				Debug.LogError($"{StageData.Data[index].generatorInfo.ClassName}有問題，請去確認所有ClassName");
			}
			index++;
		}
	}
	
	private void PassTimeToGenerator(double timeStamp)
	{
		foreach (GameObject generator in aliveGenerator)
		{
			BulletGenerator nowGen = generator.GetComponent<BulletGenerator>();
			nowGen.Operation(timeStamp);
		}	
	}

	private void UpdateAllBulletPos(double timeStamp)
	{
		GameObject[] bullets = FindAllBullet();
		foreach(GameObject bullet in bullets)
		{
			bullet.SendMessage(FunctionNames.ReceiveCurrentTimePoint, timeStamp);
		}
	}

	private GameObject[] FindAllBullet()
	{
		return GameObject.FindGameObjectsWithTag("bullet");
	}

	private void CheckGeneratorVisibility()
	{
		if(isEditMode) 
		{
			foreach(GameObject generator in aliveGenerator)
			{
				if (generator.GetComponent<SpriteRenderer>()==null)
				{
					generator.AddComponent<SpriteRenderer>().sprite = 
						Resources.Load<Sprite>("Material/Generator/GeneratorIcon");
					generator.GetComponent<BoxCollider2D>().enabled = true;
				}
			}	
		}
		else
		{
			foreach (GameObject generator in aliveGenerator)
			{
				if (generator.GetComponent<SpriteRenderer>() != null)
				{
					Destroy(generator.GetComponent<SpriteRenderer>());
					generator.GetComponent<BoxCollider2D>().enabled = false;
				}
			}
		}
	}

	private void CleanStage()
	{
		enabled = false;
		int Count = aliveGenerator.Count;
		for (int i = 0; i < Count; ++i)
		{
			Destroy(aliveGenerator[0]);
			aliveGenerator.RemoveAt(0);
		}
		GameObject[] bullets = FindAllBullet();
		foreach (GameObject bullet in bullets)
			Destroy(bullet);
		enabled = true;
	}

	private void UpdataStageData(int genratorIndex,SaveData.GeneratorInfo info)
	{
		StageData.Data[genratorIndex].UpdataGeneratorInfo(info);
	}
	public void CreateNewGenerator()
	{
		StageData.Data.Add(new StageFragment(SaveData.GeneratorInfo.CreateEmptyInfo(timeStamp),
			new List<SaveData.BulletInfo>()));
		StageData.Data.Sort();
		EditorUIManager.ClickPasueButton();
	}
	public void LoadStageInfo(string DataDir)
	{
		StageData = JsonController.LoadStageInfo(DataDir);
		index = 0;
		CleanStage();
	}
}

[Serializable]
public class StageInfo
{
	[Tooltip("Only for looking Data")]
	public List<StageFragment> Data;
	public StageInfo()
	{
		Data = new List<StageFragment>();
	}
	public void CreateTempData()
	{
		Debug.LogWarning("注意!Stage資料使用預設Demo資料，並沒有讀取成功");
		List<StageFragment> tempData = new List<StageFragment>();
		Dictionary<string, string> BulletParams = new Dictionary<string, string>();
		BulletParams["SpritePath"] = "Assets/Bullets/Material/bullet_1.png";
		BulletParams["subSpriteName"] = "bullet_1_0";
		BulletParams["Speed"] = "2";
		BulletParams["Direction"] = "{X:1,Y:2}";
		BulletParams["Duration"] = "5";
		BulletParams["posDescribe"] = "GeneratorPos";
		SaveData.BulletInfo bulletInfo = new SaveData.BulletInfo("DurationBullet",
			BulletParams, 0,20,0.1);
		List<SaveData.BulletInfo> bullets = new List<SaveData.BulletInfo>();
		
		SaveData.GeneratorInfo generatorInfo = new SaveData.GeneratorInfo("BulletGenerator", new Vector2(-11,-1),
			0,2,new Dictionary<string, string>());
		bullets.Add(bulletInfo);
		StageFragment temp1 = new StageFragment(generatorInfo,bullets);
		tempData.Add(temp1);
		Data = tempData;
	}
}


[Serializable]
public class StageDescription
{
	public string MusicName;
	[NonSerialized]
	public string MusicPath;
	public int TotalLife;
	public string Name;
	public string Description;
	public StageDescription(string musicPath, int totalLife, string name, string description)
	{
		MusicPath = musicPath;
		MusicName = Path.GetFileName(musicPath);
		TotalLife = totalLife;
		Name = name;
		Description = description;
	}
	public StageDescription() 
	{
		MusicPath = "";
		MusicName = "";
		TotalLife = 0;
		Name = "";
		Description = "";
	}
}