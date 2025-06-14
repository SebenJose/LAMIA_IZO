using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class TransitionControler : MonoBehaviour
{
    [SerializeField] private Image transitionImage;
    [SerializeField] private float transitionDuration = 1f;

    public void TransitionToScene(string sceneName)
    {
        StartCoroutine(Transition(sceneName));
    }

    private IEnumerator Transition(string sceneName)
    {
        float t = 0f;
        Color c = transitionImage.color;
        while (t < transitionDuration)
        {
            t += Time.deltaTime;
            c.a = Mathf.Clamp01(t / transitionDuration);
            transitionImage.color = c;
            yield return null;
        }

        SceneManager.LoadScene(sceneName);
    }

    public void FadeIn()
    {
        StartCoroutine(FadeFromBlack());
    }

    private IEnumerator FadeFromBlack()
    {
        float t = 0f;
        Color c = transitionImage.color;

        c.a = 1f;
        transitionImage.color = c;

        while (t < transitionDuration)
        {
            t += Time.deltaTime;
            c.a = 1f - Mathf.Clamp01(t / transitionDuration);
            transitionImage.color = c;
            yield return null;
        }

        c.a = 0f;
        transitionImage.color = c;
    }

}
