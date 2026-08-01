using UnityEngine;

public class EnemyController : MonoBehaviour
{
    [SerializeField] private EnemyData _data;
    [SerializeField] private EnemyHealth _health;
    [SerializeField] private EnemyMovement _movement;
    [SerializeField] private EnemyShooter _shooter;
    [SerializeField] private SpriteRenderer _spriteRenderer;

    private StateMachine _stateMachine;
    private EnemyPool _ownerPool;
    private Transform _playerTransform;
    private EnemyFormationController _formationController;

    public EnemyData Data => _data;
    public EnemyHealth Health => _health;

    public void Initialize(
        EnemyData data,
        EnemyAITier tier,
        float hpMultiplier,
        float speedMultiplier,
        float fireRateMultiplier,
        Transform playerTransform,
        EnemyPool ownerPool,
        RunConfig runConfig,
        ProjectileManager projectileManager,
        EnemyFormationController formationController,
        BoxCollider2D mapBounds,
        float assignedAngle = 0f)
    {
        _data = data;
        _ownerPool = ownerPool;
        _playerTransform = playerTransform;
        _formationController = formationController;

        _health.Initialize(data.BaseHp * hpMultiplier);
        _movement.Initialize(data.BaseSpeed * speedMultiplier);
        _movement.FormationAngle = assignedAngle;
        _shooter.Initialize(data, playerTransform, fireRateMultiplier, projectileManager);

        _stateMachine = new StateMachine();
        _stateMachine.SetState(BuildInitialState(tier, playerTransform, runConfig, mapBounds));

        _health.OnDied += HandleDied;

        if (IsFormationEnemy())
            _formationController?.Register(_movement);
    }

    private bool IsFormationEnemy()
    {
        return _data.Type != EnemyType.Chaser
            && _data.Type != EnemyType.Warper
            && _data.Type != EnemyType.Sniper;
    }

    private void Update()
    {
        _stateMachine?.Update();
        RotateTowardPlayer();
    }

    private void RotateTowardPlayer()
    {
        if (_playerTransform == null) return;
        Vector2 direction = ((Vector2)_playerTransform.position - (Vector2)transform.position).normalized;
        float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg - 90f;
        transform.rotation = Quaternion.Euler(0f, 0f, angle);
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (_data.Type != EnemyType.Chaser) return;
        if (!other.CompareTag("Player")) return;

        if (other.TryGetComponent<IDamageable>(out var damageable))
            damageable.TakeDamage(_data.BaseDamage);

        _health.ForceKill();
    }

    private void OnDisable()
    {
        if (_health != null)
            _health.OnDied -= HandleDied;
        _formationController?.Unregister(_movement);
        _stateMachine = null;
    }

    private void HandleDied()
    {
        Vector2 deathPosition = transform.position;
        Vector2 deathSize = _spriteRenderer.bounds.size;
        EnemyExplosionPool.Instance?.SpawnExplosion(deathPosition, deathSize);

        _ownerPool?.Return(this);
    }

    private IState BuildInitialState(
        EnemyAITier tier,
        Transform playerTransform,
        RunConfig runConfig,
        BoxCollider2D mapBounds)
    {
        return _data.Type switch
        {
            EnemyType.Chaser => new ChaserRushState(_movement, _health, playerTransform),
            EnemyType.Warper => new WarpState(_movement, _shooter, playerTransform, _data, mapBounds),
            EnemyType.Sniper => new SniperKeepRangeState(_movement, _shooter, playerTransform, _data),
            _ => tier switch
            {
                EnemyAITier.Tier1_Basic => new EnemyChaseState(_movement, _shooter, playerTransform, _data),
                EnemyAITier.Tier2_Dodge => new EnemyDodgeState(_movement, _shooter, playerTransform, _data),
                EnemyAITier.Tier3_Flank => new EnemyFlankState(_movement, _shooter, playerTransform),
                EnemyAITier.Tier4_Surround => new EnemySurroundState(_movement, _shooter, playerTransform, tier),
                EnemyAITier.Tier5_Maximum => new EnemySurroundState(_movement, _shooter, playerTransform, tier),
                _ => new EnemyChaseState(_movement, _shooter, playerTransform, _data)
            }
        };
    }
}
