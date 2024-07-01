using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LevelTest_Drone : MonoBehaviour
{
    public Transform player; // 角色的 Transform
    public float followDistance = 1f; // 跟隨的距離
    public float followSpeed = 2f; // 跟隨的速度
    public GameObject bulletPrefab; // 子彈預製件
    public float attackRange = 5f; // 攻擊範圍
    public float attackCooldown = 1f; // 攻擊冷卻時間
    private float attackTimer = 0f; // 攻擊計時器
    private SpriteRenderer spriteRenderer; // GreenCube 的 SpriteRenderer 組件
    private Color originalColor; // 原始顏色（白色）
    public Color targetColor; // 目標顏色（綠色）
    private bool isFollowing = false; // 是否在跟隨
    private float contactTime = 0f; // 累積接觸時間
    private float requiredContactTime = 1f; // 需要的接觸時間來開始跟隨
    private bool hasTurnedGreen = false; // 是否已經變綠

    private Transform currentTarget; // 當前攻擊的目標

    private void Start()
    {
        spriteRenderer = GetComponent<SpriteRenderer>(); // 獲取 SpriteRenderer 組件
        originalColor = spriteRenderer.color; // 設定原始顏色
    }

    private void Update()
    {
        if (isFollowing)
        {
            // 計算目標位置，保持與角色的距離
            Vector3 targetPosition = player.position - (player.position - transform.position).normalized * followDistance;
            transform.position = Vector3.MoveTowards(transform.position, targetPosition, followSpeed * Time.deltaTime);

            // 避讓其他 GreenCube
            AvoidOtherDrones();

            // 檢測攻擊範圍內的 WhiteCube
            DetectAndAttackWhiteCubes();
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player") && !hasTurnedGreen)
        {
            StartCoroutine(StartFollowing());
        }
    }

    private IEnumerator StartFollowing()
    {
        while (contactTime < requiredContactTime)
        {
            contactTime += Time.deltaTime; // 累積接觸時間
            // 顏色從白色漸變到綠色
            spriteRenderer.color = Color.Lerp(originalColor, targetColor, contactTime / requiredContactTime);
            yield return null; // 等待下一幀
        }
        // 完成變綠
        spriteRenderer.color = targetColor;
        hasTurnedGreen = true; // 標記為已變綠
        isFollowing = true; // 開始跟隨
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("Player") && !hasTurnedGreen)
        {
            StopCoroutine(StartFollowing()); // 停止變色協程
            contactTime = 0f; // 重置接觸時間
            spriteRenderer.color = originalColor; // 恢復原始顏色
        }
    }

    private void AvoidOtherDrones()
    {
        // 獲取所有 GreenCube
        LevelTest_Drone[] allDrones = FindObjectsOfType<LevelTest_Drone>();

        foreach (LevelTest_Drone drone in allDrones)
        {
            if (drone != this && drone.isFollowing)
            {
                // 計算與其他 GreenCube 的距離
                Vector3 directionToDrone = transform.position - drone.transform.position;
                float distanceToDrone = directionToDrone.magnitude;

                if (distanceToDrone < followDistance)
                {
                    // 如果距離過近，則移動遠離其他 GreenCube
                    Vector3 avoidDirection = directionToDrone.normalized;
                    transform.position += avoidDirection * followSpeed * Time.deltaTime;
                }
            }
        }
    }

    private void DetectAndAttackWhiteCubes()
    {
        attackTimer += Time.deltaTime;

        if (attackTimer >= attackCooldown)
        {
            float minDistance = float.MaxValue;
            Transform closestTarget = null;

            Collider2D[] hitColliders = Physics2D.OverlapCircleAll(player.position, 3f);
            foreach (var hitCollider in hitColliders)
            {
                if (hitCollider.CompareTag("BreakableWall"))
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
                    // 關閉之前目標的鎖定符號
                    ShowLockOnSymbol(currentTarget, false);
                }

                currentTarget = closestTarget;
                ShowLockOnSymbol(currentTarget, true);
                ShootBullet(currentTarget);
                attackTimer = 0f; // 重置攻擊計時器
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
        LevelTest_DroneBullet bulletScript = bullet.GetComponent<LevelTest_DroneBullet>();
        bulletScript.Initialize(target.position);
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

    private void OnDrawGizmosSelected()
    {
        // 畫出攻擊範圍的圓圈
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(player.position, 2f);
        Gizmos.color = Color.blue;
        Gizmos.DrawWireSphere(transform.position, attackRange);
    }
}
