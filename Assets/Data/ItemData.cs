using UnityEngine;

[CreateAssetMenu(fileName = "ItemData", menuName = "Scriptable Objects/ItemData")]
public class ItemData : ScriptableObject
{
    // dinh ngia cac muc de điền thông tin 
    [Header("Thoong tin co ban")]
    public string itemName;
    [TextArea(3, 10)]
    public string itemDescription;
    public Sprite itemSprite;
    
    [Min(0)]
    public int price;
    [Min(0)]
    public int damage;
    
    // taoj ra một bộ khung để tái sd trong tương lai 
    // ẽ súng , ak , áo , quần .......
    
    
}
