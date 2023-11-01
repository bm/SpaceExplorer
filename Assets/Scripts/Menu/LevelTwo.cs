using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class LevelTwo : MonoBehaviour
{
    public GameObject Panel;
    public GameObject level2DescriptionPanel;

    private void Start()
    {
        // Find the button component on the current GameObject
        Button button = GetComponent<Button>();

        // Attach a listener to the button's click event
        button.onClick.AddListener(OnTwoOneClicked);
    }

    public void OnTwoOneClicked()
    {
        // Disable the current panel and enable the level 1 description panel
        Panel.SetActive(false);
        level2DescriptionPanel.SetActive(true);
    }
}






/*
public class LevelTwo : MonoBehaviour
{
    public string sceneName = "Mars Surface";

    public void LoadScene()
    {
        SceneManager.LoadScene(sceneName);
        Time.timeScale = 1f;
    }
}
*/