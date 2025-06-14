using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MantemCanvas : MonoBehaviour
{

    private static MantemCanvas instance;

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }
}
