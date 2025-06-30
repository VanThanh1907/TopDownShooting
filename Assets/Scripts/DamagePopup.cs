using TMPro;
using UnityEngine;

public class DamagePopup : MonoBehaviour
{
    public float moveSpeed = 1f;
    public float gravity = 10f; // tốc độ rơi xuống (tăng dần)
    public float duration = 1f;

    private Vector3 moveDirection;
    private Vector3 velocity;
    private TextMeshPro text;
    private float timer;
    private Color startColor;
    private Vector3 originalScale;

    void Awake()
    {
        text = GetComponent<TextMeshPro>();
        startColor = text.color;
        originalScale = transform.localScale;
    }

    public void Setup(float damage)
    {
        if (text != null)
        {
            text.text = damage.ToString("0");
            transform.localScale = originalScale * 1.5f;
        }

        // ✅ Bắt đầu "bắn ra" ngược lên một chút, lệch trái hoặc phải
        moveDirection = new Vector3(Random.Range(-0.5f, 0.5f), 1f, 0f).normalized;
        velocity = moveDirection * moveSpeed;
    }

    void Update()
    {
        // ✅ Rơi xuống do "trọng lực"
        velocity += Vector3.down * gravity * Time.deltaTime;
        transform.position += velocity * Time.deltaTime;

        // ✅ Scale nhỏ dần
        transform.localScale = Vector3.Lerp(transform.localScale, originalScale, 5f * Time.deltaTime);

        // ✅ Fade mờ dần
        float fade = Mathf.Clamp01(1 - (timer / duration));
        text.color = new Color(startColor.r, startColor.g, startColor.b, fade);

        timer += Time.deltaTime;
        if (timer >= duration)
        {
            Destroy(gameObject);
        }
    }
}
