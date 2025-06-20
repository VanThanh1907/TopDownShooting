using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum BossState
{
    Idle,
    MoveToPlayer,
    Attack,
    Dead
}

public class BossController : MonoBehaviour
{
    private float fireTimer;

    [Header("Runtime Data")]
    public BossData data;

    public Transform firePoint;
    private Transform player;

    public BossState currentState = BossState.Idle;

    private float stateTimer;
    private int currentPhaseIndex = 0;
    private BossPhaseData currentPhase;
    private Health health;

    private float spiralAngle = 0f;

    public void Setup(BossData bossData)
    {
        data = bossData;
        player = GameObject.FindGameObjectWithTag("Player")?.transform;
        health = GetComponent<Health>(); // Gán health ở đây thay vì Start()

        SwitchToPhase(0); // Khởi động phase đầu tiên
        ChangeState(BossState.Idle, 2f); // Đứng yên 2s
    }

    void Update()
    {
        if (data == null || player == null || currentState == BossState.Dead) return;

        stateTimer -= Time.deltaTime;

        switch (currentState)
        {
            case BossState.Idle:
                if (stateTimer <= 0)
                    ChangeState(BossState.MoveToPlayer, 3f);
                break;

            case BossState.MoveToPlayer:
                MoveTowardsPlayer();
                if (stateTimer <= 0)
                    ChangeState(BossState.Attack, 6f);
                break;

            case BossState.Attack:
                fireTimer += Time.deltaTime;
                if (fireTimer >= 1f / currentPhase.fireRate)
                {
                    Fire();
                    fireTimer = 0f;
                }
                
                if (stateTimer <= 0)
                    ChangeState(BossState.MoveToPlayer, 3f);
                break;
        }

        CheckPhaseChange();
    }

    void MoveTowardsPlayer()
    {
        Vector2 direction = (player.position - transform.position).normalized;
        transform.position += (Vector3)(direction * currentPhase.moveSpeed * Time.deltaTime);
    }

    void Fire()
    {
        switch (currentPhase.pattern)
        {
            case BossPhaseData.FirePattern.TargetPlayer:
                ShootAtPlayer();
                break;
            case BossPhaseData.FirePattern.CircleSpread:
                ShootCircle();
                ShootAtPlayer();
                break;
            case BossPhaseData.FirePattern.Spiral:
                ShootSpiral();
                break;
            case BossPhaseData.FirePattern.ShootDoubleSpiral:
                ShootDoubleSpiral();
                break;
            case BossPhaseData.FirePattern.ShootBurstAtPlayer:
                ShootBurstAtPlayer();
                break;
        }
    }

    void ChangeState(BossState newState, float duration)
    {
        currentState = newState;
        stateTimer = duration;
        fireTimer = 0f;
    }

    void CheckPhaseChange()
    {
       

        float percent = health.CurrentPercent;

        for (int i = data.phases.Count - 1; i >= 0; i--)
        {
            if (percent <= data.phases[i].triggerAtPercent && i > currentPhaseIndex)
            {
                SwitchToPhase(i);
                break;
            }
        }
    }

    void SwitchToPhase(int index)
    {
        currentPhaseIndex = index;
        currentPhase = data.phases[index];
        Debug.Log($"[Boss] Switch to Phase {index + 1}: {currentPhase.pattern}");
    }

    private void SpawnBullet(Vector2 dir)
    {
        GameObject bullet = Instantiate(currentPhase.bulletPrefab, firePoint.position, Quaternion.identity);
        BulletController bc = bullet.GetComponent<BulletController>();
        if (bc != null)
        {
            bc.SetDirection(dir);
        }
    }

    private void ShootAtPlayer()
    {
        Vector2 dir = (player.position - firePoint.position).normalized;
        SpawnBullet(dir);
    }

    private void ShootCircle()
    {
        int bulletCount = 8;
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

    private void ShootBurstAtPlayer()
    {
        Vector2 mainDir = (player.position - firePoint.position).normalized;
        float spreadAngle = 15f;
        for (int i = -2; i <= 2; i++)
        {
            Vector2 dir = Quaternion.Euler(0, 0, i * spreadAngle) * mainDir;
            SpawnBullet(dir);
        }
    }
}
