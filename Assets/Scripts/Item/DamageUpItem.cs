using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DamageUpItem : ItemBase
{
    public float bonusDamage = 10f;

    public override void Apply(PlayerController player)
    {
        player.weaponData.damage += bonusDamage;
        Debug.Log("Tăng damage lên: " + player.weaponData.damage);
    }
}
