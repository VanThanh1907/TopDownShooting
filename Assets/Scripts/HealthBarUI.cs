using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HealthBarUI : MonoBehaviour
{
    public Image fillImage;
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
            transform.rotation = Quaternion.identity; // luôn thẳng
        }
    }

    public void SetFill(float percent)
    {
        fillImage.fillAmount = percent;
    }
}
