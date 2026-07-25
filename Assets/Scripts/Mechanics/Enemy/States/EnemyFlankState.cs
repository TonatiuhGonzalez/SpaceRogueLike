using UnityEngine;

public class EnemyFlankState : IState
{
    protected readonly EnemyMovement _movement;
    protected readonly EnemyShooter _shooter;
    protected readonly Transform _player;

    private const float FLANK_DISTANCE = 5f;
    private const float ARRIVAL_THRESHOLD_SQR = 0.25f;

    public EnemyFlankState(EnemyMovement movement, EnemyShooter shooter, Transform player)
    {
        _movement = movement;
        _shooter = shooter;
        _player = player;
    }

    public virtual void Enter() { }

    public virtual void Update()
    {
        if (_player == null) return;

        Vector2 targetOffset = MathUtils.AngleToDirection(_movement.FormationAngle) * FLANK_DISTANCE;

        Vector2 targetPos = (Vector2)_player.position + targetOffset;
        float sqrDist = ((Vector2)_movement.Position - targetPos).sqrMagnitude;

        if (sqrDist > ARRIVAL_THRESHOLD_SQR)
            _movement.MoveToward(targetPos);
        else
            _movement.Stop();

        _shooter.TryShoot();
    }

    public virtual void Exit() => _movement.Stop();
}
