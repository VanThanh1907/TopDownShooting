using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MeleeEnemy : EnemyController
{
    public float attackRange = 1.5f;
    public float damage = 10f;
    public float attackCooldown = 1.5f;

    private float lastAttackTime;
    private Animator animator;

    

    protected override void Start()
    {
        base.Start();
        animator = GetComponent<Animator>();
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
            animator.SetBool("isAttacking", true);
            animator.SetBool("isRunning", false);
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
        animator.SetBool("isAttacking", false);
        animator.SetBool("isRunning", true); 
    }
}


