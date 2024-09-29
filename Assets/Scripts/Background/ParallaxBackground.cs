using System.Linq;
using UnityEngine;
using UnityEngine.UI;

public class ParallaxBackground : MonoBehaviour
{

    public GameObject[] backgroundLayers;
    private Material[] materials;
    const float baseScrollSpeed = 0.002f;

    void Awake() {
        materials = new Material[4];
        for (int i = 0; i < backgroundLayers.Length; i++) {
            materials[i] = backgroundLayers[i].GetComponent<Image>().material;
        }
    }

    void FixedUpdate()
    {
        for (int i = 0; i < materials.Length; i++) {
            float newOffsetX = Time.deltaTime * baseScrollSpeed * (i + 1);
            materials[i].mainTextureOffset += new Vector2(newOffsetX, 0);
        }
    }
}
