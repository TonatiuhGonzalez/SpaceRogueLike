using UnityEngine;

public class EnemyShooter : MonoBehaviour
{
    [SerializeField] private AudioData _audioData;
    [SerializeField] private LayerMask _playerLayer;
    [SerializeField] private float _projectileSpawnOffset = 0.5f;

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

        _cooldown -= Time.deltaTime;
        if (_cooldown > 0f) return;

        Vector2 direction = ((Vector2)_playerTransform.position - (Vector2)transform.position).normalized;

        ProjectileConfig config = new()
        {
            Direction    = direction,
            Speed        = _data.BaseProjectileSpeed,
            Damage       = _data.BaseDamage,
            TargetLayer  = _playerLayer,
            BulletSizeMultiplier = 1f,
        };

        Vector2 spawnPosition = (Vector2)transform.position + direction * _projectileSpawnOffset;
        _projectileManager.Spawn(_data.ProjectilePrefab, spawnPosition, config);
        AudioManager.Instance.PlaySFX(_audioData.ShootDefault);
        _cooldown += _fireInterval;
    }

    private void OnDisable()
    {
        _cooldown = _fireInterval;
    }
}
