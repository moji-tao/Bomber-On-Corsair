using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Big_Guy : Enemy, IDamageable
{
    public Transform pickupPoint;
    public float power;
    public AudioSource throwAudio;

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

    public void PickUpBomb()//拾起炸弹 animation event
    {
        if (targetPoint.CompareTag("Bomb"))
        {
            targetPoint.gameObject.transform.position = pickupPoint.position;//拾起炸弹

            targetPoint.SetParent(pickupPoint);//将炸弹设为子集

            targetPoint.GetComponent<Rigidbody2D>().bodyType = RigidbodyType2D.Kinematic;//取消炸弹重力

            //targetPoint.GetComponent<Bomb>().TurnOff();

            hasBomb = true;
        }
    }

    public void ThrowAway()
    {
        if (hasBomb)
        {
            targetPoint.GetComponent<Rigidbody2D>().bodyType = RigidbodyType2D.Dynamic;
            targetPoint.SetParent(transform.parent.parent);
            //targetPoint.GetComponent<Bomb>().TurnOn();

            if ((FindObjectOfType<PlayerControl>().transform.position.x) -
                transform.position.x < 0)//玩家在Big Guy左侧
            {
                targetPoint.GetComponent<Rigidbody2D>().AddForce(new Vector2(-1, 1) * power, ForceMode2D.Impulse);
            }
            else
            {
                targetPoint.GetComponent<Rigidbody2D>().AddForce(new Vector2(1, 1) * power, ForceMode2D.Impulse);
            }

            throwAudio.Play();
        }
        hasBomb = false;
    }
}
