using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bald_Pirate : Enemy, IDamageable
{
    public float power;
    public void GetHit(float damage)
    {
        health -= damage;
        if(health<1)
        {
            health = 0;
            isDead = true;
        }
        anim.SetTrigger("hit");
    }

    public void Kick()
    {
        int dir;
        if (transform.position.x > targetPoint.transform.position.x)
        {
            dir = -1;
        }
        else
        {
            dir = 1;
        }
        targetPoint.gameObject.GetComponent<Rigidbody2D>().AddForce(new Vector2(dir, 1) * power, ForceMode2D.Impulse);
    }
}
