using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace OneCannonOneArmy
{
    public class Cannon
    {
        #region Fields & Properties

        Texture2D cannonExtImg;
        Rectangle cannonExtRect;
        Texture2D cannonTubeImg;
        Rectangle cannonTubeRect;

        Texture2D cannonBgImg;
        Rectangle cannonBgRect;
        Color[] bgPixels = new Color[1];

        const float EXT_TUBE_RATIO_HEIGHT = 0.3f; // Ratio of Ext:Tube = 3:10
        const float TUBE_EXT_RATIO_WIDTH = 0.33333f; // Tube:Ext = 1:3 (width)

        KeyboardState keyState;
        KeyboardState prevKeyState;

        public int Speed = 4;

        int windowWidth;
        int windowHeight;

        int XSPACING = 0;
        int YSPACING = 0;

        List<Projectile> projectiles = new List<Projectile>();
        Projectile loadedDrawProj;
        bool hasLoadedProjectile = true;

        bool slidingIn = true;
        bool waitSlide = false;
        Timer slideTimer;

        bool animatingDown = false;
        bool animatingUp = false;
        const int ANIMATION_AMOUNT = 4;

        int fullWidth;
        int fullHeight;

        public bool MachineCannon = false;

        event Action<ProjectileType> onLaunch;
        event Action projRemoved;

        Action<ProjectileType> changeProj;

        CannonSettings cannonSettings;

        Texture2D accuracyBeamImg;
        Rectangle accuracyBeamRect;
        const int ACCURACY_SIZE_MULTIPLIER = 20;

        #endregion

        #region Constructors

        public Cannon(int fullWidth, int fullHeight, int windowWidth, int windowHeight,
            GraphicsDevice graphics, Action<ProjectileType> changeProj)
        {
            this.changeProj = changeProj;

            cannonExtImg = Utilities.NormCannonBottomImg;
            cannonTubeImg = Utilities.NormCannonTubeImg;

            this.windowWidth = windowWidth;
            this.windowHeight = windowHeight;

            this.fullWidth = fullWidth;
            this.fullHeight = fullHeight;

            cannonExtRect = new Rectangle(0, 0, fullWidth, (int)(EXT_TUBE_RATIO_HEIGHT * fullHeight));
            cannonExtRect.X = windowWidth / 2 - (cannonExtRect.Width / 2);
            cannonTubeRect = new Rectangle(0, 0, (int)(fullWidth * TUBE_EXT_RATIO_WIDTH),
                (int)(fullHeight - (EXT_TUBE_RATIO_HEIGHT * fullHeight)));
            cannonTubeRect.Y = windowHeight - fullHeight;
            cannonTubeRect.X = cannonExtRect.X + (cannonExtRect.Width / 2 - (cannonTubeRect.Width / 2));
            cannonExtRect.Y = cannonTubeRect.Bottom;
            cannonBgImg = Utilities.RectImage;
            cannonBgRect = new Rectangle(cannonExtRect.X, cannonExtRect.Y, cannonExtRect.Width, cannonExtRect.Height);

            slideTimer = new Timer(0, TimerUnits.Milliseconds);

            loadedDrawProj = GameInfo.CreateProj(ProjectileType.Rock);
            loadedDrawProj.Type = ProjectileType.None;
            loadedDrawProj.SetPosition(cannonBgRect.X + cannonBgRect.Width / 2,
                        cannonExtRect.Y + YSPACING);

            XSPACING = cannonExtRect.Width / 2 - (loadedDrawProj.Width / 2);
            YSPACING = cannonExtRect.Height / 2;

            accuracyBeamImg = new Texture2D(graphics, 5, 1);
            Color[] beamData = new Color[5];
            beamData[0] = Color.White * 0.2f;
            beamData[1] = Color.White * 0.4f;
            beamData[2] = Color.White * 0.6f;
            beamData[3] = Color.White * 0.4f;
            beamData[4] = Color.White * 0.2f;
            accuracyBeamImg.SetData(beamData);
            accuracyBeamRect = new Rectangle(0, 0, 5, 1);
            accuracyBeamRect.X = cannonTubeRect.X + (cannonTubeRect.Width / 2 - (accuracyBeamRect.Width / 2));
            accuracyBeamRect.Y = cannonTubeRect.Y - accuracyBeamRect.Height;
        }

        #endregion

        #region Public Methods

        public void Initialize(ProjectileType firstProjectileType)
        {
            //ResetProjectilePos(firstProjectile);
            if (firstProjectileType != ProjectileType.None)
            {
                loadedDrawProj = GameInfo.CreateProj(firstProjectileType);
                loadedDrawProj.SetPosition(cannonBgRect.X + cannonBgRect.Width / 2,
                        cannonExtRect.Y + YSPACING);
                hasLoadedProjectile = true;
            }
            else
            {
                hasLoadedProjectile = false;
            }
        }

        public void Reset()
        {
            cannonExtRect.X = windowWidth / 2 - (cannonExtRect.Width / 2);
            cannonTubeRect.X = cannonExtRect.X + (cannonExtRect.Width / 2 - (cannonTubeRect.Width / 2));
            cannonBgRect.X = cannonExtRect.X;
            loadedDrawProj.SetPosition(windowWidth / 2, loadedDrawProj.Y);
            accuracyBeamRect.X = cannonTubeRect.X + (cannonTubeRect.Width / 2 - (accuracyBeamRect.Width / 2));

            // Reset the firing animation
            animatingDown = animatingUp = false;
            cannonTubeRect.Width = (int)(fullWidth * TUBE_EXT_RATIO_WIDTH);
            cannonTubeRect.Height = (int)(fullHeight - (EXT_TUBE_RATIO_HEIGHT * fullHeight));
            slidingIn = waitSlide = false;
        }

        public void ChangeSettings(CannonSettings settings)
        {
            cannonSettings = settings;
            slideTimer.WaitTime = GameInfo.ReloadSpeedEquation(settings.ReloadSpeed);
            Speed = settings.MoveSpeed;
            SetImgs(settings.CannonType);
            accuracyBeamRect.Height = settings.Accuracy * ACCURACY_SIZE_MULTIPLIER;
            accuracyBeamRect.Y = cannonTubeRect.Y - accuracyBeamRect.Height;
        }

        public void Update(ref List<Projectile> projectiles, GameTime gameTime, ProjectileType primaryProj, User user,
            bool removeProjectiles)
        {
            keyState = Keyboard.GetState();

            // Check and handle key presses
            if (keyState.IsKeyDown(Controls.MoveLeftKey) && !keyState.IsKeyDown(Controls.MoveRightKey)
                && !Controls.CtrlPressed(keyState) && cannonExtRect.Left > 0)
            {
                cannonExtRect.X -= Speed;
                cannonTubeRect.X -= Speed;
                cannonBgRect.X -= Speed;
                accuracyBeamRect.X -= Speed;
                loadedDrawProj.Move(Speed, Direction.Left);
            }
            else if (keyState.IsKeyDown(Controls.MoveRightKey) && !keyState.IsKeyDown(Controls.MoveLeftKey)
                && !Controls.CtrlPressed(keyState) && cannonExtRect.Right < windowWidth)
            {
                cannonExtRect.X += Speed;
                cannonTubeRect.X += Speed;
                cannonBgRect.X += Speed;
                accuracyBeamRect.X += Speed;
                loadedDrawProj.Move(Speed, Direction.Right);
            }

            // Gets the number of stored projectiles of the current primary type
            int primaryProjCount = GameInfo.CountOf(GameInfo.ProjListToTypesWithoutFlying(projectiles), primaryProj);

            if (primaryProjCount <= 0 && GameInfo.ProjListToTypesWithoutFlying(projectiles).Count > 0)
            {
                ProjectileType newPrimary = user.Hotbar[0];
                changeProj(newPrimary);
                primaryProj = newPrimary;
            }

            if (loadedDrawProj.Type != primaryProj && projectiles.Count > 0 && primaryProj != ProjectileType.None)
            {
                hasLoadedProjectile = true;
                loadedDrawProj = GameInfo.CreateProj(primaryProj);
                loadedDrawProj.SetPosition(cannonBgRect.X + cannonBgRect.Width / 2,
                        cannonExtRect.Y + YSPACING);
            }
            if (primaryProj == ProjectileType.None)
            {
                hasLoadedProjectile = false;
            }

            if (keyState.IsKeyDown(Controls.LaunchKey) && !Controls.CtrlPressed(keyState) && projectiles.Count > 0
                && !slidingIn && !waitSlide && !animatingDown && !animatingUp && primaryProjCount > 0)
            {
                if (!prevKeyState.IsKeyDown(Controls.LaunchKey) || MachineCannon)
                {
                    foreach (Projectile p in projectiles)
                    {
                        if (p.Type == primaryProj && !p.Flying)
                        {
                            if (p is ExplosiveProjectile)
                            {
                                ExplosiveProjectile e = p as ExplosiveProjectile;
                                if (e.Exploding)
                                {
                                    // Makes sure that projectiles that are exploding can't be
                                    // fired
                                    continue;
                                }
                            }
                            p.X = loadedDrawProj.X;
                            p.Y = cannonTubeRect.Y;
                            p.Damage += cannonSettings.Damage;
                            if (cannonSettings.EffectAdded != StatusEffect.None)
                            {
                                p.Effects.Add(cannonSettings.EffectAdded);
                            }
                            if (cannonSettings.Freezes)
                            {
                                p.Freezes = true;
                            }
                            p.Launch();
                            Sound.PlaySound(p.SoundWhenFired);
                            onLaunch?.Invoke(p.Type);
                            break;
                        }
                    }

                    //if (currentProjIndx + 1 >= projectiles.Count)
                    //{
                    //    // Here we make sure that adding 1 to the projectile index won't put it
                    //    // out of the range of the list
                    //    currentProjIndx--;
                    //}
                    //else
                    //{
                    //    currentProjIndx++;
                    //}

                    animatingDown = true;
                    waitSlide = true;
                    loadedDrawProj.SetPosition(cannonBgRect.X + cannonBgRect.Width / 2,
                        cannonExtRect.Y + YSPACING);
                }
            }
            else if ((projectiles.Count <= 0 || primaryProjCount <= 0) && keyState.IsKeyDown(Controls.LaunchKey) && !prevKeyState.IsKeyDown(Controls.LaunchKey))
            {
                Sound.PlaySound(Sounds.CannonClunk);
            }

            for (int i = 0; i < projectiles.Count; i++)
            {
                if (projectiles[i].Flying ||
                    (projectiles[i] is ExplosiveProjectile && (projectiles[i] as ExplosiveProjectile).Exploding))
                {
                    projectiles[i].Update(gameTime);
                }
                if (projectiles[i].Y < 0)
                {
                    if (removeProjectiles)
                    {
                        projectiles.RemoveAt(i);
                    }
                    projRemoved?.Invoke();
                    //ResetNextProjectile();
                    continue;
                }
                if (!projectiles[i].Active && removeProjectiles)
                {
                    //if (i < currentProjIndx)
                    //{
                    //    // The inactive projectile appears before the current projectile
                    //    // in the list, which will affect our updating logic.
                    //    // Thus, we must decrease the index and reset the projectile
                    //    ResetNextProjectile();
                    //}
                    projectiles.RemoveAt(i);
                }
            }

            if (waitSlide)
            {
                slideTimer.Update(gameTime);
                if (slideTimer.QueryWaitTime(gameTime))
                {
                    waitSlide = false;
                    slidingIn = true;
                    loadedDrawProj.SetPosition(cannonBgRect.X + loadedDrawProj.Width / 2, loadedDrawProj.Y);
                }
            }

            if (hasLoadedProjectile)
            {
                loadedDrawProj.Update(gameTime);
            }

            //if (currentProjIndx < projectiles.Count)
            //{
            //    if (!(slidingIn || projectiles[currentProjIndx].Flying))
            //    {
            //        ResetProjectilePos(projectiles[currentProjIndx]);
            //    }
            //}

            //if (!waitSlide && !slidingIn)
            //{
            //    ResetProjPos(projectiles[currentProjIndx]);
            //}

            if (slidingIn)
            {
                if (loadedDrawProj.X < cannonBgRect.X + cannonBgRect.Width / 2)
                {
                    loadedDrawProj.Move(2, Direction.Right);
                }
                else
                {
                    slidingIn = false;
                }
            }
            if (loadedDrawProj != null)
            {
                loadedDrawProj.Y = cannonExtRect.Y + YSPACING;
            }

            if (animatingDown)
            {
                cannonTubeRect.Width += ANIMATION_AMOUNT;
                cannonTubeRect.X -= ANIMATION_AMOUNT / 2;
                cannonTubeRect.Height -= ANIMATION_AMOUNT;
                cannonTubeRect.Y += ANIMATION_AMOUNT;
                if (cannonTubeRect.Width >= 0.5 * cannonExtRect.Width)
                {
                    animatingDown = false;
                    animatingUp = true;
                }
            }
            if (animatingUp)
            {
                cannonTubeRect.Width -= ANIMATION_AMOUNT;
                cannonTubeRect.X += ANIMATION_AMOUNT / 2;
                cannonTubeRect.Height += ANIMATION_AMOUNT;
                cannonTubeRect.Y -= ANIMATION_AMOUNT;
                if (cannonTubeRect.Width <= fullWidth * TUBE_EXT_RATIO_WIDTH)
                {
                    animatingUp = false;
                }
            }

            prevKeyState = keyState;
            this.projectiles = projectiles;
        }

        public void Draw(SpriteBatch spriteBatch, ref List<Projectile> projectiles)
        {
            spriteBatch.Draw(cannonBgImg, cannonBgRect, Color.White);
            if (cannonSettings.Accuracy > 0)
            {
                spriteBatch.Draw(accuracyBeamImg, accuracyBeamRect, Color.White);
            }
            if (!waitSlide && projectiles.WithoutFlying().Count > 0 && hasLoadedProjectile)
            {
                loadedDrawProj.Draw(spriteBatch, true);
            }
            for (int i = 0; i < projectiles.Count; i++)
            {
                if (projectiles[i].Launched && projectiles[i].Active)
                {
                    projectiles[i].Draw(spriteBatch, true);
                }
            }
            spriteBatch.Draw(cannonExtImg, cannonExtRect, Color.White);
            spriteBatch.Draw(cannonTubeImg, cannonTubeRect, Color.White);
        }

        public void AddOnLaunchHandler(Action<ProjectileType> a)
        {
            onLaunch += a;
        }
        public void AddProjRemovedHandler(Action handler)
        {
            projRemoved += handler;
        }

        #endregion

        #region Private Methods

        private void ResetProjectilePos(Projectile firstProjectile)
        {
            firstProjectile.SetPosition(cannonExtRect.X + XSPACING, cannonExtRect.Y + YSPACING);
        }

        private void PrepareSliding(Projectile projectile)
        {
            projectile.SetPosition(cannonExtRect.X + projectile.Width / 2, cannonExtRect.Y + YSPACING);
        }

        private void SetImgs(CannonType cannonType)
        {
            cannonExtImg = Utilities.GetBottomOfCannon(cannonType);
            cannonTubeImg = Utilities.GetTubeOfCannon(cannonType);
            //switch (cannonType)
            //{
            //    case CannonType.Normal:
            //        cannonExtImg = Utilities.NormCannonBottomImg;
            //        cannonTubeImg = Utilities.NormCannonTubeImg;
            //        break;
            //    case CannonType.Bronze:
            //        cannonExtImg = Utilities.BronzeCannonBottomImg;
            //        cannonTubeImg = Utilities.BronzeCannonTubeImg;
            //        break;
            //    case CannonType.Silver:
            //        cannonExtImg = Utilities.SilverCannonBottomImg;
            //        cannonTubeImg = Utilities.SilverCannonTubeImg;
            //        break;
            //    case CannonType.Gold:
            //        cannonExtImg = Utilities.GoldCannonBottomImg;
            //        cannonTubeImg = Utilities.GoldCannonTubeImg;
            //        break;
            //    case CannonType.Master:
            //        cannonExtImg = Utilities.MasterCannonBottomImg;
            //        cannonTubeImg = Utilities.MasterCannonTubeImg;
            //        break;
            //}
        }

        //private void ResetNextProjectile()
        //{
        //    bool go = true;
        //    foreach (Projectile p in projectiles)
        //    {
        //        if (p.Flying)
        //        {
        //            go = false;
        //        }
        //    }
        //    if (go)
        //    {
        //        currentProjIndx--;
        //        if (projectiles[currentProjIndx].Flying)
        //        {
        //            currentProjIndx = 0;
        //            while (projectiles[currentProjIndx].Flying)
        //            {
        //                currentProjIndx++;
        //            }
        //        }
        //        PrepareSliding(projectiles[currentProjIndx]);
        //    }
        //}

        #endregion
    }

    public delegate void UpgradeCannon(CannonStats stat, int value, int cost);

    public class CannonUpgradeInterface
    {
        #region Fields & Properties

        public CannonStats Stat;

        Texture2D bgImg;
        Rectangle bgRect;

        Texture2D icon;
        Rectangle iconRect;

        MenuButton upgradeButton;

        SpriteFont font;
        int currentValue;
        string origValueString;
        string valueString;
        Vector2 valuePos;
        CannonStats stat;

        UpgradeCannon upgrade;

        const int SPACING = 5;

        int cost;
        Vector2 costPos;
        string costString = "";

        public bool HoveringOnItem = false;

        public int X
        {
            get
            {
                return bgRect.X;
            }
            set
            {
                bgRect.X = value;
                iconRect.X = bgRect.X + (bgRect.Width / 2 - (iconRect.Width / 2));
                valuePos.X = bgRect.X + (bgRect.Width / 2 - (font.MeasureString(valueString + ": 0").X / 2));
                upgradeButton.X = bgRect.X + (bgRect.Width / 2 - (upgradeButton.Width / 2));
            }
        }
        public int Y
        {
            get
            {
                return bgRect.Y;
            }
            set
            {
                bgRect.Y = value;
                iconRect.Y = bgRect.Y + SPACING;
                valuePos.Y = iconRect.Bottom + SPACING;
                upgradeButton.Y = (int)valuePos.Y + SPACING * 3;
                costPos.Y = upgradeButton.Y + upgradeButton.Height + SPACING;
            }
        }
        public int Width
        {
            get
            {
                return bgRect.Width;
            }
        }
        public int Height
        {
            get
            {
                return bgRect.Height;
            }
        }

        GraphicsDevice graphics;

        #endregion

        #region Constructors

        public CannonUpgradeInterface(UpgradeCannon upgrade, Texture2D icon, string valueString, GraphicsDevice graphics,
            int x, int y, int width, int height, SpriteFont font, CannonStats stat)
        {
            Stat = stat;

            this.icon = icon;
            this.upgrade = upgrade;
            this.font = font;
            this.stat = stat;
            this.graphics = graphics;
            origValueString = valueString;

            bgImg = new Texture2D(graphics, width, height);
            bgImg = DrawHelper.AddBorder(bgImg, 3, Color.Gray, Color.LightGray);
            bgRect = new Rectangle(x, y, width, height);

            iconRect = new Rectangle(0, bgRect.Y + SPACING, bgRect.Width - (SPACING * 2), 0);
            iconRect.Height = iconRect.Width;
            iconRect.X = bgRect.X + (bgRect.Width / 2 - (iconRect.Width / 2));

            this.valueString = LanguageTranslator.Translate(valueString);

            upgradeButton = new MenuButton(Upgrade, LanguageTranslator.Translate("Upgrade") + " " + valueString, 0,
                0, true, font, graphics);
            Position();

            costPos = new Vector2(0, upgradeButton.Y + upgradeButton.Height + SPACING);
        }

        #endregion

        #region Public Methods

        public void Update(CannonSettings settings, User user)
        {
            bool atMax = user.CannonSettings.GetValueOfStat(stat) == GameInfo.MaxStats[stat];
            upgradeButton.Active = (user.Coins >= cost) && !atMax;
            upgradeButton.Text = atMax ? LanguageTranslator.Translate("Max Level") : LanguageTranslator.Translate("Upgrade") + " " + valueString;
            upgradeButton.Update();
            currentValue = settings.GetValueOfStat(stat);

            cost = GameInfo.GetCostOfStat(stat, settings.GetValueOfStat(stat));
            costString = atMax ? LanguageTranslator.Translate("Max Level") : LanguageTranslator.Translate("Cost") + ": " + cost;
            costPos.X = bgRect.X + (bgRect.Width / 2 - (font.MeasureString(costString).X / 2));

            Rectangle mouseRect = new Rectangle(Mouse.GetState().X, Mouse.GetState().Y, 1, 1);
            HoveringOnItem = mouseRect.Intersects(iconRect);
        }

        public void Draw(SpriteBatch spriteBatch)
        {
            spriteBatch.Draw(bgImg, bgRect, Color.DarkGray);
            spriteBatch.Draw(icon, iconRect, Color.Black);
            spriteBatch.DrawString(font, valueString + ": " + currentValue, valuePos, Color.LightGray);
            upgradeButton.Draw(spriteBatch);
            spriteBatch.DrawString(font, costString, costPos, Color.Gold);
        }

        public CannonUpgradeInterface Clone()
        {
            return new CannonUpgradeInterface(upgrade, icon, valueString, graphics, bgRect.X,
                bgRect.Y, bgRect.Width, bgRect.Height, font, stat);
        }

        public List<MenuButton> GetButtons()
        {
            return new List<MenuButton>()
            {
                upgradeButton,
            };
        }

        public void LangChanged()
        {
            valueString = LanguageTranslator.Translate(origValueString);

            Position();
        }

        #endregion

        #region Private Methods

        private void Upgrade()
        {
            upgrade(stat, 1, cost);
        }

        private void Position()
        {
            upgradeButton.X = bgRect.X + (bgRect.Width / 2 - (upgradeButton.Width / 2));
            upgradeButton.Y = (int)valuePos.Y + SPACING * 3;
            valuePos = new Vector2(bgRect.X + (bgRect.Width / 2 - (font.MeasureString(valueString + ": 0").X / 2)),
                iconRect.Bottom + SPACING);
        }

        #endregion
    }

    public class CannonUpgradeMenu
    {
        #region Fields & Properties

        int page;
        List<CannonUpgradeInterface> interfaces = new List<CannonUpgradeInterface>();
        List<List<CannonUpgradeInterface>> pages = new List<List<CannonUpgradeInterface>>();

        int windowWidth;
        int windowHeight;

        const int ITEMS_PER_ROW = 4;
        const int ROWS = 2;

        const int SLIDE_SPEED = 15;
        bool slidingOver;
        int slideOffset;
        int slideSpd;
        int transitionPage;

        MenuButton nextButton;
        MenuButton prevButton;

        const int INT_WIDTH = 175;
        const int INT_HEIGHT = 240;
        int X_OFFSET = 0;
        const int Y_OFFSET = 150;

        const int SPACING = 10;

        StatInfoHover statInfoHover;

        #endregion

        #region Constructors

        public CannonUpgradeMenu(int windowWidth, int windowHeight, GraphicsDevice graphics, Texture2D arrowImg,
            List<CannonStats> stats, UpgradeCannon upgrade, List<Texture2D> statIcons, SpriteFont font)
        {
            this.windowWidth = windowWidth;
            this.windowHeight = windowHeight;

            for (int i = 0; i < stats.Count; i++)
            {
                interfaces.Add(new CannonUpgradeInterface(upgrade, statIcons[i], stats[i].ToString().AddSpaces(),
                    graphics, 0, 0, INT_WIDTH, INT_HEIGHT, font, stats[i]));
            }
            UpdatePagesToInterfaces();

            nextButton = new MenuButton(NextPage, 0, 0, true, graphics, arrowImg);
            nextButton.X = windowWidth - SPACING - nextButton.Width;
            nextButton.Y = windowHeight - SPACING - nextButton.Height;
            prevButton = new MenuButton(PrevPage, SPACING, nextButton.Y, true, graphics, arrowImg);

            X_OFFSET = (windowWidth - (interfaces[0].Width * ITEMS_PER_ROW + SPACING * ITEMS_PER_ROW)) / 2;

            statInfoHover = new StatInfoHover(CannonStats.Accuracy, font, graphics, windowWidth);
        }

        #endregion

        #region Public Methods

        public void Update(CannonSettings settings, User user)
        {
            if (slideOffset + X_OFFSET >= windowWidth || slideOffset + (X_OFFSET * -1) <= windowWidth * -1)
            {
                slideOffset = 0;
                slidingOver = false;
                page = transitionPage;
            }

            if (page <= pages.Count - 1)
            {
                if (slidingOver)
                {
                    int offset = 0;
                    if (slideSpd < 0)
                    {
                        offset = (interfaces[0].Width + SPACING) * 5 + X_OFFSET;
                    }
                    else
                    {
                        offset = ((interfaces[0].Width + SPACING) * 5 + X_OFFSET) * -1;
                    }
                    UpdatePositions(transitionPage, offset, settings, user);
                }

                UpdatePositions(page, 0, settings, user);
            }

            for (int i = 0; i <= pages.Count - 1; i++)
            {
                if (pages[i].Count <= 0)
                {
                    pages.RemoveAt(i);
                }
            }

            //if (pages.Count <= 1)
            //{
            //    // Both buttons should be disabled, as there is only 1 page
            //    // This must be tested for first, otherwise, since the page always starts equal to 0, the below criteria will be met
            //    // and the game will act like there are 2 pages when there is only 1
            //    nextButton.Active = false;
            //    prevButton.Active = false;
            //    // Since there is only one page, we have to set the page integer to 0
            //    page = 0;
            //}
            //else if (page == 0)
            //{
            //    // There is another page, but we are on the first. Therefore, we cannot go backwards, so we'll disable the backwards button
            //    nextButton.Active = true;
            //    prevButton.Active = false;
            //}
            //else if (page + 1 == pages.Count)
            //{
            //    // We are on the last page, so the forward button must be disabled
            //    nextButton.Active = false;
            //    prevButton.Active = true;
            //}
            //else
            //{
            //    // We must be somewhere in the middle, so both buttons should be enabled
            //    nextButton.Active = true;
            //    prevButton.Active = true;
            //}

            //nextButton.Update();
            //prevButton.Update();

            if (slidingOver)
            {
                slideOffset += slideSpd;
            }

            bool active = false;
            for (int i = 0; i < pages[page].Count; i++)
            {
                if (pages[page][i].HoveringOnItem)
                {
                    active = true;
                    statInfoHover.ResetStat(pages[page][i].Stat);
                }
            }
            statInfoHover.Active = active;
            if (statInfoHover.Active)
            {
                statInfoHover.Update();
            }
        }

        public void Draw(SpriteBatch spriteBatch)
        {
            foreach (CannonUpgradeInterface cu in pages[page])
            {
                cu.Draw(spriteBatch);
            }
            if (statInfoHover.Active)
            {
                statInfoHover.Draw(spriteBatch);
            }

            //nextButton.Draw(spriteBatch);
            //prevButton.Draw(spriteBatch, null, SpriteEffects.FlipHorizontally);
        }

        public List<MenuButton> GetButtons()
        {
            List<MenuButton> returnVal = new List<MenuButton>();
            foreach (CannonUpgradeInterface cu in pages[page])
            {
                returnVal.AddRange(cu.GetButtons());
            }
            returnVal.Add(nextButton);
            returnVal.Add(prevButton);
            return returnVal;
        }

        public void LangChanged()
        {
            for (int i = 0; i < interfaces.Count; i++)
            {
                interfaces[i].LangChanged();
            }
            UpdatePagesToInterfaces();
        }

        #endregion

        #region Private Methods

        private void UpdatePositions(int page, int xOffset, CannonSettings settings, User user)
        {
            int row = 0;
            int lastX = X_OFFSET;
            int lastY = Y_OFFSET;
            int interfacesInCurrentRow = 0;
            foreach (CannonUpgradeInterface cu in pages[page])
            {
                cu.X = lastX + slideOffset + xOffset;
                lastX += cu.Width + SPACING;
                if (interfacesInCurrentRow >= ITEMS_PER_ROW)
                {
                    row++;
                    lastX = X_OFFSET;
                    cu.X = lastX + slideOffset + xOffset;
                    lastX += cu.Width + SPACING;
                    lastY += cu.Height + SPACING;
                    interfacesInCurrentRow = 0;
                }

                cu.Y = lastY;
                interfacesInCurrentRow++;

                cu.Update(settings, user);
            }

            lastX = X_OFFSET + slideOffset;
            lastY = Y_OFFSET;
            interfacesInCurrentRow = 0;
            row = 0;
        }

        private void NextPage()
        {
            transitionPage = page + 1;
            slidingOver = true;
            slideSpd = SLIDE_SPEED * -1;
        }
        private void PrevPage()
        {
            transitionPage = page - 1;
            slidingOver = true;
            slideSpd = SLIDE_SPEED;
        }

        private void AddItem(CannonUpgradeInterface interfaceToAdd)
        {
            // This gets the currently used page and adds to it. 
            // Unfortunately, we need to add extra code to check if the latest page is full.
            if (pages.Count > 0)
            {
                for (int i = 0; i < pages.Count; i++)
                {
                    if (pages[i].Count < ITEMS_PER_ROW * ROWS)
                    {
                        pages[i].Add(interfaceToAdd);
                        break;
                    }
                    else
                    {
                        pages.Add(new List<CannonUpgradeInterface>());
                        continue;
                    }
                }
            }
            else // pages.Count <= 0
            {
                pages.Add(new List<CannonUpgradeInterface>());
                pages[0].Add(interfaceToAdd);
            }
        }

        private void UpdatePagesToInterfaces()
        {
            pages.Clear();

            for (int i = 0; i < interfaces.Count; i++)
            {
                AddItem(interfaces[i].Clone());
            }
        }

        #endregion
    }

    [Serializable]
    public struct CannonSettings
    {
        public int MaxHealth;
        public CannonType CannonType;
        public int Damage;
        public int ReloadSpeed;
        public int MoveSpeed;
        public int Accuracy;
        public int RapidFire;
        public StatusEffect EffectAdded;
        public bool Freezes;

        public CannonSettings(int maxHealth, CannonType cannonType, int damage, int reloadSpeed, int moveSpeed,
            int accuracy, StatusEffect effectAdded, bool freezes, int rapidFire)
        {
            MaxHealth = maxHealth;
            CannonType = cannonType;
            Damage = damage;
            ReloadSpeed = reloadSpeed;
            MoveSpeed = moveSpeed;
            Accuracy = accuracy;
            EffectAdded = effectAdded;
            Freezes = freezes;
            RapidFire = rapidFire;
        }

        public int GetValueOfStat(CannonStats stat)
        {
            switch (stat)
            {
                case CannonStats.Damage:
                    return Damage;
                case CannonStats.Accuracy:
                    return Accuracy;
                case CannonStats.Health:
                    return MaxHealth;
                case CannonStats.MoveSpeed:
                    return MoveSpeed;
                case CannonStats.ReloadSpeed:
                    return ReloadSpeed;
                case CannonStats.RapidFire:
                    return RapidFire;
                default:
                    return 0;
            }
        }

        public void Replace(CannonSettings newValue)
        {
            this = newValue;
        }

        public void SetValueOfStat(CannonStats stat, int value)
        {
            switch (stat)
            {
                case CannonStats.Damage:
                    Damage = value;
                    break;
                case CannonStats.Accuracy:
                    Accuracy = value;
                    break;
                case CannonStats.Health:
                    MaxHealth = value;
                    break;
                case CannonStats.MoveSpeed:
                    MoveSpeed = value;
                    break;
                case CannonStats.ReloadSpeed:
                    ReloadSpeed = value;
                    break;
                case CannonStats.RapidFire:
                    RapidFire = value;
                    break;
            }
        }

        #region Cannons

        public static CannonSettings SettingsForType(CannonType t)
        {
            switch (t)
            {
                case CannonType.Normal:
                    return NormalCannon;
                case CannonType.Bronze:
                    return BronzeCannon;
                case CannonType.Silver:
                    return SilverCannon;
                case CannonType.Gold:
                    return GoldCannon;
                case CannonType.Elite:
                    return EliteCannon;
                case CannonType.Inferno:
                    return InfernoCannon;
                case CannonType.Frozen:
                    return FrozenCannon;
            }
            return new CannonSettings();
        }

        public static CannonSettings NormalCannon
        {
            get
            {
                return new CannonSettings(GameInfo.STARTING_MAX_HEALTH, CannonType.Normal,
                    0, GameInfo.STARTING_CANNON_RELOAD_SPD, GameInfo.STARTING_CANNON_SPD, GameInfo.STARTING_CANNON_ACCURACY,
                    StatusEffect.None, false, 0);
            }
        }
        public static CannonSettings BronzeCannon
        {
            get
            {
                return new CannonSettings(GameInfo.STARTING_MAX_HEALTH, CannonType.Bronze, 2, GameInfo.STARTING_CANNON_RELOAD_SPD,
                    GameInfo.STARTING_CANNON_SPD, 5, StatusEffect.None, false, 0);
            }
        }
        public static CannonSettings SilverCannon
        {
            get
            {
                return new CannonSettings(GameInfo.STARTING_MAX_HEALTH, CannonType.Silver, 4, GameInfo.STARTING_CANNON_RELOAD_SPD - 1,
                    GameInfo.STARTING_CANNON_SPD + 1, 7, StatusEffect.None, false, 0);
            }
        }
        public static CannonSettings GoldCannon
        {
            get
            {
                return new CannonSettings(GameInfo.STARTING_MAX_HEALTH, CannonType.Gold, 6, GameInfo.STARTING_CANNON_RELOAD_SPD - 1,
                    GameInfo.STARTING_CANNON_SPD + 1, 10, StatusEffect.None, false, 0);
            }
        }
        public static CannonSettings EliteCannon
        {
            get
            {
                return new CannonSettings(GameInfo.STARTING_MAX_HEALTH, CannonType.Elite, 10,
                    GameInfo.STARTING_CANNON_RELOAD_SPD - 2, GameInfo.STARTING_CANNON_SPD + 2, 15, StatusEffect.None, false, 0);
            }
        }
        public static CannonSettings InfernoCannon
        {
            get
            {
                return new CannonSettings(GameInfo.STARTING_MAX_HEALTH, CannonType.Inferno, 4,
                    GameInfo.STARTING_CANNON_RELOAD_SPD - 1, GameInfo.STARTING_CANNON_SPD + 1, 7, StatusEffect.Fire, false, 0);
            }
        }
        public static CannonSettings FrozenCannon
        {
            get
            {
                return new CannonSettings(GameInfo.STARTING_MAX_HEALTH, CannonType.Frozen, 4,
                    GameInfo.STARTING_CANNON_RELOAD_SPD - 1, GameInfo.STARTING_CANNON_SPD + 1, 7, StatusEffect.None, true, 0);
            }
        }

        #endregion

        public static bool operator ==(CannonSettings c1, CannonSettings c2)
        {
            return c1.Accuracy == c2.Accuracy &&
                c1.CannonType == c2.CannonType &&
                c1.Damage == c2.Damage &&
                c1.EffectAdded == c2.EffectAdded &&
                c1.Freezes == c2.Freezes &&
                c1.MaxHealth == c2.MaxHealth &&
                c1.MoveSpeed == c2.MoveSpeed &&
                c1.ReloadSpeed == c2.ReloadSpeed;
        }
        public static bool operator !=(CannonSettings c1, CannonSettings c2)
        {
            return !(c1 == c2);
        }
    }

    public enum CannonType
    {
        Normal, // Black
        Bronze, // Bronze
        Silver, // Silver
        Gold, // Gold
        Elite, // Purple
        Inferno, // Orange
        Frozen, // Cyan
    }

    public enum CannonStats
    {
        Health,
        CannonType,
        Damage,
        ReloadSpeed,
        MoveSpeed,
        Accuracy,
        Effect,
        RapidFire,
    }
}
