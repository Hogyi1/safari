using System.Collections.Generic;
using UnityEngine;

public class ItemManager : MonoBehaviour
{

    public HashSet<Item> items;
    public static ItemManager Instance;
    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(this);
            return;
        }
        Instance = this;
        DontDestroyOnLoad(gameObject);
        LoadAllItem();
    }

    public void UnlockItem(int itemId) {

        foreach (var item in items) {

            if (item.id == itemId && item.state == LockState.LOCKED) { 
                
                item.state = LockState.UNLOCKED;
            }        
        }
    }

    public void LoadAllItem()
    {
        items = new HashSet<Item>(Resources.LoadAll<Item>("Items"));
        Debug.Log($"Betöltve {items.Count} Item.");
    }
    public HashSet<Item> getItems()
    {
        return items;
    }
}
