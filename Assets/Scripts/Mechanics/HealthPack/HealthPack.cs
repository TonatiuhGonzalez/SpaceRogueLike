using System;
using UnityEngine;

public class HealthPack : MonoBehaviour
{
    [SerializeField] private float _healAmount = 20f;

    private Action _onReturn;

    public void Initialize(Vector2 position, Action onReturn)
    {
        transform.position = position;
        _onReturn = onReturn;
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (!other.CompareTag("Player")) return;

        if (other.TryGetComponent<PlayerHealth>(out var health))
            health.Heal(_healAmount);

        DamageNumberPool.Instance.Spawn(transform.position, _healAmount, true);

        _onReturn?.Invoke();
    }

    private void OnDisable()
    {
        _onReturn = null;
    }
}
