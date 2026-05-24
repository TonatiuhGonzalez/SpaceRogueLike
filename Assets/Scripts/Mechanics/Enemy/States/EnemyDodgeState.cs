using UnityEngine;

public class EnemyDodgeState : EnemyChaseState
{
    private const float DODGE_CHECK_RADIUS = 2f;
    private const float DODGE_STRENGTH = 1.5f;
    private float _dodgeCheckTimer;
    private const float DODGE_CHECK_INTERVAL = 0.2f;
    private Vector2 _dodgeOffset;
    private LayerMask _projectileLayer;

    public EnemyDodgeState(EnemyMovement movement, EnemyShooter shooter, Transform player)
        : base(movement, shooter, player)
    {
        _projectileLayer = LayerMask.GetMask("PlayerProjectile");
    }

    public override void Update()
    {
        if (_player == null) return;

        _dodgeCheckTimer -= Time.deltaTime;
        if (_dodgeCheckTimer <= 0f)
        {
            _dodgeCheckTimer = DODGE_CHECK_INTERVAL;
            _dodgeOffset = CalculateDodgeOffset();
        }

        Vector2 target = (Vector2)_player.position + _dodgeOffset;
        _movement.MoveToward(target);
        _shooter.TryShoot();
    }

    private Vector2 CalculateDodgeOffset()
    {
        Collider2D[] hits = Physics2D.OverlapCircleAll(
            _movement.Position, DODGE_CHECK_RADIUS, _projectileLayer);

        if (hits.Length == 0) return Vector2.zero;

        Vector2 avoidance = Vector2.zero;
        for (int i = 0; i < hits.Length; i++)
        {
            Vector2 away = _movement.Position - (Vector2)hits[i].transform.position;
            avoidance += away.normalized;
        }

        return avoidance.normalized * DODGE_STRENGTH;
    }
}
