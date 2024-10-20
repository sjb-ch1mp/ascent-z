using System.Collections;
using UnityEngine;

public class TutorialManager : MonoBehaviour
{

    // Exports
    public TalkingHead talkingHead;
    public Sprite spawnerSprite;
    public Sprite weaponCacheSprite;
    public Sprite crateSprite;
    public Sprite cocoonSprite;

    // State
    public bool firstSpawnerContact { get; set; }
    public bool firstSurvivorCocoonImpact { get; set; }
    bool gameIntroductionPlayed = false;
    bool firstReviveOccurred = false;

    public static TutorialManager Instance { get; private set; }

    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    public IEnumerator PlayIntroduction() {
        while (talkingHead.IsTalking) {
            yield return new WaitForSeconds(0.25f);
        }
        if (!gameIntroductionPlayed) {
            gameIntroductionPlayed = true;
            talkingHead.NewMessage(
                "Welcome to hell, private!\nOur recon team has detected an anomaly at the top of this building and we need you to investigate. .", 
                TalkingHead.MessageDestination.Communication, 
                null
            );
            // Wait for player to dismiss head
            while (talkingHead.gameObject.activeSelf) {
                yield return new WaitForSeconds(0.25f);
            }
            talkingHead.NewMessage(
                "This area is swarming with the dead, so you'll have to fight your way to the top with minimal support.\nKeep an eye out for air drop locations marked by flares! .", 
                TalkingHead.MessageDestination.Communication, 
                null
            );
            // Wait for player to dismiss head
            while (talkingHead.gameObject.activeSelf) {
                yield return new WaitForSeconds(0.25f);
            }
            talkingHead.NewMessage(
                "If you're as green as you look, make sure you press 'TAB' so you know what the controls are.\nGet to the top of this building and try not to die. .", 
                TalkingHead.MessageDestination.Communication, 
                null
            );
        } else if (!firstReviveOccurred) {
            firstReviveOccurred = true;
            talkingHead.NewMessage(
                "I thought I said try not to die, soldier. You're lucky we have some revive kits lying around, but they don't grow on trees! .", 
                TalkingHead.MessageDestination.Communication, 
                null
            );
        }
    }

    public IEnumerator SpawnerFirstHitEvent() {
        if (!firstSpawnerContact) {   
            while (talkingHead.IsTalking) {
                yield return new WaitForSeconds(0.25f);
            }
            firstSpawnerContact = true;
            talkingHead.NewMessage(
                "Uh oh, you've come into contact with a SPAWNER. Those disgusting blobs are the source of all these zombies. Kill it before it overwhelms you! .",
                TalkingHead.MessageDestination.Communication,
                spawnerSprite
            );    
        }
    }

    public IEnumerator FirstCocoonHitEvent() {
        if (!firstSurvivorCocoonImpact) {
            while (talkingHead.IsTalking) {
                yield return new WaitForSeconds(0.25f);
            }
            firstSurvivorCocoonImpact = true;
            talkingHead.NewMessage(
                "Oh my god, those cocoons contain survivors! Ensure that you save as many of those poor souls as you can, soldier! .",
                TalkingHead.MessageDestination.Communication,
                cocoonSprite
            );    
        }
    }
}
