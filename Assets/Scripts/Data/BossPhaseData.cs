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
    public FirePattern pattern; // Enum chiêu thức
    public float idleDuration = 2f; // Thời gian đứng yên
    public float moveDuration = 3f; // Thời gian di chuyển
    public float attackDuration = 6f; // Thời gian tấn công

    public enum FirePattern
    {
        TargetPlayer,
        CircleSpread,
        Spiral,
        ShootDoubleSpiral,
        ShootBurstAtPlayer
    }
}
