using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Jump : MonoBehaviour
{
    private Rigidbody2D rb;
    public float jumprange = 8;
    private int jumps = 0;
    private bool canjump = true;

    void Start()
    {
        rb = gameObject.GetComponent<Rigidbody2D>();  
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.W) && canjump == true || Input.GetKeyDown(KeyCode.Space) && canjump == true)
        {
            jump();
            Debug.Log(jumps);
        }
        if(jumps  >= 2)
        {
            //StartCoroutine(waitforsec());
            canjump = false;

        }
    }

    private void jump()
    {
        rb.velocity = new Vector3(0, jumprange, 0);
        jumps += 1;
    }

    private void OnCollisionEnter2D(Collision2D other)
    {
        if (other.gameObject.transform.tag == "ground")
        {
            jumps = 0;
            canjump = true;
            Debug.Log(other);
        }
    }

    /*
    private IEnumerator waitforsec()
    {
        canjump = false;
        jumps = 0;
        yield return new WaitForSeconds(3);
        canjump = true;
    }
    */
}
