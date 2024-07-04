using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LevelTest_EnemySpawner : MonoBehaviour
{
    public GameObject enemyPrefab; // 要生成的敌人预制件
    public int spawnCount = 3; // 每次生成的敌人数
    public float spawnInterval = 5f; // 生成间隔时间
    public int maxEnemies = 10; // 初始敌人数量上限
    public int maxEnemiesIncrement = 3; // 每次增加的敌人数量上限
    public float incrementInterval = 10f; // 增加上限的时间间隔
    public float spawnRange = 1f; // 生成偏移范围

    private float nextSpawnTime;
    private float nextIncrementTime;
    private int currentEnemyCount = 0;
    private List<GameObject> enemies = new List<GameObject>();
    private Transform[] spawnPoints;

    void Start()
    {
        // 获取所有子对象作为生成点
        List<Transform> spawnPointsList = new List<Transform>(GetComponentsInChildren<Transform>());
        spawnPointsList.Remove(transform); // 移除自身
        spawnPoints = spawnPointsList.ToArray();

        nextSpawnTime = Time.time + spawnInterval;
        nextIncrementTime = Time.time + incrementInterval;
    }

    void Update()
    {
        if (Time.time >= nextIncrementTime && maxEnemies<30)
        {
            maxEnemies += maxEnemiesIncrement;
            nextIncrementTime = Time.time + incrementInterval;
        }

        if (Time.time >= nextSpawnTime && currentEnemyCount < maxEnemies)
        {
            SpawnEnemies();
            nextSpawnTime = Time.time + spawnInterval;
        }
    }

    void SpawnEnemies()
    {
        for (int i = 0; i < spawnCount; i++)
        {
            if (currentEnemyCount >= maxEnemies) break;

            Transform spawnPoint = spawnPoints[Random.Range(0, spawnPoints.Length)];
            Vector3 spawnPosition = spawnPoint.position + new Vector3(Random.Range(-spawnRange, spawnRange), Random.Range(-spawnRange, spawnRange), 0);
            GameObject enemy = Instantiate(enemyPrefab, spawnPosition, spawnPoint.rotation);
            enemies.Add(enemy);
            currentEnemyCount++;
            enemy.GetComponent<LevelTest_HealthSystem>().OnDied += () => { currentEnemyCount--; enemies.Remove(enemy); };
        }
    }

    void OnDrawGizmos()
    {
        if (spawnPoints == null)
        {
            // 获取所有子对象作为生成点
            List<Transform> spawnPointsList = new List<Transform>(GetComponentsInChildren<Transform>());
            spawnPointsList.Remove(transform); // 移除自身
            spawnPoints = spawnPointsList.ToArray();
        }

        Gizmos.color = Color.yellow;
        foreach (Transform spawnPoint in spawnPoints)
        {
            if (spawnPoint != transform) // 不绘制自身的位置
            {
                Gizmos.DrawWireSphere(spawnPoint.position, 0.5f);
            }
        }
    }
}
