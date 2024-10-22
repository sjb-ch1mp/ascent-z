using System.Collections.Generic;

public static class Resources
{
    public static float SPIN_TIME = 0.2f;
    public const int MAX_RANK_PROGRESS = 2500;
    public static int MAX_LIVES = 3;
    public static float MAX_HEALTH = 100f;
    public static float MAX_ARMOUR = 100f;
    public static int MAX_ZOMBIES_PER_SPAWNER = 50;
    public static int MAX_AMMO = 100;
    public static float GRAVITY_SCALE = 5;

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
        AMMUNITION,
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
                return 50;
            case Collectible.LIFE:
                return 1;
            case Collectible.MEDPACK:
                return 25;
            case Collectible.GRENADES:
                return 3;
            case Collectible.AMMUNITION:
                return 0; // Not needed, the player just picks up the same weapon
            default:
                return 0;
        }
    }

    public static Dictionary<Weapon, int> GetAmmoCache() {
        return new Dictionary<Weapon, int>(){
            {Weapon.HANDGUN, 0},
            {Weapon.SHOTGUN, 0},
            {Weapon.ASSAULT_RIFLE, 0},
            {Weapon.SNIPER_RIFLE, 0},
        };
    }

    // Medal calculators
    public enum MedalType {
        Bronze,
        Silver,
        Gold
    }
    public static MedalType GetMedalForKillScore(int score) {
        if (score <= 500) {
            return MedalType.Bronze;
        } else if (score <= 1000) {
            return MedalType.Silver;
        } else {
            return MedalType.Gold;
        }
    }
    public static MedalType GetMedalForSurvivorCount(int score) {
        if (score <= 3) {
            return MedalType.Bronze;
        } else if (score <= 5) {
            return MedalType.Silver;
        } else {
            return MedalType.Gold;
        }
    }
    public static MedalType GetMedalForReviveScore(int score) {
        if (score == 0) {
            return MedalType.Gold;
        } else if (score == 1) {
            return MedalType.Silver;
        } else {
            return MedalType.Bronze;
        }
    }

    // Ranks
    public enum Rank {
        Private,
        Corporal,
        Sergeant,
        WarrantOfficerClass1,
        WarrantOfficerClass2,
        SecondLieutenant,
        Lieutenant,
        Captain,
        Major,
        LieutenantColonel,
        Colonel,
        Brigadier,
        MajorGeneral,
        LieutenantGeneral,
        General,
        FieldMarshal
    }

    public static string GetNameForRank(Rank rank) {
        switch (rank) {
            case Rank.Private: 
                return "Private";
            case Rank.Corporal:
                return "Corporal";
            case Rank.Sergeant:
                return "Sergeant";
            case Rank.WarrantOfficerClass1:
                return "Warrant Officer Class One";
            case Rank.WarrantOfficerClass2:
                return "Warrant Officer Class Two";
            case Rank.SecondLieutenant:
                return "Second Lieutenant";
            case Rank.Lieutenant:
                return "Lieutenant";
            case Rank.Captain:
                return "Captain";
            case Rank.Major:
                return "Major";
            case Rank.LieutenantColonel:
                return "Lieutenant Colonel";
            case Rank.Colonel:
                return "Colonel";
            case Rank.Brigadier:
                return "Brigadier";
            case Rank.MajorGeneral:
                return "Major General";
            case Rank.LieutenantGeneral:
                return "Lieutenant General";
            case Rank.General:
                return "General";
            case Rank.FieldMarshal:
                return "Field Marshal";
        }
        return "";
    }
    
}
