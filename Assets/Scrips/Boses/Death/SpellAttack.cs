using UnityEngine;

public class SpellAttack : MonoBehaviour
{
    [SerializeField] private int damage = 2;
    [SerializeField] private float lifetime = 2f; // tempo até desaparecer

    private void Start()
    {
        Destroy(gameObject, lifetime); // destrói depois que a animação acabar
    }

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
