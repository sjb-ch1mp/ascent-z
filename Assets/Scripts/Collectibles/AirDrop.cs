using UnityEngine;

public class AirDrop : MonoBehaviour
{
    public Transform startPositionTransform;
    public Animator animator;
    public AudioSource audioSource;
    
    Vector2 startPosn;
    Vector2 dest;
    int type;
    bool landed;
    float fallSpeed = 5;

    void Start() {
        dest = transform.position;
        startPosn = startPositionTransform.position;
        transform.position = startPosn;
        if (GetComponent<UtilityCrate>() != null) {
            type = (int) GetComponent<UtilityCrate>().CollectibleType;
            animator.SetInteger("collectibleType", type + 1); // If set to zero, animation immediately transitions
        } else {
            type = (int) GetComponent<WeaponsCache>().WeaponType;
            animator.SetInteger("weaponType", type);
        }
    }
    
    void Update() {
        if (!landed) {
            transform.position = Vector2.MoveTowards(transform.position, new Vector2(transform.position.x, dest.y), Time.deltaTime * fallSpeed);
            if (transform.position.y <= dest.y) {
                landed = true;
                animator.SetTrigger("isLanded");
                audioSource.Stop();
            }
        }
    }
}
