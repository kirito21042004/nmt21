using System;
using UnityEngine;

namespace DefaultNamespace
{
    public class PlayerHealth: MonoBehaviour

    {
        // array thanh mau 
        public Sprite[] healthSprites;
        private SpriteRenderer spriteRenderer;
        // dang ki su kien action tang giam mau tu GameManager 
        private void Awake()
        {
            spriteRenderer = GetComponent<SpriteRenderer>();
        }

        private void OnEnable()
        {
            if (GameManager.Instance != null)
            {
                GameManager.Instance.OnHPChange += UpdateHP;
                UpdateHP(GameManager.Instance.currentHP);
            }
        }

        private void OnDisable()
        {
            if (GameManager.Instance != null)
            {
                GameManager.Instance.OnHPChange -= UpdateHP;
            }
        }

        private void UpdateHP(int hp)
        {
            if (spriteRenderer == null || healthSprites == null || healthSprites.Length == 0) return;

            // cap nhat giao dien cho hp tai day 
            int index = GameManager.Instance.maxHP - hp;

            // tranh loi vuot qua array
            index = Mathf.Clamp(index, 0, healthSprites.Length - 1);

            if (healthSprites[index] != null)
            {
                spriteRenderer.sprite = healthSprites[index];
            }
        }


    }
}