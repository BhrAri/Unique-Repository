using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HorseHorseBinks : MonoBehaviour
{
    private void OnEnable()
    {
        if (FindAnyObjectByType<Player>().GetComponent<Inventory>().dogFood > 0)
        {
            //game won
        } else
        {
            //your dog is too hungry to even blink
        }
    }
}
