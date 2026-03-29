using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.Events;

public class SoundManager : MonoBehaviour
{
    public static SoundManager Instance { get; private set; }

    [Header("Audio Sources")]
    [SerializeField] private AudioSource _bgMusicSource;
    [SerializeField] private AudioSource _keyMusicSource;
    [SerializeField] private AudioSource _sfxSource;
    [SerializeField] private AudioSource _pooledSfxPrefab; // префаб для пула

    [Header("Audio Mixer (optional)")]
    [SerializeField] private AudioMixerGroup _masterMixerGroup;
    [SerializeField] private AudioMixerGroup _musicMixerGroup;
    [SerializeField] private AudioMixerGroup _sfxMixerGroup;

    [Header("Settings")]
    [Range(0f, 1f)] [SerializeField] private float _masterVolume = 1f;
    [Range(0f, 1f)] [SerializeField] private float _musicVolume = 1f;
    [Range(0f, 1f)] [SerializeField] private float _sfxVolume = 1f;
    [SerializeField] private float _crossfadeDuration = 1f;
    [SerializeField] private int _sfxPoolSize = 10;

    [Header("Events")]
    public UnityEvent<float> OnMasterVolumeChanged;
    public UnityEvent<float> OnMusicVolumeChanged;
    public UnityEvent<float> OnSfxVolumeChanged;

    private Queue<AudioSource> _sfxPool = new Queue<AudioSource>();
    private List<AudioSource> _activeSfx = new List<AudioSource>();
    private Coroutine _crossfadeRoutine;

    public float MasterVolume
    {
        get => _masterVolume;
        set
        {
            _masterVolume = Mathf.Clamp01(value);
            ApplyVolumes();
            OnMasterVolumeChanged?.Invoke(_masterVolume);
        }
    }

    public float MusicVolume
    {
        get => _musicVolume;
        set
        {
            _musicVolume = Mathf.Clamp01(value);
            ApplyVolumes();
            OnMusicVolumeChanged?.Invoke(_musicVolume);
        }
    }

    public float SfxVolume
    {
        get => _sfxVolume;
        set
        {
            _sfxVolume = Mathf.Clamp01(value);
            ApplyVolumes();
            OnSfxVolumeChanged?.Invoke(_sfxVolume);
        }
    }

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
        DontDestroyOnLoad(gameObject);

        // Создаём источники, если не заданы
        if (_bgMusicSource == null) _bgMusicSource = gameObject.AddComponent<AudioSource>();
        if (_keyMusicSource == null) _keyMusicSource = gameObject.AddComponent<AudioSource>();
        if (_sfxSource == null) _sfxSource = gameObject.AddComponent<AudioSource>();

        _bgMusicSource.loop = true;
        _bgMusicSource.playOnAwake = false;
        _keyMusicSource.loop = true;
        _keyMusicSource.playOnAwake = false;
        _sfxSource.playOnAwake = false;

        ApplyVolumes();

        // Создаём пул SFX
        if (_pooledSfxPrefab == null)
        {
            GameObject go = new GameObject("PooledAudioSource");
            _pooledSfxPrefab = go.AddComponent<AudioSource>();
            _pooledSfxPrefab.playOnAwake = false;
            go.SetActive(false);
            DontDestroyOnLoad(go);
        }

        for (int i = 0; i < _sfxPoolSize; i++)
        {
            AudioSource source = Instantiate(_pooledSfxPrefab, transform);
            source.gameObject.SetActive(false);
            _sfxPool.Enqueue(source);
        }
    }

    private void ApplyVolumes()
    {
        float master = _masterVolume;
        float music = _musicVolume * master;
        float sfx = _sfxVolume * master;

        if (_masterMixerGroup != null)
        {
            SetMixerVolume(_masterMixerGroup, master);
            SetMixerVolume(_musicMixerGroup, music);
            SetMixerVolume(_sfxMixerGroup, sfx);
        }
        else
        {
            _bgMusicSource.volume = music;
            _keyMusicSource.volume = music;
            _sfxSource.volume = sfx;
        }
    }

    private void SetMixerVolume(AudioMixerGroup group, float value)
    {
        if (group != null)
            group.audioMixer.SetFloat(group.name + "Volume", Mathf.Log10(value) * 20);
    }

    #region Music

    public void PlayBackgroundMusic(AudioClip clip)
    {
        if (_bgMusicSource.clip == clip && _bgMusicSource.isPlaying) return;

        if (_crossfadeRoutine != null) StopCoroutine(_crossfadeRoutine);
        _crossfadeRoutine = StartCoroutine(CrossfadeMusic(_bgMusicSource, clip));
    }

    public void StopBackgroundMusic(float fadeOut = 0.5f)
    {
        StartCoroutine(FadeOutAndStop(_bgMusicSource, fadeOut));
    }

    public void PlayKeyMusic(AudioClip clip)
    {
        if (_keyMusicSource.clip == clip && _keyMusicSource.isPlaying) return;

        StartCoroutine(CrossfadeMusic(_keyMusicSource, clip, true));
    }

    public void StopKeyMusic(float fadeOut = 0.5f)
    {
        StartCoroutine(FadeOutAndStop(_keyMusicSource, fadeOut, true));
    }

    private IEnumerator CrossfadeMusic(AudioSource source, AudioClip newClip, bool isKeyMusic = false)
    {
        float startVolume = source.volume;
        float time = 0;
        while (time < _crossfadeDuration)
        {
            time += Time.deltaTime;
            source.volume = Mathf.Lerp(startVolume, 0, time / _crossfadeDuration);
            yield return null;
        }
        source.Stop();
        source.clip = newClip;
        source.Play();

        time = 0;
        while (time < _crossfadeDuration)
        {
            time += Time.deltaTime;
            source.volume = Mathf.Lerp(0, startVolume, time / _crossfadeDuration);
            yield return null;
        }
        _crossfadeRoutine = null;
    }

    private IEnumerator FadeOutAndStop(AudioSource source, float duration, bool restoreBg = false)
    {
        float startVolume = source.volume;
        float time = 0;
        while (time < duration)
        {
            time += Time.deltaTime;
            source.volume = Mathf.Lerp(startVolume, 0, time / duration);
            yield return null;
        }
        source.Stop();
        source.volume = startVolume;

        if (restoreBg && _bgMusicSource.clip != null && !_bgMusicSource.isPlaying)
            PlayBackgroundMusic(_bgMusicSource.clip);
    }

    #endregion

    #region Sound Effects

    public void PlaySfx(AudioClip clip, Vector3? position = null, float volume = 1f)
    {
        if (clip == null) return;

        AudioSource source = GetPooledSfxSource();
        if (source == null) return;

        source.clip = clip;
        source.volume = volume * _sfxVolume * _masterVolume;
        source.transform.position = position ?? transform.position;

        source.gameObject.SetActive(true);
        source.Play();

        _activeSfx.Add(source);
        StartCoroutine(ReturnToPoolAfterPlay(source));
    }

    private AudioSource GetPooledSfxSource()
    {
        if (_sfxPool.Count > 0)
            return _sfxPool.Dequeue();
        else
        {
            Debug.LogWarning("SFX pool depleted, creating new source");
            AudioSource newSource = Instantiate(_pooledSfxPrefab, transform);
            newSource.gameObject.SetActive(false);
            return newSource;
        }
    }

    private IEnumerator ReturnToPoolAfterPlay(AudioSource source)
    {
        yield return new WaitForSeconds(source.clip.length + 0.1f);
        source.Stop();
        source.gameObject.SetActive(false);
        _activeSfx.Remove(source);
        _sfxPool.Enqueue(source);
    }

    public void PlayOneShot(AudioClip clip, float volumeScale = 1f)
    {
        _sfxSource.PlayOneShot(clip, volumeScale * _sfxVolume * _masterVolume);
    }

    #endregion

    #region Utilities

    public void StopAll()
    {
        _bgMusicSource.Stop();
        _keyMusicSource.Stop();
        foreach (var src in _activeSfx)
            src.Stop();
    }

    #endregion
}