using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class DamageVignetteUI : MonoBehaviour
{
    [SerializeField] private Image _vignetteImage;
    [SerializeField] private DamageVignetteData _data;
    [SerializeField] private PlayerHealth _playerHealth;

    private float _currentIntensity;
    private Coroutine _fadeCoroutine;

    private void Awake()
    {
        ApplyAlpha(0f);
    }

    private void OnEnable()
    {
        _playerHealth.OnDamageTaken += HandleDamageTaken;
    }

    private void OnDisable()
    {
        _playerHealth.OnDamageTaken -= HandleDamageTaken;

        if (_fadeCoroutine != null)
        {
            StopCoroutine(_fadeCoroutine);
            _fadeCoroutine = null;
        }
    }

    private void HandleDamageTaken(float damageAmount)
    {
        float t = Mathf.InverseLerp(_data.MinDamageThreshold, _data.MaxDamageThreshold, damageAmount);
        float hitIntensity = Mathf.Lerp(0f, _data.MaxOpacity, t);

        _currentIntensity = Mathf.Min(_data.MaxOpacity, _currentIntensity + hitIntensity);
        ApplyAlpha(_currentIntensity);

        if (_fadeCoroutine != null)
            StopCoroutine(_fadeCoroutine);
        _fadeCoroutine = StartCoroutine(FadeOutRoutine());
    }

    private IEnumerator FadeOutRoutine()
    {
        float startIntensity = _currentIntensity;
        float elapsed = 0f;

        while (elapsed < _data.FadeOutDuration)
        {
            elapsed += Time.deltaTime;
            float t = Mathf.Clamp01(elapsed / _data.FadeOutDuration);
            _currentIntensity = Mathf.Lerp(startIntensity, 0f, t);
            ApplyAlpha(_currentIntensity);
            yield return null;
        }

        _currentIntensity = 0f;
        ApplyAlpha(_currentIntensity);
        _fadeCoroutine = null;
    }

    private void ApplyAlpha(float alpha)
    {
        Color color = _data.VignetteColor;
        color.a = alpha;
        _vignetteImage.color = color;
    }
}
