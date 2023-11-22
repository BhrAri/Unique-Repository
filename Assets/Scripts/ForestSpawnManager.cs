using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class ForestSpawnManager : MonoBehaviour
{
    private Vector2 wormOnly = new (600f, 600f); //x > -150,z > -200
    private readonly float moreSpawnsZ = 600; //z < -300 spawn more often
    public bool spawning = true;
    public Player player;
    private float x;
    private float z;
    public float spawnRadius = 10f;
    public float spawnTime;
    public TerrainData terrain;
    public GameObject[] enemyPrefabs;
    private Vector3 terrainablePosition;
    public int timesFailed = 0;
    /*
     * find a cartian radius around the player
     * raycast downward at a certian height to make sure you are above terrain and not on water or a tree 
     * (likely no masking just look for returning terrain tag or layer)
     * if z is above a certian number worm
     * if z is below a certian number dog or worm
     * set height to terrain
     * spawn more after a certian z
     * if past that certian z spawn more after a certian x
     * make a no spawn zone at the boss crater
     * maybe have a trigger zone that sets a public bool in spawn manager to no
     * have a public float that controls spawn rate in case i want to add an item or ability to reduce spawns
     * make an enemy cap
     * maybe kill enemies too far from the player so that they can't just wait for a bunch of worms in the beginning and then be free
     * test how many worms and wolves you can have out at some point, to see how much preformance it takes
     */
    private void Start()
    {
        player = FindFirstObjectByType<Player>();
        terrain = FindAnyObjectByType<Terrain>().terrainData;
    }
    private void Update()
    {
        spawnTime -= Time.deltaTime;
        if (spawning && spawnTime <= 0f)
        {
            SpawnOne();
            spawnTime = 10f;
            if (player.transform.position.z > moreSpawnsZ)
            {
                spawnTime *= 2f;
            }
        }
    }
    private void SpawnOne() 
    //spawns a worm in the worm only section, spawns a wolf in the land bridge, and spawns a random enemy in the main area 
    {
        TerrainablePosition();
        if (terrainablePosition == Vector3.zero)
        {
            Debug.Log("Nothing Happens");
        }
        else if (terrainablePosition.x > wormOnly.x) 
        {
            if (terrainablePosition.z > wormOnly.y)
            {
                Instantiate(enemyPrefabs[0], terrainablePosition, enemyPrefabs[0].transform.rotation);
            }
            else
            {
                Instantiate(enemyPrefabs[1], terrainablePosition + Vector3.up, enemyPrefabs[1].transform.rotation);
            }
        } else
        {
            int enemyRoll = Random.Range (0, enemyPrefabs.Length);
            if (enemyRoll > 0)
            {
                terrainablePosition += Vector3.up;
            }
            Instantiate(enemyPrefabs[enemyRoll], terrainablePosition, enemyPrefabs[enemyRoll].transform.rotation);
        }
    }
    private void GenerateRandomSpawnPosition() 
    {
        x = Random.Range(-spawnRadius, spawnRadius);
        if (Random.Range(0,2) == 0)
        {
            z = (spawnRadius * spawnRadius) - (x * x);
            z = Mathf.Sqrt(z);
            z += player.transform.position.z;
        }
        else
        {
            z = (spawnRadius * spawnRadius) - (x * x);
            z = Mathf.Sqrt(z);
            z = player.transform.position.z - z;
        }
        if (Random.Range(0, 2) == 0)
        {
            x += player.transform.position.x;
        } else
        {
            x = player.transform.position.x - x;
        }

        terrainablePosition = new Vector3(x,terrain.GetInterpolatedHeight(x/1000,z/1000),z);
        Debug.Log(terrainablePosition);
    }
    private bool CheckPosition(Vector3 position)
    {
        if (Physics.Raycast(position + Vector3.up, Vector3.down,out RaycastHit bit, 2f))
        {
            if (bit.collider.gameObject.layer == 3)
            {
                timesFailed = 0;
                return true;
            }
            timesFailed++;
            return false;
            //did it hit terrain?
        } else
        {
            Debug.Log("We hit nothing somehow");
            timesFailed++;
            return false;
        }
    }
    private Vector3 TerrainablePosition()
    {
        tryAgain:
        GenerateRandomSpawnPosition();
        if (CheckPosition(terrainablePosition))
        {
            return terrainablePosition;
        }
        else if (timesFailed < 7) 
        {
            goto tryAgain;
        } else
        {
            Debug.Log("Too many failed attempts");
            terrainablePosition = Vector3.zero;
            return terrainablePosition;
        }
    }
}
