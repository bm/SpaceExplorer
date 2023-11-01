using System;
using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;
using StarterAssets;
using UnityEngine.SceneManagement;
using TMPro;
using UnityEngine.InputSystem;


public class CharacterControllerCustom : MonoBehaviour
{
    public GameObject gameOverObject;
    public GameObject gameWinObject;
    private CharacterController controller;
    private Vector3 moveDirection;
    public float speed = 5f;
    public float rotationSpeed = 100f;
    //public float gravity = 9.81f;
    public int maxHealth = 100;
    public int currentHealth;
    public TextMeshProUGUI healthText;
    public TextMeshProUGUI batteryText;
    public int batteryCount = 0;  
    public int maxBatteries = 6;
    public Image damageImage;

    private float timeRemaining = -1;
    private bool timerOn = false;

    private PlayerInput _playerInput;
    private GameObject _mainCamera;

    public AudioClip collectAudio;
    public AudioClip powerUpAudio;
    
    public GameObject CinemachineCameraTarget;
    
    public float TopClamp = 100.0f;
    public float BottomClamp = -30.0f;
    public float CameraAngleOverride = 0.0f;
    public bool LockCameraPosition = false;
    
    private float _targetRotation;
    private float _rotationVelocity;
    public float RotationSmoothTime = 0.12f;
    
    // cinemachine
    private float _cinemachineTargetYaw;
    private float _cinemachineTargetPitch;
    
    private const float _threshold = 0.01f;

    private StarterAssetsInputs _input;
    
    [SerializeField] private GameObject healthBar;
    private ProgressBarPro healthBarObj;
    [SerializeField] private GameObject batteryBar;
    private ProgressBarPro batteryBarObj;
    [SerializeField] private GameObject powerUpBar;
    private ProgressBarPro powerUpBarObj;
    
    private void Start()
    {
        controller = GetComponent<CharacterController>();
        _playerInput = GetComponent<PlayerInput>();
        _input = GetComponent<StarterAssetsInputs>();
        currentHealth = maxHealth;
        Cursor.lockState = CursorLockMode.Locked;
        _cinemachineTargetYaw = CinemachineCameraTarget.transform.rotation.eulerAngles.y;
        
        healthBarObj = healthBar.GetComponent<ProgressBarPro>();
        batteryBarObj = batteryBar.GetComponent<ProgressBarPro>();
        powerUpBarObj = powerUpBar.GetComponent<ProgressBarPro>();
    }

    private void Awake()
    {
        if (_mainCamera == null)
        {
            _mainCamera = GameObject.FindGameObjectWithTag("MainCamera");
        }
    }

    private void Update()
    {
        float horizontalInput = Input.GetAxis("Horizontal");
        float verticalInput = Input.GetAxis("Vertical");

        // Apply rotation
        transform.Rotate(0f, horizontalInput * rotationSpeed * Time.deltaTime, 0f);

        // Apply movement
        Vector3 forwardMovement = transform.forward * verticalInput * speed;
        moveDirection = new Vector3(forwardMovement.x, moveDirection.y, forwardMovement.z);

        // Apply gravity
        // moveDirection.y -= gravity * Time.deltaTime;

        // Move the character
        controller.Move(moveDirection * Time.deltaTime);
        
        // normalise input direction
        Vector3 inputDirection = new Vector3(_input.move.x, 0.0f, _input.move.y).normalized;

        // note: Vector2's != operator uses approximation so is not floating point error prone, and is cheaper than magnitude
        // if there is a move input rotate player when the player is moving
        if (_input.move != Vector2.zero)
        {
            _targetRotation = Mathf.Atan2(inputDirection.x, inputDirection.z) * Mathf.Rad2Deg +
                              _mainCamera.transform.eulerAngles.y;
            float rotation = Mathf.SmoothDampAngle(transform.eulerAngles.y, _targetRotation, ref _rotationVelocity,
                RotationSmoothTime);

            // rotate to face input direction relative to camera position
            transform.rotation = Quaternion.Euler(0.0f, rotation, 0.0f);
        }

        // Align with the ground
        RaycastHit hit;
        if (Physics.Raycast(transform.position, Vector3.down, out hit, Mathf.Infinity))
        {
            float targetHeight = hit.point.y + controller.height / 2f;
            transform.position = new Vector3(transform.position.x, targetHeight, transform.position.z);
        }

        healthBarObj.SetValue(currentHealth, maxHealth);
        batteryBarObj.SetValue(batteryCount, maxBatteries);

        if (timerOn)
        {
            if (timeRemaining > 0)
            {
                timeRemaining -= Time.deltaTime;
                powerUpBarObj.SetValue(timeRemaining, 30);
            }
            else
            {
                timeRemaining = -1;
                timerOn = false;
                powerUpBar.SetActive(false);
            }
        }

        healthText.text = "Health: " + currentHealth;
        batteryText.text = "Batteries: " + batteryCount + "/" + maxBatteries;

        if (currentHealth <= 0)
        {
            Die();
        }

        if (batteryCount == maxBatteries)
        {
            WinRound();
        }
    }
    
    private void LateUpdate()
    {
        CameraRotation();
    }
    
    private void CameraRotation()
    {
        // if there is an input and camera position is not fixed
        if (_input.look.sqrMagnitude >= _threshold && !LockCameraPosition)
        {
            //Don't multiply mouse input by Time.deltaTime;
            float deltaTimeMultiplier = IsCurrentDeviceMouse ? 1.0f : Time.deltaTime;

            _cinemachineTargetYaw += _input.look.x * deltaTimeMultiplier;
            _cinemachineTargetPitch += _input.look.y * deltaTimeMultiplier;
        }

        // clamp our rotations so our values are limited 360 degrees
        _cinemachineTargetYaw = ClampAngle(_cinemachineTargetYaw, float.MinValue, float.MaxValue);
        _cinemachineTargetPitch = ClampAngle(_cinemachineTargetPitch, BottomClamp, TopClamp);

        // Cinemachine will follow this target
        CinemachineCameraTarget.transform.rotation = Quaternion.Euler(_cinemachineTargetPitch + CameraAngleOverride,
            _cinemachineTargetYaw, 0.0f);
    }
    
    private static float ClampAngle(float lfAngle, float lfMin, float lfMax)
    {
        if (lfAngle < -360f) lfAngle += 360f;
        if (lfAngle > 360f) lfAngle -= 360f;
        return Mathf.Clamp(lfAngle, lfMin, lfMax);
    }
    
    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("Collect"))
        {
            AudioSource.PlayClipAtPoint(collectAudio, other.gameObject.transform.position);
            other.gameObject.SetActive(false);
            batteryCount++;
        }
        if (other.gameObject.CompareTag("PowerUp"))
        {
            AudioSource.PlayClipAtPoint(powerUpAudio, other.gameObject.transform.position);
            other.gameObject.SetActive(false);
            StartCoroutine(ActivatePowerUp());
        }
    }
    
    IEnumerator ActivatePowerUp()
    {
        // power up active
        speed *= 2;
        timeRemaining = 30;
        powerUpBar.SetActive(true);
        timerOn = true;
        yield return new WaitForSeconds(30);
        //power up inactive
        speed /= 2;
    }

    public void TakeDamage(int damage)
    {
        currentHealth -= damage;

        // Start damage effect
        StartCoroutine(DamageEffect());
    }

    IEnumerator DamageEffect()
    {
        Color originalColor = damageImage.color;

        // Flash the damage image
        damageImage.color = new Color(1f, 0f, 0f, 0.5f);

        // Gradually fade the damage image
        while (damageImage.color.a > 0)
        {
            damageImage.color = Color.Lerp(damageImage.color, originalColor, Time.deltaTime*2);
            yield return null;
        }

        // Ensure the damage image is completely transparent
        damageImage.color = originalColor;
    }


    void Die()
    {
        // Enable the GameObject
        gameOverObject.SetActive(true);

        Time.timeScale = 0f;
        if (Input.GetKeyDown(KeyCode.R))
        {
            SceneManager.LoadScene("MenuScene");
        }
    }

    void WinRound()
    {
        // Enable the GameObject
        gameWinObject.SetActive(true);

        Time.timeScale = 0f;
        if (Input.GetKeyDown(KeyCode.Space))
        {
            SceneManager.LoadScene("MenuScene");
        }
    }
    
    private bool IsCurrentDeviceMouse
    {
        get
        {
#if ENABLE_INPUT_SYSTEM
            return _playerInput.currentControlScheme == "KeyboardMouse";
#else
				return false;
#endif
        }
    }
}
