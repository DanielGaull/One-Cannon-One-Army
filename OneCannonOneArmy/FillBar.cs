using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Media;

namespace OneCannonOneArmy
{
    public class FillBar // Previously known as XpBar
    {
        #region Fields & Properties

        Texture2D whiteRectImg;
        Rectangle bgRect;
        Rectangle interiorRect;
        float PIXELS_PER_VALUE = 0.0f;
        const int SPACING = 3;
        float value = 0.0f;
        float maxValue = 0.0f;
        SpriteFont font;

        public bool ShowFraction = true;

        public float Value
        {
            get
            {
                return value;
            }
            set
            {
                this.value = value;
                interiorRect.Width = (int)(this.value * PIXELS_PER_VALUE);
                if (value == maxValue)
                {
                    interiorRect.Width = bgRect.Width;
                }
            }
        }
        public float MaxValue
        {
            get
            {
                return maxValue;
            }
            set
            {
                maxValue = value;
                PIXELS_PER_VALUE = bgRect.Width / maxValue;
                interiorRect.Width = (int)(this.value * PIXELS_PER_VALUE);
                if (value == maxValue)
                {
                    interiorRect.Width = bgRect.Width;
                }
            }
        }

        public int X
        {
            get
            {
                return bgRect.X;
            }
            set
            {
                bgRect.X = value;
                interiorRect.X = value;
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
                interiorRect.Y = value;
            }
        }
        public int Width
        {
            get
            {
                return bgRect.Width;
            }
            set
            {
                bgRect.Width = value;
            }
        }
        public int Height
        {
            get
            {
                return bgRect.Height;
            }
            set
            {
                bgRect.Height = value;
            }
        }

        #endregion

        #region Constructors

        public FillBar(int value, int x, int y, int width, int height, float maxValue, SpriteFont font)
        {
            this.value = value;
            this.maxValue = maxValue;
            this.font = font;

            whiteRectImg = Utilities.RectImage;

            bgRect = new Rectangle(x, y, width, height);
            PIXELS_PER_VALUE = bgRect.Width / maxValue;

            interiorRect = new Rectangle(x, y, (int)(this.value * PIXELS_PER_VALUE), height);
            if (value == maxValue)
            {
                interiorRect.Width = bgRect.Width;
            }
        }

        #endregion

        #region Public Methods

        public void Draw(SpriteBatch spriteBatch, Color fillColor, Color textColor)
        {
            spriteBatch.Draw(whiteRectImg, bgRect, Color.DarkGray);
            spriteBatch.Draw(whiteRectImg, interiorRect, fillColor);
            if (font != null && ShowFraction)
            {
                spriteBatch.DrawString(font, value + "/" + maxValue, new Vector2(bgRect.X, bgRect.Y), textColor);
            }
        }

        public void Increase(int amount)
        {
            if (value + amount >= 0)
            {
                // Make sure there are no negative values allowed
                value += amount;
                interiorRect.Width = (int)(value * PIXELS_PER_VALUE);
                if (value == maxValue)
                {
                    interiorRect.Width = bgRect.Width;
                }
            }
            else
            {
                value = 0;
                interiorRect.Width = 0;
            }
        }

        #endregion
    }
}
