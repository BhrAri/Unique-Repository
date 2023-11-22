using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class Sword : Weapon
{
    private Player player;
    private CapsuleCollider swordBox;
    [SerializeField] private Transform swordHand;
    [SerializeField] private Transform swordArm;
    private Material swordMaterial;
    public float swingSpeed;

    [SerializeField] private bool swinging = false; //make sure not to swing twice
    [SerializeField] private bool ready = true; //(swing phase) moves the sword from weaponStandardPosition to swing position   
    [SerializeField] private bool center = true; //(swing phase) the attack swing of the sword
    [SerializeField] private bool reset = true; //(swing phase) returns the sword to weaponStandardPosition
    [SerializeField] private bool glowGet = false; //is true if they hit the glow timing
    [SerializeField] private bool swing2 = false; //(swing phase) the timing was good and a second swing will happen

    [SerializeField] private int swingType; // changes the type of swing that will happen
    public int swapSwingType; // type of swing storage
    [SerializeField] private float t; //the percentage a swing is currently at, from 0-1
    private bool tReset; //important for making sure swings start at 0% completed
    public float glowUpperBound; //percentage of the swing in decimal format that the glow timing is active before the center of the swing (higher number)
    public float glowLowerBound; //percentage of the swing in decimal format that the glow timing is active after the center of the swing (lower number)

    //position variables for swinging
    //weapon starts off at weaponStandardPosition = new(0f, 90f, 0f);
    private Vector3 readyPosition = new(90f, 90f, 0f); //first movement target position, beginning of first swing
    private Vector3 swingCentralPosition = new(90f, 0f, 0f); //(swing phase) midpoint of swings
    private Vector3 centerPosition = new(90f, -90f, 0f); // second movement target position, end of first swing, beginning of second swing
    //weapon ends at weaponStandardPosition = new(0f, 90f, 0f);

    //test variable
    public Vector4 whereHandLocal;

    void Start()
    {
        player = FindAnyObjectByType<Player>();
        brokenWeaponStorage = FindObjectOfType<BrokenWeaponStorage>().transform;
        swingType = swapSwingType;
        weaponStandardPosition = new(0f, 90f, 0f);
        swingSpeed = 1f;
        durability = 20; //durability goes down once per slash, but extra slashes from good glow timing don't lower it
        damage = 5;
        swordBox = GetComponent<CapsuleCollider>();
        swordBox.enabled = false;
        swordHand = transform.parent;
        swordArm = swordHand.parent;
        glowLowerBound = .2f;
        glowUpperBound = .8f;
        swordMaterial = GetComponent<Renderer>().material;
    }
    private void Update()
    {
        if (Input.GetButtonUp("Fire3")) //I usually hate GetButtonUp for personal petty reasons, but it is the best tool to use here
        {
            if (swapSwingType < 2)
            { 
                swapSwingType++; 
            }
            else { swapSwingType = 0; }
        }
        if (swinging)
        {
            Swing();
        }
        whereHandLocal = new Vector4(swordHand.localEulerAngles.x, swordHand.localEulerAngles.y, swordHand.localEulerAngles.z, swordHand.localRotation.w);
    }

    public override void SwingWeapon()
    {
        if (!swinging) 
        {
            swingType = swapSwingType;
            swinging = true;
            swordBox.enabled = true;
        }
    }
    private void Swing()
    {
        // I want to make this a place where I can call three different swing methods that
        // a player can select to use in a menu

        // Maybe attack and continue combo/glow is always mapped to [space] but
        // by pressing [shift] you can change during the swing to have the next swing in the combo be different
        // line 86 doesn't apply to up and down slash right now, but normal can cancel into either

        switch (swingType)
        {
            case 0: //normaler slash (previously normal slash) is still suboptimal
                NormalerSlash(weaponStandardPosition, readyPosition, swingCentralPosition, centerPosition);
                swingSpeed = 2.5f + player.dexterity;
                break;
            case 1: // closer to optimal but not quite there
                UpwardSlash(new (30,-90,-270),new (30,-90, -150), new (30,-90,-90), new (30,-90,10));
                swingSpeed = 4f + player.dexterity;
                break;
            case 2: //should add a couple variables to the v3's but basically the other slashes wish they were DownSlash
                DownwardSlash(new ( 0f, 0f, 0f), new (90f,0f,0f), new (90f,0f,360f), 1f, 4f, swordMaterial);
                swingSpeed = 1f + player.dexterity;
                break;
        }
        
    }

    //Beginning of abstraction. Everything important referenced above

    private void NormalerSlash(Vector3 weaponStandardPosition, Vector3 readyPosition, Vector3 swingCentralPosition, Vector3 centerPosition)
    {
        if (ready) //weapon down
        {
            swordHand.localEulerAngles = Vector3.Lerp(weaponStandardPosition, readyPosition, t += Time.deltaTime * swingSpeed);
            if (t >= 1f)
            {
                t = 0f;
                swordHand.localEulerAngles = readyPosition;
                ready = false;
            }
        }
        else if (center) //swing 1
        {
            if (t < 1f && !tReset)
            {
                swordHand.localEulerAngles = Vector3.Lerp(readyPosition, swingCentralPosition, t += Time.deltaTime * swingSpeed);
            }
            else
            {
                if (!tReset) { swordHand.localEulerAngles = swingCentralPosition; t = 0f; tReset = true; }
                swordHand.localEulerAngles = Vector3.Lerp(swingCentralPosition, centerPosition, t += Time.deltaTime * swingSpeed);
            }

            if (!tReset && t > glowUpperBound || tReset && t < glowLowerBound)
            {
                Glow();
            }
            else { GetComponent<Renderer>().material.SetColor("_Color", Color.white); }

            if (t >= 1f && tReset)
            {
                t = 0f;
                tReset = false;
                swordHand.localEulerAngles = centerPosition;
                center = false;
                if (glowGet)
                {
                    glowGet = false;
                    swing2 = true;
                    swingType = swapSwingType;
                }
            }
        }
        else if (reset && swing2) //swing 2
        {
            if (t < 1f && !tReset)
            {
                swordHand.localEulerAngles = Vector3.Lerp(centerPosition, swingCentralPosition, t += Time.deltaTime * swingSpeed);
            }
            else
            {
                if (!tReset) { swordHand.localEulerAngles = swingCentralPosition; t = 0f; tReset = true; }
                swordHand.localEulerAngles = Vector3.Lerp(swingCentralPosition, readyPosition, t += Time.deltaTime * swingSpeed);
            }
            if (!tReset && t > glowUpperBound || tReset && t < glowLowerBound)
            {
                Glow();
            }
            else { GetComponent<Renderer>().material.SetColor("_Color", Color.white); }

            if (t >= 1f && tReset)
            {
                tReset = false;
                t = 0f;
                swordHand.localEulerAngles = readyPosition;
                if (glowGet)
                {
                    glowGet = false;
                    center = true;
                    swing2 = false;
                } else
                {
                    reset = false;
                }
                
            }
        }
        else if (reset) //weapon up from swing 1
        {
            swordHand.localEulerAngles = Vector3.Lerp(centerPosition, weaponStandardPosition, t += Time.deltaTime * swingSpeed);
            if (t >= 1f)
            {
                
                t = 0f;
                swordHand.localEulerAngles = weaponStandardPosition;
                reset = false;
            }
        }
        else if (swing2) //weapon up from swing 2
        {
            
            swordHand.localEulerAngles = Vector3.Lerp(readyPosition, weaponStandardPosition, t += Time.deltaTime * swingSpeed);
            if (t >= 1f)
            {
                t = 0f;
                swordHand.localEulerAngles = weaponStandardPosition;
                swing2 = false;
            }
        }
        else
        {
            ready = true;
            center = true;
            reset = true;
            swinging = false;
            swordBox.enabled = false;
            durability--;
            Break();
        }
    }
    private void UpwardSlash(Vector3 weaponStandardPosition, Vector3 readyPosition, Vector3 swingCentralPosition, Vector3 centerPosition)
    {
        if (ready) //weapon down
        {
            swordHand.localEulerAngles = readyPosition;
            swordArm.localPosition = new Vector3(0f,-0.5f,0f);
            ready = false;
        }
        else if (center) //swing 1
        {
            

            if (t < 1f && !tReset)
            {
                swordHand.localEulerAngles = Vector3.Lerp(readyPosition, swingCentralPosition, t += Time.deltaTime * swingSpeed);
            }
            else
            {
                if (!tReset) { swordHand.localEulerAngles = swingCentralPosition; t = 0f; tReset = true; }
                swordHand.localEulerAngles = Vector3.Lerp(swingCentralPosition, centerPosition, t += Time.deltaTime * swingSpeed);
                swordArm.localPosition = new Vector3(0.5f - t * 1.3f, t/2.3f, 0.5f);
            }

            if (!tReset && t > glowUpperBound || tReset && t < glowLowerBound)
            {
                Glow();
            }
            else { GetComponent<Renderer>().material.SetColor("_Color", Color.white); }

            if (t >= 1f && tReset)
            {
                t = 0f;
                tReset = false;
                if (glowGet && swapSwingType == 1)
                {
                    glowGet = false;
                    swordHand.localEulerAngles = readyPosition;
                    swordArm.localPosition = new Vector3(0.5f, -0.5f, 0.5f);
                }
                else
                {
                    swordHand.localEulerAngles = centerPosition;
                    swordArm.localPosition = new Vector3(0.5f, -0.25f, 0.5f);
                    center = false;
                }
            }

        }
        else if (reset) //weapon back from swing 1
        {
            swordHand.localEulerAngles = Vector3.Lerp(centerPosition, new(-90f, -90f, 90f), t += Time.deltaTime * swingSpeed);
            if (t >= 1f)
            {
                t = 0f;
                swordHand.localEulerAngles = weaponStandardPosition;
                reset = false;
            }
        }
        else
        {
            ready = true;
            center = true;
            reset = true;
            swinging = false;
            swordBox.enabled = false;
            durability--;
            Break();
        }
    }
    private void DownwardSlash(Vector3 readyPosition, Vector3 swingEnd , Vector3 glowSwingEnd, float speed1, float speed2, Material material)
    {
        if (center) // prepare to swing
        {
            swordArm.localPosition = new Vector3(0f, 0.5f, 0f);
            swordHand.localEulerAngles = readyPosition;
            center = false;
        }
        if (ready) //swing 1
        {
            swordHand.localEulerAngles = Vector3.Lerp(readyPosition, swingEnd, t += Time.deltaTime * swingSpeed);
            swordArm.localPosition = new Vector3(0f, 0.5f - t/0.75f,0f);

            if (t > glowUpperBound) { Glow();}

            if (t >= 1f)
            {
                material.SetColor("_Color", Color.white);
                t = 0f;
                ready = false;
                swordHand.localEulerAngles = swingEnd;
                swordArm.localPosition = new Vector3(0f, -0.25f, 0f);
                if (glowGet)
                {
                    glowGet = false;
                    swing2 = true;
                }
            }
            
        }
        else if (swing2) // swing 2
        {
            swingSpeed = speed2;

            swordHand.localEulerAngles = Vector3.Lerp(swingEnd, glowSwingEnd, t += Time.deltaTime * swingSpeed);

            if (t > glowUpperBound) { Glow(); }

            if (t >= 1)
            {
                material.SetColor("_Color", Color.white);
                t = 0f;
                swordHand.localEulerAngles = swingEnd;
                if (!glowGet)
                {
                    swingSpeed = speed1;
                    swing2 = false;
                }
                glowGet = false;
            }
        }
        else //weapon back from swing 1 or 2
        {
            swordHand.localEulerAngles = Vector3.Lerp(swingEnd, readyPosition, t += Time.deltaTime * swingSpeed);
            swordArm.localPosition = new Vector3(0f, -0.25f + t * 0.75f, 0f);

            if (t >= 1f)
            {
                t = 0f;
                swordHand.localEulerAngles = readyPosition;
                swordArm.localPosition = new Vector3(0f, 0.5f, 0f);
                center = true;
                ready = true;
                reset = true;
                swinging = false;
                swordBox.enabled = false;
                durability--;
                Break();
            }
        }
    }
    private void Glow()
    {
        swordMaterial.SetColor("_Color", Color.cyan);
        if (Input.GetButtonDown("Jump"))
        {
            glowGet = true;
            Debug.Log("timing GENIUS");
        }
        Debug.Log("timing is now");
    }
    public void DisableHitUntilNextSwing()
    {
        //not needed yet, hopefully never needed
    }
    public override void DamageSet() //sets the damage and glow timing based off of player stats
    {
        damage = player.strength + 5;
        if (glowLowerBound < 1f)
        {
            glowLowerBound = 0.2f + (player.dexterity / 100f);
        }
        if (glowUpperBound > 0f)
        {
            glowUpperBound = 0.8f + (player.dexterity / 100f);
        }
    }
}
