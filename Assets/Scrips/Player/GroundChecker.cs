using UnityEngine;

public class GroundChecker : MonoBehaviour
{
    public PlayerController player;

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.gameObject.layer == LayerMask.NameToLayer("Ground"))
        {
            player.isGrounded = true;
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.gameObject.layer == LayerMask.NameToLayer("Ground"))
        {
            player.isGrounded = false;
        }
    }
}