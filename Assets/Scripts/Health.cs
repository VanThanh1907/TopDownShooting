using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class Health : MonoBehaviour
{
    public float maxHP = 100f;
    private float currentHP;
    public GameObject healthBarPrefab;
    private HealthBarUI healthBarUI;
    public float CurrentPercent => currentHP / maxHP;
    public UnityEvent onDeath;
    [SerializeField] private GameObject damagePopupPrefab;
    [SerializeField] private Vector3 popupOffset = Vector3.zero;

    void Awake()
    {
        currentHP = maxHP;
    }
    void Start()
    {
        if (healthBarPrefab != null)
        {
            GameObject ui = Instantiate(healthBarPrefab, transform.position, Quaternion.identity);

            ui.transform.SetParent(transform);
            ui.transform.localScale = healthBarPrefab.transform.localScale;
            healthBarUI = ui.GetComponent<HealthBarUI>();
            healthBarUI.SetTarget(transform);
            healthBarUI.SetFill(GetHPPercent());
        }

    }

    public void TakeDamage(float dmg)
    {
        currentHP -= dmg;
        Debug.Log($"{gameObject.name} took {dmg} damage, {currentHP}/{maxHP}");

        if (damagePopupPrefab != null)
        {
            Vector3 spawnPos = transform.position + popupOffset;
            spawnPos.z = -1f;
            GameObject popup = Instantiate(damagePopupPrefab, spawnPos, Quaternion.identity);
            popup.GetComponent<DamagePopup>().Setup(dmg);
            Debug.Log("🟢 Popup created at: " + spawnPos);
        }

        if (healthBarUI != null)
            healthBarUI.SetFill(GetHPPercent());

        if (currentHP <= 0)
        {
            Die();
        }
    }


    public void Die()
    {
        onDeath?.Invoke();
        if (healthBarUI != null)
        {
            Destroy(healthBarUI.gameObject);
            healthBarUI = null;
        }
        Animator animator = GetComponent<Animator>();
        if (animator != null)
        {
            animator.SetTrigger("Die"); // Tự động chạy đúng kiểu chết của loại enemy đó
        }

        Destroy(gameObject, 1f);
    }


    public float GetHPPercent()
    {
        return currentHP / maxHP;
    }
    public void SetFullHP()
    {
        currentHP = maxHP;

        if (healthBarUI != null)
        {
            healthBarUI.SetFill(1f); // 100%
        }
    }

}
