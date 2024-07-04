using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro; // 使用 TextMeshPro

public abstract class LevelTest_DroneBase : MonoBehaviour
{
    public Transform player;
    public float followDistance = 1f;
    public float followSpeed = 2f;
    public GameObject bulletPrefab;
    public float attackRange = 5f;
    public float attackCooldown = 1f;
    protected float attackTimer = 0f;
    protected SpriteRenderer spriteRenderer;
    protected Color originalColor;
    public Color targetColor;
    protected bool isFollowing = false;
    protected float contactTime = 0f;
    protected float requiredContactTime = 0.5f;
    protected bool hasTurnedGreen = false;
    public int fuel = 50;
    private Coroutine fuelConsumptionCoroutine;
    public TextMeshPro fuelText; // 燃料值的 TextMeshPro 組件

    protected virtual void Start()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        originalColor = spriteRenderer.color;
        UpdateFuelText(); // 初始時更新顯示燃料值
    }

    protected virtual void Update()
    {
        if (isFollowing)
        {
            FollowPlayer();
            AvoidOtherDrones();
            PerformAction();
            UpdateFuelText(); // 每幀更新顯示燃料值
        }
    }

    protected abstract void PerformAction();

    private void FollowPlayer()
    {
        float distanceToPlayer = Vector3.Distance(transform.position, player.position);
        if (distanceToPlayer > followDistance)
        {
            Vector3 targetPosition = player.position - (player.position - transform.position).normalized * followDistance;
            transform.position = Vector3.MoveTowards(transform.position, targetPosition, followSpeed * Time.deltaTime);
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player") && !hasTurnedGreen)
        {
            StartCoroutine(StartFollowing());
        }
    }

    private IEnumerator StartFollowing()
    {
        while (contactTime < requiredContactTime)
        {
            contactTime += Time.deltaTime;
            spriteRenderer.color = Color.Lerp(originalColor, targetColor, contactTime / requiredContactTime);
            yield return null;
        }
        spriteRenderer.color = targetColor;
        hasTurnedGreen = true;
        isFollowing = true;

        fuelConsumptionCoroutine = StartCoroutine(ConsumeFuel());
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("Player") && !hasTurnedGreen)
        {
            StopCoroutine(StartFollowing());
            contactTime = 0f;
            spriteRenderer.color = originalColor;
        }
    }

    protected void AvoidOtherDrones()
    {
        LevelTest_DroneBase[] allDrones = FindObjectsOfType<LevelTest_DroneBase>();
        foreach (LevelTest_DroneBase drone in allDrones)
        {
            if (drone != this && drone.isFollowing)
            {
                Vector3 directionToDrone = transform.position - drone.transform.position;
                float distanceToDrone = directionToDrone.magnitude;
                if (distanceToDrone < followDistance)
                {
                    Vector3 avoidDirection = directionToDrone.normalized;
                    transform.position += avoidDirection * followSpeed * Time.deltaTime;
                }
            }
        }
    }

    private IEnumerator ConsumeFuel()
    {
        while (true)
        {
            yield return new WaitForSeconds(0.4f);
            fuel = Mathf.Max(0, fuel - 1);
            UpdateFuelText(); // 每次消耗燃料後更新顯示燃料值
        }
    }

    public void AddFuel(int amount)
    {
        fuel += amount;
        UpdateFuelText(); // 每次增加燃料後更新顯示燃料值
    }

    public bool IsActivated()
    {
        return isFollowing;
    }

    protected bool CanShoot()
    {
        return fuel > 0;
    }

    protected float GetDamageMultiplier()
    {
        return fuel > 100 ? 2f : 1f;
    }

    protected float GetSpeedMultiplier()
    {
        return fuel > 100 ? 2f : 1f;
    }

    private void UpdateFuelText()
    {
        if (fuelText != null)
        {
            fuelText.text = "Fuel: " + fuel;
        }
    }
}
