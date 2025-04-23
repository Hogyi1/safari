using UnityEngine;


[CreateAssetMenu(fileName = "NewItem", menuName = "Inventory/Item")]
public class Item : ScriptableObject
{
    [SerializeField] private int Id;
    [SerializeField] private string nameIN;
    [SerializeField] private int price;
    [SerializeField] private Category category;
    [SerializeField] private LockState state;
    [SerializeField] private int count;
    [SerializeField] private Sprite imagePath;

    public string namE => nameIN;
    public int id => Id;
    public int Price => price;
    public Category Category => category;
    public int Count => count;
    public Sprite ImagePath => imagePath;

    public int calculateSellingPrice() {
        return (int)Mathf.Round(this.price * 0.5f);
    }

    public void increaseCount(int amount) {
        this.count += amount;
    }

    public void decreaseCount(int amount)
    {
        count = Mathf.Max(0, count - amount);
    }


}
