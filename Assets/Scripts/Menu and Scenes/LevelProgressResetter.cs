using UnityEngine;
using UnityEngine.SceneManagement;

public class LevelProgressResetter : MonoBehaviour
{
    public void ResetLevelProgress()
    {
        int levelsCompleted = PlayerPrefs.GetInt("LevelsCompletedCount", 0);

        for (int i = 0; i <= levelsCompleted; i++)
        {
            PlayerPrefs.DeleteKey($"LevelPassed_{i}");
            PlayerPrefs.DeleteKey($"LevelGems_{i}");
            PlayerPrefs.DeleteKey($"LevelDifficulty_{i}");
        }

        PlayerPrefs.DeleteKey("LevelsCompletedCount");

        PlayerPrefs.Save();
        Debug.Log("Level progress has been reset (difficulty not affected).");
    }
}
