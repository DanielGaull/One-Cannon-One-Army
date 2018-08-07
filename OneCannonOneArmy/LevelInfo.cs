using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OneCannonOneArmy
{
    public class LevelGoalPopup
    {
        #region Fields & Properties

        Texture2D bgImg;
        Rectangle bgRect;

        SpriteFont font;
        string goalText;
        Vector2 goalPos;

        MissionGoal goal;
        Texture2D currentThumbImg;
        Rectangle thumbRect;

        Timer timer;

        const string KILL_ALL_TEXT = "Kill all the aliens!";
        const string MECH_TEXT = "Destroy {0} aliens carrying mechanical supplies!";
        const string SAVE_TEXT1 = "Break open {0} cages!";
        const string SAVE_TEXT2 = "Break open {0} cage!";
        const string LASER_TEXT = "Defeat the aliens, then destroy the laser!";
        const string BOSS_TEXT = "Kill Malos!";

        const int SPACING = 10;
        const int SLIDE_SPD = 5;
        const int THUMB_SIZE = 50;

        bool slidingIn = false;
        bool slidingOut = false;

        int windowWidth;
        int windowHeight;

        public bool Active = false;

        event System.Action onHide;

        #endregion

        #region Constructors

        public LevelGoalPopup(GraphicsDevice graphics, SpriteFont font, int width, int height)
        {
            this.font = font;
            bgRect = new Rectangle(0, 0, width, height);
            thumbRect = new Rectangle(0, 0, THUMB_SIZE, THUMB_SIZE);
            timer = new Timer(2.0f, TimerUnits.Seconds);
            goalPos = new Vector2();

            bgImg = new Texture2D(graphics, width, height);
            bgImg = DrawHelper.AddBorder(bgImg, 3, Color.Gray, Color.LightGray);
        }

        #endregion

        #region Public Methods

        public void Show(int windowWidth, int windowHeight, MissionGoal goal, int amount)
        {
            this.windowWidth = windowWidth;
            this.windowHeight = windowHeight;
            this.goal = goal;
            slidingIn = true;
            bgRect.X = windowWidth / 2 - (bgRect.Width / 2);
            bgRect.Y = bgRect.Height * -1;
            thumbRect.X = bgRect.X + (bgRect.Width / 2 - (thumbRect.Width / 2));
            thumbRect.Y = bgRect.Y + SPACING;
            goalText = TextFor(goal, amount);
            goalPos.X = bgRect.X + (bgRect.Width / 2 - (font.MeasureString(goalText).X / 2));
            goalPos.Y = thumbRect.Bottom + SPACING;
            currentThumbImg = Utilities.GetImgOfGoal(goal);
            Active = true;
        }

        public void Update(GameTime gameTime)
        {
            if (slidingIn)
            {
                if (bgRect.Y < windowHeight / 2 - (bgRect.Height / 2))
                {
                    bgRect.Y += SLIDE_SPD;
                    thumbRect.Y += SLIDE_SPD;
                    goalPos.Y += SLIDE_SPD;
                }
                else
                {
                    slidingIn = false;
                }
            }
            else if (slidingOut)
            {
                if (bgRect.Y < windowHeight)
                {
                    bgRect.Y += SLIDE_SPD;
                    thumbRect.Y += SLIDE_SPD;
                    goalPos.Y += SLIDE_SPD;
                }
                else
                {
                    slidingOut = false;
                    Active = false;
                    onHide?.Invoke();
                }
            }
            else
            {
                timer.Update(gameTime);
                if (timer.QueryWaitTime(gameTime))
                {
                    slidingOut = true;
                }
            }
        }

        public void Draw(SpriteBatch spriteBatch)
        {
            spriteBatch.Draw(bgImg, bgRect, Color.Gray);
            spriteBatch.Draw(currentThumbImg, thumbRect, Color.White);
            spriteBatch.DrawString(font, goalText, goalPos, Color.White);
        }

        public void AddOnHideHandler(System.Action handler)
        {
            onHide += handler;
        }

        #endregion

        #region Private Methods

        private string TextFor(MissionGoal goal, int amount)
        {
            switch (goal)
            {
                case MissionGoal.KillAll:
                    return Language.Translate(KILL_ALL_TEXT);
                case MissionGoal.DestroyMechanics:
                    return string.Format(Language.Translate(MECH_TEXT), amount);
                case MissionGoal.SavePeople:
                    return string.Format(Language.Translate(amount > 1 ? SAVE_TEXT1 : SAVE_TEXT2), amount);
                case MissionGoal.DestroyLaser:
                    return Language.Translate(LASER_TEXT);
                case MissionGoal.KillMalos:
                    return Language.Translate(BOSS_TEXT);
                default:
                    return "";
            }
        }

        #endregion
    }

    #region Unused
    //public class LevelUnlockPopup
    //{
    //    #region Fields

    //    Texture2D bgImg;
    //    Rectangle bgRect;

    //    List<Alien> drawAliens = new List<Alien>();
    //    List<Projectile> drawProjs = new List<Projectile>();
    //    List<Texture2D> otherUnlocks = new List<Texture2D>();
    //    List<Rectangle> otherUnlockRects = new List<Rectangle>();

    //    List<string> unlocks = new List<string>();
    //    List<Vector2> unlockPositions = new List<Vector2>();

    //    SpriteFont font;

    //    public bool Active = false;

    //    Timer timer;
    //    bool slidingIn = false;
    //    bool slidingOut = false;

    //    const int SPACING = 10;
    //    const int ALIEN_WIDTH = 200;
    //    const int ALIEN_HEIGHT = 100;
    //    const int PROJ_SIZE = 100;
    //    const int SLIDE_SPD = 5;

    //    int windowWidth;
    //    int windowHeight;

    //    event Action onHide;

    //    #endregion

    //    #region Constructors

    //    public LevelUnlockPopup(int level, GraphicsDevice graphics, int width, int height, SpriteFont font)
    //    {
    //        this.font = font;

    //        bgImg = new Texture2D(graphics, width, height);
    //        bgImg = DrawHelper.AddBorder(bgImg, 3, Color.Gray, Color.LightGray);
    //        bgRect = new Rectangle(0, 0, width, height);

    //        SetValuesForLevel(level);

    //        timer = new Timer(3.5f, TimerUnits.Seconds);
    //    }

    //    #endregion

    //    #region Public Methods

    //    public void Show(int windowWidth, int windowHeight)
    //    {
    //        this.windowWidth = windowWidth;
    //        this.windowHeight = windowHeight;
    //        Active = true;
    //        slidingIn = true;
    //        bgRect.X = windowWidth / 2 - (bgRect.Width / 2);
    //        bgRect.Y = bgRect.Height * -1;
    //        Position();
    //    }

    //    public void Update(GameTime gameTime)
    //    {
    //        if (slidingIn)
    //        {
    //            if (bgRect.Y < windowHeight / 2 - (bgRect.Height / 2))
    //            {
    //                bgRect.Y += SLIDE_SPD;
    //                Position();
    //            }
    //            else
    //            {
    //                slidingIn = false;
    //            }
    //        }
    //        else if (slidingOut)
    //        {
    //            if (bgRect.Y < windowHeight)
    //            {
    //                bgRect.Y += SLIDE_SPD;
    //                Position();
    //            }
    //            else
    //            {
    //                slidingOut = false;
    //                Active = false;
    //                onHide?.Invoke();
    //            }
    //        }
    //        else
    //        {
    //            timer.Update(gameTime);
    //            if (timer.QueryWaitTime(gameTime))
    //            {
    //                slidingOut = true;
    //            }
    //        }
    //    }

    //    public void Draw(SpriteBatch spriteBatch)
    //    {
    //        spriteBatch.Draw(bgImg, bgRect, Color.White);
    //        for (int i = 0; i < drawAliens.Count; i++)
    //        {
    //            drawAliens[i].Draw(spriteBatch);
    //        }
    //        for (int i = 0; i < drawProjs.Count; i++)
    //        {
    //            drawProjs[i].Draw(spriteBatch, false);
    //        }
    //        for (int i = 0; i < otherUnlocks.Count; i++)
    //        {
    //            spriteBatch.Draw(otherUnlocks[i], otherUnlockRects[i], Color.White);
    //        }
    //        for (int i = 0; i < unlocks.Count; i++)
    //        {
    //            spriteBatch.DrawString(font, unlocks[i], unlockPositions[i], Color.Black);
    //        }
    //    }

    //    public void AddOnHideHandler(Action handler)
    //    {
    //        onHide += handler;
    //    }

    //    #endregion

    //    #region Private Methods

    //    private void SetValuesForLevel(int level)
    //    {
    //        // Check and add any aliens for the level first
    //        if (GameInfo.AlienVisibilityLvls.ContainsKey(level))
    //        {
    //            drawAliens.Add(GameInfo.CreateAlien(GameInfo.AlienVisibilityLvls[level], false, 1000, 1000, ALIEN_WIDTH,
    //                ALIEN_HEIGHT, null));
    //            unlocks.Add(GameInfo.ToText(GameInfo.AlienVisibilityLvls[level]));
    //            unlockPositions.Add(new Vector2());
    //        }
    //        // Now check and add projectiles
    //        if (GameInfo.ProjVisibilityLvls.ContainsValue(level))
    //        {
    //            ProjectileType proj = GameInfo.ProjVisibilityLvls.FirstOrDefault(x => x.Value == level).Key;
    //            drawProjs.Add(GameInfo.CreateProj(proj));
    //            unlocks.Add(proj.ToString().AddSpaces());
    //            unlockPositions.Add(new Vector2());
    //        }
    //        // Finally, check for special features added in levels (such as shields or cannon upgrades)
    //        if (level == GameInfo.SHIELD_LVL)
    //        {
    //            otherUnlocks.Add(Utilities.AlienShieldImg);
    //            otherUnlockRects.Add(new Rectangle(0, 0, PROJ_SIZE, PROJ_SIZE));
    //            unlocks.Add("Alien Shields");
    //            unlockPositions.Add(new Vector2());
    //        }
    //    }

    //    private void Position()
    //    {
    //        int lastY = bgRect.Y + SPACING;
    //        int x = bgRect.X + SPACING;
    //        for (int i = 0; i < drawAliens.Count; i++)
    //        {
    //            drawAliens[i].X = x;
    //            drawAliens[i].Y = lastY;
    //            lastY += ALIEN_HEIGHT + SPACING / 2;
    //            unlockPositions[i] = new Vector2(drawAliens[i].X + ALIEN_WIDTH, drawAliens[i].Y);
    //        }
    //        for (int i = 0; i < drawProjs.Count; i++)
    //        {
    //            drawProjs[i].X = x;
    //            drawProjs[i].Y = lastY;
    //            lastY += PROJ_SIZE + SPACING / 2;
    //            // We add the count of the drawAliens list to get the position value for THIS
    //            // projectile. If we simply used "i", then the value of the alien text positioning
    //            // would be ruined
    //            unlockPositions[i + drawAliens.Count] = new Vector2(drawProjs[i].X + ALIEN_WIDTH, drawProjs[i].Y);
    //        }
    //        for (int i = 0; i < otherUnlockRects.Count; i++)
    //        {
    //            Rectangle newRect = new Rectangle(0, 0, otherUnlockRects[i].Width, otherUnlockRects[i].Height);
    //            newRect.X = x;
    //            newRect.Y = lastY;
    //            otherUnlockRects[i] = newRect;
    //            lastY += PROJ_SIZE + SPACING / 2;
    //            // Like before, we add the counts of the previous lists to set the value for THIS 
    //            // item
    //            unlockPositions[i + drawAliens.Count + drawProjs.Count] = new Vector2(otherUnlockRects[i].X + ALIEN_WIDTH,
    //                otherUnlockRects[i].Y);
    //        }
    //    }

    //    #endregion
    //}
    #endregion

    public class LevelCompletePopup
    {
        #region Fields

        Texture2D bgImg;
        Rectangle bgRect;

        Texture2D unlockBgImg;
        Rectangle unlockBgRect;
        const float UNLOCK_NORMAL_RATIO = 0.6f;

        MenuButton levelSelectButton;
        MenuButton nextLevelButton;
        MenuButton replayButton;

        SpriteFont font;
        string congratsText = "";
        Vector2 congratsPos;

        const int SPACING = 5;
        const int BUTTON_IMG_SPACING = 10;

        public bool Active = false;

        List<Alien> drawAliens = new List<Alien>();
        List<Texture2D> projImgs = new List<Texture2D>();
        List<Rectangle> projRects = new List<Rectangle>();
        List<Texture2D> otherUnlockImgs = new List<Texture2D>();
        List<Rectangle> otherUnlockRects = new List<Rectangle>();
        List<string> unlocks = new List<string>();
        List<Vector2> unlockPositions = new List<Vector2>();

        const int ALIEN_WIDTH = 50;
        const int ALIEN_HEIGHT = 25;
        const int ITEM_SIZE = 30;
        const int BUTTON_SIZE = 60;

        bool success = true;

        #endregion

        #region Constructors

        public LevelCompletePopup(GraphicsDevice graphics, SpriteFont font, int width, int height,
            System.Action levelSelect, System.Action nextLevel, System.Action replay, Texture2D levelSelectImg,
            Texture2D nextLevelImg, Texture2D replayImg)
        {
            // Creates a popup for level completion/failure

            this.font = font;

            int bgHeight = (int)(UNLOCK_NORMAL_RATIO * height);
            int unlockHeight = height - bgHeight;

            bgImg = new Texture2D(graphics, width, bgHeight);
            bgImg = DrawHelper.AddBorder(bgImg, 3, Color.Gray, Color.LightGray);
            bgRect = new Rectangle(0, 0, width, bgHeight);

            unlockBgImg = new Texture2D(graphics, width, unlockHeight);
            unlockBgImg = DrawHelper.AddBorder(unlockBgImg, 3, Color.Gray, Color.LightGray);
            unlockBgRect = new Rectangle(0, 0, width, unlockHeight);

            levelSelectButton = new MenuButton(levelSelect, 0, 0, true, graphics, levelSelectImg);
            levelSelectButton.ImgWidth = levelSelectButton.ImgHeight = BUTTON_SIZE;
            levelSelectButton.Width = levelSelectButton.Height = BUTTON_SIZE + BUTTON_IMG_SPACING;
            nextLevelButton = new MenuButton(nextLevel, 0, 0, true, graphics, nextLevelImg);
            nextLevelButton.ImgWidth = nextLevelButton.ImgHeight = BUTTON_SIZE;
            nextLevelButton.Width = nextLevelButton.Height = BUTTON_SIZE + BUTTON_IMG_SPACING;
            replayButton = new MenuButton(replay, 0, 0, true, graphics, replayImg);
            replayButton.ImgWidth = replayButton.ImgHeight = BUTTON_SIZE;
            replayButton.Width = replayButton.Height = BUTTON_SIZE + BUTTON_IMG_SPACING;

            congratsPos = new Vector2();
        }

        #endregion

        #region Public Methods

        public void Show(int windowWidth, int windowHeight, string congratsText, int mission, bool levelSuccess)
        {
            // "mission" is the current mission. We need the info on the NEXT mission
            mission++;
            success = levelSuccess;
            this.congratsText = congratsText;
            bgRect.X = windowWidth / 2 - (bgRect.Width / 2);
            bgRect.Y = windowHeight / 2 - (bgRect.Height / 2);
            unlockBgRect.X = bgRect.X;
            unlockBgRect.Y = bgRect.Bottom;
            congratsPos.X = windowWidth / 2 - (font.MeasureString(congratsText).X / 2);
            congratsPos.Y = bgRect.Y + SPACING;
            nextLevelButton.X = bgRect.X + (bgRect.Width / 2 - (nextLevelButton.Width / 2));
            nextLevelButton.Y = (int)(congratsPos.Y + font.MeasureString(congratsText).Y + SPACING);
            levelSelectButton.X = nextLevelButton.X - levelSelectButton.Width - SPACING;
            levelSelectButton.Y = nextLevelButton.Y;
            replayButton.X = nextLevelButton.DrawRectangle.Right + SPACING;
            replayButton.Y = nextLevelButton.Y;
            Active = true;
            SetUnlocksForMission(mission);
            Position();
        }

        public void Update()
        {
            levelSelectButton.Update();
            if (success)
            {
                nextLevelButton.Update();
            }
            replayButton.Update();
        }

        public void Draw(SpriteBatch spriteBatch)
        {
            spriteBatch.Draw(bgImg, bgRect, Color.White);
            spriteBatch.DrawString(font, congratsText, congratsPos, Color.Black);
            if (success)
            {
                nextLevelButton.Draw(spriteBatch);
            }
            levelSelectButton.Draw(spriteBatch);
            replayButton.Draw(spriteBatch);

            if (success && unlocks.Count > 0)
            {
                // Don't draw unlocks unless you've actually unlocked something
                spriteBatch.Draw(unlockBgImg, unlockBgRect, Color.White);
                for (int i = 0; i < drawAliens.Count; i++)
                {
                    drawAliens[i].Draw(spriteBatch);
                }
                for (int i = 0; i < projImgs.Count; i++)
                {
                    spriteBatch.Draw(projImgs[i], projRects[i], Color.White);
                }
                for (int i = 0; i < otherUnlockImgs.Count; i++)
                {
                    spriteBatch.Draw(otherUnlockImgs[i], otherUnlockRects[i], Color.White);
                }
                for (int i = 0; i < unlocks.Count; i++)
                {
                    spriteBatch.DrawString(font, unlocks[i], unlockPositions[i], Color.Black);
                }
            }
        }

        public List<MenuButton> GetButtons()
        {
            List<MenuButton> returnVal = new List<MenuButton>()
            {
                levelSelectButton,
                replayButton
            };

            if (success)
            {
                returnVal.Add(nextLevelButton);
            }

            return returnVal;
        }

        #endregion

        #region Private Methods

        private void SetUnlocksForMission(int mission)
        {
            drawAliens.Clear();
            projImgs.Clear();
            projRects.Clear();
            otherUnlockImgs.Clear();
            otherUnlockRects.Clear();
            unlocks.Clear();
            unlockPositions.Clear();

            // Check and add any aliens for the level first
            if (GameInfo.AlienVisibilityLvls.ContainsKey(mission))
            {
                List<Aliens> aliens = GameInfo.AlienVisibilityLvls
                    .Where(x => x.Key == mission)
                    .Select(kvp => kvp.Value)
                    .ToList();
                for (int i = 0; i < aliens.Count; i++)
                {
                    Alien newAlien = GameInfo.CreateAlien(aliens[i], false, 1000, 1000, ALIEN_WIDTH,
                    ALIEN_HEIGHT, null);
                    newAlien.ShowHealthbar = false;
                    drawAliens.Add(newAlien);
                    unlocks.Add(Language.Translate("New Alien") + ": " +
                        Language.Translate(GameInfo.ToText(aliens[i])));
                    unlockPositions.Add(new Vector2());
                }

            }
            // Now check and add projectiles
            if (GameInfo.ProjVisibilityLvls.ContainsValue(mission))
            {
                List<ProjectileType> projs = GameInfo.ProjVisibilityLvls
                    .Where(x => x.Value == mission)
                    .Select(kvp => kvp.Key)
                    .ToList();
                for (int i = 0; i < projs.Count; i++)
                {
                    projImgs.Add(Utilities.GetIconOf(projs[i]));
                    projRects.Add(new Rectangle(0, 0, ITEM_SIZE, ITEM_SIZE));
                    unlocks.Add(Language.Translate("New Projectile") + ": " +
                        Language.Translate(projs[i].ToString().AddSpaces()));
                    unlockPositions.Add(new Vector2());
                }
            }
            // Finally, check for special features added in levels (such as shields or cannon upgrades)
            if (mission == GameInfo.SHIELD_LVL)
            {
                otherUnlockImgs.Add(Utilities.AlienShieldImg);
                otherUnlockRects.Add(new Rectangle(0, 0, ITEM_SIZE, ITEM_SIZE));
                unlocks.Add(Language.Translate("Alien Shields"));
                unlockPositions.Add(new Vector2());
            }
        }

        private void Position()
        {
            int lastY = unlockBgRect.Y + SPACING;
            int x = unlockBgRect.X + SPACING;
            for (int i = 0; i < drawAliens.Count; i++)
            {
                drawAliens[i].X = x;
                drawAliens[i].Y = lastY;
                lastY += ALIEN_HEIGHT + SPACING / 2;
                unlockPositions[i] = new Vector2(drawAliens[i].X + ALIEN_WIDTH, drawAliens[i].Y);
            }
            for (int i = 0; i < projRects.Count; i++)
            {
                projRects[i] = new Rectangle(x, lastY, ITEM_SIZE, ITEM_SIZE);
                lastY += projRects[i].Height + SPACING / 2;
                // We add the count of the drawAliens list to get the position value for THIS
                // projectile. If we simply used "i", then the value of the alien text positioning
                // would be ruined
                unlockPositions[i + drawAliens.Count] = new Vector2(projRects[i].X + ALIEN_WIDTH, projRects[i].Y);
            }
            for (int i = 0; i < otherUnlockRects.Count; i++)
            {
                Rectangle newRect = new Rectangle(0, 0, otherUnlockRects[i].Width, otherUnlockRects[i].Height);
                newRect.X = x;
                newRect.Y = lastY;
                otherUnlockRects[i] = newRect;
                lastY += otherUnlockRects[i].Height + SPACING / 2;
                // Like before, we add the counts of the previous lists to set the value for THIS 
                // item
                unlockPositions[i + drawAliens.Count + projImgs.Count] = new Vector2(otherUnlockRects[i].X + ALIEN_WIDTH,
                    otherUnlockRects[i].Y);
            }
        }

        #endregion
    }

    public static class LevelInfo
    {
        #region Fields

        static int windowWidth;
        static int windowHeight;
        static SpriteFont font;
        static GraphicsDevice graphics;

        const int GOAL_WIDTH = 600;
        const int GOAL_HEIGHT = 150;

        static List<LevelGoalPopup> goalPopups = new List<LevelGoalPopup>();

        public static bool HasActiveGoalPopups
        {
            get
            {
                for (int i = 0; i < goalPopups.Count; i++)
                {
                    if (goalPopups[i].Active)
                    {
                        // At least one popup is active
                        return true;
                    }
                }
                return false;
            }
        }

        #endregion

        #region Public Methods

        public static void Initialize(int windowWidth, int windowHeight, SpriteFont font, GraphicsDevice graphics)
        {
            LevelInfo.windowWidth = windowWidth;
            LevelInfo.windowHeight = windowHeight;
            LevelInfo.font = font;
            LevelInfo.graphics = graphics;
        }

        public static void ShowGoalPopup(MissionGoal goal, int amount)
        {
            LevelGoalPopup newP = new LevelGoalPopup(graphics, font, GOAL_WIDTH, GOAL_HEIGHT);
            newP.Show(windowWidth, windowHeight, goal, amount);
            goalPopups.Add(newP);
        }
        public static void ShowGoalPopup(MissionGoal goal, int amount, System.Action onHide)
        {
            LevelGoalPopup newP = new LevelGoalPopup(graphics, font, GOAL_WIDTH, GOAL_HEIGHT);
            newP.AddOnHideHandler(onHide);
            newP.Show(windowWidth, windowHeight, goal, amount);
            goalPopups.Add(newP);
        }

        public static void Update(GameTime gameTime)
        {
            for (int i = 0; i < goalPopups.Count; i++)
            {
                goalPopups[i].Update(gameTime);
                if (!goalPopups[i].Active)
                {
                    goalPopups.RemoveAt(i);
                }
            }
        }

        public static void Draw(SpriteBatch spriteBatch)
        {
            for (int i = 0; i < goalPopups.Count; i++)
            {
                goalPopups[i].Draw(spriteBatch);
            }
        }

        public static void ClearGoalPopups()
        {
            goalPopups.Clear();
        }

        #endregion
    }
}