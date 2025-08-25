using System.Collections;
using Assets.HeroEditor.Common.CommonScripts;
using UnityEngine;


[RequireComponent(typeof(Rigidbody2D))]
public class PlayerController : MonoBehaviour
{
    [Header("Movement")]
    public float moveSpeed = 5f;
    private bool isFrozen = false; // Trạng thái đóng băng
    public float freezeImmunityTimer = 0f; // Thời gian miễn nhiễm sau khi bị đóng băng
    private float freezeImmunityDuration = 2f;  // 2 giây miễn nhiễm


    [Header("Weapon")]
    public WeaponData weaponData;
    public Transform firePoint;
    [SerializeField] private GameObject freezeEffectPrefab;

    [HideInInspector]
    public WeaponData runtimeWeaponData;

    private float nextFireTime = 0f;
    private Rigidbody2D rb;
    private Vector2 moveInput;
    Animator animator;
    private bool isStanding;
    private Health health;
    public bool isFlipped = true;
    public Coroutine speedUpCoroutine;
    public Coroutine rateUpCoroutine;
    public bool isSpeedUpActive = false;
    public bool isRateUpActive = false;


    void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        animator = GetComponentInChildren<Animator>();
        runtimeWeaponData = Instantiate(weaponData);
        health = GetComponent<Health>();

    }

    void Update()
    {
        if (health != null && health.IsDead()) return;
        if (isFrozen) return;

        // Giảm thời gian miễn nhiễm
        if (freezeImmunityTimer > 0)
        {
            freezeImmunityTimer -= Time.deltaTime;
        }

        if (moveInput.magnitude > 0 && !isStanding)
            animator.SetInteger("State", 0);

        HandleMovementInput();
        FlipToMouseDirection();
        HandleShooting();
    }

    void FixedUpdate()
    {

        Move();
    }

    void HandleMovementInput()
    {

        moveInput = new Vector2(Input.GetAxisRaw("Horizontal"), Input.GetAxisRaw("Vertical")).normalized;
        if (moveInput.magnitude > 0)
        {
            isStanding = false;
            animator.SetInteger("State", 1);
        }
    }

    void Move()
    {
        if (health != null && health.IsDead())
        {
            rb.velocity = Vector2.zero;
            return;
        }

        rb.velocity = moveInput * moveSpeed;
    }

    void FlipToMouseDirection()
    {
        Vector3 mouseWorld = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        float dir = mouseWorld.x - transform.position.x;

        Vector3 scale = transform.localScale;
        if (dir > 0)
        {
            scale.x = Mathf.Abs(scale.x); // mặt phải
            isFlipped = true;
        }
        else if (dir < 0)
        {
            scale.x = -Mathf.Abs(scale.x); // mặt trái
            isFlipped = false;
        }

        transform.localScale = scale;
    }

    void HandleShooting()
    {
        if (Input.GetMouseButton(0) && Time.time >= nextFireTime)
        {
            Shoot();
            nextFireTime = Time.time + 1f / runtimeWeaponData.fireRate;
            animator.Play("PumpShotgun");
        }
    }

    void Shoot()
    {
        GameObject bulletGO = MyPoolManager.Instance.Get(runtimeWeaponData.bulletPrefab, firePoint.position);
        Vector2 shootDir = (Camera.main.ScreenToWorldPoint(Input.mousePosition) - firePoint.position).normalized;

        BulletController bullet = bulletGO.GetComponent<BulletController>();
        if (bullet != null)
        {
            bullet.SetDirection(shootDir);
            bullet.damage = runtimeWeaponData.damage;
        }

        // Sử dụng shootVFX từ WeaponData
        if (runtimeWeaponData.shootVFX != null)
        {
            GameObject muzzleVFX = MyPoolManager.Instance.Get(runtimeWeaponData.shootVFX, firePoint.position);

            if (muzzleVFX != null)
            {

                ParticleSystem ps = muzzleVFX.GetComponentInChildren<ParticleSystem>();
                if (ps != null)
                {
                    float angle = Mathf.Atan2(shootDir.y, shootDir.x) * Mathf.Rad2Deg;
                    muzzleVFX.transform.rotation = Quaternion.Euler(0, 0, angle + 90);
                    ps.Play();
                    StartCoroutine(DisableObjectAfterDuration(muzzleVFX, 1f));
                }
            }
        }
        if (runtimeWeaponData.shootSFX)
            AudioSource.PlayClipAtPoint(runtimeWeaponData.shootSFX, transform.position);
    }

    public void Freeze(float freezeDuration)
    {
        if (freezeImmunityTimer > 0) return;
        isFrozen = true;
        rb.velocity = Vector2.zero; // Đặt vận tốc về 0 ngay lập tức
        rb.constraints = RigidbodyConstraints2D.FreezePosition; // Khóa vị trí Rigidbody
        StartCoroutine(FreezeCoroutine(freezeDuration));
    }

    private IEnumerator FreezeCoroutine(float freezeDuration)
    {
        yield return new WaitForSeconds(freezeDuration);
        // Kết thúc trạng thái đóng băng
        isFrozen = false;
        rb.constraints = RigidbodyConstraints2D.None;
        // Kích hoạt miễn nhiễm 2 giây
        freezeImmunityTimer = freezeImmunityDuration;
    }

    private IEnumerator DisableObjectAfterDuration(GameObject obj, float duration)
    {
        if (obj != null)
        {
            yield return new WaitForSeconds(duration);
            if (obj != null) // Kiểm tra null để tránh lỗi
            {
                obj.SetActive(false); // Tắt để trả về pool
            }
        }
    }
    public bool CanBeFrozen()
    {
        return freezeImmunityTimer <= 0;
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        ItemBase item = other.GetComponent<ItemBase>();
        if (item != null)
        {
            item.Apply(this); // Gọi xử lý item
            Destroy(other.gameObject); // Xoá item sau khi nhặt
        }
    }



}
