using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraTracking : MonoBehaviour
{
    [SerializeField] private Camera cameraTarget;
    private BoxCollider2D triggerZone;
    private GameObject player;

    void Awake()
    {
        triggerZone = GetComponent<BoxCollider2D>();
        ResizeToCamera();
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
