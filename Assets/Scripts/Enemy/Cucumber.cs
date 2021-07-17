using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Cucumber : Enemy,IDamageable
{
    public void SetOff()//动画事件
    {
        targetPoint.GetComponent<Bomb>().TurnOff();
    }
    public void GetHit(float damage)
    {
        health -= damage;
        if (health < 1)
        {
            health = 0;
            isDead = true;
        }
        anim.SetTrigger("hit");
    }
}
