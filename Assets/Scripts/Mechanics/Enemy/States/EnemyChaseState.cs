using UnityEngine;

public class EnemyChaseState : IState
{
    protected readonly EnemyMovement _movement;
    protected readonly EnemyShooter _shooter;
    protected readonly Transform _player;

    public EnemyChaseState(EnemyMovement movement, EnemyShooter shooter, Transform player)
    {
        _movement = movement;
        _shooter = shooter;
        _player = player;
    }

    public virtual void Enter() { }

    public virtual void Update()
    {
        if (_player == null) return;
        _movement.MoveToward(_player.position);
        _shooter.TryShoot();
    }

    public virtual void Exit() => _movement.Stop();
}
