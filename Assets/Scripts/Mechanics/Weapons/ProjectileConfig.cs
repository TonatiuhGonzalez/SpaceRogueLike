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
    public float ChainDelay;

    public float PoisonTickPercent;
    public PoisonMode PoisonMode;
    public int PoisonMaxStacks;

    public ProjectileManager ProjectileManager;

    public Action<float> OnHealPlayer;
    public Action OnReturn;
}

public struct ZapperChainRequest
{
    public Collider2D PrimaryHit;
    public Vector2 OriginPosition;
    public float Damage;
    public int ChainCount;
    public float[] ChainDamagePercents;
    public float SearchRadius;
    public float Delay;
    public LayerMask TargetLayer;
}
