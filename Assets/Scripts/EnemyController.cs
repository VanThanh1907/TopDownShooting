using UnityEngine;

public class EnemyController : MonoBehaviour
{
    public float moveSpeed = 2f;
    protected Transform player;
    protected Health health;
    public bool isFlipped { get; private set; }

    protected virtual void Start()
    {
        if (player != null)
        {
            player = GameObject.FindGameObjectWithTag("Player").transform;
        }
        health = GetComponent<Health>();

    }

    protected virtual void Update()
    {
        Move();
    }

    protected virtual void Move()
    {
        if (health != null && health.IsDead()) return;
        if (player == null) return;
        
        // Kiểm tra nếu là ShootingEnemy và đang trong tầm
        ShootingEnemy shootingEnemy = GetComponent<ShootingEnemy>();
        if (shootingEnemy != null && shootingEnemy.isInRange)
        {
            return; // Không di chuyển nếu trong tầm
        }

        Vector2 dir = (player.position - transform.position).normalized;
        transform.position += (Vector3)(dir * moveSpeed * Time.deltaTime);
        // Flip scale + cập nhật biến trạng thái
        if (dir.x != 0)
        {
            isFlipped = dir.x < 0;

            Vector3 scale = transform.localScale;
            scale.x = Mathf.Abs(scale.x) * (isFlipped ? -1 : 1);
            transform.localScale = scale;
        }
    }

}
