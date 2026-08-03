using System.Collections;
using UnityEngine;
using UnityEngine.Audio;

public class AudioManager : MonoBehaviour
{
    public static AudioManager Instance { get; private set; }

    [Header("Sources")]
    [SerializeField] private AudioSource _musicSource;
    [SerializeField] private AudioSourceWrapper _sfxSourcePrefab;

    [Header("Mixer")]
    [SerializeField] private AudioMixer _mixer;

    [Header("Pool")]
    [SerializeField] private int _sfxPoolSize = 12;

    private ObjectPool<AudioSourceWrapper> _sfxPool;

    private const string MUSIC_PARAM = "MusicVolume";
    private const string SFX_PARAM = "SFXVolume";
    private const string PREF_MUSIC_ENABLED = "MusicEnabled";
    private const string PREF_SFX_ENABLED = "SFXEnabled";

    public bool IsMusicEnabled { get; private set; }
    public bool IsSFXEnabled { get; private set; }

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
        DontDestroyOnLoad(gameObject);
        _sfxPool = new ObjectPool<AudioSourceWrapper>(_sfxSourcePrefab, _sfxPoolSize, transform);

        IsMusicEnabled = PlayerPrefs.GetInt(PREF_MUSIC_ENABLED, 1) == 1;
        IsSFXEnabled = PlayerPrefs.GetInt(PREF_SFX_ENABLED, 1) == 1;
        SetMusicVolume(IsMusicEnabled ? 1f : 0f);
        SetSFXVolume(IsSFXEnabled ? 1f : 0f);
    }

    public void SetMusicEnabled(bool enabled)
    {
        IsMusicEnabled = enabled;
        SetMusicVolume(enabled ? 1f : 0f);
        PlayerPrefs.SetInt(PREF_MUSIC_ENABLED, enabled ? 1 : 0);
    }

    public void SetSFXEnabled(bool enabled)
    {
        IsSFXEnabled = enabled;
        SetSFXVolume(enabled ? 1f : 0f);
        PlayerPrefs.SetInt(PREF_SFX_ENABLED, enabled ? 1 : 0);
    }

    public void PlaySFX(AudioClip clip, float volume = 1f, float pitch = 1f)
    {
        if (clip == null) return;
        AudioSourceWrapper wrapper = _sfxPool.Get();
        wrapper.Source.clip = clip;
        wrapper.Source.volume = volume;
        wrapper.Source.pitch = pitch;
        wrapper.Source.Play();
        StartCoroutine(ReturnWhenDone(wrapper, clip.length / pitch));
    }

    public void PlayMusic(AudioClip clip, bool loop = true)
    {
        if (clip == null || _musicSource.clip == clip) return;
        _musicSource.clip = clip;
        _musicSource.loop = loop;
        _musicSource.Play();
    }

    public void StopMusic() => _musicSource.Stop();

    public void SetMusicVolume(float normalized)
    {
        float db = normalized > 0.001f ? Mathf.Log10(normalized) * 20f : -80f;
        _mixer.SetFloat(MUSIC_PARAM, db);
    }

    public void SetSFXVolume(float normalized)
    {
        float db = normalized > 0.001f ? Mathf.Log10(normalized) * 20f : -80f;
        _mixer.SetFloat(SFX_PARAM, db);
    }

    private IEnumerator ReturnWhenDone(AudioSourceWrapper wrapper, float duration)
    {
        yield return new WaitForSeconds(duration);
        _sfxPool.Return(wrapper);
    }
}
