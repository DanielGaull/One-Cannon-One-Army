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
        SpriteFont font;
        const int BG_WIDTH = 600;
        const int BG_HEIGHT = 500;

        string hotbarLabel;
        Vector2 hotbarLblLoc;
        HotbarDisplay hotbarDisplay;

        const int SPACING = 10;

        #endregion

        #region Constructors

        public PreLvlPopup(SpriteFont font, List<ProjectileType> projectileHotbar, List<int> hotbarCounts, int windowWidth,
            int windowHeight)
        {
            bgImg = Utilities.RectImage;
            bgRect = new Rectangle(windowWidth / 2 - BG_WIDTH / 2, windowHeight / 2 - BG_HEIGHT / 2, BG_WIDTH, BG_HEIGHT);

            hotbarLabel = Language.Translate("Hotbar");
            hotbarLblLoc = new Vector2(bgRect.X + SPACING, bgRect.Y + SPACING);
            hotbarDisplay = new HotbarDisplay(projectileHotbar, hotbarCounts, font, (int)hotbarLblLoc.X,
                (int)(hotbarLblLoc.Y + font.MeasureString(hotbarLabel).Y + SPACING / 2));
        }

        #endregion

        #region Public Methods

        public void Update()
        {

        }
        public void Draw(SpriteBatch spriteBatch)
        {
            spriteBatch.Draw(bgImg, bgRect, new Color(25, 25, 25));
            spriteBatch.DrawString(font, hotbarLabel, hotbarLblLoc, Color.White);
            hotbarDisplay.Draw(spriteBatch);
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
                slots.Add(new HotbarSlotDisplay(projectiles[i], counts[i], x, y, font));
                x += HotbarSlotDisplay.SIZE + SPACING;
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
        public const int SIZE = 25;
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
            spriteBatch.DrawString(font, countLabel, countLblLoc, Color.Black);
        }
    }
}
