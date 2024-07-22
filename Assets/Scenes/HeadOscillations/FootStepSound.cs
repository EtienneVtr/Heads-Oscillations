using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR;

public class FootstepSound : MonoBehaviour { // This class handles the footstep sound for a VR application
    public AudioSource footstepAudioSource; // Reference to the AudioSource component for footstep sounds
    private bool footstepSoundEnabled = true; // Tracks if the footstep sound is enabled
    private bool leftGripPressed = false; // Tracks the previous state of the left grip button

    void Update() { // Called once per frame
        UpdateFootstepSoundState(); // Updates the state of the footstep sound based on controller input
    }

    // Play footstep sound
    public void PlayFootstepSound() { // Plays the footstep sound if conditions are met
        if (footstepSoundEnabled && footstepAudioSource != null && !footstepAudioSource.isPlaying) { 
            footstepAudioSource.Play(); // Play the footstep sound
        }
    }

    private void UpdateFootstepSoundState() { // Updates the footstep sound state based on the left grip button
        bool leftGripCurrentState = CheckControllerButton(InputDeviceRole.LeftHanded, CommonUsages.gripButton); // Check the current state of the left grip button

        if (leftGripCurrentState && !leftGripPressed) { // Toggle footstep sound if the left grip button is newly pressed
            footstepSoundEnabled = !footstepSoundEnabled;
            Debug.Log("Footstep sound " + (footstepSoundEnabled ? "enabled" : "disabled")); // Log the current state
        }

        leftGripPressed = leftGripCurrentState; // Update the previous state of the left grip button
    }

    bool CheckControllerButton(InputDeviceRole role, InputFeatureUsage<bool> button) { // Checks if a specific button is pressed on a controller with the given role
        List<InputDevice> devices = new List<InputDevice>();
        InputDevices.GetDevicesWithRole(role, devices);

        foreach (var device in devices) { // Iterate through all devices with the given role
            if (device.TryGetFeatureValue(button, out bool pressed) && pressed) { // Check if the button is pressed
                return true; // Return true if the button is pressed
            }
        }
        return false; // Return false if the button is not pressed
    }
}
