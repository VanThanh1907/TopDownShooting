using UnityEngine;
using System.Collections;

public class ClickFirePointEffect : MonoBehaviour
{
    public AudioClip audioClip;
    public GameObject effectPrefab;
    public Transform firePoint;

    private AudioSource audioSource;

    void Start()
    {
        audioSource = gameObject.AddComponent<AudioSource>();
    }

    void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            // Phát âm thanh
            if (audioClip != null)
                audioSource.PlayOneShot(audioClip);

            // Tạo hiệu ứng tại firePoint
            if (effectPrefab != null && firePoint != null)
            {
                GameObject eff = MyPoolManager.Instance.Get(effectPrefab, firePoint.position);
                if (eff == null)
                {
                    Debug.LogWarning("aaa");
                }
                StartCoroutine(DisableAfterSeconds(eff, 1f));
            }
        }
    }
      private IEnumerator DisableAfterSeconds(GameObject obj, float seconds)
    {
        yield return new WaitForSeconds(seconds);
        obj.SetActive(false);
    }
}
