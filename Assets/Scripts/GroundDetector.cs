using UnityEngine;

/// <summary>
/// Detects when the sandbag hits the ground.
/// This version is updated to work with both single-player and multiplayer sandbags.
/// </summary>
public class GroundDetector : MonoBehaviour
{
    public IAudioManager audioManager;
     void Start()
    {
        audioManager = FindAnyObjectByType<AudioManager>();
    }
    void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            SandbagMultiPlayer multiPlayerBag = collision.gameObject.GetComponent<SandbagMultiPlayer>();
            if (multiPlayerBag != null && !multiPlayerBag.HasHitGround)
            {
                multiPlayerBag.HasHitGround = true;
                audioManager.PlayClip(1);
                return; 
            }

            // If it wasn't a multiplayer bag, try to get the single-player component
            SandbagController singlePlayerBag = collision.gameObject.GetComponent<SandbagController>();
            if (singlePlayerBag != null && !singlePlayerBag.HasHitGround)
            {
                singlePlayerBag.HasHitGround = true;
                audioManager.PlayClip(1); 
            }
        }
    }
}
