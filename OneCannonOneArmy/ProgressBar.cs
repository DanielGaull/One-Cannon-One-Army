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
    class ProgressBar
    {
        #region Fields & Properties

        Texture2D boxImage;
        Rectangle boxDrawRectangle;
        Texture2D progressImage;
        Rectangle progressRectangle;

        SpriteFont font;
        Vector2 percentLocation;

        Color color;

        const int FIX_X = 2;
        const int FIX_Y = 2;

        public bool ShowPercentage { get; set; }
        public int Percentage { get; set; }
        public int X 
        { 
            get 
            { 
                return boxDrawRectangle.X; 
            } 
            set
            { 
                boxDrawRectangle.X = value;

                progressRectangle.X = boxDrawRectangle.X + FIX_X;
                progressRectangle.Y = boxDrawRectangle.Y + FIX_Y;
                progressRectangle.Width = this.Width;
                progressRectangle.Width = (Percentage * 2) - 5;
                progressRectangle.Height = boxDrawRectangle.Height - 5;

                percentLocation.X = boxDrawRectangle.X + boxImage.Width + FIX_X;
                percentLocation.Y = this.Y;
            } 
        }
        public int Y 
        { 
            get 
            { 
                return boxDrawRectangle.Y; 
            } 
            set 
            { 
                boxDrawRectangle.Y = value;

                progressRectangle.X = boxDrawRectangle.X + FIX_X;
                progressRectangle.Y = boxDrawRectangle.Y + FIX_Y;
                progressRectangle.Width = this.Width;
                progressRectangle.Width = (Percentage * 2) - 5;
                progressRectangle.Height = boxDrawRectangle.Height - 5;

                percentLocation.X = this.X + boxImage.Width + FIX_X;
                percentLocation.Y = boxDrawRectangle.Y;
            } 
        }
        public int Width 
        { 
            get 
            { 
                return boxDrawRectangle.Width; 
            } 
            set 
            { 
                boxDrawRectangle.Width = value;

                progressRectangle.X = boxDrawRectangle.X + FIX_X;
                progressRectangle.Y = boxDrawRectangle.Y + FIX_Y;
                progressRectangle.Width = boxDrawRectangle.Width;
                progressRectangle.Width = (Percentage * 2) - 5;
                progressRectangle.Height = boxDrawRectangle.Height - 5;

                percentLocation.X = this.X + boxImage.Width + FIX_X;
                percentLocation.Y = this.Y;
            } 
        }
        public int Height 
        { 
            get 
            { 
                return boxDrawRectangle.Height; 
            } 
            set 
            { 
                boxDrawRectangle.Height = value;

                progressRectangle.X = boxDrawRectangle.X + FIX_X;
                progressRectangle.Y = boxDrawRectangle.Y + FIX_Y;
                progressRectangle.Width = this.Width;
                progressRectangle.Width = (Percentage * 2) - 5;
                progressRectangle.Height = boxDrawRectangle.Height - 5;

                percentLocation.X = this.X + boxImage.Width + FIX_X;
                percentLocation.Y = this.Y;
            } 
        }

        #endregion

        #region Constructors

        public ProgressBar(ContentManager content, string boxImageName, string progressImageName, string fontName, Color progressColor, 
            int x, int y, bool showPercentage)
        {
            boxImage = content.Load<Texture2D>(boxImageName);
            boxDrawRectangle = new Rectangle(x, y, boxImage.Width + FIX_X, boxImage.Height);
            progressImage = content.Load<Texture2D>(progressImageName);

            progressRectangle = new Rectangle(x + FIX_X, y + FIX_Y, 0, boxDrawRectangle.Height - FIX_Y);

            font = content.Load<SpriteFont>(fontName);

            this.color = progressColor;

            this.ShowPercentage = showPercentage;

            this.X = x;
            this.Y = y;

            percentLocation = new Vector2(x + boxImage.Width + FIX_X, y);
        }

        public ProgressBar(ContentManager content, string boxImageName, string progressImageName, string fontName, Color progressColor,
            int x, int y, bool showPercentage, int percentage)
            : this(content, boxImageName, progressImageName, fontName, progressColor, x, y, showPercentage)
        {
            this.Percentage = percentage;
        }

        #endregion

        #region Public Methods

        public void Update()
        {
            // Percentage clamping
            if (Percentage > 99)
            {
                Percentage = 100;
            }
            else if (Percentage < 0)
            {
                Percentage = 0;
            }
            progressRectangle.X = boxDrawRectangle.X + FIX_X;
            progressRectangle.Y = boxDrawRectangle.Y + FIX_Y;
            progressRectangle.Width = this.Width;
            progressRectangle.Width = (Percentage * 2) - 5;
            progressRectangle.Height = boxDrawRectangle.Height - 5;

            percentLocation.X = this.X + boxImage.Width + FIX_X;
            percentLocation.Y = this.Y;
        }

        public void Draw(SpriteBatch spriteBatch, Color color)
        {
            spriteBatch.Draw(progressImage, progressRectangle, this.color);
            spriteBatch.Draw(boxImage, boxDrawRectangle, Color.White);

            boxDrawRectangle.Width = this.Width;
            boxDrawRectangle.Height = this.Height;
            progressRectangle.Height = this.Height;

            if (ShowPercentage)
            {
                spriteBatch.DrawString(font, Percentage.ToString() + "%", percentLocation, color);
            }
        }

        public void AddPercentage(int addedPercentage)
        {
            Percentage += addedPercentage;
            // Percentage clamping
            if (Percentage > 99)
            {
                Percentage = 100;
            }
            else if (Percentage < 0)
            {
                Percentage = 0;
            }
        }

        #endregion

        #region Private Methods

        #endregion
    }
}
