using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class ItemDrop
{
    public GameObject prefab;
    [Range(0f, 1f)]
    public float chance;
}


public class ItemDropper : MonoBehaviour
{
    [Tooltip("Danh sách item kèm tỉ lệ rơi")]
    public List<ItemDrop> dropItems;

    public void TryDropItem()
    {
        foreach (var item in dropItems)
        {
            if (item.prefab != null && Random.value <= item.chance)
            {
                Instantiate(item.prefab, transform.position, Quaternion.identity);
                // break;     // nếu chỉ muốn rơi 1 item
            }
        }
    }
}
