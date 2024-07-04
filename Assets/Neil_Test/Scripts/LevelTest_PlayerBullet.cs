using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LevelTest_PlayerBullet : MonoBehaviour
{
    public float speed = 10f; // 子弹速度
    private Transform target; // 目标
    private int damage; // 子弹伤害

    public void Initialize(Transform target, int damage)
    {
        this.target = target;
        this.damage = damage;
    }

    void Update()
    {
        if (target != null)
        {
            transform.position = Vector3.MoveTowards(transform.position, target.position, speed * Time.deltaTime);

            if (Vector3.Distance(transform.position, target.position) < 0.1f)
            {
                HitTarget();
            }
        }
        else
        {
            Destroy(gameObject);
        }
    }

    void HitTarget()
    {
        LevelTest_HealthSystem targetHealth = target.GetComponent<LevelTest_HealthSystem>();
        if (targetHealth != null)
        {
            targetHealth.OnDamageDealt(damage);
        }
        Destroy(gameObject);
    }

	void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Wall") || collision.CompareTag("BreakableWall"))
        {
            Destroy(gameObject);
        }
        
    }

}