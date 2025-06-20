using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BossManager : MonoBehaviour
{
    public BossData bossDataToSpawn;

    void Start()
    {
        SpawnBoss();
    }

    void SpawnBoss()
    {
        if (bossDataToSpawn != null && bossDataToSpawn.bossPrefab != null)
        {
            GameObject boss = Instantiate(bossDataToSpawn.bossPrefab, transform.position, Quaternion.identity);

            BossController controller = boss.GetComponent<BossController>();
            if (controller != null)
            {
                controller.Setup(bossDataToSpawn);
            }
        }
    }
}

