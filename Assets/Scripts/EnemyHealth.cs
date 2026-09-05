using UnityEngine;
using UnityEngine.UI;

public class EnemyHealth : MonoBehaviour
{
    [Header("UI Slider Reference")]
    public Slider slider;
    
    [Header("Settings")]
    public float showDuration = 3f;

    private Coroutine hideCoroutine;

    private void Awake()
    {
        if (slider == null)
        {
            slider = GetComponentInChildren<Slider>();
        }
    }

    private void Start()
    {
        if (slider != null)
        {
            slider.gameObject.SetActive(false);
        }
    }

    public void Init(int maxHp)
    {
        if (slider == null)
        {
            slider = GetComponentInChildren<Slider>();
        }

        if (slider != null)
        {
            slider.maxValue = maxHp;
            slider.value = maxHp;
            slider.gameObject.SetActive(false);
        }
    }

    public void UpdateHP(int currentHp)
    {
        if (slider != null)
        {
            slider.value = Mathf.Clamp(currentHp, 0, slider.maxValue);
            
            slider.gameObject.SetActive(true);

            if (hideCoroutine != null)
            {
                StopCoroutine(hideCoroutine);
            }
            hideCoroutine = StartCoroutine(HideHPBarAfterDelay());
        }
    }

    private System.Collections.IEnumerator HideHPBarAfterDelay()
    {
        yield return new WaitForSeconds(showDuration);
        if (slider != null)
        {
            slider.gameObject.SetActive(false);
        }
    }
}
