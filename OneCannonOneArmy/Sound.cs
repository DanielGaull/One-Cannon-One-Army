using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OneCannonOneArmy
{
    public static class Sound
    {
        #region Fields & Properties

        static SpriteFont font;
        static Timer timer;
        static bool showingSubtitle = false;
        static string subtitle = "";
        const float WAIT_TIME = 4.0f;
        static Vector2 subtitlePos = new Vector2();
        static Texture2D subtitleBgImg;
        static Rectangle subtitleBgRect;
        static GraphicsDevice graphics;

        static int windowWidth;
        static int windowHeight;

        public static bool ShowSubtitles;

        static SoundEffectInstance currentSong;
        static Music currentSongType;

        public static Dictionary<Sounds, SoundEffect> SoundEffects = new Dictionary<Sounds, SoundEffect>();
        public static Dictionary<Music, SoundEffect> Songs = new Dictionary<Music, SoundEffect>();

        static List<SoundEffectInstance> currentSoundEffects = new List<SoundEffectInstance>();
        static List<Sounds> currentSounds = new List<Sounds>();

        static float systemVolume = 1.0f;
        public static float SystemVolume
        {
            get
            {
                return systemVolume;
            }
            set
            {
                systemVolume = value;
                SetVolumeOfExistingSounds();
            }
        }
        static float musicVolume = 1.0f;
        public static float MusicVolume
        {
            get
            {
                return musicVolume;
            }
            set
            {
                musicVolume = value;
                SetVolumeOfExistingSounds();
            }
        }
        static float sfxVolume = 1.0f;
        public static float SoundEffectsVolume
        {
            get
            {
                return sfxVolume;
            }
            set
            {
                sfxVolume = value;
                SetVolumeOfExistingSounds();
            }
        }

        public static bool PlaySounds
        {
            set
            {
                if (value == false)
                {
                    PauseCurrentSong();
                    PauseAllSounds();
                }
                else
                {
                    ResumeAllSounds();
                    CheckAndPlaySong(currentSongType);
                }
            }
        }

        const int SPACING = 14;

        #endregion

        #region Public Methods

        public static void PlaySound(Sounds sound)
        {
            if (SoundEffects.ContainsKey(sound))
            {
                if (!new UniversalSettings().Muted && GetVolumeForSound(sound) > 0.0f)
                {
                    SoundEffectInstance newS = SoundEffects[sound].CreateInstance();
                    newS.Volume = GetVolumeForSound(sound);
                    newS.Play();
                    currentSoundEffects.Add(newS);
                    currentSounds.Add(sound);
                }

                showingSubtitle = ShowSubtitles;
                if (showingSubtitle)
                {
                    subtitle = Language.Translate(SubtitleForSound(sound));
                    timer.WaitTime = WAIT_TIME;
                    subtitlePos.X = windowWidth / 2 - font.MeasureString(subtitle).X / 2;
                    subtitlePos.Y = windowHeight - font.MeasureString(subtitle).Y - SPACING * 2;
                    subtitleBgImg = new Texture2D(graphics, (int)font.MeasureString(subtitle).X + SPACING, 
                        (int)font.MeasureString(subtitle).Y + SPACING);
                    subtitleBgImg = DrawHelper.AddBorder(subtitleBgImg, 2, Color.DarkGray, Color.White);
                    subtitleBgRect = new Rectangle((int)subtitlePos.X - SPACING / 2, (int)subtitlePos.Y - SPACING / 2, 
                        subtitleBgImg.Width, subtitleBgImg.Height);
                    timer.Reset();
                }
            }
        }
        public static bool IsPlaying(Sounds sound)
        {
            return currentSounds.Contains(sound);
        }
        public static void Stop(Sounds sound)
        {
            if (IsPlaying(sound))
            {
                int index = currentSounds.IndexOf(sound);
                currentSoundEffects[index].Stop(true);
                currentSounds.RemoveAt(index);
                currentSoundEffects.RemoveAt(index);
            }
        }
        public static void StopAllSounds()
        {
            for (int i = 0; i < currentSoundEffects.Count; i++)
            {
                currentSoundEffects[i].Stop();
            }
            currentSoundEffects.Clear();
            currentSounds.Clear();
        }
        public static void PauseAllSounds()
        {
            for (int i = 0; i < currentSoundEffects.Count; i++)
            {
                currentSoundEffects[i].Pause();
            }
        }
        public static void ResumeAllSounds()
        {
            if (!new UniversalSettings().Muted)
            {
                for (int i = 0; i < currentSounds.Count; i++)
                {
                    if (currentSoundEffects[i].State == SoundState.Paused)
                    {
                        currentSoundEffects[i].Resume();
                    }
                }
            }
        }

        public static void Initialize(SpriteFont font, int windowWidth, int windowHeight, ContentManager content,
            Dictionary<Sounds, string> fxAssets, Dictionary<Music, string> songAssets, GraphicsDevice graphics)
        {
            Sound.font = font;
            Sound.windowWidth = windowWidth;
            Sound.windowHeight = windowHeight;
            Sound.graphics = graphics;
            timer = new Timer(WAIT_TIME, TimerUnits.Seconds);

            SoundEffects.Clear();
            Songs.Clear();
            currentSoundEffects.Clear();
            currentSong = null;

            foreach (KeyValuePair<Sounds, string> kv in fxAssets)
            {
                SoundEffects.Add(kv.Key, content.Load<SoundEffect>(kv.Value));
            }
            foreach (KeyValuePair<Music, string> kv in songAssets)
            {
                Songs.Add(kv.Key, content.Load<SoundEffect>(kv.Value));
            }
        }
        public static void Update(GameTime gameTime)
        {
            if (showingSubtitle)
            {
                timer.Update(gameTime);
                showingSubtitle = !timer.QueryWaitTime(gameTime);
            }
            for (int i = 0; i < currentSoundEffects.Count; i++)
            {
                if (currentSoundEffects[i].State == SoundState.Stopped)
                {
                    currentSoundEffects.RemoveAt(i);
                    currentSounds.RemoveAt(i);
                }
            }
        }
        public static void Draw(SpriteBatch spriteBatch)
        {
            if (showingSubtitle)
            {
                spriteBatch.Draw(subtitleBgImg, subtitleBgRect, Color.DarkGray);
                spriteBatch.DrawString(font, subtitle, subtitlePos, Color.LightGray);
            }
        }

        public static void CheckAndPlaySong(Music music)
        {
            CheckAndPlaySong(music, true);
        }
        public static void CheckAndPlaySong(Music music, bool isLooped)
        {
            if ((currentSongType != music || currentSong?.State == SoundState.Stopped) && !new UniversalSettings().Muted)
            {
                StopAllMusic();
                if (Songs.ContainsKey(music))
                {
                    currentSong = Songs[music].CreateInstance();
                    currentSong.Volume = musicVolume;
                    currentSong.Play();
                    currentSongType = music;
                    currentSong.IsLooped = isLooped;
                }
            }
            else if (currentSong?.State == SoundState.Paused && !new UniversalSettings().Muted)
            {
                currentSong.Resume();
            }
        }
        public static void PauseCurrentSong()
        {
            try
            {
                if (currentSong?.State != SoundState.Paused)
                {
                    currentSong?.Pause();
                }
            }
            catch (Exception)
            {
                // This error is thrown when the user restarts the program to replay the tutorial
                currentSong = null;
            }
        }
        public static void StopAllMusic()
        {
            currentSong?.Stop();
        }

        #endregion

        #region Private Methods

        private static string SubtitleForSound(Sounds sound)
        {
            switch (sound)
            {
                case Sounds.Intro:
                    return "Purasu Intro";
                case Sounds.SpellLaunch:
                    return "Spell";
                case Sounds.LaserSustained:
                    return "Laser";
                case Sounds.Open:
                    return "Doors Open";
                case Sounds.Close:
                    return "Doors Close";
                case Sounds.Craft:
                    return "Item Crafted";
                case Sounds.MaterialGift:
                    return "Material Found";
                case Sounds.ProjectileGift:
                    return "Projectile Found";
                default:
                    return sound.ToString().AddSpaces();
            }
        }

        private static float GetVolumeForSound(Sounds sound)
        {
            switch (sound)
            {
                case Sounds.Achievement:
                case Sounds.CaChing:
                case Sounds.Click:
                case Sounds.Close:
                case Sounds.Craft:
                case Sounds.Failure:
                case Sounds.Intro:
                case Sounds.Firework:
                case Sounds.LifeEarned:
                case Sounds.Open:
                case Sounds.Success:
                case Sounds.Notification:
                    return systemVolume;
                case Sounds.AlienDeath:
                case Sounds.AlienHit:
                case Sounds.CannonClunk:
                case Sounds.CannonFire:
                case Sounds.Coin:
                case Sounds.Explosion:
                case Sounds.Laser:
                case Sounds.LaserCharge:
                case Sounds.LaserSustained:
                case Sounds.MetalShaking:
                case Sounds.PlayerHit:
                case Sounds.SpellLaunch:
                case Sounds.Sweep:
                case Sounds.Whoosh:
                case Sounds.ShieldBreak:
                case Sounds.Zap:
                    return sfxVolume;
                default:
                    return 1.0f;
            }
        }

        private static void SetVolumeOfExistingSounds()
        {
            for (int i = 0; i < currentSoundEffects.Count; i++)
            {
                currentSoundEffects[i].Volume = GetVolumeForSound(currentSounds[i]);
            }
            if (currentSong != null)
            {
                currentSong.Volume = musicVolume;
            }
        }

        #endregion
    }

    public enum Sounds
    {
        Intro,
        CannonFire,
        CannonClunk,
        Explosion,
        SpellLaunch,
        Zap,
        Whoosh,
        Laser,
        LaserSustained,
        Click,
        Open,
        Close,
        Firework,
        CaChing,
        LifeEarned,
        Craft,
        Achievement,
        AlienDeath,
        Sweep,
        Coin,
        Success,
        Failure,
        ShieldBreak,
        AlienHit,
        LaserCharge,
        MetalShaking,
        PlayerHit,
        Notification,
        MaterialGift,
        ProjectileGift,
        CageHit,
        Upgrade,
    }

    public enum Music
    {
        LevelMusic,
        MainMenu,
        Credits,
        BossBattle,
        Final3,
        StoryMusic,
        TutorialMusic,
    }
}
