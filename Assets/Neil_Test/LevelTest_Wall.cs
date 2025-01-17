using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LevelTest_Wall : MonoBehaviour
{
    public SpriteRenderer spriteRenderer; // 方塊的 SpriteRenderer 組件
    private Color originalColor; // 原始顏色（白色）
    private Color targetColor = Color.red; // 目標顏色（紅色）
    private bool isTouchingPlayer = false; // 角色是否在接觸方塊
    public float requiredContactTime = 0.8f; // 需要的接觸時間來銷毀方塊
    public GameObject effectPrefab; // 銷毀時的特效
    public GameObject mine; // 隨機出現的地雷
    private float health = 5f; // 生命值初始為 5
    private float damageAmount = 1f; // 每次受到攻擊時降低的生命值
    private float bulletDamage = 1.25f; // 子彈每次命中降低的生命值比例
    float colorProcess = 0f;
    private bool isTakingDamage = false; // 是否正在扣血

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
        if (isTouchingPlayer && !isTakingDamage)
        {
            StartCoroutine(TakeDamageOverTime(0.1f, 0.8f));
        }
    }
    IEnumerator TakeDamageOverTime(float interval, float damage)
    {
        isTakingDamage = true;
        if (health > 0)
        {
            health -= damage;
            colorProcess = 1 - (health / 5f); // 計算紅色程度
            spriteRenderer.color = Color.Lerp(originalColor, targetColor, colorProcess);

            

            yield return new WaitForSeconds(interval);
        }
        if (health <= 0)
        {
            Instantiate(effectPrefab, transform.position, Quaternion.identity); // 生成特效
            Destroy(gameObject);
            yield break;
        }
        
        isTakingDamage = false;
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
            

        }
    }

    

    // 當方塊受到子彈攻擊時調用此方法
    public void TakeBulletDamage()
    {
        health -= bulletDamage; // 將生命值按照比例減少
        colorProcess = 1 - (health / 5f); // 計算紅色程度
        spriteRenderer.color = Color.Lerp(originalColor, targetColor, colorProcess);

        if (health <= 0)
        {
            Instantiate(effectPrefab, transform.position, Quaternion.identity); // 生成特效
            Destroy(gameObject); // 生命值耗盡時銷毀方塊
        }
    }
}
