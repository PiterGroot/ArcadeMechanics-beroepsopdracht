using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Combat : MonoBehaviour
{
    #region Variables
    //private componenten
    private SpriteRenderer SpriteRenderer;
    private Animator NPCAnim;
    private Vector2 PlayerPosition;
    private Rigidbody2D rigidPlayer;
    private int deltaTimeMultiplier = 75;
    private FlipSprite flipSprite;
    [HideInInspector]public bool canAttackRight = false;
    [HideInInspector]public bool canAttackLeft = false;

    [Header("Combat")]
    [Tooltip("Is de npc momenteel in de angry (aanvallen) status.")]
    public bool isAngry = false;
    [Tooltip("Doet de npc een eigen attack? (schieten, gooien ect.) Zo ja, call je functie dan in de PerformExternalAttack()")]
    public bool ExternalAttack = false;
    [Tooltip("Nummer van de angry layer voor de npc")]
    public int AngryLayerInt = 11;
    [Tooltip("Hoe snel de npc de positie van de speler kan ondekken en verversen. In secondes")]
    [SerializeField, Range(0, .2f)]private float SearchRate = .075f;
    [Tooltip("De afstand tot wanneer de npc stopt en een aanval gaat proberen te uitvoeren.")]
    [SerializeField, Range(0, 10f)]private float AttackDistance = 1.2f;
    [Tooltip("De snelheid van de npc als hij achter de speler aan gaat")]
    [SerializeField, Range(0, 40f)]private float AngryMoveSpeed = 5f;
    [Tooltip("Min(x) max(y) values van de random damage range.")]
    public Vector2Int DamageStrenght = new Vector2Int(5, 10);
    [Tooltip("Sprite van de npc als hij in attack state is, als hij dat niet hoeft stop dan gewoon de normale sprite van de npc hierin.")]
    public Sprite AngrySprite;
    public int health = 100;
    #endregion

    #region General
    private void Start()
    {
        SpriteRenderer = gameObject.GetComponent<SpriteRenderer>();
        NPCAnim = gameObject.GetComponent<Animator>();
        flipSprite = gameObject.GetComponent<FlipSprite>();
         InvokeRepeating("PerformExternalAttack", 0f, 2f);
    }
    private IEnumerator OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player") && !isAngry)
        {
            rigidPlayer = collision.GetComponent<Rigidbody2D>();
            NPC_Movement npcObj = gameObject.GetComponent<NPC_Movement>();
            if (npcObj.NPCType == NPCType.Slime)
            {
                //combat code for slime npc
                isAngry = true;
                npcObj.canMoveRandomly = false;
                NPCAnim.SetTrigger(npcObj.AngryAnimationName);
                gameObject.layer = AngryLayerInt;
                npcObj.IsEnabled = false;
                npcObj.Activity = 0;
                yield return new WaitForSeconds(.001f);
                npcObj.Activity = 1;
                 yield return new WaitForSeconds(.001f);
                npcObj.Activity = 0;
                SpriteRenderer.sprite = AngrySprite;
                yield return new WaitForSeconds(Random.Range(.5f, 1f));
                StartCoroutine(SearchSlime());
            }
            if (npcObj.NPCType == NPCType.Character)
            {
                //combat code for character npc
                isAngry = true;
                npcObj.canMoveRandomly = false;
                NPCAnim.SetTrigger(npcObj.AngryAnimationName);
                gameObject.layer = AngryLayerInt;
                npcObj.IsEnabled = false;
                npcObj.Activity = 0;
                yield return new WaitForSeconds(.001f);
                npcObj.Activity = 1;
                 yield return new WaitForSeconds(.001f);
                npcObj.Activity = 0;
                SpriteRenderer.sprite = AngrySprite;
                yield return new WaitForSeconds(Random.Range(.5f, 1f));
                StartCoroutine(SearchCharacter());
            }
        }
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.collider.tag == "Player")
        {
            //force naar links
            if (transform.position.x >= PlayerPosition.x)
            {
                PerformAttack();
                int randLeft = gameObject.GetComponent<NPC_Movement>().RandomInt(-160, -171);
                rigidPlayer.AddForce(new Vector2(randLeft, 240f) * Time.deltaTime * deltaTimeMultiplier);
            }
            //force naar rechts
            if (transform.position.x <= PlayerPosition.x)
            {
                PerformAttack();
                int randRight = gameObject.GetComponent<NPC_Movement>().RandomInt(160, 171);
                rigidPlayer.AddForce(new Vector2(randRight, 240f) * Time.deltaTime * deltaTimeMultiplier);
            }
        }
    }
    //geeft player x en y positie terug
    public Vector2 GetPlayerPos()
    {
        Vector2 PlayerPos = GameObject.FindGameObjectWithTag("Player").GetComponent<Transform>().position;
        return PlayerPos;
    }
    //attack met een random value
    private void PerformAttack()
    {
        FindObjectOfType<AudioManager>().Play("Land");
        int randDamage = Random.Range(DamageStrenght.x, DamageStrenght.y);
        print($"Damage for: {randDamage}");
    }
    private void PerformExternalAttack()
    {
        if(canAttackLeft && canAttackRight && ExternalAttack)
        {
            //hier de shoot code 
        }
    }

    public void Takedamage(int damage)
    {
        health -= damage;

        if (health <= 0)
        {
            Die();
        }
    }

    void Die()
    {
        Destroy(gameObject);
    }

    #endregion

    #region SlimeCombat

    //zoekt speler en beweegt npc 
    private IEnumerator SearchSlime()
    {
        StartCoroutine(SlimeMoveNPC());
        PlayerPosition = GetPlayerPos();
        while (true)
        {
            yield return new WaitForSeconds(SearchRate);
            PlayerPosition = GetPlayerPos();
        }
    }
    private IEnumerator SlimeMoveNPC()
    {
        while (true)
        {
            yield return new WaitForSeconds(.005f);
            if (transform.position.x >= PlayerPosition.x - AttackDistance)
            {
                //npc moet naar links 
                flipSprite.FlipLeft(SpriteRenderer);
                transform.position = new Vector3(transform.position.x - AngryMoveSpeed * Time.deltaTime, transform.position.y);
                gameObject.GetComponent<NPC_Movement>().CreateJumpDust();
            }
            if (transform.position.x <= PlayerPosition.x + AttackDistance)
            {
                //npc moet naar rechts 
                flipSprite.FlipRight(SpriteRenderer);
                transform.position = new Vector3(transform.position.x + AngryMoveSpeed * Time.deltaTime, transform.position.y);
                gameObject.GetComponent<NPC_Movement>().CreateJumpDust();
            }
        }
    }  
    #endregion
    
    #region CharacterCombat
     private IEnumerator SearchCharacter()
    {
        StartCoroutine(CharacterMoveNPC());
        PlayerPosition = GetPlayerPos();
        while (true)
        {
            yield return new WaitForSeconds(SearchRate);
            PlayerPosition = GetPlayerPos();
        }
    }
    private IEnumerator CharacterMoveNPC()
    {
        while (true)
        {
            yield return new WaitForSeconds(.005f);
            if (transform.position.x >= PlayerPosition.x - AttackDistance)
            {
                //npc moet naar links 
                transform.position = new Vector3(transform.position.x - AngryMoveSpeed * Time.deltaTime, transform.position.y);
                gameObject.GetComponent<NPC_Movement>().CreateJumpDust();
                canAttackLeft = true;
            }
            else{
                canAttackLeft = false;
            }
            if (transform.position.x <= PlayerPosition.x + AttackDistance)
            {
                //npc moet naar rechts
                transform.position = new Vector3(transform.position.x + AngryMoveSpeed * Time.deltaTime, transform.position.y);
                gameObject.GetComponent<NPC_Movement>().CreateJumpDust();
                canAttackRight = true;
            }
            else{
                canAttackRight = false;
            }
            if (transform.position.x < PlayerPosition.x)
            {
                //npc moet naar rechts flippen
                flipSprite.FlipLeft(SpriteRenderer);
            }
            if (transform.position.x > PlayerPosition.x)
            {
                //npc moet naar links flippen
                flipSprite.FlipRight(SpriteRenderer);
            }
        }
    }
    
    #endregion
}
