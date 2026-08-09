using System;
using System.Collections;
using UnityEngine;

public class VampiricHealAura : MonoBehaviour
{
    private const float PULSE_DURATION = 0.4f;

    [SerializeField] private SpriteRenderer _spriteRenderer;

    private static readonly Color _healColor = new Color(0.2f, 1f, 0.2f);

    private Action _onReturn;
    private Coroutine _pulseCoroutine;

    public void Initialize(Action onReturn)
    {
        _onReturn = onReturn;
        SetAlpha(0f);

        if (_pulseCoroutine != null)
            StopCoroutine(_pulseCoroutine);
        _pulseCoroutine = StartCoroutine(PulseAndReturn());
    }

    private IEnumerator PulseAndReturn()
    {
        float halfDuration = PULSE_DURATION * 0.5f;

        float elapsed = 0f;
        while (elapsed < halfDuration)
        {
            elapsed += Time.deltaTime;
            SetAlpha(elapsed / halfDuration);
            yield return null;
        }

        elapsed = 0f;
        while (elapsed < halfDuration)
        {
            elapsed += Time.deltaTime;
            SetAlpha(1f - elapsed / halfDuration);
            yield return null;
        }

        _pulseCoroutine = null;
        _onReturn?.Invoke();
    }

    private void SetAlpha(float alpha)
    {
        Color c = _healColor;
        c.a = alpha;
        _spriteRenderer.color = c;
    }

    private void OnDisable()
    {
        if (_pulseCoroutine != null)
        {
            StopCoroutine(_pulseCoroutine);
            _pulseCoroutine = null;
        }
        _onReturn = null;
    }
}
