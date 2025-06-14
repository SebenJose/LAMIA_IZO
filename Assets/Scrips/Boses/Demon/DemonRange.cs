using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DemonRange : MonoBehaviour
{
    private void OnTriggerStay2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            DemonController demonController = GetComponentInParent<DemonController>();

            if(demonController != null)
            {
                demonController.TriggerAttack();
            }
        }
    }
}
