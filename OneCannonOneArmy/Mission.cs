using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace OneCannonOneArmy
{
    public class Mission
    {
        #region Fields

        [XmlArray(ElementName = "AlienList")]
        [XmlArrayItem(ElementName = "AlienType")]
        public List<AlienType> AlienList = new List<AlienType>();
        public int Count = 0;

        public MissionGoal Goal;

        public int Id = 0;
        public string Name = "";
        public string City = "";
        public string StateCountry = "";
        public Planet Planet;
        public int MechAliens = 0;
        int everyThird;
        public int Cages = 0;

        event Action<AlienType, bool> alienAdded;

        public static List<Mission> Missions;
        public const string FILE_PATH = "missions.ocoam";

        #endregion

        #region Constructors

        public Mission()
        {
            // Empty constructor is required for XML serialization
        }
        public Mission(int id, List<AlienType> aliens, string name, string city,
            string secondLocName, Planet planet, int mechParts, int cages, MissionGoal goal)
        {
            Id = id;
            AlienList = aliens;
            Name = name;
            City = city;
            StateCountry = secondLocName;
            Planet = planet;
            MechAliens = mechParts;
            Cages = cages;
            Goal = goal;
        }

        #endregion

        #region Public Methods

        public void AddAlienSpawnHandler(Action<AlienType, bool> handler)
        {
            alienAdded += handler;
        }

        public void Initialize()
        {
            AlienList.Shuffle();
            Count = 0;
            alienAdded = null;
        }

        public void SpawnAlien()
        {
            switch (Goal)
            {
                case MissionGoal.KillAll:
                case MissionGoal.DestroyLaser:
                    if (Count < AlienList.Count)
                    {
                        alienAdded?.Invoke(AlienList[Count], false);
                        Count++;
                    }
                    break;
                case MissionGoal.DestroyMechanics:
                    if (Count < MechAliens)
                    {
                        if (everyThird < 2)
                        {
                            // We can spawn a regular alien
                            SpawnRandomAlien();
                            everyThird++;
                        }
                        else
                        {
                            // Spawn a mech alien
                            everyThird = 0;
                            AlienType alien = AlienList[Utilities.Rand.Next(AlienList.Count)];
                            while (alien.HasShield)
                            {
                                // Make sure aliens with shields can't carry mechanical supplies
                                alien = AlienList[Utilities.Rand.Next(AlienList.Count)];
                            }
                            alienAdded?.Invoke(alien, true);
                        }
                    }
                    break;
                case MissionGoal.SavePeople:
                    SpawnRandomAlien();
                    break;
                case MissionGoal.KillMalos:
                    if (Count <= 0)
                    {
                        // Malos has not yet been spawned
                        alienAdded?.Invoke(new AlienType(Aliens.Boss, false), false);
                        Count++;
                    }
                    else
                    {
                        // Malos is already spawned, so we keep spawning other aliens to keep the player occupied
                        alienAdded?.Invoke(AlienList.Random(), false);
                    }
                    break;
            }
        }

        public int CalculateWorth(AlienLaserCannon cannon, List<Alien> activeAliens, List<Cage> cages)
        {
            int worth = 0;
            switch (Goal)
            {
                case MissionGoal.KillAll:
                case MissionGoal.DestroyLaser:
                    for (int i = Count; i < AlienList.Count; i++)
                    {
                        // This iterates through all un-spawned aliens
                        worth += GameInfo.WorthOf(AlienList[i]);
                    }
                    for (int i = 0; i < activeAliens.Count; i++)
                    {
                        // The first for statement doesn't include aliens that
                        // have been spawned,
                        // Only those that haven't
                        // This includes any aliens that are currently attacking
                        AlienType a = new AlienType(activeAliens[i].Type, activeAliens[i].HasShield);
                        worth += GameInfo.WorthOf(a);
                    }
                    if (Goal == MissionGoal.DestroyLaser)
                    {
                        worth += cannon.Health;
                    }
                    break;

                case MissionGoal.DestroyMechanics:
                    worth = MechAliens - Count;
                    break;

                case MissionGoal.KillMalos:
                    for (int i = 0; i < activeAliens.Count; i++)
                    {
                        if (activeAliens[i].Type == Aliens.Boss)
                        {
                            worth = (int)activeAliens[i].Health;
                        }
                    }
                    break;
                case MissionGoal.SavePeople:
                    for (int i = 0; i < cages.Count; i++)
                    {
                        if (cages[i].Health > 0)
                        {
                            worth += (int)cages[i].Health;
                        }
                    }
                    break;
            }

            return worth;
        }
        public int CalculateTotalWorth()
        {
            int worth = 0;
            switch (Goal)
            {
                case MissionGoal.KillAll:
                case MissionGoal.DestroyLaser:
                    for (int i = 0; i < AlienList.Count; i++)
                    {
                        worth += GameInfo.WorthOf(AlienList[i]);
                    }
                    if (Goal == MissionGoal.DestroyLaser)
                    {
                        worth += AlienLaserCannon.INIT_HEALTH;
                    }
                    break;
                case MissionGoal.DestroyMechanics:
                    worth = MechAliens;
                    break;
                case MissionGoal.SavePeople:
                    worth = Cages * (int)Cage.INIT_HEALTH;
                    break;
                case MissionGoal.KillMalos:
                    worth = GameInfo.BOSS_ALIEN_HEALTH;
                    break;
            }
            return worth;
        }

        #region Static Methods

        public static void InitializeMissions()
        {
            Missions = LoadMissions();
        }
        public static List<Mission> LoadMissions()
        {
            FileStream stream = null;
            stream = File.Open(Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData) + 
                @"\Duoplus Software\One Cannon One Army", 
                FILE_PATH), FileMode.Open);
            XmlSerializer xmlS = new XmlSerializer(typeof(List<Mission>));
            List<Mission> returnVal = (List<Mission>)xmlS.Deserialize(stream);
            stream.Close();
            return returnVal;
        }

        public static int DamageForLevel(Mission mission)
        {
            int damage = 0;
            switch (mission.Goal)
            {
                case MissionGoal.DestroyLaser:
                    // Simply add up damage from the aliens plus the laser
                    for (int i = 0; i < mission.AlienList.Count; i++)
                    {
                        damage += (int)GameInfo.HealthOf(mission.AlienList[i]);
                    }
                    return damage + AlienLaserCannon.INIT_HEALTH;
                case MissionGoal.SavePeople:
                    // Start with the damage of the cages
                    damage += (int)(mission.Cages * Cage.INIT_HEALTH);
                    // Add the health of 2 random aliens per cage
                    for (int i = 0; i < mission.Cages * 2; i++)
                    {
                        damage += (int)GameInfo.HealthOf(mission.AlienList.Random());
                    }
                    return damage;
                case MissionGoal.KillAll:
                    // Damage is simply health of all the aliens added up
                    for (int i = 0; i < mission.AlienList.Count; i++)
                    {
                        damage += (int)GameInfo.HealthOf(mission.AlienList[i]);
                    }
                    return damage;
                case MissionGoal.DestroyMechanics:
                    // Mech Alien spawned every 3 aliens
                    // Simply add health of 3 random aliens, mutliplied by number of mech aliens
                    for (int i = 0; i < mission.MechAliens * 3; i++)
                    {
                        damage += (int)GameInfo.HealthOf(mission.AlienList.Random());
                    }
                    return damage;
                case MissionGoal.KillMalos:
                    // Add about 25 random aliens, plus health of Malos
                    damage = (int)GameInfo.HealthOf(new AlienType(Aliens.Boss, false));
                    for (int i = 0; i < 25; i++)
                    {
                        damage += (int)GameInfo.HealthOf(mission.AlienList.Random());
                    }
                    break;
            }
            return damage;
        }

        #region Create File

        // Only run this once, to build the XML file that can be used for the missions
        private static void SaveMissions()
        {
            XmlSerializer xmlS = new XmlSerializer(typeof(List<Mission>));

            List<Mission> missions = new List<Mission>()
            {
                #region Missions

                new Mission(0, Enumerable.Repeat(new AlienType(Aliens.Normal, false), 2).ToList(),
                    "Tutorial", "Cannon Training Facility", "", Planet.Earth, 0, 0, MissionGoal.KillAll),
                new Mission(1, Enumerable.Repeat(new AlienType(Aliens.Normal, false), 10).ToList(),
                    "Rise of the Aliens", "Estherville", "Iowa", Planet.Earth, 0, 0, MissionGoal.KillAll),
                new Mission(2, Enumerable.Repeat(new AlienType(Aliens.Normal, false), 7)
                    .Concat(Enumerable.Repeat(new AlienType(Aliens.LDefense, false), 3))
                    .ToList(),
                    "Rome Was Not Built in One Day", "Estherville", "Iowa", Planet.Earth, 0, 0, MissionGoal.KillAll),
                new Mission(3, Enumerable.Repeat(new AlienType(Aliens.Normal, false), 10)
                    .Concat(Enumerable.Repeat(new AlienType(Aliens.LDefense, false), 5))
                    .ToList(),
                    "Hat Trick", "Estherville", "Iowa", Planet.Earth, 0, 0, MissionGoal.KillAll),
                new Mission(4, Enumerable.Repeat(new AlienType(Aliens.Normal, false), 12)
                    .Concat(Enumerable.Repeat(new AlienType(Aliens.LDefense, false), 10))
                    .ToList(),
                    "The Last Straw", "Superior", "Iowa", Planet.Earth, 0, 0, MissionGoal.KillAll),
                new Mission(5, Enumerable.Repeat(new AlienType(Aliens.Normal, false), 10)
                    .Concat(Enumerable.Repeat(new AlienType(Aliens.LDefense, false), 10))
                    .Concat(Enumerable.Repeat(new AlienType(Aliens.Defense, false), 3))
                    .ToList(),
                    "Hit Two Aliens With One Stone", "Orleans", "Iowa", Planet.Earth, 0, 0, MissionGoal.KillAll),
                new Mission(6, new List<AlienType>() {
                        new AlienType(Aliens.Normal, false),
                        new AlienType(Aliens.LDefense, false),
                        new AlienType(Aliens.Defense, false),
                    },
                    "Back to the Drawing Board", "Montgomery", "Iowa", Planet.Earth, 3, 0, MissionGoal.DestroyMechanics),
                new Mission(7, new List<AlienType>() {
                        new AlienType(Aliens.Normal, false),
                        new AlienType(Aliens.LDefense, false),
                        new AlienType(Aliens.Defense, false),
                        new AlienType(Aliens.HDefense, false),
                    },
                    "Armored Assault", "Lake Park", "Iowa", Planet.Earth, 5, 0, MissionGoal.DestroyMechanics),
                new Mission(8, new List<AlienType>() {
                        new AlienType(Aliens.Normal, false),
                        new AlienType(Aliens.Normal, true),
                        new AlienType(Aliens.LDefense, false),
                        new AlienType(Aliens.Defense, false),
                        new AlienType(Aliens.HDefense, false),
                    },
                    "It's a Small World", "Harris", "Iowa", Planet.Earth, 7, 0, MissionGoal.DestroyMechanics),
                new Mission(9, Enumerable.Repeat(new AlienType(Aliens.Normal, true), 5)
                    .Concat(Enumerable.Repeat(new AlienType(Aliens.LDefense, true), 3))
                    .Concat(Enumerable.Repeat(new AlienType(Aliens.LFireDefense, false), 5))
                    .Concat(Enumerable.Repeat(new AlienType(Aliens.Defense, false), 5))
                    .Concat(Enumerable.Repeat(new AlienType(Aliens.HDefense, false), 2))
                    .ToList(),
                    "Breaking Ice", "Allendorf", "Iowa", Planet.Earth, 0, 0, MissionGoal.KillAll),
                new Mission(10, new List<AlienType>() {
                        new AlienType(Aliens.LPoisonResistant, false),
                        new AlienType(Aliens.LFireDefense, false),
                        new AlienType(Aliens.LDefense, true),
                        new AlienType(Aliens.Defense, false),
                        new AlienType(Aliens.HDefense, false),
                    },
                    "Spell Game", "Sibley", "Iowa", Planet.Earth, 10, 0, MissionGoal.DestroyMechanics),
                new Mission(11, new List<AlienType>() {
                        new AlienType(Aliens.FireDefense, false),
                        new AlienType(Aliens.Defense, false),
                        new AlienType(Aliens.HDefense, false),
                    },
                    "Battle of Boston", "Boston", "Massachusetts", Planet.Earth, 0, 1, MissionGoal.SavePeople),
                new Mission(12, Enumerable.Repeat(new AlienType(Aliens.FreezeProof, true), 10)
                    .Concat(Enumerable.Repeat(new AlienType(Aliens.FireDefense, true), 10))
                    .Concat(Enumerable.Repeat(new AlienType(Aliens.Defense, true), 10))
                    .Concat(Enumerable.Repeat(new AlienType(Aliens.HFireDefense, false), 5))
                    .Concat(Enumerable.Repeat(new AlienType(Aliens.HDefense, true), 5))
                    .ToList(),
                    "Skirmish in San Antonio", "San Antonio", "Texas", Planet.Earth, 0, 0, MissionGoal.KillAll),
                new Mission(13, new List<AlienType>() {
                        new AlienType(Aliens.PoisonResistant, false),
                        new AlienType(Aliens.FireDefense, false),
                        new AlienType(Aliens.Defense, false),
                        new AlienType(Aliens.HDefense, false),
                        new AlienType(Aliens.FreezeProof, false),
                    },
                    "Chicago Combat", "Chicago", "Illinois", Planet.Earth, 15, 0, MissionGoal.DestroyMechanics),
                new Mission(14, new List<AlienType>() {
                        new AlienType(Aliens.FireDefense, false),
                        new AlienType(Aliens.Defense, false),
                        new AlienType(Aliens.HDefense, false),
                        new AlienType(Aliens.FreezeProof, false),
                    },
                    "Liberation of Los Angeles", "Los Angeles", "California", Planet.Earth, 0, 2, MissionGoal.SavePeople),
                new Mission(15, Enumerable.Repeat(new AlienType(Aliens.FreezeProof, true), 10)
                    .Concat(Enumerable.Repeat(new AlienType(Aliens.Defense, true), 10))
                    .Concat(Enumerable.Repeat(new AlienType(Aliens.PoisonResistant, true), 10))
                    .Concat(Enumerable.Repeat(new AlienType(Aliens.HPoisonResistant, false), 5))
                    .Concat(Enumerable.Repeat(new AlienType(Aliens.HDefense, true), 5))
                    .ToList(),
                    "D.C. Defense", "Washington", "D.C.", Planet.Earth, 0, 0, MissionGoal.KillAll),
                new Mission(16, new List<AlienType>() {
                        new AlienType(Aliens.HDefense, true),
                        new AlienType(Aliens.HFireDefense, false),
                        new AlienType(Aliens.HPoisonResistant, false),
                        new AlienType(Aliens.FreezeProof, true),
                        new AlienType(Aliens.Ninja, false),
                    },
                    "Seoul Duel", "Seoul", "South Korea", Planet.Earth, 20, 0, MissionGoal.DestroyMechanics),
                new Mission(17, new List<AlienType>() {
                        new AlienType(Aliens.HDefense, true),
                        new AlienType(Aliens.HFireDefense, false),
                        new AlienType(Aliens.HPoisonResistant, false),
                        new AlienType(Aliens.FreezeProof, true),
                        new AlienType(Aliens.Ninja, false),
                    },
                    "Clash of Sao Paulo", "Sao Paulo", "Brazil", Planet.Earth, 0, 5, MissionGoal.SavePeople),
                new Mission(18, Enumerable.Repeat(new AlienType(Aliens.Chaos, false), 1)
                    .Concat(Enumerable.Repeat(new AlienType(Aliens.HDefense, true), 5))
                    .Concat(Enumerable.Repeat(new AlienType(Aliens.HFireDefense, true), 5))
                    .Concat(Enumerable.Repeat(new AlienType(Aliens.HPoisonResistant, true), 5))
                    .Concat(Enumerable.Repeat(new AlienType(Aliens.FreezeProof, true), 5))
                    .Concat(Enumerable.Repeat(new AlienType(Aliens.Ninja, false), 5))
                    .ToList(),
                    "Strike Against New York", "New York", "U.S.A.", Planet.Earth, 0, 0, MissionGoal.KillAll),
                new Mission(19, new List<AlienType>() {
                        new AlienType(Aliens.LPlasmaResistant, false),
                        new AlienType(Aliens.HDefense, false),
                        new AlienType(Aliens.HFireDefense, false),
                        new AlienType(Aliens.HPoisonResistant, false),
                        new AlienType(Aliens.Chaos, false),
                        new AlienType(Aliens.Ninja, false),
                    },
                    "Tokyo Invasion", "Tokyo", "Japan", Planet.Earth, 0, 10, MissionGoal.SavePeople),
                new Mission(20, new List<AlienType>() {
                        new AlienType(Aliens.Defense, false),
                        new AlienType(Aliens.FireDefense, false),
                        new AlienType(Aliens.PoisonResistant, false),
                        new AlienType(Aliens.PlasmaResistant, true),
                        new AlienType(Aliens.Chaos, false),
                        new AlienType(Aliens.Ninja, true),
                    },
                    "London Warfare", "London", "England", Planet.Earth, 20, 0, MissionGoal.DestroyMechanics),
                new Mission(21, Enumerable.Repeat(new AlienType(Aliens.Defense, true), 10)
                    .Concat(Enumerable.Repeat(new AlienType(Aliens.FireDefense, true), 10))
                    .Concat(Enumerable.Repeat(new AlienType(Aliens.PoisonResistant, true), 10))
                    .Concat(Enumerable.Repeat(new AlienType(Aliens.PlasmaResistant, true), 10))
                    .Concat(Enumerable.Repeat(new AlienType(Aliens.FreezeProof, true), 10))
                    .Concat(Enumerable.Repeat(new AlienType(Aliens.Chaos, false), 15))
                    .ToList(),
                    "Liftoff", "Space", "", Planet.Space, 0, 0, MissionGoal.KillAll),
                new Mission(22, Enumerable.Repeat(new AlienType(Aliens.HDefense, true), 15)
                    .Concat(Enumerable.Repeat(new AlienType(Aliens.HPoisonResistant, true), 15))
                    .Concat(Enumerable.Repeat(new AlienType(Aliens.FreezeProof, true), 15))
                    .Concat(Enumerable.Repeat(new AlienType(Aliens.Chaos, true), 10))
                    .Concat(Enumerable.Repeat(new AlienType(Aliens.Omega, false), 5))
                    .ToList(),
                    "Attack on Nantak", "Nantak", "", Planet.Nantak, 0, 0, MissionGoal.DestroyLaser),
                new Mission(23, Enumerable.Repeat(new AlienType(Aliens.HDefense, true), 15)
                    .Concat(Enumerable.Repeat(new AlienType(Aliens.HFireDefense, true), 15))
                    .Concat(Enumerable.Repeat(new AlienType(Aliens.FreezeProof, true), 15))
                    .Concat(Enumerable.Repeat(new AlienType(Aliens.Chaos, true), 10))
                    .Concat(Enumerable.Repeat(new AlienType(Aliens.Omega, false), 5))
                    .ToList(),
                    "Attack on Carinus", "Carinus", "", Planet.Carinus, 0, 0, MissionGoal.DestroyLaser),
                new Mission(24, Enumerable.Repeat(new AlienType(Aliens.HDefense, true), 15)
                    .Concat(Enumerable.Repeat(new AlienType(Aliens.HPlasmaResistant, true), 15))
                    .Concat(Enumerable.Repeat(new AlienType(Aliens.FreezeProof, true), 15))
                    .Concat(Enumerable.Repeat(new AlienType(Aliens.Chaos, true), 10))
                    .Concat(Enumerable.Repeat(new AlienType(Aliens.Omega, false), 5))
                    .ToList(),
                    "Attack on Mikara", "Mikara", "", Planet.Mikara, 0, 0, MissionGoal.DestroyLaser),
                new Mission(25, Enumerable.Repeat(new AlienType(Aliens.Normal, false), 10)
                    .Concat(Enumerable.Repeat(new AlienType(Aliens.Omega, false), 1))
                    .Concat(Enumerable.Repeat(new AlienType(Aliens.Normal, true), 1))
                    .ToList(),
                    "Attack on Lithios", "Lithios", "",
                    Planet.Lithios, 0, 0, MissionGoal.KillMalos),

                #endregion
            };

            FileStream stream = File.Open(@"C:\Users\danie\AppData\Roaming\Duoplus Software\One Cannon One Army\missions.ocoam", 
                FileMode.OpenOrCreate);
            xmlS.Serialize(stream, missions);
            stream.Close();
        }

        //public static void SaveMissions()
        //{
        //    XmlSerializer xmlS = new XmlSerializer(typeof(List<Mission>));

        //    List<Mission> missions = new List<Mission>()
        //    {

        //        new Mission(0, Enumerable.Repeat(new AlienType(Aliens.Normal, false), 2).ToList(),
        //            "Tutorial", "Cannon Training Facility", "", Planet.Earth, false, false),
        //        new Mission(1, Enumerable.Repeat(new AlienType(Aliens.Normal, false), 10).ToList(),
        //            "Rise of the Aliens", "Estherville", "Iowa", Planet.Earth, false, false),
        //        new Mission(2, Enumerable.Repeat(new AlienType(Aliens.Normal, false), 7)
        //            .Concat(Enumerable.Repeat(new AlienType(Aliens.LDefense, false), 3)).ToList(),
        //            "Rome Was Not Built in One Day", "Estherville", "Iowa", Planet.Earth, false, false),
        //        new Mission(3, Enumerable.Repeat(new AlienType(Aliens.Normal, false), 10)
        //            .Concat(Enumerable.Repeat(new AlienType(Aliens.LDefense, false), 5)).ToList(),
        //            "Hat Trick", "Estherville", "Iowa", Planet.Earth, false, false),
        //        new Mission(4, Enumerable.Repeat(new AlienType(Aliens.Normal, false), 12)
        //            .Concat(Enumerable.Repeat(new AlienType(Aliens.LDefense, false), 10)).ToList(),
        //            "The Last Straw", "Superior", "Iowa", Planet.Earth, false, false),
        //        new Mission(5, Enumerable.Repeat(new AlienType(Aliens.Normal, false), 10)
        //            .Concat(Enumerable.Repeat(new AlienType(Aliens.LDefense, false), 5))
        //            .Concat(Enumerable.Repeat(new AlienType(Aliens.Defense, false), 3)).ToList(),
        //            "Hit Two Aliens with One Stone", "Orleans", "Iowa", Planet.Earth, false, false),
        //        new Mission(6, Enumerable.Repeat(new AlienType(Aliens.Normal, false), 15)
        //            .Concat(Enumerable.Repeat(new AlienType(Aliens.LDefense, false), 10))
        //            .Concat(Enumerable.Repeat(new AlienType(Aliens.Defense, false), 7)).ToList(),
        //            "Back to the Drawing Board", "Montgomery", "Iowa", Planet.Earth, false, true),
        //        new Mission(7, Enumerable.Repeat(new AlienType(Aliens.Normal, false), 5)
        //            .Concat(Enumerable.Repeat(new AlienType(Aliens.LDefense, false), 7))
        //            .Concat(Enumerable.Repeat(new AlienType(Aliens.Defense, false), 10))
        //            .Concat(Enumerable.Repeat(new AlienType(Aliens.HDefense, false), 5)).ToList(),
        //            "Armored Assault", "Lake Park", "Iowa", Planet.Earth, false, false),
        //        new Mission(8, Enumerable.Repeat(new AlienType(Aliens.Normal, false), 10)
        //            .Concat(Enumerable.Repeat(new AlienType(Aliens.LDefense, false), 10))
        //            .Concat(Enumerable.Repeat(new AlienType(Aliens.Defense, false), 10))
        //            .Concat(Enumerable.Repeat(new AlienType(Aliens.HDefense, false), 10))
        //            .Concat(Enumerable.Repeat(new AlienType(Aliens.Normal, true), 1))
        //            .ToList(),
        //            "It's a Small World", "Harris", "Iowa", Planet.Earth, false, false),
        //        new Mission(9, Enumerable.Repeat(new AlienType(Aliens.LDefense, false), 10)
        //            .Concat(Enumerable.Repeat(new AlienType(Aliens.Defense, false), 5))
        //            .Concat(Enumerable.Repeat(new AlienType(Aliens.HDefense, false), 2))
        //            .Concat(Enumerable.Repeat(new AlienType(Aliens.Normal, true), 5))
        //            .Concat(Enumerable.Repeat(new AlienType(Aliens.LDefense, true), 3))
        //            .Concat(Enumerable.Repeat(new AlienType(Aliens.LFireDefense, false), 5))
        //            .ToList(),
        //            "Breaking Ice", "Allendorf", "Iowa", Planet.Earth, false, false),
        //        new Mission(10, Enumerable.Repeat(new AlienType(Aliens.LPoisonResistant, false), 3)
        //            .Concat(Enumerable.Repeat(new AlienType(Aliens.Defense, false), 5))
        //            .Concat(Enumerable.Repeat(new AlienType(Aliens.HDefense, false), 2))
        //            .Concat(Enumerable.Repeat(new AlienType(Aliens.LDefense, true), 7))
        //            .Concat(Enumerable.Repeat(new AlienType(Aliens.LFireDefense, false), 7))
        //            .ToList(),
        //            "Spell Game", "Sibely", "Iowa", Planet.Earth, false, false),
        //        new Mission(11, Enumerable.Repeat(new AlienType(Aliens.FireDefense, false), 5)
        //            .Concat(Enumerable.Repeat(new AlienType(Aliens.Defense, false), 10))
        //            .Concat(Enumerable.Repeat(new AlienType(Aliens.HDefense, false), 10))
        //            .ToList(),
        //            "Battle of Boston", "Boston", "Massachusetts", Planet.Earth, false, false),
        //        new Mission(12, Enumerable.Repeat(new AlienType(Aliens.FireDefense, false), 10)
        //            .Concat(Enumerable.Repeat(new AlienType(Aliens.Defense, false), 10))
        //            .Concat(Enumerable.Repeat(new AlienType(Aliens.HDefense, false), 10))
        //            .Concat(Enumerable.Repeat(new AlienType(Aliens.FreezeProof, false), 5))
        //            .ToList(),
        //            "Skirmish in San Antonio", "San Antonio", "Texas", Planet.Earth, false, false),
        //        new Mission(13, Enumerable.Repeat(new AlienType(Aliens.FireDefense, false), 10)
        //            .Concat(Enumerable.Repeat(new AlienType(Aliens.Defense, false), 10))
        //            .Concat(Enumerable.Repeat(new AlienType(Aliens.HDefense, false), 10))
        //            .Concat(Enumerable.Repeat(new AlienType(Aliens.FreezeProof, false), 10))
        //            .Concat(Enumerable.Repeat(new AlienType(Aliens.PoisonResistant, false), 5))
        //            .ToList(),
        //            "Chicago Combat", "Chicago", "Illinois", Planet.Earth, false, false),
        //        new Mission(14, Enumerable.Repeat(new AlienType(Aliens.HFireDefense, false), 5)
        //            .Concat(Enumerable.Repeat(new AlienType(Aliens.Defense, true), 10))
        //            .Concat(Enumerable.Repeat(new AlienType(Aliens.HDefense, true), 5))
        //            .Concat(Enumerable.Repeat(new AlienType(Aliens.FreezeProof, true), 10))
        //            .Concat(Enumerable.Repeat(new AlienType(Aliens.FireDefense, true), 10))
        //            .ToList(),
        //            "Liberation of Los Angeles", "Los Angeles", "California", Planet.Earth, false, false),
        //        new Mission(15, Enumerable.Repeat(new AlienType(Aliens.HPoisonResistant, false), 5)
        //            .Concat(Enumerable.Repeat(new AlienType(Aliens.Defense, true), 10))
        //            .Concat(Enumerable.Repeat(new AlienType(Aliens.HDefense, true), 5))
        //            .Concat(Enumerable.Repeat(new AlienType(Aliens.FreezeProof, true), 10))
        //            .Concat(Enumerable.Repeat(new AlienType(Aliens.PoisonResistant, true), 10))
        //            .ToList(),
        //            "D.C. Defense", "Washington", "D.C.", Planet.Earth, false, false),
        //        new Mission(16, Enumerable.Repeat(new AlienType(Aliens.HDefense, true), 5)
        //            .Concat(Enumerable.Repeat(new AlienType(Aliens.HFireDefense, false), 5))
        //            .Concat(Enumerable.Repeat(new AlienType(Aliens.HPoisonResistant, false), 5))
        //            .Concat(Enumerable.Repeat(new AlienType(Aliens.FreezeProof, true), 5))
        //            .ToList(),
        //            "Seoul Duel", "Seoul", "South Korea", Planet.Earth, false, false),
        //        new Mission(17, Enumerable.Repeat(new AlienType(Aliens.HDefense, true), 10)
        //            .Concat(Enumerable.Repeat(new AlienType(Aliens.HFireDefense, false), 10))
        //            .Concat(Enumerable.Repeat(new AlienType(Aliens.HPoisonResistant, false), 10))
        //            .Concat(Enumerable.Repeat(new AlienType(Aliens.FreezeProof, true), 10))
        //            .ToList(),
        //            "Clash of Sao Paulo", "Sao Paulo", "Brazil", Planet.Earth, false, false),
        //        new Mission(18, Enumerable.Repeat(new AlienType(Aliens.HDefense, true), 5)
        //            .Concat(Enumerable.Repeat(new AlienType(Aliens.HFireDefense, true), 5))
        //            .Concat(Enumerable.Repeat(new AlienType(Aliens.HPoisonResistant, true), 5))
        //            .Concat(Enumerable.Repeat(new AlienType(Aliens.FreezeProof, true), 5))
        //            .Concat(Enumerable.Repeat(new AlienType(Aliens.Chaos, false), 1))
        //            .ToList(),
        //            "Strike Against New York", "New York", "U.S.A", Planet.Earth, false, false),
        //        new Mission(19, Enumerable.Repeat(new AlienType(Aliens.HDefense, false), 10)
        //            .Concat(Enumerable.Repeat(new AlienType(Aliens.HFireDefense, false), 10))
        //            .Concat(Enumerable.Repeat(new AlienType(Aliens.HPoisonResistant, false), 10))
        //            .Concat(Enumerable.Repeat(new AlienType(Aliens.LPlasmaResistant, false), 10))
        //            .Concat(Enumerable.Repeat(new AlienType(Aliens.Chaos, false), 5))
        //            .ToList(),
        //            "Tokyo Invasion", "Tokyo", "Japan", Planet.Earth, false, false),
        //        new Mission(20, Enumerable.Repeat(new AlienType(Aliens.Defense, false), 15)
        //            .Concat(Enumerable.Repeat(new AlienType(Aliens.FireDefense, false), 15))
        //            .Concat(Enumerable.Repeat(new AlienType(Aliens.PoisonResistant, false), 15))
        //            .Concat(Enumerable.Repeat(new AlienType(Aliens.PlasmaResistant, true), 15))
        //            .Concat(Enumerable.Repeat(new AlienType(Aliens.Chaos, false), 5))
        //            .ToList(),
        //            "London Warfare", "London", "England", Planet.Earth, false, false),
        //        new Mission(21, Enumerable.Repeat(new AlienType(Aliens.Defense, true), 10)
        //            .Concat(Enumerable.Repeat(new AlienType(Aliens.FireDefense, true), 10))
        //            .Concat(Enumerable.Repeat(new AlienType(Aliens.PoisonResistant, true), 10))
        //            .Concat(Enumerable.Repeat(new AlienType(Aliens.PlasmaResistant, true), 10))
        //            .Concat(Enumerable.Repeat(new AlienType(Aliens.FreezeProof, true), 10))
        //            .Concat(Enumerable.Repeat(new AlienType(Aliens.Chaos, false), 15))
        //            .ToList(),
        //            "Liftoff", "Space", "", Planet.Space, false, false),
        //        new Mission(22, Enumerable.Repeat(new AlienType(Aliens.HDefense, true), 15)
        //            .Concat(Enumerable.Repeat(new AlienType(Aliens.Omega, true), 5))
        //            .Concat(Enumerable.Repeat(new AlienType(Aliens.HPoisonResistant, true), 15))
        //            .Concat(Enumerable.Repeat(new AlienType(Aliens.FreezeProof, true), 15))
        //            .Concat(Enumerable.Repeat(new AlienType(Aliens.Chaos, true), 10))
        //            .ToList(),
        //            "Attack on Nantak", "Nantak", "", Planet.Nantak, true, false),
        //        new Mission(23, Enumerable.Repeat(new AlienType(Aliens.HDefense, true), 15)
        //            .Concat(Enumerable.Repeat(new AlienType(Aliens.Omega, true), 5))
        //            .Concat(Enumerable.Repeat(new AlienType(Aliens.HFireDefense, true), 15))
        //            .Concat(Enumerable.Repeat(new AlienType(Aliens.FreezeProof, true), 15))
        //            .Concat(Enumerable.Repeat(new AlienType(Aliens.Chaos, true), 10))
        //            .ToList(),
        //            "Attack on Carinus", "Carinus", "", Planet.Carinus, true, false),
        //        new Mission(24, Enumerable.Repeat(new AlienType(Aliens.HDefense, true), 15)
        //            .Concat(Enumerable.Repeat(new AlienType(Aliens.Omega, true), 5))
        //            .Concat(Enumerable.Repeat(new AlienType(Aliens.HPlasmaResistant, true), 15))
        //            .Concat(Enumerable.Repeat(new AlienType(Aliens.FreezeProof, true), 15))
        //            .Concat(Enumerable.Repeat(new AlienType(Aliens.Chaos, true), 10))
        //            .ToList(),
        //            "Attack on Mikara", "Mikara", "", Planet.Mikara, true, false),
        //        new Mission(25, new List<AlienType> { new AlienType(Aliens.Boss, false) },
        //            "Attack on Lithios", "Lithios", "", Planet.Lithios, false, false),

        //    };
        //    FileStream stream = File.Open(FILE_PATH, FileMode.OpenOrCreate);
        //    xmlS.Serialize(stream, missions);
        //    stream.Close();
        //}

        #endregion

        #endregion

        #endregion

        #region Private Methods

        private void SpawnRandomAlien()
        {
            int index = Utilities.Rand.Next(AlienList.Count);
            alienAdded?.Invoke(AlienList[index], false);
        }

        #endregion
    }

    public struct Planet
    {
        public Color SkyColor;
        public Color GroundColor;

        public Planet(Color sky, Color ground)
        {
            SkyColor = sky;
            GroundColor = ground;
        }

        #region Planets

        public static Planet Earth
        {
            get
            {
                return new Planet(GameInfo.EARTH_BG, GameInfo.EARTH_GROUND);
            }
        }
        public static Planet Space
        {
            get
            {
                return new Planet(GameInfo.SPACE_BG, GameInfo.SPACE_GROUND);
            }
        }
        public static Planet Nantak
        {
            get
            {
                return new Planet(GameInfo.NANTAK_BG, GameInfo.NANTAK_GROUND);
            }
        }
        public static Planet Carinus
        {
            get
            {
                return new Planet(GameInfo.CARINUS_BG, GameInfo.CARINUS_GROUND);
            }
        }
        public static Planet Mikara
        {
            get
            {
                return new Planet(GameInfo.MIKARA_BG, GameInfo.MIKARA_GROUND);
            }
        }
        public static Planet Lithios
        {
            get
            {
                return new Planet(GameInfo.LITHIOS_BG, GameInfo.LITHIOS_GROUND);
            }
        }

        #endregion
    }

    public class MissionMenu
    {
        #region Fields

        List<MenuButton> buttons = new List<MenuButton>();
        List<int> ids = new List<int>();
        const int BUTTONS_PER_ROW = 5;

        const int XSPACING = 100;
        const int YSPACING = 5;
        const int MOUSE_SPACING = 20;

        int X_OFFSET;
        const int Y_OFFSET = Utilities.MENU_Y_OFFSET;

        SpriteFont font;
        Vector2 mousePos = new Vector2();
        string currentName = "";
        bool hovering = false;
        bool showCompletePrevMissionsFirst = false;

        event Action<int> missionSelected;

        int windowWidth;

        Texture2D checkImg;
        List<Rectangle> checkMarks = new List<Rectangle>();

        #endregion

        #region Constructors

        public MissionMenu(GraphicsDevice graphics, SpriteFont smallFont, SpriteFont bigFont, int windowWidth, int windowHeight,
            Texture2D checkImg)
        {
            font = smallFont;
            this.windowWidth = windowWidth;
            this.checkImg = checkImg;
            for (int i = 0; i < Mission.Missions.Count; i++)
            {
                ids.Add(Mission.Missions[i].Id);
                buttons.Add(new MenuButton(OnClick, Mission.Missions[i].Id.ToString(), 0, 0, true, bigFont, graphics));
            }
            int maxWidth = buttons[buttons.Count - 1].Width;
            X_OFFSET = windowWidth / 2 - (((maxWidth * BUTTONS_PER_ROW) + (XSPACING * (BUTTONS_PER_ROW - 1))) / 2);

            // Position buttons
            int lastX = X_OFFSET;
            int lastY = Y_OFFSET;
            int buttonsInCurrentRow = 0;
            int firstIdInRow = 0;
            for (int i = 0; i < buttons.Count; i++)
            {
                buttons[i].Width = maxWidth;
                if (buttonsInCurrentRow >= BUTTONS_PER_ROW)
                {
                    // Go to the next row
                    buttonsInCurrentRow = 0;
                    lastX = X_OFFSET;
                    lastY += buttons[i].Height + YSPACING;
                    firstIdInRow += BUTTONS_PER_ROW;
                }
                if (buttons.Count - firstIdInRow == 1)
                {
                    // There's only one button in this row
                    lastX = windowWidth / 2 - (buttons[i].Width / 2);
                }
                buttons[i].X = lastX;
                buttons[i].Y = lastY;
                lastX += buttons[i].Width + XSPACING;
                buttonsInCurrentRow++;
            }
        }

        #endregion

        #region Public Methods

        public void Update(User user, GameTime gameTime)
        {
            int mouseX = Mouse.GetState().X;
            if (mouseX > windowWidth / 2)
            {
                mousePos.X = mouseX - font.MeasureString(currentName).X - MOUSE_SPACING;
            }
            else
            {
                mousePos.X = mouseX + MOUSE_SPACING;
            }
            mousePos.Y = Mouse.GetState().Y;

            hovering = false;
            for (int i = 0; i < buttons.Count; i++)
            {
                buttons[i].Update(gameTime);
                if (buttons[i].Hovered)
                {
                    string location = "";
                    Mission m = Mission.Missions[ids[i]];
                    if (m.StateCountry.Trim() == "")
                    {
                        location = m.City;
                    }
                    else
                    {
                        location = m.City + ", " + m.StateCountry;
                    }
                    currentName = Language.Translate(m.Name) + "\n" + location;
                    showCompletePrevMissionsFirst = user.CurrentMission < ids[i];
                    hovering = true;
                }
                buttons[i].Active = user.CurrentMission >= ids[i];
            }
        }

        public void Draw(SpriteBatch spriteBatch)
        {
            for (int i = 0; i < buttons.Count; i++)
            {
                buttons[i].Draw(spriteBatch);
            }
            for (int i = 0; i < checkMarks.Count; i++)
            {
                spriteBatch.Draw(checkImg, checkMarks[i], Color.White);
            }
            if (hovering)
            {
                spriteBatch.DrawString(font, currentName, mousePos, Color.Black);
                if (showCompletePrevMissionsFirst)
                {
                    spriteBatch.DrawString(font, Language.Translate("Complete previous missions first."),
                        new Vector2(mousePos.X, mousePos.Y + font.MeasureString(currentName).Y), Color.Red);
                }
            }
        }

        public void AddClickHandler(Action<int> handler)
        {
            missionSelected += handler;
        }

        public List<MenuButton> GetButtons()
        {
            return buttons;
        }

        public void Initialize(User user)
        {
            checkMarks.Clear();
            for (int i = 0; i < buttons.Count; i++)
            {
                if (i < user.CurrentMission)
                {
                    MissionComplete(i);
                }
            }
        }
        public void MissionComplete(int id)
        {
            checkMarks.Add(new Rectangle(buttons[id].X + YSPACING, buttons[id].Y + YSPACING, 
                buttons[id].Width - YSPACING * 2, buttons[id].Height - YSPACING * 2));
        }

        #endregion

        #region Private Methods

        private void OnClick(MenuButton b)
        {
            int index = buttons.IndexOf(b);
            missionSelected?.Invoke(index);
        }

        #endregion
    }

    public class MissionPlayable
    {
        // This class handles all the mission, projectile,
        // and alien info
        // Like the various menu classes, but for GameState.Playing
        #region Fields & Properties

        public Mission Mission;
        AlienLaserCannon laserCannon;
        const int LASER_CANNON_WIDTH = 68;
        const int LASER_CANNON_HEIGHT = 110;

        public bool Sweeping
        {
            get
            {
                return playerInterface.Sweeping;
            }
        }

        TimeSpan levelTime = new TimeSpan();
        public bool RemoveLifeOnQuit { get; private set; }
        const int TIME_BEFORE_LIFE_REMOVED = 10;

        List<Projectile> projectiles = new List<Projectile>();
        Cannon cannon;
        const int CANNON_WIDTH = 60;
        const int CANNON_HEIGHT = 120;

        List<Item> drops = new List<Item>();

        List<Alien> aliens = new List<Alien>();
        List<Projectile> alienProjectiles = new List<Projectile>();
        Timer alienTimer;
        const int ALIEN_TIME_MIN = 2;
        const int ALIEN_TIME_MAX = 7;
        const int ALIEN_WIDTH = 50;
        const int ALIEN_HEIGHT = 25;

        PlayerInterface playerInterface;

        Random rand;

        int windowWidth;
        int windowHeight;

        GraphicsDevice graphics;
        Texture2D alienImg;
        Texture2D alienEyeImg;

        event Action<bool, int> missionOver;
        bool canCallMissionOver = false;
        bool calledMissionOver = false;
        bool waitingForSweep = false;
        bool missionSuccess = false;

        event OnAlienHit onAlienHit;
        event Action<Material> onMaterialCollect;
        event Action<Badge> onBadgeCollect;
        event OnAlienHit onAlienDeath;

        List<Cage> cages = new List<Cage>();
        int cagesDestroyed = 0;
        const int CAGE_SIZE = 50;

        float defensePercentage;

        User user;

        public float MaxHealth
        {
            get
            {
                return playerInterface.MaxHealth;
            }
            set
            {
                playerInterface.MaxHealth = value;
            }
        }
        public float Health
        {
            get
            {
                return playerInterface.Health;
            }
            set
            {
                playerInterface.Health = value;
            }
        }
        public List<Projectile> Projectiles
        {
            get
            {
                return projectiles;
            }
        }

        SpriteFont font;

        const int COIN_SIZE = 15;
        const int SPACING = 2;

        #endregion

        #region Constructors

        public MissionPlayable(GraphicsDevice graphics, SpriteFont bigFont, SpriteFont mediumFont,
            SpriteFont smallFont, int windowWidth, int windowHeight,

            Texture2D sweepImg, Texture2D laserCannonImg, Texture2D alienImg, Texture2D alienEyeImg,
            Texture2D lifeImg, Texture2D rapidFireIcon, Texture2D crossImg)
        {
            this.windowWidth = windowWidth;
            this.windowHeight = windowHeight;
            this.graphics = graphics;
            this.alienImg = alienImg;
            this.alienEyeImg = alienEyeImg;

            font = mediumFont;

            rand = Utilities.Rand;

            playerInterface = new PlayerInterface(windowWidth, windowHeight, sweepImg,
                graphics, bigFont, mediumFont, GameInfo.STARTING_MAX_HEALTH, lifeImg, rapidFireIcon, crossImg);

            cannon = new Cannon(CANNON_WIDTH, CANNON_HEIGHT, windowWidth, windowHeight - PlayerInterface.BOTTOM_HEIGHT,
                graphics, playerInterface.ChangePrimaryProj);

            laserCannon = new AlienLaserCannon(laserCannonImg, 0, 0, LASER_CANNON_WIDTH,
                LASER_CANNON_HEIGHT);
            laserCannon.AddLaserHitHandler(MissionFailed);
            laserCannon.AddOnDeathHandler(new System.Action(() => MissionCompleted(ref user)));

            alienTimer = new Timer(1.0f, TimerUnits.Seconds);
        }

        #endregion

        #region Public Methods

        public void Initialize(User user)
        {
            cannon.ChangeSettings(user.CannonSettings);
            playerInterface.MaxHealth = user.CannonSettings.MaxHealth;
            playerInterface.Health = playerInterface.MaxHealth;
            defensePercentage = user.CannonSettings.Defense / 100.0f;

            projectiles.Clear();
            foreach (ProjectileType item in user.ProjectileInventory)
            {
                AddProjectile(item);
            }
        }

        public void Update(GameTime gameTime, ref User user, bool needSweep, bool needProjFire, bool restrictActions)
        {
            // Makes sure all the hotbar slots are moved as far left as possible
            List<ProjectileType> hotbar = user.Hotbar;
            for (int i = 0; i < hotbar.Count; i++)
            {
                if (i > 0)
                {
                    if (hotbar[i - 1] == ProjectileType.None && hotbar[i] != ProjectileType.None)
                    {
                        // The previous slot is empty, while the current one is not
                        hotbar[i - 1] = hotbar[i];
                        hotbar[i] = ProjectileType.None;
                    }
                }
            }
            user.SetHotbar(hotbar);

            levelTime += gameTime.ElapsedGameTime;
            if (!RemoveLifeOnQuit && levelTime.TotalSeconds > TIME_BEFORE_LIFE_REMOVED)
            {
                RemoveLifeOnQuit = true;
            }

            if (!restrictActions)
            {
                // Check for and play music
                if (!canCallMissionOver)
                {
                    if (this.Mission.Id == GameInfo.FINAL_LEVEL)
                    {
                        Sound.CheckAndPlaySong(Music.BossBattle);
                    }
                    else if (this.Mission?.Id < GameInfo.FINAL_LEVEL &&
                        this.Mission?.Id >= GameInfo.FINAL_LEVEL - GameInfo.FINAL_THREE_CONST)
                    {
                        Sound.CheckAndPlaySong(Music.Final3);
                    }
                    else
                    {
                        Sound.CheckAndPlaySong(Music.LevelMusic);
                    }
                }
                else
                {
                    Sound.StopAllMusic();
                }

                // Why stop the player from selecting their projectile while the level info is showing?
                if (cannon.MachineCannon != playerInterface.RapidFire && user.CannonSettings.RapidFire > 0)
                {
                    cannon.MachineCannon = playerInterface.RapidFire;
                }
                else
                {
                    cannon.MachineCannon = false;
                }
                playerInterface.AllowRapidFire = user.CannonSettings.RapidFire > 0;

                if (waitingForSweep || (!LevelInfo.HasActiveGoalPopups && !canCallMissionOver))
                {
                    for (int i = 0; i < drops.Count; i++)
                    {
                        drops[i].Update(gameTime);

                        // Keeps drops within the sweeper's range
                        if (drops[i].Y + drops[i].Height > windowHeight - PlayerInterface.BOTTOM_HEIGHT)
                        {
                            drops[i].Y = windowHeight - PlayerInterface.BOTTOM_HEIGHT - drops[i].Height;
                        }
                        else if (drops[i].Y < PlayerInterface.TOP_HEIGHT && !drops[i].Flying)
                        {
                            drops[i].Y = PlayerInterface.TOP_HEIGHT;
                        }

                        if (drops[i].Rectangle.Intersects(playerInterface.UsernameRect) &&
                            (drops[i] is MaterialDrop || drops[i] is BadgeDrop))
                        {
                            CollectItem(drops[i], ref user);
                            continue;
                        }
                        else if (drops[i].Rectangle.Intersects(playerInterface.CoinDisplayRect) &&
                            drops[i] is Coin)
                        {
                            CollectItem(drops[i], ref user);
                            continue;
                        }
                    }
                }
                if (drops.Count == 0 && waitingForSweep)
                {
                    waitingForSweep = false;
                }
                if (canCallMissionOver && !waitingForSweep && !calledMissionOver)
                {
                    missionOver?.Invoke(missionSuccess, Mission.Id);
                    calledMissionOver = true;
                }

                if (!LevelInfo.HasActiveGoalPopups && !canCallMissionOver)
                {
                    // We don't start the level until we know that the information popup is gone

                    if (Mission.Goal == MissionGoal.KillMalos && Mission.Count <= 0)
                    {
                        // We need to spawn Malos immediately
                        Mission.SpawnAlien();
                    }
                    alienTimer.Update(gameTime);
                    if (alienTimer.QueryWaitTime(gameTime))
                    {
                        Mission.SpawnAlien();
                        alienTimer.WaitTime = rand.Next(ALIEN_TIME_MIN, ALIEN_TIME_MAX);
                        alienTimer.Reset();
                    }

                    if (IsMissionCompleted(Mission))
                    {
                        if (Mission.Goal == MissionGoal.DestroyLaser)
                        {
                            // Now the player has to destroy the laser
                            if (!laserCannon.Active && laserCannon.Health > 0)
                            {
                                laserCannon.Spawn(windowWidth, windowHeight, PlayerInterface.TOP_HEIGHT);
                            }
                            else
                            {
                                MissionCompleted(ref user);
                            }
                        }
                        else
                        {
                            MissionCompleted(ref user);
                        }
                    }
                    if (Mission.Goal == MissionGoal.DestroyLaser && laserCannon.Active)
                    {
                        laserCannon.Update(gameTime, ref projectiles);
                    }

                    if (Mission.Goal == MissionGoal.SavePeople)
                    {
                        for (int i = 0; i < cages.Count; i++)
                        {
                            if (cages[i].Health > 0)
                            {
                                cages[i].Update(ref projectiles);
                            }
                        }
                    }

                    for (int i = 0; i < aliens.Count; i++)
                    {
                        aliens[i].Update(ref projectiles, gameTime, ref alienProjectiles);
                        if (aliens[i].Y >= windowHeight - PlayerInterface.BOTTOM_HEIGHT)
                        {
                            Sound.PlaySound(Sounds.PlayerHit);
                            int damage = aliens[i].Worth;
                            playerInterface.AddHealth((int)((damage - (defensePercentage * damage)) * -1));
                            aliens.RemoveAt(i);
                            continue;
                        }
                        if (!aliens[i].Active)
                        {
                            if (aliens[i].Dead)
                            {
                                user.AliensKilled++;
                            }
                            aliens.RemoveAt(i);
                        }
                    }

                    for (int i = 0; i < alienProjectiles.Count; i++)
                    {
                        alienProjectiles[i].Update(gameTime);
                        if (alienProjectiles[i].Y >= windowHeight - PlayerInterface.BOTTOM_HEIGHT)
                        {
                            int damage = alienProjectiles[i].Damage;
                            playerInterface.AddHealth((int)((damage - damage * defensePercentage) * -1));
                            alienProjectiles.RemoveAt(i);
                            Sound.PlaySound(Sounds.PlayerHit);
                        }
                    }

                    foreach (Projectile p in alienProjectiles)
                    {
                        p.Move(p.Speed, Direction.Down);
                    }

                    if (playerInterface.Health <= 0)
                    {
                        MissionFailed();
                    }

                    playerInterface.SetProgress(Mission.CalculateTotalWorth() - Mission.CalculateWorth(laserCannon, aliens, cages));
                }
            }
            playerInterface.Update(ref drops, ref user, projectiles, needSweep, restrictActions, gameTime);
            if (needProjFire || !restrictActions)
            {
                if ((!LevelInfo.HasActiveGoalPopups && !canCallMissionOver) || needProjFire)
                {
                    cannon.Update(ref projectiles, gameTime, playerInterface.PrimaryProj, user, !needProjFire);
                }
            }

            this.user = user;
        }

        public void Draw(SpriteBatch spriteBatch, User user, GameState gameState)
        {
            foreach (Alien a in aliens)
            {
                a.Draw(spriteBatch);
            }
            foreach (Projectile p in alienProjectiles)
            {
                p.Draw(spriteBatch, true);
            }
            if (laserCannon.Active)
            {
                laserCannon.Draw(spriteBatch);
            }

            cannon.Draw(spriteBatch, ref projectiles);
            playerInterface.Draw(spriteBatch, user);

            // Draw coins first
            List<Item> dropsToDraw = drops.Where(x => x is Coin).ToList();
            for (int i = 0; i < dropsToDraw.Count(); i++)
            {
                dropsToDraw[i].Draw(spriteBatch);
            }
            // Then materials and badges
            dropsToDraw = drops.Where(x => x is BadgeDrop || x is MaterialDrop).ToList();
            for (int i = 0; i < dropsToDraw.Count(); i++)
            {
                dropsToDraw[i].Draw(spriteBatch);
            }

            if (Mission.Goal == MissionGoal.SavePeople)
            {
                for (int i = 0; i < cages.Count; i++)
                {
                    if (cages[i].Health > 0)
                    {
                        cages[i].Draw(spriteBatch);
                    }
                }
            }
        }
        public void DrawCoinDisplay(SpriteBatch spriteBatch, User user)
        {
            playerInterface.DrawUserInfoDisplay(spriteBatch, user);
        }
        public void DrawLives(SpriteBatch spriteBatch, User user, bool small)
        {
            playerInterface.DrawLives(spriteBatch, user, small);
        }

        public void LoadMission(int id, User user)
        {
            if (projectiles.Count > 0)
            {
                cannon.Initialize(user.Hotbar[0]);
            }

            playerInterface.Initialize(user);
            playerInterface.Reset();
            cannon.Reset();

            aliens.Clear();
            alienProjectiles.Clear();
            drops.Clear();

            Mission = Mission.Missions[id];
            Mission.Initialize();
            GameInfo.Planet = Mission.Planet;
            Mission.AddAlienSpawnHandler(AddAlien);
            playerInterface.SetProgressMax(Mission.CalculateTotalWorth());
            playerInterface.SetProgress(0);

            if (Mission.Goal == MissionGoal.SavePeople)
            {
                cages.Clear();

                // Calculate the size of the space between the cages
                int xSpacing = (windowWidth - CAGE_SIZE * Mission.Cages) / (Mission.Cages + 1);
                int x = xSpacing;
                for (int i = 0; i < Mission.Cages; i++)
                {
                    Cage newCage = new Cage(x, PlayerInterface.TOP_HEIGHT, CAGE_SIZE, CAGE_SIZE);
                    newCage.AddOnDeathHandler(CageDestroyed);
                    cages.Add(newCage);
                    x += xSpacing + CAGE_SIZE;
                }
            }

            cagesDestroyed = 0;

            // Get rid of any projectiles that were fired in the last mission
            // and are left over
            for (int i = 0; i < projectiles.Count; i++)
            {
                if (projectiles[i].Flying)
                {
                    projectiles.RemoveAt(i);
                }
                if (projectiles[i] is ExplosiveProjectile)
                {
                    if ((projectiles[i] as ExplosiveProjectile).Exploding)
                    {
                        projectiles.RemoveAt(i);
                    }
                }
            }

            RemoveLifeOnQuit = false;
            levelTime = new TimeSpan();

            int amount = 0;
            if (Mission.Goal == MissionGoal.DestroyMechanics)
            {
                amount = Mission.MechAliens;
            }
            else if (Mission.Goal == MissionGoal.SavePeople)
            {
                amount = Mission.Cages;
            }
            // If the user started a level then left before the level info could exit, then
            // there is an unwanted LevelInfoPopup that's still active
            // Thus, we must clear the LevelInfo list
            LevelInfo.ClearGoalPopups();
            LevelInfo.ShowGoalPopup(Mission.Goal, amount);

            //if (Mission.Id == 24)
            //{
            //    Mission.AlienList.Clear();
            //    Mission.AlienList.Add(new AlienType(Aliens.Normal, false));
            //}

            canCallMissionOver = false;
            calledMissionOver = false;

            
        }

        public List<MenuButton> GetButtons()
        {
            List<MenuButton> returnVal = new List<MenuButton>();

            returnVal.AddRange(playerInterface.GetButtons());

            return returnVal;
        }

        public void ChangeLives(int lives)
        {
            playerInterface.ChangeLives(lives);
        }

        public void ChangeCannonSettings(CannonSettings settings)
        {
            cannon.ChangeSettings(settings);
        }
        public void AddProjectile(ProjectileType pType)
        {
            Projectile newP = GameInfo.CreateProj(pType);
            newP.AddOnImpactHandler(ProjectileImpact);
            projectiles.Add(newP);
        }
        public void RemoveProjectile(ProjectileType pType)
        {
            for (int i = 0; i < projectiles.Count; i++)
            {
                if (projectiles[i].Type == pType)
                {
                    projectiles.RemoveAt(i);
                    // Make sure we only remove one projectile
                    return;
                }
            }
        }

        public void HandleExplosion(Vector2 location, float damage, float aoe)
        {
            for (int i = 0; i < aliens.Count; i++)
            {
                // Check if the alien is within the area of effect
                // Use the equation (x - x2) ^ 2 + (y - y2) < radius ^ 2
                if ((float)(Math.Pow(aliens[i].X - location.X, (double)2.0)) + Math.Pow(aliens[i].Y - location.Y, (double)2.0) <
                    Math.Pow(aoe, 2))
                {
                    // The alien is within the radius to take damage
                    aliens[i].ApplyDamage(damage, true);
                }
            }
        }

        #region Add Event Handlers

        public void AddOnLaunchHandler(Action<ProjectileType> handler)
        {
            cannon.AddOnLaunchHandler(handler);
        }
        public void AddMissionOverHandler(Action<bool, int> handler)
        {
            missionOver += handler;
        }
        public void AddOnAlienHitHandler(OnAlienHit handler)
        {
            onAlienHit += handler;
        }
        public void AddOnMaterialCollectHandler(Action<Material> handler)
        {
            onMaterialCollect += handler;
        }
        public void AddOnBadgeCollectHandler(Action<Badge> handler)
        {
            onBadgeCollect += handler;
        }
        public void AddSweepFinishedHandler(Action handler)
        {
            playerInterface.AddSweepFinishedHandler(handler);
        }
        public void AddProjRemovedHandler(Action handler)
        {
            cannon.AddProjRemovedHandler(handler);
        }
        public void AddOnAlienDeathHandler(OnAlienHit handler)
        {
            onAlienDeath += handler;
        }

        #endregion

        #endregion

        #region Private Methods

        private void ProjectileImpact(Projectile p)
        {
            int i = projectiles.IndexOf(p);
            if (p.DestroyOnHit)
            {
                projectiles[i].Active = false;
                projectiles.RemoveAt(i);
            }
            if (p.HealingPower > 0 && Health < MaxHealth)
            {
                Health += p.HealingPower;
                if (Health > MaxHealth)
                {
                    Health = MaxHealth;
                }
            }
        }

        private bool IsMissionCompleted(Mission mission)
        {
            switch (mission.Goal)
            {
                case MissionGoal.KillAll:
                    return (aliens.Count == 0 && mission.Count >= mission.AlienList.Count);
                case MissionGoal.DestroyMechanics:
                    return (mission.Count >= mission.MechAliens && aliens.Count == 0);
                case MissionGoal.SavePeople:
                    return (cagesDestroyed >= mission.Cages);
                case MissionGoal.KillMalos:
                    for (int i = 0; i < aliens.Count; i++)
                    {
                        if (aliens[i].Type == Aliens.Boss)
                        {
                            return false;
                        }
                    }
                    return true;
                case MissionGoal.DestroyLaser:
                    if (aliens.Count == 0 && mission.Count >= mission.AlienList.Count)
                    {
                        if (!laserCannon.Active)
                        {
                            laserCannon.Spawn(windowWidth, windowHeight, PlayerInterface.TOP_HEIGHT);
                        }
                        else if (laserCannon.Health <= 0)
                        {
                            return true;
                        }
                    }
                    return false;
            }

            return false;
        }

        private Alien CreateAlien(Aliens type, bool hasShield)
        {
            return GameInfo.CreateAlien(type, hasShield, windowWidth, windowHeight, ALIEN_WIDTH, ALIEN_HEIGHT,
                ItemCollected);
        }

        private void AddAlien(AlienType type, bool mechAlien)
        {
            Alien newAlien = CreateAlien(type.Type, type.HasShield);
            newAlien.AddOnHitHandler(new OnAlienHit(onAlienHit));
            newAlien.AddOnDeathHandler(new OnAlienHit(OnAlienDeath));
            newAlien.IsMechAlien = mechAlien;
            aliens.Add(newAlien);
        }

        private void OnAlienDeath(Alien alien)
        {
            drops.AddRange(alien.Drops);
            if (alien.IsMechAlien)
            {
                Mission.Count++;
            }
            onAlienDeath?.Invoke(alien);
        }

        private void ItemCollected(Item item)
        {
            if (item is Coin)
            {
                if (drops.Contains(item))
                {
                    int index = drops.IndexOf(item);
                    drops[index].StartFlying(playerInterface.CoinDisplayPoint);
                }
            }
            else if (item is MaterialDrop)
            {
                if (drops.Contains(item))
                {
                    MaterialDrop m = item as MaterialDrop;
                    onMaterialCollect?.Invoke(m.Drop);
                    int index = drops.IndexOf(item);
                    drops[index].StartFlying(playerInterface.UsernamePoint);
                }
            }
            else if (item is BadgeDrop)
            {
                if (drops.Contains(item))
                {
                    BadgeDrop b = item as BadgeDrop;
                    onBadgeCollect?.Invoke(b.Badge);
                    int index = drops.IndexOf(item);
                    drops[index].StartFlying(playerInterface.UsernamePoint);
                }
            }
        }
        private void CollectItem(Item item, ref User user)
        {
            if (item is MaterialDrop)
            {
                MaterialDrop m = item as MaterialDrop;
                user.MaterialInventory.AddItem(m.Drop, 1);

                // We have collected our item, so now let's remove it
                drops.Remove(item);
            }
            else if (item is Coin)
            {
                Coin c = item as Coin;
                user.Coins += c.Worth;
                user.CoinsCollected += c.Worth;
                Sound.PlaySound(Sounds.Coin);

                // We have collected our item, so now let's remove it
                drops.Remove(item);
            }
            else if (item is BadgeDrop)
            {
                BadgeDrop b = item as BadgeDrop;
                user.Collection.Add(b.Badge);

                // We have collected our item, so now let's remove it
                drops.Remove(item);
            }
        }

        private void MissionCompleted(ref User user)
        {
            if (!canCallMissionOver)
            {
                Sound.StopAllMusic();
                Sound.PlaySound(Sounds.Success);

                MissionOver();

                missionSuccess = true;
                if (Mission.Id == user.CurrentMission)
                {
                    // Only increment the current mission if the user is doing the highest
                    // possible mission. Otherwise, users can keep completing the tutorial
                    // and all the other missions will unlock
                    user.CurrentMission++;
                }

                if (Mission.Goal == MissionGoal.DestroyLaser)
                {
                    LaserCannonDestroyed();
                    laserCannon.Reset();
                }
            }
        }
        private void MissionFailed()
        {
            if (!canCallMissionOver)
            {
                Sound.StopAllMusic();
                Sound.PlaySound(Sounds.Failure);

                MissionOver();
                missionSuccess = false;
                if (Mission.Goal == MissionGoal.DestroyLaser)
                {
                    laserCannon.Reset();
                }
            }
        }
        private void MissionOver()
        {
            canCallMissionOver = true;
            waitingForSweep = true;
            playerInterface.Sweep();

            // Remove any explosive projectiles playing their animation
            for (int i = 0; i < projectiles.Count; i++)
            {
                if (projectiles[i] is ExplosiveProjectile)
                {
                    ExplosiveProjectile p = projectiles[i] as ExplosiveProjectile;
                    if (p.Exploding)
                    {
                        projectiles.RemoveAt(i);
                    }
                }
            }
        }

        private void CageDestroyed(Cage cage)
        {
            cagesDestroyed++;
            AddCoins(GameInfo.CAGE_WORTH, cage.X, cage.Y);
        }
        private void LaserCannonDestroyed()
        {
            AddCoins(GameInfo.LASER_CANNON_WORTH, laserCannon.X, laserCannon.Y);
        }
        private void AddCoins(int coins, int x, int y)
        {
            int randomNumX, randomNumY;
            for (int i = 0; i < coins; i++)
            {
                Coin newC = new Coin(Utilities.CoinImage, 0, 0, COIN_SIZE, COIN_SIZE, 1);
                newC.AddOnCollectHandler(ItemCollected);
                randomNumX = Utilities.Rand.Next((COIN_SIZE + SPACING) * -1, COIN_SIZE + SPACING);
                randomNumY = Utilities.Rand.Next((COIN_SIZE + SPACING) * -1, COIN_SIZE + SPACING);
                newC.X = x + randomNumX;
                newC.Y = y + randomNumY;
                drops.Add(newC);
            }
        }

        #endregion
    }

    public enum MissionGoal
    {
        KillAll,
        KillMalos,
        DestroyMechanics,
        SavePeople,
        DestroyLaser,
    }
}