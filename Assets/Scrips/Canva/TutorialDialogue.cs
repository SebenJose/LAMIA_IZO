using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class TutorialDialogue : MonoBehaviour
{
    public GameObject panel;
    public TMP_Text dialogueText;

    [TextArea(3, 5)]
    public string[] messages;

    private int currentIndex = 0;

    [SerializeField] private TransitionControler transitionControler;

    void Start()
    {
        // Verifica se é uma cena que deve iniciar o diálogo automaticamente
        if ((SceneManager.GetActiveScene().name == "Introducion" || SceneManager.GetActiveScene().name == "Final") && messages.Length > 0)
        { 
            StartCoroutine(StartIntroDialogue());
        }
        else
        {
            panel.SetActive(false);
        }
    }

    void Update()
    {
        if (panel.activeSelf && Input.GetKeyDown(KeyCode.Return))
        {
            currentIndex++;

            if (currentIndex < messages.Length)
            {
                dialogueText.text = messages[currentIndex];
            }
            else
            {
                panel.SetActive(false);

                string currentScene = SceneManager.GetActiveScene().name;

                switch (currentScene)
                {
                    case "Introducion":
                        transitionControler.TransitionToScene("Fase_1");
                        break;
                    case "Fase_1":
                        transitionControler.TransitionToScene("Fase_2");
                        break;
                    case "Fase_2":
                        transitionControler.TransitionToScene("Fase_3");
                        break;
                    case "Fase_3":
                        transitionControler.TransitionToScene("Final");
                        break;
                    case "Final":
                        transitionControler.TransitionToScene("SceneFinal");
                        break;
                }
            }
        }
    }

    private IEnumerator StartIntroDialogue()
    {
        yield return new WaitForSeconds(0.5f); // Pequeno delay
        panel.SetActive(true);
        currentIndex = 0;
        dialogueText.text = messages[currentIndex];
    }

    public void StartDialogue(string[] newMessages)
    {
        messages = newMessages;
        currentIndex = 0;
        panel.SetActive(true);
        dialogueText.text = messages[currentIndex];
    }

    public void TriggerDialogueFromInspector()
    {
        if (messages.Length > 0)
        {
            currentIndex = 0;
            panel.SetActive(true);
            dialogueText.text = messages[currentIndex];
        }
    }
}
