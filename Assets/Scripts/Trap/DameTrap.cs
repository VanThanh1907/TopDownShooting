using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class DameTrap : MonoBehaviour
{
    public float damage = 10f;
    public float damageInterval = 0.5f;
    public GameObject EffectPrefab; // ✅ Gán Particle X ở Inspector

    private Dictionary<Health, Coroutine> activeCoroutines = new Dictionary<Health, Coroutine>();
    private Dictionary<Health, GameObject> activeFireEffects = new Dictionary<Health, GameObject>();

    private void OnTriggerEnter2D(Collider2D other)
    {
        Health health = other.GetComponent<Health>();
        if (health != null && !activeCoroutines.ContainsKey(health))
        {
            // ✅ Bắt đầu gây sát thương
            Coroutine damageRoutine = StartCoroutine(ApplyDamageOverTime(health));
            activeCoroutines.Add(health, damageRoutine);

            // ✅ Tạo hiệu ứng lửa nếu chưa có
            if (EffectPrefab != null && !activeFireEffects.ContainsKey(health))
            {
                GameObject fireEffect = Instantiate(EffectPrefab, health.transform);
                fireEffect.transform.localPosition = Vector3.zero;
                activeFireEffects.Add(health, fireEffect);
            }
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        Health health = other.GetComponent<Health>();
        if (health != null)
        {
            // ✅ Ngưng gây sát thương
            if (activeCoroutines.ContainsKey(health))
            {
                StopCoroutine(activeCoroutines[health]);
                activeCoroutines.Remove(health);
            }

            // ✅ Xoá hiệu ứng
            if (activeFireEffects.ContainsKey(health))
            {
                Destroy(activeFireEffects[health]);
                activeFireEffects.Remove(health);
            }
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
