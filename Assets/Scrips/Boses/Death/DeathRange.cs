using UnityEngine;

public class DeathRange : MonoBehaviour
{
    private void OnTriggerStay2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            DeathController controller = GetComponentInParent<DeathController>();
            if (controller != null)
            {
                controller.StartCombat();
            }
        }
    }
}