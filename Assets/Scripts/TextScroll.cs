using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class TextScroll : MonoBehaviour
{
    [Header("Typewriter Settings")]
    // Text component hien thi noi dung
    public Text displayText;
    // Toan bo noi dung muon hien thi
    [TextArea(5, 20)]
    public string fullText;
    // Toc do hien thi: so giay giua moi ky tu (0.05 = nhanh, 0.1 = cham)
    public float charDelay = 0.05f;

    [Header("Skip")]
    // Nhan Space hoac Click de hien toan bo ngay lap tuc
    public bool allowSkip = true;

    private Coroutine typingCoroutine;
    private bool isFinished = false;

    void Start()
    {
        if (displayText == null)
            displayText = GetComponent<Text>();

        // Bat dau hien tung chu
        typingCoroutine = StartCoroutine(TypewriterEffect());
    }

    void Update()
    {
        // Nhan Space hoac chuot trai de skip
        if (allowSkip && !isFinished)
        {
            if (Input.GetKeyDown(KeyCode.Space) || Input.GetMouseButtonDown(0))
            {
                SkipToEnd();
            }
        }

        // Nhan Esc sau khi doc xong de quay ve MainMenu
        if (isFinished && Input.GetKeyDown(KeyCode.Escape))
        {
            SceneManager.LoadScene("MainMenu");
        }
    }

    IEnumerator TypewriterEffect()
    {
        displayText.text = "";
        isFinished = false;

        foreach (char c in fullText)
        {
            displayText.text += c;
            yield return new WaitForSeconds(charDelay);
        }

        isFinished = true;
        OnTypingFinished();
    }

    // Hien toan bo van ban ngay lap tuc (skip)
    public void SkipToEnd()
    {
        if (typingCoroutine != null)
            StopCoroutine(typingCoroutine);

        displayText.text = fullText;
        isFinished = true;
        OnTypingFinished();
    }

    // Goi lai khi hien xong toan bo van ban
    void OnTypingFinished()
    {
        Debug.Log("Typewriter: Da hien het noi dung. Nhan Esc de quay ve MainMenu.");
    }
}