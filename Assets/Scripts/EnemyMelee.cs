using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MeleeEnemy : EnemyController
{
    public float attackRange = 1.5f;
    public float damage = 10f;
    public float attackCooldown = 1.5f;

    private float lastAttackTime;
    
    Spine.Unity.SkeletonAnimation skeletonAnim;




    protected override void Start()
    {
        base.Start();
        skeletonAnim = GetComponent<Spine.Unity.SkeletonAnimation>();
        
    }

    protected override void Update()
    {
        base.Update();
         if ( player == null)
            return;
        float distance = Vector2.Distance(transform.position, player.position);

        // Nếu trong tầm và cooldown xong
        if (distance <= attackRange && Time.time >= lastAttackTime + attackCooldown)
        {
            lastAttackTime = Time.time;

            // Gọi animation đánh
            skeletonAnim.AnimationState.SetAnimation(0, "Attack", false);
           
            // Gây damage sau 0.3s (giả định tay vung trúng)
            Invoke(nameof(ApplyDamage), 0.3f);
            // Tắt trạng thái đánh sau thời gian (giả định thời gian animation)
            Invoke(nameof(EndAttack), 0.6f);
        }
    }

    void ApplyDamage()
    {
        if (player != null && Vector2.Distance(transform.position, player.position) <= attackRange)
        {
            player.GetComponent<Health>()?.TakeDamage(damage);
        }
    }

    void EndAttack()
    {
        skeletonAnim.AnimationState.SetAnimation(0, "Walk", true);
    }
}


