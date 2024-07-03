using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LevelTest_PlayerAttack : MonoBehaviour
{
    public GameObject bulletPrefab; // 子弹预制件
    public Transform firePoint; // 子弹发射点
    public float attackCooldown = 1f; // 攻击冷却时间
    public float attackRange = 5f; // 攻击范围
    public int bulletDamage = 10; // 子弹伤害

    private float attackTimer = 0f; // 攻击计时器

    void Update()
    {
        attackTimer += Time.deltaTime;

        // 检测敌人在攻击范围内
        Collider2D[] hits = Physics2D.OverlapCircleAll(transform.position, attackRange);
        foreach (Collider2D hit in hits)
        {
            if (hit.CompareTag("Enemy") && attackTimer >= attackCooldown)
            {
                Shoot(hit.transform);
                attackTimer = 0f; // 重置攻击计时器
                break; // 只攻击一个敌人
            }
        }
    }

    void Shoot(Transform target)
    {
        GameObject bullet = Instantiate(bulletPrefab, firePoint.position, Quaternion.identity);
        LevelTest_PlayerBullet bulletScript = bullet.GetComponent<LevelTest_PlayerBullet>();
        bulletScript.Initialize(target, bulletDamage);
    }

    void OnDrawGizmosSelected()
    {
        // 画出攻击范围的圆圈
        Gizmos.color = Color.green;
        Gizmos.DrawWireSphere(transform.position, attackRange);
    }
}