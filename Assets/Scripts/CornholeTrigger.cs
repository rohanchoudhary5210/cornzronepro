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
        if (other.gameObject.CompareTag("Player"))
        {
            SandbagMultiPlayer multiPlayerBag = other.gameObject.GetComponent<SandbagMultiPlayer>();
            if (multiPlayerBag != null && !multiPlayerBag.HasScoredInHole)
            {
                multiPlayerBag.HasScoredInHole = true;
                audioManager.PlayClip(3);
            }

            SandbagController singlePlayerBag = other.gameObject.GetComponent<SandbagController>();
            if (singlePlayerBag != null && !singlePlayerBag.HasScoredInHole)
            {
                singlePlayerBag.HasScoredInHole = true;
                audioManager.PlayClip(3);
                
            }
        }
    }
}
