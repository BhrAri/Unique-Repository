using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WeaponHand : MonoBehaviour
{
    public bool hasWeapon;
    public Weapon weapon;

    public void YouCalled()
    {
        weapon = GetComponentInChildren<Weapon>();
        if (weapon != null)
        {
            hasWeapon = true;
        }
        else { hasWeapon = false; }
    }
}
