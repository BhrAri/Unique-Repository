using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Runtime.CompilerServices;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.UI;
using Random = UnityEngine.Random;

public class VillageManager : MonoBehaviour
{
    public float runtimeSinceActive;
    public float lastTimeSaved;
    public float population;
    public float populationDivided;
    public float villageWellness;

    public int b; // why are you here b? why??
    public int numberOfHouses;
    public int numberOfUtilities;
    public GameObject villager;
    public Material[] materials;
    public GameObject[] buildingPrefabs;
    public Vector3[] positions;
    void Start()
    {
        ReadSave();
        RebuildBuildings();
        CalculateWellness();
        CalculatePopulationAndSpawnVillagers();
    }
    private GameObject CustomVillager() 
    {
        MeshRenderer[] villagerParts = villager.GetComponentsInChildren<MeshRenderer>();
        villagerParts[0].material = materials[Random.Range(0, materials.Length)];
        villagerParts[1].material = materials[Random.Range(0, materials.Length)];
        villagerParts[2].material = materials[Random.Range(0, materials.Length)];
        villagerParts[7].material = materials[Random.Range(0, materials.Length)];
        return villager;
    }
    private Vector3 SpawnPosition()
    {
        return new(0, 0, 0);
    }
    private void OnDestroy()
    {
        VillageSave beenig = new()
        {
            houses = numberOfHouses, population = population, lastTimeSaved = Time.fixedTime, utilities = numberOfUtilities
        };
        string json = JsonUtility.ToJson(beenig);
        File.WriteAllText(Application.persistentDataPath + "/savefile.json", json);
    }
    private void CalculateWellness()
    {
        if (numberOfHouses > 0 && numberOfUtilities > 0)
        {
            if (population / 4 <= numberOfHouses)
            {
                villageWellness = 0.5f;
            }
            else
            {
                villageWellness = 0.5f - 0.05f * (population - (numberOfHouses * 4));
            }//village wellness might be negative at this point
            if (population / 4 <= numberOfUtilities)
            {
                villageWellness += 0.5f;
            }
            else
            {
                villageWellness -= 0.5f - 0.05f * (population - (numberOfUtilities * 4));
            }
            if (villageWellness < -1)
            {
                villageWellness = -1;
            }
        }
    }
    private void ReadSave()
    {
        string path = Application.persistentDataPath + "/savefile.json";
        if (File.Exists(path))
        {
            string json = File.ReadAllText(path);
            VillageSave beenig = JsonUtility.FromJson<VillageSave>(json);
            numberOfHouses = beenig.houses;
            numberOfUtilities = beenig.utilities;
            lastTimeSaved = beenig.lastTimeSaved;
            population = beenig.population;
        }
    }
    private void CalculatePopulationAndSpawnVillagers()
    {
        if (lastTimeSaved != 0)
        {
            runtimeSinceActive = Time.fixedTime - lastTimeSaved;
        }
        population *= runtimeSinceActive * villageWellness;

        populationDivided = (float)Math.Round(population / 10f) + 1f;
        for (int i = 0; i < populationDivided; i++)
        {
            Instantiate(CustomVillager(), SpawnPosition(), villager.transform.rotation);
            b++; // I don't remember why this exists
        }
        b = 0;
    }
    [System.Serializable]
    class VillageSave
    {
        public int houses;
        public int utilities;
        public float population;
        public float lastTimeSaved;
    }
    private void RebuildBuildings()
    {
        buildingPrefabs[0].GetComponent<Building>().placed = true;
        buildingPrefabs[1].GetComponent<Building>().placed = true;
        int h = numberOfHouses;
        int u = numberOfUtilities;
        if (h > 15)
        {
            numberOfHouses = 15;
            h = 15;
        }
        if (u > 15)
        {
            numberOfUtilities = 15;
            u = 15;
        }
        int houseNumber = 1;
        for (b = h; b > 0; b--)
        {
            BuildBuilding(houseNumber);
        }
        int utilityNumber = 1;
        for (b = u; b > 0; b--)
        {
            BuildUtility(utilityNumber);
        }
        buildingPrefabs[0].GetComponent<Building>().placed = false;
        buildingPrefabs[1].GetComponent<Building>().placed = false;
    }
    private void BuildBuilding(int buildingNumber)
    {
        Instantiate(buildingPrefabs[0], BuildingSpawnPosition(buildingNumber), BuildingSpawnRotation(buildingNumber));
        buildingNumber++;
    }
    private Vector3 BuildingSpawnPosition(int buildingNumber)
    {
        Vector3 naturalSpawnPosition = new(14.2f, 1.8315f, -10.9f);
        if (buildingNumber > 6) 
        {
            Increment(true);
        } else if (buildingNumber > 11)
        {
            naturalSpawnPosition.x = 30.2f;
            naturalSpawnPosition.z = 14.5f;
        } else if (buildingNumber > 16)
        {
            naturalSpawnPosition.x = 45f;
            naturalSpawnPosition.z = -15f;
        }
        return naturalSpawnPosition;
        void Increment(bool horizontal)
        {
            if (horizontal)
            {
                switch (buildingNumber % 5)
                {
                    case 0: 
                        naturalSpawnPosition.x -= 16f;
                        break;
                    case 1:
                        break;
                    case 2:
                        naturalSpawnPosition.x += 8f;
                        break;
                    case 3:
                        naturalSpawnPosition.x -= 8f;
                        break;
                    case 4:
                        naturalSpawnPosition.x += 16f;
                        break;
                }
            }
            else
            {
                switch (buildingNumber % 5)
                {
                    case 0:
                        naturalSpawnPosition.z -= 16f;
                        break;
                    case 1:
                        break;
                    case 2:
                        naturalSpawnPosition.z += 8f;
                        break;
                    case 3:
                        naturalSpawnPosition.z -= 8f;
                        break;
                    case 4:
                        naturalSpawnPosition.z += 16f;
                        break;
                }
            }
        }
    }
    private Quaternion BuildingSpawnRotation(int buildingNumber)
    {
        Quaternion naturalRotation;
        if (buildingNumber < 6)
        {
            naturalRotation = Quaternion.Euler(0f, 90f, 0);
        }
        else if (buildingNumber < 11)
        {
            naturalRotation = Quaternion.Euler(0f, 90f, 0);
        }
        else
        {
            naturalRotation = Quaternion.Euler(0f, -90f, 0);
        }
        return naturalRotation;
    }
    private void BuildUtility(int utilityNumber)
    {
        Instantiate(buildingPrefabs[1], UtilitySpawnPosition(utilityNumber), Quaternion.Euler(0f, Random.Range(-180f, 180f), 0f));
        utilityNumber++;
    }
    private Vector3 UtilitySpawnPosition(int buildingNumber)
    {
        return positions[buildingNumber];
    }
}
