using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

static public class JsonContorller
{
	static public void SaveStageInfo(StageInfo SaveData,string SaveFileName)
	{
		//TODO: 檢測SaveDir是相對位置還是絕對位置
		string SaveDir = $"{Application.dataPath}/StageChart/{SaveFileName}";
		StreamWriter writer = File.CreateText(SaveDir);
		//下面兩個debug是看存的資料是不是可以序列化，如果為空或有少代表序列化不徹底
		//Debug.Log(SaveData.Data.Count);
		var data = JsonUtility.ToJson(SaveData);
		//Debug.Log(data);
		writer.Write(data);
		writer.Flush();
		writer.Close();
	}
	static public StageInfo LoadStageInfo(string LoadFileName)
	{
		string LoadDir = $"{Application.dataPath}/StageChart/{LoadFileName}";
		//Debug.Log(LoadDir);
		StreamReader reader = File.OpenText(LoadDir);
		string data = reader.ReadToEnd();
		reader.Close();
		StageInfo Info = JsonUtility.FromJson<StageInfo>(data);
		//Debug.Log(schedule.allPeriod.Count);
		return Info;
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