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
    private bool haveMine = false;
    public GameObject healthPack; // 隨機出現的血包
    private bool haveHealthPack = false;
    public float health = 5f; // 生命值初始為 
    float colorProcess = 0f;
    private bool isTakingDamage = false; // 是否正在扣血

    public static bool isTouchingWall = false; // 靜態變量，記錄角色是否在接觸牆

    void Start()
    {
        originalColor = spriteRenderer.color; // 設定原始顏色
        if (Random.Range(0, 15) == 1)
        {
            mine.SetActive(true);
            haveMine = true;
        }
        if (!haveMine && Random.Range(0, 30) == 1)
        {
            healthPack.SetActive(true);
            haveHealthPack = true;
        }
    }

    void Update()
    {
        if (isTouchingPlayer && !isTakingDamage)
        {
            StartCoroutine(TakeDamageOverTime(0.1f, 0.6f));
        }
    }

    IEnumerator TakeDamageOverTime(float interval, float damage)
    {
        isTakingDamage = true;
        if (health > 0)
        {
            health -= damage;
            colorProcess = 1 - (health / 3f); // 計算紅色程度
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
            isTouchingWall = true; // 設置靜態變量
        }
    }

    // 當角色離開方塊時觸發
    void OnCollisionExit2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            isTouchingPlayer = false;
            isTouchingWall = false; // 重置靜態變量
        }
    }

    // 當方塊受到子彈攻擊時調用此方法
    public void TakeBulletDamage(float bulletDamage)
    {
        health -= bulletDamage; // 將生命值按照比例減少
        colorProcess = 1 - (health / 3f); // 計算紅色程度
        spriteRenderer.color = Color.Lerp(originalColor, targetColor, colorProcess);

        if (health <= 0)
        {
            Instantiate(effectPrefab, transform.position, Quaternion.identity); // 生成特效
            Destroy(gameObject); // 生命值耗盡時銷毀方塊
        }
    }
}
