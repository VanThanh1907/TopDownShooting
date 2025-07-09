using UnityEngine;
using System.Collections;

public class SpeedUpItem : ItemBase
{
    public float bonusSpeed = 2f;                 // Tốc độ tăng thêm
    public float duration = 5f;                   // Thời gian hiệu lực
    public GameObject speedEffectPrefab;          // Hiệu ứng chạy nhanh 
    public Vector3 offset = new Vector3(0, 1, 0);  // Vị trí hiệu ứng so với player

    public override void Apply(PlayerController player)
    {
        Debug.Log("Tăng tốc độ di chuyển!");

        // Tăng tốc độ
        player.moveSpeed += bonusSpeed;

        // Hiệu ứng
        if (speedEffectPrefab != null)
        {
            GameObject fx = GameObject.Instantiate(speedEffectPrefab, player.transform.position, Quaternion.identity, player.transform);
            fx.transform.localPosition = offset;
            GameObject.Destroy(fx, duration);
        }

        // Sau thời gian thì trả lại tốc độ cũ
        player.StartCoroutine(RevertSpeed(player));
    }

    private IEnumerator RevertSpeed(PlayerController player)
    {
        yield return new WaitForSeconds(duration);
        player.moveSpeed -= bonusSpeed;
        Debug.Log("Hết hiệu lực tăng tốc.");
    }
}
