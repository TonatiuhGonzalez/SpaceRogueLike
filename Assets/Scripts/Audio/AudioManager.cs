using System.Collections;
using UnityEngine;
using UnityEngine.Audio;

public class AudioManager : MonoBehaviour
{
    public static AudioManager Instance { get; private set; }

    [Header("Sources")]
    [SerializeField] private AudioSource _musicSource;
    [SerializeField] private AudioSource _sfxSourcePrefab;

    [Header("Mixer")]
    [SerializeField] private AudioMixer _mixer;

    [Header("Pool")]
    [SerializeField] private int _sfxPoolSize = 12;

    private ObjectPool<AudioSource> _sfxPool;

    private const string MUSIC_PARAM = "MusicVolume";
    private const string SFX_PARAM = "SFXVolume";

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
        DontDestroyOnLoad(gameObject);
        _sfxPool = new ObjectPool<AudioSource>(_sfxSourcePrefab, _sfxPoolSize, transform);
    }

    public void PlaySFX(AudioClip clip, float volume = 1f, float pitch = 1f)
    {
        if (clip == null) return;
        AudioSource source = _sfxPool.Get();
        source.clip = clip;
        source.volume = volume;
        source.pitch = pitch;
        source.Play();
        StartCoroutine(ReturnWhenDone(source, clip.length / pitch));
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

    private IEnumerator ReturnWhenDone(AudioSource source, float duration)
    {
        yield return new WaitForSeconds(duration);
        _sfxPool.Return(source);
    }
}
