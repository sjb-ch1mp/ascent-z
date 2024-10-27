using UnityEngine;

using System.Diagnostics;

public class LevelActivated : MonoBehaviour {

    public void Activate() {
        StackTrace st = new StackTrace();
        UnityEngine.Debug.Log(st.ToString());
        UnityEngine.Debug.Log("Activating");
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
