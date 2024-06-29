using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LevelTest_DroneBullet : MonoBehaviour
{
    public float speed = 5f; // 子彈速度
    private Vector3 target; // 目標位置

    public void Initialize(Vector3 targetPosition)
    {
        target = targetPosition;
    }

    void Update()
    {
        // 移動子彈
        transform.position = Vector3.MoveTowards(transform.position, target, speed * Time.deltaTime);

        // 如果子彈到達目標位置，銷毀子彈
        if (transform.position == target)
        {
            Destroy(gameObject);
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("BreakableWall"))
        {
            collision.GetComponent<LevelTest_Wall>().TakeBulletDamage();
            Destroy(gameObject); // 碰到 WhiteCube 後銷毀子彈
        }
    }
}
