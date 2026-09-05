using UnityEngine;
using UnityEngine.UI;
using DefaultNamespace;

public class FarmHealth : MonoBehaviour
{
    [Header("Farm HP")]
    public int maxHp = 500;

    private int currentHp;
    private bool isDestroyed = false;


    // =========================================
    // TARGET POINTS
    // =========================================
    [Header("Enemy Target Points")]
    public Transform[] targetPoints;


    // =========================================
    // UI LEGACY
    // =========================================
    [Header("UI")]
    public Slider healthSlider;
    public Text healthText;


    // =========================================
    // START
    // =========================================
    private void Start()
    {
        currentHp = maxHp;


        if (healthSlider != null)
        {
            healthSlider.minValue = 0;
            healthSlider.maxValue = maxHp;
            healthSlider.value = currentHp;
        }


        UpdateUI();
    }


    // =========================================
    // NHẬN DAMAGE
    // =========================================
    public void TakeDamage(int damage)
    {
        if (isDestroyed)
            return;


        currentHp -= damage;


        currentHp = Mathf.Clamp(
            currentHp,
            0,
            maxHp
        );


        Debug.Log(
            "Farm nhận " + damage +
            " damage | HP: " +
            currentHp + " / " + maxHp
        );


        UpdateUI();


        // =====================================
        // FARM HP = 0
        // =====================================
        if (currentHp <= 0)
        {
            DestroyFarm();
        }
    }


    // =========================================
    // UPDATE UI
    // =========================================
    private void UpdateUI()
    {
        if (healthSlider != null)
        {
            healthSlider.value = currentHp;
        }


        if (healthText != null)
        {
            healthText.text =
                "Farm HP: "
                + currentHp
                + " / "
                + maxHp;
        }
    }


    // =========================================
    // LẤY TARGET POINT GẦN ENEMY NHẤT
    // =========================================
    public Transform GetClosestTargetPoint(Vector3 enemyPosition)
    {
        // Nếu không có TargetPoint
        // Enemy sẽ chạy tới chính Farm
        if (targetPoints == null ||
            targetPoints.Length == 0)
        {
            return transform;
        }


        Transform closestPoint = null;

        float closestDistance = Mathf.Infinity;


        foreach (Transform point in targetPoints)
        {
            if (point == null)
                continue;


            float distance =
                Vector2.Distance(
                    enemyPosition,
                    point.position
                );


            if (distance < closestDistance)
            {
                closestDistance = distance;

                closestPoint = point;
            }
        }


        if (closestPoint == null)
        {
            return transform;
        }


        return closestPoint;
    }


    // =========================================
    // FARM BỊ PHÁ HỦY
    // =========================================
    private void DestroyFarm()
    {
        if (isDestroyed)
            return;


        isDestroyed = true;


        Debug.Log("FARM DESTROYED!");


        // =====================================
        // MISSION FAILED
        // =====================================
        if (GameManager.Instance != null)
        {
            GameManager.Instance.MissionFailed();
        }
        else
        {
            Debug.LogWarning(
                "Không tìm thấy GameManager!"
            );
        }
    }


    // =========================================
    // KIỂM TRA FARM ĐÃ CHẾT CHƯA
    // =========================================
    public bool IsDestroyed()
    {
        return isDestroyed;
    }
}