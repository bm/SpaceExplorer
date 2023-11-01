using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class LevelOne : MonoBehaviour
{
    public GameObject Panel;
    public GameObject level1DescriptionPanel;

    private void Start()
    {
        Cursor.lockState = CursorLockMode.None;
        // Find the button component on the current GameObject
        Button button = GetComponent<Button>();

        // Attach a listener to the button's click event
        button.onClick.AddListener(OnLevelOneClicked);
    }

    public void OnLevelOneClicked()
    {
        // Disable the current panel and enable the level 1 description panel
        Panel.SetActive(false);
        level1DescriptionPanel.SetActive(true);
    }
}





/*
public class LevelOne : MonoBehaviour
{
    public string sceneName = "New Moon Surface";

    public void LoadScene()
    {
        SceneManager.LoadScene(sceneName);
        Time.timeScale = 1f;
    }
}
*/