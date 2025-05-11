using System.Collections.Generic;
using UnityEngine;

public class ServiceManager : MonoBehaviour
{
    public ServiceView serviceView;
    public static ServiceManager Instance;
    public HashSet<Item> items;
    public Item currentItem;


    void Start()
    {
        items = ItemManager.Instance.getItems();
        serviceView.GenerateServiceItems(items);
    }

    public void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(this);
            return;
        }
        Instance = this;
        
    }

    public void ShowItemDetails(Item item)
    {
        serviceView.ShowItemDetails(item);
        currentItem = item;
    }

}
