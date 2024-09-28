using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{

    // Exports
    public GameObject gameOverUI;
    
    // References
    UserInterface ui;
    
    // Start is called before the first frame update
    void Start()
    {
        ui = GameObject.Find("UserInterface").GetComponent<UserInterface>();
    }

    public void PickUpWeapon(Resources.Weapon weapon) {
        ui.PickUpWeapon(weapon);
    }

    public void ConsumeAmmo() {
        ui.ConsumeAmmo();
    }

    public void ConsumeGrenade() {
        ui.ConsumeGrenade();
    }

    public bool HasAmmo() {
        return ui.HasAmmo();
    }

    public bool HasGrenades() {
        return ui.HasGrenades();
    }

    public void RenderLives(int lives) {
        ui.RenderLives(lives);
    }

    public void gameOver() {
        gameOverUI.SetActive(true);
    }

    public void restart() {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }

}
