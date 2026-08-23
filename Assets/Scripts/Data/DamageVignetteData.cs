using UnityEngine;

[CreateAssetMenu(fileName = "DamageVignetteData", menuName = "Game/UI/Damage Vignette Data")]
public class DamageVignetteData : ScriptableObject
{
    [Header("Damage Range")]
    public float MinDamageThreshold = 8f;
    public float MaxDamageThreshold = 50f;

    [Header("Intensity")]
    [Range(0f, 1f)] public float MaxOpacity = 0.8f;

    [Header("Fade")]
    public float FadeOutDuration = 0.8f;

    [Header("Appearance")]
    public Color VignetteColor = Color.red;
}
