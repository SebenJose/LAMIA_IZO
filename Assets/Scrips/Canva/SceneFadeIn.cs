using UnityEngine;

public class SceneFadeIn : MonoBehaviour
{
    private void Start()
    {
        TransitionControler tc = FindObjectOfType<TransitionControler>();
        if (tc != null)
        {
            tc.FadeIn();
        }
    }
}