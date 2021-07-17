using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Heart : MonoBehaviour
{
    public Animator anim;
    // Start is called before the first frame update
    public void Start()
    {
        anim = GetComponent<Animator>();
    }

    // Update is called once per frame
    public void Update()
    {
        
    }
    private void OnTriggerStay2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            if (collision.GetComponent<PlayerControl>().health < 3)
            {
                collision.GetComponent<PlayerControl>().GetHit(-1);
                anim.Play("Heart_Live+1");
            }
        }
    }

    public void Eat()
    {
        Destroy(gameObject);
    }
}
