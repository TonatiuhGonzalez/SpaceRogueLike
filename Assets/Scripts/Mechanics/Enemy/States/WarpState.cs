using UnityEngine;

public class WarpState : IState
{
    private readonly EnemyMovement _movement;
    private readonly EnemyShooter _shooter;
    private readonly Transform _player;
    private readonly RunConfig _runConfig;

    private float _warpTimer;
    private const float WARP_INTERVAL = 3f;
    private const float MAP_HALF_SIZE = 20f;

    public WarpState(EnemyMovement movement, EnemyShooter shooter,
        Transform player, RunConfig runConfig)
    {
        _movement = movement;
        _shooter = shooter;
        _player = player;
        _runConfig = runConfig;
        _warpTimer = WARP_INTERVAL;
    }

    public void Enter() { }

    public void Update()
    {
        _warpTimer -= Time.deltaTime;
        if (_warpTimer <= 0f)
        {
            TeleportToSafePosition();
            _warpTimer = WARP_INTERVAL;
        }

        _shooter.TryShoot();
    }

    public void Exit() => _movement.Stop();

    private void TeleportToSafePosition()
    {
        float safeZone = _runConfig != null ? _runConfig.WarperSafeZoneRadius : 3f;
        Vector2 candidate;
        int attempts = 0;

        do
        {
            candidate = new Vector2(
                Random.Range(-MAP_HALF_SIZE, MAP_HALF_SIZE),
                Random.Range(-MAP_HALF_SIZE, MAP_HALF_SIZE));
            attempts++;
        }
        while (_player != null &&
               ((Vector2)_player.position - candidate).sqrMagnitude < safeZone * safeZone &&
               attempts < 10);

        _movement.TeleportTo(candidate);
    }
}
