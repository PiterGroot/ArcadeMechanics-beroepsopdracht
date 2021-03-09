using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Inventory : MonoBehaviour
{
    public int AmountOfCoins = 0;
    public Transform coin;

    void Update()
    {

    }

    private void OnTriggerEnter(Collider other)
    {
        AmountOfCoins += 1;
        Debug.Log(AmountOfCoins);
    }
}
