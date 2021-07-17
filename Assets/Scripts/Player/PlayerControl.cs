using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class PlayerControl : MonoBehaviour,IDamageable
{
    [Header("Player FSM")]
    public static PlayerControl instance;
    PlayerBaseState currentState;
    public UseBombState useBombState = new UseBombState();
    public UseItemsState useItemsState = new UseItemsState();
    public Collider2D idem;

    private Rigidbody2D rb;
    private Animator anim;

    [Header("Player Attribute")]
    public float speed;
    public float jumpForce;

    [Header("Player State")]
    public float health;
    public bool isDead;
    public AudioSource hitAudio;
    public AudioSource reAudio;

    [Header("Ground Check")]
    public Transform groundCheck;
    public float checkRadius;
    public LayerMask groundLayer;

    [Header("States Check")]
    public bool isGround;
    public bool isJump;
    public bool canJump;
    public Collider2D checkPlatform;

    [Header("FX")]
    public GameObject jumpFX;
    public GameObject landFX;
    public AudioSource jumpAudio;

    [Header("Attack Settings")]
    public GameObject bombPrefab;
    public float nextAttack = 0;
    public float attackRate;
    public bool bombAttack;

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    // Start is called before the first frame update
    void Start()
    {
        TransitionToState(useBombState);//一开始执行可放置炸弹模式

        rb = GetComponent<Rigidbody2D>();
        anim = GetComponent<Animator>();
        //bombAttack = true;

        GameManager.instance.IsPlayer(this);

        //继承血量
        health = GameManager.instance.LoadHealth();
        UIManager.instance.UpdateHealth(health);
    }

    void Update()
    {
        anim.SetBool("dead", isDead);
        if (isDead)
        { 
            return;
        }
        CheckInput();
        
    }

    public void TransitionToState(PlayerBaseState state)//转换模式
    {
        currentState = state;
        currentState.EnterState(this);
    }

    private void FixedUpdate()
    {
        if(isDead)
        {
            rb.velocity = Vector2.zero;
            return;
        }
        PhysicsCheck();
        Movement();
        Jump();
    }

    void CheckInput()
    {
        if (isGround) 
        {
            rb.gravityScale = 1;
            if(Input.GetButtonDown("Jump"))
            {
                canJump = true;
            }
        }
        else
        {
            rb.gravityScale = 5;
        }

        if (Input.GetKeyDown(KeyCode.J))//按下攻击键 
        {
            currentState.OnAttack(this);//对不同的物体调用不同的攻击方式
            //Attack();
        }

        if (Input.GetKey(KeyCode.S) && Input.GetKey(KeyCode.K))
        {
            if (checkPlatform != null)
            {
                Fall();
            }
        }
    }

    void Movement()
    {
        //float horizontalInput = Input.GetAxis("Horizontal");//-1 ~ 1
        float horizontalInput = Input.GetAxisRaw("Horizontal");//-1, 1

        rb.velocity = new Vector2(horizontalInput * speed, rb.velocity.y);//控制玩家速度

        if (horizontalInput > 0)
        {
            //transform.localScale = new Vector3(horizontalInput, 1, 1);//玩家面向
            transform.eulerAngles = new Vector3(0, 0, 0);
        }
        else if (horizontalInput < 0)
        {
            transform.eulerAngles = new Vector3(0, 180, 0);
        }
    }

    void Jump()
    {
        if (canJump) 
        {
            rb.velocity = new Vector2(rb.velocity.x, jumpForce);//跳跃
            jumpAudio.Play();
            jumpFX.SetActive(true);
            jumpFX.transform.position = transform.position + new Vector3(0.03f, -0.73f, 0);//跳跃产生粒子效果
            //rb.gravityScale = 5;
            canJump = false;
            isJump = true;
        }
    }

    /*
    public void JumpButton()
    {
        if (isGround)
        {
            rb.gravityScale = 1;
            if (Input.GetButtonDown("Jump"))
            {
                canJump = true;
            }
        }
        else
        {
            rb.gravityScale = 5;
        }
    }
    */

    void Fall()
    {
        if (checkPlatform != null)
        {
            StartCoroutine(FallPlatform()); 
        }
        
    }

    //炸弹攻击
    public void Attack()
    {
        if (Time.time > nextAttack && bombAttack) 
        {
            Instantiate(bombPrefab, transform.position, bombPrefab.transform.rotation);//实例化炸弹

            nextAttack = Time.time + attackRate;//设置下次攻击的最短时间
        }
    }

    //地面检测
    void PhysicsCheck()
    {
        isGround = Physics2D.OverlapCircle(groundCheck.position, checkRadius, groundLayer);
        if (isGround) //如果在地面上则可以跳跃
        {
            rb.gravityScale = 1;
            isJump = false;
        }
    }

    IEnumerator FallPlatform()
    {
        checkPlatform.enabled = false;
        yield return new WaitForSeconds(0.2f);//等待0.2秒
        checkPlatform.enabled = true;
        checkPlatform = null;
    }

    //动画事件
    public void LandFX()
    {
        landFX.SetActive(true);
        landFX.transform.position = transform.position + new Vector3(0.03f, -0.73f, 0);
    }

    public void OnDrawGizmos()
    {
        Gizmos.DrawWireSphere(groundCheck.position, checkRadius);
    }

    //改变生命值
    public void GetHit(float damage)
    {
        if (!anim.GetCurrentAnimatorStateInfo(1).IsName("Player_hit"))//当正在收到伤害时，产生无敌帧
        {
            health -= damage;
            if (health < 1)
            {
                health = 0;
                isDead = true;
            }
            if (damage > 0)
            {
                anim.SetTrigger("hit");
                hitAudio.Play();
            }
            else if (damage < 0)
            {
                reAudio.Play();
            }


            UIManager.instance.UpdateHealth(health);
        }
    }

    //接触到可交换物品
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if(collision.tag=="Item")//碰到可触发物品转换模式
        {
            TransitionToState(useItemsState);
            idem = collision;
        }
    }

    //离开可交互物品
    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.tag == "Item")//离开可触发物品回到普通模式
        {
            TransitionToState(useBombState);
            idem = null;
        }
    }
    
    //站在平台上
    private void OnCollisionEnter2D(Collision2D coll)
    {
        if(coll.gameObject.tag=="Platform")
        {
            checkPlatform = coll.collider;
        }
    }
}
