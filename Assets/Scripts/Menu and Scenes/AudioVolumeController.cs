using UnityEngine;
using System.Collections.Generic;

public class AudioVolumeController : MonoBehaviour
{
    [SerializeField] private Transform musicRoot;
    [SerializeField] private Transform sfxRoot;

    [SerializeField] private float volumeScale = 0.04f; // 25 * 0.04 = 1.0

    private const string MusicKey = "MusicVolume_0_50";
    private const string SfxKey = "SfxVolume_0_50";

    private readonly List<VolumeEntry> _musicSources = new();
    private readonly List<VolumeEntry> _sfxSources = new();

    private void Start()
    {
        CollectAudioSources();
        UpdateFromPrefs();
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
