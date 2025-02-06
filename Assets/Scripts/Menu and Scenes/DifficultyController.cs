using UnityEngine;

public class LevelController : MonoBehaviour
{
    public HealthUIUpdater healthUIUpdater;
    public SimplePlayerController simplePlayerController;
    public PlayerMovement playerMovement;

    void Awake()
    {
        healthUIUpdater = FindObjectOfType<HealthUIUpdater>();

        if (healthUIUpdater == null)
        {
            Debug.LogError("HealthUIUpdater not found in the scene!");
            return;
        }

        int difficulty = PlayerPrefs.GetInt("Difficulty", 1);
        switch (difficulty)
        {
            case 1:
                SetDifficulty1();
                break;
            case 2:
                SetDifficulty2();
                break;
            case 3:
                SetDifficulty3();
                break;
            default:
                SetDifficulty2();
                break;
        }
    }

    private void SetDifficulty1()
    {
        healthUIUpdater.MaxHealth = 10;
        //healthUIUpdater.SetCurrentHealth(10);
        simplePlayerController.shieldEffectDuration = 20f;
        playerMovement.doubleJumpEffectDuration = 20f;
    }

    private void SetDifficulty2()
    {
        healthUIUpdater.MaxHealth = 5;
        //healthUIUpdater.SetCurrentHealth(5);
        simplePlayerController.shieldEffectDuration = 15f;
        playerMovement.doubleJumpEffectDuration = 15f;
    }

    private void SetDifficulty3()
    {
        healthUIUpdater.MaxHealth = 3;
        //healthUIUpdater.SetCurrentHealth(3);
        simplePlayerController.shieldEffectDuration = 10f;
        playerMovement.doubleJumpEffectDuration = 10f;
    }
}
