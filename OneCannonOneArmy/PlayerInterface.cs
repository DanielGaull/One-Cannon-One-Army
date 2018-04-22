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
    public class PlayerInterface
    {
        #region Fields & Properties

        Texture2D bgImg;
        Rectangle bgBottomRect;
        Rectangle bgTopRect;
        public const int BOTTOM_HEIGHT = 150;
        public const int TOP_HEIGHT = 80;

        Texture2D highlightedImg;
        Rectangle highlightedRect;

        MenuButton sweeperButton;
        Sweeper sweeper;

        ToggleButton rapidFireButton;
        public bool AllowRapidFire
        {
            get
            {
                return rapidFireButton.Active;
            }
            set
            {
                rapidFireButton.Active = value;
                if (value == false)
                {
                    rapidFireButton.Toggled = true;
                }
            }
        }
        public bool RapidFire
        {
            get
            {
                return !rapidFireButton.Toggled;
            }
            set
            {
                rapidFireButton.Toggled = !value;
            }
        }

        KeyboardState keyState;
        KeyboardState prevKeyState;

        Texture2D lifeImg;
        List<Rectangle> lifeRects = new List<Rectangle>();
        List<Color> lifeColors = new List<Color>();
        const int LIFE_SIZE = 20;
        const int LIFE_SIZE_BIG = 50;

        Rectangle coinDisplayRect;
        Coin displayCoin;
        const int COIN_DISP_WIDTH = 200;
        const int COIN_DISP_HEIGHT = 50;
        const int COIN_SIZE = 30;

        SpriteFont bigFont;
        SpriteFont mediumFont;

        const int SPACING = 6;
        const int LARGE_SPACING = 80;

        public Point CoinDisplayPoint
        {
            get
            {
                return new Point(displayCoin.X, displayCoin.Y);
            }
        }
        public Rectangle CoinDisplayRect
        {
            get
            {
                return displayCoin.Rectangle;
            }
        }

        Rectangle toolTipRect = new Rectangle();
        string toolTip = "";
        Vector2 toolTipLoc;
        const string MACH_TOOLTIP = "Toggle Rapid Fire";
        const string SWEEP_TOOLTIP = "Sweep (Collect All Alien Drops)";
        const string UPGRADE_NOT_PURCHASED = "(Upgrade Not Purchased)";
        bool showingToolTip = false;

        FillBar healthBar;
        const int FILLBAR_WIDTH = 300;
        const int HEALTHBAR_HEIGHT = 60;

        string username;
        Vector2 usernamePos;
        public Point UsernamePoint
        {
            get
            {
                return new Point((int)usernamePos.X, (int)usernamePos.Y);
            }
        }
        public Rectangle UsernameRect
        {
            get
            {
                return new Rectangle((int)usernamePos.X, (int)usernamePos.Y, (int)bigFont.MeasureString(username).X,
                    (int)bigFont.MeasureString(username).Y);
            }
        }

        List<Rectangle> projRects = new List<Rectangle>(GameInfo.HOTBAR_SLOTS);
        List<Texture2D> projImgs = new List<Texture2D>(GameInfo.HOTBAR_SLOTS);
        List<Vector2> projAmountPositions = new List<Vector2>(GameInfo.HOTBAR_SLOTS);
        List<string> projAmountStrings = new List<string>(GameInfo.HOTBAR_SLOTS);
        List<int> projAmounts = new List<int>(GameInfo.HOTBAR_SLOTS);
        List<ProjectileType> projs = new List<ProjectileType>(GameInfo.HOTBAR_SLOTS);
        const int PROJ_SIZE = 110;

        public ProjectileType PrimaryProj = ProjectileType.Rock;

        int windowWidth;
        int windowHeight;

        public float MaxHealth
        {
            get
            {
                return healthBar.MaxValue;
            }
            set
            {
                healthBar.MaxValue = value;
            }
        }
        public float Health
        {
            get
            {
                return healthBar.Value;
            }
            set
            {
                healthBar.Value = value;
            }
        }

        FillBar progressBar;
        const int PROGRESSBAR_WIDTH = 75;
        const int PROGRESSBAR_HEIGHT = 10;

        public bool Sweeping
        {
            get
            {
                return sweeper.Sweeping;
            }
        }

        #endregion

        #region Constructors

        public PlayerInterface(int windowWidth, int windowHeight, Texture2D sweepImg, GraphicsDevice graphics, SpriteFont bigFont,
            SpriteFont mediumFont, int healthMax, Texture2D lifeImg, Texture2D rapidFireIcon, Texture2D crossImg)
        {
            this.windowWidth = windowWidth;
            this.windowHeight = windowHeight;
            this.lifeImg = lifeImg;

            bgImg = Utilities.RectImage;
            bgBottomRect = new Rectangle(0, windowHeight - BOTTOM_HEIGHT, windowWidth, BOTTOM_HEIGHT);
            bgTopRect = new Rectangle(0, 0, windowWidth, TOP_HEIGHT);

            sweeper = new Sweeper(graphics, windowWidth, windowHeight - BOTTOM_HEIGHT);

            sweeperButton = new MenuButton(new System.Action(sweeper.Sweep), 0, 0, true, graphics, sweepImg);
            sweeperButton.X = bgBottomRect.Width - sweeperButton.Width - SPACING;
            sweeperButton.Y = bgBottomRect.Y + (bgBottomRect.Height / 2 - (sweeperButton.Height / 2));

            rapidFireButton = new ToggleButton(rapidFireIcon, crossImg, graphics, 0, sweeperButton.Y, sweeperButton.Width,
                sweeperButton.Height);
            rapidFireButton.Toggled = true;
            rapidFireButton.X = sweeperButton.X - rapidFireIcon.Width - SPACING;

            coinDisplayRect = new Rectangle(windowWidth - COIN_DISP_WIDTH - SPACING,
                TOP_HEIGHT / 2 - (COIN_DISP_HEIGHT / 2), COIN_DISP_WIDTH, COIN_DISP_HEIGHT);
            displayCoin = new Coin(Utilities.CoinImage, coinDisplayRect.X + SPACING,
                coinDisplayRect.Y + (coinDisplayRect.Height / 2 - (COIN_SIZE / 2)), COIN_SIZE, COIN_SIZE, 0);

            this.bigFont = bigFont;
            this.mediumFont = mediumFont;

            healthBar = new FillBar(healthMax, SPACING, SPACING, FILLBAR_WIDTH, HEALTHBAR_HEIGHT, healthMax,
                bigFont);

            //xpBar = new FillBar(0, healthBar.X, healthBar.Y + healthBar.Height + SPACING, FILLBAR_WIDTH, XPBAR_HEIGHT,
            //    GameInfo.STARTING_XP_MAX, smallFont);

            projRects.AddRange(Enumerable.Repeat(new Rectangle(), GameInfo.HOTBAR_SLOTS));
            projImgs.AddRange(Enumerable.Repeat(Utilities.RectImage, GameInfo.HOTBAR_SLOTS));
            projAmountStrings.AddRange(Enumerable.Repeat("", GameInfo.HOTBAR_SLOTS));
            projAmountPositions.AddRange(Enumerable.Repeat(new Vector2(), GameInfo.HOTBAR_SLOTS));
            projAmounts.AddRange(Enumerable.Repeat(0, GameInfo.HOTBAR_SLOTS));
            projs.AddRange(Enumerable.Repeat(ProjectileType.None, GameInfo.HOTBAR_SLOTS));

            highlightedImg = new Texture2D(graphics, PROJ_SIZE, PROJ_SIZE);
            Color[] data = new Color[PROJ_SIZE * PROJ_SIZE];
            highlightedImg.SetData(data);
            DrawHelper.AddBorder(highlightedImg, 2, Color.Lime, Color.Transparent);
            highlightedRect = new Rectangle(0, 0, PROJ_SIZE, PROJ_SIZE);

            usernamePos = new Vector2();

            progressBar = new FillBar(0, 0, (int)(usernamePos.Y + bigFont.MeasureString("A").Y - (SPACING * 1.5f)),
                PROGRESSBAR_WIDTH, PROGRESSBAR_HEIGHT, 1, null);
            progressBar.ShowFraction = false;
            progressBar.X = windowWidth / 2 - (progressBar.Width / 2);
        }

        #endregion

        #region Public Methods

        public void Sweep()
        {
            sweeper.Sweep();
        }

        public void Initialize(User user)
        {
            for (int i = 0; i <= user.Hotbar.Count - 1; i++)
            {
                healthBar.Value = healthBar.MaxValue;
                projImgs[i] = Utilities.GetIconOf(user.Hotbar[i]);
                projAmountStrings[i] = "x" + user.AmountOfProj(user.Hotbar[i]).ToString();
                if (i == 0)
                {
                    projRects[i] = new Rectangle(bgBottomRect.X + SPACING,
                        bgBottomRect.Y + (bgBottomRect.Height / 2 - (PROJ_SIZE / 2)), PROJ_SIZE, PROJ_SIZE);
                }
                else
                {
                    projRects[i] = new Rectangle(projRects[i - 1].Right + (int)(SPACING * 3.5f), projRects[i - 1].Y,
                        PROJ_SIZE, PROJ_SIZE);
                }
                projAmountPositions[i] = new Vector2(projRects[i].Right - (mediumFont.MeasureString(projAmountStrings[i]).X / 2),
                    projRects[i].Bottom - (mediumFont.MeasureString(projAmountStrings[i]).Y / 2));
                projs = user.Hotbar;
            }

            PrimaryProj = user.Hotbar[0];

            username = user.Username;
            usernamePos.X = windowWidth / 2 - (bigFont.MeasureString(username).X / 2);

            highlightedRect = projRects[0];

            InitializeLifeRects(user, true);
        }

        public void Update(ref List<Item> items, ref User user, List<Projectile> projectiles, bool needSweep,
            bool restrictActions)
        {
            keyState = Keyboard.GetState();

            List<ProjectileType> currentHotbar = user.Hotbar;
            for (int i = 0; i <= currentHotbar.Count - 1; i++)
            {
                if (GameInfo.CountOf(GameInfo.ProjListToTypesWithoutFlying(projectiles), currentHotbar[i]) == 0)
                {
                    // Resets the hotbar if there are no available projectiles of a certain type
                    List<ProjectileType> availableTypes = GameInfo.ProjListToTypesWithoutFlying(projectiles);
                    // Make sure that we can't include any projectiles that are already on our "hotbar"
                    availableTypes = availableTypes.Except(availableTypes).ToList();
                    currentHotbar[i] = GameInfo.GetRandomProjType(availableTypes);
                }
                projAmountStrings[i] = "x" + AmountOf(currentHotbar[i], projectiles).ToString();
                projAmounts[i] = AmountOf(currentHotbar[i], projectiles);

                projImgs[i] = Utilities.GetIconOf(currentHotbar[i]);
            }
            if (user.Hotbar != currentHotbar)
            {
                user.SetHotbar(currentHotbar);
            }

            projs = user.Hotbar;

            if (!restrictActions)
            {
                Rectangle mouseRect = new Rectangle(Mouse.GetState().X, Mouse.GetState().Y, 1, 1);
                for (int i = 0; i <= projRects.Count - 1; i++)
                {
                    if (((mouseRect.Intersects(projRects[i]) && Mouse.GetState().LeftButton == ButtonState.Pressed) ||
                        (keyState.IsKeyDown(KeyForHotbarSlot(i + 1)) && prevKeyState.IsKeyUp(KeyForHotbarSlot(i + 1)))) &&
                        projAmounts[i] > 0)
                    {
                        PrimaryProj = user.Hotbar[i];
                        SetHighlightSquare(PrimaryProj);
                    }
                }
                if (mouseRect.Intersects(rapidFireButton.Button.DrawRectangle))
                {
                    toolTip = LanguageTranslator.Translate(MACH_TOOLTIP);
                    toolTipRect = new Rectangle(mouseRect.X - (int)mediumFont.MeasureString(toolTip).X,
                        mouseRect.Y, (int)mediumFont.MeasureString(toolTip).X + SPACING,
                        (int)mediumFont.MeasureString(toolTip).Y + SPACING);
                    toolTipLoc = new Vector2(toolTipRect.X + SPACING / 2, toolTipRect.Y + SPACING / 2);
                    showingToolTip = true;
                }
                else if (mouseRect.Intersects(sweeperButton.DrawRectangle))
                {
                    toolTip = LanguageTranslator.Translate(SWEEP_TOOLTIP);
                    toolTipRect = new Rectangle(mouseRect.X - (int)mediumFont.MeasureString(toolTip).X,
                        mouseRect.Y, (int)mediumFont.MeasureString(toolTip).X + SPACING,
                        (int)mediumFont.MeasureString(toolTip).Y + SPACING);
                    toolTipLoc = new Vector2(toolTipRect.X + SPACING / 2, toolTipRect.Y + SPACING / 2);
                    showingToolTip = true;
                }
                else
                {
                    showingToolTip = false;
                }

                if (keyState.IsKeyDown(Controls.ToggleRapidFireKey) && prevKeyState.IsKeyUp(Controls.ToggleRapidFireKey))
                {
                    rapidFireButton.ClickWithSound();
                }
                rapidFireButton.Update();
            }
            
            if (needSweep || !restrictActions)
            {
                if (keyState.IsKeyDown(Controls.SweepKey) && prevKeyState.IsKeyUp(Controls.SweepKey) && !Controls.CtrlPressed())
                {
                    sweeper.Sweep();
                }
                sweeperButton.Update();
                sweeper.Update(ref items);
            }

            prevKeyState = keyState;
        }

        public void Draw(SpriteBatch spriteBatch, User user)
        {
            sweeper.Draw(spriteBatch);
            spriteBatch.Draw(bgImg, bgTopRect, Color.Gray);
            spriteBatch.Draw(bgImg, bgBottomRect, GameInfo.Planet.GroundColor);
            sweeperButton.Draw(spriteBatch);
            rapidFireButton.Draw(spriteBatch);
            healthBar.Draw(spriteBatch, GameInfo.GetColorForAmount((int)healthBar.Value, (int)healthBar.MaxValue), Color.White);
            progressBar.Draw(spriteBatch, Color.RoyalBlue, Color.Transparent);
            for (int i = 0; i <= projImgs.Count - 1; i++)
            {
                if (projImgs[i] != null && projAmounts[i] > 0)
                {
                    spriteBatch.Draw(projImgs[i], projRects[i], Color.White);
                }
            }
            if (showingToolTip)
            {
                spriteBatch.Draw(bgImg, toolTipRect, Color.Gray);
                spriteBatch.DrawString(mediumFont, toolTip, toolTipLoc, Color.White);
            }
            Color usernameColor = Color.White;
            if (!(user.AvatarR == user.AvatarG && user.AvatarR == user.AvatarB &&
                user.AvatarR <= 200))
            {
                // The above checks if all the username colors are equal (creating a gray shade that
                // is hard to see against the background).
                // However, shades above the value of 200 are allowed
                usernameColor = new Color(user.AvatarR, user.AvatarG, user.AvatarB);
            }
            spriteBatch.DrawString(bigFont, username, usernamePos, usernameColor);
            if (projAmounts[0] != 0)
            {
                // If the the first slot is empty, then they all are
                spriteBatch.Draw(highlightedImg, highlightedRect, Color.White);
            }
            for (int i = 0; i <= projImgs.Count - 1; i++)
            {
                // Draw the counts after the hightlight rectangle so that we can see them on selected projectiles
                if (projImgs[i] != null && projAmounts[i] > 0)
                {
                    spriteBatch.DrawString(mediumFont, projAmountStrings[i], projAmountPositions[i], Color.Black);
                }
            }
            DrawUserInfoDisplay(spriteBatch, user);
            DrawLives(spriteBatch, user, true);
        }
        public void DrawUserInfoDisplay(SpriteBatch spriteBatch, User user)
        {
            spriteBatch.Draw(bgImg, coinDisplayRect, Color.DarkGray);
            displayCoin.Draw(spriteBatch);
            if (user != null)
            {
                spriteBatch.DrawString(bigFont, user.Coins.ToString(),
                    new Vector2(coinDisplayRect.Right - (int)(bigFont.MeasureString(user.Coins.ToString()).X + SPACING),
                    displayCoin.Y - (int)(SPACING * 2.5f)), Color.Gold);
            }
        }
        public void DrawLives(SpriteBatch spriteBatch, User user, bool small)
        {
            InitializeLifeRects(user, small);
            for (int i = 0; i < lifeRects.Count; i++)
            {
                spriteBatch.Draw(lifeImg, lifeRects[i], lifeColors[i]);
            }
        }

        public List<MenuButton> GetButtons()
        {
            List<MenuButton> buttons = new List<MenuButton>()
            {
                sweeperButton,
                rapidFireButton.Button,
            };
            return buttons;
        }

        public void AddHealth(int health)
        {
            healthBar.Increase(health);
        }

        public void ChangePrimaryProj(ProjectileType newPrimary)
        {
            PrimaryProj = newPrimary;
            SetHighlightSquare(newPrimary);
        }

        public void SetProgress(int value)
        {
            progressBar.Value = value;
        }
        public void SetProgressMax(int value)
        {
            progressBar.MaxValue = value;
        }

        public void ChangeLives(int lives)
        {
            for (int i = 0; i < lifeColors.Count; i++)
            {
                if (i >= lives)
                {
                    lifeColors[i] = Color.Gray;
                }
                else
                {
                    lifeColors[i] = new Color(220, 0, 0);
                }
            }
        }

        public void AddSweepFinishedHandler(Action handler)
        {
            sweeper.AddOnSweepFinishHandler(handler);
        }

        #endregion

        #region Private Methods

        private Keys KeyForHotbarSlot(int slot)
        {
            switch (slot)
            {
                case 1:
                    return Controls.HotbarKey1;
                case 2:
                    return Controls.HotbarKey2;
                case 3:
                    return Controls.HotbarKey3;
                case 4:
                    return Controls.HotbarKey4;
                case 5:
                    return Controls.HotbarKey5;
                default:
                    return Keys.None;
            }
        }

        private void SetHighlightSquare(ProjectileType primary)
        {
            for (int i = 0; i < projs.Count; i++)
            {
                if (projs[i] == primary)
                {
                    highlightedRect.X = projRects[i].X;
                    highlightedRect.Y = projRects[i].Y;
                }
            }
        }

        private int AmountOf(ProjectileType type, List<Projectile> projectiles)
        {
            if (type == ProjectileType.None)
            {
                return 0;
            }
            int count = 0;
            foreach (Projectile p in projectiles)
            {
                if (p.Type == type && !p.Flying)
                {
                    if (p is ExplosiveProjectile)
                    {
                        if ((p as ExplosiveProjectile).Exploding)
                        {
                            continue;
                        }
                    }
                    count++;
                }
            }
            return count;
        }

        private void InitializeLifeRects(User user, bool small)
        {
            if (user != null)
            {
                lifeRects.Clear();
                lifeColors.Clear();
                int lifeY = small ? bgTopRect.Bottom - LIFE_SIZE - (SPACING / 3) : LARGE_SPACING;
                int lifeX = small ? windowWidth / 2 - ((LIFE_SIZE * 3 + SPACING * 2) / 2) : windowWidth - (LIFE_SIZE_BIG * 3 + SPACING * 3);
                for (int i = 0; i < GameInfo.MAX_LIVES; i++)
                {
                    lifeRects.Add(new Rectangle(lifeX, lifeY, small ? LIFE_SIZE : LIFE_SIZE_BIG,
                        small ? LIFE_SIZE : LIFE_SIZE_BIG));
                    lifeX += (small ? LIFE_SIZE : LIFE_SIZE_BIG) + SPACING;
                    if (i < user.Lives)
                    {
                        lifeColors.Add(new Color(220, 0, 0));
                    }
                    else
                    {
                        lifeColors.Add(Color.LightGray);
                    }
                }
            }
        }

        #endregion
    }

    public class Sweeper
    {
        #region Fields & Properties

        Texture2D img;
        Rectangle drawRect;

        const int WIDTH = 24;

        const int SPEED = 15;

        public bool Sweeping { get; private set; }

        int windowWidth;

        event Action sweepFinished;

        #endregion

        #region Constructors

        public Sweeper(GraphicsDevice graphics, int windowWidth, int windowHeight)
        {
            this.windowWidth = windowWidth;

            img = Utilities.RectImage;

            drawRect = new Rectangle(WIDTH * -1, 0, WIDTH, windowHeight);
        }

        #endregion

        #region Public Methods

        public void Update(ref List<Item> items)
        {
            if (Sweeping)
            {
                drawRect.X += SPEED;
                for (int i = 0; i < items.Count; i++)
                {
                    if (drawRect.Intersects(items[i].Rectangle))
                    {
                        items[i].MoveX(SPEED);
                    }
                }
                if (drawRect.X >= windowWidth)
                {
                    drawRect.X = WIDTH * -1;
                    for (int i = 0; i < items.Count; i++)
                    {
                        if (items[i].X >= windowWidth)
                        {
                            items[i].Collect();
                        }
                    }
                    Sweeping = false;
                    sweepFinished?.Invoke();
                }
            }
        }

        public void Draw(SpriteBatch spriteBatch)
        {
            if (Sweeping)
            {
                spriteBatch.Draw(img, drawRect, null, Color.Gray);
            }
        }

        public void Sweep()
        {
            if (!Sweeping)
            {
                Sweeping = true;
                Sound.PlaySound(Sounds.Sweep);
            }
        }

        public void AddOnSweepFinishHandler(Action handler)
        {
            sweepFinished += handler;
        }

        #endregion
    }
}
