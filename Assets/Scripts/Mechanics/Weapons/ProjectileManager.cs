using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ProjectileManager : MonoBehaviour
{
    [SerializeField] private int _defaultPoolSize = 20;

    private readonly Dictionary<Projectile, ObjectPool<Projectile>> _pools = new();

    public Projectile Spawn(Projectile prefab, Vector2 position, ProjectileConfig config)
    {
        if (prefab == null) return null;

        ObjectPool<Projectile> pool = GetOrCreatePool(prefab);
        Projectile proj = pool.Get();
        proj.transform.position = position;

        config.OnReturn = () => pool.Return(proj);
        config.ProjectileManager = this;
        proj.Initialize(config);
        return proj;
    }

    public void ClearAllProjectiles()
    {
        for (int i = 0; i < transform.childCount; i++)
            transform.GetChild(i).gameObject.SetActive(false);
    }

    public void RunZapperChain(ZapperChainRequest request)
    {
        StartCoroutine(ZapperChainRoutine(request));
    }

    private IEnumerator ZapperChainRoutine(ZapperChainRequest request)
    {
        var hitSoFar = new HashSet<Collider2D> { request.PrimaryHit };
        Vector2 fromPosition = request.OriginPosition;

        for (int i = 0; i < request.ChainCount; i++)
        {
            yield return new WaitForSeconds(request.Delay);

            Collider2D target = FindNearestChainTarget(
                fromPosition, hitSoFar, request.SearchRadius, request.TargetLayer);
            if (target == null) yield break;

            WeaponEffectsPool.Instance.SpawnChainLightning(fromPosition, target.transform.position);

            float pct = i < request.ChainDamagePercents.Length
                ? request.ChainDamagePercents[i]
                : 0f;
            float chainDamage = request.Damage * pct;

            if (target.TryGetComponent<IDamageable>(out var damageable))
                damageable.TakeDamage(chainDamage);
            DamageNumberPool.Instance.Spawn(target.transform.position, chainDamage, false);

            hitSoFar.Add(target);
            fromPosition = target.transform.position;
        }
    }

    private static Collider2D FindNearestChainTarget(
        Vector2 fromPosition, HashSet<Collider2D> exclude, float radius, LayerMask targetLayer)
    {
        Collider2D[] nearby = Physics2D.OverlapCircleAll(fromPosition, radius, targetLayer);

        Collider2D nearest = null;
        float nearestSqrDistance = float.MaxValue;

        foreach (Collider2D col in nearby)
        {
            if (exclude.Contains(col)) continue;
            if (!col.TryGetComponent<IDamageable>(out _)) continue;

            float sqrDistance = ((Vector2)col.transform.position - fromPosition).sqrMagnitude;
            if (sqrDistance < nearestSqrDistance)
            {
                nearestSqrDistance = sqrDistance;
                nearest = col;
            }
        }

        return nearest;
    }

    private ObjectPool<Projectile> GetOrCreatePool(Projectile prefab)
    {
        if (!_pools.TryGetValue(prefab, out ObjectPool<Projectile> pool))
        {
            pool = new ObjectPool<Projectile>(prefab, _defaultPoolSize, transform);
            _pools[prefab] = pool;
        }
        return pool;
    }
}
