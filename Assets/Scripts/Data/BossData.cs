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

    //FireZone
    public GameObject fireZonePrefab;
    public float fireZoneDamage; // Sát thương mỗi giây khi chạm vòng lửa
    public float fireZoneDuration; // Thời gian tồn tại của vòng lửa
    public float fireZoneRadius; // Bán kính Lửa
    //IceZone
    public GameObject iceZonePrefab;
    public float iceZoneRadius; // Bán kính băng
    public float iceZoneDuration; // Thơi gian băng tồn tại

    // PoisonZone
    public GameObject poisonZonePrefab;
    public float poisonZoneDamage;  // Sát thương mỗi 0.5s
    public float poisonZoneDuration; // Thời gian tồn tại vùng độc
    public float poisonZoneRadius; //Bán kính độc

    // Summon
    public GameObject summonZonePrefab; // Prefab cho hiệu ứng summon
    public GameObject minionPrefab; // Prefab quái con
    public int minionCount = 4; // Số lượng quái con mỗi lần triệu hồi
    public float summonZoneDuration = 2f; // Thời gian hiệu ứng summon trước khi quái con xuất hiện
    public float summonZoneRadius = 2f; // Bán kính vùng summon

    //Teleport
    public GameObject teleportEffectPrefab;

    public List<FirePattern> patterns; // Danh sách mẫu bắn
    public List<SpecialSkill> specialSkills; // Danh sách kỹ năng đặc biệt

    public enum FirePattern
    {
        TargetPlayer,
        CircleSpread,
        ShootBurstAtPlayer,
        BoomerangShot,
        TrackingShot,
        ShootExplode,
        BarrageRain,
    }

    public enum SpecialSkill
    {
        FireZone,
        IceZone,
        PoisonZone,
        Teleport,
        Summon
    }
}

