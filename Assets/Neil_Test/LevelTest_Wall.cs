using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LevelTest_Wall : MonoBehaviour
{
    public SpriteRenderer spriteRenderer; // 方塊的 SpriteRenderer 組件
    private Color originalColor; // 原始顏色（白色）
    private Color targetColor = Color.red; // 目標顏色（紅色）
    private bool isTouchingPlayer = false; // 角色是否在接觸方塊
    private float contactTime = 0f; // 累積接觸時間
    public float requiredContactTime = 0.5f; // 需要的接觸時間來銷毀方塊
    public GameObject effectPrefab; // 銷毀時的特效
    public GameObject mine; // 隨機出現的地雷
    private float health = 5f; // 生命值初始為 5
    private float damageAmount = 2f; // 每次受到攻擊時降低的生命值
    private float bulletDamage = 0.2f; // 子彈每次命中降低的生命值比例

    void Start()
    {
        originalColor = spriteRenderer.color; // 設定原始顏色
        if (Random.Range(0, 15) == 1)
        {
            mine.SetActive(true);
        }
    }

    void Update()
    {
        if (isTouchingPlayer)
        {
            contactTime += Time.deltaTime; // 累積接觸時間

            // 根據累積接觸時間計算顏色變化
            float t = contactTime / requiredContactTime;
            spriteRenderer.color = Color.Lerp(originalColor, targetColor, t);

            // 如果累積接觸時間達到要求，銷毀方塊並生成特效
            if (contactTime >= requiredContactTime)
            {
                Instantiate(effectPrefab, transform.position, Quaternion.identity); // 生成特效
                Destroy(gameObject); // 銷毀方塊
            }
        }
    }

    // 當角色開始接觸方塊時觸發
    void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            isTouchingPlayer = true;
        }
    }

    // 當角色離開方塊時觸發
    void OnCollisionExit2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            isTouchingPlayer = false;
            contactTime = 0f; // 重置接觸時間
            spriteRenderer.color = originalColor; // 恢復原始顏色
        }
    }

    // 當方塊受到攻擊時調用此方法
    public void TakeDamage()
    {
        health -= damageAmount;
        float t = 1 - (health / 5f); // 計算紅色程度
        spriteRenderer.color = Color.Lerp(originalColor, targetColor, t);

        if (health <= 0)
        {
            Instantiate(effectPrefab, transform.position, Quaternion.identity); // 生成特效
            Destroy(gameObject); // 生命值耗盡時銷毀方塊
        }
    }

    // 當方塊受到子彈攻擊時調用此方法
    public void TakeBulletDamage()
    {
        health -= bulletDamage * 5f; // 將生命值按照比例減少
        float t = 1 - (health / 5f); // 計算紅色程度
        spriteRenderer.color = Color.Lerp(originalColor, targetColor, t);

        if (health <= 0)
        {
            Instantiate(effectPrefab, transform.position, Quaternion.identity); // 生成特效
            Destroy(gameObject); // 生命值耗盡時銷毀方塊
        }
    }
}
