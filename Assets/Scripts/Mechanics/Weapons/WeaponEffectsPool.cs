using UnityEngine;

public class WeaponEffectsPool : MonoBehaviour
{
    public static WeaponEffectsPool Instance { get; private set; }

    [SerializeField] private int _poolSize = 10;

    [Header("Chain Lightning (Zapper)")]
    [SerializeField] private Color _lightningColor = new Color(0.5f, 0.9f, 1f, 1f);
    [SerializeField] private float _lightningWidth = 0.12f;
    [SerializeField] private float _lightningDuration = 0.15f;

    [Header("Area Explosion")]
    [SerializeField] private Color _explosionColor = new Color(1f, 0.5f, 0.1f, 1f);
    [SerializeField] private float _explosionWidth = 0.2f;
    [SerializeField] private float _explosionDuration = 0.35f;

    private ObjectPool<ChainLightningEffect> _lightningPool;
    private ObjectPool<AreaExplosionEffect> _explosionPool;

    private void Awake()
    {
        if (Instance != null && Instance != this) { Destroy(gameObject); return; }
        Instance = this;

        _lightningPool = new ObjectPool<ChainLightningEffect>(CreateLightningTemplate(), _poolSize, transform);
        _explosionPool = new ObjectPool<AreaExplosionEffect>(CreateExplosionTemplate(), _poolSize, transform);
    }

    public void SpawnChainLightning(Vector2 from, Vector2 to)
    {
        ChainLightningEffect effect = _lightningPool.Get();
        effect.Initialize(from, to, _lightningDuration, () => _lightningPool.Return(effect));
    }

    public void SpawnAreaExplosion(Vector2 center, float radius)
    {
        AreaExplosionEffect effect = _explosionPool.Get();
        effect.Initialize(center, radius, _explosionDuration, () => _explosionPool.Return(effect));
    }

    private ChainLightningEffect CreateLightningTemplate()
    {
        GameObject go = new GameObject("ChainLightningEffect");
        go.transform.SetParent(transform);
        go.SetActive(false);

        LineRenderer lr = go.AddComponent<LineRenderer>();
        ConfigureLineRenderer(lr, _lightningColor, _lightningWidth);

        return go.AddComponent<ChainLightningEffect>();
    }

    private AreaExplosionEffect CreateExplosionTemplate()
    {
        GameObject go = new GameObject("AreaExplosionEffect");
        go.transform.SetParent(transform);
        go.SetActive(false);

        LineRenderer lr = go.AddComponent<LineRenderer>();
        ConfigureLineRenderer(lr, _explosionColor, _explosionWidth);

        return go.AddComponent<AreaExplosionEffect>();
    }

    private static void ConfigureLineRenderer(LineRenderer lr, Color color, float width)
    {
        lr.useWorldSpace = true;
        lr.material = new Material(Shader.Find("Sprites/Default"));
        lr.startColor = color;
        lr.endColor = color;
        lr.startWidth = width;
        lr.endWidth = width;
        lr.numCapVertices = 4;
        lr.sortingOrder = 10;
    }
}
