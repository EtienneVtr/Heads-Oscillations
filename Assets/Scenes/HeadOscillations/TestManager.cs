using System;
using System.IO;
using System.Collections.Generic; // Added for List<>
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using UnityEngine.XR;

public class TestManager : MonoBehaviour {
    [Header("References")]
    public PlayerController playerController; // Reference to the PlayerController
    public Camera mainCamera; // Reference to the main camera
    public Transform canvasTransform; // Transform of the canvas for the test
    public Slider slider; // UI Slider used in the test

    [Header("FMS Test")]
    private bool isTestActive = false; // Indicates if the test is active
    private float startTime; // Start time of the test
    private bool fmsTestDone = false; // Indicates if the current test iteration is done
    public float fmsTestFrequency; // Frequency of the test
    private int fmsCount = 0; // Count of how many tests have been conducted
    private string path; // Path to the file where test results are saved
    public float distanceFromCamera; // Distance from the camera to position the canvas
    public float fixedHeightFromGround; // Fixed height to position the canvas

    // Trigger states
    private bool leftTriggerPressed = false; // Tracks the previous state of the left trigger
    private bool rightTriggerPressed = false; // Tracks the previous state of the right trigger

    // Start is called before the first frame update
    void Start() {
        canvasTransform.gameObject.SetActive(false); // Hide the canvas at the start
        startTime = Time.time; // Record the start time

        string sceneName = SceneManager.GetActiveScene().name;
        path = CreateFile(sceneName); // Create a file for the current scene
    }

    // Update is called once per frame
    void Update() {
        fmsTest(); // Check if it's time to activate the test

        if (isTestActive) {
            HandleSliderControl(); // Handle slider control during the test

            // End the test when the A button on the controller is pressed
            if (Input.GetKeyDown(KeyCode.JoystickButton0)) {
                EndTest(); // End the test
            }
        }
    }

    void fmsTest() {
        // Check if the current time matches the test frequency to activate the test
        if (Math.Abs((Time.time - startTime) % fmsTestFrequency) <= 0.01f && !fmsTestDone && Time.time > fmsTestFrequency / 2.0f) {
            Debug.Log("It's time to make a test!");
            ActivateTest(); // Activate the test
            fmsTestDone = true; // Mark the test as done for this iteration
        } else if (Math.Abs((Time.time - startTime) % fmsTestFrequency) >= 0.01f) {
            fmsTestDone = false; // Reset the test done flag
        }
    }

    public void PositionCanvas() {
        if (canvasTransform != null) {
            Vector3 newPosition = mainCamera.transform.position + mainCamera.transform.forward * distanceFromCamera;
            newPosition.y = fixedHeightFromGround; // Fix the height of the canvas
            canvasTransform.position = newPosition;
            canvasTransform.rotation = Quaternion.LookRotation(canvasTransform.position - mainCamera.transform.position); // Rotate canvas to face the camera
            canvasTransform.gameObject.SetActive(true); // Activate the canvas
        }
    }

    void HandleSliderControl() {
        bool leftTriggerCurrentState = CheckControllerButton(InputDeviceRole.LeftHanded, CommonUsages.triggerButton); // Check left trigger state
        bool rightTriggerCurrentState = CheckControllerButton(InputDeviceRole.RightHanded, CommonUsages.triggerButton); // Check right trigger state

        // Adjust slider value based on trigger input
        if (slider != null) {
            if (rightTriggerCurrentState && !rightTriggerPressed) {
                slider.value += 1; // Increase slider value
            }
            if (leftTriggerCurrentState && !leftTriggerPressed) {
                slider.value -= 1; // Decrease slider value
            }
        }

        // Update trigger states
        leftTriggerPressed = leftTriggerCurrentState;
        rightTriggerPressed = rightTriggerCurrentState;
    }

    public void ActivateTest() {
        isTestActive = true; // Mark the test as active
        Cursor.visible = true; // Show the cursor
        playerController.isTestActive = true; // Notify player controller that the test is active
        Cursor.lockState = CursorLockMode.None; // Unlock the cursor
        PositionCanvas(); // Position the canvas
        fmsCount++; // Increment the test count
    }

    public void EndTest() {
        isTestActive = false; // Mark the test as inactive
        Cursor.visible = false; // Hide the cursor
        playerController.isTestActive = false; // Notify player controller that the test is inactive
        Cursor.lockState = CursorLockMode.Locked; // Lock the cursor
        canvasTransform.gameObject.SetActive(false); // Deactivate the canvas

        Debug.Log("FMS Test Result: " + slider.value);

        // Append test results to the file
        using (StreamWriter sw = File.AppendText(path)) {
            sw.WriteLine("(Time: " + Time.time + ") FMS Test Number " + fmsCount + ": " + slider.value);
        }
    }

    string CreateFile(string sceneName) {
        string path = "Assets/Scenes/Project/" + sceneName + ".txt";

        // Delete the file if it already exists
        if (File.Exists(path)) {
            File.Delete(path);
            Debug.Log("File deleted: " + path);
        }

        System.IO.File.Create(path).Dispose(); // Create a new file
        return path;
    }

    bool CheckControllerButton(InputDeviceRole role, InputFeatureUsage<bool> button) {
        List<InputDevice> devices = new List<InputDevice>();
        InputDevices.GetDevicesWithRole(role, devices);

        // Check if the specified button is pressed on any device with the given role
        foreach (var device in devices) {
            if (device.TryGetFeatureValue(button, out bool pressed) && pressed) {
                return true;
            }
        }
        return false;
    }
}
