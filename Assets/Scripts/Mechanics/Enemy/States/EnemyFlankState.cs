using UnityEngine;

public class EnemyFlankState : IState
{
    protected readonly EnemyMovement _movement;
    protected readonly EnemyShooter _shooter;
    protected readonly Transform _player;
    protected readonly float _flankAngle;

    private const float FLANK_DISTANCE = 5f;
    private const float ARRIVAL_THRESHOLD_SQR = 0.25f;

    public EnemyFlankState(EnemyMovement movement, EnemyShooter shooter,
        Transform player, float flankAngle)
    {
        _movement = movement;
        _shooter = shooter;
        _player = player;
        _flankAngle = flankAngle;
    }

    public virtual void Enter() { }

    public virtual void Update()
    {
        if (_player == null) return;

        Vector2 targetOffset = new Vector2(
            Mathf.Cos(_flankAngle * Mathf.Deg2Rad),
            Mathf.Sin(_flankAngle * Mathf.Deg2Rad)) * FLANK_DISTANCE;

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
