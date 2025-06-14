using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static UnityEngine.Tilemaps.Tile;

public class DemonController : MonoBehaviour
{
    private CapsuleCollider2D demonCollider;
    private Animator anim;
    private float sideSign;
    private string side;

    //variaveis de controle e status
    public int life;
    public float speed;
    public Transform player;
    public GameObject range;
    public int maxLife = 10;

    [SerializeField] private BossLifeBar bossLifeBar;

    //variaveis para a animação de hurt
    private bool isTakingDamage = false;
    [SerializeField] private float hurtCooldown = 0.5f;
    private int previousLife;

    [SerializeField] private float attackCooldown = 2.0f;
    private bool canAttack = true;

    void Start()
    {
        // --- MUDANÇA 1: INICIALIZAR A VIDA E A BARRA DE VIDA ---
        life = maxLife; // Garante que o boss comece com a vida cheia
        previousLife = life;
        demonCollider = GetComponent<CapsuleCollider2D>();
        anim = GetComponent<Animator>();
        anim.SetBool("IsWalk", false);

        // Inicializa a barra de vida com os dados deste boss
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
            demonCollider.enabled = false;
            range.SetActive(false);
            anim.Play("Die", -1);
            anim.SetBool("IsWalk", false);

            TriggerEndDialogue();
            // Garante que a barra de vida desapareça quando o boss morre
            if (bossLifeBar != null) bossLifeBar.UpdateLifeBar(0, maxLife);
            return; // Impede que o resto do Update execute após a morte
        }

        previousLife = life;

        if (anim.GetCurrentAnimatorStateInfo(0).IsName("Attack") || anim.GetCurrentAnimatorStateInfo(0).IsName("Hurt"))
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
                transform.eulerAngles = new Vector3(0f, 0f, 0f); //direita
                break;

            case "left":
                transform.eulerAngles = new Vector3(0f, 180f, 0f); //esquerda
                break;
        }

        if (Vector2.Distance(transform.position, player.position) > 0.5f)
        {
            transform.position = Vector2.MoveTowards(transform.position, new Vector2(player.position.x, transform.position.y), speed * Time.deltaTime);
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
        if (life < 0) life = 0; // Garante que a vida não fique negativa

        isTakingDamage = true;
        anim.SetTrigger("Hurt");
        StartCoroutine(ResetDamageState());

        // --- MUDANÇA 2: ATUALIZAR A BARRA DE VIDA COM OS DOIS PARÂMETROS ---
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
            anim.Play("Attack", -1);
            StartCoroutine(AttackCooldown());
        }
    }

    private IEnumerator AttackCooldown()
    {
        canAttack = false;
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
        }
    }
}