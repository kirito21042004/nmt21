using UnityEngine;

public enum EnemyType { Normal, Boss }

[CreateAssetMenu(fileName = "EnemyData", menuName = "Scriptable Objects/EnemyData")]
public class EnemyData : ScriptableObject
{
    
    [Header("Thong tin co ban")]
    public string enemyName;
    [TextArea(3, 10)]
    public string description;

    public Sprite enemySprite;
    public RuntimeAnimatorController animatorController;

    [Header("Loai ke dich")]
    public EnemyType enemyType;   // Normal hoac Boss
    public bool canFly; // Co the bay hay khong
    

    public int hp;
    public int damage;
    public float speed;
}
