using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bomb : MonoBehaviour
{
    private Cinemachine.CinemachineCollisionImpulseSource myInpulse;
    private Animator anim;
    public Collider2D coll;
    public Rigidbody2D rb;
    public AudioSource explotionAudio;

    public float startTime;
    public float waitTime;
    public float bombForce;
    public float power;

    [Header("Check")]
    public float radius;
    public LayerMask targeLayer;

    // Start is called before the first frame update
    void Start()
    {
        myInpulse = GetComponent<Cinemachine.CinemachineCollisionImpulseSource>();
        rb = GetComponent<Rigidbody2D>();
        coll = GetComponent<Collider2D>();
        anim = GetComponent<Animator>();
        explotionAudio = GetComponent<AudioSource>();
        startTime = Time.time;
    }

    // Update is called once per frame
    void Update()
    {
        if (!anim.GetCurrentAnimatorStateInfo(0).IsName("Bomb_Off"))//如果炸弹被点燃
        {
            if (Time.time > startTime + waitTime)//炸弹到了爆炸时间
            {
                anim.Play("Bomb_Explotion");//爆炸
            }
        }
    }

    public void Explotion()//animation event
    {
        explotionAudio.Play();

        coll.enabled = false;//物体碰撞关闭

        Collider2D[] aroundObjects = Physics2D.OverlapCircleAll(transform.position, radius, targeLayer);//检测周围物体

        rb.gravityScale = 0;

        myInpulse.GenerateImpulse();

        foreach (var item in aroundObjects)//对周围物体造成冲击
        {
            Vector3 pos = transform.position - item.transform.position;

            item.GetComponent<Rigidbody2D>().AddForce((-pos + Vector3.up) * bombForce, ForceMode2D.Impulse);

            /*炸弹爆炸时引燃未爆炸的炸弹*/
            if (item.CompareTag("Bomb") &&
                item.GetComponent<Animator>().GetCurrentAnimatorStateInfo(0).IsName("Bomb_Off"))
            {
                item.GetComponent<Bomb>().TurnOn();
            }

            if (item.CompareTag("Player") || item.CompareTag("Enemy"))
            {
                item.GetComponent<IDamageable>().GetHit(power);
            }
        }
    }

    public void DestroyThis()//animation event
    {
        Destroy(gameObject);
    }

    public void TurnOff()
    {
        anim.Play("Bomb_Off");
        gameObject.layer = LayerMask.NameToLayer("NPC");
    }

    public void TurnOn()
    {
        startTime = Time.time;
        anim.Play("Bomb_On");
        gameObject.layer = LayerMask.NameToLayer("Bomb");
    }

    public void OnDrawGizmos()
    {
        Gizmos.DrawWireSphere(transform.position, radius);
    }
}
