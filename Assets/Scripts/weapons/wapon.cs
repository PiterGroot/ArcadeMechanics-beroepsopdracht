using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class wapon : MonoBehaviour
{
    public Transform firepoint;
    private AudioSource shot;
    public ParticleSystem smoke;
    Animator anim;
    public GameObject player;

    private int damage = 0;
    public int mindamage = 10;
    public int maxdamage = 15;

    Inventory ammo;

    public float firecap = 1;
    bool AllowFire = true;

    public Collider2D Playerhitbox;


    private void Start()
    {
        shot = GetComponent<AudioSource>();
        anim = gameObject.GetComponent<Animator>();
        ammo = player.GetComponent<Inventory>();
    }
    void Update() {
        if (Input.GetButtonDown("Fire1") && (ammo.Ammo >= 1) && (AllowFire == true))
        {
            shoot();
            shot.Play();
            smoke.Play();
            anim.Play("flintlock_fire");

        }
        if (Input.GetButtonDown("Fire1") && (ammo.Ammo <= 0))
        {
            Debug.Log("Empty!");
        }
    }

    void shoot ()
    {
        StartCoroutine(waitforsec());

        RaycastHit2D hitinfo = Physics2D.Raycast(firepoint.position, firepoint.right);

        //haalt 1 ammo weg als je schiet
        if (ammo.Ammo > 0)
        {
            ammo.Ammo -= 1;
            Debug.Log(ammo.Ammo);
        }

        if (hitinfo)
        {
            Debug.Log(hitinfo.transform.name);

            Combat enemy = hitinfo.transform.GetComponent<Combat>();
            if(enemy != null)
            {
                damage = Random.Range(mindamage, maxdamage);
                enemy.Takedamage(damage);
                Debug.Log(damage);
            }
            
        }
    }
    private IEnumerator waitforsec()
    {
        AllowFire = false;
        yield return new WaitForSeconds(firecap);
        AllowFire = true;
    }
}
