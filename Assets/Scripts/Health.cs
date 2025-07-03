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
    Animator animator;
    private Collider2D col;
    private bool isDead = false;

    Spine.Unity.SkeletonAnimation skeletonAnim;


    void Awake()
    {
        currentHP = maxHP;
        animator = GetComponentInChildren<Animator>();
        skeletonAnim = GetComponent<Spine.Unity.SkeletonAnimation>();
        col = GetComponent<Collider2D>();
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
        if (isDead) return;

        currentHP -= dmg;
        Debug.Log($"{gameObject.name} took {dmg} damage, {currentHP}/{maxHP}");

        if (damagePopupPrefab != null)
        {
            Vector3 spawnPos = transform.position + popupOffset;
            spawnPos.z = -1f;
            GameObject popup = Instantiate(damagePopupPrefab, spawnPos, Quaternion.identity);
            popup.GetComponent<DamagePopup>().Setup(dmg);

        }
        if (animator != null)
        {
            animator.SetTrigger("Hit");
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
        if (isDead) return;

        isDead = true;
        onDeath?.Invoke();

        if (col != null) col.enabled = false;
        if (healthBarUI != null)
        {
            Destroy(healthBarUI.gameObject);
            healthBarUI = null;
        }

        // Check nếu là Spine

        if (skeletonAnim != null)
        {
            skeletonAnim.AnimationState.SetAnimation(0, "Dead", false); // gọi animation 'die'
            Destroy(gameObject, 2f); // delay để cho animation chạy xong
            return;
        }


        if (animator != null)
        {
            animator.SetInteger("State", 6); // 6 = Dead
            Destroy(gameObject, 1f);
            return;
        }

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

    public bool IsDead()
    {
        return isDead;
    }

}
