using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DemanAttack : MonoBehaviour
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
