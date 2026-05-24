---
description: Audio standard for this Unity 2D mobile project — AudioManager pattern, pooling, AudioMixer setup, and compression settings.
globs: ["Assets/Scripts/**/*.cs", "Assets/Audio/**"]
alwaysApply: false
---

# Audio Skill — Unity 2D Mobile

## AudioManager Pattern

Centralized audio control. Other systems never call `AudioSource.Play()` directly.

```csharp
public class AudioManager : MonoBehaviour
{
    public static AudioManager Instance { get; private set; }

    [Header("Sources")]
    [SerializeField] private AudioSource _musicSource;
    [SerializeField] private AudioSource _sfxSourcePrefab;

    [Header("Mixer")]
    [SerializeField] private AudioMixer _mixer;

    [Header("Config")]
    [SerializeField] private int _sfxPoolSize = 10;

    private ObjectPool<AudioSource> _sfxPool;

    private const string MUSIC_PARAM = "MusicVolume";
    private const string SFX_PARAM   = "SFXVolume";

    private void Awake()
    {
        if (Instance != null) { Destroy(gameObject); return; }
        Instance = this;
        DontDestroyOnLoad(gameObject);
        _sfxPool = new ObjectPool<AudioSource>(_sfxSourcePrefab, _sfxPoolSize, transform);
    }

    public void PlayMusic(AudioClip clip, bool loop = true)
    {
        if (_musicSource.clip == clip) return;
        _musicSource.clip = clip;
        _musicSource.loop = loop;
        _musicSource.Play();
    }

    public void PlaySFX(AudioClip clip, float volume = 1f, float pitch = 1f)
    {
        if (clip == null) return;
        var source = _sfxPool.Get();
        source.clip = clip;
        source.volume = volume;
        source.pitch = pitch;
        source.Play();
        StartCoroutine(ReturnWhenDone(source, clip.length / pitch));
    }

    public void StopMusic() => _musicSource.Stop();

    public void SetMusicVolume(float normalizedVolume)
    {
        float db = normalizedVolume > 0.001f ? Mathf.Log10(normalizedVolume) * 20f : -80f;
        _mixer.SetFloat(MUSIC_PARAM, db);
    }

    public void SetSFXVolume(float normalizedVolume)
    {
        float db = normalizedVolume > 0.001f ? Mathf.Log10(normalizedVolume) * 20f : -80f;
        _mixer.SetFloat(SFX_PARAM, db);
    }

    private IEnumerator ReturnWhenDone(AudioSource source, float duration)
    {
        yield return new WaitForSeconds(duration);
        _sfxPool.Return(source);
    }
}
```

---

## AudioData ScriptableObject

Group clips by category to avoid scattered AudioClip references:

```csharp
[CreateAssetMenu(fileName = "AudioData", menuName = "Game/Audio Data")]
public class AudioData : ScriptableObject
{
    [Header("Music")]
    public AudioClip MainMenuMusic;
    public AudioClip GameplayMusic;

    [Header("UI")]
    public AudioClip ButtonClick;
    public AudioClip PanelOpen;

    [Header("Gameplay")]
    public AudioClip PlayerJump;
    public AudioClip PlayerHit;
    public AudioClip EnemyDie;
    public AudioClip CoinCollect;
}
```

---

## AudioMixer Setup

Create one AudioMixer with three groups:

```
Master
├── Music
└── SFX
```

Expose `MusicVolume` and `SFXVolume` parameters for runtime control.
Route `_musicSource` to the Music group and SFX pool sources to the SFX group.

---

## Usage (from other scripts)

```csharp
// From any MonoBehaviour
[SerializeField] private AudioData _audioData;

private void OnJump()
{
    AudioManager.Instance.PlaySFX(_audioData.PlayerJump);
}

private void OnSceneLoaded()
{
    AudioManager.Instance.PlayMusic(_audioData.GameplayMusic);
}
```

---

## Compression Settings

| Type | Format | Load Type | Bitrate |
|------|--------|-----------|---------|
| Background music | Vorbis | Streaming | 128 kbps |
| Short SFX < 1s | PCM | Decompress On Load | — |
| Medium SFX 1–5s | Vorbis | Compressed In Memory | 96 kbps |

---

## Rules

- No direct `AudioSource.Play()` outside of AudioManager
- No `AudioClip` references scattered across MonoBehaviours — use AudioData SO
- Always pool SFX sources — never Instantiate AudioSource per sound
- Music never restarts if the same clip is already playing
- Volume stored in `PlayerPrefs` and restored on launch
- Mute/unmute via mixer parameter — never `AudioListener.pause`
