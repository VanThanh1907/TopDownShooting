using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "SO/Boss/Phase")]
public class BossPhaseData : ScriptableObject
{
    public float triggerAtPercent = 1f; // Giai đoạn bắt đầu khi máu <= % này
    public float moveSpeed = 5f;
    public float fireRate = 1f;
    public GameObject bulletPrefab;
     public List<FirePattern> patterns; // List of possible fire patterns

    public enum FirePattern
    {
        TargetPlayer,
        CircleSpread,
        Spiral,
        ShootDoubleSpiral,
        ShootBurstAtPlayer
    }
}
