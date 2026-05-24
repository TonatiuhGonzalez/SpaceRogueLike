using UnityEngine;

public class EnemyShooter : MonoBehaviour
{
    [SerializeField] private ProjectileManager _projectileManager;
    [SerializeField] private AudioData _audioData;
    [SerializeField] private LayerMask _playerLayer;

    private EnemyData _data;
    private Transform _playerTransform;
    private float _cooldown;
    private float _fireInterval;

    public void Initialize(EnemyData data, Transform playerTransform, float fireRateMultiplier)
    {
        _data = data;
        _playerTransform = playerTransform;
        _fireInterval = data.BaseFireRate > 0f ? 1f / (data.BaseFireRate * fireRateMultiplier) : 1f;
        _cooldown = _fireInterval;
    }

    public void TryShoot()
    {
        if (_data == null || _playerTransform == null) return;

        _cooldown -= Time.deltaTime;
        if (_cooldown > 0f) return;

        float sqrDist = ((Vector2)_playerTransform.position - (Vector2)transform.position).sqrMagnitude;
        if (sqrDist > _data.BaseRange * _data.BaseRange) return;

        Vector2 direction = ((Vector2)_playerTransform.position - (Vector2)transform.position).normalized;

        _projectileManager.Spawn(
            _data.ProjectilePrefab,
            transform.position,
            direction,
            _data.BaseProjectileSpeed,
            _data.BaseDamage,
            _playerLayer);

        AudioManager.Instance.PlaySFX(_audioData.ShootDefault);
        _cooldown = _fireInterval;
    }

    private void OnDisable()
    {
        _cooldown = 0f;
    }
}
