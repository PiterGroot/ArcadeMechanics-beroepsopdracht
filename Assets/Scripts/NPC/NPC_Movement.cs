using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

public enum NPCType { Slime, Character}


public class NPC_Movement : MonoBehaviour
{
    #region Variables
    //private componenten
    private Rigidbody2D rb2d;
    [HideInInspector] public Animator NPCAnim;
    private bool isMoving = false;
    private bool MoveCharRight = false;
    private bool MoveCharLeft = false;
    private float MoveSpeed = 2f; //alleen voor character (lopen over de grond)
    private FlipSprite flipSprite; 
    private SpriteRenderer SpriteRenderer;
    [HideInInspector]public Vector2 CenterPos;
    [HideInInspector]public float PositiveTriggerBoundX;
    [HideInInspector]public bool grounded = false;
    [HideInInspector]public float NegativeTriggerBoundX;
    [HideInInspector]public bool IsEnabled = true;

    [Header("Components")]
    [Tooltip("Particle effect voor het bewegen van de npc.")]
    [Space]public ParticleSystem MoveDust;
    [Tooltip("Particle effect voor de landing van de npc.")]
    public ParticleSystem LandDust;
    [Tooltip("naam sound effect voor bewegen, zie AudioManager.")]
    [SerializeField, Space]private string MovingSound = "Jump";
    [Tooltip("naam sound effect voor het landen, zie AudioManager.")]
    [SerializeField]private string LandingSound = "Land";

    [Header("Behaviour")]
    public NPCType NPCType = NPCType.Slime;
    [Space]public bool canMoveRandomly = true;
    [Tooltip("Geeft willekeurige value aan de activity slider voor meer variatie tussen de npcs.")]
    public bool RandomizeActivity = false;
    [Space]public bool AlwaysDrawGizmo = true;
    [Tooltip("Gebied waar de npc het liefst in blijft. Zie de groene vierkant in de scene view.")]
    public int DesiredArea = 7;
    [Tooltip("Float die de kansen om de npc te laten bewegen beïnvloed.")]
    [Range(0f, 1f)]public float Activity = .5f;
    [Tooltip("Als randomizeActivity aan staat, geeft willekeurig getal aan activity tussen deze min en max.")]
    public Vector2 MinMaxValue = new Vector2(.3f, .7f);
    #endregion

    void Start()
    {
        //caching componenten
        NPCAnim = gameObject.GetComponent<Animator>();
        rb2d = gameObject.GetComponent<Rigidbody2D>();
        flipSprite = gameObject.GetComponent<FlipSprite>();
        SpriteRenderer = gameObject.GetComponent<SpriteRenderer>();
        Application.targetFrameRate = 144;

        //geef componenten een value
        if (RandomizeActivity) { Activity = Random.Range(MinMaxValue.x, MinMaxValue.y); }
        CenterPos = transform.position;
        PositiveTriggerBoundX = DesiredArea / 2 + CenterPos.x;
        NegativeTriggerBoundX = DesiredArea / -2 + CenterPos.x;
        canMoveRandomly = true;

        //willekeurige vertraging voor de eerste sprong en het checken van de volgende zet
        float randDelay = Random.Range(1f, 4f);

        if (NPCType == NPCType.Slime) { NPCAnim.SetTrigger("IdleSlime"); InvokeRepeating("CheckNextMoveSlime", randDelay, 2.85f); }
        if (NPCType == NPCType.Character) { NPCAnim.SetTrigger("IdleCharacter"); InvokeRepeating("CheckNextMoveCharacter", randDelay, 2.85f); }
    }

    #region General
    private void OnTriggerEnter2D(Collider2D collision)
    {
        //ground bool aanzetten als npc op de grond staat
        if (collision.tag == "ground")
        {
            CreateLandDust();
            FindObjectOfType<AudioManager>().Play(LandingSound);
            grounded = true;
        }
    }
    private void OnTriggerExit2D(Collider2D collision)
    {
        //ground bool uitzetten als npc van de grond af springt
        if (collision.tag == "ground")
        {
            grounded = false;
            FindObjectOfType<AudioManager>().Play(MovingSound);
        }
    }
    
    void OnDrawGizmos()
    {
        //tekenen van de radius in de scene view
        if (AlwaysDrawGizmo)
        {
            Gizmos.color = Color.green;
            Gizmos.DrawWireCube(CenterPos, new Vector3(DesiredArea, DesiredArea, 0f));
        }
    }
    private void OnDrawGizmosSelected()
    {
        //tekenen van de radius in de scene view alleen als je op de npc klikt
        if (!AlwaysDrawGizmo)
        {
            Gizmos.color = Color.green;
            Gizmos.DrawWireCube(CenterPos, new Vector3(DesiredArea, DesiredArea, 0f));
        }
    }
    //geeft random int terug tussen een bepaalde range
    public int RandomInt(int RangeMin, int RangeMax)
    {
        int randInt = Random.Range(RangeMin, RangeMax);
        return randInt;
    }
    //geeft random float terug tussen een bepaalde range
    private float RandomFloat(float RangeMin, float RangeMax)
    {
        float randFloat = Random.Range(RangeMin, RangeMax);
        return randFloat;
    }
    //functies om particles te activeren
    public void CreateJumpDust()
    {
        MoveDust.Play();
    }
    public void CreateLandDust()
    {
        LandDust.Play();
    }
    #endregion
    
    #region SlimeCode
    void CheckNextMoveSlime()
    {   //mag en/of kan de npc een sprong uitvoeren
        if (canMoveRandomly && grounded)
        {
            //we kunnen een willekeurige sprong doen
            IdleLeap();
        }
    }

    IEnumerator CheckPositionSlime()
    {
        //checken of de npc buiten de radius zit
        if (transform.position.x >= PositiveTriggerBoundX)
        {
            //aan de rechter kant, dus moet naar links springen
            yield return new WaitForSeconds(Random.Range(1.5f, 2.1f));
            NPCAnim.SetTrigger("Leap");
            StartCoroutine(LeapLeft());
        }
        if (transform.position.x <= NegativeTriggerBoundX)
        {
            //aan de linker kant, dus moet naar rechts springen
            yield return new WaitForSeconds(Random.Range(1.5f, 2.1f));
            NPCAnim.SetTrigger("Leap");
            StartCoroutine(LeapRight());
        }
        else
        {
            canMoveRandomly = true;
        }
    }

    public void IdleLeap()
    {
        //50/50 kans om te springen of stil te staan, om het natuurlijker te laten lijken
        bool Leap = (Random.value < Activity);
        if (Leap)
        {
            //npc gaat springen, dit betekent dus een random sprong naar een willekeurig kant
            //50/50 kans om naar links of rechts te gaan
            bool LeftOrRight = (Random.value > 0.5f);
            if (LeftOrRight)
            {
                //leap animatie aanzetten en LeapRight() functie aanroepen, canLeapRandomly op
                //false zodat er geen dubbele sprongen gemaakt kunnen worden
                canMoveRandomly = false;
                NPCAnim.SetTrigger("Leap");
                StartCoroutine(LeapRight());
            }
            else
            {
                //leap animatie aanzetten en LeapLeft() functie aanroepen, canLeapRandomly op
                //false zodat er geen dubbele sprongen gemaakt kunnen worden
                canMoveRandomly = false;
                NPCAnim.SetTrigger("Leap");
                StartCoroutine(LeapLeft());
            }
        }
        else
        {
            //npc blijft stil zitten, de loop begint weer opnieuw bij CheckNextMove() door de InvokeRepeating instructie
            canMoveRandomly = true;
        }
    }
    public IEnumerator LeapRight()
    {
        flipSprite.FlipLeft(SpriteRenderer);
        //npc gaat naar rechts springen
        if (!grounded)
        {
            yield return new WaitUntil(() => grounded == true);
            yield return new WaitForSeconds(.5f);
            int randRight = RandomInt(75, 150);
            rb2d.AddForce(new Vector2(randRight, 350f));
            CreateJumpDust();
        }
        else
        {
            yield return new WaitForSeconds(.5f);
            int randRight = RandomInt(75, 150);
            rb2d.AddForce(new Vector2(randRight, 350f));
            CreateJumpDust();
        }
    }
    public IEnumerator LeapLeft()
    {
        flipSprite.FlipRight(SpriteRenderer);
        //npc gaat naar links springen
        if (!grounded)
        {
            yield return new WaitUntil(() => grounded == true);
            yield return new WaitForSeconds(.5f);
            int randLeft = RandomInt(-75, -150);
            rb2d.AddForce(new Vector2(randLeft, 350f));
            CreateJumpDust();
        }
        else
        {
            yield return new WaitForSeconds(.5f);
            int randLeft = RandomInt(-75, -150);
            rb2d.AddForce(new Vector2(randLeft, 350f));
            CreateJumpDust();
        }
    }
    public void PlayIdleAnimSlime()
    {
        //aan het eind van de Leap animatie roept de animatie controller weer de idle animatie aan
        NPCAnim.SetTrigger("IdleSlime");
        StartCoroutine(CheckPositionSlime());
    }
    #endregion

    #region CharacterCode
    void CheckNextMoveCharacter()
    {   //mag en/of kan de npc een sprong uitvoeren
        if (canMoveRandomly && grounded)
        {
            //we kunnen een willekeurige sprong doen
            IdleMove();
        }
    }

    IEnumerator CheckPositionCharacter()
    {
        //checken of de npc buiten de radius zit
        if (transform.position.x >= PositiveTriggerBoundX && IsEnabled)
        {
            //aan de rechter kant, dus moet naar links bewegen
            yield return new WaitForSeconds(Random.Range(1.5f, 2.1f));
            NPCAnim.SetBool("Move", true);
            MoveLeft();
        }
        if (transform.position.x <= NegativeTriggerBoundX && IsEnabled)
        {
            //aan de linker kant, dus moet naar rechts bewegen
            yield return new WaitForSeconds(Random.Range(1.5f, 2.1f));
            NPCAnim.SetBool("Move", true);
            MoveRight();
        }
        else
        {
            canMoveRandomly = true;
            isMoving = false;
        }
    }
    private void Update()
    {
        if(NPCType == NPCType.Character)
        {
            if (isMoving)
            {
                NPCAnim.SetBool("Move", true);
            }
            else
            {
                NPCAnim.SetBool("Move", false);
            }
            if (MoveCharRight)
            {
                flipSprite.FlipRight(SpriteRenderer);
                transform.position = new Vector3(transform.position.x + MoveSpeed * Time.deltaTime, transform.position.y);
                CreateJumpDust();
            }
            if (MoveCharLeft)
            {
                flipSprite.FlipLeft(SpriteRenderer);
                transform.position = new Vector3(transform.position.x - MoveSpeed * Time.deltaTime, transform.position.y);
                CreateJumpDust();
            }
        }
    }
    public void IdleMove()
    {
        //50/50 kans om te springen of stil te staan, om het natuurlijker te laten lijken
        bool Move = (Random.value < Activity);
        if (Move)
        {
            //npc gaat springen, dit betekent dus een random sprong naar een willekeurig kant
            //50/50 kans om naar links of rechts te gaan
            bool LeftOrRight = (Random.value > 0.5f);
            if (LeftOrRight)
            {
                //leap animatie aanzetten en LeapRight() functie aanroepen, canLeapRandomly op
                //false zodat er geen dubbele sprongen gemaakt kunnen worden
                canMoveRandomly = false;
                isMoving = true;
                MoveRight();
            }
            else
            {
                //leap animatie aanzetten en LeapLeft() functie aanroepen, canLeapRandomly op
                //false zodat er geen dubbele sprongen gemaakt kunnen worden
                canMoveRandomly = false;
                isMoving = true;
                MoveLeft();
            }
        }
        else
        {
            //npc blijft stil zitten, de loop begint weer opnieuw bij CheckNextMove() door de InvokeRepeating instructie
            canMoveRandomly = true;
            isMoving = false;
        }
    }
    public void MoveRight()
    {
        //npc gaat naar rechts springen
        float randRightTime = RandomFloat(1.65f, 3f);
        Invoke("SetIsMovingFalse", randRightTime);
        MoveCharRight = true;
    }
        
    public void MoveLeft()
    {
        //npc gaat naar links springen
        float randLeftTime = RandomFloat(1.65f, 3f);
        Invoke("SetIsMovingFalse", randLeftTime);
        MoveCharLeft = true;
    }
    public void SetIsMovingFalse()
    {
        MoveCharRight = false;
        MoveCharLeft = false;
        isMoving = false;
        PlayIdleAnimCharacter();
    }
    public void PlayIdleAnimCharacter()
    {
        NPCAnim.SetTrigger("IdleCharacter");
        StartCoroutine(CheckPositionCharacter());
    }
    #endregion


}
