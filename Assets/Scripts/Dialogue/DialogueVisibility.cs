using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DialogueVisibility : MonoBehaviour
{
   public void Test()
    {
        print("INVISIBLE");
        FindObjectOfType<DialogueManager>().MakeInvisible();
    }
}
