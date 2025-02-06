using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShieldScript : MonoBehaviour
{
    public AudioSource gemTakeSound;

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            SimplePlayerController player = other.GetComponent<SimplePlayerController>();
            if (player != null)
            {
                gemTakeSound.Play();
                player.EnableShield();
            }
            gameObject.SetActive(false);
        }
    }
}