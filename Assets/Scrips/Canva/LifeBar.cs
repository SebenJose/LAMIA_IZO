using UnityEngine;
using UnityEngine.UI;

public class LifeBar : MonoBehaviour
{
    [SerializeField] private Image fillImage;
    [SerializeField] private PlayerController player;
    [SerializeField] private GameObject dialoguePanel;

    private Image backgroundImage;
    private Image heartImage;

    private void Start()
    {
        backgroundImage = GetComponent<Image>();
        heartImage = transform.Find("heart")?.GetComponent<Image>();

        UpdateLife(player.life);
        PlayerController.OnLifeChanged += UpdateLife;
    }

    private void OnDestroy()
    {
        PlayerController.OnLifeChanged -= UpdateLife;
    }

    private void UpdateLife(int currentLife)
    {
        float lifePercent = Mathf.Clamp01(currentLife / 5f);//considerando a vida maxima do player como 5
        fillImage.fillAmount = lifePercent;
    }

    private void Update()
    {
        bool shouldShow = !dialoguePanel.activeSelf;

        // Ativa/desativa os elementos visuais
        fillImage.enabled = shouldShow;
        backgroundImage.enabled = shouldShow;
        if (heartImage != null)
            heartImage.enabled = shouldShow;
    }

}
