using UnityEngine;
using System.Collections;

public class DeathController : MonoBehaviour
{
    private Animator anim;
    private CapsuleCollider2D deathCollider;
    public Transform player;

    [Header("Configurações de Status")]
    public int life;
    public int maxLife = 20;
    public float speed = 2f;

    [Header("Referências da UI")]
    [SerializeField] private BossLifeBar bossLifeBar;

    [Header("Configurações de Ataque Físico")]
    public float attackCooldown = 2f;
    public GameObject rangeObject;
    private float attackDistance = 1.5f;
    private bool canAttack = true;

    [Header("Configurações da Magia")]
    public GameObject spellPrefab;
    public float spellYOffset = 2f;
    [Tooltip("Distância a partir da qual o chefe prefere usar magia.")]
    public float spellRange = 5f;
    [Tooltip("Duração da animação/preparação da magia.")]
    public float spellCastTime = 1.5f;
    [Tooltip("Tempo de recarga da magia após ser usada.")]
    public float spellCooldown = 5f;
    private bool canCastSpell = true;
    private int physicalAttackCount = 0;

    [Header("Configurações de Combate")]
    [Tooltip("Tempo em segundos que o chefe espera no início da fase antes de atacar.")]
    public float initialCombatDelay = 3.0f;
    private bool isCombatActive = false;

    private enum BossState { Idle, Walk, Attack, Hurt, Die, Casting }
    private BossState currentState = BossState.Idle;
    private float hurtCooldown = 0.5f;

    void Start()
    {
        anim = GetComponent<Animator>();
        deathCollider = GetComponent<CapsuleCollider2D>();
    }

    void OnEnable()
    {
        ResetBossState();
    }

    void ResetBossState()
    {
        player = GameObject.FindGameObjectWithTag("Player").transform;

        bossLifeBar = FindObjectOfType<BossLifeBar>();
        if (bossLifeBar != null)
        {
            bossLifeBar.Initialize(this.transform, maxLife);
        }

        life = maxLife;
        currentState = BossState.Idle;
        canAttack = true;
        deathCollider.enabled = true;
        physicalAttackCount = 0;
        canCastSpell = true;
        isCombatActive = false;

        if (anim != null)
        {
            anim.Play("Idle");
            anim.SetBool("IsWalk", false);
        }

        StopAllCoroutines();
        StartCoroutine(StartCombatDelay());
    }

    IEnumerator StartCombatDelay()
    {
        yield return new WaitForSeconds(initialCombatDelay);
        isCombatActive = true;
    }

    void Update()
    {
        if (!isCombatActive)
        {
            return;
        }

        if (life <= 0 && currentState != BossState.Die)
        {
            ChangeState(BossState.Die);
            return;
        }

        if (currentState == BossState.Die || currentState == BossState.Hurt || currentState == BossState.Attack || currentState == BossState.Casting)
        {
            return;
        }

        DecideNextAction();
    }

    void DecideNextAction()
    {
        if (player == null) return;

        float distance = Vector2.Distance(transform.position, player.position);

        if (canCastSpell && (physicalAttackCount >= 2 || distance > spellRange))
        {
            StartCoroutine(SpellRoutine());
            return;
        }

        if (distance <= attackDistance && canAttack)
        {
            ChangeState(BossState.Attack);
            return;
        }

        if (distance > attackDistance)
        {
            ChangeState(BossState.Walk);
            UpdateRotation();
            Vector2 direction = (player.position - transform.position).normalized;
            transform.position += new Vector3(direction.x, 0, 0) * speed * Time.deltaTime;
        }
        else
        {
            ChangeState(BossState.Idle);
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
            case BossState.Hurt:
                StartCoroutine(HurtRoutine());
                break;
            case BossState.Die:
                isCombatActive = false;
                StopAllCoroutines();
                anim.Play("Die");
                deathCollider.enabled = false;
                if (rangeObject != null) rangeObject.SetActive(false);

                // LÓGICA DE MORTE COMPLETA E CORRIGIDA
                TriggerEndDialogue();
                if (bossLifeBar != null)
                {
                    bossLifeBar.UpdateLifeBar(0, maxLife);
                }
                break;
            case BossState.Casting:
                break;
        }
    }

    void UpdateRotation()
    {
        if (player == null) return;

        if (player.position.x > transform.position.x)
            transform.eulerAngles = new Vector3(0f, 180f, 0f);
        else
            transform.eulerAngles = new Vector3(0f, 0f, 0f);
    }

    IEnumerator AttackRoutine()
    {
        canAttack = false;
        anim.Play("Attack");
        yield return new WaitForSeconds(attackCooldown);
        physicalAttackCount++;
        canAttack = true;
        if (life > 0) ChangeState(BossState.Idle);
    }

    IEnumerator SpellRoutine()
    {
        canCastSpell = false;

        ChangeState(BossState.Casting);
        UpdateRotation();
        anim.Play("Cast");

        yield return new WaitForSeconds(spellCastTime);

        if (player != null)
        {
            Vector2 spellPos = new Vector2(player.position.x, player.position.y + spellYOffset);
            Instantiate(spellPrefab, spellPos, Quaternion.identity);
        }

        physicalAttackCount = 0;

        StartCoroutine(SpellCooldownManager());

        if (life > 0) ChangeState(BossState.Idle);
    }

    IEnumerator SpellCooldownManager()
    {
        yield return new WaitForSeconds(spellCooldown);
        canCastSpell = true;
    }

    public void TakeDamage(int damage)
    {
        if (!isCombatActive || currentState == BossState.Die || currentState == BossState.Hurt) return;

        life -= damage;
        if (life < 0) life = 0;

        if (bossLifeBar != null)
        {
            bossLifeBar.UpdateLifeBar(life, maxLife);
        }

        if (life > 0)
        {
            ChangeState(BossState.Hurt);
        }
    }

    IEnumerator HurtRoutine()
    {
        anim.SetTrigger("Hurt");
        anim.SetBool("IsWalk", false);
        yield return new WaitForSeconds(hurtCooldown);

        if (life > 0)
            ChangeState(BossState.Idle);
    }

    public void StartCombat()
    {
        isCombatActive = true;
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

    public void DeactivateOnDeath()
    {
        gameObject.SetActive(false);
    }
}