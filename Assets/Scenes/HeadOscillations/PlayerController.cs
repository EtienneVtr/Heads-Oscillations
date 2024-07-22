using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR;

public class PlayerController : MonoBehaviour {
    [Header("References")]
    public GameObject player; // Reference to the player GameObject
    public Camera mainCamera; // Reference to the main camera
    public Animator headAnimator; // Reference to the Animator component for the player's head
    public Rigidbody rb; // Reference to the Rigidbody component

    [Header("Configuration")]
    public float walkSpeed; // Speed at which the player walks
    public float runSpeed; // Speed at which the player runs
    private bool oscillations; // State to control head oscillations

    [Header("Runtime")]
    Vector3 newVelocity; // Stores the new velocity for the player
    private bool isWalking; 
    private bool isRunning;

    [Header("FMS Test")]
    public bool isTestActive = false; // Indicates if the test mode is active

    private bool leftTriggerPressed = false; // Tracks the previous state of the left trigger button
    private bool rightTriggerPressed = false; // Tracks the previous state of the right trigger button
    private bool rightGripPressed = false; // Tracks the previous state of the right grip button

    void Start() { // Initialization
        Cursor.visible = false; // Hide the cursor
        Cursor.lockState = CursorLockMode.Locked; // Lock the cursor to the center of the screen
        oscillations = false;
        isWalking = false;
        isRunning = false;

        // Ensure gravity does not affect horizontal direction
        rb.useGravity = true;
        rb.constraints = RigidbodyConstraints.FreezeRotationX | RigidbodyConstraints.FreezeRotationZ | RigidbodyConstraints.FreezeRotationY;
    }

    void Update() {
        if (!isTestActive) {
            HandleMovement(); // Handle player movement
        }

        updateOscillations(); // Update the oscillation state

        if (Input.GetKeyDown(KeyCode.Escape)) {
            EndGame(); // End the game when Escape is pressed
        }
    }

    void HandleMovement() {
        testSpeed(); // Check and set the speed based on input
    }

    void FixedUpdate() {
        if (!isTestActive) {
            if (isRunning) {
                Move(runSpeed); // Move the player at running speed
                SetAnimatorBools(false, true); // Update animator for running
            } else if (isWalking) {
                Move(walkSpeed); // Move the player at walking speed
                SetAnimatorBools(true, false); // Update animator for walking
            } else {
                rb.velocity = Vector3.up * rb.velocity.y; // Maintain only the vertical velocity
                SetAnimatorBools(false, false); // Update animator for idle
            }
        } else {
            rb.velocity = Vector3.zero; // Stop all movement if the test is active
            SetAnimatorBools(false, false); // Update animator for idle
        }
    }

    void Move(float speed) {
        Vector3 forward = mainCamera.transform.forward; 
        forward.y = 0; // Ignore vertical component to prevent tilting
        forward.Normalize();
        newVelocity = forward * speed;
        rb.velocity = newVelocity + Vector3.up * rb.velocity.y; // Combine with existing vertical velocity
    }

    void testSpeed() {
        bool leftTriggerCurrentState = CheckControllerButton(InputDeviceRole.LeftHanded, CommonUsages.triggerButton); // Check left trigger state
        bool rightTriggerCurrentState = CheckControllerButton(InputDeviceRole.RightHanded, CommonUsages.triggerButton); // Check right trigger state

        if (leftTriggerCurrentState && rightTriggerCurrentState) {
            isRunning = true; // Run if both triggers are pressed
            isWalking = false;
        } else if (leftTriggerCurrentState || rightTriggerCurrentState) {
            isRunning = false;
            isWalking = true; // Walk if one trigger is pressed
        } else {
            isRunning = false;
            isWalking = false; // Idle if no trigger is pressed
        }

        leftTriggerPressed = leftTriggerCurrentState; // Update left trigger state
        rightTriggerPressed = rightTriggerCurrentState; // Update right trigger state
    }

    void updateOscillations() {
        bool rightGripCurrentState = CheckControllerButton(InputDeviceRole.RightHanded, CommonUsages.gripButton); // Check right grip state

        if (rightGripCurrentState && !rightGripPressed) {
            oscillations = !oscillations; // Toggle oscillations on grip press
            Debug.Log("oscillations " + (oscillations ? "enabled" : "disabled")); // Log the state of oscillations
        }

        rightGripPressed = rightGripCurrentState; // Update right grip state
    }

    void SetAnimatorBools(bool isWalking, bool isRunning) {
        if (oscillations) {
            headAnimator.SetBool("isWalking", isWalking); // Set walking animation
            headAnimator.SetBool("isRunning", isRunning); // Set running animation
        } else {
            headAnimator.SetBool("isWalking", false); // Disable walking animation
            headAnimator.SetBool("isRunning", false); // Disable running animation
        }
    }

    bool CheckControllerButton(InputDeviceRole role, InputFeatureUsage<bool> button) {
        List<InputDevice> devices = new List<InputDevice>();
        InputDevices.GetDevicesWithRole(role, devices);

        foreach (var device in devices) {
            if (device.TryGetFeatureValue(button, out bool pressed) && pressed) {
                return true; // Return true if button is pressed
            }
        }
        return false; // Return false if button is not pressed
    }

    public void EndGame() {
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false; // Stop play mode in editor
#else
        Application.Quit(); // Quit application
#endif
    }
}
