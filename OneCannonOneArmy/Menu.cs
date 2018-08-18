using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;

namespace OneCannonOneArmy
{
    public delegate User CreateUser(string username, Color avatarColor, Texture2D projectileImage);

    public class Menu
    {
        #region Fields & Properties

        MenuButton startButton;
        MenuButton quitButton;

        MenuButton langButton;
        MenuButton controlsButton;
        MenuButton creditsButton;

        MenuButton createNewUserButton;

        MenuButton resumeButton;
        MenuButton mainMenuButton;
        Texture2D transImg;
        Rectangle transRect;

        ToggleButton muteButton;
        ToggleButton captionButton;

        Story story;
        bool startedStory = false;
        System.Action storyFinish;
        MenuButton skipStoryButton;

        QuestInterface questInterface;

        string ALIENS_KILLED_PRE = Language.Translate("Aliens Killed") + ": ";
        string ALIENS_HIT_PRE = Language.Translate("Aliens Hit") + ": ";
        string COINS_COLLECTED_PRE = Language.Translate("Coins Collected") + ": ";
        string PROJ_FIRED_PRE = Language.Translate("Projectiles Fired") + ": ";
        string ACCURACY_PRE = Language.Translate("Accuracy") + ": ";
        string COINS_SPENT_PRE = Language.Translate("Coins Spent") + ": ";
        string PURCHASES_PRE = Language.Translate("Total Purchases") + ": ";
        string ITEMS_CRAFTED_PRE = Language.Translate("Items Crafted") + ": ";
        string ITEMS_SOLD_PRE = Language.Translate("Items Sold") + ": ";

        const int BUTTON_SPACING = 20;
        const int SPACING = 10;
        const int IMG_SIZE = 50;

        const int PROJECTILE_SIZE = 100;

        Textbox usernameBox;
        MenuButton submitUserButton;
        Rectangle alienRect;
        Rectangle alienEyeRect;
        Color avatarColor;
        string usernamePrefix = Language.Translate("Username") + ": ";
        Vector2 usernamePrePos;
        const int ALIEN_WIDTH = 100;
        const int ALIEN_HEIGHT = 50;

        Texture2D logoImg;
        Rectangle logoRect;
        const int LOGO_WIDTH = 200;
        const int LOGO_HEIGHT = 125;

        MenuButton backButton;

        int userPage = 0;
        List<List<UserInterface>> userPages = new List<List<UserInterface>>();
        List<UserInterface> userInterfaces = new List<UserInterface>();
        const int USER_INTERFACES_PER_PAGE = 4;
        
        Action<User> usernameClicked;
        Action<User> delClicked;

        GameState state;

        ContentManager content;

        SpriteFont bigFont;
        SpriteFont mediumFont;
        SpriteFont smallFont;

        MenuButton nextPageButton;
        MenuButton prevPageButton;

        Texture2D alienImg;
        Texture2D alienEyeImg;
        Texture2D trashImg;
        Texture2D lifeImg;

        int windowWidth;
        int windowHeight;

        const int USERINTERFACE_WIDTH = 850;
        const int USERINTERFACE_HEIGHT = 100;

        public bool AllowPauseExiting = true;

        CreateUser createUser;

        GraphicsDevice graphics;

        const int EYE_SPACING = 5;

        ValueSlider avatarRSlider;
        ValueSlider avatarGSlider;
        ValueSlider avatarBSlider;

        List<Texture2D> projImgs;
        Rectangle projRect;
        int projId = 0;

        MenuButton playButton;
        MenuButton achButton;
        MenuButton statButton;
        MenuButton settingsButton;
        MenuButton shopButton;
        MenuButton organizeButton;
        MenuButton craftingButton;
        MenuButton upgradeButton;
        MenuButton giftButton;
        MenuButton buyLifeButton;

        Shop shop;
        CraftingMenu craftingMenu;
        AchievementMenu achMenu;
        CannonUpgradeMenu upgradeMenu;
        OrganizeMenu organizeMenu;
        MissionMenu missionMenu;
        LanguageSelectMenu langMenu;
        ControlMenu controlMenu;
        SettingsMenu settingsMenu;
        GiftMenu giftMenu;
        Credits credits;

        string nextLifeIn = "";
        Vector2 nextLifeTimePos = new Vector2();

        event Action<int> livesChanged;

        event Action<Languages> languageChanged;

        const int USERNAME_WIDTH = 300;
        const int USERNAME_HEIGHT = 50;
        const int SLIDER_WIDTH = 400;
        const int SLIDER_HEIGHT = 20;

        public GraphicsSettings GraphicsSettings
        {
            get
            {
                return settingsMenu.GraphicsSettings;
            }
        }
        public VolumeSettings VolumeSettings
        {
            get
            {
                return settingsMenu.VolumeSettings;
            }
        }

        event System.Action backButtonClick;

        User user;

        #region Rectangle Properties

        public Rectangle StartRect
        {
            get
            {
                return startButton.DrawRectangle;
            }
        }
        public Rectangle QuitRect
        {
            get
            {
                return quitButton.DrawRectangle;
            }
        }
        public Rectangle PlayRect
        {
            get
            {
                return playButton.DrawRectangle;
            }
        }
        public Rectangle AchievementsRect
        {
            get
            {
                return achButton.DrawRectangle;
            }
        }
        public Rectangle StatsRect
        {
            get
            {
                return statButton.DrawRectangle;
            }
        }
        public Rectangle ShopRect
        {
            get
            {
                return shopButton.DrawRectangle;
            }
        }
        public Rectangle CraftRect
        {
            get
            {
                return craftingButton.DrawRectangle;
            }
        }
        public Rectangle UpgradeRect
        {
            get
            {
                return upgradeButton.DrawRectangle;
            }
        }
        public Rectangle OrganizeRect
        {
            get
            {
                return organizeButton.DrawRectangle;
            }
        }
        public Rectangle SettingsRect
        {
            get
            {
                return settingsButton.DrawRectangle;
            }
        }
        public Rectangle BackRect
        {
            get
            {
                return backButton.DrawRectangle;
            }
        }
        public Rectangle CreateNewUserRect
        {
            get
            {
                return createNewUserButton.DrawRectangle;
            }
        }
        public Rectangle SubmitInfoRect
        {
            get
            {
                return submitUserButton.DrawRectangle;
            }
        }
        public Rectangle PlayerIconRect
        {
            get
            {
                return projRect;
            }
        }
        public Rectangle ColorSliderRect
        {
            get
            {
                return new Rectangle(avatarRSlider.X - BUTTON_SPACING, avatarRSlider.Y - BUTTON_SPACING,
                    (avatarBSlider.X + avatarBSlider.Width + BUTTON_SPACING * 2) - avatarRSlider.X,
                    (avatarBSlider.Y + avatarBSlider.Height + BUTTON_SPACING * 2) - avatarRSlider.Y);
            }
        }
        public Rectangle LanguageRect
        {
            get
            {
                return langButton.DrawRectangle;
            }
        }
        public Rectangle ControlsRect
        {
            get
            {
                return controlsButton.DrawRectangle;
            }
        }
        public Rectangle UsernameBoxRect
        {
            get
            {
                return usernameBox.DrawRectangle;
            }
        }
        public Rectangle ResumeRect
        {
            get
            {
                return resumeButton.DrawRectangle;
            }
        }
        public Rectangle MainMenuRect
        {
            get
            {
                return mainMenuButton.DrawRectangle;
            }
        }
        public Rectangle GiftsRect
        {
            get
            {
                return giftButton.DrawRectangle;
            }
        }

        #endregion

        #endregion

        #region Constructors

        public Menu(GraphicsDevice graphics, int windowWidth, int windowHeight, Texture2D logoImg,
            SpriteFont bigFont, SpriteFont mediumFont, SpriteFont smallFont, ContentManager content,
            Action<User> usernameClicked, Action<User> delClicked, Texture2D alienImg, Texture2D alienEyeImg,
            Texture2D trashImg, Texture2D arrowImg, List<Texture2D> projectileImgs, List<Material> items,
            Dictionary<Material, int> itemCosts, BuyItem buy, List<Projectile> projectiles, UpgradeCannon upgrade,
            List<CannonStats> cannonStats, List<Texture2D> statIcons, Texture2D checkImg, System.Action storyFinish,
            Texture2D lifeImg, List<CannonSettings> cannons, Dictionary<CannonSettings, int> cannonCosts,
            Action<CannonSettings, int> buyCannon, Action<CannonSettings> selectCannon, Texture2D langImg,
            Texture2D controlImg, Texture2D settingsImg, Action<Material> mSell, Action<ProjectileType> pSell,
            Action<Badge> bSell, Texture2D graphicsImg, Texture2D profileImg, Texture2D soundImg,
            Dictionary<GiftType, int> giftCosts, Action<GiftType, int> buyGift, Texture2D crossImg,
            Texture2D ccImg, Texture2D cannonIcon, Texture2D scrollIcon, Texture2D coinIcon,

            System.Action start, System.Action quit, System.Action resume, System.Action mainMenu, System.Action goBack,
            CreateUser createUser, System.Action createNewUser, System.Action play,
            System.Action achievements, System.Action stats, System.Action openShop, System.Action settings, System.Action openOrganize,
            System.Action openCrafting, System.Action openUpgrade, Action<int> selectMission, System.Action buyLife, System.Action openLang,
            Action<Languages> changeLang, System.Action openControls, System.Action openUserSettings, System.Action openGifts,
            System.Action openCredits)
        // Put button click delegates at the bottom
        {
            this.windowWidth = windowWidth;
            this.windowHeight = windowHeight;

            this.createUser = createUser;

            this.graphics = graphics;

            this.bigFont = bigFont;
            this.mediumFont = mediumFont;
            this.smallFont = smallFont;

            this.usernameClicked = usernameClicked;
            this.delClicked = delClicked;

            this.alienImg = alienImg;
            this.alienEyeImg = alienEyeImg;
            this.trashImg = trashImg;
            this.lifeImg = lifeImg;

            this.content = content;

            this.logoImg = logoImg;
            logoRect = new Rectangle(windowWidth / 2 - (LOGO_WIDTH / 2), BUTTON_SPACING, LOGO_WIDTH, LOGO_HEIGHT);

            nextLifeTimePos = new Vector2(windowWidth / 2 - (mediumFont.MeasureString(nextLifeIn).X / 2),
                logoRect.Bottom - SPACING);
            buyLifeButton = new MenuButton(buyLife, windowWidth / 4, (int)nextLifeTimePos.Y, true, graphics, lifeImg);
            buyLifeButton.ImgWidth = buyLifeButton.ImgHeight = (int)mediumFont.MeasureString("A").Y;
            buyLifeButton.Width = buyLifeButton.Height = buyLifeButton.ImgWidth + SPACING;

            shop = new Shop(arrowImg, windowWidth, windowHeight, graphics, items, itemCosts, mediumFont, smallFont, buy,
                cannons, cannonCosts, buyCannon, selectCannon, giftCosts, buyGift, Utilities.MetalImg, cannonIcon, Utilities.GiftImg);

            startButton = new MenuButton(start, Language.Translate("Start"), 0,
                logoRect.Y + logoRect.Height + BUTTON_SPACING, true, bigFont, graphics);

            quitButton = new MenuButton(quit, Language.Translate("Quit"), 0,
                startButton.Y + BUTTON_SPACING + startButton.Height, true, bigFont, graphics);

            langButton = new MenuButton(openLang, 0, 0, true, graphics, langImg);
            langButton.X = windowWidth - langButton.Width - BUTTON_SPACING;
            langButton.Y = windowHeight - langButton.Height - BUTTON_SPACING;
            langButton.ImgHeight = langButton.ImgWidth = IMG_SIZE;

            controlsButton = new MenuButton(openControls, 0, 0, true, graphics, controlImg);
            controlsButton.X = langButton.X - langButton.Width - BUTTON_SPACING;
            controlsButton.Y = langButton.Y;
            controlsButton.ImgWidth = langButton.ImgWidth;
            controlsButton.ImgHeight = langButton.ImgHeight;
            controlsButton.Width = langButton.Width;
            controlsButton.Height = langButton.Height;

            settingsButton = new MenuButton(openUserSettings, langButton.X, langButton.Y, true, graphics, settingsImg);
            settingsButton.ImgWidth = langButton.ImgWidth;
            settingsButton.ImgHeight = langButton.ImgHeight;
            settingsButton.Width = langButton.Width;
            settingsButton.Height = langButton.Height;

            muteButton = new ToggleButton(soundImg, crossImg, graphics, 0, controlsButton.Y, controlsButton.Width, controlsButton.Height);
            muteButton.X = controlsButton.X - muteButton.Width - BUTTON_SPACING;
            muteButton.AddOnToggleHandler(MuteToggle);
            muteButton.Toggled = new UniversalSettings().Muted;

            captionButton = new ToggleButton(ccImg, crossImg, graphics, 0, muteButton.Y, muteButton.Width, muteButton.Height);
            captionButton.X = muteButton.X - captionButton.Width - BUTTON_SPACING;
            captionButton.AddOnToggleHandler(CcToggle);
            Sound.ShowSubtitles = new UniversalSettings().Captions;
            captionButton.Toggled = !Sound.ShowSubtitles;

            resumeButton = new MenuButton(resume, Language.Translate("Resume"), 0, startButton.Y, true,
                bigFont, graphics);

            mainMenuButton = new MenuButton(mainMenu, Language.Translate("Main Menu"), 0,
                resumeButton.Y + resumeButton.Height + BUTTON_SPACING,
                true, bigFont, graphics);

            transImg = Utilities.RectImage;
            transRect = new Rectangle(0, 0, windowWidth, windowHeight);

            usernameBox = new Textbox(0, logoRect.Bottom + SPACING, USERNAME_WIDTH, USERNAME_HEIGHT, bigFont, false, graphics);
            usernameBox.AddEnterPressedHandler(new Action<string>(x => submitUserButton.ClickWithSound()));
            usernameBox.X = windowWidth / 2 - (usernameBox.Width / 2);
            usernamePrePos = new Vector2(usernameBox.X - bigFont.MeasureString(usernamePrefix).X - SPACING,
                usernameBox.Y + (usernameBox.Height / 2 - (bigFont.MeasureString(usernamePrefix).Y / 2)));

            backButtonClick += goBack;
            backButtonClick += GoBackChecking;
            backButton = new MenuButton(backButtonClick, Language.Translate("Back"), BUTTON_SPACING, 0, true,
                bigFont, graphics);
            backButton.Y = windowHeight - (int)(backButton.Height * 2.3f);

            alienRect = new Rectangle(windowWidth / 2 - (ALIEN_WIDTH / 2),
                usernameBox.DrawRectangle.Bottom + BUTTON_SPACING, ALIEN_WIDTH, ALIEN_HEIGHT);
            alienEyeRect = new Rectangle(0, alienRect.Y + (EYE_SPACING * 4),
                EYE_SPACING * 5, EYE_SPACING);
            alienEyeRect.X = alienRect.X + (alienRect.Width / 2 - (alienEyeRect.Width / 2));

            List<User> users = User.LoadUsers();
            if (users != null)
            {
                foreach (User u in users)
                {
                    UserInterface interfaceToAdd = new UserInterface(u, alienImg, alienEyeImg, graphics,
                        usernameClicked, QueryUserDel, bigFont, trashImg, 0, 0, USERINTERFACE_WIDTH,
                        USERINTERFACE_HEIGHT, content, lifeImg);
                    userInterfaces.Add(interfaceToAdd);
                }
                UpdateUserPagesToInterfaces();
            }

            nextPageButton = new MenuButton(NextPage, 0, 0, true, graphics, arrowImg);
            prevPageButton = new MenuButton(PrevPage, 0, nextPageButton.Y, true, graphics, arrowImg);
            prevPageButton.X = BUTTON_SPACING;
            prevPageButton.ImgWidth = prevPageButton.ImgHeight = nextPageButton.ImgWidth = nextPageButton.ImgHeight = IMG_SIZE;
            nextPageButton.X = windowWidth - nextPageButton.Width - BUTTON_SPACING;
            nextPageButton.Y = windowHeight - nextPageButton.Height - BUTTON_SPACING;

            submitUserButton = new MenuButton(new System.Action(CreateUser), Language.Translate("Create User"), 0,
                backButton.Y, true, bigFont, graphics);
            submitUserButton.X = windowWidth / 2 - (submitUserButton.Width / 2);

            createNewUserButton = new MenuButton(createNewUser, Language.Translate("Create New User"), 0,
                submitUserButton.Y, true, bigFont, graphics);
            createNewUserButton.X = windowWidth / 2 - (createNewUserButton.Width / 2);

            avatarRSlider = new ValueSlider(0, transImg, 0, alienRect.Bottom + SPACING, SLIDER_WIDTH, SLIDER_HEIGHT, smallFont,
                Language.Translate("Red"), 255.0f);
            avatarRSlider.X = windowWidth / 2 - (avatarRSlider.Width / 2);
            avatarGSlider = new ValueSlider(255, transImg, 0, avatarRSlider.Y + avatarRSlider.Height + BUTTON_SPACING,
                 SLIDER_WIDTH, SLIDER_HEIGHT, smallFont, Language.Translate("Green"), 255.0f);
            avatarGSlider.X = windowWidth / 2 - (avatarRSlider.Width / 2);
            avatarBSlider = new ValueSlider(0, transImg, 0, avatarGSlider.Y + avatarGSlider.Height + BUTTON_SPACING,
                 SLIDER_WIDTH, SLIDER_HEIGHT, smallFont, Language.Translate("Blue"), 255.0f);
            avatarBSlider.X = windowWidth / 2 - (avatarRSlider.Width / 2);

            avatarColor = new Color(avatarRSlider.Value, avatarGSlider.Value, avatarBSlider.Value);

            projImgs = projectileImgs;
            projRect = new Rectangle(windowWidth / 2 - (PROJECTILE_SIZE / 2),
                avatarBSlider.Y + avatarBSlider.Height + SPACING, PROJECTILE_SIZE, PROJECTILE_SIZE);

            questInterface = new QuestInterface(graphics, scrollIcon, 0, logoRect.Bottom + BUTTON_SPACING,
                mediumFont, user, windowWidth, windowHeight, coinIcon);
            questInterface.X = windowWidth / 2 - questInterface.Width / 2;

            playButton = new MenuButton(play, Language.Translate("Play"), 0, questInterface.Y + questInterface.Height + BUTTON_SPACING,
                true, bigFont, graphics);
            //playButton.X = windowWidth / 2 - (playButton.Width / 2);

            achButton = new MenuButton(achievements, Language.Translate("Achievements"), 0, 0, true, bigFont, graphics);
            statButton = new MenuButton(stats, Language.Translate("Stats"), 0, 0, true, bigFont, graphics);

            //int achStatWidth = achButton.Width + statButton.Width + BUTTON_SPACING;
            //achButton.X = windowWidth / 2 - (achStatWidth / 2);
            //statButton.X = achButton.X + achButton.Width + BUTTON_SPACING;

            shopButton = new MenuButton(openShop, Language.Translate("Shop"), 0, 0, true, bigFont, graphics);
            craftingButton = new MenuButton(openCrafting, Language.Translate("Crafting"), 0, 0, true, bigFont, graphics);

            //int craftShopWidth = craftingButton.Width + shopButton.Width + BUTTON_SPACING;
            //shopButton.X = windowWidth / 2 - (craftShopWidth / 2);
            //craftingButton.X = shopButton.X + BUTTON_SPACING + shopButton.Width;

            upgradeButton = new MenuButton(openUpgrade, Language.Translate("Upgrade"), 0, 0, true, bigFont, graphics);
            organizeButton = new MenuButton(openOrganize, Language.Translate("Organize"), 0, 0, true, bigFont, graphics);

            //int upgradeOrganizeWidth = upgradeButton.Width + organizeButton.Width + BUTTON_SPACING;
            //upgradeButton.X = windowWidth / 2 - (upgradeOrganizeWidth / 2);
            //organizeButton.X = upgradeButton.X + upgradeButton.Width + BUTTON_SPACING;

            giftButton = new MenuButton(openGifts, Language.Translate("Gifts"), 0, 0, true, bigFont, graphics);

            PositionButtons();

            craftingMenu = new CraftingMenu(graphics, mediumFont, smallFont, arrowImg, windowWidth, windowHeight, projectiles);

            achMenu = new AchievementMenu(content, windowWidth, windowHeight, smallFont, bigFont, graphics, arrowImg,
                checkImg);

            upgradeMenu = new CannonUpgradeMenu(windowWidth, windowHeight, graphics, arrowImg, cannonStats, upgrade,
                statIcons, smallFont);

            organizeMenu = new OrganizeMenu(smallFont, bigFont, mediumFont, graphics, windowWidth, windowHeight, mSell, pSell, bSell);

            langMenu = new LanguageSelectMenu(windowWidth, windowHeight, graphics, bigFont, changeLang);

            controlMenu = new ControlMenu(graphics, windowWidth, windowHeight, bigFont, smallFont);

            giftMenu = new GiftMenu(graphics, arrowImg, windowWidth, windowHeight, mediumFont);

            story = new Story(windowWidth, windowHeight, mediumFont);
            this.storyFinish = storyFinish;
            skipStoryButton = new MenuButton(storyFinish, Language.Translate("Skip"), 0, 0,
                true, bigFont, graphics);
            skipStoryButton.X = windowWidth - skipStoryButton.Width - BUTTON_SPACING;
            skipStoryButton.Y = windowHeight - skipStoryButton.Height - BUTTON_SPACING;

            missionMenu = new MissionMenu(graphics, mediumFont, bigFont, windowWidth, windowHeight, checkImg);
            missionMenu.AddClickHandler(selectMission);

            settingsMenu = new SettingsMenu(windowWidth, windowHeight, graphics, USERNAME_WIDTH, USERNAME_HEIGHT,
                bigFont, mediumFont, smallFont, SLIDER_WIDTH, SLIDER_HEIGHT, projectileImgs, arrowImg, graphicsImg,
                profileImg, soundImg, crossImg);

            credits = new Credits(mediumFont, windowWidth, windowHeight, logoImg, LOGO_WIDTH, LOGO_HEIGHT);
            credits.AddExitCreditsHandler(backButtonClick);
            creditsButton = new MenuButton(openCredits, Language.Translate("Credits"), 0, 0, true, bigFont, graphics);
            creditsButton.X = windowWidth / 2 - creditsButton.Width / 2;
            creditsButton.Y = windowHeight - creditsButton.Height - BUTTON_SPACING;
        }

        #endregion

        #region Public Methods

        public void Update(GameState state, GameTime gameTime, User user, List<Projectile> projectiles, bool? onlyBuyStone,
            bool playMusic)
        {
            #region Universal Updating

            this.state = state;
            this.user = user;

            usernameBox.Drawn = (state == GameState.NewUser);

            if (state == GameState.NewUser)
            {
                nextPageButton.Y = projRect.Bottom;
                prevPageButton.Y = nextPageButton.Y;
                nextPageButton.X = projRect.Right + BUTTON_SPACING;
                prevPageButton.X = projRect.X - prevPageButton.Width - BUTTON_SPACING;
            }
            else if (state == GameState.PickUser || state == GameState.Achievements)
            {
                nextPageButton.Y = windowHeight - nextPageButton.Height - BUTTON_SPACING;
                prevPageButton.Y = nextPageButton.Y;
                nextPageButton.X = windowWidth - nextPageButton.Width - BUTTON_SPACING;
                prevPageButton.X = BUTTON_SPACING;
            }

            if (state == GameState.Organize || state == GameState.Credits)
            {
                backButton.Y = windowHeight - backButton.Height - SPACING;
            }
            else if (state == GameState.UserSettings)
            {
                backButton.Y = windowHeight - backButton.Height - SettingsMenu.SPACING;
            }
            else
            {
                backButton.Y = windowHeight - (int)(backButton.Height * 2.3f);
            }

            if (state != GameState.None && state != GameState.Paused && state != GameState.Playing &&
                state != GameState.Story && state != GameState.Credits && state != GameState.Story
                && playMusic)
            {
                Sound.CheckAndPlaySong(Music.MainMenu);
            }

            #endregion

            switch (state)
            {
                #region Start Menu
                case GameState.StartMenu:

                    startButton.Update(gameTime);
                    langButton.Update(gameTime);
                    controlsButton.Update(gameTime);
                    muteButton.Update(gameTime);
                    captionButton.Update(gameTime);
                    creditsButton.Update(gameTime);

                    quitButton.Active = true;
                    quitButton.Y = startButton.Y + startButton.Height + BUTTON_SPACING;
                    quitButton.Update(gameTime);

                    break;
                #endregion

                #region Paused

                case GameState.Paused:

                    mainMenuButton.Active = quitButton.Active = AllowPauseExiting;

                    resumeButton.Update(gameTime);
                    mainMenuButton.Update(gameTime);

                    quitButton.Y = mainMenuButton.Y + mainMenuButton.Height + BUTTON_SPACING;
                    quitButton.Update(gameTime);

                    break;

                #endregion

                #region Pick User

                case GameState.PickUser:

                    // Clamping, Efficiency Fixes, and Bug Fixes
                    for (int i = 0; i < userPages.Count; i++)
                    {
                        if (userPages[i].Count == 0)
                        {
                            userPages.RemoveAt(i);
                            if (userPage >= userPages.Count)
                            {
                                userPage = userPages.Count - 1;
                            }
                        }
                    }

                    if (userPage > userPages.Count - 1)
                    {
                        userPage = userPages.Count - 1;
                    }

                    // Update objects
                    if (userPages.Count > 0)
                    {
                        for (int i = 0; i < userPages[userPage].Count; i++)
                        {
                            userPages[userPage][i].Update(gameTime);
                        }
                    }

                    if (userPages.Count <= 1)
                    {
                        // Both buttons should be disabled, as there is only 1 page
                        // This must be tested for first, otherwise, the since the page always starts equal to 0, the below criteria will be met
                        // and the game will act like there are 2 pages when there is only 1
                        nextPageButton.Active = false;
                        prevPageButton.Active = false;
                        // Since there is only one page, we have to set the page integer to 0
                        userPage = 0;
                    }
                    else if (userPage == 0)
                    {
                        // There is another page, but we are on the first. Therefore, we cannot go backwards, so we'll disable the backwards button
                        nextPageButton.Active = true;
                        prevPageButton.Active = false;
                    }
                    else if (userPage + 1 == userPages.Count)
                    {
                        // We are on the last page, so the forward button must be disabled
                        nextPageButton.Active = false;
                        prevPageButton.Active = true;
                    }
                    else
                    {
                        // We must be somewhere in the middle, so both buttons should be enabled
                        nextPageButton.Active = true;
                        prevPageButton.Active = true;
                    }

                    nextPageButton.Update(gameTime);
                    prevPageButton.Update(gameTime);
                    backButton.Update(gameTime);
                    createNewUserButton.Update(gameTime);

                    break;

                #endregion

                #region New User

                case GameState.NewUser:

                    backButton.Update(gameTime);
                    usernameBox.Update(gameTime);
                    submitUserButton.Update(gameTime);
                    avatarRSlider.Update();
                    avatarGSlider.Update();
                    avatarBSlider.Update();
                    avatarColor.R = (byte)avatarRSlider.Value;
                    avatarColor.G = (byte)avatarGSlider.Value;
                    avatarColor.B = (byte)avatarBSlider.Value;
                    nextPageButton.Active = true;
                    prevPageButton.Active = true;
                    nextPageButton.Update(gameTime);
                    prevPageButton.Update(gameTime);

                    break;

                #endregion

                #region Main Menu

                case GameState.MainMenu:

                    quitButton.Active = true;

                    questInterface.Update(user, gameTime);
                    if (!questInterface.ShowingPopup)
                    {
                        playButton.Update(gameTime);
                        achButton.Update(gameTime);
                        shopButton.Update(gameTime);
                        statButton.Update(gameTime);
                        craftingButton.Update(gameTime);
                        upgradeButton.Update(gameTime);
                        organizeButton.Update(gameTime);
                        settingsButton.Update(gameTime);
                        giftButton.Update(gameTime);

                        backButton.Update(gameTime);

                        quitButton.Y = craftingButton.Y + BUTTON_SPACING + craftingButton.Height;
                        quitButton.Update(gameTime);

                        if (user != null)
                        {
                            if (user.Lives < GameInfo.MAX_LIVES)
                            {
                                // First check if the user is eligible for another life
                                if (DateTime.Now - user.TimeOfNextLife >= TimeSpan.Zero)
                                {
                                    user.AddLives(1);
                                    user.TimeOfNextLife = DateTime.Now.AddMinutes(GameInfo.MINUTES_UNTIL_NEXT_LIFE);
                                    livesChanged?.Invoke(user.Lives);
                                }
                                // We need to tell the user how much time is left
                                // before they receive their next life
                                nextLifeIn = Language.Translate("Time until next life: ")
                                    + user.TimeOfNextLife.FormatDistanceToNow();
                                nextLifeTimePos.X = windowWidth / 2 - (mediumFont.MeasureString(nextLifeIn).X / 2);
                                buyLifeButton.Active = true;
                            }
                            else
                            {
                                buyLifeButton.Active = false;
                            }
                        }

                        if (buyLifeButton.Active)
                        {
                            buyLifeButton.Update(gameTime);
                        }
                    }

                    break;

                #endregion

                #region Achievements

                case GameState.Achievements:

                    backButton.Update(gameTime);
                    achMenu.Update(user, gameTime);

                    break;

                #endregion

                #region Stats

                case GameState.Stats:

                    backButton.Update(gameTime);

                    break;

                #endregion

                #region Shop

                case GameState.Shop:

                    backButton.Update(gameTime);
                    shop.Update(user, onlyBuyStone, gameTime);

                    break;

                #endregion

                #region Crafting

                case GameState.Crafting:

                    craftingMenu.Update(user, projectiles, gameTime);
                    backButton.Update(gameTime);

                    break;

                #endregion

                #region Upgrade

                case GameState.Upgrade:

                    backButton.Update(gameTime);
                    upgradeMenu.Update(user.CannonSettings, user, gameTime);

                    break;

                #endregion

                #region Organize

                case GameState.Organize:

                    organizeMenu.Update(user, gameTime);
                    backButton.Update(gameTime);

                    break;

                #endregion

                #region Story

                case GameState.Story:

                    if (!startedStory)
                    {
                        story.StartStory(new UniversalSettings().ViewedStartingStory ? 1 : 0);
                        startedStory = true;
                    }
                    story.Update();
                    skipStoryButton.Update(gameTime);
                    if (!story.Scrolling)
                    {
                        storyFinish?.Invoke();
                    }

                    break;

                #endregion

                #region Missions

                case GameState.SelectMission:

                    if (!Popup.HasActivePopups)
                    {
                        missionMenu.Update(user, gameTime);
                        backButton.Update(gameTime);
                    }

                    break;

                #endregion

                #region Language

                case GameState.Language:

                    langMenu.Update(gameTime);
                    backButton.Update(gameTime);

                    break;

                #endregion

                #region Controls

                case GameState.ChangeControls:

                    backButton.Update(gameTime);
                    controlMenu.Update(gameTime);

                    break;

                #endregion

                #region User Settings

                case GameState.UserSettings:

                    settingsMenu.Update(gameTime);
                    backButton.Update(gameTime);

                    break;

                #endregion

                #region Gifts

                case GameState.Gifts:

                    if (!giftMenu.ShowingGiftPopup)
                    {
                        backButton.Update(gameTime);
                    }
                    giftMenu.Update(gameTime, user);

                    break;

                #endregion

                #region Credits

                case GameState.Credits:

                    backButton.Update(gameTime);
                    credits.Update(gameTime);

                    break;

                    #endregion
            }
        }

        public void Draw(GameState state, SpriteBatch spriteBatch, User user)
        {
            switch (state)
            {
                #region Start Menu
                case GameState.StartMenu:

                    DrawTitle(spriteBatch);
                    startButton.Draw(spriteBatch);
                    quitButton.Draw(spriteBatch);
                    langButton.Draw(spriteBatch);
                    controlsButton.Draw(spriteBatch);
                    muteButton.Draw(spriteBatch);
                    captionButton.Draw(spriteBatch);
                    creditsButton.Draw(spriteBatch);

                    break;
                #endregion

                #region Paused
                case GameState.Paused:

                    spriteBatch.Draw(transImg, transRect, Color.Gray * 0.5f);

                    resumeButton.Draw(spriteBatch);
                    mainMenuButton.Draw(spriteBatch);
                    quitButton.Draw(spriteBatch);

                    break;
                #endregion

                #region Pick User

                case GameState.PickUser:

                    int lastYUser = logoRect.Y + logoRect.Height + SPACING;
                    if (userPage <= userPages.Count - 1)
                    {
                        foreach (UserInterface ui in userPages[userPage])
                        {
                            ui.X = windowWidth / 2 - (ui.Width / 2);
                            ui.Y = lastYUser;
                            ui.Draw(spriteBatch);
                            lastYUser += BUTTON_SPACING + ui.Height;
                        }
                    }

                    nextPageButton.Draw(spriteBatch);
                    prevPageButton.Draw(spriteBatch, null, SpriteEffects.FlipHorizontally);
                    DrawTitle(spriteBatch);
                    backButton.Draw(spriteBatch);
                    createNewUserButton.Draw(spriteBatch);

                    break;

                #endregion

                #region New User

                case GameState.NewUser:

                    usernameBox.Draw(spriteBatch);
                    spriteBatch.DrawString(bigFont, usernamePrefix, usernamePrePos, Textbox.ColorTheme);
                    backButton.Draw(spriteBatch);
                    DrawTitle(spriteBatch);
                    submitUserButton.Draw(spriteBatch);
                    spriteBatch.Draw(alienImg, alienRect, avatarColor);
                    spriteBatch.Draw(alienEyeImg, alienEyeRect, Color.White);
                    avatarRSlider.Draw(spriteBatch);
                    avatarGSlider.Draw(spriteBatch);
                    avatarBSlider.Draw(spriteBatch);
                    nextPageButton.Draw(spriteBatch);
                    prevPageButton.Draw(spriteBatch, null, SpriteEffects.FlipHorizontally);
                    spriteBatch.Draw(projImgs[projId], projRect, Color.White);

                    break;

                #endregion

                #region Main Menu

                case GameState.MainMenu:

                    DrawTitle(spriteBatch);

                    playButton.Draw(spriteBatch);
                    achButton.Draw(spriteBatch);
                    shopButton.Draw(spriteBatch);
                    statButton.Draw(spriteBatch);
                    quitButton.Draw(spriteBatch);
                    craftingButton.Draw(spriteBatch);
                    upgradeButton.Draw(spriteBatch);
                    organizeButton.Draw(spriteBatch);
                    settingsButton.Draw(spriteBatch);
                    giftButton.Draw(spriteBatch);

                    backButton.Draw(spriteBatch);

                    if (buyLifeButton.Active)
                    {
                        buyLifeButton.Draw(spriteBatch);
                    }

                    if (user != null)
                    {
                        if (user.Lives < GameInfo.MAX_LIVES)
                        {
                            spriteBatch.DrawString(mediumFont, nextLifeIn, nextLifeTimePos, Color.Black);
                        }
                    }

                    questInterface.Draw(spriteBatch);

                    break;

                #endregion

                #region Achievements

                case GameState.Achievements:

                    DrawTitle(spriteBatch);

                    backButton.Draw(spriteBatch);
                    achMenu.Draw(spriteBatch);

                    break;

                #endregion

                #region Stats

                case GameState.Stats:

                    DrawTitle(spriteBatch);

                    int nextY = logoRect.Bottom + BUTTON_SPACING;
                    spriteBatch.DrawString(mediumFont, ALIENS_HIT_PRE + user.AliensHit,
                        new Vector2(windowWidth / 2 - (mediumFont.MeasureString(ALIENS_HIT_PRE + user.AliensHit).X / 2),
                        nextY), Color.DarkRed);
                    nextY += (int)(BUTTON_SPACING + mediumFont.MeasureString("A").Y);

                    spriteBatch.DrawString(mediumFont, PROJ_FIRED_PRE + user.ProjectilesFired,
                        new Vector2(windowWidth / 2 - (mediumFont.MeasureString(PROJ_FIRED_PRE + user.ProjectilesFired).X / 2),
                        nextY), Color.Orange);
                    nextY += (int)(BUTTON_SPACING + mediumFont.MeasureString("A").Y);

                    spriteBatch.DrawString(mediumFont, ACCURACY_PRE + user.Accuracy + "%",
                        new Vector2(windowWidth / 2 - (mediumFont.MeasureString(ACCURACY_PRE + user.Accuracy + "%").X / 2),
                        nextY), Color.Yellow);
                    nextY += (int)(BUTTON_SPACING + mediumFont.MeasureString("A").Y);

                    spriteBatch.DrawString(mediumFont, ALIENS_KILLED_PRE + user.AliensKilled,
                        new Vector2(windowWidth / 2 - (mediumFont.MeasureString(ALIENS_KILLED_PRE + user.AliensKilled).X / 2),
                        nextY), Color.Green);
                    nextY += (int)(BUTTON_SPACING + mediumFont.MeasureString("A").Y);

                    spriteBatch.DrawString(mediumFont, COINS_COLLECTED_PRE + user.CoinsCollected,
                        new Vector2(windowWidth / 2 - (mediumFont.MeasureString(COINS_COLLECTED_PRE + user.CoinsCollected).X / 2),
                        nextY), Color.Blue);
                    nextY += (int)(BUTTON_SPACING + mediumFont.MeasureString("A").Y);

                    spriteBatch.DrawString(mediumFont, COINS_SPENT_PRE + user.CoinsSpent,
                        new Vector2(windowWidth / 2 - (mediumFont.MeasureString(COINS_SPENT_PRE + user.CoinsSpent).X / 2),
                        nextY), Color.Indigo);
                    nextY += (int)(BUTTON_SPACING + mediumFont.MeasureString("A").Y);

                    spriteBatch.DrawString(mediumFont, ITEMS_SOLD_PRE + user.ItemsSold,
                        new Vector2(windowWidth / 2 - (mediumFont.MeasureString(ITEMS_SOLD_PRE + user.ItemsSold).X / 2),
                        nextY), Color.MediumPurple);
                    nextY += (int)(BUTTON_SPACING + mediumFont.MeasureString("A").Y);

                    spriteBatch.DrawString(mediumFont, PURCHASES_PRE + user.Purchases,
                        new Vector2(windowWidth / 2 - (mediumFont.MeasureString(PURCHASES_PRE + user.Purchases).X / 2),
                        nextY), Color.Black);
                    nextY += (int)(BUTTON_SPACING + mediumFont.MeasureString("A").Y);

                    spriteBatch.DrawString(mediumFont, ITEMS_CRAFTED_PRE + user.ItemsCrafted,
                        new Vector2(windowWidth / 2 - (mediumFont.MeasureString(ITEMS_CRAFTED_PRE + user.ItemsCrafted).X / 2),
                        nextY), Color.White);
                    nextY += (int)(BUTTON_SPACING + mediumFont.MeasureString("A").Y);

                    backButton.Draw(spriteBatch);

                    break;

                #endregion

                #region Shop

                case GameState.Shop:

                    DrawTitle(spriteBatch);

                    backButton.Draw(spriteBatch);
                    shop.Draw(spriteBatch);

                    break;

                #endregion

                #region Crafting

                case GameState.Crafting:

                    DrawTitle(spriteBatch);
                    craftingMenu.Draw(spriteBatch, user);
                    backButton.Draw(spriteBatch);

                    break;

                #endregion

                #region Upgrade

                case GameState.Upgrade:

                    DrawTitle(spriteBatch);
                    upgradeMenu.Draw(spriteBatch);
                    backButton.Draw(spriteBatch);

                    break;

                #endregion

                #region Organize

                case GameState.Organize:

                    organizeMenu.Draw(spriteBatch);
                    backButton.Draw(spriteBatch);
                    DrawTitle(spriteBatch);

                    break;

                #endregion

                #region Story

                case GameState.Story:

                    story.Draw(spriteBatch);
                    skipStoryButton.Draw(spriteBatch);

                    break;

                #endregion

                #region Missions

                case GameState.SelectMission:

                    DrawTitle(spriteBatch);
                    missionMenu.Draw(spriteBatch);
                    backButton.Draw(spriteBatch);

                    break;

                #endregion

                #region Language

                case GameState.Language:

                    DrawTitle(spriteBatch);
                    backButton.Draw(spriteBatch);
                    langMenu.Draw(spriteBatch);

                    break;

                #endregion

                #region Controls

                case GameState.ChangeControls:

                    DrawTitle(spriteBatch);
                    controlMenu.Draw(spriteBatch);
                    backButton.Draw(spriteBatch);

                    break;

                #endregion

                #region User Settings

                case GameState.UserSettings:

                    DrawTitle(spriteBatch);
                    settingsMenu.Draw(spriteBatch);
                    backButton.Draw(spriteBatch);

                    break;

                #endregion

                #region Gifts

                case GameState.Gifts:

                    DrawTitle(spriteBatch);
                    backButton.Draw(spriteBatch);
                    giftMenu.Draw(spriteBatch);

                    break;

                #endregion

                #region Credits

                case GameState.Credits:

                    credits.Draw(spriteBatch);
                    backButton.Draw(spriteBatch);

                    break;

                    #endregion
            }
        }

        public List<Textbox> GetTextboxes(GameState state)
        {
            List<Textbox> returnVal = new List<Textbox>();
            switch (state)
            {
                case GameState.NewUser:
                    returnVal.Add(usernameBox);
                    break;
                case GameState.UserSettings:
                    returnVal.AddRange(settingsMenu.GetTextboxes());
                    break;
            }
            return returnVal;
        }
        public List<MenuButton> GetButtons(GameState state)
        {
            List<MenuButton> returnVal = new List<MenuButton>();

            switch (state)
            {
                case GameState.StartMenu:
                    returnVal.Add(startButton);
                    returnVal.Add(quitButton);
                    returnVal.Add(langButton);
                    returnVal.Add(controlsButton);
                    returnVal.Add(muteButton.Button);
                    returnVal.Add(captionButton.Button);
                    returnVal.Add(creditsButton);
                    break;

                case GameState.Paused:
                    returnVal.Add(resumeButton);
                    returnVal.Add(mainMenuButton);
                    returnVal.Add(quitButton);
                    break;

                case GameState.PickUser:
                    if (userPages.Count > 0)
                    {
                        foreach (UserInterface ui in userPages[userPage])
                        {
                            returnVal.AddRange(ui.GetButtons());
                        }
                    }
                    returnVal.Add(backButton);
                    returnVal.Add(createNewUserButton);
                    returnVal.Add(nextPageButton);
                    returnVal.Add(prevPageButton);
                    break;

                case GameState.NewUser:
                    returnVal.Add(backButton);
                    returnVal.Add(submitUserButton);
                    returnVal.Add(nextPageButton);
                    returnVal.Add(prevPageButton);
                    break;

                case GameState.MainMenu:
                    if (!questInterface.ShowingPopup)
                    {
                        returnVal.Add(playButton);
                        returnVal.Add(achButton);
                        returnVal.Add(shopButton);
                        returnVal.Add(statButton);
                        returnVal.Add(settingsButton);
                        returnVal.Add(backButton);
                        returnVal.Add(quitButton);
                        returnVal.Add(craftingButton);
                        returnVal.Add(upgradeButton);
                        returnVal.Add(organizeButton);
                        returnVal.Add(buyLifeButton);
                        returnVal.Add(giftButton);
                    }
                    returnVal.AddRange(questInterface.GetButtons());
                    break;

                case GameState.Achievements:
                    returnVal.AddRange(achMenu.GetButtons());
                    returnVal.Add(backButton);
                    break;

                case GameState.Stats:
                case GameState.Credits:
                    returnVal.Add(backButton);
                    break;

                case GameState.Shop:
                    returnVal.AddRange(shop.GetButtons());
                    returnVal.Add(backButton);
                    break;

                case GameState.Crafting:
                    returnVal.AddRange(craftingMenu.GetButtons());
                    returnVal.Add(backButton);
                    break;

                case GameState.Upgrade:
                    returnVal.AddRange(upgradeMenu.GetButtons());
                    returnVal.Add(backButton);
                    break;

                case GameState.Organize:
                    returnVal.AddRange(organizeMenu.GetButtons());
                    returnVal.Add(backButton);
                    break;

                case GameState.Story:
                    returnVal.Add(skipStoryButton);
                    break;

                case GameState.SelectMission:
                    returnVal.Add(backButton);
                    returnVal.AddRange(missionMenu.GetButtons());
                    break;

                case GameState.Language:
                    returnVal.Add(backButton);
                    returnVal.AddRange(langMenu.GetButtons());
                    break;

                case GameState.ChangeControls:
                    returnVal.Add(backButton);
                    returnVal.AddRange(controlMenu.GetButtons());
                    break;

                case GameState.UserSettings:
                    returnVal.Add(backButton);
                    returnVal.AddRange(settingsMenu.GetButtons());
                    break;

                case GameState.Gifts:
                    returnVal.Add(backButton);
                    returnVal.AddRange(giftMenu.GetButtons());
                    break;

                    // case GameState.Credits: is above w/ Stats GameState
            }

            return returnVal;
        }
        public List<DraggableProjectile> GetDraggables(GameState state)
        {
            if (state == GameState.Organize)
            {
                return organizeMenu.GetDraggables();
            }
            return new List<DraggableProjectile>();
        }
        public List<ValueSlider> GetSliders(GameState state)
        {
            List<ValueSlider> returnVal = new List<ValueSlider>();

            if (state == GameState.NewUser)
            {
                returnVal.Add(avatarRSlider);
                returnVal.Add(avatarGSlider);
                returnVal.Add(avatarBSlider);
            }
            else if (state == GameState.UserSettings)
            {
                returnVal.AddRange(settingsMenu.GetSliders());
            }

            return returnVal;
        }

        public void UserChanged()
        {
            List<User> users = User.LoadUsers();
            for (int i = 0; i < userInterfaces.Count; i++)
            {
                userInterfaces[i].UpdateInfo(users[i]);
            }
            UpdateUserPagesToInterfaces();
        }

        public void WhenMissionCompleted(int id)
        {
            missionMenu.MissionComplete(id);
            if (GameInfo.ProjVisibilityLvls.ContainsValue(id))
            {
                for (int i = 0; i < GameInfo.ProjVisibilityLvls.Count; i++)
                {
                    if (GameInfo.ProjVisibilityLvls.ElementAt(i).Value == id)
                    {
                        // One of the new projectiles
                        ProjectileType proj = GameInfo.ProjVisibilityLvls.ElementAt(i).Key;
                        craftingMenu.AddItem(CraftingRecipe.GetRecipeForProj(proj), GameInfo.CreateProj(proj));
                    }
                }
            }
        }

        public void AddOnCraftHandler(OnCraft handler)
        {
            craftingMenu.AddOnCraftHandler(handler);
        }

        public void AddReplayTutorialHandler(System.Action handler)
        {
            settingsMenu.AddOnReplayTutorialClickHandler(handler);
        }
        public void AddSettingsSubmitHandler(SubmitUserInfo handler)
        {
            settingsMenu.AddSubmitInfoHander(handler);
        }

        public void AddClaimGiftHandler(RewardGift handler)
        {
            giftMenu.AddClaimHandler(handler);
        }

        public void OnSignIn(User user)
        {
            achMenu.Reset();
            organizeMenu.Initialize(user.ProjectileInventory, user.MaterialInventory, user.Hotbar, user.Collection);
            shop.Initialize(user);
            craftingMenu.Initialize(user);
            settingsMenu.Initialize(user);
            giftMenu.Initialize(user);
            missionMenu.Initialize(user);
        }

        public void AddMaterial(Material material)
        {
            organizeMenu.AddMaterial(material, 1);
        }
        public void Craft(CraftingRecipe recipe)
        {
            AddProj(recipe.Result);
            for (int i = 0; i < recipe.Materials.Count; i++)
            {
                organizeMenu.RemoveMaterial(recipe.Materials[i], recipe.MaterialAmounts[i]);
            }
        }
        public void AddProj(ProjectileType p)
        {
            organizeMenu.AddProjectile(p, 1);
        }
        public void RemoveMaterial(Material m)
        {
            organizeMenu.RemoveMaterial(m, 1);
        }
        public void RemoveProj(ProjectileType type)
        {
            organizeMenu.RemoveProjectile(type, 1);
        }
        public void AddBadge(Badge badge)
        {
            organizeMenu.AddBadge(badge, 1);
        }
        public void RemoveBadge(Badge badge)
        {
            organizeMenu.RemoveBadge(badge, 1);
        }
        public void AddGift(GiftType gift)
        {
            giftMenu.AddGift(gift);
        }

        public void ResetStory(int storyPos)
        {
            startedStory = true;
            story.StartStory(storyPos);
        }

        public void AddChangeLivesHandler(Action<int> handler)
        {
            livesChanged += handler;
        }

        public void LangChanged(Languages lang)
        {
            languageChanged?.Invoke(lang);
            shop.LangChanged();
            craftingMenu.LangChanged();
            achMenu.LangChanged();
            upgradeMenu.LangChanged();
            settingsMenu.LangChanged();

            playButton.Text = Language.Translate("Play");
            quitButton.Text = Language.Translate("Quit");
            startButton.Text = Language.Translate("Start");
            achButton.Text = Language.Translate("Achievements");
            statButton.Text = Language.Translate("Stats");
            craftingButton.Text = Language.Translate("Crafting");
            shopButton.Text = Language.Translate("Shop");
            upgradeButton.Text = Language.Translate("Upgrade");
            organizeButton.Text = Language.Translate("Organize");
            backButton.Text = Language.Translate("Back");
            createNewUserButton.Text = Language.Translate("Create New User");
            submitUserButton.Text = Language.Translate("Create User");
            mainMenuButton.Text = Language.Translate("Main Menu");
            skipStoryButton.Text = Language.Translate("Skip");
            resumeButton.Text = Language.Translate("Resume");
            giftButton.Text = Language.Translate("Gifts");
            creditsButton.Text = Language.Translate("Credits");

            avatarRSlider.ValueName = Language.Translate("Red");
            avatarGSlider.ValueName = Language.Translate("Green");
            avatarBSlider.ValueName = Language.Translate("Blue");

            usernamePrefix = Language.Translate("Username");

            ALIENS_KILLED_PRE = Language.Translate("Aliens Killed") + ": ";
            ALIENS_HIT_PRE = Language.Translate("Aliens Hit") + ": ";
            COINS_COLLECTED_PRE = Language.Translate("Coins Collected") + ": ";
            PROJ_FIRED_PRE = Language.Translate("Projectiles Fired") + ": ";
            ACCURACY_PRE = Language.Translate("Accuracy") + ": ";
            COINS_SPENT_PRE = Language.Translate("Coins Spent") + ": ";
            PURCHASES_PRE = Language.Translate("Total Purchases") + ": ";
            ITEMS_CRAFTED_PRE = Language.Translate("Items Crafted") + ": ";
            ITEMS_SOLD_PRE = Language.Translate("Items Sold") + ": ";

            PositionButtons();
        }

        public void OrganizeOpened(User user)
        {
            organizeMenu.Reset(user);
        }

        //public void OnlyAllowActions(TutorialActionArea[] actions)
        //{
        //    DisableAllButtons();
        //    for (int i = 0; i < actions.Count(); i++)
        //    {
        //        EnableButton(actions[i]);
        //    }
        //}

        #endregion

        #region Private Methods

        #region Paging

        private void AddUser(User user)
        {
            UserInterface interfaceToAdd = new UserInterface(user, alienImg, alienEyeImg, graphics, usernameClicked,
                QueryUserDel, bigFont, trashImg, 0, 0, USERINTERFACE_WIDTH, USERINTERFACE_HEIGHT, content, lifeImg);

            // This gets the currently used page and adds to it. 
            // Unfortunately, we need to add extra code to check if the latest page is full.
            if (userPages.Count > 0)
            {
                for (int i = 0; i < userPages.Count; i++)
                {
                    if (userPages[i].Count < USER_INTERFACES_PER_PAGE)
                    {
                        userPages[i].Add(interfaceToAdd);
                        break;
                    }
                    else
                    {
                        userPages.Add(new List<UserInterface>());
                        continue;
                    }
                }
            }
            else // pages.Count <= 0
            {
                userPages.Add(new List<UserInterface>());
                userPages[0].Add(interfaceToAdd);
            }
        }

        private void UpdateUserPagesToInterfaces()
        {
            userPages.Clear();

            for (int i = 0; i < userInterfaces.Count; i++)
            {
                AddUser(userInterfaces[i].User);
            }
        }

        private void NextPage()
        {
            if (state == GameState.PickUser)
            {
                userPage++;
            }
            else if (state == GameState.NewUser)
            {
                if (projId >= projImgs.Count - 1)
                {
                    projId = 0;
                }
                else
                {
                    projId++;
                }
            }
        }
        private void PrevPage()
        {
            if (state == GameState.PickUser)
            {
                userPage--;
            }
            else if (state == GameState.NewUser)
            {
                if (projId <= 0)
                {
                    projId = projImgs.Count - 1;
                }
                else
                {
                    projId--;
                }
            }
        }

        #endregion

        #region Drawing

        private void DrawTitle(SpriteBatch spriteBatch)
        {
            spriteBatch.Draw(logoImg, logoRect, Color.White);
        }

        #endregion

        #region User Handling

        private void QueryUserDel(User user)
        {
            Popup.Show(Language.Translate("Are you sure you want to delete") + " \"" + user.Username + "\"?", true,
                new System.Action(() => DeleteUser(user)), false);
        }

        private void CreateUser()
        {
            if (state == GameState.NewUser)
            {
                List<User> users = User.LoadUsers();
                bool namesEqual = false;
                for (int i = 0; i <= users.Count - 1; i++)
                {
                    if (users[i].Username.ToLower() == usernameBox.Content.ToLower())
                    {
                        namesEqual = true;
                        break;
                    }
                }

                if (usernameBox.Content.Trim() == "")
                {
                    Popup.Show(
                        Language.Translate("Please enter a username containing at least" +
                        " one non-whitespace character."),
                        false, false);
                }
                else if (namesEqual)
                {
                    Popup.Show(Language.Translate("That user already exists."), false, false);
                }
                else
                {
                    User newU = createUser(usernameBox.Content, avatarColor, projImgs[projId]);
                    userInterfaces.Add(new UserInterface(newU, alienImg, alienEyeImg, graphics, usernameClicked,
                        delClicked, bigFont, trashImg, 0, 0, USERINTERFACE_WIDTH, USERINTERFACE_HEIGHT, content, lifeImg));
                    UpdateUserPagesToInterfaces();
                }

                usernameBox.Content = "";
            }
        }
        private void DeleteUser(User user)
        {
            bool keepGoing = true;
            for (int i = 0; i < userPages.Count && keepGoing; i++)
            {
                for (int j = 0; j < userPages[i].Count && keepGoing; j++)
                {
                    if (userPages[i][j].User.Id == user.Id)
                    {
                        userPages[i].RemoveAt(j);
                        keepGoing = false;
                    }
                }
            }

            for (int i = 0; i < userInterfaces.Count; i++)
            {
                if (userInterfaces[i].User.Id == user.Id)
                {
                    userInterfaces.RemoveAt(i);
                    break;
                }
            }

            delClicked?.Invoke(user);
        }

        #endregion

        #region Misc

        private void GoBackChecking()
        {
            if (state == GameState.Credits)
            {
                credits.Reset();
            }
            else if (state == GameState.NewUser)
            {
                usernameBox.Content = "";
            }
            else if (state == GameState.UserSettings)
            {
                settingsMenu.Reset(user);
            }
        }

        private void MuteToggle(bool value)
        {
            UniversalSettings settings = new UniversalSettings();
            settings.Muted = value;
            settings.Save();

            if (value) // value = true, so mute
            {
                Sound.StopAllSounds();
                Sound.StopAllMusic();
            }
        }
        private void CcToggle(bool value)
        {
            UniversalSettings settings = new UniversalSettings();
            settings.Captions = !value;
            Sound.ShowSubtitles = !value;
            settings.Save();
        }

        private void PositionButtons()
        {
            List<int> widths = new List<int>()
            {
                shopButton.Width,
                giftButton.Width,
                statButton.Width,
                organizeButton.Width,
                upgradeButton.Width,
                craftingButton.Width,
            };
            int widthSmall = widths.Max();
            achButton.Width = widthSmall * 2 + BUTTON_SPACING;
            giftButton.Width = statButton.Width = shopButton.Width = craftingButton.Width =
                upgradeButton.Width = organizeButton.Width = widthSmall;

            playButton.X = windowWidth / 2 - (playButton.Width / 2);

            startButton.X = windowWidth / 2 - (startButton.Width / 2);
            quitButton.X = windowWidth / 2 - (quitButton.Width / 2);

            resumeButton.X = windowWidth / 2 - (resumeButton.Width / 2);
            mainMenuButton.X = windowWidth / 2 - (mainMenuButton.Width / 2);

            //int craftShopWidth = craftingButton.Width + shopButton.Width + BUTTON_SPACING;
            //shopButton.X = windowWidth / 2 - (craftShopWidth / 2);
            //craftingButton.X = shopButton.X + BUTTON_SPACING + shopButton.Width;

            //int upgradeOrganizeWidth = upgradeButton.Width + organizeButton.Width + BUTTON_SPACING;
            //upgradeButton.X = windowWidth / 2 - (upgradeOrganizeWidth / 2);
            //organizeButton.X = upgradeButton.X + upgradeButton.Width + BUTTON_SPACING;

            //int achStatWidth = achButton.Width + statButton.Width + BUTTON_SPACING;
            //achButton.X = windowWidth / 2 - (achStatWidth / 2);
            //statButton.X = achButton.X + achButton.Width + BUTTON_SPACING;

            achButton.X = windowWidth / 2 - achButton.Width / 2;

            giftButton.X = organizeButton.X = craftingButton.X = windowWidth / 2 + BUTTON_SPACING / 2;
            statButton.X = upgradeButton.X = shopButton.X = giftButton.X - widthSmall - BUTTON_SPACING;

            //giftButton.X = windowWidth / 2 - giftButton.Width / 2;

            achButton.Y = playButton.Y + playButton.Height + BUTTON_SPACING;
            shopButton.Y = giftButton.Y = achButton.Y + achButton.Height + BUTTON_SPACING;
            organizeButton.Y = statButton.Y = shopButton.Y + shopButton.Height + BUTTON_SPACING;
            upgradeButton.Y = craftingButton.Y = statButton.Y + statButton.Height + BUTTON_SPACING;
        }

        //private void DisableAllButtons()
        //{
        //    startButton.Active = false;
        //    quitButton.Active = false;
        //    backButton.Active = false;
        //    controlsButton.Active = false;
        //    langButton.Active = false;
        //    createNewUserButton.Active = false;
        //    submitUserButton.Active = false;
        //    playButton.Active = false;
        //    achButton.Active = false;
        //    statButton.Active = false;
        //    shopButton.Active = false;
        //    craftingButton.Active = false;
        //    // Unfinished
        //}

        #endregion

        #endregion
    }

    public enum GameState
    {
        None,
        Playing,
        StartMenu,
        Paused,
        PickUser,
        NewUser,
        UserSettings,
        Shop,
        MainMenu,
        Achievements,
        Stats,
        Organize,
        Crafting,
        Upgrade,
        Story,
        SelectMission,
        Language,
        ChangeControls,
        Gifts,
        Credits,
    }
}