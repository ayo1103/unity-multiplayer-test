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
	public GameObject effectPrefab;
    public GameObject mine;

    void Start()
    {
        //spriteRenderer = GetComponent<SpriteRenderer>(); // 獲取 SpriteRenderer 組件
        originalColor = spriteRenderer.color; // 設定原始顏色
        if (Random.Range(0,15) == 1) mine.SetActive(true);
    }

    void Update()
    {
        if (isTouchingPlayer)
        {
            contactTime += Time.deltaTime; // 累積接觸時間
        }

        // 根據累積接觸時間計算顏色變化
        spriteRenderer.color = Color.Lerp(originalColor, targetColor, contactTime / requiredContactTime);
        
        // 如果累積接觸時間達到要求，銷毀方塊並生成特效
        if (contactTime >= requiredContactTime)
        {
            Instantiate(effectPrefab, transform.position, Quaternion.identity); // 生成特效
            Destroy(gameObject); // 銷毀方塊
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
        }
    }
}
