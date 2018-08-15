using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OneCannonOneArmy
{
    [Serializable]
    public struct Quest
    {
        public QuestGoalType GoalType;
        public int TypeId;
        public int GoalNumber;
        public int RewardCoins;
        #region Range Constants
        const int LEVELCOMPLETE_MIN = 3;
        const int LEVELCOMPLETE_MAX = 7;
        const int ALIENKILL_MIN = 10;
        const int ALIENKILL_MAX = 20;
        const int PROJFIRE_MIN = 25;
        const int PROJFIRE_MAX = 100;
        const int PROJCRAFT_MIN = 25;
        const int PROJCRAFT_MAX = 50;
        const int COINSPEND_MIN = 1000;
        const int COINSPEND_MAX = 10000;
        const int COINGET_MIN = 1000;
        const int COINGET_MAX = 5000;
        const int BUYMATERIAL_MIN = 50;
        const int BUYMATERIAL_MAX = 100;
        // This value is used as a factor for some of the number values
        // For example, we want material buy goals to be divisible by 5
        const int GOAL_NUM_FACTOR = 5;
        #endregion
        #region Reward Constants
        const float LEVELCOMPLETE_COINS = 50;
        const float ALIENKILL_COINS = 10;
        const float PROJFIRE_COINS = 2;
        const float PROJCRAFT_COINS = 3;
        const float MATERIALBUY_COINS = 3;
        const float COINGET_COINS = 0.2f;
        const float COINSPEND_COINS = 0.1f;
        #endregion
        public Quest(QuestGoalType goal, int typeId, int goalNum, int rewardCoins)
        {
            GoalType = goal;
            TypeId = typeId;
            GoalNumber = goalNum;
            RewardCoins = rewardCoins;
        }

        public static Quest Random(User user)
        {
            QuestGoalType goalType = Enum.GetValues(typeof(QuestGoalType)).Cast<QuestGoalType>().ToList().Random();
            int numGoal = 0;
            int coinReward = 0;
            int typeId = 0;
            switch (goalType)
            {
                case QuestGoalType.BeatLevels:
                    numGoal = Utilities.Rand.Next(LEVELCOMPLETE_MIN, LEVELCOMPLETE_MAX + 1);
                    coinReward = (int)(numGoal * LEVELCOMPLETE_COINS);
                    break;
                case QuestGoalType.BuyMaterials:
                    numGoal = Utilities.Rand.Next(BUYMATERIAL_MIN / GOAL_NUM_FACTOR, 
                        BUYMATERIAL_MAX / GOAL_NUM_FACTOR + 1) * GOAL_NUM_FACTOR;
                    coinReward = (int)(numGoal * MATERIALBUY_COINS);
                    int materials = Enum.GetValues(typeof(Material)).Cast<Material>().Count();
                    typeId = Utilities.Rand.Next(0, materials);
                    break;
                case QuestGoalType.CraftProjectiles:
                    numGoal = Utilities.Rand.Next(PROJCRAFT_MIN / GOAL_NUM_FACTOR,
                        PROJCRAFT_MAX / GOAL_NUM_FACTOR + 1) * GOAL_NUM_FACTOR;
                    coinReward = (int)(numGoal * PROJCRAFT_COINS);
                    List<ProjectileType> allProjectiles = Enum.GetValues(typeof(ProjectileType)).Cast<ProjectileType>().ToList();
                    ProjectileType type = allProjectiles.Where(x => user.ProjectileInventory.Contains(x) &&
                        GameInfo.CanSee(user, x)).ToList().Random();
                    typeId = Utilities.Rand.Next(0, allProjectiles.IndexOf(type));
                    break;
                case QuestGoalType.FireProjectiles:
                    numGoal = Utilities.Rand.Next(PROJFIRE_MIN / GOAL_NUM_FACTOR,
                        PROJFIRE_MAX / GOAL_NUM_FACTOR + 1) * GOAL_NUM_FACTOR;
                    coinReward = (int)(numGoal * PROJFIRE_COINS);
                    allProjectiles = GameInfo.ProjectilesAllowed;
                    type = allProjectiles.Where(x => GameInfo.CanSee(user, x)).ToList().Random();
                    typeId = Utilities.Rand.Next(0, allProjectiles.IndexOf(type));
                    break;
                case QuestGoalType.KillAliens:
                    numGoal = Utilities.Rand.Next(ALIENKILL_MIN / GOAL_NUM_FACTOR,
                        ALIENKILL_MAX / GOAL_NUM_FACTOR + 1) * GOAL_NUM_FACTOR;
                    coinReward = (int)(numGoal * ALIENKILL_COINS);
                    List<BasicAlienTypes> allAliens = Enum.GetValues(typeof(BasicAlienTypes)).Cast<BasicAlienTypes>().ToList();
                    BasicAlienTypes alienType = allAliens.Where(x => GameInfo.CanSee(user, x)).ToList().Random();
                    typeId = Utilities.Rand.Next(0, allAliens.IndexOf(alienType));
                    break;
                case QuestGoalType.ObtainCoins:
                    numGoal = Utilities.Rand.Next(COINGET_MIN / GOAL_NUM_FACTOR,
                        COINGET_MAX / GOAL_NUM_FACTOR + 1) * GOAL_NUM_FACTOR;
                    coinReward = (int)(numGoal * COINGET_COINS);
                    break;
                case QuestGoalType.SpendCoins:
                    numGoal = Utilities.Rand.Next(COINSPEND_MIN / GOAL_NUM_FACTOR,
                        COINSPEND_MAX / GOAL_NUM_FACTOR + 1) * GOAL_NUM_FACTOR;
                    coinReward = (int)(numGoal * COINSPEND_COINS);
                    break;
            }
            return new Quest(goalType, typeId, numGoal, coinReward);
        }

        public override string ToString()
        {
            switch (GoalType)
            {
                case QuestGoalType.BeatLevels:
                    return Language.Translate("Complete") + " " + GoalNumber + " " + Language.Translate("levels") + ".";
                case QuestGoalType.BuyMaterials:
                    return Language.Translate("Buy") + " " + GoalNumber + " " + 
                        Language.Translate(((Material)TypeId).ToString()) + ".";
                case QuestGoalType.CraftProjectiles:
                    return Language.Translate("Craft") + " " + GoalNumber + " " +
                        Language.Translate(((ProjectileType)TypeId).ToString()) + ".";
                case QuestGoalType.FireProjectiles:
                    return Language.Translate("Fire") + " " + GoalNumber + " " +
                        Language.Translate(((ProjectileType)TypeId).ToString()) + ".";
                case QuestGoalType.KillAliens:
                    return Language.Translate("Kill") + " " + GoalNumber + " " +
                        Language.Translate(((BasicAlienTypes)TypeId).ToString().AddSpaces()) + " " + Language.Translate("Aliens") + ".";
                case QuestGoalType.ObtainCoins:
                    return Language.Translate("Collect") + " " + GoalNumber + " " + Language.Translate("coins") + ".";
                case QuestGoalType.SpendCoins:
                    return Language.Translate("Spend") + " " + GoalNumber + " " + Language.Translate("coins") + ".";
            }
            return null;
        }
    }

    public class QuestInterface
    {
        MenuButton questButton;
        string labelText;
        Vector2 labelLoc;
        SpriteFont font;
        const int SPACING = 10;
        const int BUTTON_SIZE = 50;
        QuestPopup popup;
        User user;
        public int X
        {
            get
            {
                return questButton.X;
            }
            set
            {
                questButton.X = value;
                labelLoc = new Vector2(questButton.X + questButton.Width + SPACING, questButton.Y + questButton.Height / 2 -
                    font.MeasureString(labelText).Y / 2);
            }
        }
        public int Y
        {
            get
            {
                return questButton.Y;
            }
            set
            {
                questButton.Y = value;
                labelLoc = new Vector2(questButton.X + questButton.Width + SPACING, questButton.Y + questButton.Height / 2 -
                    font.MeasureString(labelText).Y / 2);
            }
        }
        public int Width
        {
            get
            {
                return questButton.Width + (int)font.MeasureString(labelText).X + SPACING;
            }
        }
        public int Height
        {
            get
            {
                return questButton.Height;
            }
        }
        public bool ShowingPopup
        {
            get
            {
                return popup.Showing;
            }
        }
        public QuestInterface(GraphicsDevice graphics, Texture2D questImg, int x, int y, SpriteFont font, User user, 
            int windowWidth, int windowHeight, Texture2D checkImg, Texture2D coinImg)
        {
            this.font = font;
            this.user = user;
            questButton = new MenuButton(OnClick, x, y, true, graphics, questImg);
            questButton.Width = questButton.Height = BUTTON_SIZE;
            questButton.ImgWidth = questButton.ImgHeight = BUTTON_SIZE - SPACING;
            labelText = Language.Translate("Quests");
            labelLoc = new Vector2(questButton.X + questButton.Width + SPACING, questButton.Y + questButton.Height / 2 -
                font.MeasureString(labelText).Y / 2);

            popup = new QuestPopup(graphics, font, windowWidth, windowHeight, user,
                checkImg, coinImg);
        }
        public void Update(User user)
        {
            this.user = user;
            if (popup.Showing)
            {
                popup.Update();
            }
            else
            {
                questButton.Update();
            }
        }
        public void Draw(SpriteBatch spriteBatch)
        {
            questButton.Draw(spriteBatch);
            spriteBatch.DrawString(font, labelText, labelLoc, Color.White);
            if (popup.Showing)
            {
                popup.Draw(spriteBatch);
            }
        }

        public void OnLangChange()
        {
            labelText = Language.Translate("Quests");
            popup.LangChanged();
        }
        public List<MenuButton> GetButtons()
        {
            List<MenuButton> returnVal = new List<MenuButton>();
            if (!popup.Showing)
            {
                returnVal.Add(questButton);
            }
            else
            {
                returnVal.AddRange(popup.GetButtons());
            }
            return returnVal;
        }

        private void OnClick()
        {
            popup.Show(user);
        }
    }
    public class QuestPopup
    {
        #region Fields and Properties

        Quest quest;

        string questText = "";
        Vector2 questTextLoc;
        SpriteFont font;

        FillBar progressBar;
        const int PROGRESSBAR_WIDTH = 300;
        const int PROGRESSBAR_HEIGHT = 25;

        Texture2D bgImg;
        Rectangle bgRect;
        const int WIDTH = 500;
        const int HEIGHT = 400;
        const int BORDER_SIZE = 5;
        const int SPACING = 10;
        const int BIG_SPACING = 50;

        Texture2D checkImg;
        Rectangle checkRect;
        bool completed;
        const int CHECK_SIZE = 50;

        Texture2D coinIcon;
        Rectangle coinRect;
        string rewardString;
        Vector2 rewardLoc;
        const int COIN_SIZE = 25;

        MenuButton closeButton;

        public bool Showing { get; private set; }

        #endregion

        #region Constructors

        public QuestPopup(GraphicsDevice graphics, SpriteFont font, int windowWidth, int windowHeight, 
            User user, Texture2D checkImg, Texture2D coinIcon)
        {
            this.font = font;

            bgImg = DrawHelper.AddBorder(new Texture2D(graphics, WIDTH, HEIGHT), BORDER_SIZE, Color.Gray, Color.LightGray);
            bgRect = new Rectangle(windowWidth / 2 - WIDTH / 2, windowHeight / 2 - HEIGHT / 2, WIDTH, HEIGHT);

            questTextLoc.Y = bgRect.Y + BORDER_SIZE + SPACING * 2;

            progressBar = new FillBar(0, bgRect.X + bgRect.Width / 2 - PROGRESSBAR_WIDTH / 2,
                (int)(questTextLoc.Y + font.MeasureString(questText).Y + BIG_SPACING), PROGRESSBAR_WIDTH, PROGRESSBAR_HEIGHT,
                quest.GoalNumber, font);
            progressBar.ShowFraction = true;

            this.checkImg = checkImg;
            checkRect = new Rectangle(bgRect.X + BORDER_SIZE + SPACING, bgRect.Y + bgRect.Height / 2 - CHECK_SIZE / 2,
                CHECK_SIZE, CHECK_SIZE);
            completed = false;

            this.coinIcon = coinIcon;
            rewardString = Language.Translate("Reward") + ": " + 0;
            coinRect = new Rectangle((int)(bgRect.X + bgRect.Width / 2 - (font.MeasureString(rewardString).X + COIN_SIZE) / 2),
                progressBar.Y + progressBar.Height + BIG_SPACING, COIN_SIZE, COIN_SIZE);
            rewardLoc = new Vector2(coinRect.Right, coinRect.Y);

            closeButton = new MenuButton(Hide, Language.Translate("Close"), 0, 0, true, font, graphics);
            closeButton.X = bgRect.X + bgRect.Width / 2 - closeButton.Width / 2;
            closeButton.Y = bgRect.Bottom - closeButton.Height - BORDER_SIZE - SPACING;

            if (user != null)
            {
                Initialize(user);
            }
        }

        #endregion

        #region Public Methods

        public void Update()
        {
            closeButton.Update();
        }
        public void Draw(SpriteBatch spriteBatch)
        {
            spriteBatch.Draw(bgImg, bgRect, Color.White);
            spriteBatch.DrawString(font, questText, questTextLoc, Color.White);
            progressBar.Draw(spriteBatch, Color.Lime, Color.White);
            if (completed)
            {
                spriteBatch.Draw(checkImg, checkRect, Color.White);
            }
            closeButton.Draw(spriteBatch);
            spriteBatch.Draw(coinIcon, coinRect, Color.White);
            spriteBatch.DrawString(font, rewardString, rewardLoc, Color.Goldenrod);
        }

        public void LangChanged()
        {
            questText = quest.ToString();
            questTextLoc.X = bgRect.X + bgRect.Width / 2 - font.MeasureString(questText).X / 2;

            rewardString = Language.Translate("Reward") + ": " + quest.RewardCoins.ToString();
            coinRect = new Rectangle((int)(bgRect.X + bgRect.Width / 2 - (font.MeasureString(rewardString).X + COIN_SIZE) / 2),
                progressBar.Y + progressBar.Height + BIG_SPACING, COIN_SIZE, COIN_SIZE);
            rewardLoc = new Vector2(coinRect.Right, coinRect.Y);
        }
        public List<MenuButton> GetButtons()
        {
            return new List<MenuButton>() { closeButton };
        }

        public void Show(User user)
        {
            Initialize(user);
            Showing = true;
        }
        public void Hide()
        {
            Showing = false;
        }

        #endregion

        #region Private Methods

        private void Initialize(User user)
        {
            quest = user.CurrentQuest;
            completed = user.QuestProgress >= quest.GoalNumber;

            rewardString = Language.Translate("Reward") + ": " + quest.RewardCoins.ToString();
            coinRect = new Rectangle((int)(bgRect.X + bgRect.Width / 2 - (font.MeasureString(rewardString).X + COIN_SIZE) / 2),
                progressBar.Y + progressBar.Height + BIG_SPACING, COIN_SIZE, COIN_SIZE);
            rewardLoc = new Vector2(coinRect.Right, coinRect.Y);

            progressBar.MaxValue = user.CurrentQuest.GoalNumber;
            progressBar.Value = user.QuestProgress;

            questText = quest.ToString();
            questTextLoc = new Vector2(bgRect.X + bgRect.Width / 2 - font.MeasureString(questText).X / 2,
                bgRect.Y + BORDER_SIZE + SPACING * 2);
        }

        #endregion
    }

    public enum QuestGoalType
    {
        KillAliens,
        FireProjectiles,
        CraftProjectiles,
        BuyMaterials,
        SpendCoins,
        ObtainCoins,
        BeatLevels,
    }
}
