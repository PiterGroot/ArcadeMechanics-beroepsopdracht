using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Movement : MonoBehaviour
{
    public float speed = 3;
    private FlipSpriteScale FlipSprite;

    void Start()
    {
        FlipSprite = gameObject.GetComponent<FlipSpriteScale>();
    }

    void Update()
    {
        if (Input.GetKey(KeyCode.A))
        {
            transform.position += new Vector3(-speed, 0, 0) * Time.deltaTime;
            FlipSprite.FlipRight();
        }
        if (Input.GetKey(KeyCode.D))
        {
            transform.position += new Vector3(speed, 0, 0) * Time.deltaTime;
            FlipSprite.FlipLeft();
        }
        if (Input.GetKeyUp(KeyCode.A))
        {
            transform.position += new Vector3(1, 0, 0) * Time.deltaTime;
        }
        if (Input.GetKeyUp(KeyCode.D))
        {
            transform.position += new Vector3(-1, 0, 0) * Time.deltaTime;
        }
    }
}
