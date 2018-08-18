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
    public delegate void BuyItem(int cost, Material item);

    public class Shop
    {
        #region Fields

        List<ShopMaterialInterface> materialInterfaces = new List<ShopMaterialInterface>();
        List<List<ShopMaterialInterface>> materialPages = new List<List<ShopMaterialInterface>>();
        int page;

        List<ShopCannonInterface> cannonInterfaces = new List<ShopCannonInterface>();

        List<ShopGiftInterface> giftInterfaces = new List<ShopGiftInterface>();

        MaterialInfoHover mInfoHover;
        CannonInfoHover cInfoHover;

        MenuButton nextButton;
        MenuButton prevButton;

        const int ITEMS_PER_ROW = 4;
        const int ROWS = 2;

        int windowWidth;
        int windowHeight;

        const int SPACING = 10;

        const int SHOPINT_WIDTH = 175;
        const int SHOPINT_HEIGHT = 240;
        int X_OFFSET;
        const int Y_OFFSET = Utilities.MENU_Y_OFFSET;

        BuyItem buy;

        const int SLIDE_SPEED = 100;
        bool slidingOver;
        int slideOffset;
        int slideSpd;
        int transitionPage;

        ShopState state = ShopState.Materials;

        MenuButton materialButton;
        MenuButton cannonButton;
        MenuButton giftsButton;

        Action<CannonSettings> selectCannon;

        #endregion

        #region Constructors

        public Shop(Texture2D arrowImg, int windowWidth, int windowHeight, GraphicsDevice graphics,
            List<Material> items, Dictionary<Material, int> costs, SpriteFont mediumFont, SpriteFont smallFont, BuyItem buy,
            List<CannonSettings> cannons, Dictionary<CannonSettings, int> cannonCosts,
            Action<CannonSettings, int> buyCannon, Action<CannonSettings> selectCannon,
            Dictionary<GiftType, int> giftCosts, Action<GiftType, int> buyGift, Texture2D materialIcon,
            Texture2D cannonIcon, Texture2D giftIcon)
        {
            this.windowWidth = windowWidth;
            this.windowHeight = windowHeight;

            this.buy = buy;
            this.selectCannon = selectCannon;

            mInfoHover = new MaterialInfoHover(Material.Stone, smallFont, graphics, windowWidth);
            cInfoHover = new CannonInfoHover(CannonSettings.NormalCannon, smallFont, graphics, windowWidth);

            nextButton = new MenuButton(NextPage, SPACING, 0, true, graphics, arrowImg);
            nextButton.X = windowWidth - nextButton.Width - SPACING;
            nextButton.Y = windowHeight - nextButton.Height - SPACING;

            prevButton = new MenuButton(PrevPage, SPACING, nextButton.Y, true, graphics, arrowImg);

            for (int i = 0; i < items.Count; i++)
            {
                materialInterfaces.Add(new ShopMaterialInterface(Buy, 0, 0, SHOPINT_WIDTH, SHOPINT_HEIGHT, mediumFont,
                    costs[items[i]], graphics, items[i], windowWidth));
            }
            UpdatePagesToInterfaces();

            X_OFFSET = windowWidth / 2 - (((SHOPINT_WIDTH * ITEMS_PER_ROW) + (SPACING * (ITEMS_PER_ROW - 1))) / 2);

            cannonButton = new MenuButton(new System.Action(() => state = ShopState.Cannons), 0, 0, true, graphics, cannonIcon);
            cannonButton.X = windowWidth / 2 - cannonButton.Width / 2;
            cannonButton.Y = windowHeight - cannonButton.Height - SPACING;

            materialButton = new MenuButton(new System.Action(() => state = ShopState.Materials), 0, cannonButton.Y, true, graphics, materialIcon);
            materialButton.ImgWidth = cannonButton.ImgWidth;
            materialButton.ImgHeight = cannonButton.ImgHeight;
            materialButton.X = cannonButton.X - materialButton.Width - SPACING;

            giftsButton = new MenuButton(new System.Action(() => state = ShopState.Gifts), cannonButton.X + cannonButton.Width + SPACING,
                cannonButton.Y, true, graphics, giftIcon);

            for (int i = 0; i < cannons.Count; i++)
            {
                cannonInterfaces.Add(new ShopCannonInterface(cannons[i], mediumFont, cannonCosts[cannons[i]], buyCannon, SelectCannon,
                    false, graphics, 0, 0, SHOPINT_WIDTH, SHOPINT_HEIGHT));
            }

            foreach (KeyValuePair<GiftType, int> kv in giftCosts)
            {
                giftInterfaces.Add(new ShopGiftInterface(kv.Key, graphics, mediumFont, 0, 0, SHOPINT_WIDTH, SHOPINT_HEIGHT,
                    kv.Value, buyGift));
            }
            UpdateGiftPositions();
        }

        #endregion

        #region Public Methods

        public void Initialize(User user)
        {
            for (int i = 0; i < cannonInterfaces.Count; i++)
            {
                cannonInterfaces[i].SetBought(false);
                cannonInterfaces[i].SetSelected(false);
                if (user.Cannons.Any(x => x.CannonType == cannonInterfaces[i].Cannon.CannonType))
                {
                    cannonInterfaces[i].SetBought(true);
                    CannonSettings cannon = user.Cannons.First((x) => x.CannonType == cannonInterfaces[i].Cannon.CannonType);
                    if (user.Cannons[user.CannonIndex].CannonType == cannonInterfaces[i].Cannon.CannonType)
                    {
                        cannonInterfaces[i].SetSelected(true);
                    }
                }
                // Update cannon interface to current language
                cannonInterfaces[i].LangChanged();
            }
            UpdateCannonPositions();
            UpdateMaterialPositions(0, null, 0, false, null);
            UpdateGiftPositions();
        }

        public void Update(User user, bool? onlyBuyStone, GameTime gameTime)
        {
            // If onlyBuyStone == null, don't update anything
            if (onlyBuyStone == true)
            {
                ShopMaterialInterface s = materialInterfaces.Find(x => x.Item == Material.Stone);
                s.Update(user, true, gameTime);
                List<ShopMaterialInterface> woutStone = materialInterfaces.Except(new List<ShopMaterialInterface> { s }).ToList();
                for (int i = 0; i < woutStone.Count; i++)
                {
                    woutStone[i].DisableAllButtons();
                }
            }
            else if (onlyBuyStone == false)
            {
                materialButton.Update(gameTime);
                cannonButton.Update(gameTime);
                giftsButton.Update(gameTime);

                switch (state)
                {
                    case ShopState.Materials:
                        #region Materials

                        if (slideOffset + X_OFFSET >= windowWidth || slideOffset + (X_OFFSET * -1) <= windowWidth * -1)
                        {
                            slideOffset = 0;
                            slidingOver = false;
                            page = transitionPage;
                        }

                        if (page <= materialPages.Count - 1)
                        {
                            if (slidingOver)
                            {
                                int offset = 0;
                                if (slideSpd < 0)
                                {
                                    offset = (materialInterfaces[0].Width + SPACING) * 5 + X_OFFSET;
                                }
                                else
                                {
                                    offset = ((materialInterfaces[0].Width + SPACING) * 5 + X_OFFSET) * -1;
                                }
                                UpdateMaterialPositions(transitionPage, user, offset, true, gameTime);
                            }

                            UpdateMaterialPositions(page, user, 0, true, gameTime);
                        }

                        bool mActive = false;
                        for (int i = 0; i < materialPages[page].Count; i++)
                        {
                            if (materialPages[page][i].HoveringOnItem)
                            {
                                mInfoHover.ResetMaterial(materialPages[page][i].Item);
                                mActive = true;
                                mInfoHover.Update();
                                break;
                            }
                        }
                        mInfoHover.Active = mActive;

                        for (int i = 0; i <= materialPages.Count - 1; i++)
                        {
                            if (materialPages[i].Count <= 0)
                            {
                                materialPages.RemoveAt(i);
                            }
                        }

                        //if (pages.Count <= 1)
                        //{
                        //    // Both buttons should be disabled, as there is only 1 page
                        //    // This must be tested for first, otherwise, since the page always starts equal to 0, 
                        //the below criteria will be met
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
                        break;
                    #endregion

                    case ShopState.Cannons:
                        #region Cannons
                        for (int i = 0; i < cannonInterfaces.Count; i++)
                        {
                            cannonInterfaces[i].Update(user, gameTime);
                        }

                        bool cActive = false;
                        for (int i = 0; i < cannonInterfaces.Count; i++)
                        {
                            if (cannonInterfaces[i].HoveringOnItem)
                            {
                                if (user.Cannons.Where(x => x.CannonType == cannonInterfaces[i].Cannon.CannonType).Count() > 0)
                                {
                                    cInfoHover.ResetCannon(user.Cannons.Where(x => x.CannonType == cannonInterfaces[i].Cannon.CannonType).First());
                                }
                                else
                                {
                                    cInfoHover.ResetCannon(CannonSettings.SettingsForType(cannonInterfaces[i].Cannon.CannonType));
                                }
                                cActive = true;
                                cInfoHover.Update();
                                break;
                            }
                        }
                        cInfoHover.Active = cActive;

                        break;
                    #endregion

                    case ShopState.Gifts:
                        #region Gifts
                        for (int i = 0; i < giftInterfaces.Count; i++)
                        {
                            giftInterfaces[i].Update(user, gameTime);
                        }
                        break;
                        #endregion
                }
            }
        }

        public void Draw(SpriteBatch spriteBatch)
        {
            materialButton.Draw(spriteBatch);
            cannonButton.Draw(spriteBatch);
            giftsButton.Draw(spriteBatch);

            switch (state)
            {
                case ShopState.Materials:
                    #region Materials
                    for (int i = 0; i <= materialPages[page].Count - 1; i++)
                    {
                        materialPages[page][i].Draw(spriteBatch);
                    }

                    if (slidingOver)
                    {
                        for (int i = 0; i <= materialPages[transitionPage].Count - 1; i++)
                        {
                            materialPages[transitionPage][i].Draw(spriteBatch);
                        }
                    }

                    if (mInfoHover.Active)
                    {
                        mInfoHover.Draw(spriteBatch);
                    }

                    //nextButton.Draw(spriteBatch);
                    //prevButton.Draw(spriteBatch, null, SpriteEffects.FlipHorizontally);
                    break;
                #endregion

                case ShopState.Cannons:
                    #region Cannons
                    for (int i = 0; i < cannonInterfaces.Count; i++)
                    {
                        cannonInterfaces[i].Draw(spriteBatch);
                    }

                    if (cInfoHover.Active)
                    {
                        cInfoHover.Draw(spriteBatch);
                    }
                    break;
                #endregion

                case ShopState.Gifts:
                    #region Gifts
                    for (int i = 0; i < giftInterfaces.Count; i++)
                    {
                        giftInterfaces[i].Draw(spriteBatch);
                    }
                    break;
                    #endregion
            }
        }

        public List<MenuButton> GetButtons()
        {
            List<MenuButton> returnVal = new List<MenuButton>()
            {
                materialButton,
                cannonButton,
                giftsButton,
            };
            switch (state)
            {
                case ShopState.Materials:
                    returnVal.Add(nextButton);
                    returnVal.Add(prevButton);
                    foreach (ShopMaterialInterface si in materialPages[page])
                    {
                        returnVal.AddRange(si.GetButtons());
                    }
                    break;
                case ShopState.Cannons:
                    for (int i = 0; i < cannonInterfaces.Count; i++)
                    {
                        returnVal.AddRange(cannonInterfaces[i].GetButtons());
                    }
                    break;
                case ShopState.Gifts:
                    for (int i = 0; i < giftInterfaces.Count; i++)
                    {
                        returnVal.AddRange(giftInterfaces[i].GetButtons());
                    }
                    break;
            }

            return returnVal;
        }

        public void LangChanged()
        {
            for (int i = 0; i < materialInterfaces.Count; i++)
            {
                materialInterfaces[i].LangChanged();
            }
            for (int i = 0; i < cannonInterfaces.Count; i++)
            {
                cannonInterfaces[i].LangChanged();
            }
            for (int i = 0; i < giftInterfaces.Count; i++)
            {
                giftInterfaces[i].LangChanged();
            }
            UpdatePagesToInterfaces();

            materialButton.Text = Language.Translate("Materials");
            cannonButton.Text = Language.Translate("Cannons");
            giftsButton.Text = Language.Translate("Gifts");

            // Reposition material and cannon buttons
            materialButton.X = cannonButton.X - materialButton.Width - SPACING;
            cannonButton.X = windowWidth / 2 - cannonButton.Width / 2;
            giftsButton.X = cannonButton.X + cannonButton.Width + SPACING;
            giftsButton.Y = cannonButton.Y = materialButton.Y = windowHeight - materialButton.Height - SPACING;
        }

        #endregion

        #region Private Methods

        private void Buy(int cost, Material item)
        {
            buy(cost, item);
        }
        private void SelectCannon(CannonSettings cannon)
        {
            for (int i = 0; i < cannonInterfaces.Count; i++)
            {
                cannonInterfaces[i].SetSelected(false);
            }
            selectCannon(cannon);
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

        private void AddItem(ShopMaterialInterface interfaceToAdd)
        {
            // This gets the currently used page and adds to it. 
            // Unfortunately, we need to add extra code to check if the latest page is full.
            if (materialPages.Count > 0)
            {
                for (int i = 0; i < materialPages.Count; i++)
                {
                    if (materialPages[i].Count < ITEMS_PER_ROW * ROWS)
                    {
                        materialPages[i].Add(interfaceToAdd);
                        return;
                    }
                }
                // If we made it this far, then we need a new page
                materialPages.Add(new List<ShopMaterialInterface>() { interfaceToAdd });
            }
            else // pages.Count <= 0
            {
                materialPages.Add(new List<ShopMaterialInterface>());
                materialPages[0].Add(interfaceToAdd);
            }
        }

        private void UpdatePagesToInterfaces()
        {
            materialPages.Clear();

            for (int i = 0; i < materialInterfaces.Count; i++)
            {
                AddItem(materialInterfaces[i].Clone());
            }
        }

        private void UpdateMaterialPositions(int page, User user, int xOffset, bool update, GameTime gameTime)
        {
            int row = 0;
            int lastX = X_OFFSET;
            int lastY = Y_OFFSET;
            int interfacesInCurrentRow = 0;
            foreach (ShopMaterialInterface si in materialPages[page])
            {
                si.X = lastX + slideOffset + xOffset;
                lastX += si.Width + SPACING;
                if (interfacesInCurrentRow >= ITEMS_PER_ROW)
                {
                    row++;
                    lastX = X_OFFSET;
                    si.X = lastX + slideOffset + xOffset;
                    lastX += si.Width + SPACING;
                    lastY += si.Height + SPACING;
                    interfacesInCurrentRow = 0;
                }

                si.Y = lastY;
                interfacesInCurrentRow++;

                //si.Visible = GameInfo.CanSee(user, si.Item.ProjType);
                if (update)
                {
                    si.Update(user, gameTime);
                }
            }

            lastX = X_OFFSET + slideOffset;
            lastY = Y_OFFSET;
            interfacesInCurrentRow = 0;
            row = 0;
        }
        private void UpdateCannonPositions()
        {
            int row = 0;
            int lastX = X_OFFSET;
            int lastY = Y_OFFSET;
            int interfacesInCurrentRow = 0;
            foreach (ShopCannonInterface si in cannonInterfaces)
            {
                si.X = lastX;
                lastX += si.Width + SPACING;
                if (interfacesInCurrentRow >= ITEMS_PER_ROW)
                {
                    row++;
                    lastX = X_OFFSET;
                    si.X = lastX;
                    lastX += si.Width + SPACING;
                    lastY += si.Height + SPACING;
                    interfacesInCurrentRow = 0;
                }

                si.Y = lastY;
                interfacesInCurrentRow++;
            }

            lastX = X_OFFSET + slideOffset;
            lastY = Y_OFFSET;
            interfacesInCurrentRow = 0;
            row = 0;
        }
        private void UpdateGiftPositions()
        {
            int row = 0;
            int lastX = X_OFFSET;
            int lastY = Y_OFFSET;
            int interfacesInCurrentRow = 0;
            foreach (ShopGiftInterface si in giftInterfaces)
            {
                si.X = lastX;
                lastX += si.Width + SPACING;
                if (interfacesInCurrentRow >= ITEMS_PER_ROW)
                {
                    row++;
                    lastX = X_OFFSET;
                    si.X = lastX;
                    lastX += si.Width + SPACING;
                    lastY += si.Height + SPACING;
                    interfacesInCurrentRow = 0;
                }

                si.Y = lastY;
                interfacesInCurrentRow++;
            }

            lastX = X_OFFSET + slideOffset;
            lastY = Y_OFFSET;
            interfacesInCurrentRow = 0;
            row = 0;
        }

        #endregion
    }

    public class ShopMaterialInterface
    {
        #region Fields & Properties

        public Material Item;
        Texture2D itemImg;
        Rectangle itemRect;

        MenuButton buyOnceButton;
        MenuButton buyTenButton;
        MenuButton buyFiftyButton;

        public int Cost;

        SpriteFont font;
        string name;
        Vector2 namePos;

        Texture2D bgImg;
        Rectangle bgRect;

        const int SPACING = 2;
        const int EDGE_SPACING = 10;

        BuyItem buy;

        public int X
        {
            get
            {
                return bgRect.X;
            }
            set
            {
                bgRect.X = value;
                namePos.X = bgRect.X + (bgRect.Width / 2 - (font.MeasureString(name).X / 2));
                itemRect.X = bgRect.X + (bgRect.Width / 2 - (itemRect.Width / 2));
                buyOnceButton.X = bgRect.X + (bgRect.Width / 2 - (buyOnceButton.Width / 2));
                buyTenButton.X = bgRect.X + (bgRect.Width / 2 - (buyTenButton.Width / 2));
                buyFiftyButton.X = bgRect.X + (bgRect.Width / 2 - (buyFiftyButton.Width / 2));
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
                namePos.Y = bgRect.Y + SPACING;
                itemRect.Y = (int)(namePos.Y + SPACING + font.MeasureString("A").Y);
                buyFiftyButton.Y = bgRect.Bottom - buyFiftyButton.Height - EDGE_SPACING;
                buyTenButton.Y = buyFiftyButton.Y - buyTenButton.Height - SPACING;
                buyOnceButton.Y = buyTenButton.Y - buyOnceButton.Height - SPACING;
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

        public bool Visible = true;

        const int MATERIAL_SIZE = 75;

        public bool HoveringOnItem = true;

        #endregion

        #region Constructors

        public ShopMaterialInterface(BuyItem buy, int x, int y, int width, int height, SpriteFont font, int cost,
            GraphicsDevice graphics, Material item, int windowWidth)
        {
            this.font = font;
            this.buy = buy;

            Cost = cost;

            Item = item;

            bgImg = new Texture2D(graphics, width, height);
            bgImg = DrawHelper.AddBorder(bgImg, 3, Color.Gray, Color.LightGray);
            bgRect = new Rectangle(x, y, width, height);

            name = Language.Translate(Item.ToString().AddSpaces());
            namePos = new Vector2(x + (width / 2 - (font.MeasureString(name).X / 2)), y + SPACING);

            itemImg = Utilities.GetImgOfMaterial(Item);
            itemRect = new Rectangle(0, 0, MATERIAL_SIZE, MATERIAL_SIZE);
            itemRect.X = x + (width / 2 - (itemRect.Width / 2));
            itemRect.Y = (int)(namePos.Y + SPACING + font.MeasureString("A").Y);

            buyOnceButton = new MenuButton(Buy, Language.Translate("Buy") + " (" + cost.ToString() + "c)", 0,
                0, true, font, graphics);
            buyOnceButton.X = x + (width / 2 - (buyOnceButton.Width / 2));

            buyTenButton = new MenuButton(() => Buy(10), Language.Translate("Buy") + " 10 (" + (cost * 10).ToString() + "c)", 0,
                0, true, font, graphics);
            buyTenButton.X = x + (width / 2 - (buyTenButton.Width / 2));

            buyFiftyButton = new MenuButton(() => Buy(50), Language.Translate("Buy") + " 50 (" + (cost * 50).ToString() + "c)", 0,
                bgRect.Bottom - buyOnceButton.Height - EDGE_SPACING, true, font, graphics);

            buyTenButton.Y = buyFiftyButton.Y - buyTenButton.Height - SPACING;
            buyOnceButton.Y = buyTenButton.Y - buyOnceButton.Height - SPACING;
        }

        #endregion

        #region Public Methods

        public void DisableAllButtons()
        {
            buyOnceButton.Active = false;
            buyTenButton.Active = false;
            buyFiftyButton.Active = false;
        }

        public void Update(User user, GameTime gameTime)
        {
            Update(user, false, gameTime);
        }
        public void Update(User user, bool onlyBuyTen, GameTime gameTime)
        {
            buyOnceButton.Active = (user.Coins >= Cost) && !onlyBuyTen;
            buyTenButton.Active = (user.Coins >= Cost * 10);
            buyFiftyButton.Active = (user.Coins >= Cost * 50) && !onlyBuyTen;

            buyOnceButton.Update(gameTime);
            buyTenButton.Update(gameTime);
            buyFiftyButton.Update(gameTime);

            HoveringOnItem = itemRect.Intersects(new Rectangle(Mouse.GetState().X, Mouse.GetState().Y, 1, 1));
        }

        public void Draw(SpriteBatch spriteBatch)
        {
            if (Visible)
            {
                spriteBatch.Draw(bgImg, bgRect, Color.DarkGray);
                spriteBatch.DrawString(font, name, namePos, Color.LightGray);
                spriteBatch.Draw(itemImg, itemRect, Color.White);
                buyOnceButton.Draw(spriteBatch);
                buyTenButton.Draw(spriteBatch);
                buyFiftyButton.Draw(spriteBatch);
            }
        }

        public ShopMaterialInterface Clone()
        {
            return (ShopMaterialInterface)MemberwiseClone();
        }

        public List<MenuButton> GetButtons()
        {
            if (Visible)
            {
                return new List<MenuButton>()
                {
                    buyOnceButton,
                    buyTenButton,
                    buyFiftyButton,
                };
            }
            else
            {
                return new List<MenuButton>();
            }
        }

        public void LangChanged()
        {
            string buyText = Language.Translate("Buy");
            buyOnceButton.Text = buyText + " (" + Cost.ToString() + "c)";
            buyTenButton.Text = buyText + " 10 (" + (Cost * 10).ToString() + "c)";
            buyFiftyButton.Text = buyText + " 50 (" + (Cost * 50).ToString() + "c)";

            name = Language.Translate(Item.ToString().AddSpaces());
            namePos = new Vector2(bgRect.X + (bgRect.Width / 2 - (font.MeasureString(name).X / 2)),
                bgRect.Y + SPACING);
        }

        #endregion

        #region Private Methods

        private void Buy()
        {
            buy(Cost, Item);
            Sound.PlaySound(Sounds.CaChing);
        }
        private void Buy(int itemCount)
        {
            for (int i = 0; i < itemCount; i++)
            {
                buy(Cost, Item);
            }
            Sound.PlaySound(Sounds.CaChing);
        }

        #endregion
    }
    public class ShopCannonInterface
    {
        #region Fields and Properties

        Texture2D bgImg;
        Rectangle bgRect;

        Texture2D cannonExtImg;
        Rectangle cannonExtRect;
        Texture2D cannonTubeImg;
        Rectangle cannonTubeRect;
        Texture2D cannonBgImg;
        Rectangle cannonBgRect;

        const float EXT_TUBE_RATIO_HEIGHT = 0.33f; // Ratio of Ext:Tube = 33:100
        const float TUBE_EXT_RATIO_WIDTH = 0.33333f; // Tube:Ext = 1:3 (width)
        const int CANNON_WIDTH = 80;
        const int CANNON_HEIGHT = 150;

        MenuButton buyOrSelect;
        Action<CannonSettings, int> buy;
        Action<CannonSettings> select;

        public CannonSettings Cannon;

        bool bought = false;

        int cost;

        const int SPACING = 5;

        SpriteFont font;
        string name = "";
        Vector2 namePos = new Vector2();

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
                buyOrSelect.X = bgRect.X + (bgRect.Width / 2 - (buyOrSelect.Width / 2));
                cannonBgRect.X = cannonExtRect.X = bgRect.X + (bgRect.Width / 2 - (cannonExtRect.Width / 2));
                cannonTubeRect.X = cannonExtRect.X + (cannonExtRect.Width / 2 - (cannonTubeRect.Width / 2));
                namePos.X = bgRect.X + (bgRect.Width / 2 - (font.MeasureString(name).X / 2));
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
                buyOrSelect.Y = bgRect.Bottom - buyOrSelect.Height - SPACING;
                cannonTubeRect.Y = bgRect.Y + SPACING;
                cannonBgRect.Y = cannonExtRect.Y = cannonTubeRect.Y + cannonTubeRect.Height;
                namePos.Y = cannonExtRect.Bottom;
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

        public ShopCannonInterface(CannonSettings cannon, SpriteFont font, int cost,
            Action<CannonSettings, int> buy, Action<CannonSettings> select, bool bought, GraphicsDevice graphics,
            int x, int y, int width, int height)
        {
            Cannon = cannon;
            this.buy = buy;
            this.select = select;
            this.bought = bought;
            this.cost = cost;
            this.font = font;

            bgRect = new Rectangle(x, y, width, height);
            bgImg = new Texture2D(graphics, width, height);
            bgImg = DrawHelper.AddBorder(bgImg, 3, Color.Gray, Color.LightGray);

            string buttonText = "";
            if (bought)
            {
                buttonText = Language.Translate("Select");
            }
            else
            {
                buttonText = Language.Translate("Buy") + " (" + cost + ")";
            }
            buyOrSelect = new MenuButton(BuyOrSelect, buttonText, 0, 0, true, font, graphics);
            buyOrSelect.X = bgRect.X + (bgRect.Width / 2 - (buyOrSelect.Width / 2));
            buyOrSelect.Y = bgRect.Bottom - buyOrSelect.Height - SPACING;

            cannonExtImg = Utilities.GetBottomOfCannon(cannon.CannonType);
            cannonExtRect = new Rectangle(0, 0, CANNON_WIDTH, (int)(EXT_TUBE_RATIO_HEIGHT * CANNON_HEIGHT));
            cannonExtRect.X = bgRect.X + (bgRect.Width / 2 - (cannonExtRect.Width / 2));

            cannonTubeImg = Utilities.GetTubeOfCannon(cannon.CannonType);
            cannonTubeRect = new Rectangle(0, 0, (int)(CANNON_WIDTH * TUBE_EXT_RATIO_WIDTH),
                (int)(CANNON_HEIGHT - (EXT_TUBE_RATIO_HEIGHT * CANNON_HEIGHT)));
            cannonTubeRect.X = cannonExtRect.X + (cannonExtRect.Width / 2 - (cannonTubeRect.Width / 2));
            cannonTubeRect.Y = bgRect.Y + SPACING;

            cannonExtRect.Y = cannonTubeRect.Y + cannonTubeRect.Height;

            cannonBgImg = Utilities.RectImage;
            cannonBgRect = new Rectangle(cannonExtRect.X, cannonExtRect.Y, cannonExtRect.Width, cannonExtRect.Height);

            name = Language.Translate(cannon.CannonType.ToString().AddSpaces());
            namePos = new Vector2(bgRect.X + (bgRect.Width / 2 - (font.MeasureString(name).X / 2)),
                cannonExtRect.Bottom);
        }

        #endregion

        #region Public Methods

        public void Update(User user, GameTime gameTime)
        {
            buyOrSelect.Active = ((user.Coins >= cost || bought) && user.Cannons[user.CannonIndex].CannonType != Cannon.CannonType);

            buyOrSelect.Update(gameTime);

            Rectangle mouse = new Rectangle(Mouse.GetState().X, Mouse.GetState().Y, 1, 1);
            HoveringOnItem = mouse.Intersects(cannonTubeRect) || mouse.Intersects(cannonExtRect);
        }

        public void Draw(SpriteBatch spriteBatch)
        {
            spriteBatch.Draw(bgImg, bgRect, Color.DarkGray);
            spriteBatch.Draw(cannonBgImg, cannonBgRect, Color.White);
            spriteBatch.Draw(cannonTubeImg, cannonTubeRect, Color.White);
            spriteBatch.Draw(cannonExtImg, cannonExtRect, Color.White);
            spriteBatch.DrawString(font, name, namePos, Color.Goldenrod);

            buyOrSelect.Draw(spriteBatch);
        }

        public List<MenuButton> GetButtons()
        {
            return new List<MenuButton>()
            {
                buyOrSelect
            };
        }

        public void SetBought(bool value)
        {
            bought = value;
            string buttonText = "";
            if (bought)
            {
                buttonText = Language.Translate("Select");
            }
            else
            {
                buttonText = Language.Translate("Buy") + " (" + cost + ")";
            }
            buyOrSelect.Text = buttonText;
            buyOrSelect.X = bgRect.X + (bgRect.Width / 2 - (buyOrSelect.Width / 2));
        }
        public void SetCannon(CannonSettings cannon)
        {
            Cannon = cannon;
        }
        public void SetSelected(bool value)
        {
            buyOrSelect.Active = !value;
        }

        public void LangChanged()
        {
            string buttonText = "";
            if (bought)
            {
                buttonText = Language.Translate("Select");
            }
            else
            {
                buttonText = Language.Translate("Buy") + " (" + cost + ")";
            }
            buyOrSelect.Text = buttonText;
            buyOrSelect.X = bgRect.X + (bgRect.Width / 2 - (buyOrSelect.Width / 2));

            name = Language.Translate(Cannon.CannonType.ToString().AddSpaces());
        }

        #endregion

        #region Private Methods

        private void BuyOrSelect()
        {
            if (bought)
            {
                select?.Invoke(Cannon);
                buyOrSelect.Active = false;
            }
            else
            {
                buy?.Invoke(Cannon, cost);
                SetBought(true);
            }
        }

        #endregion
    }
    public class ShopGiftInterface
    {
        #region Fields & Properties

        Texture2D bgImg;
        Rectangle bgRect;

        Texture2D giftImg;
        Rectangle giftRect;
        Color giftColor;
        public GiftType GiftType;
        int cost;

        MenuButton buyButton;

        string name = "";
        Vector2 namePos;
        SpriteFont font;

        const int SPACING = 5;
        const int GIFT_SIZE = 100;

        public int X
        {
            get
            {
                return bgRect.X;
            }
            set
            {
                bgRect.X = value;
                giftRect.X = bgRect.X + bgRect.Width / 2 - GIFT_SIZE / 2;
                buyButton.X = bgRect.X + bgRect.Width / 2 - buyButton.Width / 2;
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
                namePos.Y = bgRect.Y + SPACING;
                giftRect.Y = (int)namePos.Y + SPACING * 3 + (int)font.MeasureString(name).Y;
                buyButton.Y = bgRect.Bottom - buyButton.Height - SPACING;
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

        public ShopGiftInterface(GiftType gift, GraphicsDevice graphics, SpriteFont font, int x, int y,
            int width, int height, int cost, Action<GiftType, int> buy)
        {
            this.font = font;
            this.GiftType = gift;
            this.cost = cost;

            bgImg = new Texture2D(graphics, width, height);
            bgImg = DrawHelper.AddBorder(bgImg, 3, Color.Gray, Color.LightGray);
            bgRect = new Rectangle(x, y, width, height);

            name = Language.Translate(gift.ToString().AddSpaces()) + " " + Language.Translate("Gift");
            namePos = new Vector2(bgRect.X + bgRect.Width / 2 - font.MeasureString(name).X / 2, bgRect.Y + SPACING);

            giftImg = Utilities.GiftImg;
            giftRect = new Rectangle(bgRect.X + bgRect.Width / 2 - GIFT_SIZE / 2, (int)namePos.Y + SPACING * 3 + (int)font.MeasureString(name).Y,
                GIFT_SIZE, GIFT_SIZE);
            giftColor = Gift.GetColor(gift);

            buyButton = new MenuButton(new System.Action(() => buy?.Invoke(this.GiftType, this.cost)),
                Language.Translate("Buy") + " (" + cost.ToString() + "c)",
                0, 0, true, font, graphics);
            buyButton.X = bgRect.X + bgRect.Width / 2 - buyButton.Width / 2;
            buyButton.Y = bgRect.Bottom - buyButton.Height - SPACING;
        }

        #endregion

        #region Public Methods

        public void Update(User user, GameTime gameTime)
        {
            buyButton.Active = user.Coins >= cost;
            buyButton.Update(gameTime);
        }

        public void Draw(SpriteBatch spriteBatch)
        {
            spriteBatch.Draw(bgImg, bgRect, Color.DarkGray);
            spriteBatch.Draw(giftImg, giftRect, giftColor);
            spriteBatch.DrawString(font, name, namePos, giftColor);
            buyButton.Draw(spriteBatch);
        }

        public List<MenuButton> GetButtons()
        {
            return new List<MenuButton>()
            {
                buyButton,
            };
        }

        public void LangChanged()
        {
            buyButton.Text = Language.Translate("Buy") + " (" + cost.ToString() + "c)";
            buyButton.X = bgRect.X + bgRect.Width / 2 - buyButton.Width / 2;

            name = Language.Translate(GiftType.ToString().AddSpaces()) + " " + Language.Translate("Gift");
            namePos.X = bgRect.X + bgRect.Width / 2 - font.MeasureString(name).X / 2;
        }

        #endregion
    }

    public enum ShopState
    {
        Materials,
        Cannons,
        Gifts,
    }
}