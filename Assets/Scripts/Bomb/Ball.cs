using System.Collections;
using System.Collections.Generic;
using System.Security.Permissions;
using UnityEngine;

public class Ball : MonoBehaviour
{
    private Cinemachine.CinemachineCollisionImpulseSource myInpulse;
    public Animator anim;
    public Collider2D coll;
    public Rigidbody2D rb;
    public AudioSource explotionAudio;

    public float ballForce;
    public float power;
    public float speed;
    public float direction;

    [Header("Check")]
    public float radius;
    public LayerMask targeLayer;
    public LayerMask explotionLayer;

    // Start is called before the first frame update
    void Start()
    {
        myInpulse = GetComponent<Cinemachine.CinemachineCollisionImpulseSource>();
        anim = GetComponent<Animator>();
        coll = GetComponent<Collider2D>();
        rb = GetComponent<Rigidbody2D>();
        explotionAudio = GetComponent<AudioSource>();
    }

    // Update is called once per frame
    void Update()
    {
        if (anim.GetCurrentAnimatorStateInfo(0).IsName("Ball_Idle"))
        { 
            IsCollider(); 
        }
    }

    public void IsCollider()//检测碰撞
    {
        Vector3 oriPos = transform.position;
        transform.Translate(new Vector3(direction * speed * Time.deltaTime, 0, 0));
        bool isCollider = Physics2D.Raycast(oriPos, new Vector2(direction, -0.5f), 0.5f, explotionLayer);
        if(isCollider)
        {
            anim.Play("Bomb_Explotion");
        }
    }

    public void Explotion()//爆炸，动画事件
    {
        explotionAudio.Play();

        coll.enabled = false;

        rb.constraints = RigidbodyConstraints2D.FreezeAll;
        transform.Translate(Vector3.zero);

        Collider2D[] aroundObjects = Physics2D.OverlapCircleAll(transform.position, radius, targeLayer);

        rb.gravityScale = 0;

        myInpulse.GenerateImpulse();

        foreach (var item in aroundObjects)
        {
            Vector3 pos = transform.position - item.transform.position;

            item.GetComponent<Rigidbody2D>().AddForce((-pos + Vector3.up) * ballForce, ForceMode2D.Impulse);

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

    public void DestroyThis()//动画事件
    {
        Destroy(gameObject);
    }

    /*private void OnCollisionEnter2D(Collision2D collision)
    {
        anim.Play("Bomb_Explotion");
    }
    */
    public void OnDrawGizmos()
    {
        Gizmos.DrawWireSphere(transform.position, radius);
    }
}
