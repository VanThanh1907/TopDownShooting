using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
public class PlayerController : MonoBehaviour
{
    [Header("Movement")]
    public float moveSpeed = 5f;

    [Header("Weapon")]
    public WeaponData weaponData;
    public Transform firePoint;

    [HideInInspector]
    public WeaponData runtimeWeaponData;

    private float nextFireTime = 0f;
    private Rigidbody2D rb;
    private Vector2 moveInput;
    Animator animator;
    private bool isStanding;
    [SerializeField] private ParticleSystem shootVFX;

    public bool isFlipped = true;
    
    void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        animator = GetComponentInChildren<Animator>();
        runtimeWeaponData = Instantiate(weaponData);

    }

    void Update()
    {
        if (moveInput.magnitude > 0 && !isStanding)
            animator.SetInteger("State", 0);

        HandleMovementInput();
        FlipToMouseDirection();
        // RotateToMouse();
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
        GameObject bulletGO = Instantiate(runtimeWeaponData.bulletPrefab, firePoint.position, Quaternion.identity);
        Vector2 shootDir = (Camera.main.ScreenToWorldPoint(Input.mousePosition) - firePoint.position).normalized;

        BulletController bullet = bulletGO.GetComponent<BulletController>();
        if (bullet != null)
        {
            bullet.SetDirection(shootDir);
            bullet.damage = runtimeWeaponData.damage;
        }

        if (shootVFX != null)
        {
            float angle = Mathf.Atan2(shootDir.y, shootDir.x) * Mathf.Rad2Deg;
            shootVFX.transform.rotation = Quaternion.Euler(0, 0, angle + 90);
            shootVFX.Play();
        }


        if (runtimeWeaponData.shootSFX)
            AudioSource.PlayClipAtPoint(runtimeWeaponData.shootSFX, transform.position);
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
