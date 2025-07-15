using System.Collections;
using System.Collections.Generic;
using JetBrains.Annotations;
using UnityEngine;

public class ShootingEnemy : EnemyController
{
    public GameObject bulletPrefab;
    public float fireRate = 1f;
    public float attackRange = 5f; // Tầm bắn, điều chỉnh theo nhu cầu
   
    private float fireTimer;
    [SerializeField] private float delayAnima = 1f;

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
        float attackRangeBuffer = attackRange + 0.5f; // Vùng đệm để tránh dao động
        float chaseRange = 11f; // Khoảng cách tối đa đuổi theo
        float chaseSpeedMultiplier = 1.5f; // Tăng tốc khi đuổi

        if (distance <= attackRange)
        {
            isInRange = true;
            HandleInRangeBehavior(distance);
        }
        else if (distance > attackRangeBuffer && distance <= chaseRange) // Đuổi theo trong khoảng giữa
        {
            isInRange = false;
            // Tăng tốc độ khi đuổi
            Vector2 smoothTarget = Vector2.Lerp(transform.position, player.position, Time.deltaTime * 2f);
            transform.position = Vector2.MoveTowards(transform.position, smoothTarget, moveSpeed * chaseSpeedMultiplier * Time.deltaTime);
            if (skeletonAnim != null)
            {
                skeletonAnim.AnimationState.SetAnimation(0, "Walk", true);
            }
        }
        else if (distance > chaseRange) // Khi quá xa, giảm tốc hoặc bắn từ xa
        {
            isInRange = false;
            // Giảm tốc độ hoặc đứng yên và bắn (tùy chọn)
            Vector2 smoothTarget = Vector2.Lerp(transform.position, player.position, Time.deltaTime * 1f); // Giảm tốc độ Lerp
            transform.position = Vector2.MoveTowards(transform.position, smoothTarget, moveSpeed * 0.5f * Time.deltaTime); // Di chuyển chậm
            if (skeletonAnim != null)
            {
                skeletonAnim.AnimationState.SetAnimation(0, "Walk", true);
            }
            // Có thể thêm logic bắn từ xa nếu muốn
            if (fireTimer >= 1f / fireRate && !isFiring)
            {
                Fire();
                fireTimer = 0f;
            }
            fireTimer += Time.deltaTime;
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
        if (player != null)
        {
            Vector2 dir = (player.position - transform.position).normalized;
            // Thêm ngẫu nhiên vào góc bắn (từ -15 đến +15 độ)
            float randomAngle = Random.Range(-15f, 15f) * Mathf.Deg2Rad;
            Vector2 randomDir = new Vector2(
                dir.x * Mathf.Cos(randomAngle) - dir.y * Mathf.Sin(randomAngle),
                dir.x * Mathf.Sin(randomAngle) + dir.y * Mathf.Cos(randomAngle)
            ).normalized;
            GameObject bullet = MyPoolManager.Instance.Get(bulletPrefab, transform.position);
            if (bullet != null)
            {
                bullet.GetComponent<BulletController>().SetDirection(randomDir);
                Debug.Log($"{gameObject.name} fired bullet with random direction at {transform.position}");
            }
            else
            {
                Debug.LogWarning($"{gameObject.name} failed to get bullet from pool!");
            }
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
    }
}