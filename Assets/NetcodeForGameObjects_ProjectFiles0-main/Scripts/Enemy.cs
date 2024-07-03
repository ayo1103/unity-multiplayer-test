using System.Collections;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.AI;

[RequireComponent(typeof(HealthSystem))]
public class Enemy : NetworkBehaviour
{
    [SerializeField] float attackRange = 2;
    [SerializeField] int damage = 3;

    public EnemySpawner enemySpawner;
    private NavMeshAgent agent;
    private Transform _player = null;

    public override void OnNetworkSpawn()
    {
        if (!IsServer)
        {
            enabled = false;
            GetComponent<NavMeshAgent>().enabled = false;
            return;
        }

        NetworkManager.Singleton.OnClientDisconnectCallback += OnClientDisconnect;

        agent = GetComponent<NavMeshAgent>();
        agent.updateRotation = false;
        agent.updateUpAxis = false;
    }

    private void OnClientDisconnect(ulong obj)
    {
        _player = null;
    }

    private void Update()
    {
        foreach (var player in enemySpawner.players)
        {
            if (_player == null || Vector2.Distance(player.transform.position, transform.position) <
                Vector2.Distance(_player.position, transform.position))
            {
                _player = player.transform;
            }
        }

        Move();
    }

    private void OnEnable()
    {
        GetComponent<HealthSystem>().OnDied += OnDied;
    }

    private void OnDisable()
    {
        GetComponent<HealthSystem>().OnDied -= OnDied;
    }

    private void OnDied()
    {
        enemySpawner.enemies.Remove(transform);
        Destroy(gameObject);
    }

    private bool canAttack = true;

    private void Move()
    {
        if (_player == null)
            return;

        if (Vector2.Distance(transform.position, _player.transform.position) > attackRange)
        {
            agent.destination = _player.position;
        }
        else if (canAttack)
        {
            StartCoroutine(Attack());
        }
    }

    private IEnumerator Attack()
    {
        canAttack = false;

        _player.GetComponent<HealthSystem>().OnDamageDealt(damage);

        yield return new WaitForSeconds(2);
        canAttack = true;
    }
}