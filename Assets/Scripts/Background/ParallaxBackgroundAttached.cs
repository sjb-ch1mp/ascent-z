using UnityEngine;
using UnityEngine.UI;

public class ParallaxBackgroundAttached : MonoBehaviour
{

    public Image layer1;
    public Image layer2;
    public Image layer3;
    public Image layer4;
    public float parallaxEffect1 = 0.1f;
    public float parallaxEffect2 = 0.2f;
    public float parallaxEffect3 = 0.3f;
    public float parallaxEffect4 = 0.4f;

    private Vector2 lastPosition;

    public void MoveParallaxBackground(Vector2 newPosition) {

        if (lastPosition == null) {
            lastPosition = newPosition;
            return;
        }

        float xDiff = newPosition.x - lastPosition.x;
        lastPosition = newPosition;

        layer1.material.mainTextureOffset += new Vector2(xDiff * parallaxEffect1, 0);
        layer2.material.mainTextureOffset += new Vector2(xDiff * parallaxEffect2, 0);
        layer3.material.mainTextureOffset += new Vector2(xDiff * parallaxEffect3, 0);
        layer4.material.mainTextureOffset += new Vector2(xDiff * parallaxEffect4, 0);

    }
}
