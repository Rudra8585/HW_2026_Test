using UnityEngine;

public class AnimationBridge : MonoBehaviour
{
    //This scripts whole purpose is to bridge the gap between the player controller 
    //and the character animation events to play the footstep sound effects

    private PlayerController playerController;

    private void Start()
    {
        // Automatically find the PlayerController on the parent object
        playerController = GetComponentInParent<PlayerController>();
    }

    // This catches the event from the animation and sends it to the player controller script
    public void PlayStepSound()
    {
        if (playerController != null)
        {
            playerController.PlayStepSound();
        }
    }

    public void PlayLandSound()
    {
        if (playerController != null)
        {
            playerController.PlayLandSound();
        }
    }
}