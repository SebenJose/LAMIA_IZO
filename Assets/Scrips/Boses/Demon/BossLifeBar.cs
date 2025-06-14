using UnityEngine;
using UnityEngine.UI;

public class BossLifeBar : MonoBehaviour
{
    [Header("UI Components")]
    [SerializeField] private Image fillImage;
    [SerializeField] private CanvasGroup canvasGroup;

    [Header("Boss Reference")]
    [SerializeField] private Transform bossTransform; // Refer�ncia direta ao Transform do boss
    [SerializeField] private Vector3 positionOffset = new Vector3(0, 2f, 0);

    [Header("Visual Settings")]
    [SerializeField] private float smoothSpeed = 5f;
    [SerializeField] private bool hideWhenFull = true;

    private Camera mainCamera;
    private float targetFill;
    private bool isInitialized = false;

    private void Awake()
    {
        mainCamera = Camera.main;
        // Come�a invis�vel at� ser inicializada
        if (canvasGroup != null)
        {
            canvasGroup.alpha = 0;
        }
    }

    private void LateUpdate()
    {
        if (!isInitialized || bossTransform == null) return;

        // Atualiza a posi��o da barra de vida para seguir o boss
        if (mainCamera != null)
        {
            transform.position = mainCamera.WorldToScreenPoint(bossTransform.position + positionOffset);
        }

        // Suaviza a transi��o da barra de vida
        if (fillImage.fillAmount != targetFill)
        {
            fillImage.fillAmount = Mathf.Lerp(fillImage.fillAmount, targetFill, smoothSpeed * Time.deltaTime);
        }
    }

    // O script do Boss (ex: WizardController) deve chamar este m�todo no Start() dele.
    public void Initialize(Transform targetBoss, int maxHealth)
    {
        bossTransform = targetBoss;
        targetFill = 1f; // Come�a cheia
        fillImage.fillAmount = 1f;
        isInitialized = true;
        UpdateVisibility(maxHealth, maxHealth); // Usa o m�todo de visibilidade
    }

    // O script do Boss deve chamar este m�todo sempre que a vida dele mudar.
    public void UpdateLifeBar(int currentHealth, int maxHealth)
    {
        if (!isInitialized) return;

        if (maxHealth > 0)
        {
            targetFill = (float)currentHealth / maxHealth;
        }
        else
        {
            targetFill = 0;
        }

        UpdateVisibility(currentHealth, maxHealth);
    }

    private void UpdateVisibility(int currentHealth, int maxHealth)
    {
        if (canvasGroup == null) return;

        // A barra deve aparecer se a vida for menor que a m�xima (ou se hideWhenFull for falso) e maior que zero
        bool shouldShow = currentHealth > 0 && (!hideWhenFull || currentHealth < maxHealth);
        canvasGroup.alpha = shouldShow ? 1 : 0;
    }
}