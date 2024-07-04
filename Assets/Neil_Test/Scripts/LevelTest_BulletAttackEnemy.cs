using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LevelTest_BulletAttackEnemy : LevelTest_BulletBase
{
    public int damage = 2;
    protected override void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Enemy"))
        {
            collision.GetComponent<LevelTest_HealthSystem>().OnDamageDealt(damage); // 假設每次攻擊造成 10 點傷害
            Destroy(gameObject);
        }
        else if (Vector3.Distance(transform.position, target) < 0.1f)
        {
            // 當子彈接近目標位置且未碰到敵人時銷毀子彈
            Destroy(gameObject);
        }
        
        if (collision.CompareTag("Wall") || collision.CompareTag("BreakableWall"))
        {
            Destroy(gameObject);
        }
    }
}