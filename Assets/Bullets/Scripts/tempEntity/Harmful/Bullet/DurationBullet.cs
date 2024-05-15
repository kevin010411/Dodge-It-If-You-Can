using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics.Tracing;
using System.Numerics;
using UnityEditor;
using UnityEngine;
using Vector2 = UnityEngine.Vector2;
/*
 * BaseBullets的檔案
 * 
 */
[assembly: BulletManager.RegisterBullet(typeof(DurationBullet))]
[Serializable]
public class DurationBullet : Harmful
{
	[SerializeField]
	public DurationBulletType BulletInfo;

	private BulletTypeFactory factory;
	protected SpriteRenderer myRenderer;
	private Vector2 InitPos;
	private double StartTime;
	public void Init(Dictionary<string, string> bulletsParams, double startTime,
		Vector2 initPos)
	{
		Sprite sprite = null;
		if (bulletsParams["SpritePath"].Contains("Assets/Bullets/Material/"))
			bulletsParams["SpritePath"] = bulletsParams["SpritePath"].Replace("Assets/Bullets/Material/", "Material/Bullet/");
		if (bulletsParams["SpritePath"].Contains(".png"))
			bulletsParams["SpritePath"] = bulletsParams["SpritePath"].Replace(".png","");
		Sprite[] objs = Resources.LoadAll<Sprite>(bulletsParams["SpritePath"]);
		foreach (Sprite obj in objs)
		{
			if (obj.name == bulletsParams["subSpriteName"])
			{
				sprite = (Sprite)obj;
				break;
			}
		}
		if (sprite == null)
			Debug.LogError($"再{bulletsParams["SpritePath"]}並未找到{bulletsParams["subSpriteName"]}");
		float speed = float.Parse(bulletsParams["Speed"]);
		float duration = float.Parse(bulletsParams["Duration"]);
		Vector2 direction = NormalizeDirection(bulletsParams["Direction"]);
		string posDescribe = bulletsParams["posDescribe"];
		factory = BulletTypeFactory.GetBulletTypeFactory();
		BulletInfo = factory.GetDurationBulletType(sprite, speed, duration, 
													direction, posDescribe);
		StartTime = startTime;
		InitPos = initPos;
		myRenderer = GetComponent<SpriteRenderer>();
		myRenderer.sprite = BulletInfo.Image;
	}
	private Vector2 NormalizeDirection(string DirStr)
	{
		int startIdx = DirStr.IndexOf("X:") + 2;
		float XDir = float.Parse(DirStr.Substring(startIdx, DirStr.IndexOf(",Y") - startIdx));
		startIdx = DirStr.IndexOf("Y:") + 2;
		float YDir = float.Parse(DirStr.Substring(startIdx, DirStr.IndexOf("}") - startIdx));
		float slope = (float)Math.Sqrt(Math.Pow(XDir, 2) + Math.Pow(YDir, 2));
		XDir /= slope;
		YDir /= slope;
		return new Vector2(XDir, YDir);
	}
	public void ReceiveCurrentTimePoint(float timeStamp)
	{
		if (timeStamp > StartTime + BulletInfo.Duration)
		{
			Destroy(gameObject);
			return;
		}
		ComputePosition(timeStamp);
	}
	public void ComputePosition(float timeStamp)
	{
		transform.position = InitPos + BulletInfo.Direction * (timeStamp - (float)StartTime) * BulletInfo.Speed;
	}

	public static SaveData.BulletInfo CreateInitBulletInfo()
	{
		Dictionary<string, string>  InitParams = CreateInitBulletParam();
		SaveData.BulletInfo data = new SaveData.BulletInfo("DurationBullet", InitParams,0,0,1);
		return data;
	}
	public static Dictionary<string, string> CreateInitBulletParam()
	{
		Dictionary<string, string> InitParams = new Dictionary<string, string>();
		InitParams["Damage"] = "1";
		InitParams["SpritePath"] = "/Material/Bullet/bullet_1.png";
		InitParams["subSpriteName"] = "bullet_1_1";
		InitParams["Speed"] = "1";
		InitParams["Direction"] = "{X:-1,Y:0}";
		InitParams["Duration"] = "0";
		InitParams["posDescribe"] = "GeneratorPos";
		return InitParams;
	}
}