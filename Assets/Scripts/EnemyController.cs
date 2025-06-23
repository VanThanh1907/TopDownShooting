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
        //flip
        if (dir.x != 0)
        {
            Vector3 scale = transform.localScale;
            scale.x = Mathf.Abs(scale.x) * (dir.x > 0 ? 1 : -1);
            transform.localScale = scale;
        }
    }


}
