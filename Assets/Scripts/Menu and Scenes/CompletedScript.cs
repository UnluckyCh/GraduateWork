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
        if (pauseMenu != null)
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
        // ��������� ��������� �����
        float startVolume = eventSound.volume;

        // ����������� ���������� ���������
        while (eventSound.volume > 0)
        {
            eventSound.volume -= startVolume * Time.deltaTime / 5f; // ��������� �����
            yield return null;
        }

        // ��������� ��������� � ������������� ����
        eventSound.Stop();
        eventSound.volume = startVolume; // ���������� ��������� �� �������� �������
    }

    public void RestartGame()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }

    public void LoadMenu()
    {
        SceneManager.LoadScene(0);
    }

    void UpdateCompletedText()
    {
        if (countHeartsText != null)
        {
            // �������� ������� � ������������ ��������
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

        if (countGemsText != null)
        {
            // �������� ���������� ��������� ����� �����
            GemCounter gemCounter = FindObjectOfType<GemCounter>();
            if (gemCounter != null)
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
