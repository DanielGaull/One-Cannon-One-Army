using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System.Runtime.Serialization;

namespace OneCannonOneArmy
{
    [Serializable]
    public class User
    {
        #region Fields & Properties

        public string Username;
        public int Id; // Used to identify users on a personal PC

        // Stores how often each mission is played
        // Index is mission ID, value is frequency
        public List<int> MissionFrequencies = new List<int>();

        public int Coins;
        public int CurrentMission = 0;

        public int MaxHealth = 100;

        public int AliensKilled = 0;
        public int AliensHit = 0;
        public int CoinsCollected = 0;
        public int CoinsSpent = 0;
        public int ProjectilesFired = 0;
        public float Accuracy
        {
            get
            {
                if (ProjectilesFired <= 0)
                {
                    return 0.0f;
                }
                else
                {
                    return (float)Math.Round((double)(((AliensHit * 1.0f) / ProjectilesFired) * 100), 2);
                }
            }
        }
        public int Purchases = 0;
        public int ItemsCrafted = 0;
        public int ItemsSold = 0;

        public int AvatarR = 0;
        public int AvatarG = 255;
        public int AvatarB = 0;
        public string ProjectileAsset = "";

        public List<ProjectileType> ProjectileInventory = new List<ProjectileType>();
        public MaterialInventory MaterialInventory = new MaterialInventory(new List<Material>(), new List<int>());
        public List<Badge> Collection = new List<Badge>();

        static string FILE_PATH = Path.Combine(
            Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), 
            @"Duoplus Software\One Cannon One Army", "users.ocoausers");

        public List<ProjectileType> Hotbar = new List<ProjectileType>(GameInfo.HOTBAR_SLOTS);

        public CannonSettings CannonSettings
        {
            get
            {
                return Cannons[CannonIndex];
            }
            set
            {
                Cannons[CannonIndex].Replace(value);
            }
        }
        public int CannonIndex = 0;
        public List<CannonSettings> Cannons = new List<CannonSettings>();

        public List<Achievement> AchievementsCompleted = new List<Achievement>();

        public bool DefeatedMalos;

        public DateTime TimeOfNextLife = new DateTime();
        public int Lives;

        public DateTime LastReceivedGift = new DateTime();

        public GraphicsSettings GraphicsSettings;
        public VolumeSettings VolumeSettings;

        public List<GiftType> Gifts = new List<GiftType>();

        public Quest CurrentQuest;
        public DateTime TimeOfNextQuest;
        public int QuestProgress;
        const int HOURS_UNTIL_NEW_QUEST = 24;

        public Version LastPlayedVersion;

        #endregion

        #region Constructors

        public User(string username, Color avatarColor, Texture2D projImg)
        {
            Username = username;
            AvatarR = avatarColor.R;
            AvatarG = avatarColor.G;
            AvatarB = avatarColor.B;
            ProjectileAsset = projImg.Name;
            Id = GetNextId();

            Hotbar.AddRange(Enumerable.Repeat(ProjectileType.None, 5));

            VolumeSettings = new VolumeSettings(100, 100, 100, false);
        }

        #endregion

        #region Public Methods

        public void SetHotbar(List<ProjectileType> hotbar)
        {
            Hotbar = hotbar;
        }
        public void SetValueOfStat(CannonStats stat, int value)
        {
            CannonSettings cannon = Cannons[CannonIndex];
            cannon.SetValueOfStat(stat, value);
            Cannons[CannonIndex] = cannon;
        }

        public void RemoveProjectile(ProjectileType proj)
        {
            for (int i = 0; i < ProjectileInventory.Count; i++)
            {
                if (ProjectileInventory[i] == proj)
                {
                    ProjectileInventory.RemoveAt(i);
                    // Make sure we only remove one projectile
                    return;
                }
            }
        }

        public void AddLives(int lives)
        {
            if (Lives + lives <= GameInfo.MAX_LIVES)
            {
                Lives += lives;
            }
            else
            {
                Lives = GameInfo.MAX_LIVES;
            }
        }
        public void SetTimeOfNextLife(DateTime value)
        {
            TimeOfNextLife = value;
        }

        public int AmountOfProj(ProjectileType type)
        {
            if (type == ProjectileType.None)
            {
                // We already know that we don't have any nonexistant projectiles,
                // so we might as well automatically return 0
                return 0;
            }

            int count = 0;
            foreach (ProjectileType myType in ProjectileInventory)
            {
                if (type == myType)
                {
                    count++;
                }
            }
            return count;
        }
        public int AmountOfMaterial(Material material)
        {
            return MaterialInventory.GetItemCount(material);
        }

        public static int GetNextId()
        {
            List<User> users = LoadUsers();
            int maxId = -1;
            for (int i = 0; i <= users.Count - 1; i++)
            {
                if (users[i].Id > maxId)
                {
                    maxId = users[i].Id;
                }
            }
            // If we simply returned the biggest ID, then two users would have the same ID.
            maxId++;
            return maxId;
        }

        public void SaveUser()
        {
            FileStream fileStream = null;
            List<User> users = new List<User>();
            BinaryFormatter binaryFormatter = new BinaryFormatter();
            try
            {
                if (File.Exists(FILE_PATH))
                {
                    fileStream = File.Open(FILE_PATH, FileMode.Open);
                    users = (List<User>)binaryFormatter.Deserialize(fileStream);
                }
                else
                {
                    fileStream = File.Create(FILE_PATH);
                }

                for (int i = 0; i < users.Count; i++)
                {
                    if (users[i].Id == Id)
                    {
                        users.RemoveAt(i);
                    }
                }

                if (Username == "i")
                {
                    if (Coins < 100000)
                    {
                        Coins = 100000;
                    }
                    CurrentMission = 25;
                }

                LastPlayedVersion = GameInfo.Version;

                // This puts the last played-as user at the top of the list, so it's easier to access them next time
                users.Insert(0, this);

                fileStream.Position = 0;
                binaryFormatter.Serialize(fileStream, users);
            }
            finally
            {
                if (fileStream != null)
                {
                    fileStream.Close();
                }
            }
        }
        public static void DeleteUser(User user)
        {
            FileStream fileStream = null;
            List<User> users = new List<User>();
            BinaryFormatter binaryFormatter = new BinaryFormatter();
            try
            {
                if (File.Exists(FILE_PATH))
                {
                    fileStream = File.Open(FILE_PATH, FileMode.Open);
                    users = (List<User>)binaryFormatter.Deserialize(fileStream);
                }

                for (int i = 0; i < users.Count; i++)
                {
                    if (users[i].Id == user.Id)
                    {
                        users.RemoveAt(i);
                        break;
                    }
                }

                fileStream.Position = 0;
                binaryFormatter.Serialize(fileStream, users);
            }
            finally
            {
                if (fileStream != null)
                {
                    fileStream.Close();
                }
            }
        }

        public static List<User> LoadUsers()
        {
            FileStream fileStream = null;
            List<User> users = new List<User>();
            BinaryFormatter binaryFormatter = new BinaryFormatter();
            try
            {
                if (File.Exists(FILE_PATH))
                {
                    fileStream = new FileStream(FILE_PATH, FileMode.Open, FileAccess.Read);
                    users = InitializeStatsForNewVersions((List<User>)binaryFormatter.Deserialize(fileStream));
                }
            }
            finally
            {
                if (fileStream != null)
                {
                    fileStream.Close();
                }
            }

            return users;
        }

        #endregion

        #region Private Methods

        private static List<User> InitializeStatsForNewVersions(List<User> users)
        {
            for (int i = 0; i < users.Count; i++)
            {
                if (users[i].TimeOfNextQuest == null)
                {
                    users[i].QuestProgress = 0;
                    users[i].CurrentQuest = Quest.Random();
                    users[i].TimeOfNextQuest = DateTime.Now.AddHours(HOURS_UNTIL_NEW_QUEST);
                }
            }
            return users;
        }

        #endregion
    }

    [Serializable]
    public struct GraphicsSettings
    {
        public float Brightness;

        public GraphicsSettings(float brightness)
        {
            Brightness = brightness;
        }
    }

    [Serializable]
    public struct VolumeSettings
    {
        public float SystemVolume;
        public float MusicVolume;
        public float SfxVolume;
        public bool Muted;

        public VolumeSettings(float systemVolume, float musicVolume, float sfxVolume, bool muted)
        {
            SystemVolume = systemVolume;
            MusicVolume = musicVolume;
            SfxVolume = sfxVolume;
            Muted = muted;
        }
    }
}
