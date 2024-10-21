using System.Collections;
using UnityEngine;

public class DestroyAfterSound : MonoBehaviour
{
    AudioSource audioSource;

    void Start() {
        audioSource = GetComponent<AudioSource>();
        StartCoroutine(DestroyAfterSoundFinished());
    }

    IEnumerator DestroyAfterSoundFinished() {
        while (audioSource.isPlaying) {
            yield return new WaitForSeconds(0.25f);
        }
        Destroy(gameObject);
    }
}
