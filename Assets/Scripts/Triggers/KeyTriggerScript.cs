using UnityEngine;
using TMPro;
using System.Collections;
using System.IO;

public class KeyTriggerScript : MonoBehaviour
{
    public SimplePlayerController playerController;
    public AnomalysManager anomalysManager;
    public GameObject blueKey;
    public GameObject middleTriggerVolume;
    public GameObject firstTriggerVolume;
    public GameObject DialogBackground;
    public TextMeshProUGUI textDialog;
    public TextMeshProUGUI textSpace;
    public AudioSource eventSound;
    public AudioSource firstSound;
    public int requiredSpacePresses = 2;

    private int spacePressCount = 0;
    private bool keyPickedUp = false;
    private bool active = false;
    private string[] dialogLines;
    private bool isFirstSoundPlaying = false;
    private bool isEventSoundPlaying = false;

    private void Start()
    {
        LoadLocalization();
        eventSound.Play();
        eventSound.Stop();
        firstSound.Play();

        isFirstSoundPlaying = true;
        isEventSoundPlaying = false;

        playerController.OnPlayerDead += BlockMusicRepeat;
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

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player") && !keyPickedUp)
        {
            if (playerController != null)
            {
                playerController.StopPlayer();
                playerController.LockPlayerActions();

                blueKey.GetComponent<KeyFollowScript>().StartFollowing();
                playerController.PlayLookUpAnimation();
                
                DialogBackground.SetActive(true);
                textDialog.enabled = true;
                textSpace.enabled = false;

                StartCoroutine(TypeText(dialogLines[5]));

                keyPickedUp = true;
                active = true;
            }

            if (firstTriggerVolume != null)
            {
                firstTriggerVolume.SetActive(false);
            }

            if (middleTriggerVolume != null)
            {
                middleTriggerVolume.SetActive(true);
            }
        }
    }

    private void Update()
    {
        if (keyPickedUp && active && Input.GetKeyDown(KeyCode.Space))
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
        else if (keyPickedUp && active && Input.anyKeyDown)
        {
            textSpace.enabled = true;
            spacePressCount++;
        }

        if (!firstSound.isPlaying && isFirstSoundPlaying)
        {
            firstSound.Play();
        }

        if (!eventSound.isPlaying && isEventSoundPlaying)
        {
            eventSound.Play();
        }
    }

    private IEnumerator TypeText(string message)
    {
        textDialog.fontSize = 86;
        textDialog.text = "";

        foreach (char letter in message.ToCharArray())
        {
            textDialog.text += letter;
            yield return new WaitForSeconds(0.02f);
        }
    }

    private IEnumerator FadeOutAndFadeInSounds()
    {
        eventSound.volume = 0;
        eventSound.Play();
        float startVolumeFirst = firstSound.volume;
        float startVolumeEvent = 0f;

        while (firstSound.volume > 0 || startVolumeEvent < 0.03f)
        {
            firstSound.volume -= startVolumeFirst * Time.deltaTime / 1.5f;
            eventSound.volume = startVolumeEvent;
            startVolumeEvent += 0.03f * Time.deltaTime / 0.9f;

            yield return null;
        }

        firstSound.Stop();
        firstSound.volume = startVolumeFirst;
    }

    private void UnlockPlayerActions()
    {
        if (playerController.moveLocked)
        {
            StartCoroutine(FadeOutAndFadeInSounds());

            if (anomalysManager != null)
            {
                anomalysManager.EnableEvent();
                isFirstSoundPlaying = false;
                isEventSoundPlaying = true;
            }

            active = false;
            playerController.UnlockPlayerActions();
            spacePressCount = 0;
            textSpace.enabled = false;
            textDialog.enabled = false;
            DialogBackground.SetActive(false);
        }
    }

    public void BlockMusicRepeat()
    {
        isFirstSoundPlaying = false;
        isEventSoundPlaying = false;
    }

    private void OnDisable()
    {
        playerController.OnPlayerDead -= BlockMusicRepeat;
    }
}
