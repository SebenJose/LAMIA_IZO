using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerAttack : MonoBehaviour
{
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Demn"))
        {
            DemonController demon = collision.GetComponentInParent<DemonController>();
            if (demon != null)
            {
                demon.TakeDamage(1);
            }
        }

        if (collision.CompareTag("Wizard"))
        {
            WizardController wizard = collision.GetComponentInParent<WizardController>();
            if (wizard != null)
            {
                wizard.TakeDamage(1);
            }
        }

        if (collision.CompareTag("Death"))
        {
            DeathController death = collision.GetComponentInParent<DeathController>();
            if (death != null)
            {
                death.TakeDamage(1);
            }
        }
    }
}
