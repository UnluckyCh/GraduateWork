using UnityEngine;
using TMPro;
using System.Collections;
using System.IO;
using UnityEngine.UI;

public class MiddleTriggerScript : MonoBehaviour
{
    public GameObject blueKey;
    public GameObject funnel;
    public TextMeshProUGUI timerText;
    public GameObject timerBackground;
    public float moveSpeed = 5f;
    public float timerDuration = 40f;
    private bool triggered = false;
    public GameObject lastTriggerVolume;
    private Vector2 targetPosition;
    public Vector2 targetOffset;
    public GameObject DialogBackground;
    public TextMeshProUGUI textDialog;
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
            Debug.LogError("Файл локализации не найден в пути: " + path);
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player") && !triggered)
        {
            SetTimerUIOpacity(0.6f, 1f);

            StartCoroutine(StartTimer(timerDuration));

            triggered = true;

            blueKey.GetComponent<KeyFollowScript>().StopFollowing();
            targetPosition = (Vector2)transform.position + targetOffset;

            timerText.text = FormatTime(timerDuration);

            StartCoroutine(TypeText(dialogLines[6]));
        }
    }

    private void Update()
    {
        if (triggered)
        {
            blueKey.transform.position = Vector2.MoveTowards(blueKey.transform.position, targetPosition, moveSpeed * Time.deltaTime);
        }
    }

    private IEnumerator TypeText(string message)
    {
        yield return new WaitForSeconds(2f);
        DialogBackground.SetActive(true);
        textDialog.enabled = true;
        textDialog.fontSize = 86;
        textDialog.text = "";

        foreach (char letter in message.ToCharArray())
        {
            textDialog.text += letter;
            yield return new WaitForSeconds(0.02f);
        }
        yield return new WaitForSeconds(2f);
        DialogBackground.SetActive(false);
        textDialog.enabled = false;
    }

    private IEnumerator StartTimer(float duration)
    {
        float startTime = Time.time;

        while (Time.time - startTime < duration)
        {
            float timeRemaining = duration - (Time.time - startTime);
            timerText.text = FormatTime(timeRemaining);

            yield return null;
        }

        OnTimerComplete();
    }

    private void OnTimerComplete()
    {
        //Debug.Log("Таймер завершен!");
        SetTimerUIOpacity(0f, 0f);
        SpriteRenderer keySpriteRenderer = blueKey.GetComponent<SpriteRenderer>();
        if (keySpriteRenderer != null)
        {
            StartCoroutine(FadeSpriteOpacity(keySpriteRenderer, 1f, 0f, 0.3f));
        }

        SpriteRenderer funnelSpriteRenderer = funnel.GetComponent<SpriteRenderer>();
        if (funnelSpriteRenderer != null)
        {
            StartCoroutine(FadeSpriteOpacity(funnelSpriteRenderer, 0f, 1f, 0.3f));
        }

        if (lastTriggerVolume != null)
        {
            lastTriggerVolume.SetActive(true);
        }
    }

    private void SetTimerUIOpacity(float backgroundOpacity, float textOpacity)
    {
        if (timerBackground != null)
        {
            Image backgroundImage = timerBackground.GetComponent<Image>();
            if (backgroundImage != null)
            {
                Color backgroundColor = backgroundImage.color;
                backgroundColor.a = backgroundOpacity;
                backgroundImage.color = backgroundColor;
            }
        }

        if (timerText != null)
        {
            Color textColor = timerText.color;
            textColor.a = textOpacity;
            timerText.color = textColor;
        }
    }

    private IEnumerator FadeSpriteOpacity(SpriteRenderer spriteRenderer, float startOpacity, float targetOpacity, float duration)
    {
        Color color = spriteRenderer.color;
        color.a = startOpacity;
        spriteRenderer.color = color;

        float currentTime = 0f;

        while (currentTime < duration)
        {
            float alpha = Mathf.Lerp(startOpacity, targetOpacity, currentTime / duration);
            color.a = alpha;
            spriteRenderer.color = color;

            currentTime += Time.deltaTime;
            yield return null;
        }

        color.a = targetOpacity;
        spriteRenderer.color = color;
    }

    private string FormatTime(float time)
    {
        int minutes = Mathf.FloorToInt(time / 60);
        int seconds = Mathf.FloorToInt(time % 60);
        return string.Format("{0:00}:{1:00}", minutes, seconds);
    }
}
