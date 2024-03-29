using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.AI;

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

    public void Operation(double timeStamp)
    {
        if (index >= content.Count)
        {
            //�S���F��i�H�o�g�F
            //TODO �o�̬ݥi���i�H�����R��Generator
            return;
        }
        while (index < content.Count && content[index].FireTime < timeStamp)
        {
            GameObject tempObj = new GameObject($"Bullet-{index}");
            tempObj.tag = "bullet";
            Transform tempTransform = tempObj.transform;
            string posDescribe = content[index].BulletsParams["posDescribe"];
            if (posDescribe == "GeneratorPos")
				tempTransform.position = transform.position;
			SpriteRenderer renderer = tempObj.AddComponent<SpriteRenderer>();
			Rigidbody2D rigidbody2D = tempObj.AddComponent<Rigidbody2D>();
            rigidbody2D.isKinematic = true;
            CircleCollider2D circleCollider2D = tempObj.AddComponent<CircleCollider2D>();
            circleCollider2D.isTrigger = true;
            try
            {
                Type type = Type.GetType(content[index].ClassName);
                // TODO�o��O�ϥ�DurationBullet��@�̰������`�I
                DurationBullet bullet = (DurationBullet)tempObj.AddComponent(type);
                bullet.Init(content[index].BulletsParams);
            }
            catch (Exception e)
            {
                Debug.LogError(e.Message);
                Debug.LogError($"{content[index].ClassName}�����D�A�Хh�T�{�Ҧ�ClassName");
            }
            index++;
        }

    }
}
