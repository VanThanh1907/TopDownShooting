using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using System.IO;

public class WaveManager : MonoBehaviour
{
    [System.Serializable]
    public struct EnemySpawnConfig
    {
        public string enemyPrefabName;
        public int spawnCount;
    }

    [System.Serializable]
    public struct WaveConfig
    {
        public int waveNumber;
        public EnemySpawnConfig[] enemies;
    }

    [SerializeField]
    private WaveConfig[] waveConfigs;
    public GameObject[] enemyPrefabs; // Thêm mảng enemyPrefabs
    public float spawnRadius = 15f;
    public float spawnDelay = 0.5f;
    public int bossEveryXWave = 10;
    public TMP_Text waveText;
    public BossData[] bossList;
    public float bossWarningDuration = 2.5f;

    private int currentWave = 0;
    private bool spawning = false;
    private float timeSurvived = 0f;
    public TMP_Text timerText;
    private Camera mainCamera;
    private Dictionary<string, GameObject> prefabLookup;

    private List<GameObject> activeEnemies = new List<GameObject>();
    private List<GameObject> activeBosses = new List<GameObject>();

    void Start()
    {
        LoadWaveConfigs();
        mainCamera = Camera.main;
        prefabLookup = new Dictionary<string, GameObject>();
        // Thêm prefab từ bossList
        foreach (BossData boss in bossList)
        {
            if (boss.bossPrefab != null)
                prefabLookup[boss.bossPrefab.name] = boss.bossPrefab;
        }
        // Thêm prefab từ enemyPrefabs
        foreach (GameObject prefab in enemyPrefabs)
        {
            if (prefab != null)
                prefabLookup[prefab.name] = prefab;
        }
        StartCoroutine(StartNextWave());
    }

    private void LoadWaveConfigs()
    {
        TextAsset jsonText = Resources.Load<TextAsset>("waveConfig");
        if (jsonText != null)
        {
            WaveConfigWrapper wrapper = JsonUtility.FromJson<WaveConfigWrapper>(jsonText.text);
            waveConfigs = wrapper.waves;
        }
        else
        {
            Debug.LogError("Không tìm thấy file waveConfig.json trong Resources! Kiểm tra tên và đường dẫn.");
        }
    }

    [System.Serializable]
    private class WaveConfigWrapper
    {
        public WaveConfig[] waves;
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
    yield return StartCoroutine(SpawnEnemies(waveIndex));
    
    int index = Random.Range(0, bossList.Length);
    BossData data = bossList[index];
    float difficultyMultiplier = 1f + (currentWave * 0.01f);

    GameObject boss = MyPoolManager.Instance.Get(data.bossPrefab, GetRandomSpawnPointOutsideScreen());
    var health = boss.GetComponent<Health>();
    if (health != null)
    {
        float initialMaxHP = health.maxHP; // Kiểm tra maxHP ban đầu
        float currentHP = health.currentHP; // Kiểm tra HP hiện tại trước khi reset
        float newHP = initialMaxHP * difficultyMultiplier;
        health.ResetState(newHP);
        health.onDeath.AddListener(() => activeBosses.Remove(boss));
        Debug.Log($"Spawned boss {data.bossPrefab.name} - Initial MaxHP: {initialMaxHP}, Current HP before: {currentHP}, New HP: {newHP} (Multiplier: {difficultyMultiplier})");
    }
    activeBosses.Add(boss);
    boss.GetComponent<BossController>().Setup(data);
}

IEnumerator SpawnEnemies(int waveIndex)
{
    float difficultyMultiplier = 1f + (currentWave * 0.01f);
    Debug.Log($"Spawning enemies with difficultyMultiplier: {difficultyMultiplier}");

    WaveConfig wave = waveConfigs[waveIndex];
    foreach (EnemySpawnConfig config in wave.enemies)
    {
        if (prefabLookup.TryGetValue(config.enemyPrefabName, out GameObject prefab))
        {
            for (int i = 0; i < config.spawnCount; i++)
            {
                GameObject enemy = MyPoolManager.Instance.Get(prefab, GetRandomSpawnPointOutsideScreen());
                var health = enemy.GetComponent<Health>();
                if (health != null)
                {
                    float initialMaxHP = health.maxHP; // Kiểm tra maxHP ban đầu
                    float currentHP = health.currentHP; // Kiểm tra HP hiện tại trước khi reset
                    float newHP = initialMaxHP * difficultyMultiplier;
                    health.ResetState(newHP);
                    health.SetFullHP();
                    health.onDeath.AddListener(() => activeEnemies.Remove(enemy));
                    Debug.Log($"Spawned {config.enemyPrefabName} - Initial MaxHP: {initialMaxHP}, Current HP before: {currentHP}, New HP: {newHP} (Multiplier: {difficultyMultiplier})");
                }
                activeEnemies.Add(enemy);
                yield return new WaitForSeconds(spawnDelay);
            }
        }
        else
        {
            Debug.LogWarning($"Prefab {config.enemyPrefabName} không được tìm thấy trong prefabLookup! Danh sách prefab: {string.Join(", ", prefabLookup.Keys)}");
        }
    }
}
    Vector3 GetRandomSpawnPointOutsideScreen()
    {
        float camHeight = mainCamera.orthographicSize;
        float camWidth = camHeight * mainCamera.aspect;
        
        int side = Random.Range(0, 8);
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
            case 4: // Trên trái
                spawnPos = new Vector3(
                    cameraPos.x - camWidth - spawnRadius,
                    cameraPos.y + camHeight + spawnRadius,
                    0);
                break;
            case 5: // Trên phải
                spawnPos = new Vector3(
                    cameraPos.x + camWidth + spawnRadius,
                    cameraPos.y + camHeight + spawnRadius,
                    0);
                break;
            case 6: // Dưới trái
                spawnPos = new Vector3(
                    cameraPos.x - camWidth - spawnRadius,
                    cameraPos.y - camHeight - spawnRadius,
                    0);
                break;
            case 7: // Dưới phải
                spawnPos = new Vector3(
                    cameraPos.x + camWidth + spawnRadius,
                    cameraPos.y - camHeight - spawnRadius,
                    0);
                break;
        }

        return spawnPos;
    }
}