using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Events;

public class BossAttack : MonoBehaviour
{
    private BossPhaseData phaseData;
    private BossAnimation bossAnimation;
    public Transform firePoint;
    private float fireTimer;
    private float spiralAngle;
    public float specialSkillCooldown = 10f;
    private float lastAttackTime;
    [SerializeField] private float attackCooldown = 1.5f;

    public void Awake()
    {
        bossAnimation = GetComponent<BossAnimation>();
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
           // Gây damage tại giữa animation (0.7s)
            Invoke(nameof(ApplyMeleeDamage), 0.7f);
           
        }
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
            case BossPhaseData.SpecialSkill.Teleport:
                StartCoroutine( Teleport(player, 2f));
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
        // Tạo vùng lửa quanh người chơi
        GameObject fireZone = MyPoolManager.Instance.Get(phaseData.fireZonePrefab, player.position);
        // Cấu hình vùng lửa (sát thương theo thời gian, thời gian tồn tại, v.v.)
    }

    private void CreateIceZone(Transform player)
    {
        // Tạo vùng băng quanh người chơi
        GameObject iceZone = MyPoolManager.Instance.Get(phaseData.iceZonePrefab, player.position);
        // Cấu hình vùng băng (làm chậm, sát thương, v.v.)
    }

    private IEnumerator Teleport(Transform player, float distanceToPlayer)
    {
        if (player == null) yield break;
        bossAnimation.PlayAnimation("Dead", false);

        yield return new WaitForSeconds(2);
        
        Vector2 randomOffset = Random.insideUnitCircle * distanceToPlayer;
        Vector3 newPosition = player.position + (Vector3)randomOffset;
        transform.position = newPosition;
        PerformMeleeAttack(player);

    }
    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        // Gizmos.DrawWireSphere(transform.position, phaseData.meleeRange);
        Gizmos.DrawWireSphere(transform.position + new Vector3(0, 1, 0), phaseData.meleeRange);
    }
}
       
