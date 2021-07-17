using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy : MonoBehaviour
{
    EnemyBaseState currentState;
    public PatrolState patrolState = new PatrolState();
    public AttackState attackState = new AttackState();
    public Animator anim;
    public int animState;
    private GameObject alarmSign;

    [Header("Movement")]//移动相关
    public float speed;
    public Transform pointA, pointB;
    public Transform targetPoint;
    public List<Transform> attackList = new List<Transform>();

    [Header("Attack Setting")]//攻击相关
    public float attackRate;//攻击CD
    public float attackRange, skillRange;//攻击距离
    private float nextAttack = 0;

    [Header("Enemy State")]//状态相关
    public float maxHealth;
    public float health;
    public bool isDead;
    public bool hasBomb;
    public bool isBoss;

    public virtual void Init()
    {
        anim = GetComponent<Animator>();

        alarmSign = transform.GetChild(0).gameObject;

        maxHealth = health;
    }

    private void Awake()
    {
        Init();
    }

    // Start is called before the first frame update
    void Start()
    {
        TransitionToState(patrolState);//一开始执行巡逻模式
        if(isBoss)
        {
            UIManager.instance.SetBossHealth(health);
        }

        GameManager.instance.IsEnemy(this);
    }

    // Update is called once per frame
    public virtual void Update()
    {
        if(isBoss)//是BOSS刷新血条
        {
            UIManager.instance.bossHealthBar.transform.parent.gameObject.SetActive(true);
            UIManager.instance.UpdateBossHealth(health);
        }

        anim.SetBool("dead", isDead);

        if(isDead)
        {
            GameManager.instance.EnemyDead(this);
            return;
        }
        currentState.OnUpdate(this);
        anim.SetInteger("state", animState);
    }

    public void TransitionToState(EnemyBaseState state)//转换模式
    {
        currentState = state;
        currentState.EnterState(this);
    }

    public void MoveToTarget()//移动
    {
        transform.position = Vector2.MoveTowards(transform.position, targetPoint.position, speed * Time.deltaTime);
        FilpDirection();
    }

    public void AttackAction()//攻击玩家
    {
        if (Vector2.Distance(transform.position, targetPoint.position) <= attackRange)//目标在攻击距离之内
        {
            if (Time.time > nextAttack)//CD冷却已好
            {
                anim.SetTrigger("attack");//播放攻击动画
                nextAttack = Time.time + attackRate;//刷新CD
            }
        }
    }

    public virtual void SkillAction()//对炸弹使用技能  
    {
        if (Vector2.Distance(transform.position, targetPoint.position) <= skillRange)//目标在攻击距离之内
        {
            if (Time.time >= nextAttack)//CD冷却已好
            {
                anim.SetTrigger("skill");//播放技能动画
                nextAttack = Time.time + attackRate;//刷新CD
            }
        }
    }

    public void FilpDirection()//控制面向
    {
        if (transform.position.x < targetPoint.position.x)
        {
            transform.rotation = Quaternion.Euler(0f, 0f, 0f);
        }
        else
        {
            transform.rotation = Quaternion.Euler(0f, 180f, 0f);
        }
    }

    public void SwitchPoint()//切换目标点
    {
        if (Mathf.Abs(pointA.position.x - transform.position.x) > Mathf.Abs(pointB.position.x - transform.position.x))
        {
            targetPoint = pointA;
        }
        else
        {
            targetPoint = pointB;
        }
    }

    private void OnTriggerStay2D(Collider2D collision)
    {
        if (!attackList.Contains(collision.transform) && !hasBomb && !isDead && !GameManager.instance.gameOver && collision.tag != "Item")
        {
            attackList.Add(collision.transform); 
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        attackList.Remove(collision.transform);
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (!isDead && !GameManager.instance.gameOver && collision.tag != "Item")
        {
            StartCoroutine(OnAlarm());
        }
    }

    IEnumerator OnAlarm()//警告标识闪烁
    {
        alarmSign.SetActive(true);
        yield return new WaitForSeconds(alarmSign.GetComponent<Animator>().GetCurrentAnimatorClipInfo(0)[0].clip.length);
        alarmSign.SetActive(false);
    }

    public void OnDrawGizmos()
    {
        Gizmos.DrawWireSphere(transform.position, attackRange);
        Gizmos.DrawWireSphere(transform.position, skillRange);
    }
}
