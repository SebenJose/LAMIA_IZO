using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEngine;

public class SkeletonControler : MonoBehaviour
{
    private CapsuleCollider2D skeletonCollider;
    private Animator anim;
    private bool goRight;

    public GameObject Range;

    public int life;
    public float speed;

    public Transform a;
    public Transform b;

    void Start()
    {
        skeletonCollider = GetComponent<CapsuleCollider2D>();
        anim = GetComponent<Animator>();
    }

    // Update is called once per frame
    void Update()
    {
        if (life <= 0)
        {
            this.enabled = false;
            skeletonCollider.enabled = false;
            Range.SetActive(false);
            anim.Play("Death", -1);

        }

        if (anim.GetCurrentAnimatorStateInfo(0).IsName("Attack"))
        {
            PlayerController player = FindObjectOfType<PlayerController>();
            if(player != null && player.isDefending)
            {
                return;
            }
            else
            {
                return;
            }
        }

        if (goRight == true)
        {
            if(Vector2.Distance(transform.position, b.position) < 0.1f)
            {
                goRight = false;
            }
           
            transform.eulerAngles = new Vector3(0f, 0f, 0f); //direita
            transform.position = Vector2.MoveTowards(transform.position, b.position, speed * Time.deltaTime);

        }
        else
        {
            if(Vector2.Distance(transform.position, a.position) < 0.1f)
            {
                goRight = true;
            }
            transform.eulerAngles = new Vector3(0f, 180f, 0f); //esquerda
            transform.position = Vector2.MoveTowards(transform.position, a.position, speed * Time.deltaTime);
        }
    }
}
