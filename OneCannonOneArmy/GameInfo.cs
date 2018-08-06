using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;

namespace OneCannonOneArmy
{
    public static class GameInfo
    {
        #region Properties

        public static Version Version = new Version(1, 0, 0);
        public static Language Language = Language.English;

        public static Planet Planet = Planet.Earth;

        public const int FINAL_LEVEL = 25;
        public const int FINAL_THREE_CONST = 3;

        // Gift constants
        public const int COIN_GIFT_MULTIPLIER_MIN = 10;
        public const int COIN_GIFT_MULTIPLIER_MAX = 30;
        public const int GIFT_TYPE_PERCENT_MULTIPLIER = 16;
        public const int HOURS_UNTIL_NEXT_GIFT = 24;

        // Color values for different planets
        public static Color EARTH_BG = new Color(178, 220, 240);
        public static Color EARTH_GROUND = Color.Green;
        public static Color NANTAK_BG = Color.Purple;
        public static Color NANTAK_GROUND = Color.Brown;
        public static Color CARINUS_BG = Color.RosyBrown;
        public static Color CARINUS_GROUND = Color.SaddleBrown;
        public static Color MIKARA_BG = Color.Orchid;
        public static Color MIKARA_GROUND = Color.Teal;
        public static Color SPACE_BG = new Color(40, 40, 40);
        public static Color SPACE_GROUND = new Color(30, 30, 30);
        public static Color LITHIOS_BG = new Color(222, 37, 37);
        public static Color LITHIOS_GROUND = new Color(70, 70, 70);

        // Initial user values
        public const int HOTBAR_SLOTS = 5;
        public const int STARTING_USER_COINS = 10;
        public const int MAX_LIVES = 3;

        // Death & revival values
        public static DateTime INIT_USER_LAST_DEATH_TIME = new DateTime(1, 1, 1, 0, 0, 0, 0);
        public const int MINUTES_UNTIL_NEXT_LIFE = 30;
        public const int DEATH_COST_PER_MIN = 2;

        public const int STARTING_MAX_HEALTH = 100;
        public const int STARTING_CANNON_SPD = 4;
        public const int STARTING_CANNON_DAMAGE = 0;
        public const int STARTING_CANNON_ACCURACY = 0;
        public const int STARTING_CANNON_RELOAD_SPD = 2;
        public const CannonType STARTING_CANNON_TYPE = CannonType.Normal;

        public const float SELL_DEPRECIATION_PCT = 0.5f;

        // Alien health
        public const int NORMAL_ALIEN_HEALTH = 3;
        public const int DEFENSE_ALIEN_HEALTH = 6;
        public const int LDEFENSE_ALIEN_HEALTH = 5;
        public const int HDEFENSE_ALIEN_HEALTH = 7;
        public const int FIRE_DEFENSE_ALIEN_HEALTH = 6;
        public const int LFIRE_DEFENSE_ALIEN_HEALTH = 5;
        public const int HFIRE_DEFENSE_ALIEN_HEALTH = 7;
        public const int POISON_RESIST_ALIEN_HEALTH = 6;
        public const int LPOISON_RESIST_ALIEN_HEALTH = 5;
        public const int HPOISON_RESIST_ALIEN_HEALTH = 7;
        public const int FREEZE_PROOF_ALIEN_HEALTH = 7;
        public const int CHAOS_ALIEN_HEALTH = 25;
        public const int BOSS_ALIEN_HEALTH = 100;
        public const int PLASMA_RESIST_ALIEN_HEALTH = 6;
        public const int LPLASMA_RESIST_ALIEN_HEALTH = 5;
        public const int HPLASMA_RESIST_ALIEN_HEALTH = 7;
        public const int OMEGA_ALIEN_HEALTH = 30;
        public const int NINJA_ALIEN_HEALTH = 25;

        // Alien worth
        public const int NORMAL_ALIEN_WORTH = 4;
        public const int DEFENSE_ALIEN_WORTH = 12;
        public const int LDEFENSE_ALIEN_WORTH = 7;
        public const int HDEFENSE_ALIEN_WORTH = 29;
        public const int FIRE_DEFENSE_ALIEN_WORTH = 7;
        public const int LFIRE_DEFENSE_ALIEN_WORTH = 4;
        public const int HFIRE_DEFENSE_ALIEN_WORTH = 15;
        public const int POISON_RESIST_ALIEN_WORTH = 6;
        public const int LPOISON_RESIST_ALIEN_WORTH = 3;
        public const int HPOISON_RESIST_ALIEN_WORTH = 14;
        public const int FREEZE_PROOF_ALIEN_WORTH = 10;
        public const int CHAOS_ALIEN_WORTH = 31;
        public const int BOSS_ALIEN_WORTH = 150;
        public const int PLASMA_RESIST_ALIEN_WORTH = 8;
        public const int LPLASMA_RESIST_ALIEN_WORTH = 5;
        public const int HPLASMA_RESIST_ALIEN_WORTH = 16;
        public const int OMEGA_ALIEN_WORTH = 35;
        public const int NINJA_ALIEN_WORTH = 28;

        public const int CAGE_WORTH = 25;
        public const int MECH_WORTH = 5;
        public const int LASER_CANNON_WORTH = 50;

        // Defense values
        public const float LIGHT_DEFENSE = 0.15f;
        public const float NORMAL_DEFENSE = 0.45f;
        public const float HEAVY_DEFENSE = 0.75f;
        public const float OMEGA_DEFENSE = 0.5f;
        public const float BOSS_DEFENSE = 0.85f;

        // Projectile damages
        public const int ROCK_DMG = 3;
        public const int CANNONBALL_DMG = 4;
        public static readonly int FIREBALL_DMG = StatusEffect.Fire.Damage;
        public const int BOMB_DMG = 9;
        public const int DART_DMG = 4;
        public const int LASER_DMG = 10;
        public const int LIGHTNING_DMG = 8;
        public const int CHAOS_DMG = 11;
        public const int FROZENBLAST_DMG = 5;
        public const int METEOR_DMG = 10;
        public const int HAMMER_DMG = 6;
        public const int SNOWBALL_DMG = 2;
        public const int ROCKET_DMG = 13;
        public const int BONE_DMG = 6;
        public const int SHURIKEN_DMG = 5;
        public const int ICESHARD_DMG = 6;
        public const int ABSORBHEX_DMG = 8;
        public const int ABSORBHEX_HEAL = 4;

        // Explosion constants
        public const float AREA_DMG_MULTIPLIER = 0.25f;
        public const float AREA_OF_EFFECT = 150;

        // Projectile speeds
        public const int ROCK_SPD = 4;
        public const int CANNONBALL_SPD = 6;
        public const int FIREBALL_SPD = 5;
        public const int BOMB_SPD = 3;
        public const int DART_SPD = 20;
        public const int LASER_SPD = 30;
        public const int LIGHTNING_SPD = 20;
        public const int CHAOS_SPD = 3;
        public const int FROZENBLAST_SPD = 3;
        public const int METEOR_SPD = 8;
        public const int HAMMER_SPD = 6;
        public const int SNOWBALL_SPD = 6;
        public const int ROCKET_SPD = 10;
        public const int BONE_SPD = 5;
        public const int SHURIKEN_SPD = 7;
        public const int ICESHARD_SPD = 20;
        public const int ABSORBHEX_SPD = 3;

        // Projectile worths
        public const int ROCK_WORTH = 2;
        public const int CANNONBALL_WORTH = 3;
        public const int DART_WORTH = 6;
        public const int POISONDART_WORTH = 11;
        public const int BOMB_WORTH = 7;
        public const int FIREBALL_WORTH = 8;
        public const int LIGHTNING_WORTH = 16;
        public const int CHAOS_WORTH = 14;
        public const int LASER_WORTH = 35;
        public const int FROZENBLAST_WORTH = 7;
        public const int METEOR_WORTH = 15;
        public const int HAMMER_WORTH = 4;
        public const int SNOWBALL_WORTH = 6;
        public const int ROCKET_WORTH = 10;
        public const int POISON_ROCKET_WORTH = 14;
        public const int FIRE_ROCKET_WORTH = 18;
        public const int FROZEN_ROCKET_WORTH = 14;
        public const int PLASMA_ROCKET_WORTH = 30;
        public const int OMEGA_ROCKET_WORTH = 40;
        public const int BONE_WORTH = 5;
        public const int SHURIKEN_WORTH = 8;
        public const int ICESHARD_WORTH = 8;

        // Projectile costs (depricated)
        //public const int ROCK_COST = 2;
        //public const int CANNONBALL_COST = 3;
        //public const int DART_COST = 6;
        //public const int POISONDART_COST = 11;
        //public const int BOMB_COST = 7;
        //public const int FIREBALL_COST = 8;
        //public const int LIGHTNING_COST = 16;
        //public const int CHAOS_COST = 14;
        //public const int LASER_COST = 45;
        //public const int FROZENBLAST_COST = 7;
        //public const int METEOR_COST = 15;
        //public const int HAMMER_COST = 4;
        //public const int SNOWBALL_COST = 6;
        //public const int ROCKET_COST = 10;
        //public const int POISON_ROCKET_COST = 14;
        //public const int FIRE_ROCKET_COST = 18;
        //public const int FROZEN_ROCKET_COST = 14;
        //public const int PLASMA_ROCKET_COST = 30;
        //public const int OMEGA_ROCKET_COST = 55;

        // Material costs
        public const int STONE_COST = 1;
        public const int METAL_COST = 2;
        public const int POISON_COST = 4;
        public const int FIRE_COST = 5;
        public const int GUNPOWDER_COST = 3;
        public const int CHAOSENERGY_COST = 6;
        public const int PLASMA_COST = 7;
        public const int ICE_COST = 4;

        public const float COIN_SPD = 1.0f;
        public const float TUTORIAL_ALIEN_SPD = 0.5f;

        // Upgrade costs
        public const int INIT_DMG_COST = 10;
        public const int DMG_COST = 15;
        public const int INIT_ACCURACY_COST = 15;
        public const int ACCURACY_COST = 15;
        public const int INIT_HEALTH_COST = 5;
        public const int HEALTH_COST = 5;
        public const int INIT_RELOAD_SPD_COST = 10;
        public const int RELOAD_SPD_COST = 10;
        public const int INIT_MOVE_SPD_COST = 10;
        public const int MOVE_SPD_COST = 10;
        public const int RAPID_FIRE_COST = 1000;
        public const int INIT_POWER_COST = 20;
        public const int POWER_COST = 10;
        public const int INIT_DEFENSE_COST = 5;
        public const int DEFENSE_COST = 10;

        // Player levels and visibility
        public static readonly Dictionary<ProjectileType, int> ProjVisibilityLvls = new Dictionary<ProjectileType, int>()
        {
            { ProjectileType.Rock, 0 },
            { ProjectileType.Cannonball, 3 },
            { ProjectileType.Dart, 4 },
            { ProjectileType.PoisonDart, 6 },
            { ProjectileType.Bomb, 8 },
            { ProjectileType.Fireball, 9 },
            { ProjectileType.LightningBolt, 12 },
            { ProjectileType.Hex, 17 },
            { ProjectileType.Laser, 21 },
            { ProjectileType.FrostHex, 12 },
            { ProjectileType.Meteor, 16 },
            { ProjectileType.Hammer, 5 },
            { ProjectileType.Snowball, 9 },
            { ProjectileType.Rocket, 13 },
            { ProjectileType.PoisonRocket, 14 },
            { ProjectileType.FireRocket, 17 },
            { ProjectileType.FrozenRocket, 17 },
            { ProjectileType.PlasmaRocket, 19 },
            { ProjectileType.OmegaRocket, 23 },
            { ProjectileType.Bone, 4 },
            { ProjectileType.Shuriken, 6 },
            { ProjectileType.IceShard, 11 },
            { ProjectileType.AbsorbHex, 10 },
        };

        public static readonly List<ProjectileType> ProjectilesAllowed = new List<ProjectileType>()
        {
            ProjectileType.Rock,
            ProjectileType.Cannonball,
            ProjectileType.Fireball,
            ProjectileType.Bomb,
            ProjectileType.Dart,
            ProjectileType.PoisonDart,
            ProjectileType.Laser,
            ProjectileType.Hex,
            ProjectileType.LightningBolt,
            ProjectileType.FrostHex,
            ProjectileType.Meteor,
            ProjectileType.Hammer,
            ProjectileType.Rocket,
            ProjectileType.FireRocket,
            ProjectileType.PoisonRocket,
            ProjectileType.FrozenRocket,
            ProjectileType.PlasmaRocket,
            ProjectileType.OmegaRocket,
            ProjectileType.Snowball,
            ProjectileType.Shuriken,
            ProjectileType.Bone,
            ProjectileType.IceShard,
            ProjectileType.AbsorbHex,
        };
        public static readonly List<Material> MaterialsAllowed = new List<Material>()
        {
            Material.Stone,
            Material.Metal,
            Material.PlantMatter,
            Material.Gunpowder,
            Material.Ice,
            Material.EssenceOfFire,
            Material.ChaosEnergy,
            Material.Plasma,
        };

        public const int SHIELD_LVL = 8;
        public static readonly Dictionary<int, Aliens> AlienVisibilityLvls = new Dictionary<int, Aliens>()
        {
            { 0, Aliens.Normal },
            { 2, Aliens.LDefense },
            { 5, Aliens.Defense },
            { 7, Aliens.HDefense },
            { 9, Aliens.LFireDefense },
            { 10, Aliens.LPoisonResistant },
            { 11, Aliens.FireDefense },
            { 12, Aliens.FreezeProof },
            { 13, Aliens.PoisonResistant },
            { 14, Aliens.HFireDefense },
            { 15, Aliens.HPoisonResistant },
            { 18, Aliens.Chaos },
            { 19, Aliens.LPlasmaResistant },
            { 20, Aliens.PlasmaResistant },
            { 21, Aliens.Omega },
            { 22, Aliens.HPlasmaResistant },
            { 25, Aliens.Boss },
        };
        public static readonly Dictionary<CannonStats, int> MaxStats = new Dictionary<CannonStats, int>()
        {
            { CannonStats.Accuracy, 20 },
            { CannonStats.Damage, 25 },
            { CannonStats.Health, 500 },
            { CannonStats.MoveSpeed, 15 },
            { CannonStats.ReloadSpeed, 10 },
            { CannonStats.RapidFire, 1 },
            { CannonStats.Defense, 75 },
            { CannonStats.Power, 25 },
        };

        #region Story

        public static List<string> STORY1 = new List<string>()
        {
            "It is a dark time on Lithios.",
            "",
            "Resources are low, farms are failing, and Lithians are dying.",
            "A dangerous disease runs rampant. Death's dark grip is closing on the planet.",
            "",
            "The Lithian leader, Malos, sees no other option; they must colonize another planet.",
            "With an army of his strongest warriors, Malos heads out",
            "for a distant spiral galaxy. Upon reaching it, they find an inhabitable planet.",
            "The only problem: That planet is Earth, and it is already inhabited.",
            "Fortunately, the solar system is rich with",
            "planets and moons that can support the Lithians.",
            "Unfortunately, the Earthlings will likely defend their solar system with their lives.",
            "Malos devises a clever plan: Destroy Earth.",
            "",
            "After sending the message back home, construction",
            "begins on three deadly weapons on Lithios.",
            "Weapons with the power to ensure the Lithians' security in their new home.",
            "Weapons with the power to destroy a planet.",
            "However, one small, insignificant Lithian sees no purpose in destroying Earth.",
            "There was never an attempt to befriend the Earthlings;",
            "destroying them is ending more unnecessary lives, he thinks.",
            "So the alien, named Tucker, heads for Earth to",
            "assist the small planet in its defenses.",
            "",
            "Malos begins his attack. All is dark, hopeless.",
            "Only a single person notices.",
            "With a science experiment cannon and some small rocks,",
            "they alone must save the world.",
            "Alone, with no help.",
            "Or the Lithians will win.",
            "The planet will be destroyed.",
            "And humans will be exterminated.",
            "Forever."
        };

        public static List<string> STORY2 = new List<string>()
        {
            "It's over.",
            "",
            "Malos has been defeated.",
            "His lasers have been destroyed.",
            "His army has scattered.",
            "",
            "But what of the others of Lithios?",
            "What of those Malos tried so hard to save?",
            "",
            "Eventually, a they found a home.",
            "A small planet-sized moon they dubbed Lithios II.",
            "However, others might know it by its more common name:",
            "Europa, a moon of Jupiter.",
            "",
            "And the Lithians finally proved that they CAN live in peace",
            "with humans.",

            "And still today, if you look up in the sky,",
            "you may see a Lithian ship taking flight.",
            "",
            "They wave to us. They thank us.",
            "",
            "But really, we should be thanking them."
        };

        #endregion

        #region Credits

        public static readonly List<string> Credits = new List<string>()
        {
            "Created by Daniel Gaull",
            "\nPROGRAMMIMG",
            "Head Developer: Daniel Gaull",
            "Assistant Developers:",
            "Jim Gaull",
            "From Stack Overflow:",
            "Callum Rodgers",
            "Contango",
            "Cyral",
            "hunter",
            "Marc",
            "Omar Mahili", 
            "user1089284",
            "ART",
            "Artist: Daniel Gaull",
            "Many Thanks to the devlopers of paint.net,",
            "a free image-editing software application.",
            "\nSOUND EFFECTS",
            "From freesound.org:",
            "(some sound effects may have been modified from their original states)",
            "Ca Ching by guest",
            "Click by Swordmaster767",
            "Close 1 by simplen00b",
            "Deep Rattle 2 by ingudios",
            "Digital Life 1 by soneproject",
            "Effect Notify by ricemaster",
            "Electric Zap 001 by JoelAudio",
            "Explosion by Nbs Dark",
            "Fast Collision by qubodup",
            "Health Powerup by Randomation Pictures",
            "Laser Active Big by mazk1985",
            "Laser Bolt by peepholecircus",
            "Laser Charging by plasterbrain",
            "Level Up by Marregheriti",
            "Light Clunk 1 by BMacZero",
            "Metal Door Slam by Lunardrive",
            "Metal Machinery Rumbling in Large Basement by RutgerMuller",
            "Metallic Whoosh by MissCellany",
            "Money Pickup 2 by LeMudCrab",
            "Notify by HuntersCrossbow",
            "Notify by InfiniteLifespan",
            "Open 1 by simplen00b",
            "Punch Boxing 02 by newsagesoup",
            "Sci-Fi Spaceship Failure Beep by JapanYoshiTheGamer",
            "Short Fireball Whoosh by wjl",
            "Slap by Agaxly",
            "Slap 1 by Adam_N",
            "Slide by adcbicycle",
            "Splat 2 by gprosser",
            "Success by kimp10",
            "Tank Firing by qubodup",
            "Whistle and Explosion Single Firework by Rudmer_Rotteveel",
            "\nMUSIC",
            "From freesoung.org:",
            "Cinematic Boom by rhapsodize",
            "Epic Battle Music by Airwolf89",
            "Epic Buildup Loop by a_guy_1",
            "Man Meets Earth by Modestas Mankus",
            "Rising Tension by cheesepuff",
            "Space Chase by Romariogrande",
            "The Temple by nintend0wn",
            "From mattesar.com:",
            "Lifeline [Bane] by Mattesar",
            "\n\nPurasu Presents...",
        };

        #endregion

        #region Item Descriptions

        public static readonly Dictionary<Material, string> MaterialDescs = new Dictionary<Material, string>()
        {
            { Material.Stone,  "Almost-useless lump of earth. Can craft useful\nthings, though." },
            { Material.Metal,  "Slightly less-useless lump of earth. Basic\nmaterial; used to craft many powerful projectiles." },
            { Material.PlantMatter, "Collected from plants. Can be used for\nmaking poison, or healing concoctions." },
            { Material.Plasma, "Powerful red energy, used for magic." },
            { Material.Ice, "It can keep drinks cold, help you cool off on a hot\nday, and freeze aliens! That ice... so useful." },
            { Material.Gunpowder, "Explosive powder. When mixed with fire...Look out." },
            { Material.ChaosEnergy, "Almost uncontainable magical energy. If you can\ncontain it, magic!" +
                " If you can't... more dangerous\nmagic." },
            { Material.EssenceOfFire, "Magical fiery energy. Great for barbecuing!" },
        };

        public static readonly Dictionary<ProjectileType, string> ProjectileDescs = new Dictionary<ProjectileType, string>()
        {
            { ProjectileType.Rock, "Though weak, rocks are cheap and easy to make." },
            { ProjectileType.Cannonball, "Slightly stronger though more expensive than the rock." },
            { ProjectileType.Fireball, "Burn enemies or cook up some steak; gotta love\nthat fireball." },
            { ProjectileType.Bomb, "Explodes on impact. Don't try this at home." },
            { ProjectileType.Dart, "Quick and dangerous." },
            { ProjectileType.PoisonDart, "The poison tip makes it much more deadly than a\nplain dart." },
            { ProjectileType.FrostHex, "Slows aliens and deals light damage." },
            { ProjectileType.Laser, "Pew! Pew! Fire a powerful, deadly laser!" },
            { ProjectileType.Hex, "A basic spell to get you started with magic." },
            { ProjectileType.LightningBolt, "A more advanced spell. Shockingly effective!" },
            { ProjectileType.Hammer, "A metallic tool. Seems useless, but you DO NOT\nwant your face smashed by a hammer..." },
            { ProjectileType.Meteor, "This is a fiery rock. No one knows where they\ncome from; luckily, you can make your own!" },
            { ProjectileType.Rocket, "The most basic type of rocket." },
            { ProjectileType.FireRocket, "Explodes and sets aliens on fire." },
            { ProjectileType.PoisonRocket, "Explodes and poisons aliens." },
            { ProjectileType.FrozenRocket, "Explodes and freezes aliens." },
            { ProjectileType.PlasmaRocket, "Explodes and... plasma-fies aliens?" },
            { ProjectileType.OmegaRocket, "Explodes and freezes and burns and poisons and\nplasma-fies aliens." },
            { ProjectileType.Snowball, "Lump of ice that freezes aliens. They don't like\nsnowball fights that much." },
            { ProjectileType.Bone, "Don't know where this came from... and I don't\nthink you want to know..." },
            { ProjectileType.Shuriken, "A ninja's chosen weapon." },
            { ProjectileType.IceShard, "Ouch! An ice cold shard right to the face." },
            { ProjectileType.AbsorbHex, "A more powerful spell that heals you and\nhurts your enemies." },
        };

        public static readonly Dictionary<CannonType, string> CannonDescs = new Dictionary<CannonType, string>()
        {
            { CannonType.Normal, "Just your basic cannon." },
            { CannonType.Bronze, "Reinforced with bronze for more damage." },
            { CannonType.Silver, "Stronger, silver cannon. Increased speed,\ndamage, and accuracy." },
            { CannonType.Gold, "Who's idea was this!? This is an expensive,\nthough powerful, cannon." },
            { CannonType.Elite, "It's quick, it's accurate, it's powerful!" },
            { CannonType.Inferno, "Burn everything with this cannon! Just don't set\nthe lawn on fire..." },
            { CannonType.Frozen, "Adds a bit of ice to each projectile to make\nthem freeze aliens!" },
        };

        public static readonly Dictionary<CannonStats, string> StatDescs = new Dictionary<CannonStats, string>()
        {
            { CannonStats.Accuracy, "Adds a faint bar for easier aiming" },
            { CannonStats.Damage, "Adds an amount of damage to a projectile" },
            { CannonStats.Health, "Your maximum health" },
            { CannonStats.MoveSpeed, "How fast the cannon can move" },
            { CannonStats.RapidFire, "Allows for another projectile to fire immediately if holding down\nthe fire key" },
            { CannonStats.ReloadSpeed, "The speed at which a new projectile is ready to fire" },
        };

        #endregion

        #region Encryption Objects

        static List<char> lowercaseLetters = "abcdefghijklmnopqrstuvwxyz".ToCharArray().ToList();
        static List<char> capitalLetters = "ABCDEFGHIJKLMNOPQRSTUVWXYZ".ToCharArray().ToList();
        static List<char> symbols = "1234567890`~!@#$%^&*()-_=+[]{}\\|;:'\",<.>/? ".ToCharArray().ToList();

        static string cipherStartCode = "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ1234567890`~!@#$%^&*()-_=+[]{}\\|;:'\",<.>/? ";

        #endregion

        #endregion

        #region Public Methods

        public static int WorthOf(AlienType a)
        {
            int worth = 0;
            switch (a.Type)
            {
                case Aliens.Normal:
                    worth += NORMAL_ALIEN_HEALTH;
                    break;
                case Aliens.Defense:
                    worth += DEFENSE_ALIEN_HEALTH;
                    break;
                case Aliens.LDefense:
                    worth += LDEFENSE_ALIEN_HEALTH;
                    break;
                case Aliens.HDefense:
                    worth += HDEFENSE_ALIEN_HEALTH;
                    break;
                case Aliens.FireDefense:
                    worth += FIRE_DEFENSE_ALIEN_HEALTH;
                    break;
                case Aliens.LFireDefense:
                    worth += LFIRE_DEFENSE_ALIEN_HEALTH;
                    break;
                case Aliens.HFireDefense:
                    worth += HFIRE_DEFENSE_ALIEN_HEALTH;
                    break;
                case Aliens.PoisonResistant:
                    worth += POISON_RESIST_ALIEN_HEALTH;
                    break;
                case Aliens.LPoisonResistant:
                    worth += LPOISON_RESIST_ALIEN_HEALTH;
                    break;
                case Aliens.HPoisonResistant:
                    worth += HPOISON_RESIST_ALIEN_HEALTH;
                    break;
                case Aliens.PlasmaResistant:
                    worth += PLASMA_RESIST_ALIEN_HEALTH;
                    break;
                case Aliens.LPlasmaResistant:
                    worth += LPLASMA_RESIST_ALIEN_HEALTH;
                    break;
                case Aliens.HPlasmaResistant:
                    worth += HPLASMA_RESIST_ALIEN_HEALTH;
                    break;
                case Aliens.FreezeProof:
                    worth += FREEZE_PROOF_ALIEN_HEALTH;
                    break;
                case Aliens.Chaos:
                    worth += CHAOS_ALIEN_HEALTH;
                    break;
                case Aliens.Omega:
                    worth += OMEGA_ALIEN_HEALTH;
                    break;
                case Aliens.Boss:
                    worth += BOSS_ALIEN_HEALTH;
                    break;
            }

            if (a.HasShield)
            {
                worth += Alien.INIT_SHIELD_HEALTH;
            }

            return worth;
        }

        public static int ReloadSpeedEquation(int statSpd)
        {
            return Math.Max(500 - (50 * statSpd), 0);
        }

        public static int GetCostOfStat(CannonStats stat, int currentValue)
        {
            switch (stat)
            {
                case CannonStats.Health:
                    return INIT_HEALTH_COST + (currentValue - STARTING_MAX_HEALTH) * HEALTH_COST;
                case CannonStats.Damage:
                    return INIT_DMG_COST + currentValue * DMG_COST;
                case CannonStats.Accuracy:
                    return INIT_ACCURACY_COST + currentValue * ACCURACY_COST;
                case CannonStats.ReloadSpeed:
                    return INIT_RELOAD_SPD_COST + currentValue * RELOAD_SPD_COST;
                case CannonStats.MoveSpeed:
                    return INIT_MOVE_SPD_COST + (currentValue - STARTING_CANNON_RELOAD_SPD) * MOVE_SPD_COST;
                case CannonStats.RapidFire:
                    return RAPID_FIRE_COST;
                case CannonStats.Defense:
                    return INIT_DEFENSE_COST + currentValue * DEFENSE_COST;
                case CannonStats.Power:
                    return INIT_POWER_COST + currentValue * POWER_COST;
                default:
                    return 0;
            }
        }

        public static Color GetColorForAmount(int value, int maxValue)
        {
            Color color = new Color(0, 0, 0);

            // Calculate the percentage for the amount first
            int percentage = (int)((value / (maxValue * 1.0f)) * 100.0f);

            if (percentage >= 50)
            {
                color.G = byte.MaxValue;
                color.R = (byte)Math.Min((byte.MaxValue - (value * byte.MaxValue) / maxValue) * 2, byte.MaxValue);
            }
            else
            {
                color.R = byte.MaxValue;
                color.G = (byte)Math.Min(((value * byte.MaxValue) / maxValue) * 2, byte.MaxValue);
            }

            return color;
        }

        public static Projectile CreateProj(ProjectileType type)
        {
            switch (type)
            {
                case ProjectileType.Rock:
                    return new Rock(0, 100, Utilities.NormProjWidth, Utilities.NormProjHeight);
                case ProjectileType.Cannonball:
                    return new Cannonball(0, 100, Utilities.NormProjWidth, Utilities.NormProjHeight);
                case ProjectileType.Fireball:
                    return new Fireball(0, 100, Utilities.NormProjWidth, Utilities.NormProjHeight);
                case ProjectileType.Bomb:
                    return new Bomb(0, 100, Utilities.NormProjWidth, Utilities.NormProjHeight);
                case ProjectileType.Dart:
                    return new Dart(0, 100, Utilities.NormProjWidth, Utilities.NormProjHeight);
                case ProjectileType.PoisonDart:
                    return new PoisonDart(0, 100, Utilities.NormProjWidth, Utilities.NormProjHeight);
                case ProjectileType.Hex:
                    return new Hex(0, 100, Utilities.NormProjWidth, Utilities.NormProjHeight);
                case ProjectileType.Laser:
                    return new Laser(0, 100, Utilities.LaserWidth, Utilities.LaserHeight);
                case ProjectileType.LightningBolt:
                    return new LightningBolt(0, 100, Utilities.LightningWidth, Utilities.LightningHeight);
                case ProjectileType.FrostHex:
                    return new FrozenBlast(0, 100, Utilities.NormProjWidth, Utilities.NormProjHeight);
                case ProjectileType.Meteor:
                    return new Meteor(0, 100, Utilities.NormProjWidth, Utilities.NormProjHeight);
                case ProjectileType.Hammer:
                    return new Hammer(0, 100, Utilities.NormProjWidth, Utilities.NormProjHeight);
                case ProjectileType.Snowball:
                    return new Snowball(0, 100, Utilities.NormProjWidth, Utilities.NormProjHeight);
                case ProjectileType.Rocket:
                    return new Rocket(0, 100, Utilities.NormProjWidth, Utilities.NormProjHeight);
                case ProjectileType.FireRocket:
                    return new FireRocket(0, 100, Utilities.NormProjWidth, Utilities.NormProjHeight);
                case ProjectileType.PoisonRocket:
                    return new PoisonRocket(0, 100, Utilities.NormProjWidth, Utilities.NormProjHeight);
                case ProjectileType.FrozenRocket:
                    return new FrozenRocket(0, 100, Utilities.NormProjWidth, Utilities.NormProjHeight);
                case ProjectileType.PlasmaRocket:
                    return new PlasmaRocket(0, 100, Utilities.NormProjWidth, Utilities.NormProjHeight);
                case ProjectileType.OmegaRocket:
                    return new OmegaRocket(0, 100, Utilities.NormProjWidth, Utilities.NormProjHeight);
                case ProjectileType.Bone:
                    return new Bone(0, 100, Utilities.LightningWidth, Utilities.NormProjHeight);
                case ProjectileType.Shuriken:
                    return new Shuriken(0, 100, Utilities.NormProjWidth, Utilities.NormProjHeight);
                case ProjectileType.IceShard:
                    return new IceShard(0, 100, Utilities.NormProjWidth, Utilities.NormProjHeight);
                case ProjectileType.AbsorbHex:
                    return new AbsorbHex(0, 100, Utilities.NormProjWidth, Utilities.NormProjHeight);
                default:
                    return null;
            }
        }
        public static Projectile CreateRandomProj()
        {
            List<ProjectileType> types = Enum.GetValues(typeof(ProjectileType)).Cast<ProjectileType>().ToList();
            return CreateProj(GetRandomProjType(types));
        }

        public static Alien CreateAlien(Aliens type, bool hasShield, int windowWidth, int windowHeight, int alienWidth,
            int alienHeight, OnItemCollect itemCollected)
        {
            Alien returnVal = null;

            switch (type)
            {
                case Aliens.Normal:
                    returnVal = new NormalAlien(Utilities.AlienImg, Utilities.AlienEyeImg, Utilities.Rand.Next(windowWidth - alienWidth),
                        PlayerInterface.TOP_HEIGHT - alienHeight, alienWidth, alienHeight, itemCollected);
                    break;
                case Aliens.Defense:
                    returnVal = new DefenseAlien(Utilities.AlienImg, Utilities.AlienEyeImg, Utilities.Rand.Next(windowWidth - alienWidth),
                        PlayerInterface.TOP_HEIGHT - alienHeight, alienWidth, alienHeight, itemCollected);
                    break;
                case Aliens.LDefense:
                    returnVal = new LDefenseAlien(Utilities.AlienImg, Utilities.AlienEyeImg, Utilities.Rand.Next(windowWidth - alienWidth),
                        PlayerInterface.TOP_HEIGHT - alienHeight, alienWidth, alienHeight, itemCollected);
                    break;
                case Aliens.HDefense:
                    returnVal = new HDefenseAlien(Utilities.AlienImg, Utilities.AlienEyeImg, Utilities.Rand.Next(windowWidth - alienWidth),
                        PlayerInterface.TOP_HEIGHT - alienHeight, alienWidth, alienHeight, itemCollected);
                    break;
                case Aliens.FireDefense:
                    returnVal = new FireDefenseAlien(Utilities.AlienImg, Utilities.AlienEyeImg, Utilities.Rand.Next(windowWidth - alienWidth),
                        PlayerInterface.TOP_HEIGHT - alienHeight, alienWidth, alienHeight, itemCollected);
                    break;
                case Aliens.LFireDefense:
                    returnVal = new LFireDefenseAlien(Utilities.AlienImg, Utilities.AlienEyeImg, Utilities.Rand.Next(windowWidth - alienWidth),
                        PlayerInterface.TOP_HEIGHT - alienHeight, alienWidth, alienHeight, itemCollected);
                    break;
                case Aliens.HFireDefense:
                    returnVal = new HFireDefenseAlien(Utilities.AlienImg, Utilities.AlienEyeImg, Utilities.Rand.Next(windowWidth - alienWidth),
                        PlayerInterface.TOP_HEIGHT - alienHeight, alienWidth, alienHeight, itemCollected);
                    break;
                case Aliens.Chaos:
                    returnVal = new ChaosAlien(Utilities.AlienImg, Utilities.AlienEyeImg, Utilities.Rand.Next(windowWidth - alienWidth),
                        PlayerInterface.TOP_HEIGHT - alienHeight, alienWidth, alienHeight, itemCollected);
                    break;
                case Aliens.PoisonResistant:
                    returnVal = new PoisonResistantAlien(Utilities.AlienImg, Utilities.AlienEyeImg, Utilities.Rand.Next(windowWidth - alienWidth),
                        PlayerInterface.TOP_HEIGHT - alienHeight, alienWidth, alienHeight, itemCollected);
                    break;
                case Aliens.LPoisonResistant:
                    returnVal = new LPoisonResistantAlien(Utilities.AlienImg, Utilities.AlienEyeImg, Utilities.Rand.Next(windowWidth - alienWidth),
                        PlayerInterface.TOP_HEIGHT - alienHeight, alienWidth, alienHeight, itemCollected);
                    break;
                case Aliens.HPoisonResistant:
                    returnVal = new HPoisonResistantAlien(Utilities.AlienImg, Utilities.AlienEyeImg, Utilities.Rand.Next(windowWidth - alienWidth),
                        PlayerInterface.TOP_HEIGHT - alienHeight, alienWidth, alienHeight, itemCollected);
                    break;
                case Aliens.FreezeProof:
                    returnVal = new FreezeProofAlien(Utilities.AlienImg, Utilities.AlienEyeImg, Utilities.Rand.Next(windowWidth - alienWidth),
                        PlayerInterface.TOP_HEIGHT - alienHeight, alienWidth, alienHeight, itemCollected);
                    break;
                case Aliens.PlasmaResistant:
                    returnVal = new PlasmaResistantAlien(Utilities.AlienImg, Utilities.AlienEyeImg, Utilities.Rand.Next(windowWidth - alienWidth),
                        PlayerInterface.TOP_HEIGHT - alienHeight, alienWidth, alienHeight, itemCollected);
                    break;
                case Aliens.LPlasmaResistant:
                    returnVal = new LPlasmaResistantAlien(Utilities.AlienImg, Utilities.AlienEyeImg, Utilities.Rand.Next(windowWidth - alienWidth),
                        PlayerInterface.TOP_HEIGHT - alienHeight, alienWidth, alienHeight, itemCollected);
                    break;
                case Aliens.HPlasmaResistant:
                    returnVal = new HPlasmaResistantAlien(Utilities.AlienImg, Utilities.AlienEyeImg, Utilities.Rand.Next(windowWidth - alienWidth),
                        PlayerInterface.TOP_HEIGHT - alienHeight, alienWidth, alienHeight, itemCollected);
                    break;
                case Aliens.Omega:
                    returnVal = new OmegaAlien(Utilities.AlienImg, Utilities.AlienEyeImg, Utilities.Rand.Next(windowWidth - alienWidth),
                        PlayerInterface.TOP_HEIGHT - alienHeight, alienWidth, alienHeight, itemCollected);
                    break;
                case Aliens.Ninja:
                    returnVal = new NinjaAlien(Utilities.AlienImg, Utilities.AlienEyeImg, Utilities.Rand.Next(windowWidth - alienWidth),
                        PlayerInterface.TOP_HEIGHT - alienHeight, alienWidth, alienHeight, itemCollected);
                    break;
                case Aliens.Boss:
                    returnVal = new BossAlien(Utilities.AlienImg, Utilities.AlienEyeImg, windowWidth / 2 - (alienWidth / 2),
                        PlayerInterface.TOP_HEIGHT - alienHeight, alienWidth, alienHeight, itemCollected);
                    break;
            }

            returnVal.HasShield = hasShield;
            return returnVal;
        }

        public static ProjectileType GetRandomProjType(List<ProjectileType> projectiles)
        {
            HashSet<ProjectileType> types = new HashSet<ProjectileType>();
            foreach (ProjectileType proj in projectiles)
            {
                types.Add(proj);
            }
            if (types.Count == 0)
            {
                return ProjectileType.None;
            }
            else if (types.Count == 1)
            {
                return types.ToList()[0];
            }
            else
            {
                return types.ToList()[Utilities.Rand.Next(types.Count)];
            }
        }

        //public static int GetXpForLvl(int level)
        //{
        //    return (30 * (level - 1)) + STARTING_XP_MAX;
        //}

        public static bool CanSee(User user, ProjectileType type)
        {
            if (type != ProjectileType.None)
            {
                return (user.CurrentMission >= ProjVisibilityLvls[type]);
            }
            return false;
        }

        public static List<ProjectileType> ProjListToTypes(List<Projectile> projectiles)
        {
            List<ProjectileType> returnVal = new List<ProjectileType>();
            foreach (Projectile p in projectiles)
            {
                returnVal.Add(p.Type);
            }
            return returnVal;
        }
        public static List<ProjectileType> ProjListToTypesWithoutFlying(List<Projectile> projectiles)
        {
            List<ProjectileType> returnVal = new List<ProjectileType>();
            foreach (Projectile p in projectiles)
            {
                if (!p.Flying)
                {
                    returnVal.Add(p.Type);
                }
            }
            return returnVal;
        }

        public static int CountOf(List<ProjectileType> types, ProjectileType target)
        {
            if (target == ProjectileType.None)
            {
                return -1;
            }

            int count = 0;
            foreach (ProjectileType t in types)
            {
                if (t == target)
                {
                    count++;
                }
            }
            return count;
        }

        public static List<HotbarSlotInteractive> RectsToHotbarSlots(List<Rectangle> rects)
        {
            List<HotbarSlotInteractive> returnVal = new List<HotbarSlotInteractive>();

            for (int i = 0; i < rects.Count; i++)
            {
                //returnVal.Add(new HotbarSlotInteractive(graphics, rects.X, rects.Y, rects.Width, re)
            }

            return returnVal;
        }
        public static List<Vector2> AddToAll(this List<Vector2> list, int x, int y)
        {
            for (int i = 0; i < list.Count; i++)
            {
                list[i] = new Vector2(list[i].X + x, list[i].Y + y);
            }
            return list;
        }
        public static HotbarSlotInteractive FindMinDistance(this List<HotbarSlotInteractive> points, Vector2 mainPoint)
        {
            List<float> distances = new List<float>();
            for (int i = 0; i < points.Count; i++)
            {
                distances.Add(Vector2.Distance(new Vector2(points[i].X, points[i].Y), mainPoint));
            }

            return points[distances.IndexOf(distances.Min())];
        }

        public static void SaveTextureData(this Texture2D texture, string filename)
        {
            byte[] imageData = new byte[4 * texture.Width * texture.Height];
            texture.GetData(imageData);

            System.Drawing.Bitmap bitmap = new System.Drawing.Bitmap(texture.Width, texture.Height,
                System.Drawing.Imaging.PixelFormat.Format32bppArgb);
            System.Drawing.Imaging.BitmapData bmData =
                bitmap.LockBits(new System.Drawing.Rectangle(0, 0, texture.Width, texture.Height),
                System.Drawing.Imaging.ImageLockMode.ReadWrite, bitmap.PixelFormat);
            IntPtr pnative = bmData.Scan0;
            System.Runtime.InteropServices.Marshal.Copy(imageData, 0, pnative, 4 * texture.Width * texture.Height);
            bitmap.UnlockBits(bmData);
            bitmap.Save(filename, System.Drawing.Imaging.ImageFormat.Png);
        }

        public static string ToText(Aliens alien)
        {
            switch (alien)
            {
                case Aliens.Normal:
                    return "Normal Alien";
                case Aliens.LDefense:
                    return "Light Defense Alien";
                case Aliens.Defense:
                    return "Defense Alien";
                case Aliens.HDefense:
                    return "Heavy Defense Alien";
                case Aliens.LFireDefense:
                    return "Light Fire Resistant Alien";
                case Aliens.FireDefense:
                    return "Fire Resistant Alien";
                case Aliens.HFireDefense:
                    return "Heavy Fire Resistant Alien";
                case Aliens.LPoisonResistant:
                    return "Light Poison Resistant Alien";
                case Aliens.PoisonResistant:
                    return "Poison Resistant Alien";
                case Aliens.HPoisonResistant:
                    return "Heavy Poison Resistant Alien";
                case Aliens.LPlasmaResistant:
                    return "Light Plasma Resistant Alien";
                case Aliens.PlasmaResistant:
                    return "Plasma Resistant Alien";
                case Aliens.HPlasmaResistant:
                    return "Heavy Plasma Resistant Alien";
                case Aliens.FreezeProof:
                    return "Freeze Resistant Alien";
                case Aliens.Chaos:
                    return "Chaos Alien";
                case Aliens.Omega:
                    return "Omega Alien";
                case Aliens.Boss:
                    return "Malos";
            }
            return "";
        }

        public static int CostOf(ProjectileType p)
        {
            int cost = 0;
            CraftingRecipe recipe = CraftingRecipe.GetRecipeForProj(p);
            for (int i = 0; i < recipe.Materials.Count; i++)
            {
                cost += CostOf(recipe.Materials[i]) * recipe.MaterialAmounts[i];
            }

            return cost;
        }
        public static int CostOf(Material material)
        {
            switch (material)
            {
                case Material.ChaosEnergy:
                    return CHAOSENERGY_COST;
                case Material.EssenceOfFire:
                    return FIRE_COST;
                case Material.Gunpowder:
                    return GUNPOWDER_COST;
                case Material.Ice:
                    return ICE_COST;
                case Material.Metal:
                    return METAL_COST;
                case Material.Plasma:
                    return PLASMA_COST;
                case Material.PlantMatter:
                    return POISON_COST;
                case Material.Stone:
                    return STONE_COST;
            }
            return -1;
        }
        public static int SellValueOf(Material m)
        {
            float sellValue = CostOf(m) * SELL_DEPRECIATION_PCT;
            sellValue = (float)Math.Floor(sellValue);

            return (int)sellValue;
        }
        public static int SellValueOf(ProjectileType p)
        {
            float sellValue = CostOf(p) * SELL_DEPRECIATION_PCT;
            sellValue = (float)Math.Floor(sellValue);

            return (int)sellValue;
        }
        public static int SellValueOf(Badge b)
        {
            switch (b)
            {
                case Badge.Red:
                    return 150;
                case Badge.Orange:
                    return 6;
                case Badge.Yellow:
                    return 5;
                case Badge.Green:
                    return 4;
                case Badge.Blue:
                    return 7;
                case Badge.Purple:
                    return 8;
            }
            return 0;
        }

        public static float HealthOf(AlienType alien)
        {
            Alien a = CreateAlien(alien.Type, alien.HasShield, 0, 0, 0, 0, null);
            float health = a.Health;
            float shield = Alien.INIT_SHIELD_HEALTH;
            float def = a.Defense;
            health += health * def;
            return health + shield;
        }

        #endregion

        #region Extension Methods

        /// <summary>
        /// Returns an encrypted string
        /// </summary>
        /// <param name="method">The method to use to encrypt the string</param>
        /// <param name="code">Optional. The code to use for Caesar ciphers. Set to null if not using cipher.</param>
        /// <returns>An encrypted string</returns>
        public static string Encrypt(this string str, EncryptMethods method, string code)
        {
            string output = "";

            switch (method)
            {
                case EncryptMethods.ThreeNumCode:

                    List<char> chars = str.ToCharArray().ToList();

                    foreach (char c in chars)
                    {
                        if (lowercaseLetters.Contains(c))
                        {
                            output += "0";
                            if (lowercaseLetters.IndexOf(c).ToString().Length < 2)
                            {
                                output += "0";
                            }
                            output += lowercaseLetters.IndexOf(c);
                        }
                        else if (capitalLetters.Contains(c))
                        {
                            output += "1";
                            if (capitalLetters.IndexOf(c).ToString().Length < 2)
                            {
                                output += "0";
                            }
                            output += capitalLetters.IndexOf(c);
                        }
                        else if (symbols.Contains(c))
                        {
                            output += "2";
                            if (symbols.IndexOf(c).ToString().Length < 2)
                            {
                                output += "0";
                            }
                            output += symbols.IndexOf(c);
                        }
                        else
                        {
                            // The character is not recognized
                            throw new InvalidOperationException("The specified character to encrypt is not recognized.");
                        }
                    }

                    break;

                case EncryptMethods.CaesarCipher:

                    List<char> orig = cipherStartCode.ToCharArray().ToList();
                    List<char> codeList = code.ToCharArray().ToList();
                    List<char> characters = str.ToCharArray().ToList();

                    foreach (char c in characters)
                    {
                        if (orig.Contains(c) && codeList.Contains(c))
                        {
                            output += codeList[orig.IndexOf(c)];
                        }
                        else
                        {
                            // The character is not recognized
                            throw new InvalidOperationException("The specified character to encrypt is not recognized.");
                        }
                    }

                    break;
            }

            return output;
        }
        public static string Decrypt(this string str, EncryptMethods method, string code)
        {
            string output = "";

            switch (method)
            {
                case EncryptMethods.ThreeNumCode:

                    int count = 0;
                    while (count < str.Length / 3)
                    {
                        string numCode = str.Substring(count * 3, (count * 3) + 3);
                        int index = 0;

                        if (numCode.StartsWith("0"))
                        {
                            if (int.TryParse(numCode, out index))
                            {
                                output += lowercaseLetters[index];
                            }
                            else
                            {
                                throw new InvalidOperationException("The number code cannot be converted to an integer.");
                            }
                        }
                        else if (numCode.StartsWith("1"))
                        {
                            if (int.TryParse(numCode, out index))
                            {
                                output += capitalLetters[index];
                            }
                            else
                            {
                                throw new InvalidOperationException("The number code cannot be converted to an integer.");
                            }
                        }
                        else if (numCode.StartsWith("2"))
                        {
                            if (int.TryParse(numCode, out index))
                            {
                                output += symbols[index];
                            }
                            else
                            {
                                throw new InvalidOperationException("The number code cannot be converted to an integer.");
                            }
                        }
                        else
                        {
                            throw new InvalidOperationException("The specified code to decrypt is not recognized.");
                        }

                        count++;
                    }

                    break;

                case EncryptMethods.CaesarCipher:

                    List<char> orig = cipherStartCode.ToCharArray().ToList();
                    List<char> codeList = code.ToCharArray().ToList();
                    List<char> characters = str.ToCharArray().ToList();

                    foreach (char c in characters)
                    {
                        if (orig.Contains(c) && codeList.Contains(c))
                        {
                            output += orig[codeList.IndexOf(c)];
                        }
                        else
                        {
                            // The character is not recognized
                            throw new InvalidOperationException("The specified character to encrypt is not recognized.");
                        }
                    }

                    break;
            }

            return "";
        }

        // Returns a string with a space before every capital letter in the specified string
        public static string AddSpaces(this string str)
        {
            List<char> characters = str.ToCharArray().ToList();

            for (int i = 0; i <= characters.Count - 1; i++)
            {
                string c = characters[i].ToString();
                if (i > 0)
                {
                    char n = characters[i - 1];
                }
                if (c == c.ToUpper() && i > 0 && characters[i - 1] != ' ')
                {
                    // The character is uppercase, so we need to insert a space before it
                    // However, we make sure i is greater than 0,
                    // since, often, the first letter in a string is capitalized (and we can't insert a space
                    // before the first letter)
                    // We also need to make there isn't already a space before the letter
                    characters.Insert(i, ' ');
                }
            }
            characters.ToString();
            str = new string(characters.ToArray());

            return str;
        }

        // List extensions
        public static int CountOf<T>(this List<T> list, T item)
        {
            int count = 0;
            for (int i = 0; i < list.Count; i++)
            {
                if (list[i].Equals(item))
                {
                    count++;
                }
            }
            return count;
        }
        public static List<T> Unique<T>(this List<T> list)
        {
            return list.Select(x => x).Distinct().ToList();
        }
        public static void Shuffle<T>(this List<T> list)
        {
            int n = list.Count;
            while (n > 1)
            {
                n--;
                int k = Utilities.Rand.Next(n + 1);
                T value = list[k];
                list[k] = list[n];
                list[n] = value;
            }
        }
        public static T Random<T>(this List<T> list)
        {
            // Random.Next 2nd parameter is exclusive
            int index = Utilities.Rand.Next(0, list.Count);
            return list[index];
        }

        public static List<Projectile> WithoutFlying(this List<Projectile> projectiles)
        {
            List<Projectile> returnVal = new List<Projectile>();
            for (int i = 0; i < projectiles.Count; i++)
            {
                if (!projectiles[i].Flying)
                {
                    returnVal.Add(projectiles[i]);
                }
            }
            return returnVal;
        }

        public static string FormatDistanceToNow(this DateTime time1)
        {
            TimeSpan distance = time1 - DateTime.Now;
            return string.Format("{0:00}:{1:00}", distance.Minutes, distance.Seconds);
        }

        public static bool ContainsType(this List<CannonSettings> list, CannonType type)
        {
            for (int i = 0; i < list.Count; i++)
            {
                if (list[i].CannonType == type)
                {
                    return true;
                }
            }
            return false;
        }

        #endregion
    }

    public static class Utilities
    {
        #region Properties

        public const int MENU_Y_OFFSET = 150;

        public static Random Rand;

        public static Texture2D CoinImage;
        public static Texture2D CoinIcon;

        public static Texture2D RectImage;

        public static Texture2D RockImg;
        public static Texture2D CannonballImg;
        public static Texture2D FireballImg;
        public static Texture2D BombImg;
        public static Texture2D DartImg;
        public static Texture2D PoisonDartImg;
        public static Texture2D LaserImg;
        public static Texture2D ChaosImg;
        public static Texture2D LightningBoltImg;
        public static Texture2D FrozenBlastImg;
        public static Texture2D MeteorImg;
        public static Texture2D HammerImg;
        public static Texture2D SnowballImg;
        public static Texture2D RocketImg;
        public static Texture2D FireRktImg;
        public static Texture2D FrozenRktImg;
        public static Texture2D PoisonRktImg;
        public static Texture2D PlasmaRktImg;
        public static Texture2D OmegaRktImg;
        public static Texture2D BoneImg;
        public static Texture2D ShurikenImg;
        public static Texture2D IceShardImg;
        public static Texture2D AbsorbHexImg;

        public static Texture2D ExplosionSheet;
        public static Texture2D BatteryImg;

        public static Texture2D FireballIcon;
        public static Texture2D FrozenBlastIcon;
        public static Texture2D LightningIcon;
        public static Texture2D LaserIcon;

        public static int NormProjWidth;
        public static int NormProjHeight;
        public static int LightningWidth;
        public static int LightningHeight;
        public static int LaserWidth;
        public static int LaserHeight;
        public static int AlienWidth;
        public static int AlienHeight;

        public static Texture2D GiftImg;

        public static Texture2D StoneImg;
        public static Texture2D MetalImg;
        public static Texture2D PoisonImg;
        public static Texture2D GunpowderImg;
        public static Texture2D IceImg;
        public static Texture2D FireImg;
        public static Texture2D PlasmaImg;
        public static Texture2D ChaosEnergyImg;
        public static Texture2D BoneMaterialImg;

        public static Texture2D AlienImg;
        public static Texture2D AlienEyeImg;
        public static Texture2D AlienShieldImg;

        public static Texture2D MechImg;
        public static Texture2D CageImg;
        public static Texture2D NoMalosImg;
        public static Texture2D NoAlienImg;
        public static Texture2D NoLaserImg;

        public static Texture2D NormCannonBottomImg;
        public static Texture2D NormCannonTubeImg;
        public static Texture2D BronzeCannonBottomImg;
        public static Texture2D BronzeCannonTubeImg;
        public static Texture2D SilverCannonBottomImg;
        public static Texture2D SilverCannonTubeImg;
        public static Texture2D GoldCannonBottomImg;
        public static Texture2D GoldCannonTubeImg;
        public static Texture2D MasterCannonBottomImg;
        public static Texture2D MasterCannonTubeImg;
        public static Texture2D InfernoCannonBottomImg;
        public static Texture2D InfernoCannonTubeImg;
        public static Texture2D FrozenCannonBottomImg;
        public static Texture2D FrozenCannonTubeImg;

        public static Texture2D RedTrophyImg;
        public static Texture2D OrangeTrophyImg;
        public static Texture2D YellowTrophyImg;
        public static Texture2D GreenTrophyImg;
        public static Texture2D BlueTrophyImg;
        public static Texture2D PurpleTrophyImg;

        public static Texture2D HotbarIcon;
        public static Texture2D BadgeIcon;

        public static ContentManager Content;
        public static GraphicsDevice GraphicsDevice;
        
        public static OnExplosion OnExplosion;

        public const int LIGHTNING_FRAMES_PER_ROW = 5;
        public const int LIGHTNING_ROWS = 1;
        public const int LIGHTNING_FRAME_WIDTH = 100;
        public const int LIGHTNING_FRAME_HEIGHT = 200;
        public const int LIGHTNING_TIME = 20;
        
        #endregion

        #region Public Methods

        public static Texture2D GetIconOf(ProjectileType type)
        {
            switch (type)
            {
                case ProjectileType.Rock:
                    return RockImg;
                case ProjectileType.Cannonball:
                    return CannonballImg;
                case ProjectileType.Fireball:
                    return FireballIcon;
                case ProjectileType.Bomb:
                    return BombImg;
                case ProjectileType.Dart:
                    return DartImg;
                case ProjectileType.PoisonDart:
                    return PoisonDartImg;
                case ProjectileType.Hex:
                    return ChaosImg;
                case ProjectileType.Laser:
                    return LaserIcon;
                case ProjectileType.LightningBolt:
                    return LightningIcon;
                case ProjectileType.FrostHex:
                    return FrozenBlastIcon;
                case ProjectileType.Meteor:
                    return MeteorImg;
                case ProjectileType.Hammer:
                    return HammerImg;
                case ProjectileType.Snowball:
                    return SnowballImg;
                case ProjectileType.Rocket:
                    return RocketImg;
                case ProjectileType.FireRocket:
                    return FireRktImg;
                case ProjectileType.FrozenRocket:
                    return FrozenRktImg;
                case ProjectileType.PlasmaRocket:
                    return PlasmaRktImg;
                case ProjectileType.PoisonRocket:
                    return PoisonRktImg;
                case ProjectileType.OmegaRocket:
                    return OmegaRktImg;
                case ProjectileType.Bone:
                    return BoneImg;
                case ProjectileType.Shuriken:
                    return ShurikenImg;
                case ProjectileType.IceShard:
                    return IceShardImg;
                case ProjectileType.AbsorbHex:
                    return AbsorbHexImg;
            }
            return null;
        }
        public static Texture2D GetImgOfMaterial(Material material)
        {
            switch (material)
            {
                case Material.Stone:
                    return StoneImg;
                case Material.Metal:
                    return MetalImg;
                case Material.PlantMatter:
                    return PoisonImg;
                case Material.Gunpowder:
                    return GunpowderImg;
                case Material.Ice:
                    return IceImg;
                case Material.EssenceOfFire:
                    return FireImg;
                case Material.Plasma:
                    return PlasmaImg;
                case Material.ChaosEnergy:
                    return ChaosEnergyImg;
                default:
                    return null;
            }
        }
        public static Texture2D GetImgOfGoal(MissionGoal goal)
        {
            switch (goal)
            {
                case MissionGoal.KillAll:
                    return NoAlienImg;
                case MissionGoal.DestroyMechanics:
                    return MechImg;
                case MissionGoal.SavePeople:
                    return CageImg;
                case MissionGoal.DestroyLaser:
                    return NoLaserImg;
                case MissionGoal.KillMalos:
                    return NoMalosImg;
            }
            return null;
        }
        public static Texture2D GetImgOfBadge(Badge badge)
        {
            switch (badge)
            {
                case Badge.Red:
                    return RedTrophyImg;
                case Badge.Orange:
                    return OrangeTrophyImg;
                case Badge.Yellow:
                    return YellowTrophyImg;
                case Badge.Green:
                    return GreenTrophyImg;
                case Badge.Blue:
                    return BlueTrophyImg;
                case Badge.Purple:
                    return PurpleTrophyImg;
            }
            return null;
        }
        public static Texture2D GetBottomOfCannon(CannonType type)
        {
            switch (type)
            {
                case CannonType.Normal:
                    return NormCannonBottomImg;
                case CannonType.Bronze:
                    return BronzeCannonBottomImg;
                case CannonType.Silver:
                    return SilverCannonBottomImg;
                case CannonType.Gold:
                    return GoldCannonBottomImg;
                case CannonType.Elite:
                    return MasterCannonBottomImg;
                case CannonType.Inferno:
                    return InfernoCannonBottomImg;
                case CannonType.Frozen:
                    return FrozenCannonBottomImg;
            }
            return null;
        }
        public static Texture2D GetTubeOfCannon(CannonType type)
        {
            switch (type)
            {
                case CannonType.Normal:
                    return NormCannonTubeImg;
                case CannonType.Bronze:
                    return BronzeCannonTubeImg;
                case CannonType.Silver:
                    return SilverCannonTubeImg;
                case CannonType.Gold:
                    return GoldCannonTubeImg;
                case CannonType.Elite:
                    return MasterCannonTubeImg;
                case CannonType.Inferno:
                    return InfernoCannonTubeImg;
                case CannonType.Frozen:
                    return FrozenCannonTubeImg;
            }

            return null;
        }

        public static float Average(params int[] numbers)
        {
            int added = 0;
            for (int i = 0; i < numbers.Count(); i++)
            {
                added += numbers[i];
            }
            return added / (numbers.Count() * 1.0f);
        }

        public static string GetDescOf(Material material)
        {
            return GameInfo.MaterialDescs.ContainsKey(material) ? GameInfo.MaterialDescs[material] : "";
        }
        public static string GetDescOf(ProjectileType proj)
        {
            return GameInfo.ProjectileDescs.ContainsKey(proj) ? GameInfo.ProjectileDescs[proj] : "";
        }
        public static string GetDescOf(CannonType cannon)
        {
            return GameInfo.CannonDescs.ContainsKey(cannon) ? GameInfo.CannonDescs[cannon] : "";
        }
        public static string GetDescOf(CannonStats stat)
        {
            return GameInfo.StatDescs.ContainsKey(stat) ? GameInfo.StatDescs[stat] : "";
        }

        public static int DamageOf(ProjectileType proj)
        {
            switch (proj)
            {
                case ProjectileType.Bomb:
                    return GameInfo.BOMB_DMG;
                case ProjectileType.Cannonball:
                    return GameInfo.CANNONBALL_DMG;
                case ProjectileType.Dart:
                case ProjectileType.PoisonDart:
                    return GameInfo.DART_DMG;
                case ProjectileType.Fireball:
                    return GameInfo.FIREBALL_DMG;
                case ProjectileType.FireRocket:
                case ProjectileType.FrozenRocket:
                case ProjectileType.PlasmaRocket:
                case ProjectileType.PoisonRocket:
                case ProjectileType.OmegaRocket:
                case ProjectileType.Rocket:
                    return GameInfo.ROCKET_DMG;
                case ProjectileType.FrostHex:
                    return GameInfo.FROZENBLAST_DMG;
                case ProjectileType.Hammer:
                    return GameInfo.HAMMER_DMG;
                case ProjectileType.Hex:
                    return GameInfo.CHAOS_DMG;
                case ProjectileType.Laser:
                    return GameInfo.LASER_DMG;
                case ProjectileType.LightningBolt:
                    return GameInfo.LIGHTNING_DMG;
                case ProjectileType.Meteor:
                    return GameInfo.METEOR_DMG;
                case ProjectileType.Rock:
                    return GameInfo.ROCKET_DMG;
                case ProjectileType.Snowball:
                    return GameInfo.SNOWBALL_DMG;
                case ProjectileType.Bone:
                    return GameInfo.BONE_DMG;
                case ProjectileType.Shuriken:
                    return GameInfo.SHURIKEN_DMG;
                case ProjectileType.IceShard:
                    return GameInfo.ICESHARD_DMG;
                case ProjectileType.AbsorbHex:
                    return GameInfo.ABSORBHEX_DMG;
            }
            return -1;
        }
        public static int SpeedOf(ProjectileType proj)
        {
            switch (proj)
            {
                case ProjectileType.Bomb:
                    return GameInfo.BOMB_SPD;
                case ProjectileType.Cannonball:
                    return GameInfo.CANNONBALL_SPD;
                case ProjectileType.Dart:
                case ProjectileType.PoisonDart:
                    return GameInfo.DART_SPD;
                case ProjectileType.Fireball:
                    return GameInfo.FIREBALL_SPD;
                case ProjectileType.FireRocket:
                case ProjectileType.FrozenRocket:
                case ProjectileType.PlasmaRocket:
                case ProjectileType.PoisonRocket:
                case ProjectileType.OmegaRocket:
                case ProjectileType.Rocket:
                    return GameInfo.ROCKET_SPD;
                case ProjectileType.FrostHex:
                    return GameInfo.FROZENBLAST_SPD;
                case ProjectileType.Hammer:
                    return GameInfo.HAMMER_SPD;
                case ProjectileType.Hex:
                    return GameInfo.CHAOS_SPD;
                case ProjectileType.Laser:
                    return GameInfo.LASER_SPD;
                case ProjectileType.LightningBolt:
                    return GameInfo.LIGHTNING_SPD;
                case ProjectileType.Meteor:
                    return GameInfo.METEOR_SPD;
                case ProjectileType.Rock:
                    return GameInfo.ROCK_SPD;
                case ProjectileType.Snowball:
                    return GameInfo.SNOWBALL_SPD;
                case ProjectileType.Bone:
                    return GameInfo.BONE_SPD;
                case ProjectileType.Shuriken:
                    return GameInfo.SHURIKEN_SPD;
                case ProjectileType.IceShard:
                    return GameInfo.ICESHARD_SPD;
                case ProjectileType.AbsorbHex:
                    return GameInfo.ABSORBHEX_SPD;
            }
            return -1;
        }
        public static string EffectsOf(ProjectileType proj)
        {
            switch (proj)
            {
                case ProjectileType.PoisonDart:
                case ProjectileType.PoisonRocket:
                    return "Poison";
                case ProjectileType.Hex:
                    return "Cursed";
                case ProjectileType.Laser:
                case ProjectileType.PlasmaRocket:
                    return "Plasma";
                case ProjectileType.LightningBolt:
                case ProjectileType.Meteor:
                case ProjectileType.Fireball:
                case ProjectileType.FireRocket:
                    return "Fire";
                case ProjectileType.Snowball:
                case ProjectileType.FrostHex:
                case ProjectileType.FrozenRocket:
                case ProjectileType.IceShard:
                    return "Frozen";
                case ProjectileType.OmegaRocket:
                    return "Poison, Fire, Frozen, Plasma";
                case ProjectileType.AbsorbHex:
                    return "Heals " + GameInfo.ABSORBHEX_HEAL + " Health";
            }
            return "None";
        }
        public static int WorthOf(ProjectileType proj)
        {
            switch (proj)
            {
                case ProjectileType.Bomb:
                    return GameInfo.BOMB_WORTH;
                case ProjectileType.Cannonball:
                    return GameInfo.CANNONBALL_WORTH;
                case ProjectileType.Dart:
                    return GameInfo.DART_WORTH;
                case ProjectileType.PoisonDart:
                    return GameInfo.POISONDART_WORTH;
                case ProjectileType.Fireball:
                    return GameInfo.FIREBALL_WORTH;
                case ProjectileType.FireRocket:
                    return GameInfo.FIRE_ROCKET_WORTH;
                case ProjectileType.FrozenRocket:
                    return GameInfo.FROZEN_ROCKET_WORTH;
                case ProjectileType.PlasmaRocket:
                    return GameInfo.PLASMA_ROCKET_WORTH;
                case ProjectileType.PoisonRocket:
                    return GameInfo.POISON_ROCKET_WORTH;
                case ProjectileType.OmegaRocket:
                    return GameInfo.OMEGA_ROCKET_WORTH;
                case ProjectileType.Rocket:
                    return GameInfo.ROCKET_WORTH;
                case ProjectileType.FrostHex:
                    return GameInfo.FROZENBLAST_WORTH;
                case ProjectileType.Hammer:
                    return GameInfo.HAMMER_WORTH;
                case ProjectileType.Hex:
                    return GameInfo.CHAOS_WORTH;
                case ProjectileType.Laser:
                    return GameInfo.LASER_WORTH;
                case ProjectileType.LightningBolt:
                    return GameInfo.LIGHTNING_WORTH;
                case ProjectileType.Meteor:
                    return GameInfo.METEOR_WORTH;
                case ProjectileType.Rock:
                    return GameInfo.ROCK_WORTH;
                case ProjectileType.Snowball:
                    return GameInfo.SNOWBALL_WORTH;
                case ProjectileType.Bone:
                    return GameInfo.BONE_WORTH;
                case ProjectileType.Shuriken:
                    return GameInfo.SHURIKEN_WORTH;
                case ProjectileType.IceShard:
                    return GameInfo.ICESHARD_WORTH;
            }
            return -1;
        }

        public static Color GetColorForBrightness(float brightness)
        {
            if (brightness < 0.5f)
            {
                return Color.Black * Math.Abs(brightness - 0.5f);
            }
            else if (brightness == 0.5f)
            {
                return Color.Transparent;
            }
            else
            {
                return Color.White * (brightness - 0.5f);
            }
        }

        #endregion
    }

    public enum EncryptMethods
    {
        ThreeNumCode = 0,
        CaesarCipher = 1,
    }
}