using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Content;

namespace OneCannonOneArmy
{
    [Serializable]
    public struct Achievement
    {
        #region Fields & Properties

        public int Id;
        public string Name;
        public string Desc;
        public int Coins;

        public AchievementDifficulty Difficulty;

        public string ImageAsset;

        #endregion

        #region Constructors

        public Achievement(int id, string name, string desc, int coins, AchievementDifficulty difficulty, 
            string imgAsset)
        {
            Id = id;
            Name = name;
            Desc = desc;
            Coins = coins;
            Difficulty = difficulty;
            ImageAsset = imgAsset;
        }

        #endregion
    }

    public class AchievementDisplay
    {
        #region Fields & Properties

        Texture2D bgImg;
        Rectangle bgRect;

        Texture2D intImg;
        Rectangle intRect;

        Texture2D circleImg;
        Rectangle circleRect;

        SpriteFont bigFont;
        SpriteFont smallFont;

        string name;
        Vector2 namePos;
        string desc;
        Vector2 descPos;

        const int INTERIOR_SIZE = 50;
        const int SPACING = 6;

        public Achievement Achievement;

        public int X
        {
            get
            {
                return bgRect.X;
            }
            set
            {
                bgRect.X = value;
                intRect.X = bgRect.X + SPACING;
                namePos.X = intRect.Right + SPACING;
                descPos.X = namePos.X;
                circleRect.X = intRect.X;
                checkRect.X = bgRect.X - checkRect.Width;
                rewardPos.X = bgRect.Right - smallFont.MeasureString(rewardText).X - SPACING;
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
                intRect.Y = bgRect.Y + SPACING;
                namePos.Y = intRect.Y;
                descPos.Y = namePos.Y + (bigFont.MeasureString(name).Y);
                circleRect.Y = intRect.Bottom + (SPACING / 2);
                checkRect.Y = bgRect.Y;
                rewardPos.Y = bgRect.Bottom - smallFont.MeasureString(rewardText).Y - SPACING;
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

        ContentManager content;
        GraphicsDevice graphics;
        
        Texture2D checkImg;
        Rectangle checkRect;

        public bool Completed = false;

        string rewardText = "";
        Vector2 rewardPos = new Vector2();

        #endregion

        #region Constructors

        public AchievementDisplay(Achievement ach, SpriteFont bigFont, SpriteFont smallFont, ContentManager content,
            int x, int y, int width, int height, GraphicsDevice graphics, Texture2D checkImg)
        {
            Achievement = ach;

            this.graphics = graphics;
            this.content = content;
            this.bigFont = bigFont;
            this.smallFont = smallFont;

            bgImg = new Texture2D(graphics, width, height);
            bgImg = DrawHelper.AddBorder(bgImg, 3, Color.Gray, Color.LightGray);
            bgRect = new Rectangle(x, y, width, height);
            
            this.checkImg = checkImg;
            checkRect = new Rectangle(0, bgRect.Y, bgRect.Height, bgRect.Height);
            checkRect.X = bgRect.X - checkRect.Width;

            intImg = content.Load<Texture2D>(ach.ImageAsset);
            intRect = new Rectangle(bgRect.X + SPACING, bgRect.Y + SPACING, INTERIOR_SIZE, INTERIOR_SIZE);

            name = LanguageTranslator.Translate(ach.Name);
            namePos = new Vector2(intRect.Right + SPACING, intRect.Y);

            desc = LanguageTranslator.Translate(ach.Desc);
            descPos = new Vector2(namePos.X, namePos.Y + (bigFont.MeasureString(name).Y));

            Color circleColor = Color.White;
            switch (ach.Difficulty)
            {
                case AchievementDifficulty.Green:
                    circleColor = Color.Lime;
                    break;
                case AchievementDifficulty.Yellow:
                    circleColor = Color.Yellow;
                    break;
                case AchievementDifficulty.Orange:
                    circleColor = Color.Orange;
                    break;
                case AchievementDifficulty.Red:
                    circleColor = Color.Red;
                    break;
            }
            circleImg = DrawHelper.CreateCircle(10, graphics, circleColor);
            circleRect = new Rectangle(intRect.X, intRect.Bottom + (SPACING / 2), SPACING, SPACING);

            rewardText = LanguageTranslator.Translate("Reward") + ": " + ach.Coins + "c";
            rewardPos = new Vector2(bgRect.Right - smallFont.MeasureString(rewardText).X - SPACING, 
                bgRect.Bottom - smallFont.MeasureString(rewardText).Y - SPACING);
        }

        #endregion

        #region Public Methods

        public void Draw(SpriteBatch spriteBatch)
        {
            spriteBatch.Draw(bgImg, bgRect, Color.DarkGray);
            spriteBatch.Draw(intImg, intRect, Color.White);
            spriteBatch.DrawString(bigFont, name, namePos, Color.DarkGray);
            spriteBatch.DrawString(smallFont, desc, descPos, Color.DarkGray);
            spriteBatch.Draw(circleImg, circleRect, Color.White);
            spriteBatch.DrawString(smallFont, rewardText, rewardPos, Color.Goldenrod);
            if (Completed)
            {
                spriteBatch.Draw(checkImg, checkRect, Color.White);
            }
        }

        public AchievementDisplay Clone()
        {
            AchievementDisplay returnVal = new AchievementDisplay(Achievement, bigFont, smallFont, content,
                bgRect.X, bgRect.Y, bgRect.Width, bgRect.Height, graphics, checkImg);
            returnVal.Completed = Completed;
            return returnVal;
        }

        public void SetCompletion(bool val)
        {
            Completed = val;
        }

        public void LangChanged()
        {
            name = LanguageTranslator.Translate(Achievement.Name);
            desc = LanguageTranslator.Translate(Achievement.Desc);

            rewardText = LanguageTranslator.Translate("Reward") + ": " + Achievement.Coins + "c";
            rewardPos.X = bgRect.Right - smallFont.MeasureString(rewardText).X;
            rewardPos.Y = bgRect.Bottom - smallFont.MeasureString(rewardText).Y;
        }

        #endregion
    }

    public class AchievementMenu
    {
        #region Fields & Properties

        int page;
        List<AchievementDisplay> displays = new List<AchievementDisplay>();
        List<List<AchievementDisplay>> pages = new List<List<AchievementDisplay>>();

        const int SLIDE_SPEED = 100;
        bool slidingOver;
        int slideOffset;
        int slideSpd;
        int transitionPage;

        int windowWidth;
        int windowHeight;

        MenuButton nextButton;
        MenuButton prevButton;

        const int DISPLAYS_PER_PAGE = 4;

        const int SPACING = 10;

        int X_OFFSET;
        const int Y_OFFSET = Utilities.MENU_Y_OFFSET;

        const int ACH_WIDTH = 700;
        const int ACH_HEIGHT = 100;

        #endregion

        #region Constructors

        public AchievementMenu(ContentManager content, int windowWidth, int windowHeight, SpriteFont smallFont, 
            SpriteFont bigFont, GraphicsDevice graphics, Texture2D arrowImg, Texture2D checkImg)
        {
            this.windowWidth = windowWidth;
            this.windowHeight = windowHeight;

            nextButton = new MenuButton(NextPage, 0, 0, true, graphics, arrowImg);
            nextButton.X = windowWidth - nextButton.Width - SPACING;
            nextButton.Y = windowHeight - nextButton.Height - SPACING;
            prevButton = new MenuButton(PrevPage, SPACING, nextButton.Y, true, graphics, arrowImg);

            foreach (Achievement a in Achievements.AchievementsList)
            {
                displays.Add(new AchievementDisplay(a, bigFont, smallFont, content, 0, 0, ACH_WIDTH, ACH_HEIGHT, 
                    graphics, checkImg));
            }
            UpdatePagesToDisplays();

            X_OFFSET = windowWidth / 2 - (displays[0].Width / 2);
        }

        #endregion

        #region Public Methods

        public void Reset()
        {
            foreach (AchievementDisplay a in displays)
            {
                a.SetCompletion(false);
            }
            UpdatePagesToDisplays();
        }

        public void Update(User user)
        {
            bool changed = false;
            for (int i = 0; i < user.AchievementsCompleted.Count; i++)
            {
                for (int j = 0; j < displays.Count; j++)
                {
                    if (user.AchievementsCompleted[i].Id == displays[j].Achievement.Id && !displays[j].Completed)
                    {
                        displays[j].SetCompletion(true);
                        changed = true;
                        break;
                        // We break here because we found a display that matches the completed achievement
                        // Now we can move on to a new achievement without wasting processing power
                    }
                }
            }
            if (changed)
            {
                UpdatePagesToDisplays();
            }

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
                        offset = displays[0].Width + X_OFFSET;
                    }
                    else
                    {
                        offset = (displays[0].Width + X_OFFSET) * -1;
                    }
                    UpdatePositions(transitionPage, offset);
                }

                UpdatePositions(page, 0);
            }

            for (int i = 0; i <= pages.Count - 1; i++)
            {
                if (pages[i].Count <= 0)
                {
                    pages.RemoveAt(i);
                }
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

            if (slidingOver)
            {
                slideOffset += slideSpd;
            }

            nextButton.Update();
            prevButton.Update();
        }

        public void Draw(SpriteBatch spriteBatch)
        {
            nextButton.Draw(spriteBatch);
            prevButton.Draw(spriteBatch, null, SpriteEffects.FlipHorizontally);

            foreach (AchievementDisplay a in pages[page])
            {
                a.Draw(spriteBatch);
            }

            if (slidingOver)
            {
                foreach (AchievementDisplay a in pages[transitionPage])
                {
                    a.Draw(spriteBatch);
                }
            }
        }

        public List<MenuButton> GetButtons()
        {
            return new List<MenuButton>() { nextButton, prevButton };
        }

        public void LangChanged()
        {
            for (int i = 0; i < displays.Count; i++)
            {
                displays[i].LangChanged();
            }
            UpdatePagesToDisplays();
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

        private void AddDisplay(AchievementDisplay displayToAdd)
        {
            // This gets the currently used page and adds to it. 
            // Unfortunately, we need to add extra code to check if the latest page is full.
            if (pages.Count > 0)
            {
                for (int i = 0; i < pages.Count; i++)
                {
                    if (pages[i].Count < DISPLAYS_PER_PAGE)
                    {
                        pages[i].Add(displayToAdd);
                        break;
                    }
                    else
                    {
                        pages.Add(new List<AchievementDisplay>());
                        continue;
                    }
                }
            }
            else // pages.Count <= 0
            {
                pages.Add(new List<AchievementDisplay>());
                pages[0].Add(displayToAdd);
            }
        }

        private void UpdatePagesToDisplays()
        {
            pages.Clear();

            for (int i = 0; i < displays.Count; i++)
            {
                AddDisplay(displays[i].Clone());
            }
        }

        private void UpdatePositions(int page, int xOffset)
        {
            int lastY = Y_OFFSET;
            foreach (AchievementDisplay a in pages[page])
            {
                a.X = xOffset + slideOffset + X_OFFSET;
                a.Y = lastY;
                lastY += SPACING + a.Height;
            }
        }

        #endregion
    }

    public static class Achievements
    {
        #region Fields & Properties

        public static List<Achievement> AchievementsList = new List<Achievement>();

        static List<string> imgList = new List<string>()
        {
            "earth",
            "earth",
            "earth",
            "earth",
            "earth",
            "rock",
            "rock",
            "rock",
            "malosicon",
            "coinicon",
            "coinicon",
            "coinicon",
            "coinicon",
            "hammer",
            "hammer",
            "hammer",
            "hammer",
        };
        static List<AchievementDifficulty> diffList = new List<AchievementDifficulty>()
        {
            AchievementDifficulty.Green,
            AchievementDifficulty.Yellow,
            AchievementDifficulty.Orange,
            AchievementDifficulty.Red,
            AchievementDifficulty.Red,
            AchievementDifficulty.Green,
            AchievementDifficulty.Yellow,
            AchievementDifficulty.Orange,
            AchievementDifficulty.Red,
            AchievementDifficulty.Green,
            AchievementDifficulty.Yellow,
            AchievementDifficulty.Orange,
            AchievementDifficulty.Red,
            AchievementDifficulty.Green,
            AchievementDifficulty.Yellow,
            AchievementDifficulty.Orange,
            AchievementDifficulty.Red,
        };
        static List<int> coinList = new List<int>()
        {
            5,
            50,
            100,
            500,
            1000,
            55,
            110,
            550,
            10000,
            50,
            125,
            250,
            500,
            10,
            50,
            100,
            500,
        };
        //static List<int> xpList = new List<int>()
        //{
        //    5,
        //    55,
        //    120,
        //    550,
        //    1500,
        //    70,
        //    150,
        //    575,
        //    10000,
        //    100,
        //    250,
        //    500,
        //    1000,
        //    15,
        //    55,
        //    110,
        //    510,
        //};
        static List<string> nameList = new List<string>()
        {
            "Gettin' Warmed Up",
            "Stay Off My Planet!",
            "Alien Slayer",
            "Master of the Cannon",
            "Defender of Earth",
            "Boom, Boom",
            "Trigger Happy",
            "Fire in the Hole!",
            "One Cannon But No Army",
            "Gimme Those Rocks",
            "Big Spender",
            "Shop Addict",
            "Serial Spender",
            "Builder",
            "Artisan",
            "Blacksmith",
            "Craftoholic",
        };
        static List<string> descList = new List<string>()
        {
            "Kill an alien.",
            "Kill 100 aliens.",
            "Kill 200 aliens.",
            "Kill 500 aliens.",
            "Kill 1,000 aliens.",
            "Fire 200 projectiles.",
            "Fire 500 projectiles.",
            "Fire 1,000 projectiles.",
            "Defeat Malos.",
            "Spend 1,000 coins.",
            "Spend 2,500 coins.",
            "Spend 5,000 coins.",
            "Spend 10,000 coins.",
            "Craft 100 items.",
            "Craft 500 items.",
            "Craft 1,000 items.",
            "Craft 5,000 items.",
        };

        #endregion

        #region Public Methods

        public static void Initialize()
        {
            for (int i = 0; i < nameList.Count; i++)
            {
                AddAchievement(i);
            }
        }

        public static Achievement GetAchievement(string name)
        {
            foreach (Achievement a in AchievementsList)
            {
                if (a.Name == name)
                {
                    return a;
                }
            }
            return new Achievement();
        }
        public static Achievement GetAchievement(int id)
        {
            if (AchievementsList.Count < id)
            {
                return AchievementsList[id];
            }
            return new Achievement();
        }

        #endregion

        #region Private Methods

        private static void AddAchievement(int id)
        {
            AchievementsList.Add(new Achievement(id, GetName(id), GetDesc(id), GetCoins(id), GetDifficulty(id),
                GetImg(id)));
        }

        private static AchievementDifficulty GetDifficulty(int id)
        {
            if (diffList.Count > id)
            {
                return diffList[id];
            }
            return AchievementDifficulty.Green;
        }
        private static string GetImg(int id)
        {
            if (imgList.Count > id)
            {
                return imgList[id];
            }
            return "rock";
        }
        private static int GetCoins(int id)
        {
            if (coinList.Count > id)
            {
                return coinList[id];
            }
            return 0;
        }
        //private static int GetXp(int id)
        //{
        //    if (xpList.Count > id)
        //    {
        //        return xpList[id];
        //    }
        //    return 0;
        //}
        private static string GetName(int id)
        {
            if (nameList.Count > id)
            {
                return nameList[id];
            }
            return null;
        }
        private static string GetDesc(int id)
        {
            if (descList.Count > id)
            {
                return descList[id];
            }
            return null;
        }

        #endregion
    }

    public enum AchievementDifficulty
    {
        Green,
        Yellow,
        Orange,
        Red
    }
}
