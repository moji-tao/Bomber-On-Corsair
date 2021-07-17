using System.Collections;
using System.Collections.Generic;
using System.Xml.Serialization;
using UnityEngine;

public class Cannon : Idem
{
    public GameObject Ball;
    public Animator anim;
    public Transform point;
    public float power;
    public int number;

    // Start is called before the first frame update
    public override void Start()
    {
        anim = GetComponent<Animator>();
    }

    // Update is called once per frame
    public override void Update()
    {
        
    }

    public override void Use()
    {
        if (number == 0)
            return;
        if (Time.time > PlayerControl.instance.nextAttack)
        {
            --number;
            anim.SetTrigger("attack");
            PlayerControl.instance.nextAttack = Time.time + PlayerControl.instance.attackRate;
        }
    }

    public void FireBall()//动画事件
    {
        GameObject thisBall = Instantiate(Ball, point.position, Ball.transform.rotation);
        float direction = (point.position.x - transform.position.x)/Mathf.Abs(point.position.x - transform.position.x);
        //thisBall.GetComponent<Rigidbody2D>().AddForce(new Vector2(direction, 0) * power, ForceMode2D.Impulse);
        thisBall.GetComponent<Ball>().direction = direction;
        thisBall.GetComponent<Ball>().speed = power;
    }

    /*private void OnTriggerStay2D(Collider2D collision)
    {
        if (collision.transform.tag == "Player")
        {
            PlayerControl.instance.bombAttack = false;
            if(Input.GetKeyDown(KeyCode.J))
            {
                Attack();
            }
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.transform.tag == "Player")
        {
            PlayerControl.instance.bombAttack = true;
        }
    }*/
}
