using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR;

public class FootstepSound : MonoBehaviour {
    public AudioSource footstepAudioSource;
    private bool footstepSoundEnabled = true;
    private bool leftGripPressed = false;

    void Update() {
        UpdateFootstepSoundState();
    }

    // Play footstep sound
    public void PlayFootstepSound() {
        if (footstepSoundEnabled && footstepAudioSource != null && !footstepAudioSource.isPlaying) {
            footstepAudioSource.Play();
        }
    }

    private void UpdateFootstepSoundState() {
        bool leftGripCurrentState = CheckControllerButton(InputDeviceRole.LeftHanded, CommonUsages.gripButton);

        if (leftGripCurrentState && !leftGripPressed) {
            footstepSoundEnabled = !footstepSoundEnabled;
            Debug.Log("Footstep sound " + (footstepSoundEnabled ? "enabled" : "disabled"));
        }

        leftGripPressed = leftGripCurrentState;
    }

    bool CheckControllerButton(InputDeviceRole role, InputFeatureUsage<bool> button) {
        List<InputDevice> devices = new List<InputDevice>();
        InputDevices.GetDevicesWithRole(role, devices);

        foreach (var device in devices) {
            if (device.TryGetFeatureValue(button, out bool pressed) && pressed) {
                return true;
            }
        }
        return false;
    }
}
