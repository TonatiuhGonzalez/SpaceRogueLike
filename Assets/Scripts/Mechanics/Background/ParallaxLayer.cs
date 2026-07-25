using UnityEngine;

[RequireComponent(typeof(Renderer))]
public class ParallaxLayer : MonoBehaviour
{
    [Header("Reference")]
    [SerializeField] private Transform _referenceTransform;

    [Header("Scroll")]
    [SerializeField] private float _scrollSpeedMultiplier = 0.5f;
    [SerializeField] private bool _useMaterialOffset;
    [SerializeField] private string _textureProperty = "_MainTex";

    private Renderer _renderer;
    private Material _material;
    private Vector3 _lastReferencePosition;

    private void Awake()
    {
        _renderer = GetComponent<Renderer>();
        if (_useMaterialOffset)
            _material = _renderer.material;

        if (_referenceTransform == null && Camera.main != null)
            _referenceTransform = Camera.main.transform;
    }

    private void OnEnable()
    {
        if (_referenceTransform != null)
            _lastReferencePosition = _referenceTransform.position;
    }

    public void SetReferenceTransform(Transform reference)
    {
        _referenceTransform = reference;
        _lastReferencePosition = reference.position;
    }

    public void SetScrollSpeedMultiplier(float multiplier)
    {
        _scrollSpeedMultiplier = multiplier;
    }

    private void LateUpdate()
    {
        if (_referenceTransform == null) return;

        Vector3 delta = _referenceTransform.position - _lastReferencePosition;
        _lastReferencePosition = _referenceTransform.position;

        if (delta.sqrMagnitude <= 0f) return;

        if (_useMaterialOffset)
        {
            Vector2 offset = _material.GetTextureOffset(_textureProperty);
            offset += (Vector2)delta * _scrollSpeedMultiplier;
            _material.SetTextureOffset(_textureProperty, offset);
        }
        else
        {
            transform.position += delta * _scrollSpeedMultiplier;
        }
    }
}
