using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class HudController : MonoBehaviour
{
    public TextMeshProUGUI textLife;

    void OnEnable()
    {
        PlayerController.OnLifeChanged += AtualizarVida;
    }

    void OnDisable()
    {
        PlayerController.OnLifeChanged -= AtualizarVida;
    }

    void AtualizarVida(int novaVida)
    {
        textLife.text = novaVida.ToString();
    }
}
