using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum Rarity { Common, Uncommon, Rare, VeryRare, Psycho }

[System.Serializable]
public class ShopSettings 
{
   public string ProductName;
   public GameObject Product;
   public int Price;
    [Tooltip("Hoe zwaarder de weight, hoe vaker dit item zal spawnen")]
   public Rarity RarityLevel = Rarity.Uncommon;
}
