using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class coin : MonoBehaviour
{
    Inventory addcoin;
    public GameObject player;

    private void Start()
    {
        addcoin = player.GetComponent<Inventory>();
    }
    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.gameObject.CompareTag("Player") && other as BoxCollider2D != null)
        {
            addcoin.AmountOfCoins += 1;
            Destroy(gameObject);
        }
    }

}
