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

    [Header("Configurações de Ataque")]
    public float attackCooldown = 2f; // Cooldown do ataque físico
    public GameObject rangeObject;
    private float attackDistance = 1.5f;
    private bool canAttack = true;

    [Header("Configurações da Magia Global")]
    public GameObject spellPrefab;
    public float spellYOffset = 2f;
    [SerializeField] private float globalSpellCooldown = 8f; // Tempo entre cada magia
    [SerializeField] private float initialSpellDelay = 4f;  // Tempo até a primeira magia

    private enum BossState { Idle, Walk, Attack, Hurt, Die, Casting }
    private BossState currentState = BossState.Idle;
    private float hurtCooldown = 0.5f;

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

        // Inicia a rotina da magia global
        StartCoroutine(GlobalSpellRoutine());
    }

    void Update()
    {
        if (life <= 0 && currentState != BossState.Die)
        {
            ChangeState(BossState.Die);
            return;
        }

        // Ignora a lógica principal se estiver em um estado que não permite interrupção
        if (currentState == BossState.Die || currentState == BossState.Hurt || currentState == BossState.Attack || currentState == BossState.Casting)
        {
            return;
        }

        if (currentState == BossState.Idle)
        {
            ChangeState(BossState.Walk);
        }

        if (currentState == BossState.Walk)
        {
            WalkToPlayer();
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
                StopAllCoroutines();
                anim.Play("Die");
                deathCollider.enabled = false;
                if (rangeObject != null) rangeObject.SetActive(false);
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

    void WalkToPlayer()
    {
        if (currentState != BossState.Walk) return;

        float distance = Vector2.Distance(transform.position, player.position);

        if (distance > attackDistance)
        {
            UpdateRotation();
            Vector2 direction = (player.position - transform.position).normalized;
            transform.position += new Vector3(direction.x, 0, 0) * speed * Time.deltaTime;
        }
        else if (canAttack) // Se chegar perto, usa o ataque físico
        {
            ChangeState(BossState.Attack);
        }
    }

    void UpdateRotation()
    {
        if (player.position.x > transform.position.x)
            transform.eulerAngles = new Vector3(0f, 180f, 0f); // Direita
        else
            transform.eulerAngles = new Vector3(0f, 0f, 0f); // Esquerda
    }

    // A função PlayerInRange não é mais usada
    bool PlayerInRange()
    {
        if (player == null) return false; 
        return Vector2.Distance(transform.position, player.position) < 8f; 
    }

    public void StartCombat()
    {
        if (currentState == BossState.Idle)
        {
            ChangeState(BossState.Walk);
        }
    }

    IEnumerator AttackRoutine() // Rotina do ataque físico
    {
        canAttack = false; 
        anim.Play("Attack"); 
        yield return new WaitForSeconds(attackCooldown);
        canAttack = true;
        ChangeState(BossState.Idle);
    }

    IEnumerator GlobalSpellRoutine()
    {
        yield return new WaitForSeconds(initialSpellDelay); 

        while (life > 0)
        {
            yield return new WaitForSeconds(globalSpellCooldown);

            if (currentState == BossState.Idle || currentState == BossState.Walk)
            {
                BossState stateBeforeCast = currentState;
                ChangeState(BossState.Casting);
                anim.Play("Cast");

                yield return new WaitForSeconds(1f);

                if (player != null)
                {
                    Vector2 spellPos = new Vector2(player.position.x, player.position.y + spellYOffset);
                    Instantiate(spellPrefab, spellPos, Quaternion.identity); 
                }

                yield return new WaitForSeconds(0.5f);

                ChangeState(stateBeforeCast);
            }
        }
    }

    public void TakeDamage(int damage)
    {
        if (currentState == BossState.Die || currentState == BossState.Hurt) return;
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
        BossState stateBeforeHurt = currentState;
        anim.SetBool("IsWalk", false);
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

    public void DeactivateOnDeath()
    {
        gameObject.SetActive(false);
    }
}