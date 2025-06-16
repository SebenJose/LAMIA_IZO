using UnityEngine;
using System.Collections;

public class WizardController : MonoBehaviour
{
    private enum BossState { Idle, Chase, Attack, Hurt, Dead }
    private BossState currentState;

    private Animator anim;
    private CapsuleCollider2D wizardCollider;
    public Transform player;

    public int maxLife = 12;
    private int life;

    public float speed = 2f;

    public float attackCooldown = 2f;
    public float attackDistance = 1.5f;

    private bool canAttack = true;
    private bool isTakingDamage = false;

    private bool isNextAttackOne = true;

    [SerializeField] private float hurtCooldown = 0.5f;
    [SerializeField] private BossLifeBar bossLifeBar;

    void Start()
    {
        anim = GetComponent<Animator>();
        wizardCollider = GetComponent<CapsuleCollider2D>();
        life = maxLife;

        if (player == null)
        {
            player = GameObject.FindGameObjectWithTag("Player").transform;
        }

        if (bossLifeBar != null)
        {
            bossLifeBar.Initialize(this.transform, maxLife);
        }

        ChangeState(BossState.Chase);
    }

    void Update()
    {
        if (player == null || currentState == BossState.Dead || currentState == BossState.Hurt || currentState == BossState.Attack)
        {
            return;
        }

        switch (currentState)
        {
            case BossState.Idle:
                if (Vector2.Distance(transform.position, player.position) > attackDistance)
                {
                    ChangeState(BossState.Chase);
                }
                else if (canAttack)
                {
                    ChangeState(BossState.Attack);
                }
                break;

            case BossState.Chase:
                ChasePlayer();
                if (Vector2.Distance(transform.position, player.position) <= attackDistance)
                {
                    ChangeState(BossState.Idle);
                }
                break;
        }
    }

    void UpdateRotation()
    {
        if (player == null) return;

        if (player.position.x > transform.position.x)
            transform.eulerAngles = new Vector3(0f, 0f, 0f);
        else
            transform.eulerAngles = new Vector3(0f, 180f, 0f);
    }

    void ChangeState(BossState newState)
    {
        if (currentState == newState) return;
        currentState = newState;

        switch (newState)
        {
            case BossState.Idle:
                anim.SetBool("IsWalk", false);
                break;
            case BossState.Chase:
                anim.SetBool("IsWalk", true);
                break;
            case BossState.Attack:
                StartCoroutine(AttackRoutine());
                break;
            case BossState.Hurt:
                StartCoroutine(HurtRoutine());
                break;
            case BossState.Dead:
                life = 0;
                anim.Play("Die");
                wizardCollider.enabled = false;
                StopAllCoroutines();
                TriggerEndDialogue();
                break;
        }
    }

    void ChasePlayer()
    {
        if (player == null) return;
        UpdateRotation();
        transform.position += new Vector3((player.position - transform.position).normalized.x, 0, 0) * speed * Time.deltaTime;
    }

    public void TakeDamage(int damage)
    {
        if (isTakingDamage || currentState == BossState.Dead) return;

        life -= damage;
        if (bossLifeBar != null)
            bossLifeBar.UpdateLifeBar(life, maxLife);

        if (life <= 0)
        {
            ChangeState(BossState.Dead);
        }
        else
        {
            ChangeState(BossState.Hurt);
        }
    }

    IEnumerator HurtRoutine()
    {
        isTakingDamage = true;
        anim.SetTrigger("Hurt");
        yield return new WaitForSeconds(hurtCooldown);
        isTakingDamage = false;

        if (currentState != BossState.Dead)
        {
            ChangeState(BossState.Idle);
        }
    }

    IEnumerator AttackRoutine()
    {
        canAttack = false;
        UpdateRotation();

        if (isNextAttackOne)
        {
            anim.SetTrigger("Attack1");
        }
        else
        {
            anim.SetTrigger("Attack2");
        }

        isNextAttackOne = !isNextAttackOne;

        yield return new WaitForSeconds(attackCooldown);
        canAttack = true;

        if (currentState != BossState.Dead)
        {
            ChangeState(BossState.Idle);
        }
    }

    void TriggerEndDialogue()
    {
        Canvas canvas = FindObjectOfType<Canvas>();
        if (canvas != null)
        {
            TutorialDialogue dialogue = canvas.GetComponentInChildren<TutorialDialogue>(true);
            if (dialogue != null)
            {
                dialogue.TriggerDialogueFromInspector();
            }
        }
    }
}