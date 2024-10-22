using UnityEngine;

public class LevelManager : MonoBehaviour
{
    public Checkpoint[] checkpoints;
    private int currentCheckpoint = 0;
    public static LevelManager Instance { get; private set; }
    public int Level { get; set; }

    InfectedPlatform[] infectedPlatforms; // These are the level boundaries
    LevelActivated[] levelActivatedObjects;

    // Destructive Singleton, i.e. will refresh upon load
    void Awake() {
        if (Instance == null)
        {
            Instance = this;
            int i = 0;
            foreach (Checkpoint check in checkpoints)
            {
                check.Assign(this, i);
                i++;
            }

            // Trigger the initial checkpoint
            checkpoints[0].Trigger();
            Level = 0;
        }
        else
        {
            Destroy(gameObject);
        }

    }

    public void SetCheckpoint(int checkpoint) {
        // Silently trigger all checkpoints until the current
        for (int i = 0; i < checkpoint; i++)
        {
            checkpoints[i].SilentTrigger();
        }
        currentCheckpoint = checkpoint;
    }

    public Vector3 GetSpawn() {
        return checkpoints[currentCheckpoint].GetPosition();
    }

    public void CompleteLevel() {
        Debug.Log("CompleteLevel()");
        StartCoroutine(infectedPlatforms[Level].Die());
        if (Level == infectedPlatforms.Length - 1) { // Last level - there's no upper bound
            ActivateNextLevel(infectedPlatforms[Level].transform.position.y, 0, true);
        } else if (Level <= infectedPlatforms.Length) {
            ActivateNextLevel(infectedPlatforms[Level].transform.position.y, infectedPlatforms[Level + 1].transform.position.y, false);
        } else {
            // Game is finished
        }
        Level++;
    }

    void ActivateNextLevel(float bottomBound, float topBound, bool isLastLevel) {
        foreach (LevelActivated l in levelActivatedObjects) {
            if (l != null) {
                if (isLastLevel) {
                    if (l.gameObject.transform.position.y > bottomBound) {
                        l.Activate();
                    } else {
                        l.Deactivate();
                    }
                } else {
                    if (l.gameObject.transform.position.y > bottomBound && l.gameObject.transform.position.y < topBound) {
                        l.Activate();
                    } else if (l.gameObject.transform.position.x < bottomBound) {
                        l.Deactivate();
                    }
                }
            }
        }
    }

    public void PrepareScene() {
        // Get all the checkpoints
        checkpoints = FindObjectsOfType<Checkpoint>();
        // Get all the infected platforms
        infectedPlatforms = FindObjectsOfType<InfectedPlatform>();
        Debug.Log($"Got {infectedPlatforms.Length} platforms");
        // Sort by height ascending
        for (int i=0; i<infectedPlatforms.Length; i++) {
            for (int j=i+1; j<infectedPlatforms.Length; j++) {
                if (infectedPlatforms[i].transform.position.y > infectedPlatforms[j].transform.position.y) {
                    InfectedPlatform hold = infectedPlatforms[i];
                    infectedPlatforms[i] = infectedPlatforms[j];
                    infectedPlatforms[j] = hold;
                }
            }
        }
        // Reset the level
        Level = 0;
        // Get a reference to all level activated objects so that they can be toggled on and off
        levelActivatedObjects = FindObjectsByType<LevelActivated>(FindObjectsSortMode.None);
        // Deactivate anything not in the first level
        foreach (LevelActivated l in levelActivatedObjects) {
            if (l != null) {
                if (l.gameObject.transform.position.y > infectedPlatforms[Level].transform.position.y) {
                    l.Deactivate();
                }
            }
        }
    }

}
