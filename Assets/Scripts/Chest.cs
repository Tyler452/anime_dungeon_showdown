using System;
using TMPro;
using UnityEngine;

public class Chest : MonoBehaviour
{
    private bool playerInRange = false;
    public AudioSource ChestSource;
    public GameObject pressChestText;
    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            playerInRange = true;
            pressChestText.SetActive(true);
            Debug.Log("Press E to open chest");
        }
    }

    void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            pressChestText.SetActive(false);
            playerInRange = false;
        }
    }

    void Update()
    {
        if (playerInRange && Input.GetKeyDown(KeyCode.E))
        {
            OpenChest();
        }
    }

    void OpenChest() {
        playChestSound();
        Debug.Log("Chest opened!");
    }
    
    public void playChestSound() {
        Debug.Log("PLAY SOUND");
        ChestSource.Play();
    }
}