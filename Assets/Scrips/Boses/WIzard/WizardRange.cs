using UnityEngine;

public class WizardRange : MonoBehaviour
{
    private void OnTriggerStay2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            WizardController wizardController = GetComponentInParent<WizardController>();
            if (wizardController != null)
            {
                wizardController.TriggerAttack();
            }
        }
    }
}