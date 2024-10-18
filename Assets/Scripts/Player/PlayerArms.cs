using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerArms : MonoBehaviour
{

    // Exports
    public Vector2[] bulletOffsets; // Must be in the order of Resources.Weapon
    public ProjectileBehaviour projectilePrefab;

    // References
    Animator animator;
    GameManager gameManager;

    // State
    Weapon currentWeapon;
    bool canShoot = true;
    float angle = 0f;

    void Awake() {
        gameManager = GameManager.Instance;
        currentWeapon = Weapon.BaseballBat();
        animator = GetComponent<Animator>();
    }

    // =========== Functions
    public void FireWeapon() {
        if (canShoot) {
            StartCoroutine(ShootCooldown(currentWeapon.cooldown)); // Block further shooting
            gameManager.ConsumeAmmo();
            // Calculate the direction from the player to the mouse cursor
            Vector2 mouseWorldPosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            Vector2 direction = (mouseWorldPosition - GetBulletOffset()).normalized;
            if (currentWeapon.type == Resources.Weapon.SHOTGUN) {
                // Shotgun is unique because it has a spread
                int projectileCount = 12;
                float spreadAngle = 30f; // Total spread angle in degrees
                float angleStep = spreadAngle / (projectileCount - 1);
                float startingAngle = -spreadAngle / 2;
                for (int i=0; i<projectileCount; i++) {
                    float currentAngle = startingAngle + (angleStep * i);
                    Quaternion spreadRotation = Quaternion.Euler(0, 0, currentAngle);
                    Vector2 spreadDirection = spreadRotation * direction;
                    // Calculate the angle for projectile rotation
                    //float angle = Mathf.Atan2(spreadDirection.y, spreadDirection.x) * Mathf.Rad2Deg;// - 90f;
                    Quaternion projectileRotation = Quaternion.Euler(0, 0, angle);
                    ProjectileBehaviour projectile = Instantiate(projectilePrefab, GetBulletOffset(), projectileRotation);
                    projectile.SetCharacteristics(currentWeapon, spreadDirection);
                }
            } else {
                // All other weapons simply fire a single projectile
                //float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg - 90f; // init projectile with direction
                Quaternion rotation = Quaternion.Euler(0, 0, angle);
                ProjectileBehaviour projectile = Instantiate(projectilePrefab, GetBulletOffset(), rotation);
                projectile.SetCharacteristics(currentWeapon, direction);
                if (currentWeapon.type == Resources.Weapon.BASEBALL_BAT) {
                    animator.SetTrigger("baseballBatAttack");
                }
            }
        }
    }

    Vector2 GetBulletOffset() {
        Vector2 offset = bulletOffsets[(int) currentWeapon.type];
        return new Vector2(
            transform.position.x + offset.x,
            transform.position.y + offset.y
        );
    }

    public void SetAngle(float angle) {
        this.angle = angle;
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

        Weapon(
            Resources.Weapon type, 
            bool rapidFire, 
            float cooldown, 
            int damage, 
            float projectileSpeed,
            float projectileSize,
            bool projectileVisible,
            float projectileRange,
            float projectilePenetration
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
        }

        public static Weapon BaseballBat() {
            return new Weapon(Resources.Weapon.BASEBALL_BAT, false, 0.5f, 5, 15, 9, /* DEBUG false*/true, 0.8f, Mathf.Infinity);
        }

        public static Weapon Handgun() {
            return new Weapon(Resources.Weapon.HANDGUN, false, 0.25f, 15, 15, 3, true, -1, 1);
        }

        public static Weapon Shotgun() {
            return new Weapon(Resources.Weapon.SHOTGUN, false, 1.0f, 25, 10, 1.5f, true, -1, 2);
        }

        public static Weapon AssaultRifle() {
            return new Weapon(Resources.Weapon.ASSAULT_RIFLE, true, 0.1f, 10, 10, 2.25f, true, -1, 1);
        }

        public static Weapon SniperRifle() {
            return new Weapon(Resources.Weapon.SNIPER_RIFLE, false, 1.25f, 100, 25, 5f, true, -1, 4);
        }
    }
}
