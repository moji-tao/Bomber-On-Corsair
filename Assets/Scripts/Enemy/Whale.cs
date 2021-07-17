using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Whale : Enemy,IDamageable
{
    public float scale;
    public float maxSize;

    public override void Init()
    {
        base.Init();

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

    public void Swalow()//吞炸弹，动画事件
    {
        targetPoint.GetComponent<Bomb>().TurnOff();
        //targetPoint.gameObject.SetActive(false);
        Destroy(targetPoint.gameObject);
        if (transform.localScale.y * scale < maxSize)//最大体形
        {
            transform.localScale *= scale;//体形增长
            transform.Find("Hit point").GetComponent<HitPoint>().power += 1;//体形增长时伤害增加
            maxHealth += 1;//体形增长时最大生命值增加
            skillRange *= scale;
            attackRange *= scale;
            if (isBoss)
            {
                UIManager.instance.SetBossHealth(maxHealth);
                UIManager.instance.UpdateBossHealth(health);
            }
        }
        if (health + 1 < maxHealth)//吞炸弹时生命值增加，但不会超过最大生命值
        {
            transform.GetComponent<Enemy>().health += 1;
        }
        else
        {
            health = maxHealth;
        }
    }
}
