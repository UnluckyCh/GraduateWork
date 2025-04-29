using UnityEngine;
using UnityEngine.SceneManagement;

public class MenuController : MonoBehaviour
{
    public GameObject buttonsMenu;
    public GameObject difficultyMenu;

    public void Start()
    {
        ShowMainMenu();
    }

    public void ShowMainMenu()
    {
        buttonsMenu.SetActive(true);
        difficultyMenu.SetActive(false);
    }

    public void ShowDifficultyMenu()
    {
        buttonsMenu.SetActive(false);
        difficultyMenu.SetActive(true);
    }

    public void SetDifficulty(int level)
    {
        PlayerPrefs.SetInt("Difficulty", level);
        SceneManager.LoadScene(0);
    }

    public void QuitGame()
    {
        Debug.Log("Выход из игры!");
        Application.Quit();
    }
}
