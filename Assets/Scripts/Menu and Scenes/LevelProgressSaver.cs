using UnityEngine;
using UnityEngine.SceneManagement;

public class LevelProgressSaver : MonoBehaviour
{
    [SerializeField] private GemCounter _gemCounter;

    public void SaveCurrentLevelProgress()
    {
        int levelIndex = SceneManager.GetActiveScene().buildIndex;
        int gemsCollected = _gemCounter.CurrentGemsCollected;
        int difficulty = PlayerPrefs.GetInt("Difficulty", 0);

        PlayerPrefs.SetInt($"LevelPassed_{levelIndex}", 1);
        PlayerPrefs.SetInt($"LevelGems_{levelIndex}", gemsCollected);
        PlayerPrefs.SetInt($"LevelDifficulty_{levelIndex}", difficulty);

        int currentCompleted = PlayerPrefs.GetInt("LevelsCompletedCount", 0);
        if (currentCompleted < levelIndex)
        {
            PlayerPrefs.SetInt("LevelsCompletedCount", levelIndex);
        }

        PlayerPrefs.Save();
    }
}
