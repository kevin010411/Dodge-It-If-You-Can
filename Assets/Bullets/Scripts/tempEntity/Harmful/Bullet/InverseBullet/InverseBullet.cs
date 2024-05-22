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
[assembly: BulletManager.RegisterBullet(typeof(InverseBullet))]
[Serializable]
public class InverseBullet : DurationBullet
{

	private BulletTypeFactory factory;
	public override void Init(Dictionary<string, string> bulletsParams, double startTime,
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
			Debug.LogError($"再{bulletsParams["SpritePath"]}共{objs.Length}個，並未找到{bulletsParams["subSpriteName"]}");
		float speed = float.Parse(bulletsParams["Speed"]);
		float duration = float.Parse(bulletsParams["Duration"]);
		Vector2 direction = NormalizeDirection(bulletsParams["Direction"]);
		string posDescribe = bulletsParams["posDescribe"];
		factory = BulletTypeFactory.GetBulletTypeFactory();
		BulletInfo = (DurationBulletType)factory.GetInverseBulletType(sprite, speed, duration, 
													direction, posDescribe);
		StartTime = startTime;
		InitPos = initPos;
		myRenderer = GetComponent<SpriteRenderer>();
		myRenderer.sprite = BulletInfo.Image;
		
	}
	public override void ComputePosition(float timeStamp)
	{
		Vector2 FullDist = InitPos + BulletInfo.Direction * BulletInfo.Duration * BulletInfo.Speed;
		transform.position = FullDist - BulletInfo.Direction * (timeStamp - (float)StartTime) * BulletInfo.Speed;
	}

	public new static SaveData.BulletInfo CreateInitBulletInfo()
	{
		Dictionary<string, string>  InitParams = CreateInitBulletParam();
		SaveData.BulletInfo data = new SaveData.BulletInfo("InverseBullet", InitParams,0,0,1);
		return data;
	}
	public new static Dictionary<string, string> CreateInitBulletParam()
	{
		Dictionary<string, string> InitParams = new Dictionary<string, string>();
		InitParams["Damage"] = "1";
		InitParams["SpritePath"] = "Material/Bullet/bullet_1.png";
		InitParams["subSpriteName"] = "bullet_1_2";
		InitParams["Speed"] = "1";
		InitParams["Direction"] = "{X:-1,Y:0}";
		InitParams["Duration"] = "1";
		InitParams["posDescribe"] = "GeneratorPos";
		return InitParams;
	}
}