using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ScrollCamSize : MonoBehaviour
{
    [SerializeField]private bool invertedControl = false; 
    [SerializeField]private int MaxCamSizeMultiplier = 5;
    [SerializeField]private float DefaultCamSize = 5;
    private Camera Cam;
    private int scrollListener;

    private void Awake() {
        Cam = gameObject.GetComponent<Camera>();
        Cam.orthographicSize = DefaultCamSize;
    }
    void Update()
    {
        if(!invertedControl){
            if(Input.mouseScrollDelta.y > 0){
                scrollListener--;
                PlaySound();
                
        }
            if(Input.mouseScrollDelta.y < 0){
                scrollListener++;
                PlaySound();
            }
        }
        if(invertedControl){
            if(Input.mouseScrollDelta.y > 0){
                scrollListener++;
                PlaySound();
        }
            if(Input.mouseScrollDelta.y < 0){
                scrollListener--;
                PlaySound();
            }
        }
        if(scrollListener < 0){
            scrollListener = 0;
        }
        if(scrollListener > MaxCamSizeMultiplier){
            scrollListener = MaxCamSizeMultiplier;
        }
        Cam.orthographicSize = scrollListener + DefaultCamSize;
    }
    void PlaySound(){
        FindObjectOfType<AudioManager>().Play("Scroll");
    }
}
