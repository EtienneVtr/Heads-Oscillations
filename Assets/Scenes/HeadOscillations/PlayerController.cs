using System;
using UnityEngine;

public class PlayerController : MonoBehaviour {
    [Header("References")]
    public Rigidbody rb;
    public Transform head;
    public Camera playerCamera;
    public Animator headAnimator;

    [Header("Configuration")]
    public float walkSpeed;
    public float runSpeed;
    private bool oscillations;

    [Header("Runtime")]
    Vector3 newVelocity;

    [Header("FMS Test")]
    public bool isTestActive = false;

    // Start est appelé avant le premier frame update
    void Start() {
        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Locked;
        oscillations = false;
    }

    // Update est appelé une fois par frame
    void Update() {
        HandleRotation();
        
        if(!isTestActive){
            HandleMovement();
        }

        if (Input.GetKeyDown(KeyCode.Escape)) {
            EndGame();
        }
    }

    void HandleMovement() {
        transform.Rotate(Vector3.up * Input.GetAxis("Mouse X") * 2f);
        updateVelocity();
        updateOscillations();
    }

    void HandleRotation() {
        Vector3 e = head.eulerAngles;
        e.x -= Input.GetAxis("Mouse Y") * 2f;
        e.x = RestrictAngle(e.x, -85f, 85f);
        head.eulerAngles = e;

        transform.Rotate(Vector3.up * Input.GetAxis("Mouse X") * 2f);
    }

    void FixedUpdate() {
        if (!isTestActive) {
            bool walk = Input.GetKey(KeyCode.Alpha1);
            bool run = Input.GetKey(KeyCode.Alpha2);

            if (walk && run) {
                rb.velocity = transform.forward * runSpeed + Vector3.up * rb.velocity.y;
                SetAnimatorBools(false, true);
            } else if (walk) {
                rb.velocity = transform.forward * walkSpeed + Vector3.up * rb.velocity.y;
                SetAnimatorBools(true, false);
            } else if (run) {
                rb.velocity = transform.forward * runSpeed + Vector3.up * rb.velocity.y;
                SetAnimatorBools(false, true);
            } else {
                rb.velocity = transform.TransformDirection(newVelocity);
                SetAnimatorBools(false, false);
            }
        } else {
            rb.velocity = Vector3.zero; // Stop all movement if the test is active
        }
    }

    void SetAnimatorBools(bool isWalking, bool isRunning) {
        if (oscillations) {
            headAnimator.SetBool("isWalking", isWalking);
            headAnimator.SetBool("isRunning", isRunning);
        }
    }

    public static float RestrictAngle(float angle, float angleMin, float angleMax) {
        if (angle > 180) angle -= 360;
        else if (angle < -180) angle += 360;

        if (angle > angleMax) angle = angleMax;
        else if (angle < angleMin) angle = angleMin;

        return angle;
    }

    void updateVelocity() {
        newVelocity = Vector3.up * rb.velocity.y;
        float speed = Input.GetKey(KeyCode.LeftShift) ? runSpeed : walkSpeed;
        newVelocity.x = Input.GetAxis("Horizontal") * speed;
        newVelocity.z = Input.GetAxis("Vertical") * speed;
    }

    void updateOscillations() {
        if (Input.GetKeyDown(KeyCode.Alpha3)) {
            oscillations = !oscillations;
            Debug.Log("oscillations " + (oscillations ? "enabled" : "disabled"));
        }
    }

    public void EndGame() {
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#else
        Application.Quit();
#endif
    }
}
