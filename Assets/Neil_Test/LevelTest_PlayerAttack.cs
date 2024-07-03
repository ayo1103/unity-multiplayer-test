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
    private bool isWallTouching = false; // 用于跟踪墙触碰状态
    private float wallTouchCooldownTimer = 0f; // 墙触碰后的冷却计时器
    public float wallTouchCooldown = 0.5f; // 墙触碰冷却时间

    void Update()
    {
        if (LevelTest_Wall.isTouchingWall)
        {
            isWallTouching = true;
            wallTouchCooldownTimer = wallTouchCooldown;
            return; // 如果角色正在接触墙，则不进行攻击
        }
        else if (isWallTouching)
        {
            if (wallTouchCooldownTimer > 0)
            {
                wallTouchCooldownTimer -= Time.deltaTime;
                return; // 冷却计时未结束，不进行攻击
            }
            else
            {
                isWallTouching = false; // 冷却计时结束，恢复攻击
            }
        }

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
