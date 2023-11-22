using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class Workbench : MonoBehaviour
{  
    public bool moving = false;
    public GameObject[] buildingPrefabs;
    public GameObject benchUi;
    public int e = 0;
    public VillageManager villageManager;
    public Workbench workbench;
    public MonoBehaviour player;
    public MonoBehaviour cameraArm;
    //maybe make this a place where I have a ui to select a building to make, then handle everything else with the building
    private void Start()
    {
        workbench = this;
        player = FindAnyObjectByType<Player>(FindObjectsInactive.Include);
        cameraArm = FindAnyObjectByType<CameraArm>(FindObjectsInactive.Include);
    }
    private void OnEnable()
    {
        benchUi.SetActive(true);
    }
    private void BuildABuilding() //a click event for buttons that represent buildings
    {
        Instantiate(buildingPrefabs[e],new Vector3(30f, 2.34f, 0f), Quaternion.identity);
        benchUi.SetActive(false);
    }
    public void Building1()
    {
        EZBuildingBuilding(0);
    }
    public void Building2()
    {
        EZBuildingBuilding(1);
    }
    public void Building3()
    {
        EZBuildingBuilding(2);
    }
    public void Cancel()
    {
        benchUi.SetActive(false);
        workbench.enabled = false;
        player.enabled = true;
        cameraArm.enabled = true;
    }
    public void EZBuildingBuilding(int type)
    {
        if (player.GetComponent<Inventory>().wood >= 10 && player.GetComponent<Inventory>().stone >= 10)
        {
            e = type;
            BuildABuilding();
            villageManager.numberOfHouses++;
            workbench.enabled = false;
            player.GetComponent<Inventory>().wood -= 10;
            player.GetComponent<Inventory>().stone -= 10;
        }
    }
}
