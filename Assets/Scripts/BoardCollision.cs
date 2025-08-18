
using UnityEngine;

/// <summary>
/// Detects when the sandbag collides with the cornhole board.
/// This version is updated to work with both single-player and multiplayer sandbags.
/// </summary>
public class BoardCollision : MonoBehaviour
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
            if (multiPlayerBag != null && !multiPlayerBag.HasLandedOnBoard)
            {
                multiPlayerBag.HasLandedOnBoard = true;
                audioManager.PlayClip(2);
                return; 
            }

            
            SandbagController singlePlayerBag = collision.gameObject.GetComponent<SandbagController>();
            if (singlePlayerBag != null && !singlePlayerBag.HasLandedOnBoard)
            {
                singlePlayerBag.HasLandedOnBoard = true;
                audioManager.PlayClip(2); 
            }
        }
    }
}

