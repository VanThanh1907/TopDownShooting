using System.Collections;
using UnityEngine;

public class BulletController : MonoBehaviour
{
    public float speed = 10f;
    public float damage = 5f;
    public GameObject hitVFX;

    private Vector2 direction;
    private Rigidbody2D rb;

    void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
    }


    public void SetDirection(Vector2 dir)
    {
        direction = dir.normalized;

        float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
        transform.rotation = Quaternion.Euler(0f, 0f, angle);

        rb.velocity = direction * speed;
    }

    void Update()
    {
        // Kiểm tra nếu đạn ra khỏi màn hình
        Vector3 viewportPos = Camera.main.WorldToViewportPoint(transform.position);
        if (viewportPos.x < 0 || viewportPos.x > 1 || viewportPos.y < 0 || viewportPos.y > 1)
        {
            gameObject.SetActive(false); // Trả đạn về pool
        }
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        Health health = other.GetComponent<Health>();
        if (health != null)
        {

            health.TakeDamage(damage);
            if (hitVFX != null)
            {
                GameObject vfx = MyPoolManager.Instance.Get(hitVFX, transform.position);
                if (vfx != null)
                {
                    ParticleSystem ps = vfx.GetComponentInChildren<ParticleSystem>();
                    if (ps != null)
                    {
                        // Đặt rotation cho VFX dựa trên hướng đạn (tuỳ chọn)
                        float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
                        vfx.transform.rotation = Quaternion.Euler(0, 0, angle);

                        ps.Play();
                        // Tắt VFX sau khi phát xong
                        MyPoolManager.Instance.StartCoroutinePool(DisableObjectAfterDuration(vfx, ps.main.duration));
                    }
                }
            }
            gameObject.SetActive(false); // Trả đạn về pool
        }
    }
   private IEnumerator DisableObjectAfterDuration(GameObject obj, float duration)
    {
        if (obj != null)
        {
            yield return new WaitForSeconds(duration);
            if (obj != null)
            {
                obj.SetActive(false); // Tắt để trả về pool
            }
        }
    }
}