using UnityEngine;

public class MoveParallaxBackground : MonoBehaviour
{
    
    ParallaxBackgroundAttached parallaxBackground;

    void Start() {
        parallaxBackground = GameObject.Find("ParallaxBackground").GetComponent<ParallaxBackgroundAttached>();
    }

    void FixedUpdate() {
        parallaxBackground.MoveParallaxBackground(transform.position);     
    }
}
