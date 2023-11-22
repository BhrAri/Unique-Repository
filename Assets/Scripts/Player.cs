using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class Player : Fighter
{
    public float cameraSpeed;
    public float horizontal;
    public CameraArm cameraArm;
    public WeaponHand weaponHand;
    [SerializeField] private Vector3 movingDirection;
    [SerializeField] private float moveDirection;
    private static Player instance;
    public bool lookingToInteract;
    public GameObject hpUI;
    private Slider hpSlider;
    private TMP_Text hpText;
    public TMP_Text ammunition;
    public Inventory inventory;
    public static Player Instance { get { return instance; } }
    public int maxHp;
    public int strength;
    public int magic;
    public int dexterity;


    private void Awake()
    {
        if (instance != null && instance != this)
        {
            Destroy(this.gameObject);
        }
        else
        {
            instance = this;
        }
        DontDestroyOnLoad(this);
    }
    private void Start()
    {
        Hp = maxHp;
        hpSlider = hpUI.GetComponent<Slider>();
        hpText = hpUI.GetComponentInChildren<TMP_Text>();
        inventory = GetComponentInParent<Inventory>();
        fighterRigidbody = GetComponent<Rigidbody>();
        cameraArm = FindAnyObjectByType<CameraArm>();
        weaponHand = cameraArm.GetComponentInChildren<WeaponHand>();
    }
    void Update()
    {
        PlayerMovement();
        if (Input.GetKeyDown(KeyCode.E))
        {
            lookingToInteract = true;
            StartCoroutine(InteractTimer());
        }
        if (Input.GetButtonDown("Jump"))
        {
            SwingWeapon();
            
            if (inventory.ammunition > 0)
            {
                inventory.ammunition--;
                ammunition.text = "Uses Left: " + inventory.ammunition;
            } else
            {
                ammunition.text = "";
            }
            
        }
    }

    //Beginning of abstraction. Everything important referenced above
    private void PlayerMovement()
    {
        
        if (Input.GetAxisRaw("Horizontal") != 0)
        {
            transform.eulerAngles = new(0, horizontal += Input.GetAxis("Horizontal") * cameraSpeed * Time.deltaTime, 0);
        }
        
        moveDirection = cameraArm.horizontal;

        movingDirection = new Vector3(Mathf.Sin(moveDirection / 57.296f), 0, Mathf.Cos(moveDirection / 57.296f));
        
    
        if (Input.GetAxisRaw("Vertical") != 0)
        {
            fighterRigidbody.AddForce(Input.GetAxisRaw("Vertical") * moveSpeed * Time.deltaTime * movingDirection, ForceMode.Impulse);
        }

    }
    private void SwingWeapon()
    {
        weaponHand.YouCalled();
        if (weaponHand.hasWeapon)
        {
            weaponHand.weapon.DamageSet();
            weaponHand.weapon.SwingWeapon();
        }
        else
        {
            Debug.Log("where weapon?");
        }
    }
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("World Floor"))
        {
            transform.position += new Vector3(0f, 50f, 0f);
        }
        if (other.CompareTag("Enemy"))
        {
            Hp -= other.attachedRigidbody.gameObject.GetComponent<Enemy>().damage;
            hpSlider.value = Hp;
            hpText.text = "Hp: " + Hp;
        }
    }
    IEnumerator InteractTimer()
    {
        yield return new WaitForSeconds(1);
        lookingToInteract = false;
    }
}
