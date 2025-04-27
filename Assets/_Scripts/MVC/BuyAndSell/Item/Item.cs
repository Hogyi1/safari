using UnityEngine;


[CreateAssetMenu(fileName = "NewItem", menuName = "Inventory/Item")]
public class Item : ScriptableObject
{
    public int id;
    public int buildingId;
    public string itemName;
    public int price;
    public Category category;
    public LockState state;
    public Sprite imagePath;

    public int CalculateSellingPrice() {
        return (int)Mathf.Round(this.price * 0.5f);
    }

}
