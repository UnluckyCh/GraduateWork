using UnityEngine;

public class Gem : MonoBehaviour
{
    public AudioSource gemTakeSound;

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            PlayerMovement player = other.GetComponent<PlayerMovement>();
            if (player != null)
            {
                gemTakeSound.Play();
                player.EnableDoubleJumpEffect();
            }
            gameObject.SetActive(false);
        }
    }
}
