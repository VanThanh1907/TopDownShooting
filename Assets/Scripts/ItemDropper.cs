using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemDropper : MonoBehaviour
{
    [Tooltip("Prefab item sẽ rơi")]
    public GameObject[] dropItems;

    [Tooltip("Tỉ lệ rơi (0 = không bao giờ, 1 = luôn luôn)")]
    [Range(0f, 1f)]
    public float dropChance = 0.2f;

    public void TryDropItem()
    {
        if (dropItems.Length == 0) return;

        if (Random.value <= dropChance)
        {
            int randIndex = Random.Range(0, dropItems.Length);
            Instantiate(dropItems[randIndex], transform.position, Quaternion.identity);
        }
    }
}
