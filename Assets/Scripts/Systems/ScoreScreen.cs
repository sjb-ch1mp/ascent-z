using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class ScoreScreen : MonoBehaviour
{

    // Exports
    public Sprite[] killMedalSprites;
    public Sprite[] survivorMedalSprites;
    public Sprite[] reviveMedalSprites;

    // References
    Image revivesMedal;
    Image killsMedal;
    Image survivorMedal;

    void Start() {
        revivesMedal = transform.GetChild(1).gameObject.GetComponent<Image>();
        killsMedal = transform.GetChild(2).gameObject.GetComponent<Image>();
        survivorMedal = transform.GetChild(3).gameObject.GetComponent<Image>();
    }

    public void SetKillsMedal(Resources.MedalType medalType) {
        killsMedal.sprite = killMedalSprites[(int) medalType];
        killsMedal.gameObject.SetActive(true);
    }

    public void SetSurvivorMedal(Resources.MedalType medalType) {
        survivorMedal.sprite = survivorMedalSprites[(int) medalType];
        survivorMedal.gameObject.SetActive(true);
    }

    public void SetReviveMedal(Resources.MedalType medalType) {
        revivesMedal.sprite = reviveMedalSprites[(int) medalType];
        revivesMedal.gameObject.SetActive(true);
    }

    public void Dismiss() {
        revivesMedal.gameObject.SetActive(false);
        killsMedal.gameObject.SetActive(false);
        survivorMedal.gameObject.SetActive(false);
    }
}
