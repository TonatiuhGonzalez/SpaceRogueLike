using System;
using UnityEngine;

public struct ProjectileConfig
{
    public Vector2 Direction;
    public float Speed;
    public float Damage;
    public LayerMask TargetLayer;
    public float BulletSizeMultiplier;

    public float VampiricHealPercent;

    public float ExplosionRadius;
    public float ExplosionDamagePercent;

    public int ChainCount;
    public float[] ChainDamagePercents;
    public float ChainSearchRadius;

    public float PoisonTickPercent;
    public PoisonMode PoisonMode;
    public int PoisonMaxStacks;

    public Action<float> OnHealPlayer;
    public Action OnReturn;
}
