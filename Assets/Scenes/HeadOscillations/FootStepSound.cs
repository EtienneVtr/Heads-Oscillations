using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FootstepSound : MonoBehaviour {
    public AudioSource footstepAudioSource;
    
    // Play footstep sound
    public void PlayFootstepSound() {
        if (footstepAudioSource != null && !footstepAudioSource.isPlaying) {
            footstepAudioSource.Play();
        }
    }
}
