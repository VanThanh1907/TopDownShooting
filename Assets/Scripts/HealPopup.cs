using TMPro;
using UnityEngine;

public class HealPopup : MonoBehaviour
{
    public float moveSpeed = 1f;
    public float duration = 1f;
    public Vector3 moveDirection = Vector3.up;

    private TextMeshPro text;
    private float timer;
    private Color startColor;

    void Awake()
    {
        text = GetComponent<TextMeshPro>();
        startColor = text.color;
    }

    public void Setup(float amount)
    {
        text.text = $"+{amount:0}";
    }

    void Update()
    {
        transform.position += moveDirection * moveSpeed * Time.deltaTime;

        float fade = Mathf.Clamp01(1 - (timer / duration));
        text.color = new Color(startColor.r, startColor.g, startColor.b, fade);

        timer += Time.deltaTime;
        if (timer >= duration)
            Destroy(gameObject);
    }
}
