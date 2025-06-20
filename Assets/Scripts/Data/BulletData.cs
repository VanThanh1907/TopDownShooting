using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "NewBulletData", menuName = "ScriptableObjects/BulletData")]
public class BulletData : ScriptableObject
{
    public float speed;
    public float lifeTime;
    public float damage;
    public Sprite bulletSprite;
}
