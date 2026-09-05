using DefaultNamespace;
using UnityEngine;
using UnityEngine.UI;

public class HpUpdate : MonoBehaviour
{
    Text HpText;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        HpText = GetComponent<Text>();
        GameManager.Instance.OnHPChange += UpdateHp;
    }

    public void UpdateHp(int hp)
    {
        HpText.text = "Hp: " + hp;
    }
    // Update is called once per frame
    void Update()
    {
        
    }
}