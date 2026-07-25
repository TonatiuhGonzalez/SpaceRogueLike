using UnityEngine;

public class EnemySurroundState : IState
{
    private readonly EnemyMovement _movement;
    private readonly EnemyShooter _shooter;
    private readonly Transform _player;
    private readonly float _assignedAngle;
    private readonly EnemyAITier _tier;

    private float _currentRadius;
    private const float TARGET_RADIUS = 4f;
    private const float CLOSE_SPEED = 0.5f;
    private const float MIN_RADIUS = 2f;
    private const float ARRIVAL_THRESHOLD_SQR = 0.16f;

    private float _dodgeCheckTimer;
    private const float DODGE_CHECK_INTERVAL = 0.15f;
    private Vector2 _dodgeOffset;
    private LayerMask _projectileLayer;

    public EnemySurroundState(EnemyMovement movement, EnemyShooter shooter,
        Transform player, float assignedAngle, EnemyAITier tier)
    {
        _movement = movement;
        _shooter = shooter;
        _player = player;
        _assignedAngle = assignedAngle;
        _tier = tier;
        _currentRadius = TARGET_RADIUS;
        _projectileLayer = LayerMask.GetMask("PlayerProjectile");
    }

    public void Enter() { }

    public void Update()
    {
        if (_player == null) return;

        if (_tier == EnemyAITier.Tier5_Maximum)
        {
            _dodgeCheckTimer -= Time.deltaTime;
            if (_dodgeCheckTimer <= 0f)
            {
                _dodgeCheckTimer = DODGE_CHECK_INTERVAL;
                _dodgeOffset = CalculateDodgeOffset();
            }
        }

        _currentRadius = Mathf.Max(MIN_RADIUS, _currentRadius - CLOSE_SPEED * Time.deltaTime);

        Vector2 orbitOffset = new Vector2(
            Mathf.Cos(_assignedAngle * Mathf.Deg2Rad),
            Mathf.Sin(_assignedAngle * Mathf.Deg2Rad)) * _currentRadius;

        Vector2 targetPos = (Vector2)_player.position + orbitOffset + _dodgeOffset;
        float sqrDist = ((Vector2)_movement.Position - targetPos).sqrMagnitude;

        if (sqrDist > ARRIVAL_THRESHOLD_SQR)
            _movement.MoveToward(targetPos);
        else
            _movement.Stop();

        _shooter.TryShoot();
    }

    public void Exit() => _movement.Stop();

    private Vector2 CalculateDodgeOffset()
    {
        Collider2D[] hits = Physics2D.OverlapCircleAll(
            _movement.Position, 2f, _projectileLayer);

        if (hits.Length == 0) return Vector2.zero;

        Vector2 avoidance = Vector2.zero;
        for (int i = 0; i < hits.Length; i++)
        {
            Vector2 away = _movement.Position - (Vector2)hits[i].transform.position;
            avoidance += away.normalized;
        }
        return avoidance.normalized * 1.5f;
    }
}
