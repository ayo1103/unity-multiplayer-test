using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    // 移動速度
    public float moveSpeed = 5f;

    // Rigidbody2D 組件
    private Rigidbody2D rb;

    // 移動輸入
    private Vector2 moveInput;
    private Vector2 moveVelocity;

    void Start()
    {
        // 獲取 Rigidbody2D 組件
        rb = GetComponent<Rigidbody2D>();
    }

    void Update()
    {
        // 獲取玩家的移動輸入
        float moveX = Input.GetAxis("Horizontal");
        float moveY = Input.GetAxis("Vertical");

        // 設定移動輸入和速度
        moveInput = new Vector2(moveX, moveY);
        moveVelocity = moveInput.normalized * moveSpeed;
    }

    void FixedUpdate()
    {
        // 使用 Rigidbody2D 移動角色
        rb.MovePosition(rb.position + moveVelocity * Time.fixedDeltaTime);
    }

}
