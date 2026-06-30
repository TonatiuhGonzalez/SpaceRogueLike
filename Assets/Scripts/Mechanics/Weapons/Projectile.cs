using UnityEngine;

public class Projectile : MonoBehaviour
{
    private ProjectileConfig _config;
    private Vector2 _direction;
    private float _speed;

    public void Initialize(ProjectileConfig config)
    {
        _config = config;
        _direction = config.Direction.normalized;
        _speed = config.Speed;

        float angle = Mathf.Atan2(_direction.y, _direction.x) * Mathf.Rad2Deg - 90f;
        transform.rotation = Quaternion.Euler(0f, 0f, angle);

        float scale = Mathf.Max(0.01f, config.BulletSizeMultiplier);
        transform.localScale = Vector3.one * scale;
    }

    private void Update()
    {
        transform.Translate(_direction * _speed * Time.deltaTime, Space.World);
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (((1 << other.gameObject.layer) & _config.TargetLayer) == 0) return;

        if (other.TryGetComponent<IDamageable>(out var damageable))
            damageable.TakeDamage(_config.Damage);

        DamageNumberPool.Instance.Spawn(transform.position, _config.Damage, false);

        ApplyVampiric();
        ApplyArea(other);
        ApplyZapper(other);
        ApplyPoison(other);

        _config.OnReturn?.Invoke();
    }

    private void ApplyVampiric()
    {
        if (_config.VampiricHealPercent <= 0f) return;

        float healAmt = _config.Damage * _config.VampiricHealPercent;
        _config.OnHealPlayer?.Invoke(healAmt);
        DamageNumberPool.Instance.Spawn(transform.position, healAmt, true);
    }

    private void ApplyArea(Collider2D primaryHit)
    {
        if (_config.ExplosionRadius <= 0f) return;

        Collider2D[] hits = Physics2D.OverlapCircleAll(
            transform.position, _config.ExplosionRadius, _config.TargetLayer);

        float splashDamage = _config.Damage * _config.ExplosionDamagePercent;
        foreach (Collider2D col in hits)
        {
            if (col == primaryHit) continue;
            if (col.TryGetComponent<IDamageable>(out var d))
                d.TakeDamage(splashDamage);
        }
    }

    private void ApplyZapper(Collider2D primaryHit)
    {
        if (_config.ChainCount <= 0 || _config.ChainDamagePercents == null) return;

        Collider2D[] nearby = Physics2D.OverlapCircleAll(
            transform.position, _config.ChainSearchRadius, _config.TargetLayer);

        ShuffleInPlace(nearby);

        int chained = 0;
        foreach (Collider2D col in nearby)
        {
            if (chained >= _config.ChainCount) break;
            if (col == primaryHit) continue;
            if (!col.TryGetComponent<IDamageable>(out var d)) continue;

            float pct = chained < _config.ChainDamagePercents.Length
                ? _config.ChainDamagePercents[chained]
                : 0f;

            d.TakeDamage(_config.Damage * pct);
            chained++;
        }
    }

    private void ApplyPoison(Collider2D primaryHit)
    {
        if (_config.PoisonTickPercent <= 0f) return;

        float tickDamage = _config.Damage * _config.PoisonTickPercent;
        primaryHit.GetComponent<PoisonStatus>()?.ApplyPoison(
            tickDamage, _config.PoisonMode, _config.PoisonMaxStacks);
    }

    private static void ShuffleInPlace(Collider2D[] array)
    {
        for (int i = array.Length - 1; i > 0; i--)
        {
            int j = UnityEngine.Random.Range(0, i + 1);
            (array[i], array[j]) = (array[j], array[i]);
        }
    }

    private void OnDisable()
    {
        _config.OnReturn = null;
        _config.OnHealPlayer = null;
        transform.localScale = Vector3.one;
    }
}
