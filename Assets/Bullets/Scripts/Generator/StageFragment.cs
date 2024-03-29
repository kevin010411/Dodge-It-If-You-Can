using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class StageFragment:IComparable<StageFragment>
{
    public SaveData.GeneratorInfo generatorInfo;
    public List<SaveData.BulletInfo> content;
    public StageFragment(SaveData.GeneratorInfo generatorInfo, List<SaveData.BulletInfo> content)
    {
        this.generatorInfo = generatorInfo;
		this.content = content;
    }
	//只比較Generator的start
	public int CompareTo(StageFragment other)
	{
		if (this.generatorInfo.start < other.generatorInfo.start) return -1;
		else if (this.generatorInfo.start == other.generatorInfo.start) return 0;
		return 1;
	}
}

namespace SaveData
{
	[Serializable]
	public class GeneratorInfo
	{
		public string ClassName;
		public Vector2 position;
		public double start;
		public double end;
		public SerializedDictionary<string, string> GeneratorParams;
		public GeneratorInfo(string ClassName, Vector2 position, double start, double end, Dictionary<string, string> generatorParams)
		{
			this.ClassName = ClassName;
			this.position = position;
			this.start = start;
			this.end = end;
			GeneratorParams = new SerializedDictionary<string, string>(generatorParams);
		}
	}
	[Serializable]
	public class BulletInfo:IComparable<BulletInfo>
	{
        public string ClassName;
        public SerializedDictionary<string, string> BulletsParams;
		public double start;
		public double end;
        public double interval;
		public BulletInfo(string className,Dictionary<string, string> bulletsParams,
			double start, double end, double interval)
		{
			ClassName = className;
			BulletsParams = new SerializedDictionary<string, string>(bulletsParams);
			this.start = start;
			this.end = end;
			this.interval = interval;
		}
		//只比較start
		public int CompareTo(BulletInfo other)
		{
			if (this.start < other.start) return -1;
			else if (this.start == other.start) return 0;
			return 1;
		}
		public List<FirePoint> SplitToPoint()
		{
			List<FirePoint > points = new List<FirePoint>();
			for(double now = start;now<=end;now+=interval)
			{
				FirePoint temp = new(ClassName, BulletsParams.ToDictionary(),now);
				points.Add(temp);
			}
			return points;
		}
	}

	public class FirePoint:IComparable<FirePoint>
	{
		public string ClassName;
		public Dictionary<string, string> BulletsParams;
		public double FireTime;
		public FirePoint(string className, Dictionary<string, string> bulletsParams,
			double fireTime)
		{
			ClassName = className;
			BulletsParams = bulletsParams;
			FireTime = fireTime;
		}

		public int CompareTo(FirePoint other)
		{
			if (this.FireTime < other.FireTime) return -1;
			else if (this.FireTime == other.FireTime) return 0;
			return 1;
		}
	}
}
