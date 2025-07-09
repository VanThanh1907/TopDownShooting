using System.Collections;
using UnityEngine;

public class SpeedRateItem : ItemBase
{
    public float bonusFireRate = 1.5f; // tăng tỉ lệ bắn lên 1.5x
    public float duration = 5f;
    public GameObject fireRateEffectPrefab;
    public Vector3 offset = new Vector3(0, 1, 0);

    public override void Apply(PlayerController player)
    {
        Debug.Log("Tăng tốc độ bắn!");

        player.weaponData.fireRate *= bonusFireRate;

        if (fireRateEffectPrefab != null)
        {
            GameObject fx = GameObject.Instantiate(fireRateEffectPrefab, player.transform.position, Quaternion.identity, player.transform);
            fx.transform.localPosition = offset;
            GameObject.Destroy(fx, duration);
        }

        player.StartCoroutine(RevertFireRate(player));
    }

    private IEnumerator RevertFireRate(PlayerController player)
    {
        yield return new WaitForSeconds(duration);
        player.weaponData.fireRate /= bonusFireRate;
        Debug.Log("Hết hiệu lực tăng tốc độ bắn.");
    }
}

