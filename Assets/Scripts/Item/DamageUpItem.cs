using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DamageUpItem : ItemBase
{
    public float bonusDamage = 10f;
    public GameObject auraEffectPrefab;
    public float duration = 5f;
    public Vector3 vt = new Vector3(0, 1, 0);

    public override void Apply(PlayerController player)
    {
        player.weaponData.damage += bonusDamage;
        Debug.Log("Tăng damage lên: " + player.weaponData.damage);

        if (auraEffectPrefab != null)
        {
            GameObject aura = GameObject.Instantiate(auraEffectPrefab, player.transform.position, Quaternion.identity, player.transform);
            aura.transform.localPosition = vt;

            // Auto hủy sau thời gian
            GameObject.Destroy(aura, duration);
            Debug.Log("Player nhận hiệu ứng phát sáng!");
        }

    }
}
