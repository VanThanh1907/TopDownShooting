using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HealthBarUI : MonoBehaviour
{
    [SerializeField] private Image fillImage;

     [SerializeField] public Vector3 offset; // nằm phía trên quái
    private Transform target;
    private System.Func<bool> getFlip;
    public void SetTarget(Transform targetTransform)
    {
        target = targetTransform;

        if (target.TryGetComponent(out EnemyController enemy))
            getFlip = () => enemy.isFlipped;
        else if (target.TryGetComponent(out BossController boss))
            getFlip = () => boss.isFlipped;
    }

    void LateUpdate()
    {
        if (target != null)
        {
            transform.position = target.position + offset;
            transform.rotation = Quaternion.identity; // luôn giữ hướng
                                                      // Nếu enemy bị lật → lật lại lần nữa
            if (getFlip != null)
            {
                bool flipped = getFlip();
                fillImage.rectTransform.localScale = new Vector3(flipped ? -1 : 1, 1, 1);
            }
        }
    }


    public void SetFill(float percent)
    {
        fillImage.fillAmount = percent;
    }
}
