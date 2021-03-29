using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PointAndShoot : MonoBehaviour
{
    private Vector3 target;
    public GameObject crosshairs;
    public GameObject gun;

    void Start()
    {
        Cursor.visible = false; 
        Cursor.visible = false;
        if (Cursor.visible == false)
            Debug.Log("wank");
    }

    void Update()
    {
        target = transform.GetComponent<Camera>().ScreenToWorldPoint(new Vector3(Input.mousePosition.x, Input.mousePosition.y, transform.position.z));
        crosshairs.transform.position = new Vector2(target.x, target.y);

        Vector3 difference = target - gun.transform.position;
        float rotationz = Mathf.Atan2(difference.y, difference.x) * Mathf.Rad2Deg;
        gun.transform.rotation = Quaternion.Euler(0.0f, 0.0f, rotationz);
    }
}
