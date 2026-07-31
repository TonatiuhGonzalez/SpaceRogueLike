using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class HealthBarUI : MonoBehaviour
{
    [Header("Fill References")]
    [SerializeField] private Image _leftFill;
    [SerializeField] private Image _rightFill;

    [Header("Animation")]
    [SerializeField] private float _animationDuration = 0.15f;

    [Header("Color Thresholds")]
    [SerializeField] private Color _normalColor = Color.white;
    [SerializeField] private Color _criticalColor = Color.red;
    [SerializeField, Range(0f, 1f)] private float _criticalThreshold = 0.25f;

    private float _displayedPercent = 1f;
    private Coroutine _animationCoroutine;

    public void SetHealthPercent(float percent)
    {
        percent = Mathf.Clamp01(percent);

        if (_animationCoroutine != null)
            StopCoroutine(_animationCoroutine);

        _animationCoroutine = StartCoroutine(AnimateTo(percent));
    }

    private IEnumerator AnimateTo(float targetPercent)
    {
        float startPercent = _displayedPercent;
        float elapsed = 0f;

        while (elapsed < _animationDuration)
        {
            elapsed += Time.deltaTime;
            float t = Mathf.Clamp01(elapsed / _animationDuration);
            _displayedPercent = Mathf.Lerp(startPercent, targetPercent, t);
            ApplyFill(_displayedPercent);
            yield return null;
        }

        _displayedPercent = targetPercent;
        ApplyFill(_displayedPercent);
        _animationCoroutine = null;
    }

    private void ApplyFill(float percent)
    {
        _leftFill.fillAmount = percent;
        _rightFill.fillAmount = percent;
        ApplyColorForPercent(percent);
    }

    private void ApplyColorForPercent(float percent)
    {
        Color color = percent < _criticalThreshold ? _criticalColor : _normalColor;
        _leftFill.color = color;
        _rightFill.color = color;
    }

    private void OnDisable()
    {
        if (_animationCoroutine != null)
        {
            StopCoroutine(_animationCoroutine);
            _animationCoroutine = null;
        }
    }
}
