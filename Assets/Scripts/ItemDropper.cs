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
    
    
    public float spreadRadius = 2f; 

    public void TryDropItem()
    {
        foreach (var item in dropItems)
        {
            if (item.prefab != null && Random.value <= item.chance)
            {
                
                Vector2 randomCircle = Random.insideUnitCircle * spreadRadius;
                Vector3 spawnPosition = transform.position + new Vector3(randomCircle.x, randomCircle.y, 0);
                Instantiate(item.prefab, spawnPosition, Quaternion.identity);
                
            }
        }
    }
}