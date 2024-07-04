using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LevelTest_Fuel : MonoBehaviour
{
    public int fuelAmount = 10; // 每次拾取增加的燃料量

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            LevelTest_PlayerPickFuel player = other.GetComponent<LevelTest_PlayerPickFuel>();
            if (player != null)
            {
                player.CollectFuel(fuelAmount);
                Destroy(gameObject);
            }
        }
    }
}