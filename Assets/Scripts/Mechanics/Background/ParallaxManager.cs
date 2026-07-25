using UnityEngine;

public class ParallaxManager : MonoBehaviour
{
    [Header("Reference")]
    [SerializeField] private Transform _referenceTransform;

    [Header("Layers")]
    [SerializeField] private ParallaxLayer _farLayer;
    [SerializeField] private ParallaxLayer _midLayer;
    [SerializeField] private ParallaxLayer _nearLayer;

    private const float FAR_LAYER_MULTIPLIER = 0.2f;
    private const float MID_LAYER_MULTIPLIER = 0.5f;
    private const float NEAR_LAYER_MULTIPLIER = 0.8f;

    private void Awake()
    {
        if (_referenceTransform == null && Camera.main != null)
            _referenceTransform = Camera.main.transform;

        InitializeLayer(_farLayer, FAR_LAYER_MULTIPLIER);
        InitializeLayer(_midLayer, MID_LAYER_MULTIPLIER);
        InitializeLayer(_nearLayer, NEAR_LAYER_MULTIPLIER);
    }

    private void InitializeLayer(ParallaxLayer layer, float multiplier)
    {
        if (layer == null) return;

        layer.SetScrollSpeedMultiplier(multiplier);
        if (_referenceTransform != null)
            layer.SetReferenceTransform(_referenceTransform);
    }
}
