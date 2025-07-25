using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PoisonZoneController : MonoBehaviour
{
    private float damage;
    private float duration;
    private float radius;
    private float timeElapsed;
    private float damageTimer;
    private float damageInterval = 0.5f; // Gây sát thương định kỳ mỗi s
    private float particleInterval = 1f; // Tần suất sinh particle effect (1 giây/lần)
    private float lastParticleTime; // Thời điểm sinh particle gần nhất
    [SerializeField] private GameObject poisonEffectPrefab; // Particle effect độc
    private readonly HashSet<Health> affectedPlayers = new HashSet<Health>(); // Lưu danh sách người chơi đang chịu hiệu ứng
    private static CoroutineManager coroutineManager; // Manager để chạy coroutine

    // Khởi tạo CoroutineManager
    private void Awake()
    {
        if (coroutineManager == null)
        {
            GameObject managerObject = new GameObject("CoroutineManager");
            coroutineManager = managerObject.AddComponent<CoroutineManager>();
            DontDestroyOnLoad(managerObject);
        }
    }

    public void Setup(float damage, float duration, float radius)
    {
        this.damage = damage;
        this.duration = duration;
        this.radius = radius;
        timeElapsed = 0f;
        damageTimer = 0f;
        lastParticleTime = -particleInterval; // Cho phép sinh particle ngay lần đầu
        affectedPlayers.Clear();
    }

    void Update()
    {
        timeElapsed += Time.deltaTime;
        if (timeElapsed >= duration)
        {
            gameObject.SetActive(false);
        }
    }

    private void OnTriggerStay2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            Health playerHealth = other.GetComponent<Health>();
            if (playerHealth != null && !playerHealth.IsDead())
            {
                damageTimer += Time.deltaTime;
                if (damageTimer >= damageInterval)
                {
                    ApplyPoisonDamage(playerHealth);
                    affectedPlayers.Add(playerHealth); // Đánh dấu người chơi đang trong vùng
                    damageTimer = 0f; // Reset timer sau khi gây sát thương
                }
            }
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            Health playerHealth = other.GetComponent<Health>();
            if (playerHealth != null && affectedPlayers.Contains(playerHealth))
            {
                // Bắt đầu hiệu ứng độc kéo dài
                coroutineManager.StartCoroutine(ApplyLingeringPoison(playerHealth));
                affectedPlayers.Remove(playerHealth); // Loại bỏ khỏi danh sách
            }
        }
    }

    private void ApplyPoisonDamage(Health playerHealth)
    {
        if (playerHealth != null && !playerHealth.IsDead())
        {
            playerHealth.TakeDamage(damage);
            Debug.Log($"Applied {damage} poison damage to player at {Time.time}");

            // Kích hoạt particle effect độc (giới hạn tần suất)
            if (poisonEffectPrefab != null && Time.time >= lastParticleTime + particleInterval)
            {
                GameObject particle = MyPoolManager.Instance.Get(poisonEffectPrefab, playerHealth.transform.position);
                if (particle != null)
                {
                    // Đảm bảo particle system tự động dừng
                    ParticleSystem ps = particle.GetComponent<ParticleSystem>();
                    if (ps != null)
                    {
                        ParticleSystem.MainModule main = ps.main;
                        if (!main.loop)
                        {
                            ps.Play();
                        }
                    }
                    coroutineManager.StartCoroutine(DisableObjectAfterDuration(particle, 1f));
                    lastParticleTime = Time.time; // Cập nhật thời điểm sinh particle
                }
                else
                {
                    Debug.LogWarning("Failed to get poisonEffectPrefab from pool!");
                }
            }
        }
    }

    private IEnumerator ApplyLingeringPoison(Health playerHealth)
    {
        float lingeringDuration = Random.Range(5f, 10f); // Thời gian độc kéo dài 5-10s
        float lingeringTimer = 0f;
        float lingeringDamageInterval = damageInterval; // Giữ cùng tần suất sát thương
        float lastLingeringParticleTime = -particleInterval; // Thời điểm sinh particle trong hiệu ứng kéo dài

        while (lingeringTimer < lingeringDuration && playerHealth != null && !playerHealth.IsDead())
        {
            playerHealth.TakeDamage(damage);
            Debug.Log($"Applied {damage} lingering poison damage to player at {Time.time}");

            // Kích hoạt particle effect độc (giới hạn tần suất)
            if (poisonEffectPrefab != null && Time.time >= lastLingeringParticleTime + particleInterval)
            {
                GameObject particle = MyPoolManager.Instance.Get(poisonEffectPrefab, playerHealth.transform.position);
                if (particle != null)
                {
                    ParticleSystem ps = particle.GetComponent<ParticleSystem>();
                    if (ps != null)
                    {
                        ParticleSystem.MainModule main = ps.main;
                        if (!main.loop)
                        {
                            ps.Play();
                        }
                    }
                    coroutineManager.StartCoroutine(DisableObjectAfterDuration(particle, 1f));
                    lastLingeringParticleTime = Time.time;
                }
                else
                {
                    Debug.LogWarning("Failed to get poisonEffectPrefab from pool during lingering poison!");
                }
            }

            yield return new WaitForSeconds(lingeringDamageInterval);
            lingeringTimer += lingeringDamageInterval;
        }
    }

    private IEnumerator DisableObjectAfterDuration(GameObject obj, float duration)
    {
        yield return new WaitForSeconds(duration);
        if (obj != null)
        {
            // Dừng particle system trước khi vô hiệu hóa
            ParticleSystem ps = obj.GetComponent<ParticleSystem>();
            if (ps != null)
            {
                ps.Stop(true, ParticleSystemStopBehavior.StopEmittingAndClear);
            }
            obj.SetActive(false); // Vô hiệu hóa object
            Debug.Log($"Disabled and returned poison effect object {obj.name} to pool");
        }
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.green;
        Gizmos.DrawWireSphere(transform.position, radius);
    }
}

// Class để quản lý coroutine, tránh bị gián đoạn khi PoisonZone bị vô hiệu hóa
public class CoroutineManager : MonoBehaviour
{
    private static CoroutineManager instance;

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }
}