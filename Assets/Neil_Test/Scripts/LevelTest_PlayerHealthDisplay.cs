using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class LevelTest_PlayerHealthDisplay : MonoBehaviour
{
    public TextMeshPro healthText; // 用於顯示血量的 TextMeshPro 組件
    private LevelTest_HealthSystem healthSystem; // 用於存取角色的生命值系統

    private void Start()
    {
        healthSystem = GetComponent<LevelTest_HealthSystem>();

        if (healthSystem == null)
        {
            Debug.LogError("未找到 LevelTest_HealthSystem 組件！");
            return;
        }

        UpdateHealthText();
    }
    private void Update() {
        UpdateHealthText();
    }

    private void OnEnable()
    {
        healthSystem.OnDied += HandleOnDied;
    }

    private void OnDisable()
    {
        healthSystem.OnDied -= HandleOnDied;
    }

    public void UpdateHealthText()
    {
        if (healthText != null && healthSystem != null)
        {
            healthText.text = "HP: " + healthSystem.health;
        }
    }

    private void HandleOnDied()
    {
        UpdateHealthText();
    }

    private void OnDamageDealt()
    {
        UpdateHealthText();
    }
}
