using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Captain : Enemy,IDamageable
{
    SpriteRenderer sprite;
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

    public override void Init()
    {
        base.Init();
        sprite = GetComponent<SpriteRenderer>();
    }

    public override void Update()
    {
        base.Update();

        if (animState == 0)
        {
            sprite.flipX = false;
        }
    }

    public override void SkillAction()//重写技能方法
    {
        base.SkillAction();//在原有的基础上添加

        if (anim.GetCurrentAnimatorStateInfo(1).IsName("Captain_ScareRun"))//正在逃跑(逃跑时间为动画时间)
        {
            sprite.flipX = true;//翻转
            if (targetPoint.position.x < transform.position.x)
            {
                transform.position = Vector2.MoveTowards(transform.position, transform.position + Vector3.right, speed * 2 * Time.deltaTime);
            }
            else
            {
                transform.position = Vector2.MoveTowards(transform.position, transform.position + Vector3.left, speed * 2 * Time.deltaTime);
            }
        }
        else
        {
            sprite.flipX = false;
        }
    }
}
