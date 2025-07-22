using UnityEngine;

public class BossMovement : MonoBehaviour
{
    private float moveSpeed;
    public bool isFlipped;
    private Transform Transform;
    

    public void Setup(float speed)
    {
        moveSpeed = speed;
        Transform = this.transform;
    }

    public void MoveToPlayer(Transform player)
    {
        Vector2 direction = (player.position - transform.position).normalized;
        if (direction.x != 0)
        {
            isFlipped = direction.x < 0;
            Vector3 scale = transform.localScale;
            scale.x = Mathf.Abs(scale.x) * (isFlipped ? -1 : 1);
            transform.localScale = scale;
        }
        transform.position += (Vector3)(direction * moveSpeed * Time.deltaTime);
    }

}