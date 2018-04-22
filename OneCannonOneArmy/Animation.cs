using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace OneCannonOneArmy
{
    public class Animation
    {
        #region Fields

        Texture2D img;
        Rectangle sourceRect;

        public bool Playing;

        public int TotalRows = 0;
        public int FramesPerRow = 0;
        public int Row = 0;
        public int Column = 0;
        public int FrameWidth = 0;
        public int FrameHeight = 0;

        Timer timer;

        event System.Action onFinish;

        #endregion

        #region Constructors

		public Animation(Texture2D sourceImg, int totalRows, int framesPerRow, float time, int frameWidth, int frameHeight,
            System.Action onFinish)
        {
            if (onFinish != null)
            {
                this.onFinish += onFinish;
            }
            FrameWidth = frameWidth;
            FrameHeight = frameHeight;
            TotalRows = totalRows;
            FramesPerRow = framesPerRow;

            timer = new Timer(time, TimerUnits.Milliseconds);
            img = sourceImg;
            sourceRect = new Rectangle(0, 0, frameWidth, frameHeight);
        }

        #endregion

        #region Public Methods

        public void AddOnFinishHandler(System.Action handler)
        {
            onFinish += handler;
        }

		public void Update(GameTime gameTime)
        {
			if (Playing)
            {
                timer.Update(gameTime);
				if (timer.QueryWaitTime(gameTime))
                {
                    if (Column < FramesPerRow - 1)
                    {
                        // We are NOT on the last frame of the column
                        sourceRect.X += FrameWidth;
                        Column++;
                    }
                    else
                    {
                        // We are on the last frame of the column
                        if (Row >= TotalRows - 1 && Column >= FramesPerRow - 1)
                        {
                            // We are displaying the last image of the sprite sheet,
                            // and are no longer exploding
                            Playing = false;
                            onFinish?.Invoke();
                            return;
                        }
                        // Go to the first image of the next column
                        Column = 0;
                        Row++;
                        sourceRect.X = 0;
                        sourceRect.Y += FrameHeight;
                    }
                }
            }
        }

		public void Draw(SpriteBatch spriteBatch, Rectangle drawRectangle, Color color)
        {
            spriteBatch.Draw(img, drawRectangle, sourceRect, color);
        }
        public void Draw(SpriteBatch spriteBatch, Rectangle drawRectangle, Color color, Vector2 origin, SpriteEffects effects)
        {
            spriteBatch.Draw(img, drawRectangle, sourceRect, color, 0.0f, origin, effects, 1.0f);
        }

		public void Play()
        {
            Playing = true;
            Row = 0;
            Column = 0;
            sourceRect.X = 0;
            sourceRect.Y = 0;
        }

        #endregion
    }
}
