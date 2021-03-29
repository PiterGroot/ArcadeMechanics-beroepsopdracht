using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class Shop : MonoBehaviour
{
    
    [SerializeField]List<string> ChosenRarities = new List<string>();
    [Space, SerializeField]List<ShopSettings> ListOfItemsToSpawn = new List<ShopSettings>();
    Dictionary<string, float> RarityChancesList = new Dictionary<string, float>();
    [Tooltip("index 0 = uncommon, 1 = rare, 2 = veryrare, 3 = psycho")]
    [SerializeField, Range(0, .4f)] public float[] RarityChances;
    private List<ShopSettings> Common = new List<ShopSettings>();
    private List<ShopSettings> Uncommon = new List<ShopSettings>();
    private List<ShopSettings> Rare = new List<ShopSettings>();
    private List<ShopSettings> VeryRare = new List<ShopSettings>();
    private List<ShopSettings> Psycho = new List<ShopSettings>();
    private ItemInfo Info;
   
    [Tooltip("Hoeveel items / producten de speler kan kopen")]
    [Space, SerializeField, Header("Amount of products / items")]private int ProductOfferCount = 3;
    public ShopSettings[] Products;
    public Vector2[] SpawnPositions;
    private void Awake() {

        //kansen van de zeldzaamheid toevoegen
        RarityChancesList.Add("UNCOMMON", RarityChances[0]);
        RarityChancesList.Add("RARE", RarityChances[1]);
        RarityChancesList.Add("VERYRARE", RarityChances[2]);
        RarityChancesList.Add("PSYCHO", RarityChances[3]);

        for (int i = 0; i < ProductOfferCount; i++)
        {
            ChosenRarities.Add("COMMON");
            StartCoroutine(ChooseRarities(i));
        }
        foreach (ShopSettings product in Products)
        {
          switch (product.RarityLevel){
            case Rarity.Common:
                Common.Add(product);
                break;
            case Rarity.Uncommon:
                Uncommon.Add(product);
                break;
            case Rarity.Rare:
                Rare.Add(product);
                break;
            case Rarity.VeryRare:
                VeryRare.Add(product);
                break;
            case Rarity.Psycho:
                Psycho.Add(product);
                break;
          }
        }
       StartCoroutine(ChooseProduct());
    }
    private IEnumerator ChooseRarities(int count)
    {
        //begin met alle maal commons daarna kijken of die beter kunnen worden       
        if(RandomChance(RarityChancesList["UNCOMMON"])){
            ChosenRarities[count] = "UNCOMMON";
        }
        if(RandomChance(RarityChancesList["RARE"])){
            ChosenRarities[count] = "RARE";
        }
        if(RandomChance(RarityChancesList["VERYRARE"])){
            ChosenRarities[count] = "VERYRARE";
        }
        if(RandomChance(RarityChancesList["PSYCHO"])){
            ChosenRarities[count] = "PSYCHO";
        }
        yield return new WaitForSeconds(.1f);
    }

    public IEnumerator ChooseProduct(){
        if (Common.Any()) {
            for (int i = 0; i < ChosenRarities.Count; i++)
            {
                if(ChosenRarities[i] == "COMMON"){
                    AddProductsToList(Common[RandomInt(0, Common.Count())]);
                }
            }
        }
        yield return new WaitForSeconds(.1f);
        if (Uncommon.Any()) {
            for (int i = 0; i < ChosenRarities.Count; i++)
            {
                if(ChosenRarities[i] == "UNCOMMON"){
                    AddProductsToList(Uncommon[RandomInt(0, Uncommon.Count())]);
                }
            }
        }
        yield return new WaitForSeconds(.1f);
        if (Rare.Any()) {
            for (int i = 0; i < ChosenRarities.Count; i++)
            {
                if(ChosenRarities[i] == "RARE"){
                    AddProductsToList(Rare[RandomInt(0, Rare.Count())]);
                }
            }
        }
        yield return new WaitForSeconds(.1f);
        if (VeryRare.Any()) {
            for (int i = 0; i < ChosenRarities.Count; i++)
            {
                if(ChosenRarities[i] == "VERYRARE"){
                    AddProductsToList(VeryRare[RandomInt(0, VeryRare.Count())]);       
                }
            }
        }
        yield return new WaitForSeconds(.1f);
        if (Psycho.Any()) {
            for (int i = 0; i < ChosenRarities.Count; i++)
            {
                if(ChosenRarities[i] == "PSYCHO"){
                    AddProductsToList(Psycho[RandomInt(0, Psycho.Count())]);      
                }
            }
        }
        yield return new WaitForSeconds(.1f);
        InstantiateProducts();
    }
    
    public void AddProductsToList(ShopSettings ChosenProduct){
        ListOfItemsToSpawn.Add(ChosenProduct);
    }
    
    private void InstantiateProducts()
    {
        for (int i = 0; i < ListOfItemsToSpawn.Count; i++)
        {
            GameObject Product;
            Product = Instantiate(ListOfItemsToSpawn[i].Product, SpawnPositions[i], Quaternion.identity);
            ItemInfo info = Product.gameObject.AddComponent(typeof(ItemInfo)) as ItemInfo;
            DialogueTrigger triggerDialogue = Product.gameObject.AddComponent(typeof(DialogueTrigger)) as DialogueTrigger;
            Info = Product.GetComponent<ItemInfo>();
            Product.name = ListOfItemsToSpawn[i].ProductName;
            Info.ProductName = ListOfItemsToSpawn[i].ProductName;
            Info.Price = ListOfItemsToSpawn[i].Price;
            Info.Product = ListOfItemsToSpawn[i].Product;
            
            switch(ListOfItemsToSpawn[i].RarityLevel){
                case Rarity.Common:
                    Info.RarityLevel = "COMMON";
                    break;
                case Rarity.Uncommon:
                    Info.RarityLevel = "UNCOMMON";
                    break;
                case Rarity.Rare:
                    Info.RarityLevel = "RARE";
                    break;
                case Rarity.VeryRare:
                    Info.RarityLevel = "VERYRARE";
                    break;
                case Rarity.Psycho:
                    Info.RarityLevel = "PSYCHO";
                    break;
            }
        }
    }
    
    public int RandomInt(int min, int max){
        return Random.Range(min, max);
    }
    
    public bool RandomChance(float chances){
        bool randBool = (Random.value < chances);
        return randBool;
    }
}
