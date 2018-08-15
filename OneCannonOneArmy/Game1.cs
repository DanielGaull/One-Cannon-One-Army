using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;

namespace OneCannonOneArmy
{
    public delegate bool BoolAction();

    /// <summary>
    /// This is the main type for your game.
    /// </summary>
    public class Game1 : Game
    {
        #region Remove Exit Button Definitions

        [DllImport("user32.dll")]
        static extern bool EnableMenuItem(IntPtr hMenu, uint uIDEnableItem, uint uEnable);
        [DllImport("user32.dll")]
        static extern IntPtr GetSystemMenu(IntPtr hWnd, bool bRevert);
        internal const UInt32 SC_CLOSE = 0xF060;
        internal const UInt32 MF_ENABLED = 0x00000000;
        internal const UInt32 MF_GRAYED = 0x00000001;
        internal const UInt32 MF_DISABLED = 0x00000002;
        internal const uint MF_BYCOMMAND = 0x00000000;

        #endregion

        #region Fields

        GraphicsDeviceManager graphics;
        SpriteBatch spriteBatch;

        // Set to true so it runs the first time (sets to false in LoadContent()
        // If set to true, will restart when Exit(); is called
        public bool Restart = true;

        Texture2D imgNotFound;

        Rectangle brightnessFilterRect;
        Color brightness;

        //Mission mission;

        public const int WINDOW_WIDTH = 900;
        public const int WINDOW_HEIGHT = 800;

        //Cannon cannon;
        //List<Projectile> projectiles = new List<Projectile>();
        //List<Alien> aliens = new List<Alien>();

        UniversalSettings uSettings = new UniversalSettings();

        Menu menu;
        PreLvlPopup preLvlPopup = null;
        MissionPlayable game;

        SpriteFont bigFont;
        SpriteFont mediumFont;
        SpriteFont smallFont;

        MouseCursor mouse;

        Random rand;

        User player;

        PopupInstance ctrlW;

        GameState gameState = GameState.StartMenu;
        bool started = false;

        GameState gameStateToBe = GameState.StartMenu;
        bool opening = false;
        bool closing = false;
        bool openAfterClose = true;
        bool drawDoors = false;
        Texture2D doorImg;
        Rectangle leftDoorRect;
        Rectangle rightDoorRect;
        const int DOOR_SPEED = 15;

        LevelCompletePopup lvlComplete;
        const int LVLCOMPLETE_WIDTH = 500;
        const int LVLCOMPLETE_HEIGHT = 400;

        Texture2D bgImg;
        Rectangle bgRect;

        KeyboardState currentKeyState;
        KeyboardState prevKeyState;

        const int ALIEN_WIDTH = 50;
        const int ALIEN_HEIGHT = 25;

        //Timer alienTimer;
        Texture2D alienImg;
        Texture2D alienEyeImg;

        //List<Item> alienItems = new List<Item>();
        //List<Projectile> alienProjectiles = new List<Projectile>();

        const int ALIEN_TIME_MIN = 2;
        const int ALIEN_TIME_MAX = 7;

        //PlayerInterface playerInterface;

        StudioIntro intro;
        Tutorial tutorial;

        Texture2D rightArrowImg;

        //AlienLaserCannon laserCannon;
        //const int LASER_CANNON_WIDTH = 68;
        //const int LASER_CANNON_HEIGHT = 110;

        string versionString;
        Vector2 versionPos;

        TutorialAction actionAllowed;

        LangSelectorPopup langSelectPopup;

        Dictionary<Material, int> itemCosts;

        #endregion

        #region Constructors

        public Game1()
        {
            graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";
            Window.Title = "One Cannon, One Army";

            graphics.PreferredBackBufferWidth = WINDOW_WIDTH;
            graphics.PreferredBackBufferHeight = WINDOW_HEIGHT;

            IsMouseVisible = false;

            Error.OkClicked += Close;
        }

        #endregion

        #region Protected and Public Methods

        protected override void OnExiting(object sender, EventArgs args)
        {
            Sound.StopAllMusic();
            Sound.StopAllSounds();
            if (player != null)
            {
                SaveUser();
            }
            base.OnExiting(sender, args);
        }

        /// <summary>
        /// Allows the game to perform any initialization it needs to before starting to run.
        /// This is where it can query for any required services and load any non-graphic
        /// related content.  Calling base.Initialize will enumerate through any components
        /// and initialize them as well.
        /// </summary>
        protected override void Initialize()
        {
            // TODO: Add your initialization logic here

            base.Initialize();
        }

        /// <summary>
        /// LoadContent will be called once per game and is the place to load
        /// all of your content.
        /// </summary>
        protected override void LoadContent()
        {
            imgNotFound = Content.Load<Texture2D>("imgnotfound");
            try
            {
                EnableOrDisableCloseButton(true);

                rand = new Random();
                Utilities.Rand = rand;

                //UniversalSettings settings = new UniversalSettings();
                //settings.ViewedStartingStory = false;
                //settings.PlayedTutorial = false;
                //settings.ViewedFullIntro = false;
                //settings.ShowCtrlWPopup = true;
                //settings.Muted = false;
                //settings.Captions = false;
                //settings.Language = 0;
                //settings.Save();

                Restart = false;

                // Check if any users are eligible to gain any lives
                List<User> users = User.LoadUsers();
                TimeSpan distance;
                for (int i = 0; i < users.Count; i++)
                {
                    // First check if the user actually NEEDS any lives
                    if (users[i].Lives == GameInfo.MAX_LIVES)
                    {
                        continue;
                    }
                    // They need lives, so find the time span between when they last played and now
                    distance = DateTime.Now - users[i].TimeOfNextLife;
                    if (distance.Ticks < 0)
                    {
                        // A negative time means that no lives have been earned
                        // If we continue to execute this loop, then
                        // the negative value will be divided and we will lose lives
                        continue;
                    }
                    // Now divide the total minutes by the minutes that you gain each life
                    float livesAdded = Math.Min(GameInfo.MAX_LIVES, (float)distance.TotalMinutes / GameInfo.MINUTES_UNTIL_NEXT_LIFE);
                    users[i].SetTimeOfNextLife(DateTime.Now.AddMinutes(GameInfo.MINUTES_UNTIL_NEXT_LIFE));
                    // Then make the new lives and integer and add it
                    users[i].AddLives((int)livesAdded);
                    users[i].SaveUser();
                }

                Dictionary<Sounds, string> soundAssets = new Dictionary<Sounds, string>()
                {
                    { Sounds.Intro, "intro" },
                    { Sounds.CannonFire, "cannon shot" },
                    { Sounds.CannonClunk, "clunk fail" },
                    { Sounds.SpellLaunch, "spell launch" },
                    { Sounds.Zap, "zap" },
                    { Sounds.Whoosh, "metallic whoosh" },
                    { Sounds.Laser, "laser bolt" },
                    { Sounds.LaserSustained, "laser sustained" },
                    { Sounds.Click, "click" },
                    { Sounds.Open, "open" },
                    { Sounds.Close, "close" },
                    { Sounds.Explosion, "explosion" },
                    { Sounds.Firework, "firework" },
                    { Sounds.CaChing, "buy" },
                    { Sounds.LifeEarned, "lifeearned" },
                    { Sounds.Craft, "metal hammer" },
                    { Sounds.Achievement, "notification" },
                    { Sounds.AlienDeath, "alien death" },
                    { Sounds.Sweep, "slide" },
                    { Sounds.Coin, "coinsound" },
                    { Sounds.Success, "success" },
                    { Sounds.Failure, "failure" },
                    { Sounds.ShieldBreak, "shield break" },
                    { Sounds.AlienHit, "alien hit" },
                    { Sounds.LaserCharge, "laser charge" },
                    { Sounds.MetalShaking, "metal shaking" },
                    { Sounds.PlayerHit, "hit" },
                    { Sounds.Notification, "achievement" },
                    { Sounds.MaterialGift, "material gift" },
                    { Sounds.ProjectileGift, "projectile gift" },
                    { Sounds.CageHit, "cage hit" },
                    { Sounds.Upgrade, "upgrade" },
                };
                Dictionary<Music, string> musicAssets = new Dictionary<Music, string>()
                {
                    { Music.LevelMusic, "level music" },
                    { Music.MainMenu, "main menu" },
                    { Music.BossBattle, "boss battle" },
                    { Music.Final3, "final 3" },
                    { Music.Credits, "credits music" },
                    { Music.StoryMusic, "story music" },
                    { Music.TutorialMusic, "tutorial" },
                };

                Achievements.Initialize();
                Mission.InitializeMissions();

                GameInfo.Planet = Planet.Earth;
                GameInfo.Language = (Languages)(new UniversalSettings().Language);

                // Create a new SpriteBatch, which can be used to draw textures.
                spriteBatch = new SpriteBatch(GraphicsDevice);

                bgImg = LoadImg("ocoa bg");
                bgRect = new Rectangle(0, 0, WINDOW_WIDTH, WINDOW_HEIGHT);

                Texture2D rectImg = new Texture2D(GraphicsDevice, 1, 1);
                rectImg.SetData(new Color[] { Color.White });
                Utilities.RectImage = rectImg;

                // Initialize Material Images
                Utilities.StoneImg = LoadImg("stonematerial");
                Utilities.MetalImg = LoadImg("metalmaterial");
                Utilities.IceImg = LoadImg("icematerial");
                Utilities.PoisonImg = LoadImg("plantmatter");
                Utilities.GunpowderImg = LoadImg("gunpowdermaterial");
                Utilities.FireImg = LoadImg("firematerial");
                Utilities.PlasmaImg = LoadImg("plasmamaterial");
                Utilities.ChaosEnergyImg = LoadImg("chaosenergy");
                Utilities.BoneMaterialImg = LoadImg("bone");

                // Initialize Projectile Images
                Utilities.BatteryImg = LoadImg("battery");
                Utilities.BombImg = LoadImg("bomb");
                Utilities.CannonballImg = LoadImg("cannonball");
                Utilities.ChaosImg = LoadImg("chaosblast");
                Utilities.DartImg = LoadImg("dart");
                Utilities.ExplosionSheet = LoadImg("explosionsheet");
                Utilities.FireballIcon = LoadImg("fireballicon");
                Utilities.FireballImg = LoadImg("fireball");
                Utilities.LaserIcon = LoadImg("lasercross");
                Utilities.LaserImg = LoadImg("laser");
                Utilities.LightningBoltImg = LoadImg("lightningsheet");
                Utilities.LightningIcon = LoadImg("lightningicon");
                Utilities.PoisonDartImg = LoadImg("poisondart");
                Utilities.RockImg = LoadImg("rock");
                Utilities.FrozenBlastImg = LoadImg("frozenblast");
                Utilities.FrozenBlastIcon = LoadImg("frozenicon");
                Utilities.MeteorImg = LoadImg("meteor");
                Utilities.HammerImg = LoadImg("hammer");
                Utilities.SnowballImg = LoadImg("snowball");
                Utilities.RocketImg = LoadImg("rocket");
                Utilities.FireRktImg = LoadImg("firerocket");
                Utilities.PoisonRktImg = LoadImg("poisonrocket");
                Utilities.FrozenRktImg = LoadImg("frozenrocket");
                Utilities.PlasmaRktImg = LoadImg("plasmarocket");
                Utilities.OmegaRktImg = LoadImg("omegarocket");
                Utilities.BoneImg = LoadImg("bone");
                Utilities.ShurikenImg = LoadImg("shuriken");
                Utilities.IceShardImg = LoadImg("iceshard");
                Utilities.AbsorbHexImg = LoadImg("absorbhex");

                Utilities.AlienImg = LoadImg("alien");
                Utilities.AlienEyeImg = LoadImg("alieneyes");
                Utilities.AlienShieldImg = LoadImg("shieldicon");

                // Initialize goal icon values
                Utilities.MechImg = LoadImg("alienmech");
                Utilities.CageImg = LoadImg("cage");
                Utilities.NoMalosImg = LoadImg("malosicon");
                Utilities.NoAlienImg = LoadImg("noalienicon");
                Utilities.NoLaserImg = LoadImg("nolasericon");

                // Initialize Cannon images
                Utilities.NormCannonBottomImg = LoadImg("cannoninterior");
                Utilities.NormCannonTubeImg = LoadImg("cannontube");
                Utilities.BronzeCannonBottomImg = LoadImg("bronzecannoninterior");
                Utilities.BronzeCannonTubeImg = LoadImg("bronzecannontube");
                Utilities.SilverCannonBottomImg = LoadImg("silvercannoninterior");
                Utilities.SilverCannonTubeImg = LoadImg("silvercannontube");
                Utilities.GoldCannonBottomImg = LoadImg("goldcannoninterior");
                Utilities.GoldCannonTubeImg = LoadImg("goldcannontube");
                Utilities.MasterCannonBottomImg = LoadImg("mastercannoninterior");
                Utilities.MasterCannonTubeImg = LoadImg("mastercannontube");
                Utilities.InfernoCannonBottomImg = LoadImg("infernocannoninterior");
                Utilities.InfernoCannonTubeImg = LoadImg("infernocannontube");
                Utilities.FrozenCannonBottomImg = LoadImg("frozencannoninterior");
                Utilities.FrozenCannonTubeImg = LoadImg("frozencannontube");

                // Initialize badge values
                Utilities.RedTrophyImg = LoadImg("redtrophy");
                Utilities.OrangeTrophyImg = LoadImg("orangetrophy");
                Utilities.YellowTrophyImg = LoadImg("yellowtrophy");
                Utilities.GreenTrophyImg = LoadImg("greentrophy");
                Utilities.BlueTrophyImg = LoadImg("bluetrophy");
                Utilities.PurpleTrophyImg = LoadImg("purpletrophy");

                // Initialize Projectile size values
                Utilities.NormProjWidth = 20;
                Utilities.NormProjHeight = 20;
                Utilities.LaserWidth = 10;
                Utilities.LaserHeight = 33;
                Utilities.LightningWidth = 20;
                Utilities.LightningHeight = 37;

                Utilities.HotbarIcon = LoadImg("clickrockicon");
                Utilities.BadgeIcon = LoadImg("trophyicon");

                bigFont = Content.Load<SpriteFont>("bigfont");
                mediumFont = Content.Load<SpriteFont>("mediumfont");
                smallFont = Content.Load<SpriteFont>("smallfont");

                Sound.Initialize(mediumFont, WINDOW_WIDTH, WINDOW_HEIGHT, Content, soundAssets, musicAssets, GraphicsDevice);

                Texture2D checkImg = LoadImg("check");

                versionString = "One Cannon, One Army v" + GameInfo.Version;
                versionPos = new Vector2(0, WINDOW_HEIGHT - smallFont.MeasureString(versionString).Y);

                Popup.Initialize(WINDOW_WIDTH, WINDOW_WIDTH, smallFont, bigFont, GraphicsDevice, checkImg);
                Notification.Initialize(GraphicsDevice, WINDOW_WIDTH, WINDOW_HEIGHT, mediumFont);
                LevelInfo.Initialize(WINDOW_WIDTH, WINDOW_HEIGHT, mediumFont, GraphicsDevice);

                ctrlW = new PopupInstance(HandleCtrlW, checkImg, smallFont, GraphicsDevice, WINDOW_WIDTH,
                    WINDOW_HEIGHT, true, bigFont);

                Utilities.CoinImage = LoadImg("coin");
                Utilities.CoinIcon = LoadImg("coinicon");
                Utilities.GiftImg = LoadImg("gift");

                //alienTimer = new Timer(1.0f, TimerUnits.Seconds);
                alienImg = LoadImg("alien");
                alienEyeImg = LoadImg("alieneyes");

                List<Texture2D> projImgs = new List<Texture2D>()
            {
                Utilities.RockImg,
                Utilities.CannonballImg,
                Utilities.BombImg,
                Utilities.ChaosImg,
                Utilities.DartImg,
                Utilities.PoisonDartImg,
                Utilities.LaserIcon,
                Utilities.LightningIcon,
                Utilities.FireballIcon,
                Utilities.FrozenBlastIcon,
                Utilities.MeteorImg,
                Utilities.HammerImg,
                Utilities.SnowballImg,
                Utilities.RocketImg,
                Utilities.PoisonRktImg,
                Utilities.FireRktImg,
                Utilities.FrozenRktImg,
                Utilities.PlasmaRktImg,
                Utilities.OmegaRktImg,
                Utilities.BoneImg,
                Utilities.ShurikenImg,
                Utilities.IceShardImg,
                Utilities.AbsorbHexImg,
            };

                List<Projectile> shopProjs = new List<Projectile>()
            {
                new Rock(0, 100, Utilities.NormProjWidth * 2, Utilities.NormProjHeight * 2),
                new Cannonball(0, 100, Utilities.NormProjWidth * 2, Utilities.NormProjHeight * 2),
                new Dart(0, 100, Utilities.NormProjWidth * 2, Utilities.NormProjHeight * 2),
                new PoisonDart(0, 100, Utilities.NormProjWidth * 2, Utilities.NormProjHeight * 2),
                new Bomb(0, 100, Utilities.NormProjWidth * 2, Utilities.NormProjHeight * 2),
                new Fireball(0, 100, Utilities.NormProjWidth * 2, Utilities.NormProjHeight * 2),
                new LightningBolt(0, 100, (int)(Utilities.LightningWidth * 1.5f),
                    (int)(Utilities.LightningHeight * 1.5f)),
                new Hex(0, 100, Utilities.NormProjWidth * 2, Utilities.NormProjHeight * 2),
                new Laser(0, 100, (int)(Utilities.LaserWidth * 1.5f), (int)(Utilities.LaserHeight * 1.5f)),
                new FrozenBlast(0, 100, Utilities.NormProjWidth * 2, Utilities.NormProjHeight * 2),
                new Meteor(0, 100, Utilities.NormProjWidth * 2, Utilities.NormProjHeight * 2),
                new Hammer(0, 100, Utilities.NormProjWidth * 2, Utilities.NormProjHeight * 2),
                new Snowball(0, 100, Utilities.NormProjWidth * 2, Utilities.NormProjHeight * 2),
                new Rocket(0, 100, Utilities.NormProjWidth * 2, Utilities.NormProjHeight * 2),
                new PoisonRocket(0, 100, Utilities.NormProjWidth * 2, Utilities.NormProjHeight * 2),
                new FireRocket(0, 100, Utilities.NormProjWidth * 2, Utilities.NormProjHeight * 2),
                new FrozenRocket(0, 100, Utilities.NormProjWidth * 2, Utilities.NormProjHeight * 2),
                new PlasmaRocket(0, 100, Utilities.NormProjWidth * 2, Utilities.NormProjHeight * 2),
                new OmegaRocket(0, 100, Utilities.NormProjWidth * 2, Utilities.NormProjHeight * 2),
                new Bone(0, 100, (int)(Utilities.LightningWidth * 1.5f), Utilities.NormProjHeight * 2),
                new Shuriken(0, 100, Utilities.NormProjWidth * 2, Utilities.NormProjHeight * 2),
                new IceShard(0, 100, Utilities.NormProjWidth * 2, Utilities.NormProjHeight * 2),
                new AbsorbHex(0, 100, Utilities.NormProjWidth * 2, Utilities.NormProjHeight * 2),
                //new AlienMinion(0, 100, Utilities.AlienWidth * 2, Utilities.AlienHeight * 2),
            };

                List<Material> items = GameInfo.MaterialsAllowed;
                itemCosts = new Dictionary<Material, int>()
            {
                { Material.Stone, GameInfo.STONE_COST },
                { Material.Metal, GameInfo.METAL_COST },
                { Material.PlantMatter, GameInfo.POISON_COST },
                { Material.EssenceOfFire, GameInfo.FIRE_COST },
                { Material.Ice, GameInfo.ICE_COST },
                { Material.ChaosEnergy, GameInfo.CHAOSENERGY_COST },
                { Material.Plasma, GameInfo.PLASMA_COST },
                { Material.Gunpowder, GameInfo.GUNPOWDER_COST },
            };
                items.OrderBy(item => itemCosts[item]);

                shopProjs = shopProjs.OrderBy(p => GameInfo.ProjVisibilityLvls[p.Type]).ToList();

                List<CannonSettings> cannons = new List<CannonSettings>()
            {
                CannonSettings.NormalCannon,
                CannonSettings.BronzeCannon,
                CannonSettings.SilverCannon,
                CannonSettings.GoldCannon,
                CannonSettings.EliteCannon,
                CannonSettings.InfernoCannon,
                CannonSettings.FrozenCannon,
            };
                Dictionary<CannonSettings, int> cannonCosts = new Dictionary<CannonSettings, int>()
            {
                { CannonSettings.NormalCannon, 0 },
                { CannonSettings.BronzeCannon, 1000 },
                { CannonSettings.SilverCannon, 2000 },
                { CannonSettings.GoldCannon, 3000 },
                { CannonSettings.EliteCannon, 4500 },
                { CannonSettings.InfernoCannon, 2500 },
                { CannonSettings.FrozenCannon, 2500 },
            };
                cannons.OrderBy(x => cannonCosts[x]);

                List<CannonStats> cannonStats = new List<CannonStats>()
            {
                CannonStats.Health,
                CannonStats.Damage,
                CannonStats.Defense,
                CannonStats.Accuracy,
                CannonStats.ReloadSpeed,
                CannonStats.MoveSpeed,
                CannonStats.Power,
                CannonStats.RapidFire,
            };
                Texture2D rapidFireIcon = LoadImg("rapidfireicon");
                List<Texture2D> cannonIcons = new List<Texture2D>()
            {
                LoadImg("life"),
                LoadImg("damageicon"),
                LoadImg("defenseupgrade"),
                LoadImg("accuracyicon"),
                LoadImg("reloadicon"),
                LoadImg("speedicon"),
                LoadImg("powerupgrade"),
                rapidFireIcon,
            };

                Dictionary<GiftType, int> giftCosts = new Dictionary<GiftType, int>();
                for (int i = 0; i < Enum.GetNames(typeof(GiftType)).Count(); i++)
                {
                    giftCosts.Add((GiftType)i, GiftContents.MaxCoinsForType((GiftType)i));
                }

                Texture2D crossImg = LoadImg("cross");
                rightArrowImg = LoadImg("rightarrow");
                menu = new Menu(GraphicsDevice, WINDOW_WIDTH, WINDOW_HEIGHT, LoadImg("onecannononearmy"),
                    bigFont, mediumFont, smallFont, Content, this.SignIn, this.DeleteUser, alienImg, alienEyeImg,
                    LoadImg("trashcan"), rightArrowImg, projImgs, items, itemCosts, new BuyItem(Buy),
                    shopProjs, this.Upgrade, cannonStats, cannonIcons, checkImg, new System.Action(SkipStory),
                    LoadImg("life"), cannons, cannonCosts, this.BuyCannon, this.SelectCannon,
                    LoadImg("languageicon"), LoadImg("controlicon"),
                    LoadImg("gear"), new Action<Material>(MaterialSell), new Action<ProjectileType>(ProjSell),
                    new Action<Badge>(BadgeSell), LoadImg("graphicsicon"), LoadImg("profileicon"),
                    LoadImg("soundicon"), giftCosts, this.BuyGift,
                    crossImg, LoadImg("subtitleicon"), LoadImg("cannonicon"), LoadImg("scroll"), LoadImg("coinicon"),

                    new System.Action(Start), new System.Action(Quit), new System.Action(Resume), new System.Action(MainMenu), new System.Action(GoBack),
                    new CreateUser(CreateUser), new System.Action(CreateNewUserClicked), new System.Action(AttemptToOpenMissions),
                    new System.Action(GoToAchievements), new System.Action(Stats), new System.Action(Shop), new System.Action(Settings),
                    new System.Action(OpenOrganize), new System.Action(OpenCrafting), new System.Action(OpenUpgrade), new Action<int>(Play),
                    new System.Action(BuyLife), new System.Action(OpenLang), new Action<Languages>(ChangeLang),
                    new System.Action(OpenControls), new System.Action(OpenUserSettings), new System.Action(OpenGifts),
                    new System.Action(OpenCredits));
                menu.AddOnCraftHandler(OnCraft);
                menu.AddReplayTutorialHandler(ReplayTutorial);
                menu.AddSettingsSubmitHandler(UpdateUser);
                menu.AddClaimGiftHandler(ClaimGift);

                game = new MissionPlayable(GraphicsDevice, bigFont, mediumFont, smallFont, WINDOW_WIDTH, WINDOW_HEIGHT,
                    LoadImg("sweep"), LoadImg("lasercannon"), alienImg, alienEyeImg,
                    LoadImg("life"), rapidFireIcon, crossImg);
                game.AddOnAlienHitHandler(OnAlienHit);
                game.AddOnLaunchHandler(menu.RemoveProj);
                game.AddMissionOverHandler(MissionDone);
                game.AddOnBadgeCollectHandler(menu.AddBadge);
                game.AddOnMaterialCollectHandler(menu.AddMaterial);
                game.AddOnLaunchHandler(OnProjFire);
                game.AddSweepFinishedHandler(SweepFinished);
                game.AddProjRemovedHandler(TutorialProjRemoved);

                Utilities.OnExplosion = game.HandleExplosion;

                menu.AddChangeLivesHandler(game.ChangeLives);

                //for (int i = 0; i < 50; i++)
                //{
                //    //projectiles.Add(new Laser(0, 100, 15, 50));
                //    projectiles.Add(new Rock(0, 100, 30, 30));
                //    //projectiles.Add(new Lightning(0, 100, 30, 55));
                //}

                //playerInterface = new PlayerInterface(WINDOW_WIDTH, WINDOW_HEIGHT, LoadImg("sweep"),
                //    GraphicsDevice, bigFont, mediumFont, Cannon.DEFAULTS.MaxHealth);

                //cannon = new Cannon(90, 180, WINDOW_WIDTH, WINDOW_HEIGHT - PlayerInterface.BOTTOM_HEIGHT,
                //    GraphicsDevice, playerInterface.ChangePrimaryProj);
                //cannon.AddOnLaunchHandler(new Action<ProjectileType>(OnProjFire));
                //cannon.AddOnLaunchHandler(menu.OnFire);

                mouse = new MouseCursor(LoadImg("mousecursor"), LoadImg("clickmouse"),
                    LoadImg("typingcursor"));

                List<Texture2D> gameImgs = new List<Texture2D>()
            {
                LoadImg("mazesnake 1"),
                LoadImg("ocoa 1"),
                LoadImg("ocoa 3"),
                LoadImg("mazesnake 2"),
                LoadImg("ocoa 2"),
                LoadImg("mazesnake 8"),
                LoadImg("website 2"),
                LoadImg("ocoa 8"),
                LoadImg("mazesnake 7"),
                LoadImg("ocoa 6"),
                LoadImg("ocoa 10"),
                LoadImg("mazesnake 6"),
                LoadImg("ocoa 5"),
                LoadImg("website 1"),
                LoadImg("ocoa 11"),
                LoadImg("ocoa 4"),
                LoadImg("mazesnake 4"),
                LoadImg("ocoa 9"),
                LoadImg("ocoa 5"),
                LoadImg("website 3"),
            };

                doorImg = LoadImg("closegate");
                leftDoorRect = new Rectangle(WINDOW_WIDTH / 2 * -1, 0, WINDOW_WIDTH / 2, WINDOW_HEIGHT);
                rightDoorRect = new Rectangle(WINDOW_WIDTH, 0, WINDOW_WIDTH / 2, WINDOW_HEIGHT);

                brightnessFilterRect = new Rectangle(0, 0, WINDOW_WIDTH, WINDOW_HEIGHT);

                //laserCannon = new AlienLaserCannon(LoadImg("lasercannon"), 0, 0, LASER_CANNON_WIDTH,
                //    LASER_CANNON_HEIGHT, smallFont);
                //laserCannon.AddOnDeathHandler(MissionCompleted);
                //laserCannon.AddLaserHitHandler(MissionFailed);

                lvlComplete = new LevelCompletePopup(GraphicsDevice, mediumFont, LVLCOMPLETE_WIDTH, LVLCOMPLETE_HEIGHT,
                    LevelSelect, NextLevel, ReplayLevel,
                    LoadImg("circlelevelselect"), LoadImg("circlenext"),
                    LoadImg("circleredo"));

                tutorial = new Tutorial(bigFont, mediumFont, smallFont, LoadImg("tutorialalien"),
                    LoadImg("tutorialalieneyes"), WINDOW_WIDTH, WINDOW_HEIGHT, GraphicsDevice);
                tutorial.AddModeChangedHandler(SetAllowedAction);
                tutorial.AddOnCompleteHandler(TutorialComplete);
                tutorial.AddOnSkipHandler(TutorialComplete);

                intro = new StudioIntro(gameImgs, LoadImg("duoplus software transparent"),
                    LoadImg("duoplus software"), WINDOW_WIDTH, WINDOW_HEIGHT, mediumFont);
                intro.AddOnFinishHandler(AttemptStory);
                intro.AddOnFinishHandler(IntroFinished);

                langSelectPopup = new LangSelectorPopup(WINDOW_WIDTH, WINDOW_HEIGHT, mediumFont, GraphicsDevice);
                langSelectPopup.AddLanguageSetHandler(InitLangSet);
                if (!(new UniversalSettings().ViewedFullIntro))
                {
                    //langSelectPopup.Show();
                }
                else
                {
                    // If we've already initialized the language, we don't need to select it again; just play intro
                    intro.Play();
                }
            }
            catch (FileNotFoundException e)
            {
                if (e.FileName == Mission.FILE_PATH)
                {
                    Error.Handle(new FileNotFoundException("The missions file is missing. Please make sure you haven't deleted it. "
                        + "If you have, the download is available at purasu.itch.io/onecannononearmy.", e));
                }
                else
                {
                    Error.Handle(e);
                }
            }
            catch (Exception e)
            {
                Error.Handle(e);
            }
        }

        /// <summary>
        /// UnloadContent will be called once per game and is the place to unload
        /// game-specific content.
        /// </summary>
        protected override void UnloadContent()
        {
            // TODO: Unload any non ContentManager content here
        }

        /// <summary>
        /// Allows the game to run logic such as updating the world,
        /// checking for collisions, gathering input, and playing audio.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Update(GameTime gameTime)
        {
            currentKeyState = Keyboard.GetState();

            if (currentKeyState.IsKeyDown(Controls.FullscreenKey) && !prevKeyState.IsKeyDown(Controls.FullscreenKey))
            {
                graphics.ToggleFullScreen();
            }

            try
            {
                IsMouseVisible = !IsActive;
            }
            catch (NullReferenceException)
            {
                // For some reason we can get errors here. 
                // Should that occur, just close the game
                // Bug only happens when the game is exiting anyways,
                // but hasn't exited the game loop yet
                Close();
                return;
            }
            Sound.PlaySounds = IsActive || intro.Playing;

            Sound.Update(gameTime);

            if (!intro.Playing && !tutorial.Playing && !langSelectPopup.Active && IsActive)
            {
                Popup.Update();
                Notification.Update(gameTime);

                try
                {
                    UpdateMissionGame(gameTime);

                    if (gameState == GameState.Upgrade && !(ctrlW.Active || Popup.HasActivePopups))
                    {
                        if (player.CannonSettings.MaxHealth != game.MaxHealth)
                        {
                            float percent = game.Health / game.MaxHealth;
                            game.MaxHealth = player.CannonSettings.MaxHealth;
                            game.Health *= percent + 1.0f;
                        }
                    }

                    if (player != null)
                    {
                        foreach (Achievement ach in Achievements.AchievementsList)
                        {
                            if (GetAchMethod(ach).Invoke())
                            {
                                RewardAch(ach);
                            }
                        }

                        Color reqBrightness = Utilities.GetColorForBrightness(gameState == GameState.UserSettings ? menu.GraphicsSettings.Brightness :
                            player.GraphicsSettings.Brightness);
                        if (brightness != reqBrightness)
                        {
                            brightness = reqBrightness;
                        }

                        if (DateTime.Compare(DateTime.Now, player.LastReceivedGift.AddHours(GameInfo.HOURS_UNTIL_NEXT_GIFT)) >= 0)
                        {
                            // It is currently the time or past the time when the user should receive their next free gift
                            GiftType newGift = Gift.GetRandomGift();
                            player.Gifts.Add(newGift);
                            player.LastReceivedGift = DateTime.Now;
                            menu.AddGift(newGift);
                            Notification.Show(string.Format(Language.Translate("You've received a free {0} gift!"),
                                Language.Translate(newGift.ToString().ToLower())));
                        }

                        VolumeSettings vs = gameState == GameState.UserSettings ? menu.VolumeSettings :
                            player.VolumeSettings;
                        if (vs.Muted)
                        {
                            Sound.SoundEffectsVolume = Sound.MusicVolume = Sound.SystemVolume = 0;
                        }
                        else
                        {
                            Sound.SystemVolume = vs.SystemVolume / 100.0f;
                            Sound.MusicVolume = vs.MusicVolume / 100.0f;
                            Sound.SoundEffectsVolume = vs.SfxVolume / 100.0f;
                        }
                    }

                    if (!Popup.HasActivePopups && !ctrlW.Active && preLvlPopup == null)
                    {
                        menu.Update(gameState, gameTime, player, game.Projectiles, false, true);
                    }
                    else if (preLvlPopup != null)
                    {
                        preLvlPopup.Update();
                    }

                    if (gameState == GameState.Playing || gameState == GameState.Paused)
                    {
                        UpdatePauseDetection();
                    }

                    if (Controls.CtrlPressed(currentKeyState) && currentKeyState.IsKeyDown(Keys.W) &&
                        uSettings.ShowCtrlWPopup && !ctrlW.Active)
                    {
                        ctrlW.ShowPopup(
                            Language.Translate("The shortcut Ctrl+W exits the game. " +
                            "Are you sure you want to do this?"),
                            true, WINDOW_WIDTH / 2 - (ctrlW.Width / 2), WINDOW_HEIGHT / 2 - (ctrlW.Height / 2));
                    }
                    if (ctrlW.Active)
                    {
                        ctrlW.Update();
                    }

                    if (lvlComplete.Active)
                    {
                        lvlComplete.Update();
                    }

                    if (gameState == GameState.Paused || Popup.HasActivePopups || ctrlW.Active)
                    {
                        Sound.PauseCurrentSong();
                    }
                }
                catch (Exception e)
                {
                    Error.Handle(e);
                }

                UpdateDoors();
                UpdateMouse();
            }
            else if (langSelectPopup.Active && IsActive)
            {
                langSelectPopup.Update();
                UpdateMouse();
            }
            else if (tutorial.Playing && IsActive)
            {
                Sound.CheckAndPlaySong(Music.TutorialMusic);
                tutorial.Update(menu, gameTime);
                menu.Update(gameState, gameTime, player, game.Projectiles,
                    actionAllowed.Action == TutorialTask.BuyMaterial ? true : (bool?)null, false);
                UpdateMouse();
                UpdateDoors();
                if (gameState == GameState.Playing || gameState == GameState.Paused)
                {
                    UpdatePauseDetection();
                }
                if (actionAllowed.Action == TutorialTask.All || actionAllowed.Action == TutorialTask.ShootRock ||
                    actionAllowed.Action == TutorialTask.Sweep)
                {
                    UpdateMissionGame(gameTime);
                }
            }
            else if (intro.Playing)
            {
                intro.Update(gameTime);
            }

            prevKeyState = currentKeyState;

            base.Update(gameTime);
        }

        /// <summary>
        /// This is called when the game should draw itself.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(GameInfo.Planet.SkyColor);

            spriteBatch.Begin();

            try
            {
                if (!intro.Playing && !langSelectPopup.Active)
                {
                    if (gameState != GameState.Playing && gameState != GameState.Paused)
                    {
                        spriteBatch.Draw(bgImg, bgRect, Color.White);
                    }

                    if (gameState == GameState.Playing || gameState == GameState.Paused)
                    {
                        game.Draw(spriteBatch, player, gameState);

                        //foreach (Alien a in aliens)
                        //{
                        //    a.Draw(spriteBatch);
                        //    foreach (Projectile p in alienProjectiles)
                        //    {
                        //        p.Draw(spriteBatch, true);
                        //    }
                        //}
                        //cannon.Draw(spriteBatch, ref projectiles);
                        //if (laserCannon.Active)
                        //{
                        //    laserCannon.Draw(spriteBatch);
                        //}

                        //playerInterface.Draw(spriteBatch, player);

                        //for (int i = 0; i < alienItems.Count; i++)
                        //{
                        //    alienItems[i].Draw(spriteBatch);
                        //}
                    }
                    if (gameState == GameState.Shop || gameState == GameState.Upgrade ||
                        gameState == GameState.MainMenu || gameState == GameState.Organize)
                    {
                        game.DrawCoinDisplay(spriteBatch, player);
                    }
                    if (gameState == GameState.MainMenu)
                    {
                        game.DrawLives(spriteBatch, player, false);
                    }

                    if (gameState == GameState.StartMenu)
                    {
                        spriteBatch.DrawString(smallFont, versionString, versionPos, Color.Black);
                    }

                    if (gameState == GameState.Playing || gameState == GameState.Paused)
                    {
                        LevelInfo.Draw(spriteBatch);
                    }

                    menu.Draw(gameState, spriteBatch, player);

                    if (gameState == GameState.Paused)
                    {
                        spriteBatch.DrawString(mediumFont, Language.Translate(game.Mission.Name),
                            new Vector2(WINDOW_WIDTH / 2 - mediumFont.MeasureString(Language.Translate(game.Mission.Name)).X / 2,
                            Utilities.MENU_Y_OFFSET / 2),
                            Color.Black);
                        string name = "";
                        if (game.Mission.StateCountry == "")
                        {
                            name = game.Mission.City;
                        }
                        else
                        {
                            name = game.Mission.City + ", " + game.Mission.StateCountry;
                        }
                        spriteBatch.DrawString(mediumFont, name,
                            new Vector2(WINDOW_WIDTH / 2 - mediumFont.MeasureString(game.Mission.City + ", " + game.Mission.StateCountry).X / 2,
                            Utilities.MENU_Y_OFFSET / 2 + mediumFont.MeasureString("A").Y * 2),
                            Color.Black);
                    }

                    preLvlPopup?.Draw(spriteBatch);

                    if (drawDoors)
                    {
                        spriteBatch.Draw(doorImg, leftDoorRect, Color.White);
                        spriteBatch.Draw(doorImg, rightDoorRect, null, Color.White, 0.0f, new Vector2(),
                            SpriteEffects.FlipHorizontally, 1.0f);
                    }

                    if (lvlComplete.Active)
                    {
                        lvlComplete.Draw(spriteBatch);
                    }

                    Notification.Draw(spriteBatch);

                    // Draw popups last; the only thing after them is the mouse itself
                    Popup.Draw(spriteBatch);

                    if (!tutorial.Playing)
                    {
                        if (ctrlW.Active)
                        {
                            ctrlW.Draw(spriteBatch);
                        }
                    }
                    else
                    {
                        tutorial.Draw(spriteBatch);
                    }

                    mouse.Draw(spriteBatch);
                }
                else if (langSelectPopup.Active)
                {
                    langSelectPopup.Draw(spriteBatch);
                    mouse.Draw(spriteBatch);
                }
                else
                {
                    intro.Draw(spriteBatch);
                }

                Sound.Draw(spriteBatch);

                // Apply brightness last, over everything else
                if (player != null)
                {
                    spriteBatch.Draw(Utilities.RectImage, brightnessFilterRect, brightness);
                }
            }
            catch (Exception e)
            {
                Error.Handle(e);
            }

            spriteBatch.End();

            base.Draw(gameTime);
        }

        private void RestartProgram()
        {
            Restart = true;
            Close(); // The Quit() method shows a popup asking us to confirm exit
                     // We don't want that, so call Exit() (OnExiting will still execute)
        }

        #region Delegates

        private void Start()
        {
            CloseAnimation(GameState.PickUser);
        }
        private void Quit()
        {
            if (gameState == GameState.Paused)
            {
                // The player is trying to leave the game
                Popup.Show(GetTextForQuitLevel(), true, new System.Action(() => RemoveLifeAnd(this.Close)), false);
            }
            else
            {
                Popup.Show(Language.Translate("Are you sure you want to quit?"), true, Close, false);
            }
        }
        private void MainMenu()
        {
            if (gameState == GameState.Paused)
            {
                // The player is trying to leave the game
                Popup.Show(GetTextForQuitLevel(),
                    true, new System.Action(() => RemoveLifeAnd(new System.Action(() => CloseAnimation(GameState.MainMenu)),
                    new System.Action(() => EnableOrDisableCloseButton(true)))),
                    false);
            }
            else
            {
                CloseAnimation(GameState.MainMenu);
            }
        }
        private string GetTextForQuitLevel()
        {
            string message = Language.Translate("Are you sure you want to quit?");
            if (game.RemoveLifeOnQuit)
            {
                message += " " + Language.Translate("You will be charged with one life.");
            }
            else
            {
                message += " " + Language.Translate("You will not use a life as you have played for less than 10 seconds.");
            }   
            return message;
        }
        private void Resume()
        {
            if ((actionAllowed.Action == TutorialTask.GameState && actionAllowed.Params == (int)GameState.Playing) ||
                !tutorial.Playing)
            {
                gameState = GameState.Playing;

                if (tutorial.Playing)
                {
                    tutorial.ActionCompleted(menu);
                }
            }
        }
        private void Story()
        {
            gameState = GameState.Story;
        }
        private void SkipStory()
        {
            UniversalSettings settings = new UniversalSettings();
            GameState g = settings.ViewedStartingStory ? GameState.MainMenu : GameState.StartMenu;
            settings.ViewedStartingStory = true;
            settings.Save();
            CloseAnimation(g);
            AttemptTutorialPlay();
        }
        private void GoBack()
        {
            switch (gameState)
            {
                case GameState.MainMenu:
                    CloseAnimation(GameState.PickUser);
                    if (player != null)
                    {
                        SaveUser();
                        player = null;
                    }
                    break;

                case GameState.Achievements:
                case GameState.Stats:
                case GameState.UserSettings:
                case GameState.Organize:
                case GameState.Crafting:
                case GameState.Upgrade:
                case GameState.SelectMission:
                case GameState.Gifts:
                case GameState.Shop:
                    CloseAnimation(GameState.MainMenu);
                    break;

                case GameState.NewUser:
                    if (!tutorial.Playing)
                    {
                        // Fixes error in tutorial, where user can exit new user mode
                        // without creating a new user. This prevents them from continuing
                        // as they don't have an account to sign in to
                        CloseAnimation(GameState.PickUser);
                    }
                    break;

                case GameState.PickUser:
                case GameState.Language:
                case GameState.ChangeControls:
                    CloseAnimation(GameState.StartMenu);
                    break;

                case GameState.Credits:
                    Sound.StopAllMusic();
                    CloseAnimation(GameState.StartMenu);
                    break;
            }
        }
        private User CreateUser(string username, Color avatarColor, Texture2D projImg)
        {
            User newUser = new User(username, avatarColor, projImg);
            newUser.Coins = GameInfo.STARTING_USER_COINS;
            newUser.Lives = GameInfo.MAX_LIVES;
            newUser.Cannons.Add(CannonSettings.NormalCannon);
            newUser.GraphicsSettings.Brightness = 0.5f;
            newUser.LastReceivedGift = DateTime.Now.AddHours(GameInfo.HOURS_UNTIL_NEXT_GIFT * -1);
            for (int i = 0; i < 50; i++)
            {
                newUser.ProjectileInventory.Add(ProjectileType.Rock);
            }
            newUser.Hotbar[0] = ProjectileType.Rock;
            newUser.SaveUser();
            CloseAnimation(GameState.PickUser);
            return newUser;
        }
        private void CreateNewUserClicked()
        {
            CloseAnimation(GameState.NewUser);
        }
        private void AttemptToOpenMissions()
        {
            if (player.Lives > 0)
            {
                CloseAnimation(GameState.SelectMission);
            }
            else
            { //
                Popup.Show(Language.Translate("You cannot play until you have at least one life."), false, false);
            }
        }
        private void Play(int id)
        {
            preLvlPopup = new PreLvlPopup(smallFont, mediumFont, player.Hotbar, GetHotbarCounts(),
                WINDOW_WIDTH, WINDOW_HEIGHT, () => ContinueToLevel(id), CancelPlayingLevel, GraphicsDevice, 
                player.CannonSettings, Mission.Missions.Where(x => x.Id == id).FirstOrDefault(), player);
            
            //int req = Mission.DamageForLevel(id); // Required damage
            //int have = CalculateDamageAvailable(player); // ~ Available damage
            //if (have >= req)
            //{
            //    ContinueToLevel(id);
            //}
            //else
            //{
            //    Popup.Show(string.Format(Language.Translate(
            //        "This level requires about {0} damage to beat and you only have about {1} worth of\ndamage in your inventory. "
            //        + "Continue?"),
            //        req, have),
            //        true, new System.Action(() => ContinueToLevel(id)), false);
            //}
        }
        private void ContinueToLevel(int id)
        {
            preLvlPopup = null;

            EnableOrDisableCloseButton(false);

            CloseAnimation(GameState.Playing);
            if (!started)
            {
                started = true;
            }
            game.LoadMission(id, player);

            // No exiting the tutorial
            menu.AllowPauseExiting = !(id == 0 && player.CurrentMission == 0);
        }
        private void CancelPlayingLevel()
        {
            preLvlPopup = null;
        }
        private void GoToAchievements()
        {
            CloseAnimation(GameState.Achievements);
        }
        private void Stats()
        {
            CloseAnimation(GameState.Stats);
        }
        private void Shop()
        {
            CloseAnimation(GameState.Shop);
        }
        private void Settings()
        {
            CloseAnimation(GameState.UserSettings);
        }
        private void OpenOrganize()
        {
            menu.OrganizeOpened(player);
            CloseAnimation(GameState.Organize);
        }
        private void OpenCrafting()
        {
            CloseAnimation(GameState.Crafting);
        }
        private void OpenUpgrade()
        {
            CloseAnimation(GameState.Upgrade);
        }
        private void OpenLang()
        {
            CloseAnimation(GameState.Language);
        }
        private void OpenControls()
        {
            CloseAnimation(GameState.ChangeControls);
        }
        private void OpenUserSettings()
        {
            CloseAnimation(GameState.UserSettings);
        }
        private void OpenGifts()
        {
            CloseAnimation(GameState.Gifts);
        }
        private void OpenCredits()
        {
            CloseAnimation(GameState.Credits);
        }

        private void ChangeLang(Languages lang)
        {
            UniversalSettings u = new UniversalSettings();
            u.Language = (int)lang;
            u.Save();
            GameInfo.Language = lang;
            menu.LangChanged(lang);
        }

        private void Upgrade(CannonStats stat, int value, int cost)
        {
            if (player.Coins >= cost)
            {
                value += player.CannonSettings.GetValueOfStat(stat);
                player.SetValueOfStat(stat, value);
                game.ChangeCannonSettings(player.CannonSettings);
                player.Coins -= cost;
                player.CoinsSpent += cost;
                player.Purchases++;
                if (player.CurrentQuest.GoalType == QuestGoalType.SpendCoins)
                {
                    IncreaseQuestProgress(cost);
                }
                Sound.PlaySound(Sounds.Upgrade);
            }
        }

        private void BuyLife()
        {
            Popup.Show(Language.Translate("Buy one life for ") +
                string.Format("{0}c?", (player.TimeOfNextLife - DateTime.Now).Minutes * GameInfo.DEATH_COST_PER_MIN),
                true, LifeBought, false);
        }
        private void LifeBought()
        {
            int cost = (player.TimeOfNextLife - DateTime.Now).Minutes * GameInfo.DEATH_COST_PER_MIN;
            if (cost <= 0)
            {
                return;
            }
            if (player.Coins >= cost)
            {
                player.Coins -= cost;
                player.CoinsSpent += cost;
                player.Purchases++;
                if (player.CurrentQuest.GoalType == QuestGoalType.SpendCoins)
                {
                    IncreaseQuestProgress(cost);
                }
                player.AddLives(1);
                Sound.PlaySound(Sounds.LifeEarned);
                game.ChangeLives(player.Lives);
                player.SetTimeOfNextLife(DateTime.Now.AddMinutes(GameInfo.MINUTES_UNTIL_NEXT_LIFE));
                player.SaveUser();
            }
            else
            {
                Popup.Show(Language.Translate("You do not have enough coins."), false, false);
            }
        }

        private void UpdateUser(string name, float r, float g, float b, string assetName, GraphicsSettings graphics,
            VolumeSettings soundSettings)
        {
            if (player != null)
            {
                player.Username = name;
                player.AvatarR = (int)r;
                player.AvatarG = (int)g;
                player.AvatarB = (int)b;
                player.ProjectileAsset = assetName;
                player.GraphicsSettings = graphics;
                player.VolumeSettings = soundSettings;
                player.SaveUser();
            }
            GoBack();
        }

        private void ReplayTutorial()
        {
            UniversalSettings settings = new UniversalSettings();
            settings.ViewedStartingStory = false;
            settings.PlayedTutorial = false;
            settings.Save();
            RestartProgram();
        }

        #endregion

        #region User Delegates & Methods

        private void SignIn(User user)
        {
            if (user.CurrentMission == 0 && new UniversalSettings().PlayedTutorial)
            {
                user.CurrentMission++;
            }

            player = user;
            MainMenu();

            game.Initialize(user);

            menu.OnSignIn(player);

            //game.ChangeCannonSettings(player.CannonSettings);
            //game.MaxHealth = player.CannonSettings.MaxHealth;
            //game.Health = game.MaxHealth;

            //projectiles.Clear();
            //foreach (ProjectileType item in player.ProjectileInventory)
            //{
            //    AddProj(item);
            //}
        }
        private void DeleteUser(User user)
        {
            User.DeleteUser(user);
        }

        #endregion

        #region Achievement Methods

        private void RewardAch(Achievement ach)
        {
            Notification.Show(Language.Translate("You've earned the \nachievement \"") + ach.Name + "\"! +" + ach.Coins + "c");

            player.Coins += ach.Coins;
            player.CoinsCollected += ach.Coins;
            player.AchievementsCompleted.Add(ach);
            if (player.CurrentQuest.GoalType == QuestGoalType.ObtainCoins)
            {
                IncreaseQuestProgress(ach.Coins);
            }
        }

        private bool FirstAlienKillEarned()
        {
            if (player != null)
            {
                foreach (Achievement ach in player.AchievementsCompleted)
                {
                    if (ach.Id == 0)
                    {
                        return false;
                    }
                }
                if (player.AliensKilled >= 1)
                {
                    return true;
                }
            }
            return false;
        }
        private bool SecondAlienKillEarned()
        {
            if (player != null)
            {
                foreach (Achievement ach in player.AchievementsCompleted)
                {
                    if (ach.Id == 1)
                    {
                        return false;
                    }
                }
                if (player.AliensKilled >= 100)
                {
                    return true;
                }
            }
            return false;
        }
        private bool ThirdAlienKillEarned()
        {
            if (player != null)
            {
                foreach (Achievement ach in player.AchievementsCompleted)
                {
                    if (ach.Id == 2)
                    {
                        return false;
                    }
                }
                if (player.AliensKilled >= 200)
                {
                    return true;
                }
            }
            return false;
        }
        private bool FourthAlienKillEarned()
        {
            if (player != null)
            {
                foreach (Achievement ach in player.AchievementsCompleted)
                {
                    if (ach.Id == 3)
                    {
                        return false;
                    }
                }
                if (player.AliensKilled >= 500)
                {
                    return true;
                }
            }
            return false;
        }
        private bool FifthAlienKillEarned()
        {
            if (player != null)
            {
                foreach (Achievement ach in player.AchievementsCompleted)
                {
                    if (ach.Id == 4)
                    {
                        return false;
                    }
                }
                if (player.AliensKilled >= 1000)
                {
                    return true;
                }
            }
            return false;
        }

        private bool FirstProjFireEarned()
        {
            if (player != null)
            {
                foreach (Achievement ach in player.AchievementsCompleted)
                {
                    if (ach.Id == 5)
                    {
                        return false;
                    }
                }
                if (player.ProjectilesFired >= 200)
                {
                    return true;
                }
            }
            return false;
        }
        private bool SecondProjFireEarned()
        {
            if (player != null)
            {
                foreach (Achievement ach in player.AchievementsCompleted)
                {
                    if (ach.Id == 6)
                    {
                        return false;
                    }
                }
                if (player.ProjectilesFired >= 500)
                {
                    return true;
                }
            }
            return false;
        }
        private bool ThirdProjFireEarned()
        {
            if (player != null)
            {
                foreach (Achievement ach in player.AchievementsCompleted)
                {
                    if (ach.Id == 7)
                    {
                        return false;
                    }
                }
                if (player.ProjectilesFired >= 1000)
                {
                    return true;
                }
            }
            return false;
        }

        private bool DefeatMalosEarned()
        {
            if (player != null)
            {
                foreach (Achievement ach in player.AchievementsCompleted)
                {
                    if (ach.Id == 8)
                    {
                        return false;
                    }
                }
                if (player.DefeatedMalos)
                {
                    return true;
                }
            }
            return false;
        }

        private bool FirstSpendingEarned()
        {
            if (player != null)
            {
                foreach (Achievement ach in player.AchievementsCompleted)
                {
                    if (ach.Id == 9)
                    {
                        return false;
                    }
                }
                if (player.CoinsSpent >= 1000)
                {
                    return true;
                }
            }
            return false;
        }
        private bool SecondSpendingEarned()
        {
            if (player != null)
            {
                foreach (Achievement ach in player.AchievementsCompleted)
                {
                    if (ach.Id == 10)
                    {
                        return false;
                    }
                }
                if (player.CoinsSpent >= 2500)
                {
                    return true;
                }
            }
            return false;
        }
        private bool ThirdSpendingEarned()
        {
            if (player != null)
            {
                foreach (Achievement ach in player.AchievementsCompleted)
                {
                    if (ach.Id == 11)
                    {
                        return false;
                    }
                }
                if (player.CoinsSpent >= 5000)
                {
                    return true;
                }
            }
            return false;
        }
        private bool FourthSpendingEarned()
        {
            if (player != null)
            {
                foreach (Achievement ach in player.AchievementsCompleted)
                {
                    if (ach.Id == 12)
                    {
                        return false;
                    }
                }
                if (player.CoinsSpent >= 10000)
                {
                    return true;
                }
            }
            return false;
        }

        private bool FirstCraftingEarned()
        {
            if (player != null)
            {
                foreach (Achievement ach in player.AchievementsCompleted)
                {
                    if (ach.Id == 13)
                    {
                        return false;
                    }
                }
                if (player.ItemsCrafted >= 100)
                {
                    return true;
                }
            }
            return false;
        }
        private bool SecondCraftingEarned()
        {
            if (player != null)
            {
                foreach (Achievement ach in player.AchievementsCompleted)
                {
                    if (ach.Id == 14)
                    {
                        return false;
                    }
                }
                if (player.ItemsCrafted >= 500)
                {
                    return true;
                }
            }
            return false;
        }
        private bool ThirdCraftingEarned()
        {
            if (player != null)
            {
                foreach (Achievement ach in player.AchievementsCompleted)
                {
                    if (ach.Id == 15)
                    {
                        return false;
                    }
                }
                if (player.ItemsCrafted >= 1000)
                {
                    return true;
                }
            }
            return false;
        }
        private bool FourthCraftingEarned()
        {
            if (player != null)
            {
                foreach (Achievement ach in player.AchievementsCompleted)
                {
                    if (ach.Id == 16)
                    {
                        return false;
                    }
                }
                if (player.ItemsCrafted >= 5000)
                {
                    return true;
                }
            }
            return false;
        }

        #endregion

        #region Unused

        //private void AddAlien(AlienType type)
        //{
        //    //Alien newAlien = CreateAlien(type.Type, type.HasShield);
        //    //newAlien.AddOnHitHandler(cannon.OnHit);
        //    //newAlien.AddOnHitHandler(new OnAlienHit(OnAlienHit));
        //    //newAlien.AddOnDeathHandler(new OnAlienHit(OnAlienDeath));
        //    //aliens.Add(newAlien);
        //}

        //private void MissionCompleted()
        //{
        //    string successMsg = "";
        //    if (mission.StateCountry == "")
        //    {
        //        if (mission.Id == 0)
        //        {
        //            // Tutorial
        //            successMsg = LanguageTranslator.Translate("You've completed the tutorial") + "!";
        //        }
        //        else if (mission.Id == 25)
        //        {
        //            // Lithios
        //            successMsg = LanguageTranslator.Translate("You've saved the entire planet! The world is in your debt.");
        //        }
        //        else if (mission.Id == 21)
        //        {
        //            // Space
        //            successMsg = LanguageTranslator.Translate("You've reached the Lambda Romana star system! Onward, to Nantak!");
        //        }
        //        else if (mission.Id == 22)
        //        {
        //            // Nantak
        //            successMsg = LanguageTranslator.Translate("One down, two to go. Onward, to Carinus!");
        //        }
        //        else if (mission.Id == 23)
        //        {
        //            // Carinus
        //            successMsg = LanguageTranslator.Translate("Only one laser left. Time to go to Mikara.");
        //        }
        //        else if (mission.Id == 24)
        //        {
        //            // Mikara
        //            successMsg = LanguageTranslator.Translate("Good job destroying those lasers! Now it's time to battle Malos himself. " + 
        //                "To Lithios!");
        //        }
        //    }
        //    else
        //    {
        //        successMsg = LanguageTranslator.Translate("Excellent job! You've saved") + " " + mission.City + ", " +
        //            mission.StateCountry + "!";
        //    }
        //    Popup.Show(successMsg, false, false);
        //    player.CurrentMission++;
        //    OpenMissions();
        //    for (int i = 0; i < alienItems.Count; i++)
        //    {
        //        CollectItem(alienItems[i]);
        //    }
        //    if (mission.DestroyLaser)
        //    {
        //        laserCannon.Reset();
        //    }
        //}
        //private void MissionFailed()
        //{
        //    CloseAnimation(GameState.SelectMission);
        //    Popup.Show(LanguageTranslator.Translate("You have died and have been charged with one life."),
        //        false, false);
        //    if (mission.DestroyLaser)
        //    {
        //        laserCannon.Reset();
        //    }
        //}

        //private void CollectItem(Item item)
        //{
        //    if (item is MaterialDrop)
        //    {
        //        MaterialDrop m = item as MaterialDrop;
        //        player.MaterialInventory.AddItem(m.Drop, 1);

        //        // We have collected our item, so now let's remove it
        //        alienItems.Remove(item);
        //    }
        //    else if (item is Coin)
        //    {
        //        Coin c = item as Coin;
        //        player.Coins += c.Worth;
        //        player.CoinsCollected += c.Worth;

        //        // We have collected our item, so now let's remove it
        //        alienItems.Remove(item);
        //    }
        //}

        #endregion

        #region Misc.

        private List<int> GetHotbarCounts()
        {
            List<int> counts = new List<int>();
            for (int i = 0; i < player.Hotbar.Count; i++)
            {
                counts.Add(player.ProjectileInventory.CountOf(player.Hotbar[i]));
            }
            return counts;
        }

        private void Close()
        {
            Sound.StopAllMusic();
            Sound.StopAllSounds();
            Exit();
        }

        private Texture2D LoadImg(string asset)
        {
            Texture2D img = null;
            try
            {
                img = Content.Load<Texture2D>(asset);
            }
            catch (ContentLoadException)
            {
                img = imgNotFound;
            }
            catch (Exception e)
            {
                Error.Handle(e);
            }
            return img;
        }

        private void EnableOrDisableCloseButton(bool enabled)
        {
            IntPtr hSystemMenu = GetSystemMenu(this.Window.Handle, false);
            EnableMenuItem(hSystemMenu, SC_CLOSE, (uint)(MF_ENABLED | (enabled ? MF_ENABLED : MF_GRAYED)));
        }

        private void RemoveLifeAnd(params System.Action[] actions)
        {
            if (player != null && game.RemoveLifeOnQuit)
            {
                player.AddLives(-1);
                player.TimeOfNextLife = DateTime.Now.AddMinutes(GameInfo.MINUTES_UNTIL_NEXT_LIFE);
            }
            for (int i = 0; i < actions.Count(); i++)
            {
                actions[i]?.Invoke();
            }
        }

        private void ClaimGift(Gift gift)
        {
            GiftContents contents = gift.Contents;
            player.Coins += contents.Coins;
            player.CoinsCollected += contents.Coins;
            if (player.CurrentQuest.GoalType == QuestGoalType.ObtainCoins)
            {
                IncreaseQuestProgress(contents.Coins);
            }

            player.ProjectileInventory.AddRange(contents.Projectiles);
            for (int i = 0; i < contents.Projectiles.Count; i++)
            {
                game.AddProjectile(contents.Projectiles[i]);
                menu.AddProj(contents.Projectiles[i]);
            }

            for (int i = 0; i < contents.Materials.Count; i++)
            {
                player.MaterialInventory.AddItem(contents.Materials[i], 1);
                menu.AddMaterial(contents.Materials[i]);
            }

            for (int i = 0; i < player.Gifts.Count; i++)
            {
                if (player.Gifts[i] == gift.Type)
                {
                    player.Gifts.RemoveAt(i);
                    break;
                }
            }
        }

        private void WhenMalosDefeated()
        {
            EnableOrDisableCloseButton(true);
            menu.ResetStory(1);
            player.DefeatedMalos = true;
            Story();
        }

        private BoolAction GetAchMethod(Achievement ach)
        {
            switch (ach.Id)
            {
                case 0:
                    return FirstAlienKillEarned;
                case 1:
                    return SecondAlienKillEarned;
                case 2:
                    return ThirdAlienKillEarned;
                case 3:
                    return FourthAlienKillEarned;
                case 4:
                    return FifthAlienKillEarned;

                case 5:
                    return FirstProjFireEarned;
                case 6:
                    return SecondProjFireEarned;
                case 7:
                    return ThirdProjFireEarned;

                case 8:
                    return DefeatMalosEarned;

                case 9:
                    return FirstSpendingEarned;
                case 10:
                    return SecondSpendingEarned;
                case 11:
                    return ThirdSpendingEarned;
                case 12:
                    return FourthSpendingEarned;

                case 13:
                    return FirstCraftingEarned;
                case 14:
                    return SecondCraftingEarned;
                case 15:
                    return ThirdCraftingEarned;
                case 16:
                    return FourthCraftingEarned;

                default:
                    return null;
            }
        }

        //private void OnAlienDeath(Alien alien)
        //{
        //    alienItems.AddRange(alien.Drops);
        //}

        private void OnAlienHit(Alien alien)
        {
            if (player != null)
            {
                player.AliensHit++;
            }
        }
        private void OnAlienDeath(Alien alien)
        {
            if (player != null)
            {
                if (player.CurrentQuest.GoalType == QuestGoalType.KillAliens && 
                    player.CurrentQuest.TypeId == (int)alien.Type.GetBasicType())
                {
                    IncreaseQuestProgress(1);
                }
            }
        }

        private void IncreaseQuestProgress(int progress)
        {
            player.QuestProgress += progress;
            if (player.QuestProgress >= player.CurrentQuest.GoalNumber)
            {
                Notification.Show("You have completed your quest! (" + player.CurrentQuest.ToString() + ")");
                player.Coins += player.CurrentQuest.RewardCoins;
                player.CoinsCollected += player.CurrentQuest.RewardCoins;
                player.QuestProgress = 0;
                player.CurrentQuest = Quest.Random(player);
            }
        }

        private void OnProjFire(ProjectileType type)
        {
            if (player != null)
            {
                player.ProjectilesFired++;
                if (player.CurrentQuest.GoalType == QuestGoalType.FireProjectiles &&
                    player.CurrentQuest.TypeId == (int)type)
                {
                    IncreaseQuestProgress(1);
                }
            }
        }

        private void CloseAnimation(GameState state)
        {
            if ((actionAllowed.Action == TutorialTask.GameState && actionAllowed.Params == (int)state) || !tutorial.Playing)
            {
                gameStateToBe = state;
                openAfterClose = true;
                closing = true;
                drawDoors = true;
                Sound.PlaySound(Sounds.Close);

                if (tutorial.Playing)
                {
                    tutorial.ActionCompleted(menu);
                }

                if (state == GameState.PickUser)
                {
                    menu.UserChanged();
                }

                if (state == GameState.MainMenu)
                {
                    player.SaveUser();
                }
            }
        }
        private void CloseDoors()
        {
            openAfterClose = false;
            closing = true;
            drawDoors = true;
            Sound.PlaySound(Sounds.Close);
        }
        private void OpenDoors()
        {
            opening = true;
            drawDoors = true;
            Sound.PlaySound(Sounds.Open);
        }

        private void MissionDone(bool success, int id)
        {
            EnableOrDisableCloseButton(true);
            menu.AllowPauseExiting = true;

            string msg = "";
            string defaultMsg = Language.Translate("Excellent job! \nYou've saved") + " " + game.Mission.City + ", " +
                        game.Mission.StateCountry;
            if (success)
            {
                if (game.Mission.StateCountry == "")
                {
                    if (game.Mission.Id == 0)
                    {
                        // Tutorial
                        msg = Language.Translate("You've completed the tutorial!");
                    }
                    else if (game.Mission.Id == 2)
                    {
                        msg = defaultMsg + Language.Translate(" ... again!");
                    }
                    else if (game.Mission.Id == 3)
                    {
                        msg = defaultMsg + Language.Translate(" ... for the third time!");
                    }
                    else if (game.Mission.Id == GameInfo.FINAL_LEVEL)
                    {
                        // Lithios
                        msg = Language.Translate("You've saved the entire planet!\nThe world is in your debt.");
                    }
                    else if (game.Mission.Id == 21)
                    {
                        // Space
                        msg = Language.Translate("You've reached the Lambda Romana star system!\nOnward, to Nantak!");
                    }
                    else if (game.Mission.Id == 22)
                    {
                        // Nantak
                        msg = Language.Translate("One down, two to go.\nOnward, to Carinus!");
                    }
                    else if (game.Mission.Id == 23)
                    {
                        // Carinus
                        msg = Language.Translate("Only one laser left.\nTime to go to Mikara.");
                    }
                    else if (game.Mission.Id == 24)
                    {
                        // Mikara
                        msg = Language.Translate("Good job destroying those lasers!\nNow it's time to battle Malos himself. " +
                            "To Lithios!");
                    }
                }
                else
                {
                    msg = defaultMsg + "!";
                }

                menu.WhenMissionCompleted(id);

                if (player.CurrentQuest.GoalType == QuestGoalType.BeatLevels)
                {
                    IncreaseQuestProgress(1);
                }
            }
            else
            {
                msg = Language.Translate("You have died and have been \ncharged with one life.");
                if (player.Lives == GameInfo.MAX_LIVES)
                {
                    // We need to set the time
                    // for the player's next life
                    player.TimeOfNextLife = DateTime.Now.AddMinutes(GameInfo.MINUTES_UNTIL_NEXT_LIFE);
                }
                player.Lives--;
                game.ChangeLives(player.Lives);
            }
            if (game.Mission.Id == GameInfo.FINAL_LEVEL)
            {
                // You can't go to the next mission if you are already on the last one
                success = false;
            }

            if (id != GameInfo.FINAL_LEVEL)
            {
                CloseDoors();
                lvlComplete.Show(WINDOW_WIDTH, WINDOW_HEIGHT, msg, game.Mission.Id + 1 == player.CurrentMission ? game.Mission.Id : 0, success);
            }
            else
            {
                WhenMalosDefeated();
            }
        }

        private void NextLevel()
        {
            game.LoadMission(game.Mission.Id + 1, player);
            lvlComplete.Active = false;
            OpenDoors();
        }
        private void LevelSelect()
        {
            AttemptToOpenMissions();
            lvlComplete.Active = false;
        }
        private void ReplayLevel()
        {
            game.LoadMission(game.Mission.Id, player);
            lvlComplete.Active = false;
            OpenDoors();
        }

        private void SetAllowedAction(TutorialAction action)
        {
            Popup.BlockAllPopups();
            actionAllowed = action;
        }
        private void TutorialComplete()
        {
            Popup.RemoveBlock();
            UniversalSettings settings = new UniversalSettings();
            settings.PlayedTutorial = true;
            settings.Save();
        }
        private void AttemptTutorialPlay()
        {
            UniversalSettings settings = new UniversalSettings();
            if (!settings.PlayedTutorial)
            {
                tutorial.Play(menu);
            }
        }
        private void AttemptStory()
        {
            UniversalSettings settings = new UniversalSettings();
            if (!settings.ViewedStartingStory)
            {
                Story();
            }
        }
        private void IntroFinished()
        {
            UniversalSettings settings = new UniversalSettings();
            if (!settings.ViewedFullIntro)
            {
                settings.ViewedFullIntro = true;
                settings.Save();
            }
        }

        private void InitLangSet(Languages lang)
        {
            GameInfo.Language = lang;
            UniversalSettings settings = new UniversalSettings();
            settings.Language = (int)lang;
            settings.Save();

            menu.LangChanged(lang);

            intro.Play();
        }

        //private void ItemCollected(Item item)
        //{
        //    if (item is Coin)
        //    {
        //        if (alienItems.Contains(item))
        //        {
        //            int index = alienItems.IndexOf(item);
        //            alienItems[index].StartFlying(playerInterface.CoinDisplayPoint);
        //        }
        //    }
        //    else if (item is MaterialDrop)
        //    {
        //        if (alienItems.Contains(item))
        //        {
        //            int index = alienItems.IndexOf(item);
        //            alienItems[index].StartFlying(playerInterface.UsernamePoint);
        //        }
        //    }
        //}

        private void HandlePauseButtonPress()
        {
            if (gameState == GameState.Paused && ((actionAllowed.Action == TutorialTask.GameState && actionAllowed.Params ==
                (int)GameState.Playing) || !tutorial.Playing))
            {
                gameState = GameState.Playing;
            }
            else if (gameState == GameState.Playing && ((actionAllowed.Action == TutorialTask.GameState && actionAllowed.Params ==
                (int)GameState.Paused) || !tutorial.Playing))
            {
                gameState = GameState.Paused;
            }

            if (tutorial.Playing)
            {
                tutorial.ActionCompleted(menu);
            }
        }
        private void UpdatePauseDetection()
        {
            // Pause Controls
            if (currentKeyState.IsKeyDown(Controls.PauseKey) && prevKeyState.IsKeyUp(Controls.PauseKey))
            {
                HandlePauseButtonPress();
            }
        }

        private void Buy(int cost, Material item)
        {
            player.Coins -= cost;
            player.CoinsSpent += cost;
            player.Purchases++;
            if (player.CurrentQuest.GoalType == QuestGoalType.SpendCoins)
            {
                IncreaseQuestProgress(cost);
            }
            player.MaterialInventory.AddItem(item, 1);
            menu.AddMaterial(item);
            if (tutorial.Playing && actionAllowed.Action == TutorialTask.BuyMaterial)
            {
                tutorial.ActionCompleted(menu);
            }
            if (player.CurrentQuest.GoalType == QuestGoalType.BuyMaterials &&
                    player.CurrentQuest.TypeId == (int)item)
            {
                IncreaseQuestProgress(1);
            }
        }
        private void MaterialSell(Material item)
        {
            int value = GameInfo.SellValueOf(item);
            player.Coins += value;
            player.ItemsSold++;
            if (player.CurrentQuest.GoalType == QuestGoalType.ObtainCoins)
            {
                IncreaseQuestProgress(value);
            }

            if (Sound.IsPlaying(Sounds.CaChing))
            {
                Sound.Stop(Sounds.CaChing);
            }
            Sound.PlaySound(Sounds.CaChing);

            menu.RemoveMaterial(item);
        }
        private void ProjSell(ProjectileType proj)
        {
            int value = GameInfo.SellValueOf(proj);
            player.Coins += value;
            player.ItemsSold++;
            if (player.CurrentQuest.GoalType == QuestGoalType.ObtainCoins)
            {
                IncreaseQuestProgress(value);
            }

            if (Sound.IsPlaying(Sounds.CaChing))
            {
                Sound.Stop(Sounds.CaChing);
            }
            Sound.PlaySound(Sounds.CaChing);

            menu.RemoveProj(proj);
            game.RemoveProjectile(proj);
            player.RemoveProjectile(proj);
        }
        private void BadgeSell(Badge badge)
        {
            int value = GameInfo.SellValueOf(badge);
            player.Coins += value;
            player.ItemsSold++;
            if (player.CurrentQuest.GoalType == QuestGoalType.ObtainCoins)
            {
                IncreaseQuestProgress(value);
            }

            if (Sound.IsPlaying(Sounds.CaChing))
            {
                Sound.Stop(Sounds.CaChing);
            }
            Sound.PlaySound(Sounds.CaChing);

            menu.RemoveBadge(badge);
        }

        //private void AddProj(ProjectileType pType)
        //{
        //    projectiles.Add(GameInfo.CreateProj(pType));
        //}

        private void HandleCtrlW()
        {
            ctrlW.Active = false;
            if (ctrlW.CheckboxChecked)
            {
                uSettings.ShowCtrlWPopup = false;
                uSettings.Save();
            }
            Close();
        }

        private void SaveUser()
        {
            player.ProjectileInventory = GameInfo.ProjListToTypes(game.Projectiles);
            player.SaveUser();
        }

        private void OnCraft(List<Material> items, List<int> counts, Projectile output)
        {
            Sound.PlaySound(Sounds.Craft);
            game.AddProjectile(output.Type);
            player.ProjectileInventory.Add(output.Type);
            if (player.CurrentQuest.GoalType == QuestGoalType.CraftProjectiles &&
                    player.CurrentQuest.TypeId == (int)output.Type)
            {
                IncreaseQuestProgress(1);
            }
            //List<ProjectileType> currentHotbar = player.Hotbar;
            //if (!currentHotbar.Contains(output.Type))
            //{
            //    for (int i = 0; i < currentHotbar.Count; i++)
            //    {
            //        if (currentHotbar[i] == ProjectileType.None)
            //        {
            //            currentHotbar[i] = output.Type;
            //            break;
            //        }
            //    }
            //}
            //player.SetHotbar(currentHotbar);

            for (int i = 0; i < items.Count; i++)
            {
                player.MaterialInventory.RemoveItem(items[i], counts[i]);
            }

            player.ItemsCrafted++;
            menu.Craft(new CraftingRecipe(items, counts, output.Type));

            if (tutorial.Playing && actionAllowed.Action == TutorialTask.CraftItem)
            {
                tutorial.ActionCompleted(menu);
            }
        }

        private void BuyCannon(CannonSettings cannon, int cost)
        {
            player.Coins -= cost;
            player.CoinsSpent += cost;
            player.Purchases++;
            if (player.CurrentQuest.GoalType == QuestGoalType.SpendCoins)
            {
                IncreaseQuestProgress(cost);
            }
            player.Cannons.Add(cannon);
            Sound.PlaySound(Sounds.CaChing);
        }
        private void SelectCannon(CannonSettings cannon)
        {
            player.CannonSettings = cannon;
            player.CannonIndex = player.Cannons.IndexOf(player.Cannons.Where(x => x.CannonType == cannon.CannonType).First());
            game.ChangeCannonSettings(player.CannonSettings);
        }

        private void BuyGift(GiftType gift, int cost)
        {
            player.Coins -= cost;
            player.CoinsSpent += cost;
            player.Purchases++;
            if (player.CurrentQuest.GoalType == QuestGoalType.SpendCoins)
            {
                IncreaseQuestProgress(cost);
            }
            player.Gifts.Add(gift);
            menu.AddGift(gift);
            Sound.PlaySound(Sounds.CaChing);
        }

        //private void CheckLvlUp()
        //{
        //    if (playerInterface.Xp >= playerInterface.XpPerLvl)
        //    {
        //        player.Xp = playerInterface.Xp - player.XpPerLevel;
        //        // Make sure that leftover XP is checked
        //        player.Level++;
        //        player.XpPerLevel = GameInfo.GetXpForLvl(player.Level);
        //        playerInterface.LevelUp(player.Level);
        //        Popup.Show(LanguageTranslator.Translate("You leveled up to level") + " " + player.Level,
        //            false, false);
        //    }
        //}

        private RenderTarget2D TakeScreenshot()
        {
            // Taken from community.monogame.net/t/how-to-make-screenshot/1742/2
            int w, h;
            w = GraphicsDevice.PresentationParameters.BackBufferWidth;
            h = GraphicsDevice.PresentationParameters.BackBufferHeight;
            RenderTarget2D screenshot;
            screenshot = new RenderTarget2D(GraphicsDevice, w, h, false, SurfaceFormat.Bgra32, DepthFormat.None);
            GraphicsDevice.SetRenderTarget(screenshot);
            GraphicsDevice.Present();
            GraphicsDevice.SetRenderTarget(null);
            return screenshot;
        }

        private void UpdateDoors()
        {
            if (closing)
            {
                if (leftDoorRect.X < 0)
                {
                    leftDoorRect.X += DOOR_SPEED;
                    rightDoorRect.X -= DOOR_SPEED;
                }
                else
                {
                    closing = false;
                    if (openAfterClose)
                    {
                        Sound.PlaySound(Sounds.Open);
                        opening = true;
                        gameState = gameStateToBe;
                    }
                }
            }
            else if (opening)
            {
                if (leftDoorRect.X > leftDoorRect.Width * -1)
                {
                    leftDoorRect.X -= DOOR_SPEED;
                    rightDoorRect.X += DOOR_SPEED;
                }
                else
                {
                    opening = false;
                    drawDoors = false;
                    if (gameState == GameState.Credits)
                    {
                        Sound.CheckAndPlaySong(Music.Credits, false);
                    }
                }
            }
        }

        private void UpdateMouse()
        {
            List<MenuButton> buttons = new List<MenuButton>();
            if (lvlComplete.Active)
            {
                buttons.AddRange(lvlComplete.GetButtons());
            }
            if (langSelectPopup.Active)
            {
                buttons.AddRange(langSelectPopup.GetButtons());
            }
            if (!Popup.HasActivePopups && !langSelectPopup.Active && preLvlPopup == null)
            {
                buttons.AddRange(menu.GetButtons(gameState));
                if (gameState == GameState.Playing)
                {
                    buttons.AddRange(game.GetButtons());
                }
            }
            if (preLvlPopup != null)
            {
                buttons.AddRange(preLvlPopup.GetButtons());
            }
            if (ctrlW.Active)
            {
                buttons.AddRange(ctrlW.GetButtons());
            }
            if (tutorial.Playing)
            {
                buttons.AddRange(tutorial.GetButtons());
            }
            buttons.AddRange(Popup.GetButtons());
            buttons.AddRange(Notification.GetButtons());
            mouse.Update(buttons, menu.GetTextboxes(gameState), menu.GetSliders(gameState), menu.GetDraggables(gameState));
        }

        private void UpdateMissionGame(GameTime gameTime)
        {
            if (!(ctrlW.Active || Popup.HasActivePopups) && !(closing || opening)
                    && gameState == GameState.Playing)
            {
                game.Update(gameTime, ref player, actionAllowed.Action == TutorialTask.Sweep,
                    actionAllowed.Action == TutorialTask.ShootRock, tutorial.Playing);
                if (!tutorial.Playing)
                {
                    LevelInfo.Update(gameTime);
                }
            }
        }
        private void SweepFinished()
        {
            if (tutorial.Playing && actionAllowed.Action == TutorialTask.Sweep)
            {
                tutorial.ActionCompleted(menu);
            }
        }
        private void TutorialProjRemoved()
        {
            if (tutorial.Playing && actionAllowed.Action == TutorialTask.ShootRock)
            {
                // We've killed an alien
                tutorial.ActionCompleted(menu);
            }
        }

        //public static void Save(this Texture2D texture, ImageFormat imageFormat, Stream stream)
        //{
        //    var width = texture.Width;
        //    var height = texture.Height;
        //    using (Bitmap bitmap = new Bitmap(width, height, PixelFormat.Format32bppArgb))
        //    {
        //        IntPtr safePtr;
        //        BitmapData bitmapData;
        //        Microsoft.Xna.Framework.Color[] textureData = new Microsoft.Xna.Framework.Color[width * height];
        //        System.Drawing.Rectangle rect = new System.Drawing.Rectangle(0, 0, width, height);

        //        texture.GetData(textureData);
        //        bitmapData = bitmap.LockBits(rect, ImageLockMode.WriteOnly, PixelFormat.Format32bppArgb);
        //        safePtr = bitmapData.Scan0;
        //        Marshal.Copy(textureData, 0, safePtr, textureData.Length);
        //        bitmap.UnlockBits(bitmapData);
        //        bitmap.Save(stream, imageFormat);

        //        textureData = null;
        //    }
        //    GC.Collect();
        //}

        #endregion

        #endregion
    }
}