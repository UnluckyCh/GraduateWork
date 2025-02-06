using UnityEngine;
using TMPro;
using System.Collections;
using System.IO;

public class StartScript : MonoBehaviour
{
    public SimplePlayerController playerController;
    public AnomalysManager anomalysManager;
    public int requiredSpacePresses = 2;
    public GameObject DialogBackgroundRight;
    public TextMeshProUGUI textDialog;
    public TextMeshProUGUI textSpace;
    public GameObject completedObject;
    public bool _startDialogs = true;
    public bool _disableCursor = true;

    private int spacePressCount = 0;
    private bool startTriggered = false;
    private bool skip = false;
    private string[] dialogLines;

    private void Start()
    {
        if (_disableCursor)
        {
            Cursor.visible = false;
        }

        LoadLocalization();

        if (_startDialogs)
        {
            StartCoroutine(StartDialog());
        }

        if (anomalysManager)
        {
            anomalysManager.DisableEvent();
        }
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
            Debug.LogError("Файл локализации не найден в пути: " + path);
        }
    }

    private IEnumerator StartDialog()
    {
        if (completedObject != null)
            completedObject.SetActive(false);

        if (playerController == null)
        {
            Debug.LogError("Player controller is not assigned in StartScript!");
            yield break;
        }
        DialogBackgroundRight.SetActive(false);
        textDialog.enabled = false;
        textSpace.enabled = false;
        startTriggered = true;

        playerController.LockPlayerActions();
        Vector3 originalScale = playerController.transform.localScale;

        yield return new WaitForSeconds(0.6f);

        if (!skip)
        {
            playerController.transform.localScale = new Vector3(-originalScale.x, originalScale.y, originalScale.z);
            yield return new WaitForSeconds(0.6f);
        }
        if (!skip)
        {
            playerController.transform.localScale = originalScale;
            yield return new WaitForSeconds(0.3f);
        }
        if (!skip)
        {
            DialogBackgroundRight.SetActive(true);
            textDialog.enabled = true;
            yield return new WaitForSeconds(0.1f);
        }
        if (!skip)
        {
            yield return StartCoroutine(TypeText(dialogLines[0]));
            yield return new WaitForSeconds(2f);
        }

        if (!skip)
            yield return StartCoroutine(TypeText(dialogLines[1]));
    }

    private IEnumerator TypeText(string message)
    {
        textDialog.text = "";

        foreach (char letter in message.ToCharArray())
        {
            textDialog.text += letter;
            yield return new WaitForSeconds(0.03f);
        }
    }

    private void Update()
    {
        if (startTriggered && Input.GetKeyDown(KeyCode.Space))
        {
            if (!DialogBackgroundRight.activeSelf && !skip)
                DialogBackgroundRight.SetActive(true);
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
        else if (startTriggered && Input.anyKeyDown)
        {
            DialogBackgroundRight.SetActive(true);
            textSpace.enabled = true;
            spacePressCount++;
        }
    }

    private void UnlockPlayerActions()
    {
        if (playerController.moveLocked)
        {
            skip = true;
            startTriggered = false;
            playerController.UnlockPlayerActions();
            spacePressCount = 0;
            textSpace.enabled = false;
            textDialog.enabled = false;
            DialogBackgroundRight.SetActive(false);
        }
    }
}
