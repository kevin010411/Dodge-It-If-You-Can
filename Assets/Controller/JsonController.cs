using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Security.Cryptography.X509Certificates;
using UnityEditor;
using UnityEngine;

static public class JsonController
{
	static public void SaveStageDescription(StageDescription SaveData,string StageName
		, string SubStageName, string FileName= "StageDescription")
	{
		string SaveDir = _ComputeSaveDir(StageName, SubStageName, FileName);
		string Data = JsonUtility.ToJson(SaveData);
		try
		{
			// TODO 檢查是否相同 相同不做事，不相同則刪除
			if (File.Exists(Path.GetDirectoryName(SaveDir) + "/StageMusic.mp3"))
				File.Delete(Path.GetDirectoryName(SaveDir) + "/StageMusic.mp3");
			File.Copy(SaveData.MusicPath, Path.GetDirectoryName(SaveDir) + "/StageMusic.mp3",true);
			//FileUtil.CopyFileOrDirectory(SaveData.MusicPath, Path.GetDirectoryName(SaveDir) + "/StageMusic.mp3");
		}
		catch (Exception ex)
		{
			Debug.LogException(ex);
		}
		_SaveData(SaveDir, Data);
	}
	static public StageDescription LoadStageDescription(string LoadDir)
	{
		if(!File.Exists(LoadDir))
			return new StageDescription();
		StreamReader reader = File.OpenText(LoadDir);
		string data = reader.ReadToEnd();
		reader.Close();
		StageDescription Info = JsonUtility.FromJson<StageDescription>(data);
		//Debug.Log(schedule.allPeriod.Count);
		return Info;
	}
	static public bool isNeedSaveStageDescription(StageDescription SaveData, string StageName
		, string SubStageName, string FileName= "StageDescription")
	{
		string SaveDir = _ComputeSaveDir(StageName, SubStageName, FileName);
		if (!File.Exists(SaveDir))
			return false;
		StreamReader reader = File.OpenText(SaveDir);
		string PreData = reader.ReadToEnd();
		reader.Close();
		string NowData = JsonUtility.ToJson(SaveData);
		if (PreData == NowData)
			return false;
		return true;
	}
	static public void SaveStageInfo(StageInfo SaveData,string StageName
		,string SubStageName,string FileName)
	{
		string SaveDir = _ComputeSaveDir(StageName, SubStageName, FileName);
		string data = JsonUtility.ToJson(SaveData);
		_SaveData(SaveDir, data);
	}
	static private void _SaveData<T>(string SaveDir,T Data)
	{
		_CheckAndCreateFile(SaveDir);
		StreamWriter writer = File.CreateText(SaveDir);	
		writer.Write(Data);
		writer.Flush();
		writer.Close();
	}
	static private void _CheckAndCreateFile(string FilePath)
	{
		string DirectoryPath = Path.GetDirectoryName(FilePath);
		if (!Directory.Exists(DirectoryPath))
		{
			Directory.CreateDirectory(DirectoryPath);
		}
		if (!File.Exists(FilePath))
		{
			File.Create(FilePath).Close();
		}
	}
	static private string _ComputeSaveDir(string StageName
		, string SubStageName, string SaveFileName)
	{
		return $"{Application.dataPath}/Resources/{StageName}/{SubStageName}/{SaveFileName}.json";
	}

	static public StageInfo LoadStageInfo(string LoadDir)
	{
		StreamReader reader = File.OpenText(LoadDir);
		string data = reader.ReadToEnd();
		reader.Close();
		StageInfo Info = JsonUtility.FromJson<StageInfo>(data);
		return Info;
	}
}

public static class JsonControllerV2
{
	public static SFXChart ToSFXChart(TextAsset JsonFile)
	{
		SFXChart NewChart = JsonUtility.FromJson<SFXChart>(JsonFile.text);
		return NewChart;
	}
}

//因為不能儲存Dictionary，所以要額外創建可Serializable的物件
[Serializable]
public class SerializedDictionary<TKey, TValue> : ISerializationCallbackReceiver
{
	[SerializeField]
	private List<TKey> keys = new List<TKey>();
	[SerializeField]
	private List<TValue> values = new List<TValue>();

	private Dictionary<TKey, TValue> target;
	public Dictionary<TKey, TValue> ToDictionary() { return target; }
	public SerializedDictionary(Dictionary<TKey,TValue> target) 
	{ 
		this.target = target;
	}
	public void OnAfterDeserialize()
	{
		int count = Math.Min(keys.Count, values.Count);
		target = new Dictionary<TKey, TValue>();
		for(int i = 0; i < count; i++)
		{
			target.Add(keys[i], values[i]);
		}
	}

	public void OnBeforeSerialize()
	{
		keys = new List<TKey>(target.Keys);
		values = new List<TValue>(target.Values);
	}
}