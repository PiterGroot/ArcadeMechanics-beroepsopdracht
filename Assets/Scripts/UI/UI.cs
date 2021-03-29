using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class UI : MonoBehaviour
{
    Inventory pickups;
    public GameObject player;
    public TMP_Text AmmoText;
    public TMP_Text CoinText;
    public TMP_Text HPText;

    private void Start()
    {
        pickups = player.GetComponent<Inventory>();
    }
    private void Update()
    {
        AmmoText.text = "Ammo: " + pickups.Ammo;
        CoinText.text = "Coins: " + pickups.AmountOfCoins;
        HPText.text = "HP: ";
    }
}
