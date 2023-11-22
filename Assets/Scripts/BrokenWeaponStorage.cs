using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEngine;

public class BrokenWeaponStorage : MonoBehaviour
{
    private static BrokenWeaponStorage instance;
    public static BrokenWeaponStorage Instance { get { return instance; } }
    private void Awake()
    {
        if (instance != null && instance != this)
        {
            Destroy(this.gameObject);
        }
        else
        {
            instance = this;
        }
        DontDestroyOnLoad(this);
    }
    public int brokenWeapons;
    public void WeaponBroke()
    {
        brokenWeapons++;
    }
}
