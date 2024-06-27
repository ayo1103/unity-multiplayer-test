using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    // 移動速度
    public float moveSpeed = 5f;

    // Rigidbody2D 組件
    private Rigidbody2D rb;

    void Start()
    {
        // 獲取 Rigidbody2D 組件
        rb = GetComponent<Rigidbody2D>();
        if (rb == null)
        {
            Debug.LogError("Rigidbody2D 組件未找到，請在角色上添加一個 Rigidbody2D 組件。");
        }
    }

    void Update()
    {
        // 獲取玩家的移動輸入
        float moveX = Input.GetAxis("Horizontal");
        float moveY = Input.GetAxis("Vertical");

        // 設定移動向量
        Vector2 moveInput = new Vector2(moveX, moveY);

        // 計算移動速度向量
        Vector2 moveVelocity = moveInput.normalized * moveSpeed;

        // 使用 Rigidbody2D 移動角色
        rb.velocity = moveVelocity;
    }
}
