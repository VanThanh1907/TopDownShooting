using UnityEngine;

public class FireZoneController : MonoBehaviour
{
    private float damage;
    private float duration;
    private float radius;
    private float timeElapsed;
    private float damageTimer; // Biến đếm thời gian để gây sát thương định kỳ
    private float damageInterval = 0.5f; // Khoảng thời gian giữa các lần gây sát thương (0.5 giây)

    public void Setup(float damage, float duration, float radius)
    {
        this.damage = damage;
        this.duration = duration;
        this.radius = radius;
        timeElapsed = 0f;
        damageTimer = 0f; // Đặt lại timer sát thương

        // Điều chỉnh kích thước vòng lửa dựa trên radius
        transform.localScale = new Vector3(radius * 2, radius * 2, 1f); // Giả sử vòng lửa là một sprite hình tròn
        Debug.LogWarning($"FireZoneController Setup: damage={damage}, duration={duration}, radius={radius}");
    }

    void Update()
    {
        // Đếm thời gian tồn tại
        timeElapsed += Time.deltaTime;
        if (timeElapsed >= duration)
        {
            gameObject.SetActive(false); // Trả vòng lửa về pool khi hết thời gian
        }
        // Cập nhật timer sát thương
        damageTimer += Time.deltaTime;
    }

    private void OnTriggerStay2D(Collider2D other)
    {
        // Kiểm tra nếu người chơi chạm vào vòng lửa
        if (other.CompareTag("Player"))
        {
            Health playerHealth = other.GetComponent<Health>();
           if (playerHealth != null && damageTimer >= damageInterval)
            {
                playerHealth.TakeDamage(damage); // Gây sát thương liên tục theo thời gian
                damageTimer = 0f;
                Debug.LogWarning($"Player takes {damage} damage from FireZone");
            }
        }
    }

    private void OnDrawGizmos()
    {
        // Vẽ vòng lửa trong editor để dễ hình dung
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, radius);
    }
}