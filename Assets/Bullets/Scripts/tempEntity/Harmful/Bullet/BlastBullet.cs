// TODO
// inherit  DurationBullet , ADD Collide detection , once func is triggered
// generate new DurationBullet every 45 degree
using System.Collections.Generic;
using UnityEngine;

// Name                 Feature          
// BlastBullet          碰到牆壁就生成8顆新的DurationBullet
public class BlastBullet : DurationBullet
{
    [SerializeField] private Transform wallCheck;
    [SerializeField] private LayerMask wallLayer;
    [SerializeField] private GameObject bulletPrefab; // Prefab for the new bullets
    [SerializeField] protected Rigidbody2D rb;
    private bool walled = false;

    private bool hasCollided = false; // Ensure bullets are spawned only once upon collision
    protected void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        if (rb == null)
        {
            rb = gameObject.AddComponent<Rigidbody2D>(); // Add Rigidbody2D if not present
        }
        rb.gravityScale = 0; // Adjust as needed
        //rb.gravityScale = 5;
        //if (wallCheck == null)
        //{
        //    wallCheck = new GameObject("Ground Check").transform;
        //}
    }

    private void Update()
    {
        //if (!hasCollided && IsWalled())
        //{
        //    Debug.LogError("EXPLODE");
        //    hasCollided = true;
        //    Explode();
        //    Destroy(gameObject); // Destroy the original bullet
        //}
    }

    private bool IsWalled()
    {
        return Physics2D.OverlapCircle(wallCheck.position, 0.1f, LayerMask.GetMask("groundLayer"));
    }

    private void OnCollisionEnter(Collision collision)
    {
        Debug.Log("GROUND1");
        if (collision.gameObject.name.Equals("Ground"))
            Debug.Log("GROUND");
    }
    private void OnCollisionEnter2D(Collision2D collision)
    {
        Debug.Log("GROUND2");
        if (collision.gameObject.name.Equals("Ground"))
            Debug.Log("GROUND");
    }

    private void Explode()
    {
        for (int i = 0; i < 8; i++)
        {
            float angle = i * 45f; // 360 degrees divided by 8
            Vector2 direction = new Vector2(Mathf.Cos(angle * Mathf.Deg2Rad), Mathf.Sin(angle * Mathf.Deg2Rad));
            SpawnBullet(direction);
        }
    }

    private void SpawnBullet(Vector2 direction)
    {
        GameObject newBullet = Instantiate(bulletPrefab, transform.position, Quaternion.identity);
        DurationBullet bulletComponent = newBullet.GetComponent<DurationBullet>();

        if (bulletComponent != null)
        {
            Dictionary<string, string> InitParams = new Dictionary<string, string>();
            InitParams["Damage"] = "1";
            InitParams["SpritePath"] = "/Material/Bullet/bullet_1.png";
            InitParams["subSpriteName"] = "bullet_1_1";
            InitParams["Speed"] = "1";
            InitParams["Direction"] = "{X:0,Y:-1}";
            InitParams["Duration"] = "0";
            InitParams["posDescribe"] = "GeneratorPos";

            bulletComponent.Init(InitParams, Time.time, transform.position); // Initialize the bullet with direction and other parameters
        }
    }
}