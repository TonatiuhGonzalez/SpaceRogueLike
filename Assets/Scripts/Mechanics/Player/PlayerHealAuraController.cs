using UnityEngine;

public class PlayerHealAuraController : MonoBehaviour
{
    [SerializeField] private PlayerHealth _playerHealth;
    [SerializeField] private VampiricHealAura _auraPrefab;
    [SerializeField] private int _poolSize = 6;
    [SerializeField] private Vector2 _localOffset = Vector2.zero;

    private ObjectPool<VampiricHealAura> _pool;

    private void Awake()
    {
        _pool = new ObjectPool<VampiricHealAura>(_auraPrefab, _poolSize, transform);
    }

    private void OnEnable()
    {
        _playerHealth.OnHealed += HandleHealed;
    }

    private void OnDisable()
    {
        _playerHealth.OnHealed -= HandleHealed;
    }

    private void HandleHealed()
    {
        VampiricHealAura aura = _pool.Get();
        aura.transform.localPosition = _localOffset;
        aura.Initialize(() => _pool.Return(aura));
    }
}
