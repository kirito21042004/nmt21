using System;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenu : MonoBehaviour
{   bool isShowStory = false;
    public GameObject story;
    
    private AudioSource audioSource;
    private void Start()
    {
        audioSource = GetComponent<AudioSource>();
    }

    public void Play()
    {
        audioSource.Play();
        Debug.Log("Play");
        // truyen vao ten cua Scene
        SceneManager.LoadScene("SampleScene");
    }
    public void Story()
    {   
        audioSource.Play();
        // hien thi story
        Debug.Log("Story");
        if (story.activeSelf){
            story.SetActive(false);
        }else{
            story.SetActive(true);
        }
        
        
    }
    public void CloseStory()
    {
        audioSource.Play();
        Debug.Log("Close Story");

        // Tắt bảng Story
        story.SetActive(false);
    }
    public void Setting()
    {
        audioSource.Play();
        // chuyeen sang scene Setting
        SceneManager.LoadScene("SettingScence");
        Debug.Log("Setting");
    }
    public void Exit()
    {
        audioSource.Play();
        Debug.Log("Exit");
        Application.Quit();
    }
}