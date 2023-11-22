using System.Collections;
using System.Collections.Generic;
using UnityEditor.Build.Reporting;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class BackToTown : MonoBehaviour
{
    public Button goHome;
    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            goHome.gameObject.SetActive(true);
        }
    }
    private void OnCollisionExit(Collision collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            goHome.gameObject.SetActive(false);
        }
    }
    public void GoHome()
    {
        SceneManager.LoadScene(1);
    }
}
