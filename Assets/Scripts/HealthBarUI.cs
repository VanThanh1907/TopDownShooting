using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HealthBarUI : MonoBehaviour
{
   [SerializeField] private Image fillImage;
    public Vector3 offset = new Vector3(0, 5f, 0); // nằm phía trên quái

    private Transform target;

    public void SetTarget(Transform targetTransform)
    {
        target = targetTransform;
    }

  void LateUpdate()
{
    if (target != null)
    {
        transform.position = target.position + offset;
        transform.rotation = Quaternion.identity; // luôn giữ hướng

        // 👉 Luôn giữ scale X > 0 để không bị lật
        Vector3 scale = transform.localScale;
        scale.x = Mathf.Abs(scale.x);
        transform.localScale = scale;
    }
}


    public void SetFill(float percent)
    {
        fillImage.fillAmount = percent;
    }
}
