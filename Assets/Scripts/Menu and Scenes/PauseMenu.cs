using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class PauseMenu : MonoBehaviour
{
    public Canvas pauseCanvas;
    public AudioSource menuClickSound;

    private bool isPaused = false;
    private bool block = false;
    public Button[] buttons;

    [Header("Переключатель сцен")]
    [SerializeField] private ScreenFader _screenFader;

    void Start()
    {
        Time.timeScale = 1f;
        isPaused = false;
        pauseCanvas.enabled = false;
        foreach (Button button in buttons)
        {
            button.interactable = false;
        }
    }

    void Update()
    {
        if (!block && Input.GetKeyDown(KeyCode.Escape))
        {
            if (isPaused)
            {
                ResumeGame();
            }
            else
            {
                PauseGame();
            }
        }
    }

    public void ResumeGame()
    {
        //Cursor.visible = false;
        Time.timeScale = 1f;
        isPaused = false;
        pauseCanvas.enabled = false;

        foreach (Button button in buttons)
        {
            button.interactable = false;
        }
    }

    void PauseGame()
    {
        Cursor.visible = true;
        Time.timeScale = 0f;
        isPaused = true;
        pauseCanvas.enabled = true;

        foreach (Button button in buttons)
        {
            button.interactable = true;
        }

        menuClickSound.Play();
    }

    public void RestartGame()
    {
        Time.timeScale = 1f;
        _screenFader.FadeInAndLoadNextScene(SceneManager.GetActiveScene().buildIndex);
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
