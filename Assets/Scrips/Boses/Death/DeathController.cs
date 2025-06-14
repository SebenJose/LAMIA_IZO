using UnityEngine;
using System.Collections;

public class DeathController : MonoBehaviour
{
    private Animator anim;
    private CapsuleCollider2D deathCollider;
    public Transform player;

    [Header("Configurações de Status")]
    public int life;
    public int maxLife = 20; // Vida máxima
    public float speed = 2f;

    [Header("Referências da UI")]
    [SerializeField] private BossLifeBar bossLifeBar; // Referência para a barra de vida

    [Header("Configurações de Ataque")]
    public float attackCooldown = 2f;
    public GameObject rangeObject;
    public GameObject spellPrefab;
    public float spellYOffset = 2f;

    private int attackCount = 0;
    private bool canAttack = true;

    private enum BossState { Idle, Walk, Attack, Cast, Hurt, Die }
    private BossState currentState = BossState.Idle;

    private float hurtCooldown = 0.5f;
    private float attackDistance = 1.5f;

    void Start()
    {
        anim = GetComponent<Animator>();
        deathCollider = GetComponent<CapsuleCollider2D>();
        player = GameObject.FindGameObjectWithTag("Player").transform;

        life = maxLife;
        if (bossLifeBar != null)
        {
            bossLifeBar.Initialize(this.transform, maxLife);
        }
    }
    void Update()
    {
        if (life <= 0 && currentState != BossState.Die)
        {
            ChangeState(BossState.Die);
            return;
        }

        switch (currentState)
        {
            case BossState.Idle:
                if (PlayerInRange()) ChangeState(BossState.Walk);
                break;

            case BossState.Walk:
                WalkToPlayer();
                break;

            case BossState.Attack:
            case BossState.Cast:
            case BossState.Hurt:
            case BossState.Die:
                // Ações já são tratadas por coroutines ou no ChangeState
                break;
        }
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

            case BossState.Walk:
                anim.SetBool("IsWalk", true);
                break;

            case BossState.Attack:
                StartCoroutine(AttackRoutine());
                break;

            case BossState.Cast:
                StartCoroutine(CastRoutine());
                break;

            case BossState.Hurt:
                StartCoroutine(HurtRoutine());
                break;

            case BossState.Die:
                anim.Play("Die");
                deathCollider.enabled = false;
                if (rangeObject != null) rangeObject.SetActive(false);
                enabled = false; // Desabilita o script

                if (bossLifeBar != null)
                {
                    bossLifeBar.UpdateLifeBar(0, maxLife);
                }
                break;
        }
    }

    void WalkToPlayer()
    {
        if (currentState != BossState.Walk) return;

        float distance = Vector2.Distance(transform.position, player.position);

        if (distance > attackDistance)
        {
            UpdateRotation();
            Vector2 direction = (player.position - transform.position).normalized;
            // Movimento apenas horizontal
            transform.position += new Vector3(direction.x, 0, 0) * speed * Time.deltaTime;
        }
        else if (canAttack)
        {
            anim.SetBool("IsWalk", false);
            attackCount++;
            ChangeState(attackCount % 2 == 0 ? BossState.Cast : BossState.Attack);
        }
    }

    void UpdateRotation()
    {
        if (player.position.x > transform.position.x)
            transform.eulerAngles = new Vector3(0f, 180f, 0f); // Olhando para a direita
        else
            transform.eulerAngles = new Vector3(0f, 0f, 0f); // Olhando para a esquerda
    }

    bool PlayerInRange()
    {
        return Vector2.Distance(transform.position, player.position) < 8f;
    }

    public void StartCombat()
    {
        if (currentState == BossState.Idle)
        {
            ChangeState(BossState.Walk);
        }
    }

    IEnumerator AttackRoutine()
    {
        canAttack = false;
        anim.Play("Attack");
        yield return new WaitForSeconds(attackCooldown);
        canAttack = true;
        ChangeState(BossState.Idle);
    }

    IEnumerator CastRoutine()
    {
        canAttack = false;
        anim.Play("Cast");
        yield return new WaitForSeconds(1f);

        // *** LINHA CORRIGIDA: Voltando para a altura original fixa ***
        Vector2 spellPos = new Vector2(player.position.x, 1.4f); // altura do chão
        Instantiate(spellPrefab, spellPos, Quaternion.identity);

        yield return new WaitForSeconds(attackCooldown);
        canAttack = true;
        ChangeState(BossState.Idle);
    }

    public void TakeDamage(int damage)
    {
        if (currentState == BossState.Die || currentState == BossState.Hurt) return;

        life -= damage;
        if (life < 0) life = 0;

        // --- CORREÇÃO 2: Atualizando a barra de vida ---
        if (bossLifeBar != null)
        {
            bossLifeBar.UpdateLifeBar(life, maxLife);
        }

        ChangeState(BossState.Hurt);
    }

    IEnumerator HurtRoutine()
    {
        anim.SetTrigger("Hurt");
        yield return new WaitForSeconds(hurtCooldown);

        if (life > 0)
            ChangeState(BossState.Idle);
    }

    private void TriggerEndDialogue()
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