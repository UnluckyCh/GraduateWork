using UnityEngine;
using UnityEngine.SceneManagement;

public class LevelProgressSaver : MonoBehaviour
{
    [SerializeField] private GemCounter _gemCounter;

    public void SaveCurrentLevelProgress()
    {
        int levelIndex = SceneManager.GetActiveScene().buildIndex;
        int newGemsCollected = _gemCounter.CurrentGemsCollected;
        int newDifficulty = PlayerPrefs.GetInt("Difficulty", 0);

        PlayerPrefs.SetInt($"LevelPassed_{levelIndex}", 1);

        string gemsKey = $"LevelGems_{levelIndex}";
        int savedGems = PlayerPrefs.GetInt(gemsKey, 0);
        if (newGemsCollected > savedGems)
        {
            PlayerPrefs.SetInt(gemsKey, newGemsCollected);
        }

        string difficultyKey = $"LevelDifficulty_{levelIndex}";
        int savedDifficulty = PlayerPrefs.GetInt(difficultyKey, 0);
        if (newDifficulty > savedDifficulty)
        {
            PlayerPrefs.SetInt(difficultyKey, newDifficulty);
            PlayerPrefs.SetInt(gemsKey, newGemsCollected);
        }

        int currentCompleted = PlayerPrefs.GetInt("LevelsCompletedCount", 0);
        if (currentCompleted < levelIndex)
        {
            PlayerPrefs.SetInt("LevelsCompletedCount", levelIndex);
        }

        PlayerPrefs.Save();
    }
}
