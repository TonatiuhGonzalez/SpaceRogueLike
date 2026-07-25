using System;
using System.Collections.Generic;
using UnityEngine;

public class ProjectileManager : MonoBehaviour
{
    [SerializeField] private int _defaultPoolSize = 20;

    private readonly Dictionary<Projectile, ObjectPool<Projectile>> _pools = new();

    public Projectile Spawn(
        Projectile prefab,
        Vector2 position,
        Vector2 direction,
        float speed,
        float damage,
        LayerMask targetLayer,
        bool isVampiric = false,
        float vampireHeal = 0f,
        Action<float> onVampiricHeal = null)
    {
        if (prefab == null) return null;

        ObjectPool<Projectile> pool = GetOrCreatePool(prefab);
        Projectile proj = pool.Get();
        proj.transform.position = position;
        proj.Initialize(direction, speed, damage, targetLayer,
            isVampiric, vampireHeal, onVampiricHeal,
            () => pool.Return(proj));
        return proj;
    }

    public void ClearAllProjectiles()
    {
        for (int i = 0; i < transform.childCount; i++)
            transform.GetChild(i).gameObject.SetActive(false);
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
