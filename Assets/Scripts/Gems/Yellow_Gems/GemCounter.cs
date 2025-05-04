using UnityEngine;
using TMPro;

public class GemCounter : MonoBehaviour
{
    public static GemCounter Instance;

    private int yellowGemsCollected = 0;
    private int totalYellowGems;

    public TextMeshProUGUI gemCountText;

    private void Start()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
            return;
        }

        YellowGemManager gemManager = FindObjectOfType<YellowGemManager>();
        if (gemManager)
        {
            totalYellowGems = gemManager.GetTotalYellowGems();
        }

        UpdateGemCountText();
    }

    public void UpdateYellowGemCount()
    {
        yellowGemsCollected++;
        UpdateGemCountText();
    }

    private void UpdateGemCountText()
    {
        if (gemCountText != null)
        {
            gemCountText.text = yellowGemsCollected + "/" + totalYellowGems;
        }
    }

    public void ResetYellowGemCount()
    {
        yellowGemsCollected = 0;
        UpdateGemCountText();
    }

    public int GetYellowGemsCollected()
    {
        return yellowGemsCollected;
    }

    public int GetTotalYellowGems()
    {
        return totalYellowGems;
    }
}
