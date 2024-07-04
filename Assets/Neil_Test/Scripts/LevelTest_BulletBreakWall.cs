using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LevelTest_BulletBreakWall : LevelTest_BulletBase
{
    public float damage = 1.25f;
    protected override void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("BreakableWall"))
        {
            collision.GetComponent<LevelTest_Wall>().TakeBulletDamage(damage);
            Destroy(gameObject);
        }
    }
}