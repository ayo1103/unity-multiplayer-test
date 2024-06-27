using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LevelTest_Fog : MonoBehaviour
{
    private SpriteRenderer spriteRenderer; // 方塊的 SpriteRenderer 組件
    private Color originalColor; // 原始顏色（黑色）
    private bool isFading = false; // 迷霧是否在淡出
    public float fadeDuration = 1f; // 淡出持續時間
    private float fadeTimer = 0f; // 淡出計時器

    void Start()
    {
        spriteRenderer = GetComponent<SpriteRenderer>(); // 獲取 SpriteRenderer 組件
        originalColor = spriteRenderer.color; // 設定原始顏色
    }

    void Update()
    {
        if (isFading)
        {
            fadeTimer += Time.deltaTime;
            float alpha = Mathf.Lerp(1f, 0f, fadeTimer / fadeDuration); // 計算淡出過程中的透明度
            spriteRenderer.color = new Color(originalColor.r, originalColor.g, originalColor.b, alpha);

            if (fadeTimer >= fadeDuration)
            {
                Destroy(gameObject); // 淡出完成後銷毀迷霧
            }
        }
    }

    // 當角色進入碰撞區域時觸發
    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            isFading = true; // 開始淡出
        }
    }
}