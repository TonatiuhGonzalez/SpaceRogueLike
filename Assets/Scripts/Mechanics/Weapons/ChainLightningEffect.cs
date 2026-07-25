using System;
using System.Collections;
using UnityEngine;

public class ChainLightningEffect : MonoBehaviour
{
    private const float JAGGED_OFFSET = 0.3f;

    private LineRenderer _lineRenderer;
    private Action _onReturn;
    private Coroutine _routine;
    private float _duration;

    private void Awake()
    {
        _lineRenderer = GetComponent<LineRenderer>();
    }

    public void Initialize(Vector2 from, Vector2 to, float duration, Action onReturn)
    {
        _onReturn = onReturn;
        _duration = duration;
        DrawJaggedLine(from, to);

        if (_routine != null) StopCoroutine(_routine);
        _routine = StartCoroutine(FadeAndReturn());
    }

    private void DrawJaggedLine(Vector2 from, Vector2 to)
    {
        Vector2 direction = (to - from).normalized;
        Vector2 perpendicular = new Vector2(-direction.y, direction.x);
        Vector2 mid = Vector2.Lerp(from, to, 0.5f) +
            perpendicular * UnityEngine.Random.Range(-JAGGED_OFFSET, JAGGED_OFFSET);

        _lineRenderer.positionCount = 3;
        _lineRenderer.SetPosition(0, from);
        _lineRenderer.SetPosition(1, mid);
        _lineRenderer.SetPosition(2, to);
    }

    private IEnumerator FadeAndReturn()
    {
        Color baseColor = _lineRenderer.startColor;
        float elapsed = 0f;

        while (elapsed < _duration)
        {
            elapsed += Time.deltaTime;
            float alpha = 1f - (elapsed / _duration);
            SetAlpha(baseColor, alpha);
            yield return null;
        }

        _routine = null;
        _onReturn?.Invoke();
    }

    private void SetAlpha(Color baseColor, float alpha)
    {
        Color c = baseColor;
        c.a = alpha;
        _lineRenderer.startColor = c;
        _lineRenderer.endColor = c;
    }

    private void OnDisable()
    {
        if (_routine != null)
        {
            StopCoroutine(_routine);
            _routine = null;
        }
        _onReturn = null;
    }
}
