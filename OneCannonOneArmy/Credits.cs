using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OneCannonOneArmy
{
    public class Credits
    {
        #region Fields and Properties

        int windowWidth;
        int windowHeight;

        float scrollValue = 0.0f;
        const float SCROLL_SPD = 0.4f;

        SpriteFont font;
        
        event System.Action exitCredits;

        Texture2D titleImg;
        Rectangle titleRect;
        const int MIN_Y = 20;

        const int START_TIME = 3;
        const int END_TIME = 4;
        Timer timer;
        bool scrolling = false;
        bool ended = false;

        #endregion

        #region Constructors

        public Credits(SpriteFont font, int windowWidth, int windowHeight, Texture2D titleImg, int titleWidth,
            int titleHeight)
        {
            this.font = font;
            this.windowWidth = windowWidth;
            this.windowHeight = windowHeight;

            this.titleImg = titleImg;
            titleRect = new Rectangle(windowWidth / 2 - titleWidth / 2, 0, titleWidth, titleHeight);

            timer = new Timer(START_TIME, TimerUnits.Seconds);
        }

        #endregion

        #region Public Methods

        public void Update(GameTime gameTime)
        {
            if (scrolling)
            {
                scrollValue -= SCROLL_SPD;
            }
            else if (ended)
            {
                timer.Update(gameTime);
                if (timer.QueryWaitTime(gameTime))
                {
                    exitCredits?.Invoke();
                }
            }
            else
            {
                timer.Update(gameTime);
                if (timer.QueryWaitTime(gameTime))
                {
                    scrolling = true;
                    timer.WaitTime = END_TIME;
                }
            }
        }

        public void Draw(SpriteBatch spriteBatch)
        {
            int y = (int)scrollValue + Utilities.MENU_Y_OFFSET;
            for (int i = 0; i < GameInfo.Credits.Count; i++)
            {
                string line = LanguageTranslator.Translate(GameInfo.Credits[i]);
                spriteBatch.DrawString(font, line, new Vector2(windowWidth / 2 - (font.MeasureString(line).X / 2), y), Color.Black);
                y += (int)font.MeasureString(line).Y * 2;
            }

            titleRect.Y = y;
            spriteBatch.Draw(titleImg, titleRect, Color.White);
            if (y <= MIN_Y)
            {
                ended = true;
                scrolling = false;
            }
        }

        public void AddExitCreditsHandler(System.Action handler)
        {
            exitCredits += handler;
        }

        public void Reset()
        {
            ended = scrolling = false;
            scrollValue = 0;
            timer.Reset();
        }

        #endregion
    }
}
