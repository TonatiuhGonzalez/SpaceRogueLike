using UnityEngine;

public class DamageNumberPool : MonoBehaviour
{
    public static DamageNumberPool Instance { get; private set; }

    [SerializeField] private DamageNumber _prefab;
    [SerializeField] private int _poolSize = 20;
    [SerializeField] private Transform _poolParent;

    private ObjectPool<DamageNumber> _pool;

    private void Awake()
    {
        if (Instance != null && Instance != this) { Destroy(gameObject); return; }
        Instance = this;
        _pool = new ObjectPool<DamageNumber>(_prefab, _poolSize, _poolParent != null ? _poolParent : transform);
    }

    public void Spawn(Vector2 worldPos, float amount, bool isHeal)
    {
        DamageNumber number = _pool.Get();

        Vector2 screenPos = Camera.main.WorldToScreenPoint(worldPos);

        RectTransformUtility.ScreenPointToLocalPointInRectangle(
            _poolParent as RectTransform,
            screenPos,
            null,
            out Vector2 localPos);

        number.Initialize(localPos, amount, isHeal, () => _pool.Return(number));
    }
}
