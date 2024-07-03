using UnityEngine;
using System;
using System.Collections;
using Unity.Netcode;

public class HealthSystem : NetworkBehaviour
{
    public int health;
    public Action OnDied;

    [SerializeField] int maxHealth = 100;

    [SerializeField] private SpriteRenderer _spriteRenderer;
    [SerializeField] private Color _damageColor = Color.red;
    [SerializeField] private float _damageDuration = 0.2f;

    private Coroutine _damageEffectRoutine;

    private void Start()
    {
        health = maxHealth;
    }

    public void OnDamageDealt(int damage)
    {
        health -= damage;

        DamageEffectClientRpc();

        if (health < 0)
        {
            OnDied?.Invoke();
        }
    }

    [ClientRpc]
    private void DamageEffectClientRpc()
    {

        
        if (_damageEffectRoutine != null)
        {
            StopCoroutine(_damageEffectRoutine);
        }

        _damageEffectRoutine = StartCoroutine(DamageEffect());
    }

    private IEnumerator DamageEffect()
    {
        _spriteRenderer.color = _damageColor;
        yield return new WaitForSeconds(_damageDuration);
        _spriteRenderer.color = Color.white;
    }
}