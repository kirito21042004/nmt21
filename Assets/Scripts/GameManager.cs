using System;
using System.IO;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace DefaultNamespace
{
    public class GameManager : MonoBehaviour
    {
        public CharacterData characterData;

        private string[] level =
            new[] { "level01", "level02", "level03" };


        public static GameManager Instance { get; private set; }


        public int score = 0;


        // =========================================
        // HP
        // =========================================

        // khai bao Action khi hp thay doi
        public Action<int> OnHPChange;


        public int maxHP = 17;

        public int currentHP = 17;


        // =========================================
        // SCORE
        // =========================================

        // khai bao Action khi diem thay doi
        public Action<int> OnScoreChange;


        public bool isGameOver = false;


        public int health { get; set; }


        // =========================================
        // MISSION FAILED
        // =========================================
        [Header("Mission Failed")]

        public GameObject missionFailedPanel;


        // =========================================
        // DIALOGUE
        // =========================================
        [Header("BỔ SUNG: Dialogue Settings")]

        public GameObject dialoguePanel;


        private static bool firstEnemyKilled = false;


        // =========================================
        // AWAKE
        // =========================================
        private void Awake()
        {
            if (Instance == null)
            {
                Instance = this;


                // Đảm bảo game chạy lại bình thường
                Time.timeScale = 1f;


                InitCharacterData();
            }
            else
            {
                Destroy(gameObject);
            }
        }


        // =========================================
        // START
        // =========================================
        private void Start()
        {
            // Tắt Mission Failed Panel khi bắt đầu
            if (missionFailedPanel != null)
            {
                missionFailedPanel.SetActive(false);
            }
        }


        // =========================================
        // DIALOGUE
        // =========================================
        public void TriggerFirstEnemyDialogue()
        {
            if (firstEnemyKilled ||
                dialoguePanel == null)
            {
                return;
            }


            firstEnemyKilled = true;


            dialoguePanel.SetActive(true);
        }


        // =========================================
        // CHARACTER DATA
        // =========================================
        private void InitCharacterData()
        {
            if (characterData != null)
            {
                maxHP = characterData.hp;

                currentHP = maxHP;
            }
            else
            {
                Debug.LogWarning(
                    "Chưa kéo CharacterData vào GameManager!"
                );
            }
        }


        // =========================================
        // ON VALIDATE
        // =========================================
        private void OnValidate()
        {
            if (characterData != null)
            {
                maxHP = characterData.hp;
            }
        }


        // =========================================
        // SAVE CHECKPOINT
        // =========================================
        public void SaveCheckpoint(
            string scencename,
            int checkPoint,
            int health,
            int score
        )
        {
            SaveData saveData =
                new SaveData();


            saveData.checkPoint =
                checkPoint;


            saveData.health =
                health;


            saveData.score =
                score;


            string json =
                JsonUtility.ToJson(
                    saveData
                );


            string path =
                Path.Combine(
                    Application.persistentDataPath,
                    "SaveData.json"
                );


            Debug.Log(path);


            File.WriteAllText(
                path,
                json
            );


            Debug.Log(
                "Saved checkpoint"
            );
        }


        // =========================================
        // CHECK SAVE
        // =========================================
        public bool IsCheckPointExist()
        {
            string path =
                Path.Combine(
                    Application.persistentDataPath,
                    "SaveData.json"
                );


            return File.Exists(path);
        }


        // =========================================
        // ADD SCORE
        // =========================================
        public void AddScore(int score)
        {
            if (isGameOver)
            {
                return;
            }


            this.score += score;


            OnScoreChange?.Invoke(
                this.score
            );
        }


        // =========================================
        // CHANGE HP
        // =========================================
        public void ChangeHp(int hp)
        {
            if (isGameOver)
            {
                return;
            }


            currentHP += hp;


            currentHP =
                Mathf.Clamp(
                    currentHP,
                    0,
                    maxHP
                );


            OnHPChange?.Invoke(
                currentHP
            );


            // =====================================
            // PLAYER HP = 0
            // =====================================
            if (currentHP <= 0)
            {
                GameOver();
            }
        }


        // =========================================
        // PLAYER GAME OVER
        // =========================================
        private void GameOver()
        {
            MissionFailed();
        }


        // =========================================
        // MISSION FAILED
        // Player chết hoặc Farm chết đều gọi hàm này
        // =========================================
        public void MissionFailed()
        {
            if (isGameOver)
            {
                return;
            }


            isGameOver = true;


            Debug.Log(
                "MISSION FAILED"
            );


            // Hiện Panel
            if (missionFailedPanel != null)
            {
                missionFailedPanel
                    .SetActive(true);
            }


            // Dừng game
            Time.timeScale = 0f;
        }


        // =========================================
        // RESTART
        // Chơi lại màn hiện tại
        // =========================================
        public void RestartGame()
        {
            Time.timeScale = 1f;


            SceneManager.LoadScene(
                SceneManager
                    .GetActiveScene()
                    .name
            );
        }


        // =========================================
        // MAIN MENU
        // =========================================
        public void MainMenu()
        {
            Time.timeScale = 1f;


            SceneManager.LoadScene(
                "MainMenu"
            );
        }


        // =========================================
        // EXIT GAME
        // =========================================
        public void ExitGame()
        {
            Time.timeScale = 1f;


#if UNITY_EDITOR

            UnityEditor
                .EditorApplication
                .isPlaying = false;

#else

            Application.Quit();

#endif
        }
    }
}