using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;
using TMPro;

public class CompletedScript : MonoBehaviour
{
    public Canvas completedCanvas;
    public TextMeshProUGUI countHeartsText;
    public TextMeshProUGUI countGemsText;
    public AudioSource eventSound;
    public PauseMenu pauseMenu;

    void Start()
    {
        completedCanvas.enabled = false;
    }

    public void CompletedGame()
    {
        if (pauseMenu)
        {
            pauseMenu.ResumeGame();
            pauseMenu.BlockPause();
        }
        Cursor.visible = true;
        completedCanvas.enabled = true;
        UpdateCompletedText();
        StartCoroutine(FadeOutSound());
    }

    IEnumerator FadeOutSound()
    {
        // Начальная громкость звука
        float startVolume = eventSound.volume;

        // Постепенное уменьшение громкости
        while (eventSound.volume > 0)
        {
            eventSound.volume -= startVolume * Time.deltaTime / 5f; // Затухание звука
            yield return null;
        }

        // Завершаем затухание и останавливаем звук
        eventSound.Stop();
        eventSound.volume = startVolume; // Возвращаем громкость на исходный уровень
    }

    public void RestartGame()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }

    public void LoadMenu()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 1);
    }

    void UpdateCompletedText()
    {
        if (countHeartsText != null)
        {
            // Получаем текущее и максимальное здоровье
            HealthUIUpdater healthUpdater = FindObjectOfType<HealthUIUpdater>();
            if (healthUpdater != null)
            {
                int currentHealth = healthUpdater.CurrentHealth;
                int maxHealth = healthUpdater.MaxHealth;
                if (maxHealth == currentHealth)
                {
                    countHeartsText.fontSize = 90;
                    countHeartsText.text = "Perfect";
                }
                else
                {
                    countHeartsText.fontSize = 106;
                    countHeartsText.text = currentHealth + "/" + maxHealth;
                }
            }
        }

        if (countGemsText)
        {
            // Получаем количество собранных жёлтых гемов
            GemCounter gemCounter = FindObjectOfType<GemCounter>();
            if (gemCounter)
            {
                int yellowGemsCollected = gemCounter.GetYellowGemsCollected();
                int totalYellowGems = gemCounter.GetTotalYellowGems();
                if (totalYellowGems == yellowGemsCollected)
                {
                    countGemsText.fontSize = 90;
                    countGemsText.text = "Perfect";
                }
                else
                {
                    countGemsText.fontSize = 106;
                    countGemsText.text = yellowGemsCollected + "/" + totalYellowGems;
                }
            }
        }
    }
}
