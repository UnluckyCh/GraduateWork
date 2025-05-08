using UnityEngine;
using System.Collections.Generic;

public class AudioVolumeController : MonoBehaviour
{
    [SerializeField] Transform musicRoot;
    [SerializeField] Transform sfxRoot;

    [SerializeField] float volumeScale = 0.04f; // 25 * 0.04 = 1.0 (т.е. 25 Ч нейтральна€ громкость)

    const string MusicKey = "MusicVolume_0_50";
    const string SfxKey = "SfxVolume_0_50";

    private List<VolumeEntry> _musicSources = new();
    private List<VolumeEntry> _sfxSources = new();

    public static AudioVolumeController Instance { get; private set; }

    private void Awake()
    {
        if (Instance) { Destroy(gameObject); return; }
        Instance = this;
        DontDestroyOnLoad(gameObject);

        CollectAudioSources();
        UpdateFromPrefs();
    }

    private void CollectAudioSources()
    {
        _musicSources.Clear();
        _sfxSources.Clear();

        foreach (var source in musicRoot.GetComponentsInChildren<AudioSource>(true))
        {
            if (!source) continue;
            _musicSources.Add(new VolumeEntry(source));
        }

        foreach (var source in sfxRoot.GetComponentsInChildren<AudioSource>(true))
        {
            if (!source) continue;
            _sfxSources.Add(new VolumeEntry(source));
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
