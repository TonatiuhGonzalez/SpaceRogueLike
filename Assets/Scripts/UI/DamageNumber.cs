using System;
using System.Collections;
using TMPro;
using UnityEngine;

public class DamageNumber : MonoBehaviour
{
    private const float FLOAT_DISTANCE = 1.2f;
    private const float DURATION = 0.8f;

    [SerializeField] private TextMeshProUGUI _text;

    private Action _onReturn;
    private Coroutine _animationCoroutine;

    private static readonly Color _damageColor = Color.white;
    private static readonly Color _healColor = new Color(0.2f, 1f, 0.2f);

    public void Initialize(Vector3 worldPos, float amount, bool isHeal, Action onReturn)
    {
        _onReturn = onReturn;
        transform.position = worldPos;
        _text.color = isHeal ? _healColor : _damageColor;
        _text.text = isHeal ? $"+{amount:0}" : $"{amount:0}";

        if (_animationCoroutine != null)
            StopCoroutine(_animationCoroutine);
        _animationCoroutine = StartCoroutine(AnimateAndReturn());
    }

    private IEnumerator AnimateAndReturn()
    {
        Vector3 startPos = transform.position;
        Vector3 endPos = startPos + Vector3.up * FLOAT_DISTANCE;
        float elapsed = 0f;

        while (elapsed < DURATION)
        {
            elapsed += Time.deltaTime;
            float t = elapsed / DURATION;
            transform.position = Vector3.Lerp(startPos, endPos, t);
            float alpha = 1f - t;
            Color c = _text.color;
            c.a = alpha;
            _text.color = c;
            yield return null;
        }

        _animationCoroutine = null;
        _onReturn?.Invoke();
    }

    private void OnDisable()
    {
        if (_animationCoroutine != null)
        {
            StopCoroutine(_animationCoroutine);
            _animationCoroutine = null;
        }
        _onReturn = null;
    }
}
