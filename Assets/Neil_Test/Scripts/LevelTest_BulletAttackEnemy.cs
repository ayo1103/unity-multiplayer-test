using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LevelTest_BulletAttackEnemy : LevelTest_BulletBase
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
        if (collision.CompareTag("Enemy"))
        {
            collision.GetComponent<LevelTest_HealthSystem>().OnDamageDealt((int)(damageMultiplier)); // 假設每次攻擊造成 10 點傷害
            Destroy(gameObject);
        }
        else if (Vector3.Distance(transform.position, target) < 0.1f)
        {
            Destroy(gameObject);
        }

        if (collision.CompareTag("Wall") || collision.CompareTag("BreakableWall"))
        {
            Destroy(gameObject);
        }
    }
}