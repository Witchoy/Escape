using System;
using UnityEngine;
using UnityEngine.SceneManagement;

public class EndCubeTouched : MonoBehaviour
{
    public static event Action CubeEntered = delegate { };
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            // SceneManager.LoadScene(2);
            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;
            CubeEntered();
        }
    }
}
