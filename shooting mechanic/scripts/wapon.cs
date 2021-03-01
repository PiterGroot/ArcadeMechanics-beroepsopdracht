using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class wapon : MonoBehaviour
{
    public Transform firepoint;
    private AudioSource shot;
    public ParticleSystem smoke;
    Animator anim;
    //als damage niet meer werkt maak dit public
    private int damage = 0;
    public int mindamage = 10;
    public int maxdamage = 15;

    private void Start()
    {
        shot = GetComponent<AudioSource>();
        anim = gameObject.GetComponent<Animator>();

    }
    void Update() {
        if (Input.GetButtonDown("Fire1"))
        {
            shoot();
            shot.Play();
            smoke.Play();
            anim.Play("flintlock_fire");

        }
    }

    void shoot ()
    {
        RaycastHit2D hitinfo = Physics2D.Raycast(firepoint.position, firepoint.right);

        if (hitinfo)
        {
            Debug.Log(hitinfo.transform.name);

            enemy enemy = hitinfo.transform.GetComponent<enemy>();
            if(enemy != null)
            {
                damage = Random.Range(mindamage, maxdamage);
                enemy.Takedamage(damage);
                Debug.Log(damage);
            }
        }
    }
}
