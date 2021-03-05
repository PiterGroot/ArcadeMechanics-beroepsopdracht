using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FlipSprite : MonoBehaviour
{
    private SpriteRenderer Sprite;

    private void Start() {
        Sprite = gameObject.GetComponent<SpriteRenderer>();
    }
    public void FlipRight(SpriteRenderer SpriteToFlip){
        SpriteToFlip.flipX = true;
    }
      public void FlipLeft(SpriteRenderer SpriteToFlip){
        SpriteToFlip.flipX = false;
    }
}
