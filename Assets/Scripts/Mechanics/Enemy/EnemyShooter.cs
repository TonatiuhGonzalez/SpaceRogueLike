using UnityEngine;

public class EnemyShooter : MonoBehaviour
{
    [SerializeField] private AudioData _audioData;
    [SerializeField] private LayerMask _playerLayer;

    private ProjectileManager _projectileManager;
    private EnemyData _data;
    private Transform _playerTransform;
    private float _cooldown;
    private float _fireInterval;

    public void Initialize(EnemyData data, Transform playerTransform, float fireRateMultiplier, ProjectileManager projectileManager)
    {
        _data = data;
        _playerTransform = playerTransform;
        _projectileManager = projectileManager;
        _fireInterval = data.BaseFireRate > 0f ? 1f / (data.BaseFireRate * fireRateMultiplier) : 1f;
        _cooldown = Random.Range(0f, _fireInterval);
    }

    public void TryShoot()
    {
        if (_data == null || _playerTransform == null) return;

        float sqrDist = ((Vector2)_playerTransform.position - (Vector2)transform.position).sqrMagnitude;
        if (sqrDist > _data.BaseRange * _data.BaseRange) return;

        Debug.Log($"fireInterval={_fireInterval} cooldown={_cooldown} después de spawn: {_cooldown + _fireInterval}");
        _cooldown -= Time.deltaTime;
        if (_cooldown > 0f) return;

        Vector2 direction = ((Vector2)_playerTransform.position - (Vector2)transform.position).normalized;

        _projectileManager.Spawn(
            _data.ProjectilePrefab,
            transform.position,
            direction,
            _data.BaseProjectileSpeed,
            _data.BaseDamage,
            _playerLayer);

        AudioManager.Instance.PlaySFX(_audioData.ShootDefault);
        _cooldown += _fireInterval;
        Debug.Log($"cooldown reseteado a: {_cooldown}");
    }

    private void OnDisable()
    {
        _cooldown = _fireInterval;
    }
}
