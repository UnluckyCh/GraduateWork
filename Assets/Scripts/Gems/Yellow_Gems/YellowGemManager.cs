using UnityEngine;

public class YellowGemManager : MonoBehaviour
{
    private int totalYellowGems;

    private void OnEnable()
    {
        totalYellowGems = transform.childCount;
    }

    public int GetTotalYellowGems()
    {
        return totalYellowGems;
    }
}
