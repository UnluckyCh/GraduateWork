using UnityEngine;
using System.Collections.Generic;
using System.Collections;

public class AudioVolumeController : MonoBehaviour
{
    [SerializeField] private Transform musicRoot;
    [SerializeField] private Transform sfxRoot;

    [SerializeField] private float volumeScale = 0.04f;
    [SerializeField] private float fadeDuration = 3f;
    [SerializeField] private bool enableMusicLoop = true;

    private const string MusicKey = "MusicVolume_0_50";
    private const string SfxKey = "SfxVolume_0_50";

    private readonly List<VolumeEntry> _musicSources = new();
    private readonly List<VolumeEntry> _sfxSources = new();

    private Coroutine _musicLoopCoroutine;
    private int _currentMusicIndex = 0;

    private AudioSource _currentTrackPlaying;
    private Coroutine _fadeOutCoroutine;
    private float CurrentMusicFactor => PlayerPrefs.GetInt(MusicKey, 25) * volumeScale;

    private void Start()
    {
        CollectAudioSources();
        UpdateFromPrefs();

        if (enableMusicLoop && _musicSources.Count > 0)
        {
            _musicLoopCoroutine = StartCoroutine(MusicLoopCoroutine());
        }
    }

    private void CollectAudioSources()
    {
        _musicSources.Clear();
        _sfxSources.Clear();

        if (musicRoot)
        {
            foreach (var source in musicRoot.GetComponentsInChildren<AudioSource>(true))
            {
                if (!source) continue;
                source.playOnAwake = false;
                source.loop = false;
                _musicSources.Add(new VolumeEntry(source));
            }
        }

        if (sfxRoot)
        {
            foreach (var source in sfxRoot.GetComponentsInChildren<AudioSource>(true))
            {
                if (!source) continue;
                _sfxSources.Add(new VolumeEntry(source));
            }
        }
    }

    public void UpdateFromPrefs()
    {
        float musicFactor = (PlayerPrefs.HasKey(MusicKey) ? PlayerPrefs.GetInt(MusicKey) : 25) * volumeScale;
        float sfxFactor = (PlayerPrefs.HasKey(SfxKey) ? PlayerPrefs.GetInt(SfxKey) : 25) * volumeScale;

        foreach (var entry in _musicSources)
        {
            if (entry.Source) { entry.Source.volume = entry.BaseVolume * musicFactor; }
        }

        foreach (var entry in _sfxSources)
        {
            if (entry.Source) { entry.Source.volume = entry.BaseVolume * sfxFactor; }
        }
    }

    public void StopCurrentMusic(float durationSec)
    {
        if (_currentTrackPlaying == null) return;

        // ищем объект VolumeEntry для текущего AudioSource
        VolumeEntry entry = _musicSources.Find(e => e.Source == _currentTrackPlaying);
        if (entry == null) return;

        if (_musicLoopCoroutine != null) StopCoroutine(_musicLoopCoroutine);
        if (_fadeOutCoroutine != null) StopCoroutine(_fadeOutCoroutine);

        _fadeOutCoroutine = StartCoroutine(FadeOutAndStop(entry, durationSec));
    }

    private IEnumerator FadeOutAndStop(VolumeEntry entry, float dur)
    {
        if (dur <= 0f)
        {
            entry.Source.volume = 0f;
            entry.Source.Stop();
            yield break;
        }

        float startWeight = entry.Source.volume /
                            (entry.BaseVolume * CurrentMusicFactor);

        for (float t = 0f; t < dur; t += Time.unscaledDeltaTime)
        {
            float w = Mathf.Lerp(startWeight, 0f, t / dur);
            entry.Source.volume = entry.BaseVolume * CurrentMusicFactor * w;
            yield return null;
        }

        entry.Source.volume = 0f;
        entry.Source.Stop();
    }

    private IEnumerator MusicLoopCoroutine()
    {
        while (true)
        {
            VolumeEntry cur = _musicSources[_currentMusicIndex];
            PrepareSource(cur.Source);
            cur.Source.volume = 0f;
            _currentTrackPlaying = cur.Source;
            cur.Source.Play();

            // fade-in
            yield return Fade(cur, 0f, 1f, fadeDuration);

            // ждём до кросс-фейда (реальное время!)
            float wait = Mathf.Max(0f, cur.Source.clip.length - fadeDuration);
            yield return new WaitForSecondsRealtime(wait);

            // выбираем следующий трек
            int nextIndex = (_currentMusicIndex + 1) % _musicSources.Count;
            VolumeEntry next = _musicSources[nextIndex];

            if (_musicSources.Count == 1)
            {
                yield return Fade(cur, 1f, 0f, fadeDuration);
                cur.Source.time = 0f;
                yield return Fade(cur, 0f, 1f, fadeDuration);
                continue;
            }

            PrepareSource(next.Source);
            next.Source.volume = 0f;
            next.Source.Play();

            // кросс-фейд
            float t = 0f;
            while (t < fadeDuration)
            {
                t += Time.unscaledDeltaTime;
                float k = t / fadeDuration;

                float curW = Mathf.Lerp(1f, 0f, k);
                float nextW = Mathf.Lerp(0f, 1f, k);

                cur.Source.volume = cur.BaseVolume * CurrentMusicFactor * curW;
                next.Source.volume = next.BaseVolume * CurrentMusicFactor * nextW;

                yield return null;
            }

            cur.Source.Stop();
            _currentMusicIndex = nextIndex;
        }
    }

    private void PrepareSource(AudioSource src)
    {
        src.loop = false;
        src.spatialBlend = 0f;
    }

    private IEnumerator Fade(VolumeEntry entry,
                             float fromWeight,
                             float toWeight,
                             float dur)
    {
        for (float t = 0f; t < dur; t += Time.unscaledDeltaTime)
        {
            float w = Mathf.Lerp(fromWeight, toWeight, t / dur);
            entry.Source.volume = entry.BaseVolume * CurrentMusicFactor * w;
            yield return null;
        }
        entry.Source.volume = entry.BaseVolume * CurrentMusicFactor * toWeight;
    }

    private class VolumeEntry
    {
        public AudioSource Source { get; }
        public float BaseVolume { get; }

        public VolumeEntry(AudioSource source)
        {
            Source = source;
            BaseVolume = source.volume;
        }
    }
}
