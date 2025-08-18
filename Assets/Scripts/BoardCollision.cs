
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
            // First, try to get the multiplayer sandbag component
            SandbagMultiPlayer multiPlayerBag = collision.gameObject.GetComponent<SandbagMultiPlayer>();
            if (multiPlayerBag != null && !multiPlayerBag.HasLandedOnBoard)
            {
                multiPlayerBag.HasLandedOnBoard = true;
                //Debug.Log("Flag set on Multiplayer Bag: HasLandedOnBoard");
                return; // Exit once we've found the correct component
            }

            // If it wasn't a multiplayer bag, try to get the single-player component
            SandbagController singlePlayerBag = collision.gameObject.GetComponent<SandbagController>();
            if (singlePlayerBag != null && !singlePlayerBag.HasLandedOnBoard)
            {
                singlePlayerBag.HasLandedOnBoard = true;
                audioManager.PlayClip(2); // Play sound for single-player bag
                //Debug.Log("Flag set on Single-Player Bag: HasLandedOnBoard");
            }
        }
    }
}

