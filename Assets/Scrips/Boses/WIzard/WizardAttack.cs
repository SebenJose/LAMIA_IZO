using UnityEngine;

public class WizardAttack : MonoBehaviour
{
    [SerializeField] private int damageAmount = 1;

    void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            PlayerController player = collision.GetComponent<PlayerController>();
            if (player != null && !player.isDefending)
            {
                player.TakeDamage(damageAmount);
            }
        }
    }
}