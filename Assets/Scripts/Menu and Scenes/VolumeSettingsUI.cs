using UnityEngine;
using UnityEngine.UI;

public class VolumeSettingsUI : MonoBehaviour
{
    [SerializeField] Slider musicSlider;
    [SerializeField] Slider sfxSlider;
    [SerializeField] AudioVolumeController _audioVolumeController;

    const string MusicKey = "MusicVolume_0_50";
    const string SfxKey = "SfxVolume_0_50";

    void Start()
    {
        Init(musicSlider, MusicKey);
        Init(sfxSlider, SfxKey);
        _audioVolumeController.UpdateFromPrefs();
    }

    void Init(Slider slider, string key)
    {
        slider.minValue = 0;
        slider.maxValue = 50;
        slider.wholeNumbers = true;

        slider.value = PlayerPrefs.HasKey(key) ? PlayerPrefs.GetInt(key) : 25;

        slider.onValueChanged.AddListener(v =>
        {
            int iv = (int)v;
            PlayerPrefs.SetInt(key, iv);
            PlayerPrefs.Save();
            _audioVolumeController.UpdateFromPrefs();
        });
    }
}
