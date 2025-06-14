using System.Collections;
using UnityEngine;

public class WizardController : MonoBehaviour
{
    private CapsuleCollider2D wizardCollider;
    private Animator anim;
    private float sideSign;
    private string side;

    public int life;
    public float speed;
    public Transform player;
    public GameObject range;

    public int maxLife = 12;

    [SerializeField] private BossLifeBar bossLifeBar;

    private bool isTakingDamage = false;
    [SerializeField] private float hurtCooldown = 0.5f;
    private int previousLife;

    [SerializeField] private float attackCooldown = 2.0f;
    private bool canAttack = true;

    // Durações das animações
    [SerializeField] private float attack1Duration = 1f;
    [SerializeField] private float attack2Duration = 1f;

    void Start()
    {
        // --- CORREÇÃO 1: Inicializando a vida e a barra de vida ---
        life = maxLife; // Define a vida inicial como a vida máxima
        previousLife = life;
        wizardCollider = GetComponent<CapsuleCollider2D>();
        anim = GetComponent<Animator>();
        anim.SetBool("IsWalk", false);

        // Diz para a barra de vida começar a funcionar para o Wizard
        if (bossLifeBar != null)
        {
            bossLifeBar.Initialize(this.transform, maxLife);
        }
    }

    void Update()
    {
        if (life <= 0)
        {
            this.enabled = false;
            wizardCollider.enabled = false;
            range.SetActive(false);
            anim.Play("Die", -1);
            anim.SetBool("IsWalk", false);
            TriggerEndDialogue();
            // Garante que a barra de vida apague ao morrer
            if (bossLifeBar != null) bossLifeBar.UpdateLifeBar(0, maxLife);
            return;
        }

        if (anim.GetCurrentAnimatorStateInfo(0).IsName("Attack1") ||
            anim.GetCurrentAnimatorStateInfo(0).IsName("Attack2") ||
            anim.GetCurrentAnimatorStateInfo(0).IsName("Hurt"))
        {
            anim.SetBool("IsWalk", false);
            return;
        }

        sideSign = Mathf.Sign(transform.position.x - player.position.x);

        if (Mathf.Abs(sideSign) == 1.0f)
        {
            side = sideSign == 1.0f ? "right" : "left";
        }

        switch (side)
        {
            case "right":
                transform.eulerAngles = new Vector3(0f, 180f, 0f);
                break;
            case "left":
                transform.eulerAngles = new Vector3(0f, 0f, 0f);
                break;
        }

        if (Vector2.Distance(transform.position, player.position) > 0.5f)
        {
            transform.position = Vector2.MoveTowards(transform.position,
                new Vector2(player.position.x, transform.position.y), speed * Time.deltaTime);
            anim.SetBool("IsWalk", true);
        }
        else
        {
            anim.SetBool("IsWalk", false);
        }
    }

    public void TakeDamage(int damage)
    {
        if (isTakingDamage || life <= 0) return;

        life -= damage;
        if (life < 0) life = 0; // Garante que a vida não seja negativa

        isTakingDamage = true;
        anim.SetTrigger("Hurt");
        StartCoroutine(ResetDamageState());

        // --- CORREÇÃO 2: Atualizando a barra de vida com a vida atual e máxima ---
        if (bossLifeBar != null)
        {
            bossLifeBar.UpdateLifeBar(life, maxLife);
        }
    }

    private IEnumerator ResetDamageState()
    {
        yield return new WaitForSeconds(hurtCooldown);
        isTakingDamage = false;
    }

    public void TriggerAttack()
    {
        if (canAttack && life > 0)
        {
            StartCoroutine(AttackSequence());
        }
    }

    private IEnumerator AttackSequence()
    {
        canAttack = false;

        // Primeiro ataque
        anim.Play("Attack1");
        yield return new WaitForSeconds(attack1Duration);

        // Segundo ataque
        anim.Play("Attack2");
        yield return new WaitForSeconds(attack2Duration);

        // Cooldown após sequência
        StartCoroutine(AttackCooldown());
    }

    private IEnumerator AttackCooldown()
    {
        yield return new WaitForSeconds(attackCooldown);
        canAttack = true;
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
            else
            {
                Debug.LogWarning("TutorialDialogue não encontrado dentro do Canvas!");
            }
        }
        else
        {
            Debug.LogWarning("Canvas não encontrado na cena!");
        }
    }
}