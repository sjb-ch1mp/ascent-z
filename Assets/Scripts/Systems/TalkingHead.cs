using System.Collections;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;

public class TalkingHead : MonoBehaviour
{
    // References
    public TextMeshProUGUI messageBox;

    // Exports
    public float characterDelay = 0.02f;
    public AudioClip radioActivate;
    public AudioClip morseCode;
    public AudioSource audioSource;

    GameManager gameManager;

    void Awake() {
        gameManager = GameObject.Find("GameManager").GetComponent<GameManager>();
    }

    public void Dismiss() {
        gameManager.SetPaused(false);
        audioSource.Stop();
        audioSource.PlayOneShot(radioActivate);
        StopAllCoroutines();
        StartCoroutine(WaitForRadioSoundBeforeClose());
    }

    IEnumerator WaitForRadioSoundBeforeClose() {
        while (audioSource.isPlaying) {
            yield return new WaitForSeconds(characterDelay);
        }
        gameObject.SetActive(false);
    }

    public void NewMessage(string message) {
        if (!gameObject.activeSelf) {
            gameObject.SetActive(true);
            gameManager.SetPaused(true);
        }
        audioSource.PlayOneShot(radioActivate);
        StartCoroutine(RevealMessage(message));
    }

    IEnumerator RevealMessage(string message) {
        int len = 1;
        string messageRevealed = message.Substring(0, len);
        while (len < message.Length) {
            if (!audioSource.isPlaying) {
                audioSource.clip = morseCode;
                audioSource.Play();
            }
            messageBox.text = messageRevealed;
            yield return new WaitForSeconds(characterDelay);
            len++;
            messageRevealed = message.Substring(0, len);
        }
    }
}
