using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LevelTest_EnemySpawner : MonoBehaviour
{
    public GameObject enemyPrefab; // 要生成的敌人预制件
    public GameObject bossPrefab; // 要生成的Boss预制件
    public int spawnCount = 3; // 每次生成的敌人数
    public float spawnInterval = 5f; // 生成间隔时间
    public int initialMaxEnemies = 10; // 初始敌人数量上限
    public int maxEnemies = 60; // 最大敌人数量上限
    public int maxEnemiesIncrement = 3; // 每次增加的敌人数量上限
    public float incrementInterval = 10f; // 增加上限的时间间隔
    public float spawnRange = 1f; // 生成偏移范围
    public float bossSpawnInterval = 80f; // Boss生成间隔时间

    private float nextSpawnTime;
    private float nextIncrementTime;
    private float nextBossSpawnTime;
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
        nextBossSpawnTime = Time.time + bossSpawnInterval;
    }

    void Update()
    {
        if (Time.time >= nextIncrementTime && initialMaxEnemies < maxEnemies)
        {
            initialMaxEnemies += maxEnemiesIncrement;
            nextIncrementTime = Time.time + incrementInterval;
        }

        if (Time.time >= nextSpawnTime && currentEnemyCount < initialMaxEnemies)
        {
            SpawnEnemies();
            nextSpawnTime = Time.time + spawnInterval;
        }

        if (Time.time >= nextBossSpawnTime)
        {
            SpawnBoss();
            nextBossSpawnTime = Time.time + bossSpawnInterval;
        }
    }

    void SpawnEnemies()
    {
        for (int i = 0; i < spawnCount; i++)
        {
            if (currentEnemyCount >= initialMaxEnemies) break;

            Transform spawnPoint = spawnPoints[Random.Range(0, spawnPoints.Length)];
            Vector3 spawnPosition = spawnPoint.position + new Vector3(Random.Range(-spawnRange, spawnRange), Random.Range(-spawnRange, spawnRange), 0);
            GameObject enemy = Instantiate(enemyPrefab, spawnPosition, spawnPoint.rotation);
            enemies.Add(enemy);
            currentEnemyCount++;
            enemy.GetComponent<LevelTest_HealthSystem>().OnDied += () => { currentEnemyCount--; enemies.Remove(enemy); };
        }
    }

    void SpawnBoss()
    {
        Transform spawnPoint = spawnPoints[Random.Range(0, spawnPoints.Length)];
        Vector3 spawnPosition = spawnPoint.position + new Vector3(Random.Range(-spawnRange, spawnRange), Random.Range(-spawnRange, spawnRange), 0);
        GameObject boss = Instantiate(bossPrefab, spawnPosition, spawnPoint.rotation);
        enemies.Add(boss);
        currentEnemyCount++;
        boss.GetComponent<LevelTest_HealthSystem>().OnDied += () => { currentEnemyCount--; enemies.Remove(boss); };
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
