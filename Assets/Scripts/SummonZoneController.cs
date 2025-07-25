using System.Collections;
using UnityEngine;

public class SummonZoneController : MonoBehaviour
{
    private int minionCount;
    private float duration;
    private float radius;
    private GameObject minionPrefab;

    public void Setup(GameObject minionPrefab, int minionCount, float duration, float radius)
    {
        this.minionPrefab = minionPrefab;
        this.minionCount = minionCount;
        this.duration = duration;
        this.radius = radius;

        // Phát particle system của chính summonZonePrefab (nếu có)
        ParticleSystem ps = GetComponent<ParticleSystem>();
        if (ps != null)
        {
            ParticleSystem.MainModule main = ps.main;
            if (!main.loop) ps.Play();
        }

        // Bắt đầu quá trình triệu hồi
        StartCoroutine(SummonMinions());
    }

    private IEnumerator SummonMinions()
    {
        // Chờ đến khi hết thời gian hiệu ứng summon
        yield return new WaitForSeconds(duration);

        // Sinh quái con tại các vị trí ngẫu nhiên trong bán kính
        for (int i = 0; i < minionCount; i++)
        {
            Vector2 randomOffset = Random.insideUnitCircle * radius;
            Vector3 spawnPosition = transform.position + new Vector3(randomOffset.x, randomOffset.y, 0);

            GameObject minion = MyPoolManager.Instance.Get(minionPrefab, spawnPosition);
            if (minion != null)
            {
                Health minionHealth = minion.GetComponent<Health>();
                
                if (minionHealth != null)
                {
                    minionHealth.ResetState(minionHealth.maxHP);

                }
                minion.SetActive(true);
                Debug.Log($"Summoned minion {i + 1} at {spawnPosition}");
            }
        }
        ParticleSystem ps = GetComponent<ParticleSystem>();
        if (ps != null)
        {
            ps.Stop(true, ParticleSystemStopBehavior.StopEmittingAndClear);
        }
        gameObject.SetActive(false);
       
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.magenta;
        Gizmos.DrawWireSphere(transform.position, radius);
    }
}