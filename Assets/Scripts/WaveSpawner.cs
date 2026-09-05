using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class WaveSpawner : MonoBehaviour
{
    // =========================================
    // THÔNG TIN QUÁI TRONG WAVE
    // =========================================
    [Serializable]
    public class EnemySpawnInfo
    {
        public GameObject enemyPrefab;

        [Min(1)]
        public int amount = 1;
    }

    // =========================================
    // THÔNG TIN MỘT WAVE
    // =========================================
    [Serializable]
    public class Wave
    {
        public string waveName;

        // Có phải Boss Wave không
        public bool isBossWave = false;

        public List<EnemySpawnInfo> enemies = new List<EnemySpawnInfo>();
    }

    // =========================================
    // WAVES
    // =========================================
    [Header("Waves")]
    public List<Wave> waves = new List<Wave>();


    // =========================================
    // SPAWN POINT
    // =========================================
    [Header("Spawn Points")]
    public Transform[] spawnPoints;


    // =========================================
    // TIME
    // =========================================
    [Header("Time")]
    public float countdownTime = 5f;

    // Khoảng cách giữa mỗi enemy spawn
    public float spawnInterval = 1f;

    // Sau khi giết hết quái, nghỉ một chút
    public float delayAfterWave = 2f;


    // =========================================
    // UI LEGACY
    // =========================================
    [Header("UI")]

    // Wave 1 / 10
    public Text waveText;

    // Enemies: 10
    public Text enemyLeftText;

    // Wave starts in: 5
    public Text countdownText;

    // BOSS WAVE!
    public Text bossWaveText;

    // Panel Win
    public GameObject victoryPanel;


    // =========================================
    // RUNTIME
    // =========================================
    private int currentWave = 0;
    private int aliveEnemy = 0;

    private bool gameWon = false;


    // =========================================
    // START
    // =========================================
    private void Start()
    {
        // Đảm bảo game chạy bình thường khi load scene
        Time.timeScale = 1f;

        if (victoryPanel != null)
        {
            victoryPanel.SetActive(false);
        }

        if (bossWaveText != null)
        {
            bossWaveText.gameObject.SetActive(false);
        }

        if (countdownText != null)
        {
            countdownText.gameObject.SetActive(false);
        }

        UpdateWaveUI();
        UpdateEnemyUI();

        StartCoroutine(StartWaves());
    }


    // =========================================
    // HỆ THỐNG WAVE
    // =========================================
    IEnumerator StartWaves()
    {
        for (currentWave = 0; currentWave < waves.Count; currentWave++)
        {
            Wave wave = waves[currentWave];

            UpdateWaveUI();

            // Countdown trước Wave
            yield return StartCoroutine(CountdownBeforeWave());


            // Boss Wave
            if (wave.isBossWave)
            {
                yield return StartCoroutine(ShowBossWarning());
            }


            // Spawn Wave
            yield return StartCoroutine(SpawnWave(wave));


            // Chờ quái chết hết
            yield return new WaitUntil(() => aliveEnemy <= 0);


            Debug.Log(
                "Wave " + (currentWave + 1) + " Complete"
            );


            // Chờ trước Wave tiếp theo
            yield return new WaitForSeconds(delayAfterWave);
        }


        // Hết tất cả Wave
        WinGame();
    }


    // =========================================
    // COUNTDOWN
    // =========================================
    IEnumerator CountdownBeforeWave()
    {
        if (countdownText != null)
        {
            countdownText.gameObject.SetActive(true);
        }


        float timer = countdownTime;


        while (timer > 0)
        {
            if (countdownText != null)
            {
                countdownText.text =
                    "Wave starts in: "
                    + Mathf.CeilToInt(timer);
            }


            yield return new WaitForSeconds(1f);

            timer--;
        }


        if (countdownText != null)
        {
            countdownText.text = "GO!";
        }


        yield return new WaitForSeconds(1f);


        if (countdownText != null)
        {
            countdownText.gameObject.SetActive(false);
        }
    }


    // =========================================
    // BOSS WARNING
    // =========================================
    IEnumerator ShowBossWarning()
    {
        if (bossWaveText == null)
        {
            yield break;
        }


        bossWaveText.gameObject.SetActive(true);

        bossWaveText.text = "BOSS WAVE!";


        yield return new WaitForSeconds(2f);


        bossWaveText.gameObject.SetActive(false);
    }


    // =========================================
    // SPAWN WAVE
    // =========================================
    IEnumerator SpawnWave(Wave wave)
    {
        Debug.Log(
            "Start Wave: " + (currentWave + 1)
        );


        foreach (EnemySpawnInfo enemyInfo in wave.enemies)
        {
            for (int i = 0; i < enemyInfo.amount; i++)
            {
                SpawnEnemy(enemyInfo.enemyPrefab);


                yield return new WaitForSeconds(
                    spawnInterval
                );
            }
        }
    }


    // =========================================
    // SPAWN ENEMY
    // =========================================
    void SpawnEnemy(GameObject enemyPrefab)
    {
        if (enemyPrefab == null)
        {
            return;
        }


        if (spawnPoints == null || spawnPoints.Length == 0)
        {
            Debug.LogWarning("Không có Spawn Point!");
            return;
        }


        // Random Spawn Point
        int randomPoint = UnityEngine.Random.Range(
            0,
            spawnPoints.Length
        );


        GameObject enemy = Instantiate(
            enemyPrefab,
            spawnPoints[randomPoint].position,
            Quaternion.identity
        );


        // Tăng số quái đang sống
        aliveEnemy++;


        UpdateEnemyUI();


        // Gắn EnemyWaveMember
        EnemyWaveMember member =
            enemy.GetComponent<EnemyWaveMember>();


        if (member == null)
        {
            member =
                enemy.AddComponent<EnemyWaveMember>();
        }


        member.Init(this);
    }


    // =========================================
    // ENEMY CHẾT
    // =========================================
    public void EnemyDie()
    {
        aliveEnemy--;


        if (aliveEnemy < 0)
        {
            aliveEnemy = 0;
        }


        UpdateEnemyUI();
    }


    // =========================================
    // UPDATE WAVE UI
    // =========================================
    void UpdateWaveUI()
    {
        if (waveText == null)
        {
            return;
        }


        if (waves.Count == 0)
        {
            waveText.text = "Wave 0 / 0";

            return;
        }


        waveText.text =
            "Wave "
            + (currentWave + 1)
            + " / "
            + waves.Count;
    }


    // =========================================
    // UPDATE ENEMY UI
    // =========================================
    void UpdateEnemyUI()
    {
        if (enemyLeftText == null)
        {
            return;
        }


        enemyLeftText.text =
            "Enemies: " + aliveEnemy;
    }


    // =========================================
    // WIN GAME
    // =========================================
    void WinGame()
    {
        if (gameWon)
        {
            return;
        }


        gameWon = true;


        Debug.Log("YOU WIN!");


        if (waveText != null)
        {
            waveText.text = "ALL WAVES COMPLETE";
        }


        if (enemyLeftText != null)
        {
            enemyLeftText.text = "Enemies: 0";
        }


        if (victoryPanel != null)
        {
            victoryPanel.SetActive(true);
        }


        // Dừng game khi thắng
        Time.timeScale = 0f;
    }


    // =========================================
    // RESTART
    // Chơi lại màn hiện tại
    // =========================================
    public void Restart()
    {
        Time.timeScale = 1f;

        SceneManager.LoadScene(
            SceneManager.GetActiveScene().name
        );
    }


    // =========================================
    // MAIN MENU
    // Load Scene MainMenu
    // =========================================
    public void MainMenu()
    {
        Time.timeScale = 1f;

        SceneManager.LoadScene("MainMenu");
    }


    // =========================================
    // NEXT LEVEL
    // Load Scene man2
    // =========================================
    public void NextLevel()
    {
        Time.timeScale = 1f;

        SceneManager.LoadScene("man2");
    }
}