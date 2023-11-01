using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class LevelOne_pt2 : MonoBehaviour
{
    public string sceneName = "Mars Surface";

    public void LoadScene()
    {
        SceneManager.LoadScene(sceneName);
        Time.timeScale = 1f;
        Cursor.lockState = CursorLockMode.Locked;
    }
}