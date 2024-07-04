using System.Collections;
using System.Collections.Generic;
using UnityEngine;

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

    protected virtual void Start()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        originalColor = spriteRenderer.color;
    }

    protected virtual void Update()
    {
        if (isFollowing)
        {
            FollowPlayer();
            AvoidOtherDrones();
            PerformAction();
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
}
