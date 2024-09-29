public static class Resources
{
    public static float SPIN_TIME = 0.2f;

    public static int MAX_LIVES = 3;
    public static float MAX_HEALTH = 100f;
    public static float MAX_ARMOUR = 100f;

    public enum Weapon {
        BASEBALL_BAT,
        HANDGUN,
        SHOTGUN,
        ASSAULT_RIFLE,
        SNIPER_RIFLE,
    }

    public enum Collectible {
        ARMOUR,
        LIFE,
        MEDPACK,
        GRENADES,
    }

    public static int GetAmmoForWeapon(Weapon weapon) {
        switch (weapon) {
            case Weapon.HANDGUN:
                return 30;
            case Weapon.SHOTGUN:
                return 10;
            case Weapon.ASSAULT_RIFLE:
                return 35;
            case Weapon.SNIPER_RIFLE:
                return 5;
            default:
                return 0;
        }
    }

    public static int GetAmountForCollectible(Collectible collectible) {
        switch (collectible) {
            case Collectible.ARMOUR:
                return 25;
            case Collectible.LIFE:
                return 1;
            case Collectible.MEDPACK:
                return 25;
            case Collectible.GRENADES:
                return 3;
            default:
                return 0;
        }
    }
}
