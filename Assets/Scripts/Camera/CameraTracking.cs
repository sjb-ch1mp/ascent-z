using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraTracking : MonoBehaviour
{
    [SerializeField] private Camera cameraTarget;
    [SerializeField] private BoxCollider2D triggerZone;
    private GameObject player;
    public static CameraTracking Instance { get; private set; }

    // Destructive Singleton, i.e. will refresh upon load
    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            ResizeToCamera();
        }
        else
        {
            Destroy(gameObject);
        }

    }

    public void DisableTrigger()
    {
        triggerZone.enabled = false;
    }

    public void EnableTrigger()
    {
        triggerZone.enabled = true;
    }

    // Update is called once per frame
    void Update()
    {
        if (player == null)
        {
            player = GameManager.Instance.GetPlayer();
            // No active player
            if (player == null)
            {
                return;
            }
        }
        
        Vector3 pos = player.transform.position;
        if (pos.y < gameObject.transform.position.y)
        {
            pos.y = gameObject.transform.position.y;
        }
       
        gameObject.transform.position = pos;
    }

    public void ResetTo(Vector3 pos)
    {
        gameObject.transform.position = pos;
    }

    public void ResizeToCamera()
    {
        float cameraHeight = cameraTarget.orthographicSize * 2;
        float cameraWidth = cameraHeight * cameraTarget.aspect;

        // Adjust the size of the BoxCollider2D to match the camera's view
        triggerZone.size = new Vector2(cameraWidth, cameraHeight);
    }
}
