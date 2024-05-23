// TODO
// inherit  DurationBullet , ADD Collide detection , once func is triggered
// generate new DurationBullet every 45 degree
using System;
using System.Collections.Generic;
using UnityEngine;

// Name                 Feature          
// BlastBullet          碰到牆壁就生成8顆新的DurationBullet
[assembly: BulletManager.RegisterBullet(typeof(BounceBullet))]
[Serializable]
public class BounceBullet : DurationBullet
{
    [SerializeField] private Rigidbody2D rb;
    private int index = 0;
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
        if (itemName == "Ground" || itemName == "flotFloor")
        {
            Bounce(vec);
        }
    }

    private void Bounce(Vector2 pos)
    {
        Vector2 dir = BulletInfo.Direction;
        Vector2 newDir = Vector2.Reflect(dir, Vector2.up);
        Debug.Log(BulletInfo.Image.ToString());
        SpawnBullet(newDir, pos);
        Destroy(gameObject);
    }

    private void SpawnBullet(Vector2 direction, Vector2 pos)
    {
        string _x = direction.x.ToString("0.0");
        string _y = direction.y.ToString("0.0");
        string vec = "{X:" + _x + ",Y:" + _y + "}";

        Dictionary<string, string> InitParams = new Dictionary<string, string>();
        InitParams["Damage"] = "1";
        InitParams["SpritePath"] = "Material/Bullet/bullet_1.png";
        InitParams["subSpriteName"] = "bullet_1_11";
        InitParams["Speed"] = BulletInfo.Speed.ToString();
        InitParams["Direction"] = vec;
        InitParams["Duration"] = BulletInfo.Duration.ToString();
        InitParams["posDescribe"] = BulletInfo.PosDescribe.ToString();

        GameObject tempObj = new GameObject($"BounceBullet-{index}");
        index++;

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
        SaveData.BulletInfo data = new SaveData.BulletInfo("BounceBullet", InitParams, 0, 0, 1);
        return data;
    }
    public new static Dictionary<string, string> CreateInitBulletParam()
    {
        Dictionary<string, string> InitParams = new Dictionary<string, string>();
        InitParams["Damage"] = "1";
        InitParams["SpritePath"] = "Material/Bullet/bullet_1.png";
        InitParams["subSpriteName"] = "bullet_1_11";
        InitParams["Speed"] = "1";
        InitParams["Direction"] = "{X:-1,Y:0}";
        InitParams["Duration"] = "1";
        InitParams["posDescribe"] = "GeneratorPos";
        return InitParams;
    }


}