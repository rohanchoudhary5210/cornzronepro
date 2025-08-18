using UnityEngine;

/// <summary>
/// Detects when the sandbag enters the hole's trigger collider.
/// This version is updated to work with both single-player and multiplayer sandbags.
/// </summary>
public class CornholeTrigger : MonoBehaviour
{
    public IAudioManager audioManager;

    void Start()
    {
         audioManager = FindAnyObjectByType<AudioManager>();
    }

    void OnTriggerEnter(Collider other)
    {
        //Debug.Log("Cornhole Trigger Entered by: " + other.gameObject.name);
        if (other.gameObject.CompareTag("Player"))
        {
            // First, try to get the multiplayer sandbag component
            SandbagMultiPlayer multiPlayerBag = other.gameObject.GetComponent<SandbagMultiPlayer>();
            if (multiPlayerBag != null && !multiPlayerBag.HasScoredInHole)
            {
                multiPlayerBag.HasScoredInHole = true;
                //Debug.Log("Flag set on Multiplayer Bag: HasScoredInHole");
            }

            // If it wasn't a multiplayer bag, try to get the single-player component
            SandbagController singlePlayerBag = other.gameObject.GetComponent<SandbagController>();
            if (singlePlayerBag != null && !singlePlayerBag.HasScoredInHole)
            {
                singlePlayerBag.HasScoredInHole = true;
                audioManager.PlayClip(3);
                //Debug.Log("Flag set on Single-Player Bag: HasScoredInHole");
            }
        }
    }
}
