using System;
using System.IO;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class TestManager : MonoBehaviour {
    [Header("References")]
    public PlayerController playerController;
    public Transform canvasTransform;
    public Slider slider;

    [Header("FMS Test")]
    private bool isTestActive = false;
    private float startTime;
    private bool fmsTestDone = false;
    private float fmsTestFrequency = 30.0f;
    private int fmsCount = 0;
    private string path;

    // Start est appelé avant le premier frame update
    void Start() {
        canvasTransform.gameObject.SetActive(false);
        startTime = Time.time;

        string sceneName = SceneManager.GetActiveScene().name;
        path = CreateFile(sceneName); // Create a file with the name of the scene
    }

    // Update est appelé une fois par frame
    void Update() {
        fmsTest();

        if (isTestActive) {
            HandleSliderControl();

            if (Input.GetKeyDown(KeyCode.Return)) {
                EndTest();
            }
        }
    }

    void fmsTest() {
        if (Math.Abs((Time.time - startTime) % fmsTestFrequency) <= 0.01f && !fmsTestDone && Time.time > fmsTestFrequency / 2.0f) {
            Debug.Log("It's time to make a test!");
            ActivateTest();
            fmsTestDone = true;
        } else if (Math.Abs((Time.time - startTime) % fmsTestFrequency) >= 0.01f) {
            fmsTestDone = false;
        }
    }

    public void PositionCanvas() {
        if (canvasTransform != null) {
            float distanceFromCamera = 5.0f;

            Vector3 newPosition = playerController.playerCamera.transform.position + playerController.playerCamera.transform.forward * distanceFromCamera;
            canvasTransform.position = newPosition;
            canvasTransform.rotation = Quaternion.LookRotation(canvasTransform.position - playerController.playerCamera.transform.position);
            canvasTransform.gameObject.SetActive(true);
        }
    }

    void HandleSliderControl() {
        if (slider != null) {
            if (Input.GetKeyDown(KeyCode.LeftArrow)) {
                slider.value -= 1;
            }
            if (Input.GetKeyDown(KeyCode.RightArrow)) {
                slider.value += 1;
            }
        }
    }

    public void ActivateTest() {
        isTestActive = true;
        Cursor.visible = true;
        playerController.isTestActive = true;
        Cursor.lockState = CursorLockMode.None;
        PositionCanvas();
        fmsCount++;
    }

    public void EndTest() {
        isTestActive = false;
        Cursor.visible = false;
        playerController.isTestActive = false;
        Cursor.lockState = CursorLockMode.Locked;
        canvasTransform.gameObject.SetActive(false);

        Debug.Log("FMS Test Result: " + slider.value);

        using (StreamWriter sw = File.AppendText(path)) {
            sw.WriteLine("(Time: " + Time.time + ") FMS Test Number " + fmsCount + ": " + slider.value);
        }
    }

    string CreateFile(string sceneName) {
        string path = "Assets/Scenes/Project/" + sceneName + ".txt";

        if (File.Exists(path)) {
            File.Delete(path);
            Debug.Log("Fichier supprimé : " + path);
        }

        System.IO.File.Create(path).Dispose();
        return path;
    }
}
