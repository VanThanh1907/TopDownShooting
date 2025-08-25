using UnityEngine;
using UnityEngine.UI;
using System;
using System.Collections;

public class EffectTimerUI : MonoBehaviour
{
    [SerializeField] private Image effectImage;
    public Action onEffectEnd;

    public void Show(float duration, Sprite icon)
    {
        effectImage.sprite = icon;
        effectImage.fillAmount = 1f;
        effectImage.enabled = true;
        StopAllCoroutines();
        StartCoroutine(FillDown(duration));
    }

    private IEnumerator FillDown(float duration)
    {
        float time = 0f;
        while (time < duration)
        {
            effectImage.fillAmount = 1f - (time / duration);
            time += Time.deltaTime;
            yield return null;
        }
        effectImage.fillAmount = 0f;
        effectImage.enabled = false;
        onEffectEnd?.Invoke();
    }
}