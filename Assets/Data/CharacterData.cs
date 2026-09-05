using UnityEngine;

[CreateAssetMenu(fileName = "CharacterData", menuName = "Scriptable Objects/CharacterData")]
public class CharacterData : ScriptableObject
{
    [Header("Thoong tin co ban")] 
    public string PlayerName;
    [TextArea(3, 10)]
    public string description;
    
    public Sprite Player;
    public Animator animator;

    public int hp;
    public int maxHp;
    public int damage;
    public float speed;
}
