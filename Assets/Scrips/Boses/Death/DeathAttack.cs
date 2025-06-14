using UnityEngine;

public class DeathAttack : MonoBehaviour
{
    [SerializeField] private int damage = 1;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            PlayerController player = collision.GetComponent<PlayerController>();
            if (player != null && !player.isDefending)
            {
                player.TakeDamage(damage);
            }
        }
    }
}