using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Title : MonoBehaviour
{

    public AudioClip startSound;
    AudioSource audioSource;

    void Awake() {
        audioSource = GetComponent<AudioSource>();
    }

    public void StartGame() {
        audioSource.Stop();
        audioSource.PlayOneShot(startSound);
        StartCoroutine(StartAfterShot()); 
    }

    IEnumerator StartAfterShot() {
        while (audioSource.isPlaying) {
            yield return new WaitForSeconds(0.1f);
        }
        SceneManager.LoadScene("Level");
    }
}
