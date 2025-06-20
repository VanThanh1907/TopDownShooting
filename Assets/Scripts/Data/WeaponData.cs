using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "NewWeaponData", menuName = "ScriptableObjects/WeaponData")]
public class WeaponData : ScriptableObject
{
    public string weaponName;
    public float fireRate;
    public float damage;
    public GameObject bulletPrefab;
    public AudioClip shootSFX;
}

