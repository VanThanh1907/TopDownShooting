using System.Collections;
using UnityEngine;

public class IceZoneController : MonoBehaviour
{
    private float duration;
    private float radius;
    private float slowAmount; // Mức độ làm chậm (0-1, 0 là đứng yên)
    private float timeElapsed;
    [SerializeField] private GameObject freezeEffectPrefab; // Hiệu ứng particle đóng băng
    private bool hasAppliedFreezeEffect = false; // biến theo dõi việc tạo hiệu ứng
    private PlayerController lastFrozenPlayer;

    // Hàm khởi tạo vùng băng với các thông số
    public void Setup(float duration, float radius, float slowAmount = 0f)
    {
        this.duration = duration;
        this.radius = radius;
        this.slowAmount = slowAmount;
        timeElapsed = 0f;
        hasAppliedFreezeEffect = false;
        lastFrozenPlayer = null;

    }

    void Update()
    {
        // Cập nhật thời gian tồn tại của vùng băng
        timeElapsed += Time.deltaTime;
        if (timeElapsed >= duration)
        {
            gameObject.SetActive(false); // Tắt vùng băng để trả về pool
        }
        if (lastFrozenPlayer != null && !lastFrozenPlayer.CanBeFrozen())
        {
            hasAppliedFreezeEffect = false; // Reset để cho phép tạo hiệu ứng mới
        }
    }

    private void OnTriggerStay2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            PlayerController playerController = other.GetComponent<PlayerController>();
            if (playerController != null && playerController.CanBeFrozen() && !hasAppliedFreezeEffect)
            {
                playerController.Freeze(5f);
                lastFrozenPlayer = playerController; // Lưu player để theo dõi

                // Kích hoạt hiệu ứng đóng băng tại vị trí player
                if (freezeEffectPrefab != null)
                {
                    GameObject particle = MyPoolManager.Instance.Get(freezeEffectPrefab, other.transform.position);
                    StartCoroutine(DisableObjectAfterDuration(particle, 5f));
                    hasAppliedFreezeEffect = true; // Đánh dấu đã tạo hiệu ứng
                }
            }
        }
    }

    // Tắt đối tượng sau một khoảng thời gian
    private IEnumerator DisableObjectAfterDuration(GameObject obj, float duration)
    {
        yield return new WaitForSeconds(duration);
        if (obj != null)
        {
            obj.SetActive(false); // Tắt để trả về pool
        }
    }

    // Vẽ gizmo để kiểm tra bán kính vùng băng trong editor
    private void OnDrawGizmos()
    {
        Gizmos.color = Color.cyan;
        Gizmos.DrawWireSphere(transform.position, radius);
    }
}