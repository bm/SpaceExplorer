using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Back_Level2Button : MonoBehaviour
{
    public GameObject level2Description;
    public GameObject panel;

    public Button backButton;

    private void Start()
    {
        backButton.onClick.AddListener(OnBackButtonClicked);
    }

    private void OnBackButtonClicked()
    {
        level2Description.SetActive(false); // Disable the "Level 1 Description" panel
        panel.SetActive(true); // Enable the "Panel" panel
    }
}
