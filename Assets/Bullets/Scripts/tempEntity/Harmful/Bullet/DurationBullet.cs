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
[Serializable]
public class DurationBullet : Harmful
{
	[SerializeField]
	public DurationBulletType BulletInfo;

	private BulletTypeFactory factory;
	protected Rigidbody2D myRigidbody;
	protected SpriteRenderer myRenderer;

	public void Init(Dictionary<string, string> bulletsParams)
	{
		Sprite sprite = null;
		UnityEngine.Object[] objs = AssetDatabase.LoadAllAssetRepresentationsAtPath(bulletsParams["SpritePath"]);
		foreach (var obj in objs)
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
		string DirStr = bulletsParams["Direction"];
		int startIdx = DirStr.IndexOf("X:") + 2;
		float XDir = float.Parse(DirStr.Substring(startIdx, DirStr.IndexOf(",Y") - startIdx));
		startIdx = DirStr.IndexOf("Y:") + 2;
		float YDir = float.Parse(DirStr.Substring(startIdx, DirStr.IndexOf("}")- startIdx));
		float slope = (float)Math.Sqrt(Math.Pow(XDir, 2) + Math.Pow(YDir, 2));
		XDir /= slope;
		YDir /= slope;
		Vector2 direction = new Vector2(XDir,YDir);
		string posDescribe = bulletsParams["posDescribe"];
		factory = BulletTypeFactory.GetBulletTypeFactory();
		BulletInfo = factory.GetDurationBulletType(sprite, speed, duration, 
													direction, posDescribe);
		myRigidbody = GetComponent<Rigidbody2D>();
		myRenderer = GetComponent<SpriteRenderer>();
		myRenderer.sprite = BulletInfo.Image;
		Destroy(gameObject, BulletInfo.Duration);
		setDirect(BulletInfo.Direction);
	}
	public void Awake()
	{
		
	}
	public void FixedUpdate()
	{
		transform.Rotate(0,0, 300f * Time.deltaTime);
	}
	public void setDirect(Vector2 direct)
	{
		if (myRigidbody != null)
		{
			if (BulletInfo == null)
			{
				myRigidbody.velocity = direct * 2;
			}
			else
			{
				myRigidbody.velocity = direct * BulletInfo.Speed;
			}
		}
	}
}