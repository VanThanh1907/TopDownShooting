using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MeleeEnemy : EnemyController
{
    public float attackRange = 1.5f;
    public float damage = 10f;

    protected override void Update()
    {
        base.Update();

        if (player != null && Vector2.Distance(transform.position, player.position) <= attackRange)
        {
            Attack();
        }
    }

    void Attack()
    {
        player.GetComponent<Health>()?.TakeDamage(damage);
        Destroy(gameObject); // hoặc cooldown, hoặc animation chém
    }
}
