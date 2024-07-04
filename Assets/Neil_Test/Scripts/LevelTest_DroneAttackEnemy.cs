using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LevelTest_DroneAttackEnemy : LevelTest_DroneBase
{
    private Transform currentTarget;

    protected override void PerformAction()
    {
        if (CanShoot())
        {
            DetectAndAttackEnemies();
        }
    }

    private void DetectAndAttackEnemies()
    {
        attackTimer += Time.deltaTime;

        if (attackTimer >= attackCooldown)
        {
            float minDistance = float.MaxValue;
            Transform closestTarget = null;

            Collider2D[] hitColliders = Physics2D.OverlapCircleAll(player.position, attackRange);
            foreach (var hitCollider in hitColliders)
            {
                if (hitCollider.CompareTag("Enemy"))
                {
                    float distance = Vector3.Distance(player.position, hitCollider.transform.position);
                    if (distance < minDistance)
                    {
                        minDistance = distance;
                        closestTarget = hitCollider.transform;
                    }
                }
            }

            if (closestTarget != null)
            {
                if (currentTarget != null && currentTarget != closestTarget)
                {
                    ShowLockOnSymbol(currentTarget, false);
                }

                currentTarget = closestTarget;
                ShowLockOnSymbol(currentTarget, true);
                ShootBullet(currentTarget);
                attackTimer = 0f;
            }
            else
            {
                if (currentTarget != null)
                {
                    ShowLockOnSymbol(currentTarget, false);
                    currentTarget = null;
                }
            }
        }
    }

    private void ShootBullet(Transform target)
    {
        GameObject bullet = Instantiate(bulletPrefab, transform.position, Quaternion.identity);
        LevelTest_BulletAttackEnemy bulletScript = bullet.GetComponent<LevelTest_BulletAttackEnemy>();
        bulletScript.Initialize(target.position, GetDamageMultiplier(), GetSpeedMultiplier());
    }

    private void ShowLockOnSymbol(Transform target, bool show)
    {
        if (target != null)
        {
            Transform lockOnSymbol = target.Find("Target");
            if (lockOnSymbol != null)
            {
                lockOnSymbol.gameObject.SetActive(show);
            }
        }
    }
}
