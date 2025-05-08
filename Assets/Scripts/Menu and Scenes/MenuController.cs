using UnityEngine;

public class MenuController : MonoBehaviour
{
    public GameObject _buttonsMenu;
    public GameObject _difficultyMenu;
    public GameObject _levelSelection;
    public GameObject _settingsMenu;

    private enum MenuState
    {
        Main,
        Difficulty,
        LevelSelection,
        Settings
    }

    private MenuState _currentMenu;
    private MenuState _previousMenu;

    public void Start()
    {
        ShowMainMenu();
    }

    public void ShowMainMenu()
    {
        _currentMenu = MenuState.Main;
        ActivateMenu(_buttonsMenu);
    }

    public void ShowDifficultyMenu()
    {
        _previousMenu = _currentMenu;
        _currentMenu = MenuState.Difficulty;
        ActivateMenu(_difficultyMenu);
    }

    public void ShowLevelSelection()
    {
        _previousMenu = _currentMenu;
        _currentMenu = MenuState.LevelSelection;
        ActivateMenu(_levelSelection);
    }

    public void ShowSettingsMenu()
    {
        _previousMenu = _currentMenu;
        _currentMenu = MenuState.Settings;
        ActivateMenu(_settingsMenu);
    }

    public void GoBack()
    {
        switch (_previousMenu)
        {
            case MenuState.LevelSelection:
                ShowLevelSelection();
                break;
            default:
                ShowMainMenu();
                break;
        }
    }

    private void ActivateMenu(GameObject target)
    {
        _buttonsMenu.SetActive(false);
        _difficultyMenu.SetActive(false);
        _levelSelection.SetActive(false);
        if (_settingsMenu)
        {
            _settingsMenu.SetActive(false);
        }

        target.SetActive(true);
    }

    public void QuitGame()
    {
        Debug.Log("Выход из игры!");
        Application.Quit();
    }
}
