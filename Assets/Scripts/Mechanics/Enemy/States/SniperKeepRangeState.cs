using UnityEngine;

public class SniperKeepRangeState : IState
{
    private readonly EnemyMovement _movement;
    private readonly EnemyShooter _shooter;
    private readonly Transform _player;
    private readonly EnemyData _data;

    private const float RANGE_TOLERANCE = 0.5f;

    public SniperKeepRangeState(EnemyMovement movement, EnemyShooter shooter,
        Transform player, EnemyData data)
    {
        _movement = movement;
        _shooter = shooter;
        _player = player;
        _data = data;
    }

    public void Enter() { }

    public void Update()
    {
        if (_player == null) return;

        float desiredRange = _data.BaseRange * 0.9f;
        float currentDist = Vector2.Distance(_movement.Position, _player.position);

        if (currentDist < desiredRange - RANGE_TOLERANCE)
        {
            Vector2 awayDir = (_movement.Position - (Vector2)_player.position).normalized;
            _movement.MoveToward(_movement.Position + awayDir * 2f);
        }
        else if (currentDist > desiredRange + RANGE_TOLERANCE)
        {
            _movement.MoveToward(_player.position);
        }
        else
        {
            _movement.Stop();
        }

        _shooter.TryShoot();
    }

    public void Exit() => _movement.Stop();
}
