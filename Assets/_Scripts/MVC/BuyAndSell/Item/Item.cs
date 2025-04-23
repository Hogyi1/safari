using UnityEngine;


[CreateAssetMenu(fileName = "NewItem", menuName = "Inventory/Item")]
public class Item : ScriptableObject
{
    [SerializeField] private int id;
    [SerializeField] private string itemName;
    [SerializeField] private int price;
    [SerializeField] private Category category;
    [SerializeField] private LockState state;
    [SerializeField] private int count;
    [SerializeField] private Sprite imagePath;

    public int Id => id;
    public string ItemName => name;
    public int Price => price;
    public Category Category => category;
    public int Count => count;
    public Sprite ImagePath => imagePath;

    public int CalculateSellingPrice() {
        return (int)Mathf.Round(this.price * 0.5f);
    }

    public void IncreaseCount(int amount) {
        this.count += amount;
    }

    public void DecreaseCount(int amount)
    {
        count = Mathf.Max(0, count - amount);
    }


}
