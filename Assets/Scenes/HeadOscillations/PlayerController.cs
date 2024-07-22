using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR;

public class PlayerController : MonoBehaviour
{
    [Header("References")]
    public GameObject player;
    public Camera mainCamera;
    public Animator headAnimator;
    public Rigidbody rb; // Assure-toi de référencer le Rigidbody

    [Header("Configuration")]
    public float walkSpeed;
    public float runSpeed;
    private bool oscillations;

    [Header("Runtime")]
    Vector3 newVelocity;
    private bool isWalking;
    private bool isRunning;

    [Header("FMS Test")]
    public bool isTestActive = false;

    private bool leftTriggerPressed = false;
    private bool rightTriggerPressed = false;

    void Start()
    {
        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Locked;
        oscillations = false;

        isWalking = false;
        isRunning = false;

        // Assurez-vous que la gravité n'affecte pas la direction horizontale
        rb.useGravity = true;
        rb.constraints = RigidbodyConstraints.FreezeRotationX | RigidbodyConstraints.FreezeRotationZ;
    }

    void Update()
    {
        if (!isTestActive)
        {
            HandleMovement();
        }

        if (Input.GetKeyDown(KeyCode.Escape))
        {
            EndGame();
        }
    }

    void HandleMovement()
    {
        testSpeed();
        updateOscillations();
    }

    void FixedUpdate()
    {
        if (!isTestActive)
        {
            if (isRunning)
            {
                Move(runSpeed);
                SetAnimatorBools(false, true);
            }
            else if (isWalking)
            {
                Move(walkSpeed);
                SetAnimatorBools(true, false);
            }
            else
            {
                rb.velocity = Vector3.up * rb.velocity.y; // Gardez seulement la composante verticale de la vitesse
                SetAnimatorBools(false, false);
            }
        }
        else
        {
            rb.velocity = Vector3.zero; // Stop all movement if the test is active
            SetAnimatorBools(false, false);
        }
    }

    void Move(float speed)
    {
        Vector3 forward = mainCamera.transform.forward;
        forward.y = 0; // Ignorer la composante verticale pour éviter de pencher
        forward.Normalize();
        newVelocity = forward * speed;
        rb.velocity = newVelocity + Vector3.up * rb.velocity.y; // Combinez avec la composante verticale existante
    }

    void testSpeed()
    {
        // Vérifier les triggers
        bool leftTriggerCurrentState = CheckControllerButton(InputDeviceRole.LeftHanded, CommonUsages.triggerButton);
        bool rightTriggerCurrentState = CheckControllerButton(InputDeviceRole.RightHanded, CommonUsages.triggerButton);

        // Détecter les changements d'état pour les triggers
        if (leftTriggerCurrentState && rightTriggerCurrentState)
        {
            isRunning = true;
            isWalking = false;
        }
        else if ((leftTriggerCurrentState && !rightTriggerCurrentState) || (!leftTriggerCurrentState && rightTriggerCurrentState))
        {
            isRunning = false;
            isWalking = true;
        }
        else if (!leftTriggerCurrentState && !rightTriggerCurrentState)
        {
            isRunning = false;
            isWalking = false;
        }

        // Mettre à jour les états des triggers
        leftTriggerPressed = leftTriggerCurrentState;
        rightTriggerPressed = rightTriggerCurrentState;
    }

    void updateOscillations()
    {
        if (Input.GetKeyDown(KeyCode.Alpha3))
        {
            oscillations = !oscillations;
            Debug.Log("oscillations " + (oscillations ? "enabled" : "disabled"));
        }
    }

    void SetAnimatorBools(bool isWalking, bool isRunning)
    {
        if (oscillations)
        {
            headAnimator.SetBool("isWalking", isWalking);
            headAnimator.SetBool("isRunning", isRunning);
        } else
        {
            headAnimator.SetBool("isWalking", false);
            headAnimator.SetBool("isRunning", false);
        }
    }

    bool CheckControllerButton(InputDeviceRole role, InputFeatureUsage<bool> button)
    {
        List<InputDevice> devices = new List<InputDevice>();
        InputDevices.GetDevicesWithRole(role, devices);

        foreach (var device in devices)
        {
            if (device.TryGetFeatureValue(button, out bool pressed) && pressed)
            {
                return true;
            }
        }
        return false;
    }

    public void EndGame()
    {
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#else
        Application.Quit();
#endif
    }
}
