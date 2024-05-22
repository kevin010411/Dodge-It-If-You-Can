// TODO
// inherit  DurationBullet , ADD Collide detection , once func is triggered
// generate new DurationBullet every 45 degree
using System;
using System.Collections.Generic;
using UnityEngine;

// Name                 Feature          
// BlastBullet          碰到牆壁就生成8顆新的DurationBullet
public class BlastBullet : DurationBullet
{
    //[SerializeField] private Transform wallCheck;
    //[SerializeField] private LayerMask wallLayer;
    [SerializeField] private DurationBullet bulletPrefab;
    private Rigidbody2D rb;

    protected void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        if (rb == null)
        {
            rb = gameObject.AddComponent<Rigidbody2D>();
            Debug.Log("GROUND2");
        }
        gameObject.layer = LayerMask.NameToLayer("groundLayer");

    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        Debug.Log("EXPLODE");
        if (collision.gameObject.name == "Ground")
        {
            Explode();
        }
    }

    private void Explode()
    {
        for (int i = 0; i < 8; i++)
        {
            float angle = i * 45f; // 360 degrees divided by 8
            Vector2 direction = new Vector2(Mathf.Cos(angle * Mathf.Deg2Rad), Mathf.Sin(angle * Mathf.Deg2Rad));
            SpawnBullet(direction, i);
        }
        Destroy(gameObject);
    }

    private void SpawnBullet(Vector2 direction, int index)
    {
        string _x = direction.x.ToString("0.0");
        string _y = direction.y.ToString("0.0");
        string vec = "{X:" + _x + ",Y:" + _y + "}";
        Debug.Log(vec);
        Dictionary<string, string> InitParams = new Dictionary<string, string>();
        InitParams["Damage"] = "1";
        InitParams["SpritePath"] = "/Material/Bullet/bullet_1.png";
        InitParams["subSpriteName"] = "bullet_1_1";
        InitParams["Speed"] = "1";
        InitParams["Direction"] = vec;
        InitParams["Duration"] = "0";
        InitParams["posDescribe"] = "GeneratorPos";

        GameObject tempObj = new GameObject($"BlastScrap-{index}");
        tempObj.tag = "bullet";
        Transform tempTransform = tempObj.transform;
        tempTransform.position = transform.position;
        SpriteRenderer renderer = tempObj.AddComponent<SpriteRenderer>();
        CircleCollider2D circleCollider2D = tempObj.AddComponent<CircleCollider2D>();
        circleCollider2D.isTrigger = true;
        circleCollider2D.radius = 0.22f;
        try
        {
            DurationBullet bullet = (DurationBullet)tempObj.AddComponent(Type.GetType("DurationBullet"));
            bullet.Init(InitParams, 20, transform.position);
        }
        catch (Exception e)
        {
            Debug.LogError(e.Message);
            Debug.LogError("Blast Fail");
        }
    }
}