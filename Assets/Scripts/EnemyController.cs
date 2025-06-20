using UnityEngine;

public class EnemyController : MonoBehaviour
{
    public float moveSpeed = 2f;
    protected Transform player;
    protected Health health;

    protected virtual void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player").transform;
       
    }

    protected virtual void Update()
    {
        Move();
    }

    protected virtual void Move()
    {
        if (player == null) return;
        Vector2 dir = (player.position - transform.position).normalized;
        transform.position += (Vector3)(dir * moveSpeed * Time.deltaTime);
    }

    protected virtual void OnDeath()
    {
        // Gọi animation chết ở subclass nếu có
        Destroy(gameObject);
    }
}
