using UnityEngine;

public class UtilityCrate : MonoBehaviour
{

    // Exports
    public Sprite[] utilityCrateSprites;
    public float healthDropThreshold = 0.25f;
    public float grenadeDropThreshold = 0.5f;
    public float ammoDropThreshold = 0.75f;
    public float armourDropThreshold = 0.95f;

    // State
    public Resources.Collectible CollectibleType { get; set; }

    // Start is called before the first frame update
    void Awake() {
        float randomValue = Random.value;
        if (randomValue < healthDropThreshold) {
            CollectibleType = Resources.Collectible.MEDPACK;
        } else if (randomValue < grenadeDropThreshold) {
            CollectibleType = Resources.Collectible.GRENADES;
        } else if (randomValue < ammoDropThreshold) {
            CollectibleType = Resources.Collectible.AMMUNITION;
        } else if (randomValue < armourDropThreshold) {
            CollectibleType = Resources.Collectible.ARMOUR;
        } else {
            CollectibleType = Resources.Collectible.LIFE;
        }
    }
}
