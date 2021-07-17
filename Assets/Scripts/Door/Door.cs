using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Door : MonoBehaviour
{
    Animator anim;
    BoxCollider2D coll;
    // Start is called before the first frame update
    void Start()
    {
        anim = GetComponent<Animator>();
        coll = GetComponent<BoxCollider2D>();
        coll.enabled = false;
        GameManager.instance.IsExitDoor(this);
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void OpenDoor()//开门
    {
        anim.Play("Door_Opening");
        coll.enabled = true;
    }

    private void OnTriggerStay2D(Collider2D collision)//进入下一房间
    {
        if (collision.CompareTag("Player"))
        {
            GameManager.instance.SaveData();
            GameManager.instance.NextLevel();
        }
    }
}
