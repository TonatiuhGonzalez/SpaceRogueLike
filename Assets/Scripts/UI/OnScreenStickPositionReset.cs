using UnityEngine;

[RequireComponent(typeof(RectTransform))]
public class OnScreenStickPositionReset : MonoBehaviour
{
    private RectTransform _rectTransform;
    private Vector2 _initialAnchoredPosition;

    private void Awake()
    {
        _rectTransform = (RectTransform)transform;
        _initialAnchoredPosition = _rectTransform.anchoredPosition;
    }

    private void OnEnable()
    {
        _rectTransform.anchoredPosition = _initialAnchoredPosition;
    }
}
