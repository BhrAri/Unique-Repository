using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Fighter : MonoBehaviour
{
    public Rigidbody fighterRigidbody;
    public float moveSpeed = 5f;
    public float Hp = 10f;
    public int wound = 0; //increase when hit so each successive hit can do a little more damage
    public void Perish()
    {
        Destroy(gameObject);
    }
}
