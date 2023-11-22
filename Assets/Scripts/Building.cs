using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEditor.UI;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.UIElements;
using Button = UnityEngine.UI.Button;

public class Building : MonoBehaviour
{
    public bool moving;
    public GameObject bottomOfBuilding;
    public GameObject wholeBuilding;
    public GameObject buildingUi;
    public bool placed;
    public bool placable;
    public float mouseX;
    public float mouseZ;
    public Camera buildingCamera;
    public Camera otherCamera;
    public Camera[] cameras;
    public float buildingHeightOffGround;
    public bool release; // makes sure the building cant be placed instantly on accident
    public MonoBehaviour player;
    public MonoBehaviour cameraArm;
    public Rigidbody buildingRb;
    private MeshCollider bottomOfBuildingCollider;
    private bool bottomOfBoolding;
    public List<GameObject> buildingsTouching;
    void Start()
    {
        buildingRb = GetComponent<Rigidbody>();
        player = FindAnyObjectByType<Player>(FindObjectsInactive.Include);
        cameraArm = FindAnyObjectByType<CameraArm>(FindObjectsInactive.Include);
        buildingUi = GameObject.Find("Find Me").GetComponentInChildren<BuildingUi>(true).gameObject;
        if (!placed)
        {
            buildingUi.SetActive(true);
            cameras = FindObjectsOfType<Camera>(true);
            if (cameras.Length == 2 ) 
            {
                if (cameras[0].enabled)
                {
                    otherCamera = cameras[0];
                    buildingCamera = cameras[1];
                } else
                {
                    otherCamera = cameras[1];
                    buildingCamera = cameras[0];
                }
            } else { Debug.Log("Too many cameras"); }
            buildingCamera.gameObject.GetComponent<Camera>().enabled = true;
            otherCamera.gameObject.GetComponent<Camera>().enabled = false;
            placable = true;
            if (bottomOfBuilding.TryGetComponent<MeshCollider>(out bottomOfBuildingCollider))
            {
                bottomOfBoolding = bottomOfBuildingCollider.convex;
                bottomOfBuildingCollider.convex = true;
            }
            bottomOfBuilding.GetComponent<Collider>().isTrigger = true;
        }
        else
        {
            buildingUi.SetActive(false);
            wholeBuilding.GetComponent<Building>().enabled = false;
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (buildingsTouching.Count > 0)
        {
            placable = false;
        } else { placable = true; }
        mouseZ = -Input.mousePosition.x / 10f; 
        mouseX = Input.mousePosition.y / 10f;
        transform.position = new Vector3(mouseX, buildingHeightOffGround, mouseZ);
        if (placed)
        {
            enabled = false;
        }
        if (Input.GetMouseButtonUp(0))
        {
            release = true;
        }
        if (release && Input.GetMouseButtonDown(0))
        {
            PlaceBuilding();
        }
    }
    public void PlaceBuilding()
    {
        if (placable)
        {
            Destroy(buildingRb);
            bottomOfBuilding.GetComponent<Collider>().isTrigger = false;
            if (bottomOfBuildingCollider != null)
            {
                bottomOfBuildingCollider.convex = bottomOfBoolding;
            }
            placed = true;
            buildingCamera.gameObject.GetComponent<Camera>().enabled = false;
            otherCamera.gameObject.GetComponent<Camera>().enabled = true;
            buildingUi.SetActive(false);
            enabled = false;
            player.enabled = true;
            cameraArm.enabled = true;
        }
    }
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Building"))
        {
            if (!buildingsTouching.Contains(other.gameObject))
            {
                buildingsTouching.Add(other.gameObject);
            }
        }
    }
    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Building"))
        {
            if (buildingsTouching.Contains(other.gameObject))
            {
                buildingsTouching.Remove(other.gameObject);
            }
        }
    }
}
