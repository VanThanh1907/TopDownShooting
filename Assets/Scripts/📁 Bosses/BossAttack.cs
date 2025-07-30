using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Events;

public class BossAttack : MonoBehaviour
{
    private BossPhaseData phaseData;
    private BossAnimation bossAnimation;
    private BossMovement bossMove;
    public Transform firePoint;
    private float fireTimer;
    private float spiralAngle;
    public float specialSkillCooldown = 10f;
    private float lastAttackTime;
    [SerializeField] private float attackCooldown = 1.5f;

    public void Awake()
    {
        bossAnimation = GetComponent<BossAnimation>();
        bossMove = GetComponent<BossMovement>();
    }
    public void Setup(BossPhaseData phase, Transform firePoint)
    {
        this.phaseData = phase;
        this.firePoint = firePoint;
        lastAttackTime = -attackCooldown;
    }
    public bool CanPerformMeleeAttack(Transform player)
    {
        if (player == null) return false;
        Health health = GetComponent<Health>();
        if (health != null && health.IsDead()) return false;
        float distance = Vector2.Distance(transform.position, player.position);
        return distance <= phaseData.meleeRange;
    }

    public bool HasSpecialSkill()
    {
        return phaseData != null && phaseData.specialSkills != null && phaseData.specialSkills.Count > 0;
    }

    public void PerformMeleeAttack(Transform player)
    {
        float distance = Vector2.Distance(player.position, transform.position + new Vector3(0, 1, 0));
        if (distance <= phaseData.meleeRange && Time.time >= lastAttackTime + attackCooldown)
        {
           
            lastAttackTime = Time.time;
            bossAnimation.PlayAnimation("Attack", false);

            bool isFacingPlayer = IsFacingPlayer(player);
            // Gây damage tại (0.7s)
            if (isFacingPlayer)
            {
                Invoke(nameof(ApplyMeleeDamage), 0.7f);
            }

        }
    }
    private bool IsFacingPlayer(Transform player)
    {
        bool playerOnLeft = player.position.x < transform.position.x;
        // Boss nhìn đúng hướng nếu:
        // - Người chơi ở bên trái (playerOnLeft = true) và boss lật sang trái (isFlipped = true)
        // - Người chơi ở bên phải (playerOnLeft = false) và boss lật sang phải (isFlipped = false)
        return playerOnLeft == bossMove.isFlipped;
    }
    private void ApplyMeleeDamage()
    {
        Transform player = GameObject.FindGameObjectWithTag("Player")?.transform;
        if (player != null)
        {
            Health playerHealth = player.GetComponent<Health>();
            if (playerHealth != null)
            {
                playerHealth.TakeDamage(phaseData.meleeDamage);
                Debug.Log($"Applied {phaseData.meleeDamage} melee damage to player");
            }
        }
    }

    public void PerformRangedAttack(Transform player)
    {
        fireTimer += Time.deltaTime;
        if (fireTimer >= 1f / phaseData.fireRate)
        {
            Fire(player);
            fireTimer = 0f;
        }
    }

    public void PerformSpecialSkill(Transform player)
    {
        Debug.LogWarning($"Performing Special Skill at {Time.time}");
        if (phaseData.specialSkills == null || phaseData.specialSkills.Count == 0)
        {
            Debug.LogWarning("No special skills configured for this phase!");
            return;
        }
        // Chọn ngẫu nhiên kỹ năng đặc biệt
        int skillIndex = Random.Range(0, phaseData.specialSkills.Count);
        switch (phaseData.specialSkills[skillIndex])
        {
            case BossPhaseData.SpecialSkill.FireZone:
                CreateFireZone(player);
                break;
            case BossPhaseData.SpecialSkill.IceZone:
                CreateIceZone(player);
                break;
            case BossPhaseData.SpecialSkill.PoisonZone:
                CreatePoisonZone(player);
                break;
            case BossPhaseData.SpecialSkill.Summon:
                CreateSummonZone(player);
                break;
            case BossPhaseData.SpecialSkill.Teleport:
                StartCoroutine(Teleport(player, 2f));
                break;
        }
    }

    private void Fire(Transform player)
    {
        // Tái sử dụng logic bắn đạn từ script gốc
        if (phaseData.patterns == null || phaseData.patterns.Count == 0) return;
        BossPhaseData.FirePattern selectedPattern = phaseData.patterns[Random.Range(0, phaseData.patterns.Count)];

        switch (selectedPattern)
        {
            case BossPhaseData.FirePattern.TargetPlayer:
                ShootAtPlayer(player);
                break;
            case BossPhaseData.FirePattern.CircleSpread:
                ShootCircle();
                break;
            case BossPhaseData.FirePattern.Spiral:
                ShootSpiral();
                break;
            case BossPhaseData.FirePattern.ShootDoubleSpiral:
                ShootDoubleSpiral();
                break;
            case BossPhaseData.FirePattern.ShootBurstAtPlayer:
                ShootBurstAtPlayer(player);
                break;
        }
    }

    private void ShootAtPlayer(Transform player)
    {
        Vector2 dir = (player.position - firePoint.position).normalized;
        SpawnBullet(dir);
    }

    private void ShootCircle()
    {
        int bulletCount = 5;
        for (int i = 0; i < bulletCount; i++)
        {
            float angle = i * (360f / bulletCount);
            Vector2 dir = Quaternion.Euler(0, 0, angle) * Vector2.right;
            SpawnBullet(dir);
        }
    }

    private void ShootSpiral()
    {
        float angle = spiralAngle % 360f;
        spiralAngle += 20f;
        Vector2 dir = Quaternion.Euler(0, 0, angle) * Vector2.right;
        SpawnBullet(dir);
    }

    private void ShootDoubleSpiral()
    {
        float angle1 = spiralAngle % 360f;
        float angle2 = (spiralAngle + 180f) % 360f;
        spiralAngle += 15f;

        Vector2 dir1 = Quaternion.Euler(0, 0, angle1) * Vector2.right;
        Vector2 dir2 = Quaternion.Euler(0, 0, angle2) * Vector2.right;

        SpawnBullet(dir1);
        SpawnBullet(dir2);
    }

    private void ShootBurstAtPlayer(Transform player)
    {
        Vector2 mainDir = (player.position - firePoint.position).normalized;
        float spreadAngle = 15f;
        for (int i = -2; i <= 2; i++)
        {
            Vector2 dir = Quaternion.Euler(0, 0, i * spreadAngle) * mainDir;
            SpawnBullet(dir);
        }
    }

    private void SpawnBullet(Vector2 dir)
    {
        GameObject bullet = MyPoolManager.Instance.Get(phaseData.bulletPrefab, firePoint.position);
        BulletController bc = bullet.GetComponent<BulletController>();
        if (bc != null)
        {
            bc.SetDirection(dir);
        }
    }

    private void CreateFireZone(Transform player)
    {
        if (player == null || phaseData.fireZonePrefab == null)
        {
            return;
        }

        // Cấu hình vòng tròn lửa
        int fireZoneCount = 30; // Số lượng vùng lửa trong vòng tròn
        float circleRadius = phaseData.fireZoneRadius * 2.5f; // Bán kính vòng tròn (lớn hơn bán kính mỗi vùng lửa)
        Vector3 center = player.position; // Tâm vòng tròn là vị trí người chơi

        for (int i = 0; i < fireZoneCount; i++)
        {
            // Tính góc cho mỗi vùng lửa
            float angle = i * (360f / fireZoneCount);
            Vector3 offset = Quaternion.Euler(0, 0, angle) * Vector3.right * circleRadius;
            Vector3 fireZonePosition = center + offset;

            // Tạo vùng lửa từ pool
            GameObject fireZone = MyPoolManager.Instance.Get(phaseData.fireZonePrefab, fireZonePosition);
            FireZoneController fireZoneController = fireZone.GetComponent<FireZoneController>();
            if (fireZoneController != null)
            {
                fireZoneController.Setup(phaseData.fireZoneDamage, phaseData.fireZoneDuration, phaseData.fireZoneRadius);
            }
        }
    }

    private void CreateIceZone(Transform player)
    {
        if (player == null || phaseData.iceZonePrefab == null)
        {
            return;
        }

        // Cấu hình các vùng băng bao quanh người chơi
        int iceZoneCount = 6; // Số lượng vùng băng (chẵn để tạo khoảng trống đều)
        float minRadius = phaseData.iceZoneRadius * 5f; // Khoảng cách tối thiểu từ người chơi
        float maxRadius = phaseData.iceZoneRadius * 10f; // Khoảng cách tối đa
        float minDistanceBetweenZones = phaseData.iceZoneRadius * 2f; // Khoảng cách tối thiểu giữa các vùng
        Vector3 center = player.position; // Tâm là vị trí người chơi
        List<Vector3> placedPositions = new List<Vector3>(); // Lưu các vị trí đã đặt

        for (int i = 0; i < iceZoneCount; i++)
        {
            Vector3 iceZonePosition = Vector3.zero; // Gán giá trị mặc định
            bool validPosition = false;
            int attempts = 0;
            const int maxAttempts = 30;

            // Tính góc đều cho mỗi vùng
            float angle = i * (360f / iceZoneCount);
            Vector2 direction = new Vector2(Mathf.Cos(angle * Mathf.Deg2Rad), Mathf.Sin(angle * Mathf.Deg2Rad));

            // Thử tạo vị trí ngẫu nhiên trong phạm vi minRadius đến maxRadius
            while (!validPosition && attempts < maxAttempts)
            {
                float randomRadius = Random.Range(minRadius, maxRadius);
                iceZonePosition = center + new Vector3(direction.x * randomRadius, direction.y * randomRadius, 0);

                // Kiểm tra khoảng cách với các vùng khác
                bool tooCloseToOtherZones = false;
                foreach (Vector3 pos in placedPositions)
                {
                    if (Vector3.Distance(iceZonePosition, pos) < minDistanceBetweenZones)
                    {
                        tooCloseToOtherZones = true;
                        break;
                    }
                }

                if (!tooCloseToOtherZones)
                {
                    validPosition = true;
                }

                attempts++;
            }

            if (validPosition)
            {
                // Tạo vùng băng từ pool chỉ khi vị trí hợp lệ
                GameObject iceZone = MyPoolManager.Instance.Get(phaseData.iceZonePrefab, iceZonePosition);
                IceZoneController iceZoneController = iceZone.GetComponent<IceZoneController>();
                if (iceZoneController != null)
                {
                    iceZoneController.Setup(phaseData.iceZoneDuration, phaseData.iceZoneRadius, 0f);
                }
                placedPositions.Add(iceZonePosition);
            }
        }
    }

    private void CreatePoisonZone(Transform player)
    {
        if (player == null || phaseData.poisonZonePrefab == null)
        {
            Debug.LogWarning("Cannot create PoisonZone: Player or poisonZonePrefab is null!");
            return;
        }

        // Cấu hình các vùng độc bao quanh người chơi
        int poisonZoneCount = 6; // Số lượng vùng độc
        float minRadius = phaseData.poisonZoneRadius * 5f; // Khoảng cách tối thiểu từ người chơi
        float maxRadius = phaseData.poisonZoneRadius * 10f; // Khoảng cách tối đa
        float minDistanceBetweenZones = phaseData.poisonZoneRadius * 2f; // Khoảng cách tối thiểu giữa các vùng
        Vector3 center = player.position; // Tâm là vị trí người chơi
        List<Vector3> placedPositions = new List<Vector3>(); // Lưu các vị trí đã đặt

        for (int i = 0; i < poisonZoneCount; i++)
        {
            Vector3 poisonZonePosition = Vector3.zero;
            bool validPosition = false;
            int attempts = 0;
            const int maxAttempts = 30;

            // Tính góc đều cho mỗi vùng
            float angle = i * (360f / poisonZoneCount);
            Vector2 direction = new Vector2(Mathf.Cos(angle * Mathf.Deg2Rad), Mathf.Sin(angle * Mathf.Deg2Rad));

            // Thử tạo vị trí ngẫu nhiên trong phạm vi minRadius đến maxRadius
            while (!validPosition && attempts < maxAttempts)
            {
                float randomRadius = Random.Range(minRadius, maxRadius);
                poisonZonePosition = center + new Vector3(direction.x * randomRadius, direction.y * randomRadius, 0);

                // Kiểm tra khoảng cách với các vùng khác
                bool tooCloseToOtherZones = false;
                foreach (Vector3 pos in placedPositions)
                {
                    if (Vector3.Distance(poisonZonePosition, pos) < minDistanceBetweenZones)
                    {
                        tooCloseToOtherZones = true;
                        break;
                    }
                }

                if (!tooCloseToOtherZones)
                {
                    validPosition = true;
                }

                attempts++;
            }

            if (validPosition)
            {
                // Tạo vùng độc từ pool chỉ khi vị trí hợp lệ
                GameObject poisonZone = MyPoolManager.Instance.Get(phaseData.poisonZonePrefab, poisonZonePosition);
                PoisonZoneController poisonZoneController = poisonZone.GetComponent<PoisonZoneController>();
                if (poisonZoneController != null)
                {
                    poisonZoneController.Setup(phaseData.poisonZoneDamage, phaseData.poisonZoneDuration, phaseData.poisonZoneRadius);
                    Debug.Log($"Created PoisonZone {i + 1} at {poisonZonePosition} with damage {phaseData.poisonZoneDamage}, duration {phaseData.poisonZoneDuration}, radius {phaseData.poisonZoneRadius}");
                }
                else
                {
                    Debug.LogWarning("PoisonZoneController component not found on poisonZonePrefab!");
                }
                placedPositions.Add(poisonZonePosition);
            }
        }

        Debug.Log($"Created {poisonZoneCount} PoisonZones in a circle around player at {center}");
    }

    private void CreateSummonZone(Transform player)
    {
        if (player == null || phaseData.summonZonePrefab == null || phaseData.minionPrefab == null)
        {
            Debug.LogWarning("Cannot create SummonZone: Player, summonZonePrefab, or minionPrefab is null!");
            return;
        }

        int summonZoneCount = 4; // Số lượng vùng summon
        float minRadius = phaseData.summonZoneRadius * 5f; // Khoảng cách tối thiểu từ người chơi
        float maxRadius = phaseData.summonZoneRadius * 10f; // Khoảng cách tối đa
        float minDistanceBetweenZones = phaseData.summonZoneRadius * 2f; // Khoảng cách tối thiểu giữa các vùng
        Vector3 center = player.position; // Tâm là vị trí người chơi
        List<Vector3> placedPositions = new List<Vector3>(); // Lưu các vị trí đã đặt

        for (int i = 0; i < summonZoneCount; i++)
        {
            Vector3 summonZonePosition = Vector3.zero;
            bool validPosition = false;
            int attempts = 0;
            const int maxAttempts = 30;

            float angle = i * (360f / summonZoneCount);
            Vector2 direction = new Vector2(Mathf.Cos(angle * Mathf.Deg2Rad), Mathf.Sin(angle * Mathf.Deg2Rad));

            while (!validPosition && attempts < maxAttempts)
            {
                float randomRadius = Random.Range(minRadius, maxRadius);
                summonZonePosition = center + new Vector3(direction.x * randomRadius, direction.y * randomRadius, 0);

                bool tooCloseToOtherZones = false;
                foreach (Vector3 pos in placedPositions)
                {
                    if (Vector3.Distance(summonZonePosition, pos) < minDistanceBetweenZones)
                    {
                        tooCloseToOtherZones = true;
                        break;
                    }
                }

                if (!tooCloseToOtherZones)
                {
                    validPosition = true;
                }

                attempts++;
            }

            if (validPosition)
            {
                GameObject summonZone = MyPoolManager.Instance.Get(phaseData.summonZonePrefab, summonZonePosition);
                SummonZoneController summonZoneController = summonZone.GetComponent<SummonZoneController>();
                if (summonZoneController != null)
                {
                    summonZoneController.Setup(
                        phaseData.minionPrefab,
                        phaseData.minionCount,
                        phaseData.summonZoneDuration,
                        phaseData.summonZoneRadius
                    );
                    Debug.Log($"Created SummonZone {i + 1} at {summonZonePosition} with {phaseData.minionCount} minions");
                }
                else
                {
                    Debug.LogWarning("SummonZoneController component not found on summonZonePrefab!");
                }
                placedPositions.Add(summonZonePosition);
            }
        }

        Debug.Log($"Created {placedPositions.Count} SummonZones around player at {center}");
    }


    private IEnumerator Teleport(Transform player, float distanceToPlayer)
    {
        if (player == null || phaseData == null || phaseData.teleportEffectPrefab == null)
        {
            yield break;
        }
        bossAnimation.PlayAnimation("Dead", false);
        yield return new WaitForSeconds(2f);

        Vector2 randomOffset = Random.insideUnitCircle * distanceToPlayer;
        Vector3 newPosition = player.position + (Vector3)randomOffset;


        GameObject teleportEffect = MyPoolManager.Instance.Get(phaseData.teleportEffectPrefab, newPosition);
        if (teleportEffect != null)
        {
            ParticleSystem ps = teleportEffect.GetComponent<ParticleSystem>();
            if (ps != null)
            {
                ps.Play();
                yield return new WaitForSeconds(0.1f);
            }

            teleportEffect.SetActive(false);
        }
        transform.position = newPosition;
        bossMove.MoveToPlayer(player);
        PerformMeleeAttack(player);
    }


    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position + new Vector3(0, 1, 0), phaseData.meleeRange);
    }
}

