using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using static UnityEngine.GraphicsBuffer;

public class Enemy : Fighter
{
    public float targetRadius = 5f;
    public float fastTargetRadius = 3f;
    public float attackRadius = 0.5f;
    public float playerDistance;
    public float timeSinceLastUpdate;
    public float timeUntilNextUpdate;
    public float damage;
    public float attackJumpSpeed = 1f;
    public float attackActiveTime = 1f;
    public bool attackJumped;
    public bool checkTime = true;
    public bool slowFollowPlayer;
    public bool fastFollowPlayer;
    public bool attacking;
    public Player player;
    public Vector3 naturalRotation;
    public Vector3 up = Vector3.up;
    public GameObject attackArea;
    public GameObject drop;
    //I might use this as a polymorph class
    // this worked so well the first time that I tried it that I never want to touch it again. Blessed am I
    public void Start()
    {
        player = FindAnyObjectByType<Player>();
        fighterRigidbody = GetComponent<Rigidbody>();
    }

    public void Update()
    {
        if (attacking)
        {
            Attack();
        }
        if (checkTime)
        {
            timeSinceLastUpdate += Time.deltaTime;
            if (timeSinceLastUpdate >= timeUntilNextUpdate)
            {
                CheckTime();
            }
        }
        
        if (!checkTime && Hp > 0)
        {
            EnemyMovement();
        } else if (Hp <= 0)
        {
            Death();
        }
        
    }
    public void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("PlayerWeapon"))
        {
            float damage = other.GetComponent<Weapon>().damage;
            Hp -= damage + damage * wound/7;
            wound++;
        }

    }
    virtual public void EnemyMovement()
    {   /* either way I should give the player an awakening radius so that enemys only have a script when the player is close enough
          
        use an update method determined by how far the player was last time I checked so the enemy only checks sometimes instead of every frame

         function would probably be find player position-> find distance to player-> if (playerSpeed != 0){distance - target radius / speed}->
           call update after that time has elapsed -> if player is too far despawn

           a lot of enemys in the same area will update near the same time, may cause unstable preformance

         */

        playerDistance = Vector3.Distance(player.transform.position, transform.position);

        if (!slowFollowPlayer)
        {
            if (playerDistance <= targetRadius)
            {
                slowFollowPlayer = true;
            }
        }
        else if (!fastFollowPlayer)
        {
            if (playerDistance <= fastTargetRadius)
            {
                fastFollowPlayer = true;
            }
        }

        if (slowFollowPlayer && !fastFollowPlayer)
        {
            transform.LookAt(new Vector3(player.transform.position.x, transform.position.y, player.transform.position.z), up);

            fighterRigidbody.AddRelativeForce((moveSpeed / 3) * Time.deltaTime * Vector3.forward, ForceMode.Impulse);

            if (playerDistance > targetRadius) 
            { 
                checkTime = true; 
                slowFollowPlayer = false;
                fighterRigidbody.velocity = Vector3.zero;
                fighterRigidbody.angularVelocity = Vector3.zero;
            }
        }
        else if (fastFollowPlayer)
        {
            slowFollowPlayer = false;

            transform.LookAt(new Vector3(player.transform.position.x, transform.position.y, player.transform.position.z), up);

            if (playerDistance <= attackRadius)
            {
                if (!attacking)
                {
                    attacking = true;
                    StartCoroutine(AttackHappen());
                }
            } 
            else if (playerDistance > targetRadius * 3) 
            {
                checkTime = true; 
                slowFollowPlayer = false;
                fighterRigidbody.velocity = Vector3.zero;
                fighterRigidbody.angularVelocity = Vector3.zero;
            }
            else
            {
                fighterRigidbody.AddRelativeForce(moveSpeed * Time.deltaTime * Vector3.forward, ForceMode.Impulse);
            }
            
        }
        
    }
    public void CheckTime()
    {
        timeSinceLastUpdate = 0;
        playerDistance = Vector3.Distance(player.transform.position, transform.position);
        float playerSpeed = player.moveSpeed;
        if (playerDistance > targetRadius)
        {
            if (playerSpeed != 0)
            {
                timeUntilNextUpdate = (playerDistance - targetRadius) / playerSpeed; //this is wrong, but is it wrong enough? likely
            }
        }
        else { checkTime = false; }
    }
    public virtual void Attack()
    {
        attackArea.SetActive(true);
        if (!attackJumped)
        {
            fighterRigidbody.AddRelativeForce(new Vector3(0f, 0.1f, 1f) * attackJumpSpeed, ForceMode.Impulse);
            attackJumped = true;
        }
    }
    public virtual IEnumerator AttackHappen()
    {
        yield return new WaitForSeconds(attackActiveTime);
        attacking = false;
        attackArea.SetActive(false);
        attackJumped = false;
    }
    public void OnDestroy()
    {
        Instantiate(drop, transform.position, drop.transform.rotation);
    }
    public virtual void Death()
    {
        Destroy(gameObject);
    }
}
