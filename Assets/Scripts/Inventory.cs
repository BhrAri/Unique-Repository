using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using TMPro;
using UnityEngine;


public class Inventory : MonoBehaviour
{
    public string uniqueInventoryName;
    public int wood;
    public int stone;
    public int ammunition;
    public int dogFood;
    public GameObject InventoryScreen;
    public Player player;

    void Start()
    {
        player = GetComponent<Player>();
        if (uniqueInventoryName != null)
        {
            enabled = false;
        } else
        {
            ReadSave();
        }
    }

    private void OnDestroy()
    {
        InventorySave Dave = new()
        {
            wood = wood,
            stone = stone,
            ammunition = ammunition,
            dogFood = dogFood,
            hp = player.maxHp,
            strength = player.strength,
            magic = player.magic,
            dexterity = player.dexterity
        };
        string json = JsonUtility.ToJson(Dave);
        File.WriteAllText(Application.persistentDataPath + uniqueInventoryName, json);
    }
    private void ReadSave()
    {
        string path = Application.persistentDataPath + uniqueInventoryName;
        if (File.Exists(path))
        {
            string json = File.ReadAllText(path);
            InventorySave Dave = JsonUtility.FromJson<InventorySave>(json);
            wood = Dave.wood;
            stone = Dave.stone;
            ammunition = Dave.ammunition;
            dogFood = Dave.dogFood;
            player.maxHp = Dave.hp;
            player.strength = Dave.strength;
            player.magic = Dave.magic;
            player.dexterity = Dave.dexterity;
        }
    }
    [System.Serializable]
    class InventorySave
    {
        public int wood;
        public int stone;
        public int ammunition;
        public int dogFood;
        public int hp;
        public int strength;
        public int magic;
        public int dexterity;
    }
    public void InventoryButton()
    {
        if (InventoryScreen.activeInHierarchy == true)
        {
            InventoryScreen.SetActive(false);
        } else
        {
            TMP_Text[] texts = InventoryScreen.GetComponents<TextMeshPro>();
            texts[0].text = "Wood: " + wood;
            texts[1].text = "Stone: " + stone;
            texts[2].text = "Ammunition: " + ammunition;
            texts[3].text = "Dog Food: " + dogFood;
            texts[4].text = "HP: " + player.maxHp + "\nStrength: " + player.strength + "\nMagic: " + player.magic + "\nDexterity: " + player.dexterity;

            InventoryScreen.SetActive(true);
        }
    }
}
