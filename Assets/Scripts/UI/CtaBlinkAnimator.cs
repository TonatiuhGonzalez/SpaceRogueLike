using TMPro;
using UnityEngine;

public class CtaBlinkAnimator : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI _text;
    [SerializeField] private Color _colorA = Color.white;
    [SerializeField] private Color _colorB = new Color(0.733f, 0.733f, 0.733f);
    [SerializeField] private float _blinkDuration = 1.2f;

    private void Update()
    {
        float t = Mathf.PingPong(Time.time / _blinkDuration, 1f);
        _text.color = Color.Lerp(_colorA, _colorB, t);
    }
}
