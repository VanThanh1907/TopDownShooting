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

    public void Teleport(Transform player, float maxDistance)
    {
        // Tạo vị trí ngẫu nhiên trong khoảng cách maxDistance quanh người chơi
        Vector2 randomOffset = Random.insideUnitCircle * maxDistance;
        Vector3 newPosition = player.position + (Vector3)randomOffset;
        transform.position = newPosition;
    }
}