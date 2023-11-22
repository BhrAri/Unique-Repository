using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpawnManager : MonoBehaviour
{
    public float upperZbound = 19f;
    public float lowerZbound = 49f;
    public float y = 3f;
    public float upperXbound = 49f;
    public float lowerXbound = -13f;
    public GameObject[] enemyPrefabs;
    public VillageManager villageManager;
    public float monsterAttraction;
    public float timeUntilNextSpawn = 60;
    public float changableSpawnSpeed = 60;
    void Start()
    {
        CalculateMonsterAttraction();
    }
    private void Update()
    {
        timeUntilNextSpawn -= Time.deltaTime;
        if (timeUntilNextSpawn <= 0)
        {
            SpawnAWorm();
            timeUntilNextSpawn = changableSpawnSpeed / monsterAttraction;
            // works out to changable spawn speed / between 1 and 3
            // or between 60 and 20 seconds
        }
        WormConsititancy();
    }
    public Vector3 SpawnPosition()
    {
        return new Vector3 (Random.Range(lowerXbound, upperXbound), y, Random.Range(lowerZbound, upperZbound));
    }
    public void SpawnAWorm()
    {
        Instantiate(enemyPrefabs[0], SpawnPosition(), enemyPrefabs[0].transform.rotation);
    }
    private void CalculateMonsterAttraction()
    {
        villageManager = GetComponent<VillageManager>();
        if (villageManager.villageWellness >= 0)
        {
            monsterAttraction = 2 - villageManager.villageWellness;
        }
        else
        {
            monsterAttraction = 2 + (-villageManager.villageWellness);
        }
    }
    private void WormConsititancy()
    {
        int worms = FindObjectsOfType<Worm>().Length;
        if (worms > 25) 
        {
            changableSpawnSpeed = 120f;
        }
        if (worms < 3)
        {
            changableSpawnSpeed = 6f;
        }
        if (worms > 7) 
        {
            changableSpawnSpeed = 60f;
        }
        if (worms > 40f)
        {
            changableSpawnSpeed = 0f;
        }
    }
}
