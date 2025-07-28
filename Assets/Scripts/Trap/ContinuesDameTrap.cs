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

        // Dừng coroutine cũ nếu có
        if (burnCoroutines.ContainsKey(health) && burnCoroutines[health] != null)
        {
            StopCoroutine(burnCoroutines[health]);
            burnCoroutines[health] = null;
            Debug.Log($"Stopped previous burn for {health.gameObject.name}");
        }

        // Bắt đầu coroutine gây sát thương liên tục
        Coroutine routine = StartCoroutine(BurnRoutine(health, true));
        burnCoroutines[health] = routine;

        // Tạo hiệu ứng lửa từ MyPoolManager
        if (fireEffectPrefab && !fireEffects.ContainsKey(health))
        {
            GameObject fx = MyPoolManager.Instance.Get(fireEffectPrefab, health.transform.position);
            fx.transform.SetParent(health.transform);
            fx.transform.localPosition = Vector3.zero;
            ParticleSystem ps = fx.GetComponent<ParticleSystem>();
            if (ps != null)
            {
                ps.Play();
                Debug.Log($"Started fire effect for {health.gameObject.name} at {health.transform.position}");
            }
            else
            {
                Debug.LogWarning($"No ParticleSystem found on {fireEffectPrefab.name}!");
                fx.SetActive(false); // Trả về pool nếu không có ParticleSystem
            }
            fireEffects[health] = fx;
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        Health health = other.GetComponent<Health>();
        if (health == null || health.IsDead() || !burnCoroutines.ContainsKey(health)) return;

        // Dừng coroutine cũ (infinite = true)
        if (burnCoroutines[health] != null)
        {
            StopCoroutine(burnCoroutines[health]);
            burnCoroutines[health] = null;
            Debug.Log($"Stopped infinite burn for {health.gameObject.name}");
        }

        // Bắt đầu coroutine giới hạn thời gian
        Coroutine routine = StartCoroutine(BurnRoutine(health, false));
        burnCoroutines[health] = routine;
    }

    private IEnumerator BurnRoutine(Health health, bool infinite)
    {
        float timer = 0f;
        while (health != null && !health.IsDead() && (infinite || timer < burnDurationAfterExit))
        {
            if (health.gameObject == null)
            {
                Debug.LogWarning($"Health gameObject for {health} is null. Cleaning up.");
                Cleanup(health);
                yield break;
            }

            health.TakeDamage(damage);
            Debug.Log($"Applied {damage} damage to {health.gameObject.name}. Infinite: {infinite}, Timer: {timer}/{burnDurationAfterExit}");
            timer += damageInterval;
            yield return new WaitForSeconds(damageInterval);
        }

        Debug.Log($"Burn routine ended for {health.gameObject.name}. Infinite: {infinite}, Dead: {health.IsDead()}, Timer: {timer}");
        Cleanup(health);
    }

    private void Cleanup(Health health)
    {
        if (health == null) return;

        // Xóa coroutine
        if (burnCoroutines.ContainsKey(health))
        {
            if (burnCoroutines[health] != null)
            {
                StopCoroutine(burnCoroutines[health]);
            }
            burnCoroutines.Remove(health);
            Debug.Log($"Cleaned up coroutine for {health.gameObject.name}");
        }

        // Trả fire effect về pool
        if (fireEffects.ContainsKey(health))
        {
            GameObject fx = fireEffects[health];
            if (fx != null)
            {
                ParticleSystem ps = fx.GetComponent<ParticleSystem>();
                if (ps != null)
                {
                    ps.Stop(true, ParticleSystemStopBehavior.StopEmittingAndClear);
                }
                fx.transform.SetParent(null); // Tách khỏi health trước khi trả về pool
                fx.SetActive(false); // MyPoolManager sẽ xử lý trả về pool
                
            }
            fireEffects.Remove(health);
        }
    }
}