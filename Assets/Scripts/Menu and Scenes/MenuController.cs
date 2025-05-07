using UnityEngine;

public class MenuController : MonoBehaviour
{
    public GameObject _buttonsMenu;
    public GameObject _difficultyMenu;
    public GameObject _levelSelection;

    public void Start()
    {
        ShowMainMenu();
    }

    public void ShowMainMenu()
    {
        _buttonsMenu.SetActive(true);
        _difficultyMenu.SetActive(false);
        _levelSelection.SetActive(false);
    }

    public void ShowDifficultyMenu()
    {
        _buttonsMenu.SetActive(false);
        _difficultyMenu.SetActive(true);
        _levelSelection.SetActive(false);
    }

    public void ShowLevelSelection()
    {
        _buttonsMenu.SetActive(false);
        _difficultyMenu.SetActive(false);
        _levelSelection.SetActive(true);
    }

    public void QuitGame()
    {
        Debug.Log("Выход из игры!");
        Application.Quit();
    }
}
