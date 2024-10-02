using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{

    // Exports
    public GameOverScreen gameOverScreen;

    
    // References
    UserInterface ui;

    // State
    bool paused = false;
    bool gameOver = false;
    
    // Start is called before the first frame update
    void Start()
    {
        ui = GameObject.Find("UserInterface").GetComponent<UserInterface>();
    }

    // UI functions
    public void PickUpWeapon(Resources.Weapon weapon) {
        ui.PickUpWeapon(weapon);
    }

    public void PickUpCollectible(Resources.Collectible collectible) {
        ui.PickUpCollectible(collectible);
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

    public void DepleteArmour() {
        ui.DepleteArmour();
    }

    // Game flow
    public void SetPaused(bool pauseGame) {
        paused = pauseGame;
    }

    public bool IsPaused() {
        return paused;
    }

    public bool IsGameOver() {
        return gameOver;
    }

    public void GameOver() {
        gameOver = true;
        gameOverScreen.GameOver();
    }

    public void Restart() {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }

}
