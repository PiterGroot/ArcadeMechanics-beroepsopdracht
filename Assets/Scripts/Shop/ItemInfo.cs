using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemInfo : MonoBehaviour
{
    public bool Activate = false; 
    public bool TestBuy = false; 
    public string ProductName;
    public string RarityLevel;
    public int Price;
    public GameObject Product;
    private DialogueTrigger trigger;
    private ShopKeeperDialogue ShopKeeperDialogue;

    [TextArea(3, 10)]
    [HideInInspector]public string OnEnterDialogue;
    private void Start() {
        ShopKeeperDialogue = FindObjectOfType<ShopKeeperDialogue>();
        trigger = gameObject.GetComponent<DialogueTrigger>();
        OnEnterDialogue = ProductName.ToUpper();
    }
    public void ActivateDefaultDialogue(){
        trigger.dialogue.name = ShopKeeperDialogue.ShopKeeperName.ToUpper();
        trigger.dialogue.Sentences.Clear();
        trigger.dialogue.Sentences.Add(ProductName.ToUpper());
        trigger.dialogue.SoundFileName = "Land";
        trigger.dialogue.PositionsOnScreen = new Vector2(26f, 160f);
        trigger.dialogue.AutoPlay = true;
        trigger.TriggerDialogue();
    }
    public void ActivateDialogue(string sentence){
        trigger.dialogue.name = ShopKeeperDialogue.ShopKeeperName.ToUpper();
        trigger.dialogue.Sentences.Clear();
        trigger.dialogue.Sentences.Add(sentence.ToUpper());
        trigger.dialogue.SoundFileName = "Land";
        trigger.dialogue.PositionsOnScreen = new Vector2(26f, 160f);
        trigger.dialogue.AutoPlay = true;
        trigger.TriggerDialogue();
    }
    private void Update() {
       if(Activate){
           Activate = false;   
           ActivateDefaultDialogue();
       }
       if(TestBuy){
           TestBuy = false; 
           BuyProduct();
       }
    }
 
    public void BuyProduct(){
        if(FindObjectOfType<TestBuy>().money >= Price){
            FindObjectOfType<TestBuy>().money -= Price;
            //succesfull transaction
            print($"Money is now: {FindObjectOfType<TestBuy>().money}");
            print($"Player bought: {ProductName} for {Price} coins");
            ActivateDialogue(ShopKeeperDialogue.TransactionDialogue[RandomInt(0, ShopKeeperDialogue.TransactionDialogue.Length)]);
            FindObjectOfType<AudioManager>().Play("BuyItem");
            Destroy(gameObject);
        }
        else{
            //failed transaction
            print($"Player didn't have enough coins for: {ProductName} ({Price} coins)");
            ActivateDialogue(ShopKeeperDialogue.FailedTransactionDialogue[RandomInt(0, ShopKeeperDialogue.FailedTransactionDialogue.Length)]);
            FindObjectOfType<AudioManager>().Play("Error");
        }    
    }
    public int RandomInt(int min, int max){
        return UnityEngine.Random.Range(min, max);
    }
}
