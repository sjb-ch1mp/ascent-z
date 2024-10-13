using UnityEngine;
using UnityEngine.UI;

public class RankScreen : MonoBehaviour
{

    // Exports
    public Sprite[] ranks;

    public void SetRank(Resources.Rank rank) {
        gameObject.SetActive(true);
        GetComponent<Image>().sprite = ranks[(int) rank];
    }

    public void Dismiss() {
        gameObject.SetActive(false);
    }

}
