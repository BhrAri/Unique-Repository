using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SceneSpawnPositioner : MonoBehaviour
{
    public GameObject player;
    public GameObject cameraArm;
    public GameObject weaponBox;
    public GameObject loadScreen;
    void Start()
    {
        player = FindAnyObjectByType<Player>().gameObject;
        cameraArm = FindAnyObjectByType<CameraArm>().gameObject;
        weaponBox = FindAnyObjectByType<BrokenWeaponStorage>().gameObject;
        loadScreen.SetActive(true);
    }

    // Update is called once per frame
    void Update()
    {
        if (weaponBox.transform.position != transform.position)
        {
            player.transform.position = transform.position;
            cameraArm.transform.position = transform.position;
            weaponBox.transform.position = transform.position;
        }
        else
        {
            loadScreen.SetActive(false);
            enabled = false;
        }
        
    }
}
