using System;
using System.Collections;
using UnityEngine;

public class EnemyExplosionEffect : MonoBehaviour
{
    [SerializeField] private SpriteRenderer _spriteRenderer;
    [SerializeField] private Sprite[] _frames;

    private Action _onReturn;
    private Coroutine _routine;

    public void Initialize(Vector2 position, Vector2 targetSize, float frameDuration, Action onReturn)
    {
        transform.position = position;
        transform.localScale = Vector3.one * CalculateScale(targetSize);
        _onReturn = onReturn;

        if (_routine != null) StopCoroutine(_routine);
        _routine = StartCoroutine(PlayFrames(frameDuration));
    }

    private float CalculateScale(Vector2 targetSize)
    {
        if (_frames == null || _frames.Length == 0) return 1f;

        float nativeWidth = _frames[0].bounds.size.x;
        return nativeWidth > 0f ? targetSize.x / nativeWidth : 1f;
    }

    private IEnumerator PlayFrames(float frameDuration)
    {
        for (int i = 0; i < _frames.Length; i++)
        {
            _spriteRenderer.sprite = _frames[i];
            yield return new WaitForSeconds(frameDuration);
        }

        _routine = null;
        _onReturn?.Invoke();
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
