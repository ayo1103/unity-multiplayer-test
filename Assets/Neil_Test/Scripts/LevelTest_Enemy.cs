using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class LevelTest_Enemy : MonoBehaviour
{
    public float sightRange = 5f; // 视野范围
    public float moveSpeed = 3f; // 移动速度
    public Transform target; // 目标（角色或无人机）
    public int damage = 10; // 伤害数值
    public float fadeInTime = 2f; // 淡入时间
    public GameObject deathEffectPrefab; // 死亡时生成的粒子特效预制件

    private NavMeshAgent navMeshAgent;
    private bool isChasing = false;
    private LayerMask obstacleLayer;
    private SpriteRenderer spriteRenderer;
    private Color originalColor;
    private bool isFadingIn = true;

    void Start()
    {
        navMeshAgent = GetComponent<NavMeshAgent>();
        navMeshAgent.speed = moveSpeed;
        navMeshAgent.updateRotation = false;
        navMeshAgent.updateUpAxis = false;

        obstacleLayer = LayerMask.GetMask("Obstacle");
        spriteRenderer = GetComponent<SpriteRenderer>();
        originalColor = spriteRenderer.color;

        // 初始时将敌人设置为透明
        spriteRenderer.color = new Color(originalColor.r, originalColor.g, originalColor.b, 0);
        StartCoroutine(FadeIn());
        
        Vector3 position = transform.position;
        position.z = 0;
        transform.position = position;
    }

    void Update()
    {
        if (isFadingIn) return;

        if (isChasing && target != null)
        {
            navMeshAgent.SetDestination(target.position);
        }
        else
        {
            DetectTarget();
        }
    }

    void DetectTarget()
    {
        Collider2D[] hits = Physics2D.OverlapCircleAll(transform.position, sightRange);
        foreach (Collider2D hit in hits)
        {
            if (hit.CompareTag("Player"))
            {
                if (!Physics2D.Linecast(transform.position, hit.transform.position, obstacleLayer))
                {
                    target = hit.transform;
                    isChasing = true;
                    break;
                }
            }
        }
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (!isFadingIn && other.CompareTag("Player"))
        {
            HealthSystem playerHealth = other.GetComponent<HealthSystem>();
            if (playerHealth != null)
            {
                playerHealth.OnDamageDealt(damage);
            }
        }
    }

    IEnumerator FadeIn()
    {
        float elapsedTime = 0f;
        while (elapsedTime < fadeInTime)
        {
            elapsedTime += Time.deltaTime;
            float alpha = Mathf.Clamp01(elapsedTime / fadeInTime);
            spriteRenderer.color = new Color(originalColor.r, originalColor.g, originalColor.b, alpha);
            yield return null;
        }
        isFadingIn = false;
    }

    void OnDestroy()
    {
        if (deathEffectPrefab != null)
        {
            Instantiate(deathEffectPrefab, transform.position, Quaternion.identity);
        }
    }

    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, sightRange);
    }
}
