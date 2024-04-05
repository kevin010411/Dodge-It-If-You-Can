using DevUtils;
using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using Unity.VisualScripting;
using UnityEditor.U2D.Aseprite;
using UnityEngine;


public class StageManager:MonoBehaviour
{
	public StageInfo StageData;
	private List<GameObject> aliveGenerator;
	public double timeStamp = 0;
	private int index = 0;
	public string DataName;
	
	public void Start()
	{
		StageData = new StageInfo();
		aliveGenerator = new List<GameObject>();
		StageData = JsonContorller.LoadStageInfo(DataName);
		//StageData.CreateTempData();
		//JsonContorller.SaveStageInfo(StageData, DataName);
	}
	public void ReceiveCurrentTimePoint(double timeStamp)
	{
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
		UpdateAllBulletPos(timeStamp);
	}
	
	private void TimedSpawnAndDestroyGenerator(double timeStamp)
	{
		DestroyExpiredGenerator(timeStamp);
		SpawnAtTimeGenerator(timeStamp);
	}
	
	/// <summary>
	/// �ˬdGenerator�O�_���ӭn�Q�R��.
	/// Time Complexity O(n):n�O�{��Generator�ƶq
	/// </summary>
	/// <param name="timeStamp">�{�b�ɶ��W</param>
	private void DestroyExpiredGenerator(double timeStamp)
	{
		List<GameObject> removeList = new List<GameObject>();
		foreach (GameObject generator in aliveGenerator)
			if (generator.GetComponent<BulletGenerator>().end <= timeStamp)
				removeList.Add(generator);
		foreach (GameObject generator in removeList)
		{
			aliveGenerator.Remove(generator);
			Destroy(generator);
		}
	}
	
	/// <summary>
	/// �ˬdGenerator�O�_���ӭn�Q����.
	/// Time Complexity O(1)
	/// </summary>
	/// <param name="timeStamp">�{�b�ɶ��W</param>
	private void SpawnAtTimeGenerator(double timeStamp)
	{
		if(StageData.Data.Count <= index)
			return;
		while (index < StageData.Data.Count && StageData.Data[index].generatorInfo.start <= timeStamp)
		{
			GameObject Generator = new GameObject($"Generator-{index}");
			try
			{
				Type type = Type.GetType(StageData.Data[index].generatorInfo.ClassName); ;
				BulletGenerator bulletGenerator = (BulletGenerator)Generator.AddComponent(type);
				bulletGenerator.InitData(StageData.Data[index]);
				aliveGenerator.Add(Generator);
			}
			catch (Exception e)
			{
				Debug.LogError(e.Message);
				Debug.LogError($"{StageData.Data[index].generatorInfo.ClassName}�����D�A�Хh�T�{�Ҧ�ClassName");
			}
			//Debug.Log("Spawn");
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

	private void CleanStage()
	{
		int Count = aliveGenerator.Count;
		for (int i = 0; i < Count; ++i)
		{
			Destroy(aliveGenerator[0]);
			aliveGenerator.RemoveAt(0);
		}
		GameObject[] bullets = FindAllBullet();
		foreach (GameObject bullet in bullets)
			Destroy(bullet);
	}
}

[Serializable]
public class StageInfo
{
	[Tooltip("Please Don't Change")]
	public List<StageFragment> Data;
	public StageInfo()
	{
		Data = new List<StageFragment>();
	}
	public void CreateTempData()
	{
		Debug.LogWarning("�`�N!Stage��ƨϥιw�]Demo��ơA�èS��Ū�����\");
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
