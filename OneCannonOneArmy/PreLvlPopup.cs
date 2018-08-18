using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OneCannonOneArmy
{
    public class PreLvlPopup
    {
        #region Public Methods

        Texture2D bgImg;
        Rectangle bgRect;
        const int BG_WIDTH = 600;
        const int BG_HEIGHT = 500;

        SpriteFont smallFont;
        SpriteFont mediumFont;

        string hotbarLabel;
        Vector2 hotbarLblLoc;
        HotbarDisplay hotbarDisplay;
        LevelObjectiveDisplay goalDisplay;

        CannonDisplay cannonDisplay;

        const int SPACING = 10;

        MenuButton proceedButton;
        MenuButton cancelButton;

        string chanceOfSuccess;
        Vector2 chanceLoc;

        #endregion

        #region Constructors

        public PreLvlPopup(SpriteFont smallFont, SpriteFont mediumFont, List<ProjectileType> projectileHotbar,
            List<int> hotbarCounts, int windowWidth, int windowHeight, Action proceed, Action cancel, GraphicsDevice graphics, 
            CannonSettings cannon, Mission mission, User user)
        {
            this.smallFont = smallFont;
            this.mediumFont = mediumFont;

            bgImg = DrawHelper.AddBorder(new Texture2D(graphics, BG_WIDTH, BG_HEIGHT), 5, new Color(25, 25, 25),
                new Color(45, 45, 45));
            bgRect = new Rectangle(windowWidth / 2 - BG_WIDTH / 2, windowHeight / 2 - BG_HEIGHT / 2, BG_WIDTH, BG_HEIGHT);

            hotbarLabel = Language.Translate("Hotbar");
            hotbarLblLoc = new Vector2(bgRect.X + SPACING, bgRect.Y + SPACING);
            hotbarDisplay = new HotbarDisplay(projectileHotbar, hotbarCounts, smallFont, (int)hotbarLblLoc.X,
                (int)(hotbarLblLoc.Y + mediumFont.MeasureString(hotbarLabel).Y + SPACING / 2));

            cannonDisplay = new CannonDisplay(cannon, smallFont, bgRect.Right - SPACING, bgRect.Y + SPACING);

            goalDisplay = new LevelObjectiveDisplay(mediumFont, bgRect.X + bgRect.Width / 2 - LevelObjectiveDisplay.SIZE / 2, 
                cannonDisplay.Bottom, mission);

            proceedButton = new MenuButton(proceed, Language.Translate("Proceed"), 0, 0, true, mediumFont, graphics);
            cancelButton = new MenuButton(cancel, Language.Translate("Cancel"), 0, 0, true, mediumFont, graphics);
            proceedButton.X = bgRect.X + (bgRect.Width / 2 - proceedButton.Width);
            cancelButton.X = bgRect.X + (bgRect.Width / 2 + SPACING);
            proceedButton.Y = cancelButton.Y = bgRect.Bottom - SPACING - proceedButton.Height;

            chanceOfSuccess = Language.Translate("Estimated Chance of Success") + ": " + 
                ChanceOfSuccess(user, mission).ToString() + "%";
            chanceLoc = new Vector2(bgRect.X + bgRect.Width / 2 - mediumFont.MeasureString(chanceOfSuccess).X / 2, 
                goalDisplay.Bottom + SPACING);
        }

        #endregion

        #region Public Methods

        public void Update(GameTime gameTime)
        {
            proceedButton.Update(gameTime);
            cancelButton.Update(gameTime);
        }
        public void Draw(SpriteBatch spriteBatch)
        {
            spriteBatch.Draw(bgImg, bgRect, Color.White);

            spriteBatch.DrawString(mediumFont, hotbarLabel, hotbarLblLoc, Color.White);
            hotbarDisplay.Draw(spriteBatch);
            cannonDisplay.Draw(spriteBatch);
            goalDisplay.Draw(spriteBatch);
            spriteBatch.DrawString(mediumFont, chanceOfSuccess, chanceLoc, Color.White);

            proceedButton.Draw(spriteBatch);
            cancelButton.Draw(spriteBatch);
        }

        public List<MenuButton> GetButtons()
        {
            return new List<MenuButton>()
            {
                proceedButton,
                cancelButton
            };
        }

        #endregion

        #region Private Methods
        
        private int CalculateDamageAvailable(User user)
        {
            int damage = 0;
            for (int i = 0; i < user.Hotbar.Count; i++)
            {
                damage += Utilities.DamageOf(user.Hotbar[i]) * user.ProjectileInventory.CountOf(user.Hotbar[i]);
                // Add the status effect damage * 2
                if (Utilities.DamagingEffectsOf(user.Hotbar[i]).Count > 0)
                {
                    damage += StatusEffect.TotalDamage(Utilities.DamagingEffectsOf(user.Hotbar[i])) * 2;
                }
            }
            return damage;
        }

        private float ChanceOfSuccess(User user, Mission mission)
        {
            // Simple equation, multiply total damage by accuracy, then divide by required damage and clamp
            float chance = (CalculateDamageAvailable(user) * (user.Accuracy / 100.0f)) / Mission.DamageForLevel(mission);
            chance *= 100.0f;
            // Clamp the value to be between 0 and 100, then reduce it to 2 decimal digits
            chance = (float)Math.Round(MathHelper.Clamp(chance, 0, 100), 2);
            return chance;
        }

        #endregion
    }

    public class HotbarDisplay
    {
        List<HotbarSlotDisplay> slots = new List<HotbarSlotDisplay>();
        const int SPACING = 10;
        public HotbarDisplay(List<ProjectileType> projectiles, List<int> counts, SpriteFont font,
            int x, int y)
        {
            for (int i = 0; i < projectiles.Count; i++)
            {
                if (projectiles[i] != ProjectileType.None)
                {
                    slots.Add(new HotbarSlotDisplay(projectiles[i], counts[i], x, y, font));
                    x += HotbarSlotDisplay.SIZE + SPACING;
                }
            }
        }
        public void Draw(SpriteBatch spriteBatch)
        {
            for (int i = 0; i < slots.Count; i++)
            {
                slots[i].Draw(spriteBatch);
            }
        }
    }
    public class HotbarSlotDisplay
    {
        Texture2D projImg;
        Rectangle projRect;
        int count;
        string countLabel;
        Vector2 countLblLoc;
        SpriteFont font;
        public const int SIZE = 50;
        public int X
        {
            get
            {
                return projRect.X;
            }
            set
            {
                projRect.X = value;
            }
        }
        public int Y
        {
            get
            {
                return projRect.Y;
            }
            set
            {
                projRect.Y = value;
            }
        }

        public HotbarSlotDisplay(ProjectileType projectile, int count, int x, int y, SpriteFont font)
        {
            projImg = Utilities.GetIconOf(projectile);
            projRect = new Rectangle(x, y, SIZE, SIZE);
            this.count = count;
            countLabel = "x" + count;
            this.font = font;
            countLblLoc = new Vector2(projRect.Right - font.MeasureString(countLabel).X / 2,
                projRect.Bottom - font.MeasureString(countLabel).Y / 2);
        }

        public void Draw(SpriteBatch spriteBatch)
        {
            spriteBatch.Draw(projImg, projRect, Color.White);
            spriteBatch.DrawString(font, countLabel, countLblLoc, Color.White);
        }
    }

    public class CannonDisplay
    {
        #region Fields and Properties

        Texture2D cannonBottom;
        Rectangle cannonBottomRect;
        Texture2D cannonTube;
        Rectangle cannonTubeRect;

        SpriteFont font;

        List<NameValue> stats = new List<NameValue>();
        List<Vector2> statLocs = new List<Vector2>();

        const int TUBE_WIDTH = 30;
        const int TUBE_HEIGHT = 150;
        const int BOTTOM_WIDTH = 90;
        const int BOTTOM_HEIGHT = 55;

        const int SPACING = 3;
        const int BIG_SPACING = 10;

        public int Bottom
        {
            get
            {
                return cannonBottomRect.Bottom;
            }
        }

        #endregion

        #region Constructors

        public CannonDisplay(CannonSettings cannon, SpriteFont font, int rightX, int y)
        {
            this.font = font;

            cannonBottom = Utilities.GetBottomOfCannon(cannon.CannonType);
            cannonTube = Utilities.GetTubeOfCannon(cannon.CannonType);
            cannonTubeRect = new Rectangle(0, 0, TUBE_WIDTH, TUBE_HEIGHT);
            cannonBottomRect = new Rectangle(0, 0, BOTTOM_WIDTH, BOTTOM_HEIGHT);

            stats = new List<NameValue>()
            {
                new NameValue("Damage", cannon.Damage.ToString()),
                new NameValue("Health", cannon.MaxHealth.ToString()),
                new NameValue("Reload Speed", cannon.ReloadSpeed.ToString()),
                new NameValue("Move Speed", cannon.MoveSpeed.ToString()),
                new NameValue("Accuracy", cannon.Accuracy.ToString()),
                new NameValue("Power", cannon.Power.ToString()),
                new NameValue("Defense", cannon.Defense.ToString()),
                new NameValue("Rapid Fire", cannon.RapidFire > 0 ? "Y" : "N"),
            };
            if (cannon.Freezes)
            {
                stats.Add(new NameValue("Effect Added", "Frozen"));
            }
            else
            {
                stats.Add(new NameValue("Effect Added", cannon.EffectAdded.ToString().AddSpaces()));
            }
            for (int i = 0; i < stats.Count; i++)
            {
                statLocs.Add(new Vector2());
            }
            Position(rightX, y);
        }

        #endregion

        #region Public Methods

        public void Draw(SpriteBatch spriteBatch)
        {
            spriteBatch.Draw(cannonTube, cannonTubeRect, Color.White);
            spriteBatch.Draw(Utilities.RectImage, cannonBottomRect, Color.White);
            spriteBatch.Draw(cannonBottom, cannonBottomRect, Color.White);
            for (int i = 0; i < stats.Count; i++)
            {
                spriteBatch.DrawString(font, stats[i].ToString(), statLocs[i], Color.White);
            }
        }

        #endregion

        #region Private Methods

        private void Position(int rightX, int y)
        {
            cannonBottomRect.X = rightX - cannonBottomRect.Width;
            cannonTubeRect.X = cannonBottomRect.X + cannonBottomRect.Width / 2 - cannonTubeRect.Width / 2;
            cannonTubeRect.Y = y;
            cannonBottomRect.Y = cannonTubeRect.Bottom;
            int maxStatWidth = (int)stats.Select(x => font.MeasureString(x.ToString()).X).Max();
            int statX = cannonBottomRect.X - maxStatWidth - BIG_SPACING;
            int statY = y;
            for (int i = 0; i < statLocs.Count; i++)
            {
                statLocs[i] = new Vector2(statX, statY);
                statY += (int)font.MeasureString(stats[i].ToString()).Y + SPACING;
            }
        }

        #endregion
    }

    public class LevelObjectiveDisplay
    {
        Texture2D goalImg;
        Rectangle goalRect;
        string subtitle;
        Vector2 subtitleLoc;
        SpriteFont font;
        public const int SIZE = 60;
        const int SPACING = 10;
        public int Bottom
        {
            get
            {
                return (int)(subtitleLoc.Y + font.MeasureString(subtitle).Y);
            }
        }

        public LevelObjectiveDisplay(SpriteFont font, int x, int y, Mission mission)
        {
            this.font = font;

            goalImg = Utilities.GetImgOfGoal(mission.Goal);
            goalRect = new Rectangle(x, y, SIZE, SIZE);

            int amount = 0;
            switch (mission.Goal)
            {
                case MissionGoal.DestroyMechanics:
                    amount = mission.MechAliens;
                    break;
                case MissionGoal.SavePeople:
                    amount = mission.Cages;
                    break;
            }
            subtitle = LevelGoalPopup.TextFor(mission.Goal, amount);
            subtitleLoc = new Vector2(x + SIZE / 2 - font.MeasureString(subtitle).X / 2, goalRect.Bottom + SPACING);
        }

        public void Draw(SpriteBatch spriteBatch)
        {
            spriteBatch.Draw(goalImg, goalRect, Color.White);
            spriteBatch.DrawString(font, subtitle, subtitleLoc, Color.White);
        }
    }
}
