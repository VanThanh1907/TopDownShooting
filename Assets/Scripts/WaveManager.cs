using System.Collections;
using System.Collections.Generic;
using UnityEngine;
public class WaveManager : MonoBehaviour
{
    public GameObject[] enemyPrefabs;
    public Transform[] spawnPoints;
    public BossData[] bossList;

    public int enemiesPerWave = 5;
    public float spawnDelay = 0.5f;
    public int bossEveryXWave = 5;

    private int currentWave = 0;
    private bool spawning = false;

    void Start()
    {
        StartCoroutine(StartNextWave());
    }
    private void Update()
    {
        if (!spawning && GameObject.FindGameObjectsWithTag("Enemy").Length == 0)
        {
            StartCoroutine(StartNextWave());
        }
    }
    IEnumerator StartNextWave()
    {
        spawning = true;
        currentWave++;

        if (currentWave % bossEveryXWave == 0)
        {
            SpawnBoss();
        }
        else
        {
            yield return StartCoroutine(SpawnEnemies());
        }

        spawning = false;
    }

    IEnumerator SpawnEnemies()
    {
        for (int i = 0; i < enemiesPerWave; i++)
        {
            GameObject prefab = enemyPrefabs[Random.Range(0, enemyPrefabs.Length)];
            Transform point = spawnPoints[Random.Range(0, spawnPoints.Length)];

            Instantiate(prefab, point.position, Quaternion.identity);
            yield return new WaitForSeconds(spawnDelay);
        }
    }

    void SpawnBoss()
    {
        int index = Random.Range(0, bossList.Length);
        BossData data = bossList[index];

        GameObject boss = Instantiate(data.bossPrefab, GetRandomSpawnPoint(), Quaternion.identity);
        boss.GetComponent<BossController>().Setup(data);
    }

    Vector3 GetRandomSpawnPoint()
    {
        return spawnPoints[Random.Range(0, spawnPoints.Length)].position;
    }
}
