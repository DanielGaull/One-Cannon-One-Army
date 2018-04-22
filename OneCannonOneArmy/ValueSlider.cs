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
    public class ValueSlider
    {
        #region Fields & Properties

        float value = 0.0f;
        public float Value
        {
            get
            {
                return value;
            }
            set
            {
                this.value = value;
                SliderRectangle.X = (int)(value * PIXELS_PER_VALUE) + bgRect.X - (SliderRectangle.Width / 2);
            }
        }
        public string ValueName = "";
        public bool ShowName = true;

        Texture2D image;
        Rectangle bgRect;
        public Rectangle SliderRectangle;
        const int SLIDER_WIDTH = 30;
        const int SPACING = 4;

        // The x value to increase by for every 1 added to value
        float PIXELS_PER_VALUE = 0.0f;

        SpriteFont font;

        MouseState mouse;
        bool sliding = false;

        public int X
        {
            get
            {
                return bgRect.X;
            }
            set
            {
                bgRect.X = value;
                SliderRectangle.X = (int)(this.value * PIXELS_PER_VALUE) + bgRect.X - (SliderRectangle.Width / 2);
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
                SliderRectangle.Y = (int)(this.value - (SliderRectangle.Height - (SPACING / 2)));
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
                PIXELS_PER_VALUE = value / 100.0f;
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

        event System.Action valueChanged;

        #endregion

        #region Constructors

        public ValueSlider(int value, Texture2D image, int x, int y, int width, int height, 
            SpriteFont valueFont, string valueName)
        {
            this.value = value;
            font = valueFont;
            if (valueName.Length > 0 && valueName != null) // Empty or null string
            {
                ValueName = valueName;
            }
            else
            {
                ValueName = "Value";
            }
            bgRect = new Rectangle(x, y, width, height);
            PIXELS_PER_VALUE = bgRect.Width / 100.0f;

            SliderRectangle = new Rectangle(0, 0, SLIDER_WIDTH, height + SPACING);
            SliderRectangle.X = (int)(value * PIXELS_PER_VALUE) + bgRect.X - (SliderRectangle.Width / 2);
            SliderRectangle.Y = y - (SPACING / 2);

            this.image = image;
        }

        public ValueSlider(int value, Texture2D image, int x, int y, int width, int height,
            SpriteFont valueFont, string valueName, float maxValue)
            : this(value, image, x, y, width, height, valueFont, valueName)
        {
            PIXELS_PER_VALUE = bgRect.Width / maxValue;
        }

        #endregion

        #region Public Methods

        public void Update()
        {
            // Check for a click and update Value accordingly
            mouse = Mouse.GetState();

            // If true, the mouse is currently clicked over the slider rectangle
            sliding = (bgRect.Intersects(new Rectangle(mouse.X, mouse.Y, 1, 1)) || 
                SliderRectangle.Intersects(new Rectangle(mouse.X, mouse.Y, 1, 1))) 
                && mouse.LeftButton == ButtonState.Pressed;

            if (sliding)
            {
                // We have to let the user drag the slider
                SliderRectangle.X = mouse.X - (SliderRectangle.Width / 2);
                valueChanged?.Invoke();
            }

            // We must clamp the sliderRect position
            if (SliderRectangle.X + (SliderRectangle.Width / 2) < bgRect.X)
            {
                SliderRectangle.X = bgRect.X - (SliderRectangle.Width / 2);
            }
            else if (SliderRectangle.X + (SliderRectangle.Width / 2) > bgRect.X + bgRect.Width)
            {
                // The slider is at the end of the allowed value
                SliderRectangle.X = (bgRect.X + bgRect.Width) - (SliderRectangle.Width / 2);
            }

            // Finally, we must edit the value based on the slider's position
            value = ((SliderRectangle.X + (SliderRectangle.Width / 2)) - bgRect.X) /* the center of the slider */ / PIXELS_PER_VALUE;
        }

        public void Draw(SpriteBatch spriteBatch)
        {
            spriteBatch.Draw(image, bgRect, Color.LightGray);
            spriteBatch.Draw(image, SliderRectangle, Color.Gray);
            spriteBatch.DrawString(font, ValueName + ": " + ((int)value).ToString(), 
                new Vector2(bgRect.X, SliderRectangle.Y - (SPACING * 5)), Color.Black);
        }

        public void AddValueChangedHandler(System.Action handler)
        {
            valueChanged += handler;
        }

        #endregion
    }
}
