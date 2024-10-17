using System.Collections;
using UnityEngine;

public class TutorialManager : MonoBehaviour
{

    // Exports
    public Sprite spawnerSprite;
    public Sprite weaponCacheSprite;
    public Sprite crateSprite;
    public Sprite cocoonSprite;

    // State
    public bool firstSpawnerContact { get; set; }
    public bool firstWeaponCollected { get; set; }
    public bool firstCollectibleCollected { get; set; }
    public bool firstSurvivorCocoonImpact { get; set; }

    // References
    TalkingHead talkingHead;

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

    void Start() {
        talkingHead = GameObject.Find("TalkingHead").GetComponent<TalkingHead>();
    }

    public IEnumerator SpawnerFirstHitEvent() {
        Debug.Log("SpawnerFirstHitEvent");
        if (!firstSpawnerContact) {   
            Debug.Log($"Talking head talking? {talkingHead.IsTalking}"); 
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

    public IEnumerator FirstWeaponCacheCollectedEvent() {
        if (!firstWeaponCollected) {    
            while (talkingHead.IsTalking) {
                yield return new WaitForSeconds(0.25f);
            }
            firstWeaponCollected = true;
            talkingHead.NewMessage(
                "Looks like you've found a weapon cache. Make sure to grab these for more firepower! .",
                TalkingHead.MessageDestination.Communication,
                weaponCacheSprite
            );   
        }
    }

    public IEnumerator FirstCrateCollectedEvent() {
        if (!firstCollectibleCollected) {
            while (talkingHead.IsTalking) {
                yield return new WaitForSeconds(0.25f);
            }
            firstCollectibleCollected = true;
            talkingHead.NewMessage(
                "I see you've picked up a crate. These contain useful objects like armour, health and explosives. .",
                TalkingHead.MessageDestination.Communication,
                crateSprite
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
