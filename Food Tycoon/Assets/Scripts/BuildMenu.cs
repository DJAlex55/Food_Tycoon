using System;
using System.Collections.Generic;
using UnityEngine;

public class BuildMenu : MonoBehaviour {

    [SerializeField] private ShopItem ShopItemPrefab;
    [SerializeField] private Transform ShopItemsParent;


    private void Awake()
    {
        GenerateShopItems();
    }


    private void GenerateShopItems()
    {
        for (int i = 0; i < Enum.GetNames(typeof(GridObjectID)).Length; i++)
        {
            ShopItem item = Instantiate(ShopItemPrefab, ShopItemsParent);
            item.ID = (GridObjectID)i;
        }
    }
}
