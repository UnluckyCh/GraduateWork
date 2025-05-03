using UnityEngine;
using UnityEngine.SceneManagement;

public class LevelStarter : MonoBehaviour
{
    public void SetDifficulty(int level)
    {
        PlayerPrefs.SetInt("Difficulty", level);
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 1);
    }
}
