using UnityEngine;
using System.Collections.Generic;


public class ShopModell : MonoBehaviour
{

    public static ShopModell Instance;
    public List<Item> items;
    public Item currentItem;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
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
}
