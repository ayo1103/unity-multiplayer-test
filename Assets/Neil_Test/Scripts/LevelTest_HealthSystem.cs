using UnityEngine;
using System;

public class LevelTest_HealthSystem : MonoBehaviour
{
    public int health;
    public Action OnDied;

    [SerializeField] int maxHealth = 100;

    private void Start()
    {
        health = maxHealth;
    }

    public void OnDamageDealt(int damage)
    {
        health -= damage;

        if (health <= 0)
        {
            OnDied?.Invoke();
            Destroy(gameObject); // 生命值耗尽时销毁对象
        }
    }
}