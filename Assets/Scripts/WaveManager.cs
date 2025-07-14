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

    private List<GameObject> activeEnemies = new List<GameObject>();
    private List<GameObject> activeBosses = new List<GameObject>();

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
        if (!spawning && activeEnemies.Count == 0 && activeBosses.Count == 0)
        {
            StartCoroutine(StartNextWave());
        }
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
        Debug.Log($"Spawning enemies with difficultyMultiplier: {difficultyMultiplier}");
        for (int i = 0; i < enemiesPerWave; i++)
        {
            GameObject prefab = enemyPrefabs[Random.Range(0, enemyPrefabs.Length)];
            Transform point = spawnPoints[Random.Range(0, spawnPoints.Length)];

            GameObject enemy = MyPoolManager.Instance.Get(prefab, point.position);
            var health = enemy.GetComponent<Health>();
            if (health != null)
            {
                float initialMaxHP = health.maxHP; // Lấy HP ban đầu từ prefab
                health.ResetState(initialMaxHP * difficultyMultiplier); // Reset với HP mới
                health.SetFullHP(); // Đảm bảo HP đầy
                health.onDeath.AddListener(() => activeEnemies.Remove(enemy)); // Xóa khi chết
            }
            activeEnemies.Add(enemy);
            yield return new WaitForSeconds(spawnDelay);
        }
    }

    void SpawnBoss()
    {
        int index = Random.Range(0, bossList.Length);
        BossData data = bossList[index];
        float difficultyMultiplier = 1f + Mathf.Sqrt(timeSurvived / 60f) * 0.5f;
        Debug.Log($"Spawning boss with difficultyMultiplier: {difficultyMultiplier}");

        GameObject boss = MyPoolManager.Instance.Get(data.bossPrefab, GetRandomSpawnPoint());
        var health = boss.GetComponent<Health>();
        if (health != null)
        {
            health.ResetState(health.maxHP * difficultyMultiplier); // Reset với HP mới
            health.onDeath.AddListener(() => activeBosses.Remove(boss)); // Xóa khi chết
        }
        activeBosses.Add(boss);
        boss.GetComponent<BossController>().Setup(data);
    }

    Vector3 GetRandomSpawnPoint()
    {
        return spawnPoints[Random.Range(0, spawnPoints.Length)].position;
    }
}