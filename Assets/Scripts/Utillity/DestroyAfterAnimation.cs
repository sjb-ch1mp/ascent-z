using UnityEngine;

public class DestroyAfterAnimation : MonoBehaviour
{
    public void DestroyAfterAnimationCompleted() {
        // For animation events
        Destroy(gameObject);
    }
}
