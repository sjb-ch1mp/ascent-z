public static class Resources
{
    public static int MAX_LIVES = 3;

    public enum Weapon {
        BASEBALL_BAT,
        HANDGUN,
        SHOTGUN,
        ASSAULT_RIFLE,
        SNIPER_RIFLE,
        GRENADE,
    }

    public static int GetAmmoForWeapon(Weapon weapon) {
        switch (weapon) {
            case Weapon.GRENADE:
                return 3;
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

    public static bool IsRapidFire(Weapon weapon) {
        return weapon == Weapon.ASSAULT_RIFLE;
    }
}
