using UnityEngine;
using TMPro;
using System.Linq;
using System.Collections.Generic;

public class ScreenSettingsManager : MonoBehaviour
{
    [SerializeField] private TMP_Dropdown _resolutionDropdown;
    [SerializeField] private TMP_Dropdown _screenModeDropdown;

    private Resolution[] _resolutions;

    void Awake()
    {
        InitResolutions();
        InitScreenModes();
        LoadSettings();
    }

    void InitResolutions()
    {
        _resolutions = Screen.resolutions
            .GroupBy(r => new Vector2Int(r.width, r.height))
            .Select(g => g.OrderByDescending(r => r.refreshRateRatio.value).First())
            .OrderByDescending(r => r.width * r.height)
            .ToArray();

        var options = _resolutions
            .Select(r => $"{r.width} x {r.height}")
            .ToList();

        _resolutionDropdown.ClearOptions();
        _resolutionDropdown.AddOptions(options);
    }

    void InitScreenModes()
    {
        var modes = new List<string> { "Полноэкранный", "Безрамочный", "Оконный" };
        _screenModeDropdown.ClearOptions();
        _screenModeDropdown.AddOptions(modes);
    }

    public void ApplySettings()
    {
        int resIndex = _resolutionDropdown.value;
        int modeIndex = _screenModeDropdown.value;

        Resolution res = _resolutions[resIndex];

        FullScreenMode mode = modeIndex switch
        {
            0 => FullScreenMode.ExclusiveFullScreen,
            1 => FullScreenMode.FullScreenWindow,
            2 => FullScreenMode.Windowed,
            _ => FullScreenMode.FullScreenWindow
        };

        Screen.fullScreenMode = mode;
        Screen.SetResolution(res.width, res.height, mode);

        Cursor.lockState = (mode == FullScreenMode.ExclusiveFullScreen)
            ? CursorLockMode.Confined
            : CursorLockMode.None;

        //PlayerPrefs.DeleteAll();
        //PlayerPrefs.DeleteKey("FinalMessage_Stage2_Shown");

        PlayerPrefs.SetInt("ScreenResWidth", res.width);
        PlayerPrefs.SetInt("ScreenResHeight", res.height);
        PlayerPrefs.SetInt("ScreenModeIndex", modeIndex);
        PlayerPrefs.Save();
    }

    void LoadSettings()
    {
        int savedW = PlayerPrefs.GetInt("ScreenResWidth", Screen.currentResolution.width);
        int savedH = PlayerPrefs.GetInt("ScreenResHeight", Screen.currentResolution.height);
        int savedM = PlayerPrefs.GetInt("ScreenModeIndex", 1);

        int resIndex = System.Array.FindIndex(
            _resolutions,
            r => r.width == savedW && r.height == savedH);

        if (resIndex == -1) resIndex = 0;

        _resolutionDropdown.value = resIndex;
        _screenModeDropdown.value = savedM;

        _resolutionDropdown.RefreshShownValue();
        _screenModeDropdown.RefreshShownValue();

        ApplySettings();
    }
}
