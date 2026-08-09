using System;
using UnityEngine;

public class PlayerHealth : MonoBehaviour, IDamageable
{
    private float _maxHealth;
    private float _currentHealth;
    private bool _isDead;

    public float HealthPercent => _maxHealth > 0f ? _currentHealth / _maxHealth : 0f;

    public event Action<float> OnHealthChanged;
    public event Action OnDied;
    public event Action OnHealed;

    public void Initialize(float maxHealth)
    {
        _maxHealth = maxHealth;
        _currentHealth = maxHealth;
        _isDead = false;
        OnHealthChanged?.Invoke(HealthPercent);
    }

    public void TakeDamage(float amount)
    {
        if (_isDead || amount <= 0f) return;

        _currentHealth = Mathf.Max(0f, _currentHealth - amount);
        OnHealthChanged?.Invoke(HealthPercent);

        if (_currentHealth <= 0f)
            Die();
    }

    public void Heal(float amount)
    {
        if (_isDead || amount <= 0f) return;

        _currentHealth = Mathf.Min(_maxHealth, _currentHealth + amount);
        OnHealthChanged?.Invoke(HealthPercent);
        OnHealed?.Invoke();
    }

    private void Die()
    {
        _isDead = true;
        OnDied?.Invoke();
        gameObject.SetActive(false);
    }
}
