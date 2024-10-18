using UnityEngine;
using UnityEngine.SceneManagement;
using Cinemachine;

public class GameManager : MonoBehaviour
{

    // Exports
    public AudioClip gameOverSound;
    [SerializeField] private GameObject player;
    [SerializeField] private CinemachineVirtualCamera virtualCamera;

    // References
    UserInterface ui;
    ScoreManager scoreManager;
    AudioSource audioSource;

    // State
    bool paused = false;
    bool gameOver = false;
    int zombieSpawnerId = 0;
    private GameObject in_player;
    private GameOverScreen gameOverScreen;
    private LevelManager levelManager;

    public static GameManager Instance { get; private set; }

    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
            SceneManager.sceneLoaded += OnSceneLoaded;
        }
        else
        {
            Destroy(gameObject);
        }

    }

    void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        Debug.Log("Loaded");
        levelManager = LevelManager.Instance;
        ui = UserInterface.Instance;
        gameOverScreen = ui.gameOverScreen;
        scoreManager = new ScoreManager();
        audioSource = GetComponent<AudioSource>();
        audioSource.Play();
        SpawnPlayer(); 
    }

    void OnDestroy()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    public GameObject GetPlayer()
    {
        return in_player;
    }

    public void SpawnPlayer()
    {
        if (in_player == null)
        {
            in_player = Instantiate(player, Vector3.zero, Quaternion.identity);
        }

        // Ensure the player can move
        in_player.GetComponent<Rigidbody2D>().bodyType = RigidbodyType2D.Dynamic;
        in_player.transform.position = levelManager.GetSpawn();
        ui.PlayerSpawn();
        virtualCamera.Follow = in_player.transform;
        virtualCamera.LookAt = in_player.transform;
        //cameraTracking.ResetTo(in_player.transform.position);
        //cameraTracking.EnableTrigger();
    }

    // Zombie Spawner system
    public int GetNewZombieSpawnerId() {
        int id = zombieSpawnerId;
        zombieSpawnerId++;
        return id;
    }

    public void KillZombiesForSpawner(int id) {
        GameObject zombieContainer = GameObject.Find("Zombies");
        Enemy[] zombies = zombieContainer.GetComponentsInChildren<Enemy>();
        foreach (Enemy z in zombies) {
            if (z != null && z.GetSpawnerId() == id) {
                z.KillImmediately();
            }
        }
    }

    public bool SpawnCapReached(int id) {
        GameObject zombieContainer = GameObject.Find("Zombies");
        Enemy[] zombies = zombieContainer.GetComponentsInChildren<Enemy>();
        int zombieCount = 0;
        foreach (Enemy z in zombies) {
            if (z != null && z.GetSpawnerId() == id) {
                zombieCount++;
            }
        }
        return zombieCount >= Resources.MAX_ZOMBIES_PER_SPAWNER;
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

    public Resources.Weapon GetCurrentWeapon() {
        return ui.GetCurrentWeapon();
    }

    public void AddKillScore(int points) {
        scoreManager.KillScore += points;
    }

    public void AddSurvivorCount() {
        scoreManager.SurvivorCount++;
    }

    public void AddRevivesCount() {
        scoreManager.RevivesCount++;
    }

    public void ResetScore() {
        scoreManager.ResetScore();
    }

    public void RunScoreRoutine() {
        ui.RunScoreRoutine(
            scoreManager.KillScore,
            scoreManager.SurvivorCount, 
            scoreManager.RevivesCount,
            scoreManager.GetFinalScore()
        );
    }

    public int AddFinalScoreToRankProgress(int finalScore) {
        int increasedByRanks = scoreManager.AddFinalScoreToRankProgress(finalScore);
        if (increasedByRanks > 0) {
            ui.IncreaseRank(increasedByRanks);
        }
        return increasedByRanks;
    }

    public Resources.Rank GetCurrentRank() {
        return scoreManager.CurrentRank;
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
        ui.audioSource.PlayOneShot(gameOverSound);
        gameOver = true;
        gameOverScreen.GameOver();
    }

    public void Restart() {
        ui.audioSource.Stop();
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
        scoreManager.ResetScore();
        SpawnPlayer();
    }

}
