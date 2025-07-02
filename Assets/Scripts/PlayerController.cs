using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
public class PlayerController : MonoBehaviour
{
    [Header("Movement")]
    public float moveSpeed = 5f;

    [Header("Weapon")]
    public WeaponData weaponData;
    public Transform firePoint;

    private float nextFireTime = 0f;
    private Rigidbody2D rb;
    private Vector2 moveInput;

    void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        
    }

    void Update()
    {
        HandleMovementInput();
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
    }

    void Move()
    {
        rb.velocity = moveInput * moveSpeed;
    }

    // void RotateToMouse()
    // {
    //     Vector3 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
    //     Vector2 lookDir = mousePos - transform.position;
    //     float angle = Mathf.Atan2(lookDir.y, lookDir.x) * Mathf.Rad2Deg;
    //     transform.rotation = Quaternion.Euler(0f, 0f, angle - 90f);
    // }

    void HandleShooting()
    {
        if (Input.GetMouseButton(0) && Time.time >= nextFireTime)
        {
            Shoot();
            nextFireTime = Time.time + 1f / weaponData.fireRate;
        }
    }

    void Shoot()
    {
        GameObject bulletGO = Instantiate(weaponData.bulletPrefab, firePoint.position, Quaternion.identity);
        Vector2 shootDir = (Camera.main.ScreenToWorldPoint(Input.mousePosition) - firePoint.position).normalized;

        BulletController bullet = bulletGO.GetComponent<BulletController>();
        if (bullet != null)
        {
            bullet.SetDirection(shootDir);
            bullet.damage = weaponData.damage;
        }

        if (weaponData.shootSFX)
            AudioSource.PlayClipAtPoint(weaponData.shootSFX, transform.position);
    }
}
