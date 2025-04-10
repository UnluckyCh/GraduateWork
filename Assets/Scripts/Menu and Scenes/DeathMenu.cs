using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;
using UnityEngine.UI;

public class DeathMenu : MonoBehaviour
{
    public Canvas deathCanvas;
    public AudioSource menuClickSound;
    public AudioSource firstSound;
    public AudioSource eventSound;
    public Button[] buttons;
    public PauseMenu pauseMenu;

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
        Cursor.visible = true;
        deathCanvas.enabled = true;
        foreach (Button button in buttons)
        {
            button.interactable = true;
        }
        StartCoroutine(FadeOutSound());
    }

    public void RestartGame()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }

    public void LoadMenu()
    {
        SceneManager.LoadScene(0);
    }

    public void QuitGame()
    {
        Debug.Log("����� �� ����!");
        Application.Quit();
    }

    IEnumerator FadeOutSound()
    {
        float startVolume = eventSound.volume;
        float startVolume2 = firstSound.volume;

        // ����������� ���������� ���������
        while (eventSound.volume > 0 && firstSound.volume > 0)
        {
            eventSound.volume -= startVolume * Time.deltaTime / 3f;
            firstSound.volume -= startVolume2 * Time.deltaTime / 3f;
            yield return null;
        }

        eventSound.Stop();
        eventSound.volume = startVolume;
        firstSound.Stop();
        firstSound.volume = startVolume2;
    }
}
