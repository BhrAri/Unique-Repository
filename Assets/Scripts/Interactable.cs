using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class Interactable : MonoBehaviour
{
    public bool buttonInteractable;
    public bool drop;
    public int dropNumber;
    public bool disablePlayerWhenActivated;
    public MonoBehaviour activate;
    public Player player;
    public MonoBehaviour cameraArm;
    private void Start()
    {
        player = FindAnyObjectByType<Player>();
        cameraArm = FindAnyObjectByType<CameraArm>();
    }
    private void OnCollisionStay(Collision collision)
    {
        if (collision.collider.CompareTag("Player"))
        {
            if (buttonInteractable)
            {
                if (player.lookingToInteract)
                {
                    activate.enabled = true;
                    if (disablePlayerWhenActivated)
                    {
                        player.enabled = false;
                        cameraArm.enabled = false;
                    }
                }
            }
            else
            {
                switch (dropNumber)
                {
                    case 0:
                        player.GetComponent<Inventory>().wood++;
                        break;
                    case 1:
                        player.GetComponent<Inventory>().stone++;
                        break;
                    case 2:
                        player.GetComponent<Inventory>().ammunition++;
                        break;
                    case 3:
                        player.GetComponent<Inventory>().dogFood++;
                        break;
                }
                Destroy(gameObject);
            }
        }
    }
}
