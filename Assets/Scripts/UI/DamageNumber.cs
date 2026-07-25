using System;
using System.Collections;
using TMPro;
using UnityEngine;

public class DamageNumber : MonoBehaviour
{
    private const float FLOAT_DISTANCE = 1.2f;
    private const float DURATION = 0.8f;

    [SerializeField] private TextMeshProUGUI _text;

    private RectTransform _rectTransform;
    private Action _onReturn;
    private Coroutine _animationCoroutine;

    private void Awake()
    {
        _rectTransform = GetComponent<RectTransform>();
    }

    private static readonly Color _damageColor = Color.white;
    private static readonly Color _healColor = new Color(0.2f, 1f, 0.2f);

    public void Initialize(Vector2 localPos, float amount, bool isHeal, Action onReturn)
    {
        _onReturn = onReturn;
        _rectTransform.anchoredPosition = localPos;
        _text.color = isHeal ? _healColor : _damageColor;
        _text.text = isHeal ? $"+{amount:0}" : $"{amount:0}";

        if (_animationCoroutine != null)
            StopCoroutine(_animationCoroutine);
        _animationCoroutine = StartCoroutine(AnimateAndReturn());
    }

    private IEnumerator AnimateAndReturn()
    {
        Vector2 startPos = _rectTransform.anchoredPosition;
        Vector2 endPos = startPos + Vector2.up * FLOAT_DISTANCE * 100f;
        float elapsed = 0f;

        while (elapsed < DURATION)
        {
            elapsed += Time.deltaTime;
            float t = elapsed / DURATION;
            _rectTransform.anchoredPosition = Vector2.Lerp(startPos, endPos, t);
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
