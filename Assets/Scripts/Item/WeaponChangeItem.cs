using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WeaponChangeItem : ItemBase
{
    public WeaponData newWeapon;

    public override void Apply(PlayerController player)
    {
        Debug.Log("Đổi súng sang: " + newWeapon.weaponName);
    }
}

