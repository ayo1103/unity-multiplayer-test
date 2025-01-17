using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using Unity.Netcode;
using UnityEngine;

public class EnemySpawner : NetworkBehaviour
{
    [SerializeField] private int maxEnemyCount = 5;
    [SerializeField] private float spawnCooldown = 2;
    [SerializeField] private Transform enemyPrefab;
    [SerializeField] private Vector2 minSpawnPos;
    [SerializeField] private Vector2 maxSpawnPos;

    public List<Transform> enemies = new List<Transform>();
    public List<GameObject> players;

    public override void OnNetworkSpawn()
    {
        if (!IsServer)
        {
            enabled = false;
            return;
        }

        StartCoroutine(SpawnEnemies());

        NetworkManager.Singleton.OnClientConnectedCallback += OnClientConnected;
        NetworkManager.Singleton.OnClientDisconnectCallback += OnClientDisconnected;
    }

    private void OnClientConnected(ulong obj)
    {
        players = new List<GameObject>(GameObject.FindGameObjectsWithTag("Player"));
    }

    private async void OnClientDisconnected(ulong obj)
    {
        await Task.Yield();
        players = new List<GameObject>(GameObject.FindGameObjectsWithTag("Player"));
    }

    private IEnumerator SpawnEnemies()
    {
        while (true)
        {
            if (enemies.Count < maxEnemyCount)
            {
                Vector2 spawnPos = new Vector2(Random.Range(minSpawnPos.x, maxSpawnPos.x),
                    Random.Range(minSpawnPos.y, maxSpawnPos.y));
                Transform enemyTransform = Instantiate(enemyPrefab, spawnPos, Quaternion.identity, transform);
                enemyTransform.GetComponent<Enemy>().enemySpawner = this;
                enemyTransform.GetComponent<NetworkObject>().Spawn(true);
                enemies.Add(enemyTransform);

                yield return new WaitForSeconds(spawnCooldown);
            }

            yield return null;
        }
    }
}