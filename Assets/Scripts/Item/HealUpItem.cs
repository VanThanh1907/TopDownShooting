using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HealUpItem : ItemBase
{
    public float healAmount = 30f;
    public GameObject healEffectPrefab;
    public Vector3 offset = new Vector3(0, 1, 0);
    public GameObject healPopupPrefab;

    public override void Apply(PlayerController player)
    {
        var health = player.GetComponent<Health>();
        if (health != null && !health.IsDead())
        {
            health.Heal(healAmount);
            Debug.Log($"Hồi máu: {healAmount}");

            if (healEffectPrefab != null)
            {
                Debug.LogWarning("Spawn healEffectPrefab");
                GameObject fx = GameObject.Instantiate(healEffectPrefab, player.transform.position, Quaternion.identity, player.transform);
                fx.transform.localPosition = offset;
                GameObject.Destroy(fx, 1.5f);
            }
            if (healPopupPrefab != null)
            {
                Vector3 pos = player.transform.position + offset;
                GameObject popup = GameObject.Instantiate(healPopupPrefab, pos, Quaternion.identity);
                popup.GetComponent<HealPopup>().Setup(healAmount);
            }

        }
    }
}
