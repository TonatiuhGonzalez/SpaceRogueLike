using UnityEngine;

public class WarpState : IState
{
    private readonly EnemyMovement _movement;
    private readonly EnemyShooter _shooter;
    private readonly Transform _player;
    private readonly EnemyData _data;
    private readonly BoxCollider2D _mapBounds;

    private float _warpTimer;
    private const float WARP_INTERVAL_MIN = 7f;
    private const float WARP_INTERVAL_MAX = 10f;
    private const float RANGE_MARGIN = 5f;

    public WarpState(EnemyMovement movement, EnemyShooter shooter,
        Transform player, EnemyData data, BoxCollider2D mapBounds)
    {
        _movement = movement;
        _shooter = shooter;
        _player = player;
        _data = data;
        _mapBounds = mapBounds;
        _warpTimer = Random.Range(WARP_INTERVAL_MIN, WARP_INTERVAL_MAX);
    }

    public void Enter() { }

    public void Update()
    {
        _warpTimer -= Time.deltaTime;
        if (_warpTimer <= 0f)
        {
            TeleportToSafePosition();
            _warpTimer = Random.Range(WARP_INTERVAL_MIN, WARP_INTERVAL_MAX);
        }

        _shooter.TryShoot();
    }

    public void Exit() => _movement.Stop();

    private void TeleportToSafePosition()
    {
        if (_player == null) return;

        float distance = Mathf.Max(0f, _data.BaseRange - RANGE_MARGIN);
        float angle = Random.Range(0f, Mathf.PI * 2f);
        Vector2 offset = new Vector2(Mathf.Cos(angle), Mathf.Sin(angle)) * distance;
        Vector2 candidate = (Vector2)_player.position + offset;

        if (_mapBounds != null)
            candidate = ClampToBounds(candidate);

        _movement.TeleportTo(candidate);
    }

    private Vector2 ClampToBounds(Vector2 position)
    {
        Bounds bounds = _mapBounds.bounds;
        return new Vector2(
            Mathf.Clamp(position.x, bounds.min.x, bounds.max.x),
            Mathf.Clamp(position.y, bounds.min.y, bounds.max.y));
    }
}
