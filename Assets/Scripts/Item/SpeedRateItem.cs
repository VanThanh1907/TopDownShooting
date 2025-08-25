using System.Collections;
using UnityEngine;

public class SpeedRateItem : ItemBase
{
    public float bonusFireRate = 1.5f; // tăng tỉ lệ bắn lên 1.5x
    public Sprite effectIcon;
    public float duration = 5f;
    public GameObject fireRateEffectPrefab;
    public Vector3 offset = new Vector3(0, 1, 0);

    public override void Apply(PlayerController player)
    {
        Debug.Log("Tăng tốc độ bắn!");

        if (!player.isRateUpActive)
        {
            player.runtimeWeaponData.fireRate *= bonusFireRate;
            player.isRateUpActive = true;
        }
        else
        {
            // Nếu đã có hiệu ứng, dừng coroutine cũ để reset lại thời gian
            if (player.rateUpCoroutine != null)
                player.StopCoroutine(player.rateUpCoroutine);
        }

        if (fireRateEffectPrefab != null)
        {
            GameObject fx = GameObject.Instantiate(fireRateEffectPrefab, player.transform.position, Quaternion.identity, player.transform);
            fx.transform.localPosition = offset;
            GameObject.Destroy(fx, duration);
        }
        var uiManager = FindObjectOfType<EffectTimerUIManager>();
        if (uiManager != null)
            uiManager.ShowEffect("RateUp", duration, effectIcon); 

        player.rateUpCoroutine = player.StartCoroutine(RevertFireRate(player));
    }

    private IEnumerator RevertFireRate(PlayerController player)
    {
        yield return new WaitForSeconds(duration);
        player.runtimeWeaponData.fireRate /= bonusFireRate;
        player.isRateUpActive = false;
        Debug.Log("Hết hiệu lực tăng tốc độ bắn.");
    }
}

