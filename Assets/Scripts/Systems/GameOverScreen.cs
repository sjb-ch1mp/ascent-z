using UnityEngine;
using UnityEngine.Rendering.PostProcessing;

public class GameOverScreen : MonoBehaviour
{

    public void GameOver() {
        // Disable UI        
        for ( int i = 0; i < 4; i++) {
            transform.parent.GetChild(i).gameObject.SetActive(false); // WeaponContainer --> ControlScheme
        }
        gameObject.SetActive(true);
        Camera.main.GetComponent<PostProcessLayer>().enabled = true;
    }

    public void TryAgain() {
        GameObject.Find("GameManager").GetComponent<GameManager>().Restart();
    }
}
