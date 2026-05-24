using UnityEngine;

public class ChaserRushState : IState
{
    private readonly EnemyMovement _movement;
    private readonly EnemyHealth _health;
    private readonly Transform _player;

    private const float EXPLOSION_RADIUS = 0.5f;
    private const float CONTACT_THRESHOLD_SQR = 0.36f;

    public ChaserRushState(EnemyMovement movement, EnemyHealth health, Transform player)
    {
        _movement = movement;
        _health = health;
        _player = player;
    }

    public void Enter() { }

    public void Update()
    {
        if (_player == null) return;

        _movement.MoveToward(_player.position);

        float sqrDist = ((Vector2)_player.position - _movement.Position).sqrMagnitude;
        if (sqrDist <= CONTACT_THRESHOLD_SQR)
            Explode();
    }

    public void Exit() => _movement.Stop();

    private void Explode()
    {
        Collider2D[] hits = Physics2D.OverlapCircleAll(
            _movement.Position, EXPLOSION_RADIUS, LayerMask.GetMask("Player"));

        foreach (var hit in hits)
        {
            if (hit.TryGetComponent<IDamageable>(out var damageable))
                damageable.TakeDamage(999f);
        }

        _health.TakeDamage(9999f);
    }
}
