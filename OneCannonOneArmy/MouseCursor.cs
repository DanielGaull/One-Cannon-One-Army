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
    public class MouseCursor
    {
        #region Fields

        Texture2D currentImg;
        Texture2D regImg;
        Texture2D clickImg;
        Texture2D textImg = null;

        Rectangle drawRect;

        const int WIDTH = 20;
        const int HEIGHT = 20;

        MouseState mouse;

        #endregion

        #region Constructors

        public MouseCursor(Texture2D regImg, Texture2D clickImg, Texture2D textImg)
        {
            this.regImg = regImg;
            this.clickImg = clickImg;
            this.textImg = textImg;
            currentImg = regImg;
            drawRect = new Rectangle(0, 0, WIDTH, HEIGHT);
        }

        #endregion

        #region Public Methods

        public void Update(List<MenuButton> buttons, List<Textbox> textboxes, List<ValueSlider> sliders, List<DraggableProjectile> projDraggables)
        {
            mouse = Mouse.GetState();
            drawRect.X = mouse.X;
            drawRect.Y = mouse.Y;

            currentImg = regImg;
            Rectangle pointerRect = new Rectangle(Mouse.GetState().X, Mouse.GetState().Y, 1, 1);
            foreach (MenuButton button in buttons)
            {
                if (button.DrawRectangle.Intersects(pointerRect) && button.Active)
                {
                    currentImg = clickImg;
                }
            }
            foreach (Textbox tbox in textboxes)
            {
                if (pointerRect.Intersects(tbox.DrawRectangle) && tbox.Drawn)
                {
                    currentImg = textImg;
                }
            }
            for (int i = 0; i < sliders.Count; i++)
            {
                if (pointerRect.Intersects(sliders[i].SliderRectangle))
                {
                    currentImg = clickImg;
                }
            }
            for (int i = 0; i < projDraggables.Count; i++)
            {
                if (pointerRect.Intersects(projDraggables[i].DrawRectangle))
                {
                    currentImg = clickImg;
                }
            }
        }

        public void Draw(SpriteBatch spriteBatch)
        {
            spriteBatch.Draw(currentImg, drawRect, Color.White);
        }

        #endregion
    }
}
