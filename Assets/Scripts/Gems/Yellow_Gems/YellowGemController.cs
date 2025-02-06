using UnityEngine;

public class YellowGemController : MonoBehaviour
{
    public AudioSource gemTakeSound;

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            gemTakeSound.Play();
            gameObject.SetActive(false);
            GemCounter.Instance.UpdateYellowGemCount();
        }
    }
}
