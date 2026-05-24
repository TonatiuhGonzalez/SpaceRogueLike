using UnityEngine;

public class EnemyController : MonoBehaviour
{
    [SerializeField] private EnemyData _data;
    [SerializeField] private EnemyHealth _health;
    [SerializeField] private EnemyMovement _movement;
    [SerializeField] private EnemyShooter _shooter;

    private StateMachine _stateMachine;
    private float _assignedFlankAngle;
    private EnemyPool _ownerPool;

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
        float assignedAngle = 0f)
    {
        _data = data;
        _assignedFlankAngle = assignedAngle;
        _ownerPool = ownerPool;

        _health.Initialize(data.BaseHp * hpMultiplier);
        _movement.Initialize(data.BaseSpeed * speedMultiplier);
        _shooter.Initialize(data, playerTransform, fireRateMultiplier);

        _stateMachine = new StateMachine();
        _stateMachine.SetState(BuildInitialState(tier, statMultiplier, playerTransform, runConfig));

        _health.OnDied += HandleDied;
    }

    private void Update()
    {
        _stateMachine?.Update();
    }

    private void OnDisable()
    {
        if (_health != null)
            _health.OnDied -= HandleDied;
        _stateMachine = null;
    }

    private void HandleDied()
    {
        _ownerPool?.Return(this);
    }

    private IState BuildInitialState(
        EnemyAITier tier,
        float statMultiplier,
        Transform playerTransform,
        RunConfig runConfig)
    {
        return _data.Type switch
        {
            EnemyType.Chaser => new ChaserRushState(_movement, _health, playerTransform),
            EnemyType.Warper => new WarpState(_movement, _shooter, playerTransform, runConfig),
            EnemyType.Sniper => new SniperKeepRangeState(_movement, _shooter, playerTransform, _data),
            _ => tier switch
            {
                EnemyAITier.Tier1_Basic => new EnemyChaseState(_movement, _shooter, playerTransform),
                EnemyAITier.Tier2_Dodge => new EnemyDodgeState(_movement, _shooter, playerTransform),
                EnemyAITier.Tier3_Flank => new EnemyFlankState(_movement, _shooter, playerTransform, _assignedFlankAngle),
                EnemyAITier.Tier4_Surround => new EnemySurroundState(_movement, _shooter, playerTransform, _assignedFlankAngle, tier),
                EnemyAITier.Tier5_Maximum => new EnemySurroundState(_movement, _shooter, playerTransform, _assignedFlankAngle, tier),
                _ => new EnemyChaseState(_movement, _shooter, playerTransform)
            }
        };
    }
}
