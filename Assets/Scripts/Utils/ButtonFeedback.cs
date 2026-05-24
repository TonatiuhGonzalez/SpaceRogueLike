using System.Collections;
using UnityEngine;
using UnityEngine.EventSystems;

public class ButtonFeedback : MonoBehaviour, IPointerDownHandler, IPointerUpHandler
{
    [SerializeField] private float _punchScale = 0.9f;
    [SerializeField] private float _duration = 0.1f;

    private Vector3 _originalScale;

    private void Awake() => _originalScale = transform.localScale;

    public void OnPointerDown(PointerEventData eventData)
    {
        StopAllCoroutines();
        StartCoroutine(ScaleTo(_originalScale * _punchScale));
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        StopAllCoroutines();
        StartCoroutine(ScaleTo(_originalScale));
    }

    private IEnumerator ScaleTo(Vector3 target)
    {
        float elapsed = 0f;
        Vector3 start = transform.localScale;
        while (elapsed < _duration)
        {
            transform.localScale = Vector3.Lerp(start, target, elapsed / _duration);
            elapsed += Time.deltaTime;
            yield return null;
        }
        transform.localScale = target;
    }
}
