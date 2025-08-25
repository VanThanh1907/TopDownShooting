using UnityEngine;
using System.Collections;

public class SpeedUpItem : ItemBase
{
    public float bonusSpeed = 2f;                 // Tốc độ tăng thêm
    public float duration = 5f;                   // Thời gian hiệu lực
    public Sprite effectIcon;
    public GameObject speedEffectPrefab;          // Hiệu ứng chạy nhanh 
    public Vector3 offset = new Vector3(0, 1, 0);  // Vị trí hiệu ứng so với player

    public override void Apply(PlayerController player)
    {
        Debug.Log("Tăng tốc độ di chuyển!");

        if (!player.isSpeedUpActive)
        {
            player.moveSpeed += bonusSpeed;
            player.isSpeedUpActive = true;
        }
        else
        {
            // Nếu đã có hiệu ứng, dừng coroutine cũ để reset lại thời gian
            if (player.speedUpCoroutine != null)
                player.StopCoroutine(player.speedUpCoroutine);
        }

        // Hiệu ứng
        if (speedEffectPrefab != null)
        {
            GameObject fx = GameObject.Instantiate(speedEffectPrefab, player.transform.position, Quaternion.identity, player.transform);
            fx.transform.localPosition = offset;
            GameObject.Destroy(fx, duration);
        }
        
        var uiManager = FindObjectOfType<EffectTimerUIManager>();
        if (uiManager != null)
            uiManager.ShowEffect("SpeedUp", duration, effectIcon); // "SpeedUp" là key duy nhất cho hiệu ứng này

        player.speedUpCoroutine = player.StartCoroutine(RevertSpeed(player));
    }

    private IEnumerator RevertSpeed(PlayerController player)
    {
        yield return new WaitForSeconds(duration);
        player.moveSpeed -= bonusSpeed;
        player.isSpeedUpActive = false;
        Debug.Log("Hết hiệu lực tăng tốc.");
    }
}
