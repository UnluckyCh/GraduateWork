using UnityEngine;

public class MenuController : MonoBehaviour
{
    public GameObject buttonsMenu;
    public GameObject difficultyMenu1;
    public GameObject difficultyMenu2;

    public void Start()
    {
        ShowMainMenu();
    }

    public void ShowMainMenu()
    {
        buttonsMenu.SetActive(true);
        difficultyMenu1.SetActive(false);
        difficultyMenu2.SetActive(false);
    }

    public void ShowDifficultyMenu()
    {
        buttonsMenu.SetActive(false);
        difficultyMenu1.SetActive(true);
        difficultyMenu2.SetActive(true);
    }

    public void QuitGame()
    {
        Debug.Log("Выход из игры!");
        Application.Quit();
    }
}
