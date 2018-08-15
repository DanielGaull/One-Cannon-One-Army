using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OneCannonOneArmy
{
    public struct Gift
    {
        static readonly int MAX = Enum.GetNames(typeof(GiftType)).Count();
        static readonly int MAX2 = MAX * MAX;

        public GiftContents Contents;
        public GiftType Type;

        public Gift(GiftType type)
        {
            Contents = GiftContents.ContentsFor(type);
            Type = type;
        }

        public User RewardUser(User user)
        {
            user.Coins += Contents.Coins;
            user.CoinsCollected += Contents.Coins;
            user.ProjectileInventory.AddRange(Contents.Projectiles);
            for (int i = 0; i < Contents.Materials.Count; i++)
            {
                user.MaterialInventory.AddItem(Contents.Materials[i], Contents.Materials.CountOf(Contents.Materials[i]));
            }
            return user;
        }

        public static GiftType GetRandomGift()
        {
            // Increasing the max increases the chances of a purple gift (otherwise was less that 1 in 1000),
            // but use Math.Min to ensure that we don't return a 6
            return (GiftType)Math.Min(Math.Abs(MAX - Math.Sqrt(MAX2 - Utilities.Rand.Next(MAX2))), MAX - 1);
        }

        public static Color GetColor(GiftType type)
        {
            switch (type)
            {
                case GiftType.Red:
                    return new Color(170, 0, 0);
                case GiftType.Orange:
                    return Color.Orange;
                case GiftType.Yellow:
                    return Color.Yellow;
                case GiftType.Green:
                    return Color.Green;
                case GiftType.Blue:
                    return Color.Blue;
                case GiftType.Purple:
                    return Color.Purple;
            }
            return new Color();
        }
    }
    public struct GiftContents
    {
        public List<ProjectileType> Projectiles;
        public List<Material> Materials;
        public int Coins;

        public GiftContents(int coins, List<ProjectileType> projectiles, List<Material> materials)
        {
            Coins = coins;
            Projectiles = projectiles;
            Materials = materials;
        }

        public static GiftContents ContentsFor(GiftType type)
        {
            return new GiftContents(CoinsForType(type), ProjectilesForType(type), MaterialsForType(type));
        }

        public static int CoinsForType(GiftType type)
        {
            return Utilities.Rand.Next(GameInfo.COIN_GIFT_MULTIPLIER_MIN * ((int)type + 1), 
                GameInfo.COIN_GIFT_MULTIPLIER_MAX * ((int)type + 1));
        }
        public static List<ProjectileType> ProjectilesForType(GiftType type)
        {
            List<ProjectileType> returnVal = new List<ProjectileType>();

            // Helps us randomly iterate through all the projectiles, not just the ones that are first
            // This prevents gifts from being biased towards the first projectiles in the enum
            List<ProjectileType> projectiles = Enum.GetValues(typeof(ProjectileType)).Cast<ProjectileType>().ToList();
            projectiles.Shuffle();

            int groups = ProjGroupsFor(type);
            int valuePerGroup = GiftWorthFor(type) / groups;
            for (int i = 0; i < groups && i < projectiles.Count; i++)
            {
                int projWorth = Utilities.WorthOf(projectiles[i]);
                if (projWorth * 2 < valuePerGroup && projWorth > 0)
                {
                    int projs = valuePerGroup / projWorth;
                    returnVal.AddRange(Enumerable.Repeat(projectiles[i], projs));
                }
            }

            return returnVal;

            // Old code for gifts (pre-1.1)
            //for (int i = 0; i < projectiles.Count && returnVal.Count < (int)type + 15; i++)
            //{
            //    int worth = Utilities.WorthOf(projectiles[i]);
            //    if (worth > 0) // Prevent unimplemented projectiles from being added (or ProjectileType.None)
            //    {
            //        int percentChance = initPercentChance - worth * 2;
            //        while (percentChance >= 100)
            //        {
            //            returnVal.Add(projectiles[i]);
            //            percentChance -= Utilities.Rand.Next(10, 15);
            //        }
            //        if (Utilities.Rand.Next(100) + 1 <= percentChance && 
            //            percentChance < 100)
            //        {
            //            returnVal.Add(projectiles[i]);
            //        }
            //    }
            //}
        }
        public static List<Material> MaterialsForType(GiftType type)
        {
            List<Material> returnVal = new List<Material>();

            // Helps us randomly iterate through all the materials, not just the ones that are first
            // This prevents gifts from being biased towards the first materials in the enum
            List<Material> materials = Enum.GetValues(typeof(Material)).Cast<Material>().ToList();
            materials.Shuffle();

            int groups = MaterialGroupsFor(type);
            int valuePerGroup = GiftWorthFor(type) / groups;
            for (int i = 0; i < groups && i < materials.Count; i++)
            {
                int materialWorth = GameInfo.CostOf(materials[i]);
                if (materialWorth * 2 < valuePerGroup)
                {
                    int counts = valuePerGroup / materialWorth;
                    returnVal.AddRange(Enumerable.Repeat(materials[i], counts));
                }
            }

            return returnVal;

            // Old gift code (from pre-1.1)
            //int giftWorth = GiftWorthFor(type);

            //for (int i = 0; i < materials.Count && returnVal.Count < (int)type + 15; i++)
            //{
            //    int worth = GameInfo.CostOf(materials[i]);
            //    int percentChance = giftWorth - worth;
            //    while (percentChance >= 100)
            //    {
            //        returnVal.Add(materials[i]);
            //        percentChance -= Utilities.Rand.Next(10, 15);
            //    }
            //    if (Utilities.Rand.Next(100) + 1 <= percentChance &&
            //        percentChance < 100)
            //    {
            //        returnVal.Add(materials[i]);
            //    }
            //}
        }

        public static int MaxCoinsForType(GiftType type)
        {
            return GameInfo.COIN_GIFT_MULTIPLIER_MAX * ((int)type + 1);
        }

        private static int GiftWorthFor(GiftType type)
        {
            return (int)Math.Pow((int)type, 3) + 25;
        }
        private static int MaterialGroupsFor(GiftType type)
        {
            return (int)type + 1;
        }
        private static int ProjGroupsFor(GiftType type)
        {
            return (int)Math.Ceiling(((int)type + 1) / 2.0f);
        }
    }

    public class GiftMenu
    {
        #region Fields & Properties

        List<GiftInterface> interfaces = new List<GiftInterface>();
        List<List<GiftInterface>> pages = new List<List<GiftInterface>>();
        int page = 0;

        MenuButton nextButton;
        MenuButton prevButton;

        GraphicsDevice graphics;

        const int INT_WIDTH = 200;
        const int INT_HEIGHT = 200;

        const int ITEMS_PER_ROW = 4;
        const int ROWS = 2;

        int transitionPage;
        bool slidingOver = false;
        int slideSpd = 0;
        int slideOffset = 0;
        const int SLIDE_SPEED = 100;

        int windowWidth;
        int windowHeight;

        int X_OFFSET;

        const int SPACING = 10;

        GiftPopup giftP;
        event RewardGift claim;
        const int POPUP_WIDTH = 500;
        const int POPUP_HEIGHT = 250;
        public bool ShowingGiftPopup
        {
            get
            {
                return giftP?.Active == true;
            }
        }

        SpriteFont font;

        string nextGiftIn = "";
        Vector2 nextGiftInPos;

        #endregion

        #region Constructors

        public GiftMenu(GraphicsDevice graphics, Texture2D arrowImg, int windowWidth, int windowHeight, SpriteFont font)
        {
            this.graphics = graphics;
            this.windowWidth = windowWidth;
            this.windowHeight = windowHeight;
            this.font = font;

            nextButton = new MenuButton(NextPage, 0, 0, true, graphics, arrowImg);
            nextButton.X = windowWidth - nextButton.Width - SPACING;
            nextButton.Y = windowHeight - nextButton.Height - SPACING;
            prevButton = new MenuButton(PrevPage, SPACING, nextButton.Y, true, graphics, arrowImg);

            X_OFFSET = windowWidth / 2 - (INT_WIDTH * ITEMS_PER_ROW + SPACING * (ITEMS_PER_ROW - 1)) / 2;

            claim += RemoveInterface;

            nextGiftInPos = new Vector2(0, Utilities.MENU_Y_OFFSET);
        }

        #endregion

        #region Public Methods

        public void Initialize(User user)
        {
            interfaces.Clear();
            pages.Clear();
            for (int i = 0; i < user.Gifts.Count; i++)
            {
                interfaces.Add(new GiftInterface(user.Gifts[i], graphics, 0, 0, INT_WIDTH, INT_HEIGHT, font));
                interfaces[i].AddOpenHandler(OpenGift);
            }
            UpdatePagesToInterfaces();
            Reposition(0, 0, false);
        }

        public void AddGift(GiftType gift)
        {
            interfaces.Add(new GiftInterface(gift, graphics, 0, 0, INT_WIDTH, INT_HEIGHT, font));
            interfaces[interfaces.Count - 1].AddOpenHandler(OpenGift);
            UpdatePagesToInterfaces();
            Reposition(0, 0, false);
        }

        public void Update(GameTime gameTime, User user)
        {
            // Update displaying time until next free gift
            TimeSpan timeUntil = user.LastReceivedGift.AddHours(GameInfo.HOURS_UNTIL_NEXT_GIFT) - DateTime.Now;
            nextGiftIn = Language.Translate("Next free gift in") + ": " +
                string.Format("{0:00}:{1:00}:{2:00}", timeUntil.Hours, timeUntil.Minutes, timeUntil.Seconds);
            nextGiftInPos.X = windowWidth / 2 - font.MeasureString(nextGiftIn).X / 2;

            if (!(giftP?.Active == true))
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
                        Reposition(transitionPage, offset, true);
                    }

                    Reposition(page, 0, true);
                }

                if (pages.Count <= 1)
                {
                    // Both buttons should be disabled, as there is only 1 page
                    // This must be tested for first, otherwise, since the page always starts equal to 0, the below criteria will be met
                    // and the game will act like there are 2 pages when there is only 1
                    nextButton.Active = false;
                    prevButton.Active = false;
                    // Since there is only one page, we have to set the page integer to 0
                    page = 0;
                }
                else if (page == 0)
                {
                    // There is another page, but we are on the first. Therefore, we cannot go backwards, so we'll disable the backwards button
                    nextButton.Active = true;
                    prevButton.Active = false;
                }
                else if (page + 1 == pages.Count)
                {
                    // We are on the last page, so the forward button must be disabled
                    nextButton.Active = false;
                    prevButton.Active = true;
                }
                else
                {
                    // We must be somewhere in the middle, so both buttons should be enabled
                    nextButton.Active = true;
                    prevButton.Active = true;
                }

                nextButton.Update();
                prevButton.Update();

                if (slidingOver)
                {
                    slideOffset += slideSpd;
                }
            }

            if (giftP?.Active == true)
            {
                giftP.Update(gameTime);
            }
        }

        public void Draw(SpriteBatch spriteBatch)
        {
            if (pages.Count > 0)
            {
                for (int i = 0; i <= pages[page].Count - 1; i++)
                {
                    pages[page][i].Draw(spriteBatch);
                }
            }

            if (slidingOver)
            {
                for (int i = 0; i <= pages[transitionPage].Count - 1; i++)
                {
                    pages[transitionPage][i].Draw(spriteBatch);
                }
            }

            nextButton.Draw(spriteBatch);
            prevButton.Draw(spriteBatch, null, SpriteEffects.FlipHorizontally);

            spriteBatch.DrawString(font, nextGiftIn, nextGiftInPos, Color.Black);

            if (giftP?.Active == true)
            {
                giftP.Draw(spriteBatch);
            }
        }

        public List<MenuButton> GetButtons()
        {
            List<MenuButton> returnVal = new List<MenuButton>();

            if (giftP?.Active == true)
            {
                returnVal.AddRange(giftP.GetButtons());
            }
            else
            {
                returnVal.Add(nextButton);
                returnVal.Add(prevButton);
                if (pages.Count > 0)
                {
                    for (int i = 0; i < pages[page].Count; i++)
                    {
                        returnVal.AddRange(pages[page][i].GetButtons());
                    }
                }
            }

            return returnVal;
        }

        public void AddClaimHandler(RewardGift handler)
        {
            claim += handler;
        }

        #endregion

        #region Private Methods

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

        private void RemoveInterface(Gift gift)
        {
            for (int i = 0; i < interfaces.Count; i++)
            {
                if (interfaces[i].Gift == gift.Type)
                {
                    interfaces.RemoveAt(i);
                    break;
                }
            }
            UpdatePagesToInterfaces();
        }

        private void AddItem(GiftInterface interfaceToAdd)
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
                        return;
                    }
                }
                // If we made it this far, then we need a new page
                pages.Add(new List<GiftInterface>() { interfaceToAdd });
            }
            else // pages.Count <= 0
            {
                pages.Add(new List<GiftInterface>());
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

        private void Reposition(int page, int xOffset, bool update)
        {
            if (pages.Count > 0)
            {
                int row = 0;
                int lastX = X_OFFSET;
                int lastY = Utilities.MENU_Y_OFFSET + (int)font.MeasureString(nextGiftIn).Y;
                int interfacesInCurrentRow = 0;
                foreach (GiftInterface gi in pages[page])
                {
                    gi.X = lastX + slideOffset + xOffset;
                    lastX += gi.Width + SPACING;
                    if (interfacesInCurrentRow >= ITEMS_PER_ROW)
                    {
                        row++;
                        lastX = X_OFFSET;
                        gi.X = lastX + slideOffset + xOffset;
                        lastX += gi.Width + SPACING;
                        lastY += gi.Height + SPACING;
                        interfacesInCurrentRow = 0;
                    }

                    gi.Y = lastY;
                    interfacesInCurrentRow++;

                    if (update)
                    {
                        gi.Update();
                    }
                }

                lastX = X_OFFSET + slideOffset;
                lastY = Utilities.MENU_Y_OFFSET;
                interfacesInCurrentRow = 0;
                row = 0;
            }
        }

        private void OpenGift(GiftType gift)
        {
            giftP = new GiftPopup(graphics, font, new Gift(gift), windowWidth / 2 - POPUP_WIDTH / 2, windowHeight / 2 - POPUP_HEIGHT / 2,
                POPUP_WIDTH, POPUP_HEIGHT);
            giftP.AddRewardHandler(claim);
        }

        #endregion
    }
    public class GiftInterface
    {
        #region Fields & Properties

        Texture2D giftImg;
        Rectangle giftRect;
        Color giftColor;

        Texture2D bgImg;
        Rectangle bgRect;

        MenuButton openButton;

        string name = "";
        Vector2 namePos;
        SpriteFont font;

        const int SPACING = 5;
        const int GIFT_SIZE = 100;

        event Action<GiftType> open;

        public GiftType Gift;

        public int X
        {
            get
            {
                return bgRect.X;
            }
            set
            {
                bgRect.X = value;
                openButton.X = bgRect.X + bgRect.Width / 2 - openButton.Width / 2;
                giftRect.X = bgRect.X + bgRect.Width / 2 - GIFT_SIZE / 2;
                namePos.X = bgRect.X + bgRect.Width / 2 - font.MeasureString(name).X / 2;
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
                openButton.Y = bgRect.Bottom - openButton.Height - SPACING;
                giftRect.Y = bgRect.Y + SPACING;
                namePos.Y = giftRect.Bottom + SPACING;
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

        #endregion

        #region Constructors

        public GiftInterface(GiftType giftType, GraphicsDevice graphics, int x, int y, int width, int height,
            SpriteFont font)
        {
            this.font = font;

            bgImg = new Texture2D(graphics, width, height);
            bgImg = DrawHelper.AddBorder(bgImg, 3, Color.Gray, Color.LightGray);
            bgRect = new Rectangle(x, y, width, height);

            openButton = new MenuButton(new System.Action(() => open?.Invoke(Gift)), Language.Translate("Open"), 0, 0, true, font, graphics);
            openButton.X = bgRect.X + bgRect.Width / 2 - openButton.Width / 2;
            openButton.Y = bgRect.Bottom - openButton.Height - SPACING;

            Gift = giftType;
            giftImg = Utilities.GiftImg;
            giftRect = new Rectangle(bgRect.X + bgRect.Width / 2 - GIFT_SIZE / 2, bgRect.Y + SPACING, GIFT_SIZE, GIFT_SIZE);
            giftColor = OneCannonOneArmy.Gift.GetColor(giftType);

            name = Language.Translate(giftType.ToString()) + " " + Language.Translate("Gift");
            namePos = new Vector2(bgRect.X + bgRect.Width / 2 - font.MeasureString(name).X / 2, giftRect.Y + SPACING);
        }

        #endregion

        #region Public Methods

        public void Update()
        {
            openButton.Update();
        }

        public void Draw(SpriteBatch spriteBatch)
        {
            spriteBatch.Draw(bgImg, bgRect, Color.DarkGray);
            spriteBatch.Draw(giftImg, giftRect, giftColor);
            spriteBatch.DrawString(font, name, namePos, giftColor);
            openButton.Draw(spriteBatch);
        }

        public void AddOpenHandler(Action<GiftType> handler)
        {
            open += handler;
        }

        public GiftInterface Clone()
        {
            return (GiftInterface)MemberwiseClone();
        }

        public List<MenuButton> GetButtons()
        {
            return new List<MenuButton>()
            {
                openButton,
            };
        }

        #endregion
    }

    public delegate void RewardGift(Gift gift);

    public class GiftPopup
    {
        #region Fields

        Texture2D itemImg;
        Rectangle itemRect;
        const int ITEM_GROW_AMOUNT = 5;

        Texture2D bgImg;
        Rectangle bgRect;

        MenuButton claimButton;

        Timer itemTimer;
        Timer fireworkTimer;
        const int FIREWORK_MILLISECS_MAX = 600;
        const int FIREWORK_MILLISECS_MIN = 200;

        string itemString = "";
        Vector2 itemStringPos;
        SpriteFont font;

        Texture2D particleImg;
        List<Vector2> particles = new List<Vector2>();
        List<Vector2> particleVelocities = new List<Vector2>();
        const int PARTICLE_SIZE = 3;
        const float PARTICLE_VEL_MIN = -1.0f;
        const float PARTICLE_VEL_MAX = 1.0f;
        const int NUM_OF_PARTICLES = 100;

        Color giftColor;

        Gift gift;
        Gift uniqueGift;
        GiftSequencePosition seqPos = GiftSequencePosition.Coins;
        int index;

        const int ITEM_SIZE = 120;
        const int ITEM_SIZE_SMALL = 10;
        const int SPACING = 5;
        const float ITEM_SHOW_WAIT_TIME = 3.0f;

        bool waitingForItemToHide = false;
        bool risingItem = true;
        bool growingItem = false;
        bool done = false;

        public bool Active = true;

        event RewardGift reward;

        MouseState mouse;
        MouseState prevMouse;

        string skipTip = Language.Translate("Click to go faster.");
        Vector2 skipTipPos = new Vector2();

        #endregion

        #region Constructors

        public GiftPopup(GraphicsDevice graphics, SpriteFont font, Gift gift, int x, int y, int width, int height)
        {
            this.font = font;
            this.gift = gift;
            giftColor = Gift.GetColor(gift.Type);
            uniqueGift = new Gift(gift.Type);
            uniqueGift.Contents = new GiftContents(gift.Contents.Coins, gift.Contents.Projectiles.Unique(),
                gift.Contents.Materials.Unique());

            bgImg = new Texture2D(graphics, width, height);
            bgImg = DrawHelper.AddBorder(bgImg, 3, Color.Gray, Color.LightGray);
            bgRect = new Rectangle(x, y, width, height);

            itemRect = new Rectangle();
            ResetItemRect();
            itemImg = Utilities.CoinIcon;

            itemTimer = new Timer(ITEM_SHOW_WAIT_TIME, TimerUnits.Seconds);

            particleImg = new Texture2D(graphics, PARTICLE_SIZE, PARTICLE_SIZE);
            Color[] data = new Color[PARTICLE_SIZE * PARTICLE_SIZE];
            for (int i = 0; i < PARTICLE_SIZE * PARTICLE_SIZE; i++)
            {
                data[i] = Color.White;
            }
            particleImg.SetData(data);
            for (int i = 0; i < NUM_OF_PARTICLES; i++)
            {
                AddParticle();
            }

            claimButton = new MenuButton(new System.Action(() => reward?.Invoke(gift)), Language.Translate("Claim Gift"), 0, 0, true, font,
                    graphics);
            claimButton.X = bgRect.X + bgRect.Width / 2 - claimButton.Width / 2;
            claimButton.Y = bgRect.Bottom - claimButton.Height - SPACING * 2;
            reward += new RewardGift(w => Active = false);

            itemString = "+" + gift.Contents.Coins.ToString() + " " + Language.Translate("coins");
            itemStringPos = new Vector2(0, claimButton.Y - SPACING - font.MeasureString(itemString).Y);
            RepostionItemString();

            fireworkTimer = new Timer(0, TimerUnits.Milliseconds);

            skipTipPos.X = bgRect.X + bgRect.Width / 2 - font.MeasureString(skipTip).X / 2;
            skipTipPos.Y = bgRect.Y + SPACING;
        }

        #endregion

        #region Public Methods

        public void Update(GameTime gameTime)
        {
            if (Active)
            {
                mouse = Mouse.GetState();

                fireworkTimer.Update(gameTime);
                if (fireworkTimer.QueryWaitTime(gameTime))
                {
                    Sound.PlaySound(Sounds.Firework);
                    fireworkTimer.WaitTime = Utilities.Rand.Next(FIREWORK_MILLISECS_MIN, FIREWORK_MILLISECS_MAX);
                }

                UpdateParticles();
                claimButton.Active = done;
                claimButton.Update();
                if (done)
                {
                    itemRect.Width = itemRect.Height = ITEM_SIZE;
                    itemRect.Y = bgRect.Y + SPACING + (int)font.MeasureString("A").Y;
                    itemRect.X = bgRect.X + bgRect.Width / 2 - itemRect.Width / 2;
                }
                else
                {
                    if (waitingForItemToHide)
                    {
                        itemTimer.Update(gameTime);
                        if (itemTimer.QueryWaitTime(gameTime))
                        {
                            HideAndShowNextItem();
                        }
                    }
                    else if (growingItem)
                    {
                        GrowItemRect();
                        if (itemRect.Width >= ITEM_SIZE)
                        {
                            AfterGrow();
                        }
                    }
                    else if (risingItem)
                    {
                        RiseItemRect();
                        if (itemRect.Y <= bgRect.Y + ITEM_SIZE - ITEM_SIZE_SMALL + SPACING + font.MeasureString("A").Y)
                        {
                            itemRect.Y = bgRect.Y + ITEM_SIZE - ITEM_SIZE_SMALL + SPACING + (int)font.MeasureString("A").Y;
                            risingItem = false;
                            growingItem = true;
                        }
                    }

                    if (mouse.LeftButton == ButtonState.Pressed && 
                        prevMouse.LeftButton != ButtonState.Pressed)
                    {
                        if (waitingForItemToHide)
                        {
                            HideAndShowNextItem();
                        }
                        else if (growingItem || risingItem)
                        {
                            AfterGrow();
                        }
                    }

                    prevMouse = mouse;
                }
            }
        }

        public void Draw(SpriteBatch spriteBatch)
        {
            if (Active)
            {
                spriteBatch.Draw(bgImg, bgRect, Color.DarkGray);
                spriteBatch.Draw(itemImg, itemRect, Color.White);
                spriteBatch.DrawString(font, itemString, itemStringPos, new Color(DrawHelper.GetDominantColor(itemImg), 255.0f));
                spriteBatch.DrawString(font, skipTip, skipTipPos, Color.White);
                if (done)
                {
                    claimButton.Draw(spriteBatch);
                }
                for (int i = 0; i < particles.Count; i++)
                {
                    spriteBatch.Draw(particleImg,
                        new Rectangle((int)particles[i].X, (int)particles[i].Y, PARTICLE_SIZE, PARTICLE_SIZE), giftColor);
                }
            }
        }

        public void AddRewardHandler(RewardGift handler)
        {
            reward += handler;
        }

        public List<MenuButton> GetButtons()
        {
            if (Active)
            {
                return new List<MenuButton>()
                {
                    claimButton,
                };
            }
            else
            {
                return new List<MenuButton>();
            }
        }

        #endregion

        #region Private Methods

        private void ResetItemRect()
        {
            itemRect.Width = itemRect.Height = ITEM_SIZE_SMALL;
            itemRect.X = bgRect.X + bgRect.Width / 2 - itemRect.Width / 2;
            itemRect.Y = bgRect.Bottom - SPACING - ITEM_SIZE_SMALL;
        }
        private void GrowItemRect()
        {
            itemRect.Width += ITEM_GROW_AMOUNT;
            itemRect.Height += ITEM_GROW_AMOUNT;
            itemRect.Y -= ITEM_GROW_AMOUNT;
            itemRect.X = bgRect.X + bgRect.Width / 2 - itemRect.Width / 2;
        }
        private void RiseItemRect()
        {
            itemRect.Y -= ITEM_GROW_AMOUNT;
        }

        private void SetNextItem()
        {
            switch (seqPos)
            {
                case GiftSequencePosition.Coins:
                    if (uniqueGift.Contents.Materials.Count > 0)
                    {
                        InitMaterialSeq();
                    }
                    else if (uniqueGift.Contents.Projectiles.Count > 0)
                    {
                        InitProjectileSeq();
                    }
                    else
                    {
                        Finish();
                    }
                    break;

                case GiftSequencePosition.Materials:
                    if (index + 1 < uniqueGift.Contents.Materials.Unique().Count)
                    {
                        index++;
                        itemImg = Utilities.GetImgOfMaterial(uniqueGift.Contents.Materials[index]);
                        itemString = string.Format("+{0} {1}",
                            gift.Contents.Materials.CountOf(uniqueGift.Contents.Materials[index]),
                            Language.Translate(uniqueGift.Contents.Materials[index].ToString().AddSpaces()));
                    }
                    else if (gift.Contents.Projectiles.Count > 0)
                    {
                        InitProjectileSeq();
                    }
                    else
                    {
                        Finish();
                    }
                    break;

                case GiftSequencePosition.Projectiles:
                    if (index + 1 < uniqueGift.Contents.Projectiles.Count)
                    {
                        index++;
                        itemImg = Utilities.GetIconOf(uniqueGift.Contents.Projectiles[index]);
                        itemString = string.Format("+{0} {1}", 
                            gift.Contents.Projectiles.CountOf(uniqueGift.Contents.Projectiles[index]),
                            Language.Translate(uniqueGift.Contents.Projectiles[index].ToString().AddSpaces()));
                    }
                    else
                    {
                        Finish();
                    }
                    break;
            }
            RepostionItemString();
        }
        private void InitMaterialSeq()
        {
            index = 0;
            seqPos = GiftSequencePosition.Materials;
            itemImg = Utilities.GetImgOfMaterial(uniqueGift.Contents.Materials[index]);
            itemString = string.Format("+{0} {1}",
                gift.Contents.Materials.CountOf(uniqueGift.Contents.Materials[index]),
                Language.Translate(uniqueGift.Contents.Materials[index].ToString().AddSpaces()));
        }
        private void InitProjectileSeq()
        {
            index = 0;
            seqPos = GiftSequencePosition.Projectiles;
            itemImg = Utilities.GetIconOf(uniqueGift.Contents.Projectiles[index]);
            itemString = string.Format("+{0} {1}",
                            gift.Contents.Projectiles.CountOf(uniqueGift.Contents.Projectiles[index]),
                            Language.Translate(uniqueGift.Contents.Projectiles[index].ToString().AddSpaces()));
        }

        private void RepostionItemString()
        {
            itemStringPos.X = bgRect.X + bgRect.Width / 2 - font.MeasureString(itemString).X / 2;
        }

        private void UpdateParticles()
        {
            for (int i = 0; i < particles.Count; i++)
            {
                particles[i] = new Vector2(particles[i].X + particleVelocities[i].X, particles[i].Y + particleVelocities[i].Y);
                if ((Utilities.Rand.Next(0, 50) == 1) || // 2% chance
                    (particles[i].X > bgRect.Right || particles[i].X < bgRect.X) ||
                    (particles[i].Y > bgRect.Bottom || particles[i].Y < bgRect.Y))
                // Keep particles within area of popup
                {
                    particles.RemoveAt(i);
                    particleVelocities.RemoveAt(i);
                    AddParticle();
                }
            }
        }
        private void AddParticle()
        {
            Random rand = Utilities.Rand;
            int x = rand.Next(bgRect.X, bgRect.Right);
            int y = rand.Next(bgRect.Y, bgRect.Bottom);
            particles.Add(new Vector2(x, y));
            particleVelocities.Add(new Vector2(RandVel(), RandVel()));
        }
        private float RandVel()
        {
            return (float)(Utilities.Rand.NextDouble() * (PARTICLE_VEL_MAX - PARTICLE_VEL_MIN) + PARTICLE_VEL_MIN);
        }

        private void Finish()
        {
            done = true;
        }

        private void HideAndShowNextItem()
        {
            waitingForItemToHide = false;
            risingItem = true;
            itemTimer.WaitTime = ITEM_SHOW_WAIT_TIME;
            ResetItemRect();
            SetNextItem();
        }
        private void AfterGrow()
        {
            growingItem = false;
            waitingForItemToHide = true;
            itemRect.Width = itemRect.Height = ITEM_SIZE;
            itemRect.Y = bgRect.Y + SPACING + (int)font.MeasureString("A").Y;
            itemRect.X = bgRect.X + bgRect.Width / 2 - itemRect.Width / 2;
            switch (seqPos)
            {
                case GiftSequencePosition.Coins:
                    if (!Sound.IsPlaying(Sounds.CaChing))
                    {
                        Sound.PlaySound(Sounds.CaChing);
                    }
                    break;
                case GiftSequencePosition.Materials:
                    if (!Sound.IsPlaying(Sounds.MaterialGift))
                    {
                        Sound.PlaySound(Sounds.MaterialGift);
                    }
                    break;
                case GiftSequencePosition.Projectiles:
                    if (!Sound.IsPlaying(Sounds.ProjectileGift))
                    {
                        Sound.PlaySound(Sounds.ProjectileGift);
                    }
                    break;
            }
        }

        #endregion

        #region Enumerations

        private enum GiftSequencePosition
        {
            Coins,
            Materials,
            Projectiles
        }

        #endregion
    }

    public enum GiftType
    {
        Red = 0,
        Orange = 1,
        Yellow = 2,
        Green = 3,
        Blue = 4,
        Purple = 5,
    }
}
