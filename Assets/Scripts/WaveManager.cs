using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
public class WaveManager : MonoBehaviour
{
    public GameObject[] enemyPrefabs;
    public Transform[] spawnPoints;
    public BossData[] bossList;

    public int enemiesPerWave = 5;
    public float spawnDelay = 0.5f;
    public int bossEveryXWave = 5;
    public TMP_Text waveText;
    private int currentWave = 0;
    private bool spawning = false;
    private float timeSurvived = 0f;
    public TMP_Text timerText;


    void Start()
    {
        StartCoroutine(StartNextWave());
    }

    private void UpdateTimerUI()
    {
        int minutes = Mathf.FloorToInt(timeSurvived / 60f);
        int seconds = Mathf.FloorToInt(timeSurvived % 60f);
        timerText.text = $"{minutes:00}:{seconds:00}";
    }

    private void Update()
    {
        if (!spawning && GameObject.FindGameObjectsWithTag("Enemy").Length == 0 && GameObject.FindGameObjectsWithTag("Boss").Length == 0)
        {
            StartCoroutine(StartNextWave());
        }
        // ⏱️ Cập nhật thời gian sống
        timeSurvived += Time.deltaTime;
        UpdateTimerUI();
    }
    IEnumerator StartNextWave()
    {

        spawning = true;
        currentWave++;
        waveText.text = $"Wave {currentWave}";

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
        float difficultyMultiplier = 1f + Mathf.Sqrt(timeSurvived / 60f) * 0.5f;
        for (int i = 0; i < enemiesPerWave; i++)
        {
            GameObject prefab = enemyPrefabs[Random.Range(0, enemyPrefabs.Length)];
            Transform point = spawnPoints[Random.Range(0, spawnPoints.Length)];

            GameObject enemy = Instantiate(prefab, point.position, Quaternion.identity);

            // ✅ Gán stats tăng theo thời gian
            var health = enemy.GetComponent<Health>();
            if (health != null)
            {
                health.maxHP *= difficultyMultiplier;
                health.SetFullHP(); // ✅ gọi hàm đặt lại HP đầy và cập nhật UI
            }
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
