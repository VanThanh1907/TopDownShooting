using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

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
    public GameObject[] enemyPrefabs;
    public float spawnRadius = 15f;
    public float spawnDelay = 0.5f;
    public int bossEveryXWave = 10;
    public TMP_Text waveText;
    public BossData[] bossList;
    public float bossWarningDuration = 2.5f;

    public int currentWave = 0;
    private bool spawning = false;
    private float timeSurvived = 0f;
    public TMP_Text timerText;
    private Camera mainCamera;
    private Dictionary<string, GameObject> prefabLookup;

    private List<GameObject> activeEnemies = new List<GameObject>();
    private List<GameObject> activeBosses = new List<GameObject>();
    private float waveTimeout = 90f; // 1 phút 30 giây
    private float waveStartTime;

    void Start()
    {
        LoadWaveConfigs();
        mainCamera = Camera.main;
        prefabLookup = new Dictionary<string, GameObject>();
        foreach (BossData boss in bossList)
        {
            if (boss.bossPrefab != null)
                prefabLookup[boss.bossPrefab.name] = boss.bossPrefab;
        }
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

        // Kiểm tra timeout để chuyển wave
        if (!spawning && Time.time - waveStartTime >= waveTimeout && (activeEnemies.Count > 0 || activeBosses.Count > 0))
        {
            StartCoroutine(StartNextWave());
        }
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
        waveStartTime = Time.time; 
    }

    IEnumerator ShowBossWarning()
    {
        waveText.text = "Boss Coming!";
        yield return new WaitForSeconds(bossWarningDuration);
        waveText.text = $"Wave {currentWave}";
    }

    IEnumerator SpawnEnemies(int waveIndex)
    {
        float hpMultiplier = 1f + Mathf.Floor((currentWave - 1) / 10f); // Tăng 100% mỗi 10 wave
        Debug.Log($"Spawning enemies with hpMultiplier: {hpMultiplier}");

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
                        float initialMaxHP = health.maxHP;
                        float newHP = initialMaxHP * hpMultiplier;
                        health.ResetState(newHP);
                        health.SetFullHP();
                        health.onDeath.AddListener(() => activeEnemies.Remove(enemy));
                        Debug.LogWarning($"Spawned {config.enemyPrefabName} - Initial MaxHP: {initialMaxHP}, New HP: {newHP} (Multiplier: {hpMultiplier})");
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

    IEnumerator SpawnWaveWithBoss(int waveIndex)
    {
        yield return StartCoroutine(SpawnEnemies(waveIndex));
        
        int index = Random.Range(0, bossList.Length);
        BossData data = bossList[index];
        float hpMultiplier = 1f + Mathf.Floor((currentWave - 1) / 10f); // Tăng 100% mỗi 10 wave

        GameObject boss = MyPoolManager.Instance.Get(data.bossPrefab, GetRandomSpawnPointOutsideScreen());
        var health = boss.GetComponent<Health>();
        if (health != null)
        {
            float initialMaxHP = health.maxHP;
            float newHP = initialMaxHP * hpMultiplier;
            health.ResetState(newHP);
            health.onDeath.AddListener(() => activeBosses.Remove(boss));
            Debug.LogWarning($"Spawned boss {data.bossPrefab.name} - Initial MaxHP: {initialMaxHP}, New HP: {newHP} (Multiplier: {hpMultiplier})");
        }
        activeBosses.Add(boss);
        boss.GetComponent<BossController>().Setup(data);
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