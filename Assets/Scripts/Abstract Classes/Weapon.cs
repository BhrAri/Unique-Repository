using UnityEngine;

public abstract class Weapon : MonoBehaviour
{
    public float damage;
    public int durability;
    public Vector3 weaponStandardPosition = Vector3.zero;
    public abstract void SwingWeapon();
    public abstract void DamageSet();
    public Transform brokenWeaponStorage;
    public void Break()
    {
        if (durability <= 0)
        {
            gameObject.SetActive(false);
            transform.parent = brokenWeaponStorage;
            brokenWeaponStorage.GetComponent<BrokenWeaponStorage>().WeaponBroke();
        }
    }
    public void Equip()
    {
        transform.parent = FindFirstObjectByType<WeaponHand>().transform;
        gameObject.SetActive(true);
    }

}
