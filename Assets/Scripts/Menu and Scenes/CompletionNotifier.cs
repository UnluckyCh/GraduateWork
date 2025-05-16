using UnityEngine;
using UnityEngine.SceneManagement;

public class CompletionNotifier : MonoBehaviour
{
    [SerializeField] private Canvas notificationCanvas;
    [SerializeField] private GameObject messageNormal;
    [SerializeField] private GameObject messagePerfect;
    [SerializeField] private MenuController menuController;

    private const string NotificationStage1Key = "FinalMessage_Stage1_Shown";
    private const string NotificationStage2Key = "FinalMessage_Stage2_Shown";

    private void Start()
    {
        if (!AllLevelsPassed())
        {
            return;
        }

        bool perfect = AllLevelsPerfect(3);

        if (PlayerPrefs.HasKey(NotificationStage2Key))
        {
            return;
        }

        if (perfect)
        {
            PlayerPrefs.SetInt(NotificationStage1Key, 1);
            PlayerPrefs.SetInt(NotificationStage2Key, 1);
            PlayerPrefs.Save();

            menuController.HideAllMenus();
            notificationCanvas.gameObject.SetActive(true);
            messagePerfect.SetActive(true);
            messageNormal.SetActive(false);
        }
        else if (!PlayerPrefs.HasKey(NotificationStage1Key))
        {
            PlayerPrefs.SetInt(NotificationStage1Key, 1);
            PlayerPrefs.Save();

            menuController.HideAllMenus();
            notificationCanvas.gameObject.SetActive(true);
            messagePerfect.SetActive(false);
            messageNormal.SetActive(true);
        }
    }

    private bool AllLevelsPassed()
    {
        int totalLevels = SceneManager.sceneCountInBuildSettings - 1;
        for (int i = 1; i <= totalLevels; i++)
        {
            if (PlayerPrefs.GetInt($"LevelPassed_{i}", 0) != 1)
            {
                return false;
            }
        }
        return true;
    }

    private bool AllLevelsPerfect(int requiredDifficulty)
    {
        int totalLevels = SceneManager.sceneCountInBuildSettings - 1;
        for (int i = 1; i <= totalLevels; i++)
        {
            int difficulty = PlayerPrefs.GetInt($"LevelDifficulty_{i}", 0);
            int gems = PlayerPrefs.GetInt($"LevelGems_{i}", 0);

            if (difficulty < requiredDifficulty || gems < 3)
            {
                return false;
            }
        }
        return true;
    }
}
