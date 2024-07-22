using System;
using System.IO;
using System.Collections.Generic; // Ajout de cette ligne pour List<>
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using UnityEngine.XR;

public class TestManager : MonoBehaviour
{
    [Header("References")]
    public PlayerController playerController;
    public Camera mainCamera;
    public Transform canvasTransform;
    public Slider slider;

    [Header("FMS Test")]
    private bool isTestActive = false;
    private float startTime;
    private bool fmsTestDone = false;
    public float fmsTestFrequency;
    private int fmsCount = 0;
    private string path;
    public float distanceFromCamera;
    public float fixedHeightFromGround; // Hauteur fixe à laquelle le canvas doit être placé

    // Triggers state
    private bool leftTriggerPressed = false;
    private bool rightTriggerPressed = false;

    // Start est appelé avant le premier frame update
    void Start()
    {
        canvasTransform.gameObject.SetActive(false);
        startTime = Time.time;

        string sceneName = SceneManager.GetActiveScene().name;
        path = CreateFile(sceneName); // Create a file with the name of the scene
    }

    // Update est appelé une fois par frame
    void Update()
    {
        fmsTest();

        if (isTestActive)
        {
            HandleSliderControl();

            // Validation lorsque la touche A est enfoncée
            if (Input.GetKeyDown(KeyCode.JoystickButton0)) // Touche A du contrôleur
            {
                EndTest();
            }
        }
    }

    void fmsTest()
    {
        if (Math.Abs((Time.time - startTime) % fmsTestFrequency) <= 0.01f && !fmsTestDone && Time.time > fmsTestFrequency / 2.0f)
        {
            Debug.Log("It's time to make a test!");
            ActivateTest();
            fmsTestDone = true;
        }
        else if (Math.Abs((Time.time - startTime) % fmsTestFrequency) >= 0.01f)
        {
            fmsTestDone = false;
        }
    }

    public void PositionCanvas()
    {
        if (canvasTransform != null)
        {
            Vector3 newPosition = mainCamera.transform.position + mainCamera.transform.forward * distanceFromCamera;
            newPosition.y = fixedHeightFromGround; // Fixer la hauteur du canvas
            canvasTransform.position = newPosition;
            canvasTransform.rotation = Quaternion.LookRotation(canvasTransform.position - mainCamera.transform.position);
            canvasTransform.gameObject.SetActive(true);
        }
    }

    void HandleSliderControl()
    {
        bool leftTriggerCurrentState = CheckControllerButton(InputDeviceRole.LeftHanded, CommonUsages.triggerButton);
        bool rightTriggerCurrentState = CheckControllerButton(InputDeviceRole.RightHanded, CommonUsages.triggerButton);

        // Augmenter/diminuer la valeur du slider
        if (slider != null)
        {
            if (rightTriggerCurrentState && !rightTriggerPressed)
            {
                slider.value += 1;
            }
            if (leftTriggerCurrentState && !leftTriggerPressed)
            {
                slider.value -= 1;
            }
        }

        // Mettre à jour les états des triggers
        leftTriggerPressed = leftTriggerCurrentState;
        rightTriggerPressed = rightTriggerCurrentState;
    }

    public void ActivateTest()
    {
        isTestActive = true;
        Cursor.visible = true;
        playerController.isTestActive = true;
        Cursor.lockState = CursorLockMode.None;
        PositionCanvas();
        fmsCount++;
    }

    public void EndTest()
    {
        isTestActive = false;
        Cursor.visible = false;
        playerController.isTestActive = false;
        Cursor.lockState = CursorLockMode.Locked;
        canvasTransform.gameObject.SetActive(false);

        Debug.Log("FMS Test Result: " + slider.value);

        using (StreamWriter sw = File.AppendText(path))
        {
            sw.WriteLine("(Time: " + Time.time + ") FMS Test Number " + fmsCount + ": " + slider.value);
        }
    }

    string CreateFile(string sceneName)
    {
        string path = "Assets/Scenes/Project/" + sceneName + ".txt";

        if (File.Exists(path))
        {
            File.Delete(path);
            Debug.Log("Fichier supprimé : " + path);
        }

        System.IO.File.Create(path).Dispose();
        return path;
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
}
