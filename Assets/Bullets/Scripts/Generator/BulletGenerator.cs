using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.WSA;

public class BulletGenerator : MonoBehaviour
{
    [HideInInspector]
    public double start;
    [HideInInspector]
	public double end;
    public double nowTime;
    protected List<SaveData.FirePoint> content;
    private int index = 0;
    public void InitData(StageFragment fragment)
    {
        start = fragment.generatorInfo.start;
        end = fragment.generatorInfo.end;
        GetComponent<Transform>().position = new Vector2(fragment.generatorInfo.position.x,
                                                        fragment.generatorInfo.position.y);
        content = new List<SaveData.FirePoint>();
        foreach(SaveData.BulletInfo bullet in fragment.content)
            content.AddRange(bullet.SplitToPoint());
		content.Sort();
		index = 0;
	}
    // Start is called before the first frame update
    void Start()
    {
        
    }
    // Update is called once per frame
    void Update()
    {
        
    }
	public void ResetIndex()
	{
		index = 0;
	}
	public void Operation(double timeStamp)
    {
		while (CheckBulletSpawn(timeStamp))
		{
			CreateBullet(content[index].FireTime);
			index++;
		}
    }
	private bool CheckBulletSpawn(double timeStamp)
	{
		if (index >= content.Count)
		{
			//沒有東西可以發射了
			//TODO 這裡看可不可以直接刪除Generator
			return false;
		}
		if (index < content.Count && content[index].FireTime < timeStamp)
			return true;
		return false;
	}
    private void CreateBullet(double spawnTime)
    {
		GameObject tempObj = new GameObject($"Bullet-{index}");
		tempObj.tag = "bullet";
		Transform tempTransform = tempObj.transform;
		string posDescribe = content[index].BulletsParams["posDescribe"];
		if (posDescribe == "GeneratorPos")
			tempTransform.position = transform.position;
		SpriteRenderer renderer = tempObj.AddComponent<SpriteRenderer>();
		CircleCollider2D circleCollider2D = tempObj.AddComponent<CircleCollider2D>();
		circleCollider2D.isTrigger = true;
		circleCollider2D.radius = 0.22f;
		try
		{
			Type type = Type.GetType(content[index].ClassName);
			// TODO這邊是使用DurationBullet當作最高的父節點
			DurationBullet bullet = (DurationBullet)tempObj.AddComponent(type);
			bullet.Init(content[index].BulletsParams, spawnTime, transform.position);
		}
		catch (Exception e)
		{
			Debug.LogError(e.Message);
			Debug.LogError($"{content[index].ClassName}有問題，請去確認所有ClassName");
		}
	}
}
