using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LevelTest_PlayerPickFuel : MonoBehaviour
{
    private int fuel = 0;
    public LevelTest_DroneBase[] drones; // 無人機陣列

    public void CollectFuel(int amount)
    {
        fuel += amount;
        foreach (var drone in drones)
        {
            if (drone != null)
            {
                drone.AddFuel(amount);
            }
        }
        // 可以在這裡更新 UI 或觸發其他事件
        // Debug.Log("Collected fuel. Current fuel: " + fuel);
    }

    public int GetFuel()
    {
        return fuel;
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Fuel"))
        {
            LevelTest_Fuel fuelPickup = other.GetComponent<LevelTest_Fuel>();
            if (fuelPickup != null)
            {
                CollectFuel(fuelPickup.fuelAmount);
                Destroy(other.gameObject);
            }
        }
    }
}