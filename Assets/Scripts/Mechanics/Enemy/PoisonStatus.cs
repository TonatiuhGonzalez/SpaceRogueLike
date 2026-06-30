using System.Collections;
using UnityEngine;

public class PoisonStatus : MonoBehaviour
{
    [SerializeField] private RunConfig _runConfig;

    private EnemyHealth _health;

    private float _baseTickDamage;
    private PoisonMode _mode;
    private int _maxStacks;
    private int _currentStacks;
    private Coroutine _tickCoroutine;

    private void Awake()
    {
        _health = GetComponent<EnemyHealth>();
    }

    public void ApplyPoison(float tickDamage, PoisonMode mode, int maxStacks)
    {
        _baseTickDamage = tickDamage;
        _mode = mode;
        _maxStacks = maxStacks;

        switch (mode)
        {
            case PoisonMode.Fixed:
                _currentStacks = 1;
                break;

            case PoisonMode.Stackable:
            case PoisonMode.Exponential:
                _currentStacks = Mathf.Min(_currentStacks + 1, _maxStacks);
                break;
        }

        RestartTick();
    }

    private void RestartTick()
    {
        if (_tickCoroutine != null)
            StopCoroutine(_tickCoroutine);

        _tickCoroutine = StartCoroutine(TickLoop());
    }

    private IEnumerator TickLoop()
    {
        float interval = _runConfig != null ? _runConfig.PoisonTickInterval : 1f;

        while (true)
        {
            yield return new WaitForSeconds(interval);

            if (_health == null) yield break;

            float computedTick = ComputeTick();
            _health.TakeDamage(computedTick);
            DamageNumberPool.Instance.Spawn(transform.position, computedTick, false);
        }
    }

    private float ComputeTick()
    {
        return _mode switch
        {
            PoisonMode.Fixed        => _baseTickDamage,
            PoisonMode.Stackable    => _baseTickDamage * _currentStacks,
            PoisonMode.Exponential  => _baseTickDamage * Mathf.Pow(2f, _currentStacks),
            _                       => _baseTickDamage,
        };
    }

    private void OnDisable()
    {
        if (_tickCoroutine != null)
        {
            StopCoroutine(_tickCoroutine);
            _tickCoroutine = null;
        }
        _baseTickDamage = 0f;
        _mode = PoisonMode.None;
        _maxStacks = 0;
        _currentStacks = 0;
    }
}
