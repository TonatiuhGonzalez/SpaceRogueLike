using UnityEngine;

public class ChaserRushState : IState
{
    private readonly EnemyMovement _movement;
    private readonly Transform _player;

    public ChaserRushState(EnemyMovement movement, EnemyHealth health, Transform player)
    {
        _movement = movement;
        _player = player;
    }

    public void Enter() { }

    public void Update()
    {
        if (_player == null) return;
        _movement.MoveToward(_player.position);
    }

    public void Exit() => _movement.Stop();
}
