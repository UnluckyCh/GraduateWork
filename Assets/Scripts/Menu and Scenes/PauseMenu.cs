using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class PauseMenu : MonoBehaviour
{
    private enum PauseState
    {
        None,
        Paused,
        Settings
    }

    private PauseState _currentState = PauseState.None;

    public Canvas pauseCanvas;
    public AudioSource menuClickSound;

    [SerializeField] private Canvas _settingsCanvas;

    private bool block = false;
    public Button[] buttons;

    [Header("Переключатель сцен")]
    [SerializeField] private ScreenFader _screenFader;

    void Start()
    {
        Time.timeScale = 1f;
        pauseCanvas.enabled = false;
        foreach (Button button in buttons)
        {
            button.interactable = false;
        }

        if (_settingsCanvas)
        {
            _settingsCanvas.gameObject.SetActive(false);
        }
    }

    void Update()
    {
        if (block) return;

        if (Input.GetKeyDown(KeyCode.Escape))
        {
            switch (_currentState)
            {
                case PauseState.Paused:
                    ResumeGame();
                    menuClickSound.Play();
                    break;

                case PauseState.Settings:
                    BackToPauseMenu();
                    break;

                case PauseState.None:
                    PauseGame();
                    menuClickSound.Play();
                    break;
            }
        }
    }

    void PauseGame()
    {
        Cursor.visible = true;
        Time.timeScale = 0f;
        pauseCanvas.enabled = true;
        _settingsCanvas.gameObject.SetActive(false);

        foreach (Button button in buttons)
        {
            button.interactable = true;
        }

        _currentState = PauseState.Paused;
    }

    public void ResumeGame()
    {
        Time.timeScale = 1f;
        pauseCanvas.enabled = false;
        _settingsCanvas.gameObject.SetActive(false);

        foreach (Button button in buttons)
        {
            button.interactable = false;
        }

        _currentState = PauseState.None;
    }

    public void RestartGame()
    {
        Time.timeScale = 1f;
        _screenFader.FadeInAndLoadNextScene(SceneManager.GetActiveScene().buildIndex);
    }

    public void OpenSettingMenu()
    {
        pauseCanvas.enabled = false;
        _settingsCanvas.gameObject.SetActive(true);

        _currentState = PauseState.Settings;

        menuClickSound.Play();
    }

    public void BackToPauseMenu()
    {
        _settingsCanvas.gameObject.SetActive(false);
        pauseCanvas.enabled = true;

        _currentState = PauseState.Paused;

        menuClickSound.Play();
    }

    public void LoadMenu()
    {
        _screenFader.FadeInAndLoadNextScene(0);
        Time.timeScale = 1f;
    }

    public void QuitGame()
    {
        Debug.Log("Выход из игры!");
        Application.Quit();
    }

    public void BlockPause()
    {
        block = true;
    }

    public void UnblockPause()
    {
        block = false;
    }
}
