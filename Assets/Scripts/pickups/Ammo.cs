using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Ammo : MonoBehaviour
{
    Inventory AddAmmo;
    public GameObject player;
    void Start()
    {
        AddAmmo = player.GetComponent<Inventory>();
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.gameObject.CompareTag("Player") && other as BoxCollider2D != null)
        {
            AddAmmo.Ammo += 15;
            Destroy(gameObject);
        }
    }
}
