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
        public Quest(QuestGoalType goal, int typeId, int goalNum, int rewardCoins)
        {
            GoalType = goal;
            TypeId = typeId;
            GoalNumber = goalNum;
            RewardCoins = rewardCoins;
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
            ContentManager content)
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
