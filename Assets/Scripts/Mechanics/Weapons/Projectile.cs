using System;
using UnityEngine;

public class Projectile : MonoBehaviour
{
    private Vector2 _direction;
    private float _speed;
    private float _damage;
    private LayerMask _targetLayer;
    private bool _isVampiric;
    private float _vampireHeal;
    private Action<float> _onVampiricHeal;
    private Action _onReturn;

    public void Initialize(
        Vector2 direction,
        float speed,
        float damage,
        LayerMask targetLayer,
        bool isVampiric = false,
        float vampireHeal = 0f,
        Action<float> onVampiricHeal = null,
        Action onReturn = null)
    {
        _direction = direction.normalized;
        _speed = speed;
        _damage = damage;
        _targetLayer = targetLayer;
        _isVampiric = isVampiric;
        _vampireHeal = vampireHeal;
        _onVampiricHeal = onVampiricHeal;
        _onReturn = onReturn;
    }

    private void Update()
    {
        transform.Translate(_direction * _speed * Time.deltaTime, Space.World);
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (((1 << other.gameObject.layer) & _targetLayer) == 0) return;

        if (other.TryGetComponent<IDamageable>(out var damageable))
            damageable.TakeDamage(_damage);

        if (_isVampiric)
            _onVampiricHeal?.Invoke(_vampireHeal);

        _onReturn?.Invoke();
    }

    private void OnDisable()
    {
        _onReturn = null;
        _onVampiricHeal = null;
    }
}
