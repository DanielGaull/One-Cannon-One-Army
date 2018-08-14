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

        public static Quest Random()
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
                    int projectiles = Enum.GetValues(typeof(ProjectileType)).Cast<ProjectileType>().Count();
                    typeId = Utilities.Rand.Next(0, projectiles);
                    break;
                case QuestGoalType.FireProjectiles:
                    numGoal = Utilities.Rand.Next(PROJFIRE_MIN / GOAL_NUM_FACTOR,
                        PROJFIRE_MAX / GOAL_NUM_FACTOR + 1) * GOAL_NUM_FACTOR;
                    coinReward = (int)(numGoal * PROJFIRE_COINS);
                    projectiles = Enum.GetValues(typeof(ProjectileType)).Cast<ProjectileType>().Count();
                    typeId = Utilities.Rand.Next(0, projectiles);
                    break;
                case QuestGoalType.KillAliens:
                    numGoal = Utilities.Rand.Next(ALIENKILL_MIN / GOAL_NUM_FACTOR,
                        ALIENKILL_MAX / GOAL_NUM_FACTOR + 1) * GOAL_NUM_FACTOR;
                    coinReward = (int)(numGoal * ALIENKILL_COINS);
                    int aliens = Enum.GetValues(typeof(Aliens)).Cast<Aliens>().Count();
                    typeId = Utilities.Rand.Next(0, aliens);
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
                    return Language.Translate("Beat") + " " + GoalNumber + " " + Language.Translate("levels") + ".";
                case QuestGoalType.BuyMaterials:
                    return Language.Translate("Buy") + " " + GoalNumber + " " + 
                        Language.Translate(((Material)TypeId).ToString()) + ".";
                case QuestGoalType.CraftProjectiles:
                    return Language.Translate("Craft") + " " + GoalNumber + 
                        Language.Translate(((ProjectileType)TypeId).ToString()) + ".";
                case QuestGoalType.FireProjectiles:
                    return Language.Translate("Fire") + " " + GoalNumber +
                        Language.Translate(((ProjectileType)TypeId).ToString()) + ".";
                case QuestGoalType.KillAliens:
                    return Language.Translate("Kill") + " " + GoalNumber +
                        Language.Translate(((Aliens)TypeId).ToString().AddSpaces()) + ".";
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
        public QuestInterface(GraphicsDevice graphics, Texture2D questImg, int x, int y, SpriteFont font)
        {
            this.font = font;
            questButton = new MenuButton(OnClick, x, y, true, graphics, questImg);
            questButton.Width = questButton.Height = BUTTON_SIZE;
            questButton.ImgWidth = questButton.ImgHeight = BUTTON_SIZE - SPACING;
            labelText = Language.Translate("Quests");
            labelLoc = new Vector2(questButton.X + questButton.Width + SPACING, questButton.Y + questButton.Height / 2 - 
                font.MeasureString(labelText).Y / 2);
        }
        public void Update()
        {
            questButton.Update();
        }
        public void Draw(SpriteBatch spriteBatch)
        {
            questButton.Draw(spriteBatch);
            spriteBatch.DrawString(font, labelText, labelLoc, Color.White);
        }
        public void OnLangChange()
        {
            labelText = Language.Translate("Quests");
        }

        private void OnClick()
        {
            // Show the quest popup
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
        const int PROGRESSBAR_HEIGHT = 10;

        Texture2D bgImg;
        Rectangle bgRect;
        const int WIDTH = 500;
        const int HEIGHT = 400;
        const int BORDER_SIZE = 5;
        const int SPACING = 10;
        const int BIG_SPACING = 50;

        #endregion

        #region Constructors

        public QuestPopup(GraphicsDevice graphics, SpriteFont font, Quest quest, int windowWidth, int windowHeight, 
            ContentManager content, User user)
        {
            this.quest = quest;

            bgImg = DrawHelper.AddBorder(new Texture2D(graphics, WIDTH, HEIGHT), BORDER_SIZE, Color.Gray, Color.LightGray);
            bgRect = new Rectangle(windowWidth / 2 - WIDTH / 2, windowHeight / 2 - HEIGHT / 2, WIDTH, HEIGHT);

            questText = quest.ToString();
            questTextLoc = new Vector2(bgRect.X + bgRect.Width / 2 - font.MeasureString(questText).X / 2, 
                bgRect.Y + BORDER_SIZE + SPACING * 2);

            progressBar = new FillBar(0, bgRect.X + bgRect.Width / 2 - PROGRESSBAR_WIDTH / 2,
                (int)(questTextLoc.Y + font.MeasureString(questText).Y + BIG_SPACING), PROGRESSBAR_WIDTH, PROGRESSBAR_HEIGHT,
                quest.GoalNumber, font);
            progressBar.ShowFraction = true;
        }

        #endregion

        #region Public Methods

        public void LangChanged()
        {
            questText = quest.ToString();
            questTextLoc.X = bgRect.X + bgRect.Width / 2 - font.MeasureString(questText).X / 2;
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
