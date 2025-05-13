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

    [Header("RectTransforms для сдвига")]
    [SerializeField] private RectTransform _gemTransform;
    [SerializeField] private RectTransform _healthTransform;

    [Header("Переключатель сцен")]
    [SerializeField] private ScreenFader _screenFader;

    private Vector2 _initialGemPos;
    private Vector2 _initialHealthPos;

    private const float SHIFTX = -50f;

    void Start()
    {
        completedCanvas.enabled = false;

        if (_gemTransform)
        {
            _initialGemPos = _gemTransform.anchoredPosition;
        }

        if (_healthTransform)
        {
            _initialHealthPos = _healthTransform.anchoredPosition;
        }
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
        if (eventSound)
        {
            StartCoroutine(FadeOutSound());
        }
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
        _screenFader.FadeInAndLoadNextScene(SceneManager.GetActiveScene().buildIndex);
    }

    public void LoadNextLevel()
    {
        int currentIndex = SceneManager.GetActiveScene().buildIndex;
        int nextIndex = currentIndex + 1;

        if (nextIndex >= SceneManager.sceneCountInBuildSettings)
        {
            nextIndex = 0;
        }

        _screenFader.FadeInAndLoadNextScene(nextIndex);
    }

    public void LoadMenu()
    {
        _screenFader.FadeInAndLoadNextScene(0);
    }

    void UpdateCompletedText()
    {
        bool isHealthPerfect = false;
        bool isGemsPerfect = false;

        if (countHeartsText)
        {
            var healthUpdater = FindObjectOfType<HealthUIUpdater>();
            if (healthUpdater)
            {
                int currentHealth = healthUpdater.CurrentHealth;
                int maxHealth = healthUpdater.MaxHealth;

                isHealthPerfect = (currentHealth == maxHealth);
                countHeartsText.text = isHealthPerfect ? "Perfect" : $"{currentHealth}/{maxHealth}";
                countHeartsText.fontSize = isHealthPerfect ? 86 : 106;
                if (_healthTransform)
                {
                    _healthTransform.anchoredPosition = isHealthPerfect ? (_initialHealthPos + new Vector2(SHIFTX, 0)) : _initialHealthPos;
                }
            }
        }

        if (countGemsText)
        {
            var gemCounter = FindObjectOfType<GemCounter>();
            if (gemCounter)
            {
                int collected = gemCounter.GetYellowGemsCollected();
                int total = gemCounter.GetTotalYellowGems();

                isGemsPerfect = (collected == total);
                countGemsText.text = isGemsPerfect ? "Perfect" : $"{collected}/{total}";
                countGemsText.fontSize = isGemsPerfect ? 86 : 106;
                if (_gemTransform)
                {
                    _gemTransform.anchoredPosition = isGemsPerfect ? (_initialGemPos + new Vector2(SHIFTX, 0)) : _initialGemPos;
                }
            }
        }
    }
}
