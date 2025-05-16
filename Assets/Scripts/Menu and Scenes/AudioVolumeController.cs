using UnityEngine;
using System.Collections.Generic;
using System.Collections;

public class AudioVolumeController : MonoBehaviour
{
    [SerializeField] private Transform musicRoot;
    [SerializeField] private Transform globalSfxRoot;
    [SerializeField] private Transform fadeSfxRoot;

    [SerializeField] private float volumeScale = 0.04f;
    [SerializeField] private float fadeDuration = 3f;
    [SerializeField] private bool enableMusicLoop = true;

    private const string MusicKey = "MusicVolume_0_50";
    private const string SfxKey = "SfxVolume_0_50";

    private readonly List<VolumeEntry> _musicSources = new();
    private readonly List<VolumeEntry> _globalSfx = new();
    private readonly List<VolumeEntry> _fadeSfx = new();

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
        _globalSfx.Clear();
        _fadeSfx.Clear();

        // === Music ===
        if (musicRoot)
            foreach (var s in musicRoot.GetComponentsInChildren<AudioSource>(true))
                AddMusic(s);

        // === SFX, не затухают ===
        if (globalSfxRoot)
            foreach (var s in globalSfxRoot.GetComponentsInChildren<AudioSource>(true))
                _globalSfx.Add(new VolumeEntry(s));

        // === SFX, затухают ===
        if (fadeSfxRoot)
            foreach (var s in fadeSfxRoot.GetComponentsInChildren<AudioSource>(true))
                _fadeSfx.Add(new VolumeEntry(s));

        void AddMusic(AudioSource s)
        {
            if (!s) return;
            s.playOnAwake = false;
            s.loop = false;
            _musicSources.Add(new VolumeEntry(s));
        }
    }

    public void UpdateFromPrefs()
    {
        float musicFactor = PlayerPrefs.GetInt(MusicKey, 25) * volumeScale;
        float sfxFactor = PlayerPrefs.GetInt(SfxKey, 25) * volumeScale;

        void Apply(List<VolumeEntry> list, float factor)
        {
            foreach (var e in list)
                if (e.Source) e.Source.volume = e.BaseVolume * factor;
        }

        Apply(_musicSources, musicFactor);
        Apply(_globalSfx, sfxFactor);
        Apply(_fadeSfx, sfxFactor);
    }

    public void StopCurrentMusic(float durationSec)
    {
        if (_currentTrackPlaying == null) return;

        if (_musicLoopCoroutine != null) StopCoroutine(_musicLoopCoroutine);
        if (_fadeOutCoroutine != null) StopCoroutine(_fadeOutCoroutine);

        // собираем, что нужно затухать: сам трек и все играющие fade-SFX
        List<VolumeEntry> targets = new();
        var musicEntry = _musicSources.Find(e => e.Source == _currentTrackPlaying);
        if (musicEntry != null) targets.Add(musicEntry);

        foreach (var e in _fadeSfx)
            if (e.Source.isPlaying) targets.Add(e);

        _fadeOutCoroutine = StartCoroutine(FadeOutEntries(targets, durationSec));
    }

    private IEnumerator FadeOutEntries(List<VolumeEntry> list, float dur)
    {
        if (dur <= 0f)
        {
            foreach (var e in list) { e.Source.volume = 0f; e.Source.Stop(); }
            yield break;
        }

        // запоминаем стартовые веса
        Dictionary<VolumeEntry, float> startW = new();
        foreach (var e in list)
            startW[e] = e.Source.volume /
                        (e.BaseVolume * PlayerPrefs.GetInt(SfxKey, 25) * volumeScale);

        for (float t = 0f; t < dur; t += Time.unscaledDeltaTime)
        {
            float k = 1f - t / dur;
            foreach (var e in list)
                e.Source.volume =
                    e.BaseVolume *
                    PlayerPrefs.GetInt(SfxKey, 25) * volumeScale *
                    startW[e] * k;                // линейное затухание
            yield return null;
        }

        foreach (var e in list) { e.Source.volume = 0f; e.Source.Stop(); }
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
