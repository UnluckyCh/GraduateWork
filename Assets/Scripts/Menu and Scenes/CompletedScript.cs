using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;

public class CompletedScript : MonoBehaviour
{
    public Canvas completedCanvas;
    public TextMeshProUGUI countHeartsText;
    public TextMeshProUGUI countGemsText;
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
        if (countHeartsText)
        {
            var healthUpdater = FindObjectOfType<HealthUIUpdater>();
            if (healthUpdater)
            {
                int currentHealth = healthUpdater.CurrentHealth;
                int maxHealth = healthUpdater.MaxHealth;

                bool isHealthPerfect = (currentHealth == maxHealth);
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

                bool isGemsPerfect = (collected == total);
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
