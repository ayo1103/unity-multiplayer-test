using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LevelTest_BulletBreakWall : LevelTest_BulletBase
{
    public float damageMultiplier = 1f;
    private float speedMultiplier = 1f;

    public void Initialize(Vector3 targetPosition, float damageMultiplier, float speedMultiplier)
    {
        base.Initialize(targetPosition);
        this.damageMultiplier = damageMultiplier;
        this.speedMultiplier = speedMultiplier;
        speed *= speedMultiplier;
    }

    protected override void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("BreakableWall"))
        {
            collision.GetComponent<LevelTest_Wall>().TakeBulletDamage(damageMultiplier);
            Destroy(gameObject);
        }
        else if (Vector3.Distance(transform.position, target) < 0.1f)
        {
            Destroy(gameObject);
        }

        if (collision.CompareTag("Wall") || collision.CompareTag("Enemy"))
        {
            Destroy(gameObject);
        }
    }
}