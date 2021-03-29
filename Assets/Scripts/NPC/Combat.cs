using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

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
    private Color defaultColor;
    [HideInInspector]public bool canAttackRight = false;
    [HideInInspector]public bool canAttackLeft = false;

    public Action EnemyDied;
    [Header("Combat")]
    public int health = 100;
    public bool Kill = false;
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
    [Tooltip("hoe lang de sprite een andere kleur is wanneer hij geraakt is")]
    public float TimeToColor;
    [Tooltip("de preafab van het death effect")]
    public ParticleSystem DeathParticle;
    [Tooltip("de prefab van de coin pickup")]
    public GameObject Coin;
    [Tooltip("de prefab van de ammo pickup")]
    public GameObject AmmoPickup;
    [Tooltip("hoelang het duurt totdat de ammo despawned")]
    public float despawntime = 100;
    #endregion

    #region General
    private void Start()
    {
        SpriteRenderer = gameObject.GetComponent<SpriteRenderer>();
        NPCAnim = gameObject.GetComponent<Animator>();
        flipSprite = gameObject.GetComponent<FlipSprite>();
        InvokeRepeating("PerformExternalAttack", 0f, 2f);
        defaultColor = SpriteRenderer.color;
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
                yield return new WaitForSeconds(UnityEngine.Random.Range(.5f, 1f));
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
                yield return new WaitForSeconds(UnityEngine.Random.Range(.5f, 1f));
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
        int randDamage = UnityEngine.Random.Range(DamageStrenght.x, DamageStrenght.y);
        print($"Damage for: {randDamage}");
    }
    private void PerformExternalAttack()
    {
        if(canAttackLeft && canAttackRight && ExternalAttack)
        {
            //als npc een eigen attack heeft, voer de code dan hier uit
        }
    }

    public void Takedamage(int damage)
    {
        health -= damage;
        StartCoroutine(SwitchColor());

        if (health <= 0)
        {
            drops();
            Die();
        }
    }

    void Die()
    {
        EnemyDied();
        ParticleSystem particle = Instantiate(DeathParticle, transform.position, transform.rotation);
        particle.Play();
        Destroy(gameObject);
        Destroy(particle, 1);
    }
    void drops()
    {
        GameObject coins = Instantiate(Coin, transform.position + , transform.rotation);
        GameObject Ammopickups = Instantiate(AmmoPickup, transform.position, transform.rotation);
        Destroy(coins, despawntime);
        Destroy(Ammopickups, despawntime);
    }
    private void Update() {
        if(Kill){
            Kill = false;
            Die();
        }
    }

    IEnumerator SwitchColor()
    {
        SpriteRenderer.color = new Color(1f, 0.30196078f, 0.30196078f);
        yield return new WaitForSeconds(TimeToColor);
        SpriteRenderer.color = defaultColor;
    }
    /*
    IEnumerator DeathParticleTime()
    {
        ParticleSystem particle = Instantiate(DeathParticle, transform.position, transform.rotation);
        DeathParticle.Play();
        yield return new WaitForSeconds(1);
    }
    */
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
                flipSprite.FlipRight(SpriteRenderer);
                transform.position = new Vector3(transform.position.x - AngryMoveSpeed * Time.deltaTime, transform.position.y);
                gameObject.GetComponent<NPC_Movement>().CreateJumpDust();
            }
            if (transform.position.x <= PlayerPosition.x + AttackDistance)
            {
                //npc moet naar rechts 
                flipSprite.FlipLeft(SpriteRenderer);
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
