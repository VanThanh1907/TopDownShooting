using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DameTrap : MonoBehaviour
{
    public float damage = 100f;

    private void OnTriggerEnter2D(Collider2D other)
    {
       
        Health health = other.GetComponent<Health>();
        if (health != null && !health.IsDead())
        {
            health.TakeDamage(damage);
        }
    }
}
