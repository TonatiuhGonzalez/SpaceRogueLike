using System;
using System.Collections;
using UnityEngine;

public class AreaExplosionEffect : MonoBehaviour
{
    private const int SEGMENTS = 32;

    private LineRenderer _lineRenderer;
    private Action _onReturn;
    private Coroutine _routine;
    private float _targetRadius;
    private float _duration;

    private void Awake()
    {
        _lineRenderer = GetComponent<LineRenderer>();
        _lineRenderer.loop = true;
        _lineRenderer.positionCount = SEGMENTS;
        _lineRenderer.useWorldSpace = true;
    }

    public void Initialize(Vector2 center, float radius, float duration, Action onReturn)
    {
        transform.position = center;
        _targetRadius = radius;
        _duration = duration;
        _onReturn = onReturn;

        if (_routine != null) StopCoroutine(_routine);
        _routine = StartCoroutine(ExpandAndReturn());
    }

    private IEnumerator ExpandAndReturn()
    {
        Color baseColor = _lineRenderer.startColor;
        float elapsed = 0f;

        while (elapsed < _duration)
        {
            elapsed += Time.deltaTime;
            float t = elapsed / _duration;
            DrawRing(_targetRadius * t);
            SetAlpha(baseColor, 1f - t);
            yield return null;
        }

        _routine = null;
        _onReturn?.Invoke();
    }

    private void DrawRing(float radius)
    {
        for (int i = 0; i < SEGMENTS; i++)
        {
            float angle = (i / (float)SEGMENTS) * Mathf.PI * 2f;
            Vector3 point = new Vector3(Mathf.Cos(angle), Mathf.Sin(angle), 0f) * radius;
            _lineRenderer.SetPosition(i, transform.position + point);
        }
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
