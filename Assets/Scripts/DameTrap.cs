using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class DameTrap : MonoBehaviour
{
    public float damage = 10f;
    public float damageInterval = 0.5f;
    private Dictionary<Health, Coroutine> activeCoroutines = new Dictionary<Health, Coroutine>();

    private void OnTriggerEnter2D(Collider2D other)
    {
        Health health = other.GetComponent<Health>();
        if (health != null && !activeCoroutines.ContainsKey(health))
        {
            Coroutine damageRoutine = StartCoroutine(ApplyDamageOverTime(health));
            activeCoroutines.Add(health, damageRoutine);
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        Health health = other.GetComponent<Health>();
        if (health != null && activeCoroutines.ContainsKey(health))
        {
            StopCoroutine(activeCoroutines[health]);
            activeCoroutines.Remove(health);
        }
    }

    private IEnumerator ApplyDamageOverTime(Health health)
    {
        while (health != null && !health.IsDead())
        {
            health.TakeDamage(damage);
            yield return new WaitForSeconds(damageInterval);
        }
    }
}
