using System;
using UnityEngine;

public class EnemyHealth : MonoBehaviour, IDamageable
{
    private float _maxHealth;
    private float _currentHealth;
    private bool _isDead;

    public event Action OnDied;
    public event Action<Vector2> OnDiedAtPosition;

    public void Initialize(float maxHealth)
    {
        _maxHealth = maxHealth;
        _currentHealth = maxHealth;
        _isDead = false;
    }

    public void TakeDamage(float amount)
    {
        if (_isDead || amount <= 0f) return;

        _currentHealth = Mathf.Max(0f, _currentHealth - amount);

        if (_currentHealth <= 0f)
            Die();
    }

    public void ForceKill()
    {
        if (_isDead) return;
        _currentHealth = 0f;
        Die();
    }

    private void Die()
    {
        _isDead = true;
        OnDied?.Invoke();
        OnDiedAtPosition?.Invoke(transform.position);
    }

    private void OnDisable()
    {
        _isDead = false;
        OnDiedAtPosition = null;
    }
}
