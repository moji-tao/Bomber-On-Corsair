using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HitPoint : MonoBehaviour
{
    public bool bombAvilable;
    int dir;
    public float power;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (transform.position.x > other.transform.position.x)
        {
            dir = -1;
        }
        else
        { 
            dir = 1; 
        }

        if(other.CompareTag("Player"))
        {
            other.GetComponent<IDamageable>().GetHit(power);//对玩家造成伤害
            other.gameObject.GetComponent<Rigidbody2D>().AddForce(new Vector2(dir, 1) * 10, ForceMode2D.Impulse);//击飞
        }
        
        /*if (other.CompareTag("Bomb") && bombAvilable)//海盗踢炸弹
        {
            other.gameObject.GetComponent<Rigidbody2D>().AddForce(new Vector2(dir, 1) * 10, ForceMode2D.Impulse);
        }*/
    }
}
