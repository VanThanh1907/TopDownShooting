using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class ContinuesDameTrap : MonoBehaviour
{
    public float damage = 10f;
    public float damageInterval = 0.5f;
    public float burnDurationAfterExit = 3f;
    public GameObject fireEffectPrefab;

    private Dictionary<Health, Coroutine> burnCoroutines = new Dictionary<Health, Coroutine>();
    private Dictionary<Health, GameObject> fireEffects = new Dictionary<Health, GameObject>();

    private void OnTriggerEnter2D(Collider2D other)
    {
        Health health = other.GetComponent<Health>();
        if (health == null || health.IsDead()) return;

        // Nếu đã đang cháy thì bỏ qua
        if (burnCoroutines.ContainsKey(health)) return;

        Coroutine routine = StartCoroutine(BurnRoutine(health, true));
        burnCoroutines[health] = routine;

        if (fireEffectPrefab && !fireEffects.ContainsKey(health))
        {
            GameObject fx = Instantiate(fireEffectPrefab, health.transform);
            fx.transform.localPosition = Vector3.zero;
            fireEffects[health] = fx;
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        Health health = other.GetComponent<Health>();
        if (health == null || !burnCoroutines.ContainsKey(health)) return;
        if (burnCoroutines.ContainsKey(health)) return;

        if (burnCoroutines[health] != null)
        {
            StopCoroutine(burnCoroutines[health]);
            burnCoroutines[health] = null; // Đặt lại để tránh truy cập lại
        }

        // Khởi động coroutine đốt giới hạn thời gian sau khi ra khỏi vùng
        if (health != null && health.gameObject != null) // Kiểm tra GameObject còn tồn tại
        {
            Coroutine routine = StartCoroutine(BurnRoutine(health, false));
            burnCoroutines[health] = routine;
        }
    }

    private IEnumerator BurnRoutine(Health health, bool infinite)
    {
        float timer = 0f;
        while (health != null && !health.IsDead() && (infinite || timer < burnDurationAfterExit))
        {
            if (health.gameObject == null) 
            {
                Cleanup(health);
                yield break;
            }
            health.TakeDamage(damage);
            timer += damageInterval;
            yield return new WaitForSeconds(damageInterval);
        }

        Cleanup(health);
    }

    private void Cleanup(Health health)
    {
        if (health == null) return;
        if (burnCoroutines.ContainsKey(health))
        {
            burnCoroutines.Remove(health);
        }

        if (fireEffects.ContainsKey(health))
        {
            Destroy(fireEffects[health]);
            fireEffects.Remove(health);
        }
    }
}
