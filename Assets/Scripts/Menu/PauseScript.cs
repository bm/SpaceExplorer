using System;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;

namespace Menu
{
    public class PauseScript : MonoBehaviour
    {

        [SerializeField] private GameObject pauseMenu;
        [SerializeField] private GameObject menuButton;
        [SerializeField] private GameObject resumeButton;

        [SerializeField] private GameObject character;
        private PlayerInput _playerInput;

        private void Awake()
        {
            _playerInput = character.GetComponent<PlayerInput>();
        }

        private void Update()
        {
            if (Input.GetKeyUp(KeyCode.P))
            {
                if (pauseMenu.activeSelf)
                {
                    Unpause();
                }
                else
                {
                    Pause();
                }
            }
        }

        public void MainMenu()
        {
            Time.timeScale = 1f;
            AudioListener.pause = false;
            if (_playerInput != null) _playerInput.ActivateInput();
            SceneManager.LoadScene("Scenes/MenuScene");
        }

        public void Pause()
        {
            Time.timeScale = 0f;
            pauseMenu.SetActive(true);
            Cursor.lockState = CursorLockMode.Confined;
            AudioListener.pause = true;
            if (_playerInput != null) _playerInput.DeactivateInput();
        }

        public void Unpause()
        {
            Time.timeScale = 1f;
            pauseMenu.SetActive(false);
            Cursor.lockState = CursorLockMode.Locked;
            AudioListener.pause = false;
            if (_playerInput != null) _playerInput.ActivateInput();
        }
    }
}