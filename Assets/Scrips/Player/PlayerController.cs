using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class PlayerController : MonoBehaviour
{
    private Rigidbody2D rb;
    private Animator anim;
    private CapsuleCollider2D colliderPlayer;
    private float moveX;

    //variavies para o combo de ataque
    private int comboStep = 0;
    private float comboTimer = 0f;
    [SerializeField] private float comboWindow = 0.5f;
    private bool canCombo = false;
    private bool comboQueued = false;

    //variaveis para a defesa
    public bool isDefending;
    [SerializeField] private float defenseCooldown = 0.5f; //esta alterado na unity cooldown maior para balanceamento
    private float defenseTimer = 0f;

    //player variables
    public float speed;
    public int addJump;
    public bool isGrounded;
    public float jumpForce;
    public int life;
    private bool jumpPressed = false;

    private int previousLife;
    private bool isTakingDamage = false;

    private bool isDead = false;

    //variaveis para o dash
    [SerializeField] private float dashForce = 50;
    [SerializeField] private float dashDuration = 0.25f;
    [SerializeField] private float dashCooldown = 1f;
    //[SerializeField] private float doubleTapTime = 0.3f;
    private bool isDashing = false;
    [SerializeField] private float dashCooldownTimer = 0f;
    //[SerializeField] private int dashDirection = 1;
    private float originalGravity;

    //Canva
    public delegate void LifeChangedDelegate(int newLife);
    public static event LifeChangedDelegate OnLifeChanged;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        anim = GetComponentInChildren<Animator>();
        rb.gravityScale = rb.gravityScale;
        colliderPlayer = GetComponent<CapsuleCollider2D>();

        previousLife = life;
        OnLifeChanged?.Invoke(life);
    }


    void Update()
    {

        if (isDead) return;

        moveX = Input.GetAxisRaw("Horizontal");

        if (life <= 0)
        {
            Die();
            return;
        }

        jumpPressed = Input.GetButtonDown("Jump"); //vai capturar o pulo

        //ataque
        if (comboStep > 0)
        {
            comboTimer -= Time.deltaTime;
            if (comboTimer <= 0f)
            {
                ResetCombo();
            }
        }

        if (Input.GetButtonDown("Fire1"))
        {
            if (isDashing) return;

            if (canCombo)
            {
                comboQueued = true;
            }
            else if (comboStep == 0)
            {
                StartCombo();
            }
        }

        //defesa
        if (Input.GetMouseButton(1))
        {
            if (defenseTimer <= 0 && !isDefending && isGrounded)
            {
                StartDefense();
            }
        }
        else if (isDefending)
        {
            StopDefense();
        }

        if (defenseTimer > 0)
        {
            defenseTimer -= Time.deltaTime;
        }

        //dash
        if (Input.GetKeyDown(KeyCode.LeftAlt))
        {

            if (Input.GetKey(KeyCode.A))
            {
                DashNaDirecaoAtual();
            }
            else if (Input.GetKey(KeyCode.D))
            {
                DashNaDirecaoAtual();
            }
        }

        // Atualiza o cooldown
        if (dashCooldownTimer > 0)
        {
            dashCooldownTimer -= Time.deltaTime;
        }

        if (!anim.GetCurrentAnimatorStateInfo(0).IsName("Attacking") &&
        !anim.GetCurrentAnimatorStateInfo(0).IsName("Attacking2"))
        {
            if (comboStep > 0 && comboTimer <= 0f)
                ResetCombo();
        }

        if (comboStep > 0 && comboTimer <= 0f)
        {
            ResetCombo();
        }

        if (life < previousLife && life > 0)
        {
            anim.SetTrigger("Hurt");
            StartCoroutine(ResetDamageState());
        }

        previousLife = life;

    }

    private void FixedUpdate()
    {
        Move();

        if (isDefending || isDead) return;

        if (jumpPressed)
        {
            if (isGrounded)
            {
                //addJump = 1;
                Jump();
            }
            else if (addJump > 0)
            {
                addJump--;
                Jump();
            }
        }

        jumpPressed = false; // reseta o input após usar
}

    void Move()
    {
        if(isDashing || isDead) return;

        bool isRunning = Input.GetKey(KeyCode.LeftShift) && moveX != 0;
        float currentSpeed = isRunning ? speed * 2 : speed;

        rb.velocity = new Vector2(moveX * currentSpeed, rb.velocity.y);


        //altera a direção do personagem
        if (moveX > 0)
        {
            transform.eulerAngles = new Vector3(0f, 0f, 0f); //direita
        }
        else if (moveX < 0)
        {
            transform.eulerAngles = new Vector3(0f, 180f, 0f); //esquerda
        }

        anim.SetBool("IsWalk", moveX != 0 && !isRunning);
        anim.SetBool("IsRun", isRunning);
    }

    void Jump()
    {
        if (isDead) return;

        rb.velocity = new Vector2(rb.velocity.x, jumpForce);
        
        anim.Play("Jump", 0, 0f);

        anim.SetBool("IsJump", true);

        ResetCombo();

    }

    void Attack()
    {

        if (isDashing) return;

        if (comboStep == 0 || comboTimer > 0f)
        {
            if(comboStep >= 2)
            {
                return; // Limita o número de ataques no combo
            }

            comboStep++;
            comboTimer = comboWindow;
            anim.SetInteger("ComboStep", comboStep);

            string attackAnimation = "";

            switch (comboStep)
            {
                case 1:
                    attackAnimation = "Attacking";
                    break;

                case 2:
                    attackAnimation = "Attacking2";
                    break;

            }

            if (!string.IsNullOrEmpty(attackAnimation))
            {
                anim.Play(attackAnimation, -1, 0f);
            }
        }
    }

    void StartCombo()
    {
        comboStep = 1;
        comboTimer = comboWindow;
        anim.SetInteger("ComboStep", comboStep);
        anim.Play("Attacking", -1, 0f);
    }

    public void EnableCombo()
    {
        canCombo = true;
    }
    void ResetCombo()
    {
        comboStep = 0;
        comboTimer = 0f;
        canCombo = false;
        comboQueued = false;
        anim.SetInteger("ComboStep", comboStep);
    }
    public void TryContinueCombo()
    {

        canCombo = false;

        if (comboQueued)
        {

            comboQueued = false;
            comboStep++;
            comboTimer = comboWindow;

            anim.SetInteger("ComboStep", comboStep);

            string nextAnimation = "";

            switch (comboStep)
            {
                case 2:
                    nextAnimation = "Attacking2";
                    break;
                default:
                    ResetCombo();
                    return;
            }

            anim.Play(nextAnimation, -1, 0f);
        }
        else
        {
            ResetCombo();
        }
    }

    void StartDefense()
    {
        isDefending = true;
        anim.SetBool("IsDefend", true);
        speed = speed / 2;
        ResetCombo ();
    }

    void StopDefense()
    {
        isDefending = false;
        anim.SetBool("IsDefend", false);
        speed = speed * 2;
        defenseTimer = defenseCooldown;
    }
    void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.tag == "Ground")
        {
            isGrounded = true;
            anim.SetBool("IsJump", false);

            addJump = 1;
        }
    }

    void TryDash (int direction)
    {
        if (dashCooldownTimer <= 0 && !isDashing)
        {
            int facingDirection = (transform.eulerAngles.y == 0) ? 1 : -1;
            StartCoroutine(Dash(facingDirection));
        }
    }

    IEnumerator Dash(int direcao)
    {
        isDashing = true;
        anim.Play("Dash");

        ResetCombo();

        originalGravity = rb.gravityScale;
        float velocidadeOriginal = speed;

        rb.gravityScale = 0;
        speed = 0;
        rb.velocity = new Vector2(dashForce * direcao, 0);

        yield return new WaitForSeconds(dashDuration);

        rb.gravityScale = originalGravity;
        speed = velocidadeOriginal;
        isDashing = false;
        dashCooldownTimer = dashCooldown;
    }

    public void TakeDamage(int damage)
    {
        if (life < 0 || isTakingDamage || isDefending) return;

        life -= damage;
        OnLifeChanged?.Invoke(life);
        isTakingDamage = true;
        anim.SetTrigger("Hurt");
        StartCoroutine(ResetDamageState());
    }

    private IEnumerator ResetDamageState()
    {
        isTakingDamage = true;
        yield return new WaitForSeconds(0.5f);
        isTakingDamage = false;
    }
    
    private void Die()
    {
        if (isDead) return;
        isDead = true;
        anim.SetTrigger("Die");
        colliderPlayer.enabled = false;
        rb.gravityScale = 0;
        rb.velocity = Vector2.zero;
        //this.enabled = false;
    }

    void DashNaDirecaoAtual()
    {
        if (isDead) return;

        if (dashCooldownTimer <= 0 && !isDashing)
        {
            int direcao = (transform.eulerAngles.y == 0) ? 1 : -1;
            StartCoroutine(Dash(direcao));
        }
    }

    void OnCollisionExit2D(Collision2D collision)
    {
        if (collision.gameObject.tag == "Ground")
        {
            isGrounded = false;
        }
    }
}
