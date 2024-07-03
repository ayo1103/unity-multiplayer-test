using UnityEngine;
using UnityEngine.AI;

public class LevelTest_Enemy : MonoBehaviour
{
    public float sightRange = 5f; // 视野范围
    public float moveSpeed = 3f; // 移动速度
    public Transform target; // 目标（角色或无人机）
    public int damage = 10; // 伤害数值

    private NavMeshAgent navMeshAgent;
    private bool isChasing = false;
    private LayerMask obstacleLayer;

    void Start()
    {
        navMeshAgent = GetComponent<NavMeshAgent>();
        navMeshAgent.speed = moveSpeed;
        navMeshAgent.updateRotation = false;
        navMeshAgent.updateUpAxis = false;

        obstacleLayer = LayerMask.GetMask("Obstacle");
    }

    void Update()
    {
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
            if (hit.CompareTag("Player") || hit.CompareTag("Drone"))
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
        if (other.CompareTag("Player"))
        {
            HealthSystem playerHealth = other.GetComponent<HealthSystem>();
            if (playerHealth != null)
            {
                playerHealth.OnDamageDealt(damage);
            }
        }
    }

    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, sightRange);
    }
}