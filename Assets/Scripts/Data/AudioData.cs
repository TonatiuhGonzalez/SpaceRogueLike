using UnityEngine;

[CreateAssetMenu(fileName = "AudioData", menuName = "Game/Audio Data")]
public class AudioData : ScriptableObject
{
    [Header("Music")]
    public AudioClip MenuMusic;
    public AudioClip GameplayMusic;

    [Header("Player")]
    public AudioClip ShipHit;
    public AudioClip PlayerDeath;
    public AudioClip LowHealthAlarm;

    [Header("Weapons")]
    public AudioClip ShootDefault;
    public AudioClip Impact;

    [Header("Enemies")]
    public AudioClip EnemyExplosion;
    public AudioClip EnemySpawn;

    [Header("Items")]
    public AudioClip HealthPackDrop;
    public AudioClip HealthPickup;
    public AudioClip VampireAbsorb;

    [Header("Level")]
    public AudioClip LevelComplete;

    [Header("UI")]
    public AudioClip ButtonClick;

    [Header("UI - Pause Menu")]
    public AudioClip PauseOpen;
    public AudioClip PauseClose;
    public AudioClip ExitConfirm;
}
