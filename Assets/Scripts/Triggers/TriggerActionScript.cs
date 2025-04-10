using UnityEngine;
using TMPro;
using System.Collections;
using System.IO;

public class TriggerActionScript : MonoBehaviour
{
    public SimplePlayerController playerController;
    public int requiredSpacePresses = 2;
    private int spacePressCount = 0;
    private bool triggered = false;
    private bool active = false;
    public GameObject DialogBackground;
    public TextMeshProUGUI textDialog;
    public TextMeshProUGUI textSpace;
    private string[] dialogLines;

    private void Start()
    {
        LoadLocalization();
    }

    private void LoadLocalization()
    {
        string path = "Assets/Localization/localization_ru.txt";

        if (File.Exists(path))
        {
            dialogLines = File.ReadAllLines(path);
        }
        else
        {
            Debug.LogError("���� ����������� �� ������ � ����: " + path);
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player") && !triggered)
        {
            playerController.StopPlayer();
            playerController.LockPlayerActions();
            playerController.PlayLookUpAnimation();

            DialogBackground.SetActive(true);
            textDialog.enabled = true;
            textSpace.enabled = false;

            StartCoroutine(TypeText(dialogLines[2]));

            triggered = true;
            active = true;
        }
    }

    private IEnumerator TypeText(string message)
    {
        textDialog.text = "";

        foreach (char letter in message.ToCharArray())
        {
            textDialog.text += letter;
            yield return new WaitForSeconds(0.02f);
        }
    }

    private void Update()
    {
        if (triggered && active && Input.GetKeyDown(KeyCode.Space))
        {
            textSpace.enabled = true;
            spacePressCount++;

            if (spacePressCount >= requiredSpacePresses)
            {
                if (playerController.moveLocked)
                {
                    UnlockPlayerActions();
                }
            }
        }
        else if (triggered && active && Input.anyKeyDown)
        {
            textSpace.enabled = true;
            spacePressCount++;
        }
    }

    private void UnlockPlayerActions()
    {
        if (playerController.moveLocked)
        {
            active = false;
            playerController.UnlockPlayerActions();
            spacePressCount = 0;
            textSpace.enabled = false;
            textDialog.enabled = false;
            DialogBackground.SetActive(false);
        }
    }

    public void ResetTrigger()
    {
        triggered = false;
        active = false;
    }
}
