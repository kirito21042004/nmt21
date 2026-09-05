using UnityEngine;
using UnityEngine.SceneManagement;

public class PauseMenu : MonoBehaviour
{
    public GameObject pausePanel;

    private AudioSource audioSource;
    private bool isPaused;

    private void Start()
    {
        audioSource = GetComponent<AudioSource>();

        if (pausePanel != null)
        {
            pausePanel.SetActive(false);
        }

        Time.timeScale = 1f;
        isPaused = false;
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            if (isPaused)
            {
                Resume();
            }
            else
            {
                Pause();
            }
        }
    }

    public void Pause()
    {
        if (pausePanel == null) return;

        pausePanel.SetActive(true);
        Time.timeScale = 0f;
        isPaused = true;
    }

    public void Resume()
    {
        PlayButtonSound();

        if (pausePanel != null)
        {
            pausePanel.SetActive(false);
        }

        Time.timeScale = 1f;
        isPaused = false;
    }

    public void Restart()
    {
        PlayButtonSound();

        Time.timeScale = 1f;

        SceneManager.LoadScene(
            SceneManager.GetActiveScene().buildIndex
        );
    }

    public void Setting()
    {
        PlayButtonSound();

        // Tránh Scene Setting bị đóng băng
        Time.timeScale = 1f;

        SceneManager.LoadScene("SettingScence");
        Debug.Log("Setting");
    }

    private void PlayButtonSound()
    {
        if (audioSource != null)
        {
            audioSource.Play();
        }
    }
}