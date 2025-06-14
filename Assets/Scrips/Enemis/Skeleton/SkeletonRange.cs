using UnityEngine;

public class SkeletonRange : MonoBehaviour
{
    private Animator skeletonAnimator;
    private bool playerInRange = false;
    private bool isAttacking = false;

    void Start()
    {
        skeletonAnimator = GetComponentInParent<Animator>();
    }

    void Update()
    {
        if (playerInRange && !skeletonAnimator.GetCurrentAnimatorStateInfo(0).IsName("Attack"))
        {
            skeletonAnimator.Play("Attack");
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            playerInRange = true;
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            isAttacking = false;
        }
    }
}