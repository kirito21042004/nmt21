using DefaultNamespace;
using UnityEngine;
using UnityEngine.UI;

public class HPBarController : MonoBehaviour
{
    Slider slider;

    void Awake()
    {
        slider = GetComponent<Slider>();
    }

    private void OnEnable()
    {
        if (GameManager.Instance != null)
        {
            slider.maxValue = GameManager.Instance.maxHP;
            slider.value = GameManager.Instance.currentHP;
            GameManager.Instance.OnHPChange += UpdateHP;
        }
    }

    private void OnDisable()
    {
        if (GameManager.Instance != null)
        {
            GameManager.Instance.OnHPChange -= UpdateHP;
        }
    }

    // Duoc goi tu dong khi GameManager.ChangeHp() chay
    private void UpdateHP(int currentHP)
    {
        // Set truc tiep, Clamp bao ve khoi vuot gioi han
        slider.value = Mathf.Clamp(currentHP, 0, GameManager.Instance.maxHP);
    }

    // Update is called once per frame
    void Update()
    {

    }
}
