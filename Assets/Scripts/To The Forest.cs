using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class ToTheForest : MonoBehaviour
{
    public Button goForest;
    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            goForest.gameObject.SetActive(true);
        }
    }
    private void OnCollisionExit(Collision collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            goForest.gameObject.SetActive(false);
        }
    }
    public void GoHome()
    {
        SceneManager.LoadScene(0);
    }
}

