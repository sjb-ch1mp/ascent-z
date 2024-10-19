using System.Collections;
using UnityEngine;

public class PlayerArms : MonoBehaviour
{

    // Exports
    public ProjectileBehaviour projectilePrefab;
    public AudioClip[] shootSounds; // Must be in the order of Resources.Weapon

    // References
    Animator animator;
    GameManager gameManager;
    AudioSource audioSource;
    Transform projectileContainer;

    // State
    Weapon currentWeapon;
    bool canShoot = true;
    Vector2 currentDirection = Vector2.zero;

    void Awake() {
        gameManager = GameManager.Instance;
        currentWeapon = Weapon.BaseballBat();
        animator = GetComponent<Animator>();
        projectileContainer = GameObject.Find("Projectiles").transform;
        audioSource = GetComponent<AudioSource>();
    }

    // =========== Functions
    public void FireWeapon() {
        if (canShoot) {
            audioSource.PlayOneShot(shootSounds[(int) currentWeapon.type]);
            StartCoroutine(ShootCooldown(currentWeapon.cooldown)); // Block further shooting
            if (currentWeapon.type != Resources.Weapon.BASEBALL_BAT && gameManager.HasAmmo()) {
                gameManager.ConsumeAmmo();
            }
            if (currentWeapon.type == Resources.Weapon.SHOTGUN) {
                // Shotgun is unique because it has a spread
                int projectileCount = 12;
                float spreadAngle = 30f; // Total spread angle in degrees
                float angleStep = spreadAngle / (projectileCount - 1);
                float startingAngle = -spreadAngle / 2;
                for (int i=0; i<projectileCount; i++) {
                    float currentAngle = startingAngle + (angleStep * i);
                    Quaternion spreadRotation = Quaternion.Euler(0, 0, currentAngle);
                    Vector2 spreadDirection = spreadRotation * currentDirection;
                    // Calculate the angle for projectile rotation
                    float angle = Mathf.Atan2(spreadDirection.y, spreadDirection.x) * Mathf.Rad2Deg - 90f;
                    Quaternion projectileRotation = Quaternion.Euler(0, 0, angle);
                    ProjectileBehaviour projectile = Instantiate(projectilePrefab, transform.position, projectileRotation, projectileContainer);
                    projectile.SetCharacteristics(currentWeapon, spreadDirection);
                }
            } else {
                // All other weapons simply fire a single projectile
                float angle = Mathf.Atan2(currentDirection.y, currentDirection.x) * Mathf.Rad2Deg - 90f; // init projectile with direction
                Quaternion rotation = Quaternion.Euler(0, 0, angle);
                ProjectileBehaviour projectile = Instantiate(projectilePrefab, transform.position, rotation, projectileContainer);
                projectile.SetCharacteristics(currentWeapon, currentDirection);
                if (currentWeapon.type == Resources.Weapon.BASEBALL_BAT) {
                    animator.SetTrigger("baseballBatAttack");
                }
            }
        }
    }

    public bool RapidFire() {
        return currentWeapon.rapidFire;
    }

    public void SetDirection(Vector2 direction) {
        currentDirection = direction;
    }

    // PickUpWeapon changes the current weapon that the player is holding
    public void PickUpWeapon(Resources.Weapon newWeapon) {
        if (currentWeapon.type != newWeapon) {
            ResetAnimator();
            switch (newWeapon) {
                case Resources.Weapon.BASEBALL_BAT:
                    currentWeapon = Weapon.BaseballBat();
                    break;
                case Resources.Weapon.HANDGUN:
                    currentWeapon = Weapon.Handgun();
                    animator.SetBool("hasHandgun", true);
                    break;
                case Resources.Weapon.SHOTGUN:
                    currentWeapon = Weapon.Shotgun();
                    animator.SetBool("hasShotgun", true);
                    break;
                case Resources.Weapon.ASSAULT_RIFLE:
                    currentWeapon = Weapon.AssaultRifle();
                    animator.SetBool("hasAssaultRifle", true);
                    break;
                case Resources.Weapon.SNIPER_RIFLE:
                    currentWeapon = Weapon.SniperRifle();
                    animator.SetBool("hasSniperRifle", true);
                    break;
            }
        }
    }

    void ResetAnimator() {
        animator.SetBool("hasHandgun", false);
        animator.SetBool("hasShotgun", false);
        animator.SetBool("hasAssaultRifle", false);
        animator.SetBool("hasSniperRifle", false);
    }

    IEnumerator ShootCooldown(float cooldownTime) {
        canShoot = false;
        yield return new WaitForSeconds(cooldownTime);
        canShoot = true;
    }

    // Container class for weapons
    public class Weapon {
        public Resources.Weapon type { get; set; }
        public bool rapidFire { get; set; }
        public float cooldown { get; set; }
        public int damage { get; set; }
        public float projectileSpeed { get; set; }
        public float projectileSize { get; set; }
        public bool projectileVisible { get; set; }
        public float projectileRange { get; set; }
        public float projectilePenetration { get; set; }
        public float projectilePushback { get; set; }

        Weapon(
            Resources.Weapon type, 
            bool rapidFire, 
            float cooldown, 
            int damage, 
            float projectileSpeed,
            float projectileSize,
            bool projectileVisible,
            float projectileRange,
            float projectilePenetration,
            float projectilePushback
        ) {
            this.type = type;
            this.rapidFire = rapidFire;
            this.cooldown = cooldown;
            this.damage = damage;
            this.projectileSpeed = projectileSpeed;
            this.projectileSize = projectileSize;
            this.projectileVisible = projectileVisible;
            this.projectileRange = projectileRange;
            this.projectilePenetration = projectilePenetration;
            this.projectilePushback = projectilePushback;
        }

        public static Weapon BaseballBat() {
            return new Weapon(Resources.Weapon.BASEBALL_BAT, false, 0.5f, 10, 30, 9, false, 0.8f, 0, 5);
        }

        public static Weapon Handgun() {
            return new Weapon(Resources.Weapon.HANDGUN, false, 0.25f, 30, 25, 3, true, -1, 0, 3);
        }

        public static Weapon Shotgun() {
            return new Weapon(Resources.Weapon.SHOTGUN, false, 1.0f, 50, 10, 1.5f, true, -1, 0, 5);
        }

        public static Weapon AssaultRifle() {
            return new Weapon(Resources.Weapon.ASSAULT_RIFLE, true, 0.1f, 30, 30, 2.25f, true, -1, 0, 3);
        }

        public static Weapon SniperRifle() {
            return new Weapon(Resources.Weapon.SNIPER_RIFLE, false, 1.25f, 200, 30, 5f, true, -1, 2, 7);
        }
    }
}
