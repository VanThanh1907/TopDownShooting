using UnityEngine;

public class EnemyAvoidance : MonoBehaviour
{
    public float separationRadius = 1f;
    public float separationForce = 3f;


    void Update()
    {
        Collider2D[] others = Physics2D.OverlapCircleAll(transform.position, separationRadius);
        Vector2 push = Vector2.zero;

        foreach (var col in others)
        {
            if (col.gameObject == gameObject) continue; // skip self
            if (col.CompareTag("Enemy") || col.CompareTag("Boss"))
            {
                Vector2 diff = (Vector2)(transform.position - col.transform.position);
                float dist = diff.magnitude;
                if (dist > 0 && dist < separationRadius)
                {
                    push += diff.normalized * (1f - dist / separationRadius); // đẩy nhẹ ra
                }
            }
        }

        // Apply lực đẩy ra
        transform.position += (Vector3)(push * separationForce * Time.deltaTime);
        
    }
}
