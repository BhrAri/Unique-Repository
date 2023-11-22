using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Worm : Enemy
{
    public GameObject wormSword;
    public GameObject wormSwordHand;
    public float swingSpeed = 1f;
    private float t = -12f;
    private float swing;
    public bool attackBool1;
    public bool attackBool2;
    public bool attackBool3;

    public override void Attack()
    {
        if (!attackBool1)
        {
            swing = 0;
            wormSword.GetComponent<Collider>().enabled = true;
            attackBool1 = true;
        } else if (!attackBool2)
        {
            swing += Time.deltaTime * swingSpeed;
            wormSwordHand.transform.localEulerAngles = Vector3.Lerp(new(0f, 0f, 0f), new(-90f, 0f, 0f), swing);
            if (swing >= 1)
            {
                wormSwordHand.transform.localEulerAngles = new(-90f, 0f, 0f);
                swing = 0f;
                attackBool2 = true;
                wormSword.GetComponent<Collider>().enabled = false;
            }
        } else if (!attackBool3)
        {
            swing += Time.deltaTime * swingSpeed;
            wormSwordHand.transform.localEulerAngles = Vector3.Lerp(new(-90f, 0f, 0f), new(0f, 0f, 0f), swing);
            if (swing >= 1)
            {
                wormSwordHand.transform.localEulerAngles = new(0f, 0f, 0f);
                attackBool3 = true;
            }
        } else
        {
            attackBool1 = false;
            attackBool2 = false;
            attackBool3 = false;
        }
    }
    public override IEnumerator AttackHappen()
    {
        return base.AttackHappen();
    }
    public override void Death()
    {
        transform.localEulerAngles = naturalRotation;
        t += Time.deltaTime * 15;
        wormSword.transform.localEulerAngles = Vector3.Lerp(new(-270f, 0f, -180f), new(0f, 0f, -180f), t /-12f);
        if (t < 0)
        {
            wormSword.transform.localPosition = new(0.1f, t, 0.54f);
        }
        if (t >= 3f)
        {
            base.Death();
        }
    }
}
