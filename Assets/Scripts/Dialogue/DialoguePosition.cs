using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DialoguePosition : MonoBehaviour
{
    public void SetPosition(Vector2 coords)
    {
        gameObject.GetComponent<RectTransform>().anchoredPosition = new Vector2(coords.x, coords.y);
    }
}
