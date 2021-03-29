using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShopKeeperDialogue : MonoBehaviour
{
    public string ShopKeeperName;
    [TextArea(2, 5)]
    public string[] FailedTransactionDialogue;
    [TextArea(2, 5)]
    public string[] TransactionDialogue;
}
