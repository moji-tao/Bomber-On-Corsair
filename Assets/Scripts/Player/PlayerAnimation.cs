using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerAnimation : MonoBehaviour
{
    private Animator anim;
    private Rigidbody2D rb;
    private PlayerControl control;

    // Start is called before the first frame update
    void Start()
    {
        anim = GetComponent<Animator>();
        rb = GetComponent<Rigidbody2D>();
        control = GetComponent<PlayerControl>();
    }

    // Update is called once per frame
    void Update()
    {
        anim.SetFloat("velocityY", rb.velocity.y);
        anim.SetFloat("speed", Mathf.Abs(rb.velocity.x));
        anim.SetBool("jump", control.isJump);
        anim.SetBool("ground", control.isGround);
    }
}
