using UnityEngine;

public class LevelActivated : MonoBehaviour {

    public void Activate() {
        if (!gameObject.activeSelf) {
            gameObject.SetActive(true);
        }
    }

    public void Deactivate() {
        if (gameObject.activeSelf) {
            gameObject.SetActive(false);
        }
    }

}
