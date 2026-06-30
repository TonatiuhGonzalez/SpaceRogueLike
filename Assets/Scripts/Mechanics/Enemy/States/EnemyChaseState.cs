using UnityEngine;

public class EnemyChaseState : IState
{
    protected readonly EnemyMovement _movement;
    protected readonly EnemyShooter _shooter;
    protected readonly Transform _player;
    protected readonly float _baseRange;

    public EnemyChaseState(EnemyMovement movement, EnemyShooter shooter,
        Transform player, EnemyData data)
    {
        _movement = movement;
        _shooter = shooter;
        _player = player;
        _baseRange = data.BaseRange;
    }

    public virtual void Enter() { }

    public virtual void Update()
    {
        if (_player == null) return;

        float distance = Vector2.Distance(_movement.Position, _player.position);

        if (distance <= _baseRange)
            _movement.Stop();
        else
            _movement.MoveToward(_player.position);

        _shooter.TryShoot();
    }

    public virtual void Exit() => _movement.Stop();
}
