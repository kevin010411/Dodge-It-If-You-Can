// TODO
// inherit  DurationBullet , ADD Collide detection , once func is triggered
// generate new DurationBullet every 45 degree
using System;
using System.Collections.Generic;
using UnityEngine;

// Name                 Feature          
// BlastBullet          碰到牆壁就生成8顆新的DurationBullet
[assembly: BulletManager.RegisterBullet(typeof(BlastBullet))]
[Serializable]
public class BlastBullet : DurationBullet
{
    [SerializeField] private DurationBullet bulletPrefab;
    [SerializeField] private Rigidbody2D rb;

    protected void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        if (rb == null)
        {
            rb = gameObject.AddComponent<Rigidbody2D>();
        }
        gameObject.layer = LayerMask.NameToLayer("groundLayer");

    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        //Debug.Log("EXPLODE");
        string itemName = collision.gameObject.name;
        Vector2 vec = collision.ClosestPoint(transform.position);
        if (itemName == "Ground" || itemName == "flotFloor" || itemName == "Player")
        {
            Explode(vec);
        }
    }

    private void Explode(Vector2 pos)
    {
        for (int i = 7; i >= 0; i--)
        {
            float angle = i * 45f; // 360 degrees divided by 8
            Vector2 direction = new Vector2(Mathf.Cos(angle * Mathf.Deg2Rad), Mathf.Sin(angle * Mathf.Deg2Rad));
            SpawnBullet(direction, i, pos);
        }
        Destroy(gameObject);

    }
    private void SpawnBullet(Vector2 direction, int index, Vector2 pos)
    {
        string _x = direction.x.ToString("0.0");
        string _y = direction.y.ToString("0.0");
        string vec = "{X:" + _x + ",Y:" + _y + "}";

        Dictionary<string, string> InitParams = new Dictionary<string, string>();
        InitParams["Damage"] = "1";
        InitParams["SpritePath"] = "Assets/Bullets/Material/bullet_1.png";
        InitParams["subSpriteName"] = "bullet_1_11";
        InitParams["Speed"] = "10";
        InitParams["Direction"] = vec;
        InitParams["Duration"] = "2";
        InitParams["posDescribe"] = "GeneratorPos";


        GameObject tempObj = new GameObject($"BlastScrap-{index}");

        tempObj.tag = "bullet";
        SpriteRenderer renderer = tempObj.AddComponent<SpriteRenderer>();
        CircleCollider2D circleCollider2D = tempObj.AddComponent<CircleCollider2D>();
        circleCollider2D.isTrigger = true;
        circleCollider2D.radius = 0.22f;
        try
        {
            DurationBullet bullet = (DurationBullet)tempObj.AddComponent(Type.GetType("DurationBullet"));
            GameObject tmp = GameObject.Find("StageController");
            float time = (float)tmp.GetComponent<StageManager>().timeStamp;
            bullet.Init(InitParams, time, pos);
        }
        catch (Exception e)
        {
            Debug.LogError(e.Message);
            Debug.LogError("Blast Fail");
        }
    }

    public new static SaveData.BulletInfo CreateInitBulletInfo()
    {
        Dictionary<string, string> InitParams = CreateInitBulletParam();
        SaveData.BulletInfo data = new SaveData.BulletInfo("BlastBullet", InitParams, 0, 0, 1);
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