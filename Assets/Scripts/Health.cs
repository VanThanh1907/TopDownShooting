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
    Animator animator;

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
            healthBarUI = ui.GetComponent<HealthBarUI>();
            healthBarUI.SetTarget(transform);
            healthBarUI.SetFill(GetHPPercent());
        }
    }

    public void TakeDamage(float dmg)
    {
        currentHP -= dmg;
        Debug.Log($"{gameObject.name} took {dmg} damage, {currentHP}/{maxHP}");

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
    Animator animator = GetComponent<Animator>();
    if (animator != null)
    {
        animator.SetTrigger("Die"); // Tự động chạy đúng kiểu chết của loại enemy đó
    }

    Destroy(gameObject, 1f); // Delay huỷ (có thể thay bằng Animation Event)
}


    public float GetHPPercent()
    {
        return currentHP / maxHP;
    }
}
