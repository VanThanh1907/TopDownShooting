using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShootingEnemy : EnemyController
{
    public GameObject bulletPrefab;
    public float fireRate = 1f;
    public float attackRange = 5f; // Tầm bắn, điều chỉnh theo nhu cầu
    private float fireTimer;
    [SerializeField]private float delayAnima =1f;

    private Spine.Unity.SkeletonAnimation skeletonAnim;
    private bool isFiring = false;
    public bool isInRange = false; // Flag để kiểm soát di chuyển

    protected override void Start()
    {
        base.Start();
        skeletonAnim = GetComponent<Spine.Unity.SkeletonAnimation>();
        if (skeletonAnim == null)
        {
            Debug.LogWarning($"{gameObject.name} missing SkeletonAnimation component!");
        }
    }

    protected override void Update()
    {
        if (player == null || (health != null && health.IsDead()))
            return;

        float distance = Vector2.Distance(transform.position, player.position);

        // Nếu trong tầm attackRange, đứng yên và xử lý bắn/idle
        if (distance <= attackRange)
        {
            isInRange = true;
            HandleInRangeBehavior(distance);
        }
        else
        {
            isInRange = false;
            // Ra ngoài tầm, cho phép di chuyển theo player
            base.Update();
            if (skeletonAnim != null)
            {
                skeletonAnim.AnimationState.SetAnimation(0, "Walk", true); // Đi khi ra ngoài tầm
            }
        }
    }

    void HandleInRangeBehavior(float distance)
    {
        fireTimer += Time.deltaTime;
        if (fireTimer >= 1f / fireRate && !isFiring)
        {
            Fire();
            fireTimer = 0f;
        }

        // Chuyển về Idle khi animation Attack kết thúc
        if (isFiring && skeletonAnim != null && skeletonAnim.AnimationState.GetCurrent(0).IsComplete)
        {
            Idle();
        }
    }

    void Fire()
    {
        isFiring = true;
        if (skeletonAnim != null)
        {
            skeletonAnim.AnimationState.SetAnimation(0, "Attack", false); // Phát animation Attack
        }
        StartCoroutine(DelayedFire());
    }

        IEnumerator DelayedFire()
    {
        yield return new WaitForSeconds(delayAnima); // Điều chỉnh delay theo tốc độ animation
        Vector2 dir = (player != null) ? (player.position - transform.position).normalized : Vector2.right;
        GameObject bullet = MyPoolManager.Instance.Get(bulletPrefab, transform.position);
        if (bullet != null)
        {
            bullet.GetComponent<BulletController>().SetDirection(dir);
        }
        else
        {
            Debug.LogWarning($"{gameObject.name} failed to get bullet from pool!");
        }
    }

    void Idle()
    {
        isFiring = false;
        if (skeletonAnim != null)
        {
            skeletonAnim.AnimationState.SetAnimation(0, "Idle", true); // Chuyển về Idle khi trong tầm
        }
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, attackRange); 
        Gizmos.DrawWireSphere(transform.position, attackRange+4); 
    }
}