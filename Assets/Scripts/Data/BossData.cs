using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "NewBossData", menuName = "ScriptableObjects/BossData")]
public class BossData : ScriptableObject
{
    public GameObject bossPrefab;
    public List<BossPhaseData> phases;
}

[System.Serializable]
public class BossPhaseData
{
    public float triggerAtPercent; // % máu để kích hoạt phase
    public float moveSpeed; // Tốc độ di chuyển
    public float fireRate; // Tốc độ bắn đạn
    public GameObject bulletPrefab; // Prefab đạn
    public float meleeRange; // Tầm đánh cận chiến
    public float meleeDamage; // Sát thương cận chiến
    public GameObject fireZonePrefab; // Prefab vùng lửa
    public float fireZoneDamage = 10f; // Sát thương mỗi giây khi chạm vòng lửa
    public float fireZoneDuration = 5f; // Thời gian tồn tại của vòng lửa
    public float fireZoneRadius = 3f; // Bán kính Lửa
    public GameObject iceZonePrefab; // Prefab vùng băng
    public List<FirePattern> patterns; // Danh sách mẫu bắn
    public List<SpecialSkill> specialSkills; // Danh sách kỹ năng đặc biệt

    public enum FirePattern
    {
        TargetPlayer,
        CircleSpread,
        Spiral,
        ShootDoubleSpiral,
        ShootBurstAtPlayer
    }

    public enum SpecialSkill
    {
        FireZone,
        IceZone,
        Teleport
    }
}

