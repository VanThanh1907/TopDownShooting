using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class WaveManager : MonoBehaviour
{
    [System.Serializable]
    public struct EnemySpawnConfig
    {
        public GameObject enemyPrefab;
        public int spawnCount;
    }

    [System.Serializable]
    public struct WaveConfig
    {
        public EnemySpawnConfig[] enemies; // Danh sách enemy và số lượng cho wave này
    }

    public WaveConfig[] waveConfigs; // Cấu hình cho từng wave
    public float spawnRadius = 15f; // Khoảng cách spawn ngoài màn hình
    public float spawnDelay = 0.5f;
    public int bossEveryXWave = 10;
    public TMP_Text waveText;
    public BossData[] bossList;
    public float bossWarningDuration = 2.5f; // Thời gian hiển thị cảnh báo boss

    private int currentWave = 0;
    private bool spawning = false;
    private float timeSurvived = 0f;
    public TMP_Text timerText;
    private Camera mainCamera;

    private List<GameObject> activeEnemies = new List<GameObject>();
    private List<GameObject> activeBosses = new List<GameObject>();

    void Start()
    {
        mainCamera = Camera.main;
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

        // Nếu không có cấu hình cho wave này, dùng wave cuối cùng trong danh sách
        int waveIndex = Mathf.Min(currentWave - 1, waveConfigs.Length - 1);

        if (currentWave % bossEveryXWave == 0)
        {
            yield return StartCoroutine(ShowBossWarning());
            yield return StartCoroutine(SpawnWaveWithBoss(waveIndex));
        }
        else
        {
            yield return StartCoroutine(SpawnEnemies(waveIndex));
        }

        spawning = false;
    }

    IEnumerator ShowBossWarning()
    {
        waveText.text = "Boss Coming!";
        yield return new WaitForSeconds(bossWarningDuration);
        waveText.text = $"Wave {currentWave}";
    }

    IEnumerator SpawnWaveWithBoss(int waveIndex)
    {
        // Spawn enemies bình thường
        yield return StartCoroutine(SpawnEnemies(waveIndex));
        
        // Spawn boss
        int index = Random.Range(0, bossList.Length);
        BossData data = bossList[index];
        float difficultyMultiplier = 1f + (currentWave * 0.1f); // Tăng độ khó 10% mỗi wave

        GameObject boss = MyPoolManager.Instance.Get(data.bossPrefab, GetRandomSpawnPointOutsideScreen());
        var health = boss.GetComponent<Health>();
        if (health != null)
        {
            health.ResetState(health.maxHP * difficultyMultiplier);
            health.onDeath.AddListener(() => activeBosses.Remove(boss));
        }
        activeBosses.Add(boss);
        boss.GetComponent<BossController>().Setup(data);
    }

    IEnumerator SpawnEnemies(int waveIndex)
    {
        float difficultyMultiplier = 1f + (currentWave * 0.1f); // Tăng độ khó 10% mỗi wave
        Debug.Log($"Spawning enemies with difficultyMultiplier: {difficultyMultiplier}");

        WaveConfig wave = waveConfigs[waveIndex];
        foreach (EnemySpawnConfig config in wave.enemies)
        {
            for (int i = 0; i < config.spawnCount; i++)
            {
                GameObject enemy = MyPoolManager.Instance.Get(config.enemyPrefab, GetRandomSpawnPointOutsideScreen());
                var health = enemy.GetComponent<Health>();
                if (health != null)
                {
                    float initialMaxHP = health.maxHP;
                    health.ResetState(initialMaxHP * difficultyMultiplier);
                    health.SetFullHP();
                    health.onDeath.AddListener(() => activeEnemies.Remove(enemy));
                }
                activeEnemies.Add(enemy);
                yield return new WaitForSeconds(spawnDelay);
            }
        }
    }

    Vector3 GetRandomSpawnPointOutsideScreen()
    {
        // Lấy kích thước màn hình trong world coordinates
        float camHeight = mainCamera.orthographicSize;
        float camWidth = camHeight * mainCamera.aspect;
        
        // Random chọn một trong bốn hướng (trên, dưới, trái, phải)
        int side = Random.Range(0, 4);
        Vector3 spawnPos = Vector3.zero;
        Vector3 cameraPos = mainCamera.transform.position;

        switch (side)
        {
            case 0: // Trên
                spawnPos = new Vector3(
                    cameraPos.x + Random.Range(-camWidth, camWidth),
                    cameraPos.y + camHeight + spawnRadius,
                    0);
                break;
            case 1: // Dưới
                spawnPos = new Vector3(
                    cameraPos.x + Random.Range(-camWidth, camWidth),
                    cameraPos.y - camHeight - spawnRadius,
                    0);
                break;
            case 2: // Trái
                spawnPos = new Vector3(
                    cameraPos.x - camWidth - spawnRadius,
                    cameraPos.y + Random.Range(-camHeight, camHeight),
                    0);
                break;
            case 3: // Phải
                spawnPos = new Vector3(
                    cameraPos.x + camWidth + spawnRadius,
                    cameraPos.y + Random.Range(-camHeight, camHeight),
                    0);
                break;
        }

        return spawnPos;
    }
}