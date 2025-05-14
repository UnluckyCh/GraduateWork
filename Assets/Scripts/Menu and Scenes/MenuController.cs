using UnityEngine;

public class MenuController : MonoBehaviour
{
    public GameObject _buttonsMenu;
    public GameObject _difficultyMenu;
    public GameObject _levelSelection;
    public GameObject _settingsMenu;

    [SerializeField] AudioSource _clickSound;

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

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape) && _currentMenu != MenuState.Main)
        {
            GoBack();
            if (_clickSound)
            {
                _clickSound.Play();
            }
        }
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

    public void HideAllMenus()
    {
        _buttonsMenu.SetActive(false);
        _difficultyMenu.SetActive(false);
        _levelSelection.SetActive(false);
        _settingsMenu.SetActive(false);
    }

    private void ActivateMenu(GameObject target)
    {
        _buttonsMenu.SetActive(false);
        _difficultyMenu.SetActive(false);
        _levelSelection.SetActive(false);
        _settingsMenu.SetActive(false);

        target.SetActive(true);
    }

    public void QuitGame()
    {
        Debug.Log("Выход из игры!");
        Application.Quit();
    }
}
