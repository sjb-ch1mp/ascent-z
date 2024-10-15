using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class TalkingHead : MonoBehaviour
{
    // Exports
    public TextMeshProUGUI messageBox;
    public float characterDelay = 0.05f;
    public AudioClip radioActivate;
    public AudioClip morseCode;
    public AudioClip receiveMedal;
    public AudioClip increaseRank;
    public AudioSource audioSource;

    // References
    GameManager gameManager;
    GameObject scoreScreen;
    GameObject rankScreen;
    Image image;

    // Enum
    public enum MessageDestination {
        Communication,
        Score
    }

    // State
    public bool IsTalking { get; set; }

    void Awake() {
        gameManager = GameObject.Find("GameManager").GetComponent<GameManager>();
        scoreScreen = transform.GetChild(2).gameObject;
        rankScreen = transform.GetChild(3).gameObject;
        image = transform.GetChild(4).gameObject.GetComponent<Image>();
    }

    public void Dismiss() {
        gameManager.SetPaused(false);
        audioSource.Stop();
        audioSource.PlayOneShot(radioActivate);
        StopAllCoroutines();
        if (scoreScreen.activeSelf) { // This was a score screen message
            gameManager.ResetScore();
            scoreScreen.GetComponent<ScoreScreen>().Dismiss();
            scoreScreen.SetActive(false);
        }
        if (rankScreen.activeSelf) {
            rankScreen.GetComponent<RankScreen>().Dismiss();
        }
        if (image.gameObject.activeSelf) {
            image.gameObject.SetActive(false);
        }
        StartCoroutine(WaitForRadioSoundBeforeClose());
    }

    IEnumerator WaitForRadioSoundBeforeClose() {
        while (audioSource.isPlaying) {
            yield return new WaitForSeconds(characterDelay);
        }
        IsTalking = false;
        gameObject.SetActive(false);
    }

    public void NewMessage(string message, MessageDestination destination, Sprite messageImage) {
        gameManager.SetPaused(true);
        if (!gameObject.activeSelf) {
            gameObject.SetActive(true);
        }
        if (destination == MessageDestination.Communication) {
            audioSource.PlayOneShot(radioActivate);
        }
        if (messageImage != null) {
            messageBox.fontSize = 16; // smaller font to allow for an image
            image.sprite = messageImage;
            image.gameObject.SetActive(true);
        } else {
            messageBox.fontSize = 18;
        }
        StartCoroutine(RevealMessage(message, destination));
    }

    IEnumerator RevealMessage(string message, MessageDestination destination) {

        IsTalking = true;

        // Send the message to the correct text panel
        TextMeshProUGUI targetPanel = null;
        switch (destination) {
            case MessageDestination.Communication:
                targetPanel = messageBox;
                break;
            case MessageDestination.Score:
                targetPanel = scoreScreen.transform.GetChild(4).gameObject.GetComponent<TextMeshProUGUI>();
                break;
        }

        // Show the message
        if (targetPanel != null) {
            int len = 1;
            string messageRevealed = message.Substring(0, len);
            while (len < message.Length) {
                if (!audioSource.isPlaying) {
                    audioSource.clip = morseCode;
                    audioSource.Play();
                }
                targetPanel.text = messageRevealed;
                yield return new WaitForSeconds(characterDelay);
                len++;
                messageRevealed = message.Substring(0, len);
            }
        }

        IsTalking = false;
    }

    public void ShowScore(int killScore, int survivorCount, int reviveCount, int finalScore) {
        NewMessage($"Stand by for a situation report... .", MessageDestination.Communication, null);
        StartCoroutine(RevealScore(killScore, survivorCount, reviveCount, finalScore));
    }

    IEnumerator RevealScore(int killScore, int survivorCount, int reviveCount, int finalScore) {
        // Wait for the commander to finish
        while (IsTalking) {
            yield return new WaitForSeconds(0.25f);
        }
        ScoreScreen scoreController = scoreScreen.GetComponent<ScoreScreen>();
        scoreScreen.SetActive(true);
        // Show revives score
        NewMessage($"Revives used: {reviveCount}.", MessageDestination.Score, null);
        while (IsTalking) {
            yield return new WaitForSeconds(0.25f);
        }
        audioSource.PlayOneShot(receiveMedal);
        scoreController.SetReviveMedal(Resources.GetMedalForReviveScore(reviveCount));
        yield return new WaitForSeconds(1f);
        // Show kill score
        NewMessage($"Kill score: {killScore}.", MessageDestination.Score, null);
        while (IsTalking) {
            yield return new WaitForSeconds(0.25f);
        }
        audioSource.PlayOneShot(receiveMedal);
        scoreController.SetKillsMedal(Resources.GetMedalForKillScore(killScore));
        yield return new WaitForSeconds(1f);
        // Show survivor
        NewMessage($"Survivors rescued: {survivorCount}.", MessageDestination.Score, null);
        while (IsTalking) {
            yield return new WaitForSeconds(0.25f);
        }
        audioSource.PlayOneShot(receiveMedal);
        scoreController.SetSurvivorMedal(Resources.GetMedalForSurvivorCount(survivorCount));
        yield return new WaitForSeconds(1f);
        // Show final score
        NewMessage($"Final score: {finalScore}.", MessageDestination.Score, null);
        while (IsTalking) {
            yield return new WaitForSeconds(0.25f);
        }

        int increasedByRanks = gameManager.AddFinalScoreToRankProgress(finalScore);
        if (increasedByRanks > 0) {
            StartCoroutine(DoPromotion(increasedByRanks));
        } else {
            if (finalScore < 250) {
                NewMessage("Pathetic. Get back out there and try not to die, greenhorn. .", MessageDestination.Communication, null);
            } else if (finalScore < 500) {
                NewMessage("Nice work, soldier. But you'll have to do better than that if you want to survive in this hellhole. Over and out. .", MessageDestination.Communication, null);
            } else {
                NewMessage("Outstanding work, soldier. We need more recruits like you for this recovery effort. Over and out. .", MessageDestination.Communication, null);
            }
        }
        
    }

    IEnumerator DoPromotion(int increasedByRanks) {
        NewMessage($"Phenomenal effort, soldier. You've been promoted {((increasedByRanks > 1) ? $"by {increasedByRanks} ranks" : "" )} to {Resources.GetNameForRank(gameManager.GetCurrentRank())} ...", MessageDestination.Communication, null);
        while (IsTalking) {
            yield return new WaitForSeconds(0.5f);
        }
        audioSource.PlayOneShot(increaseRank);
        rankScreen.GetComponent<RankScreen>().SetRank(gameManager.GetCurrentRank());
    }
}
