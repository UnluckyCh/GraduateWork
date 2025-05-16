using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;
using UnityEngine.UI;

public class DeathMenu : MonoBehaviour
{
    public Canvas deathCanvas;
    public AudioSource menuClickSound;
    public AudioSource firstSound;
    public Button[] buttons;
    public PauseMenu pauseMenu;

    [Header("Переключатель сцен")]
    [SerializeField] private ScreenFader _screenFader;

    void Awake()
    {
        deathCanvas.enabled = false;
        foreach (Button button in buttons)
        {
            button.interactable = false;
        }
    }

    public void DeathGame()
    {
        if (pauseMenu != null)
        {
            pauseMenu.ResumeGame();
            pauseMenu.BlockPause();
        }

        if (GameStateTracker.Instance)
        {
            GameStateTracker.Instance.StopGame();
        }

        Cursor.visible = true;
        deathCanvas.enabled = true;
        foreach (Button button in buttons)
        {
            button.interactable = true;
        }
    }

    public void RestartGame()
    {
        _screenFader.FadeInAndLoadNextScene(SceneManager.GetActiveScene().buildIndex);
    }

    public void LoadMenu()
    {
        _screenFader.FadeInAndLoadNextScene(0);
    }

    public void QuitGame()
    {
        Debug.Log("Выход из игры!");
        Application.Quit();
    }
}
