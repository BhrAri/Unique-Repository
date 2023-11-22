using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BuildableArea : MonoBehaviour
{
    public Workbench workbench;
    public Building building;
    public void OnTriggerEnter(Collider other)
    {
        if (other.GetComponent<Building>())
        {
            building = other.GetComponent<Building>();
            building.placable = true;
        }
    }
    public void OnTriggerExit(Collider other)
    {
        if (other.GetComponent<Building>())
        {
            building = other.GetComponent<Building>();
            building.placable = false;
        }
    }
}
