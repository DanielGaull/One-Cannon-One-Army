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
    public class NotificationInstance
    {
        #region Fields & Properties

        Texture2D bgImg;
        Rectangle bgRect;

        EnterDirection currentDir;

        MenuButton viewButton;
        MenuButton hideButton;

        SpriteFont font;

        public string Text = "";
        Vector2 textPos;

        const int SPACING = 5;
        const int HEIGHT_SPACING = 10;

        const int WIDTH = 600;

        Timer timer;

        bool showing = false;

        bool entering = false;
        bool leaving = false;

        int windowWidth;
        int windowHeight;

        public bool Active = true;

        #endregion

        #region Constructors

        public NotificationInstance(GraphicsDevice graphics, string text, int windowWidth, int windowHeight, 
            SpriteFont font)
        {
            hideButton = new MenuButton(new System.Action(Hide), " X ", 0, 0, true, font, graphics);

            int height1 = (int)font.MeasureString(text).Y + HEIGHT_SPACING;
            int height2 = hideButton.Height + HEIGHT_SPACING * 2;
            int height = height1 > height2 ? height1 : height2;
            bgImg = new Texture2D(graphics, WIDTH, height);
            bgImg = DrawHelper.AddBorder(bgImg, 3, Color.Gray, Color.LightGray);
            bgRect = new Rectangle(0, 0 - height, WIDTH, height);
            bgRect.X = windowWidth / 2 - (bgRect.Width / 2);

            textPos = new Vector2();

            this.Text = text;
            this.font = font;

            this.windowWidth = windowWidth;
            this.windowHeight = windowHeight;

            timer = new Timer(3, TimerUnits.Seconds);
        }

        public NotificationInstance(GraphicsDevice graphics, string text, int windowWidth, int windowHeight, 
            SpriteFont font, System.Action whenViewButtonClicked) 
             : this(graphics, text, windowWidth, windowHeight, font)
        {
            viewButton = new MenuButton(whenViewButtonClicked, "View", 0, 0, true, font, graphics);
        }

        #endregion

        #region Public Methods

        public List<MenuButton> GetButtons()
        {
            List<MenuButton> returnVal = new List<MenuButton>()
            {
                hideButton
            };

            if (viewButton != null)
            {
                returnVal.Add(viewButton);
            }

            return returnVal;
        }

        public void Update(GameTime gameTime, EnterDirection enterDir)
        {
            // Update Location
            if (viewButton != null)
            {
                viewButton.Text = Language.Translate("View");
            }

            if (enterDir == EnterDirection.Top && bgRect.Y == windowHeight)
            {
                bgRect.Y = 0 - bgRect.Height;
            }
            if (enterDir == EnterDirection.Bottom && bgRect.Y == 0 - bgRect.Height)
            {
                bgRect.Y = windowHeight;
            }

            hideButton.X = (bgRect.Width + bgRect.X) - (SPACING + hideButton.Width);
            hideButton.Y = bgRect.Y + (bgRect.Height / 2 - (hideButton.Height / 2));
            if (viewButton != null)
            {
                viewButton.X = hideButton.X - (viewButton.Width + SPACING);
                viewButton.Y = hideButton.Y;
            }

            textPos.X = bgRect.X + SPACING;
            textPos.Y = bgRect.Y + SPACING;

            // Update Objects
            if (viewButton != null && showing)
            {
                viewButton.Update(gameTime);
            }

            hideButton.Update(gameTime);

            if (showing)
            {
                timer.Update(gameTime);
            }

            if (timer.QueryWaitTime(gameTime))
            {
                leaving = true;
            }

            #region Entering and Leaving Animation

            if (entering)
            {
                currentDir = enterDir;
                if (enterDir == EnterDirection.Top)
                {
                    if (bgRect.Y < SPACING)
                    {
                        bgRect.Y += 2;
                    }
                    else
                    {
                        entering = false;
                    }
                }
                else if (enterDir == EnterDirection.Bottom)
                {
                    if (bgRect.Y > windowHeight - bgRect.Height - SPACING)
                    {
                        bgRect.Y -= 2;
                    }
                    else
                    {
                        entering = false;
                    }
                }
            }
            else if (leaving)
            {
                if (bgRect.Y < windowHeight / 2)
                {
                    if (bgRect.Y >= 0 - bgRect.Height)
                    {
                        bgRect.Y -= 2;
                    }
                    else
                    {
                        leaving = false;
                        showing = false;
                        Active = false;
                    }
                }
                else
                {
                    if (bgRect.Y < windowHeight)
                    {
                        bgRect.Y += 2;
                    }
                    else
                    {
                        leaving = false;
                        showing = false;
                        Active = false;
                    }
                }
            }

            #endregion
        }

        public void Draw(SpriteBatch spriteBatch)
        {
            if (showing)
            {
                spriteBatch.Draw(bgImg, bgRect, Color.DarkGray);
                spriteBatch.DrawString(font, Text, textPos, Color.LightGray);
                hideButton.Draw(spriteBatch);

                if (viewButton != null)
                {
                    viewButton.Draw(spriteBatch);
                }
            }
        }

        public void Show()
        {
            showing = true;
            entering = true;
        }
        public void Hide()
        {
            showing = false;
            entering = false;
            leaving = true;
            Active = false;
        }

        #endregion
    }

    public static class Notification
    {
        static List<NotificationInstance> notifications = new List<NotificationInstance>();

        static GraphicsDevice graphics;
        static int windowWidth;
        static int windowHeight;
        static SpriteFont font;

        public static void Initialize(GraphicsDevice graphics, int windowWidth, int windowHeight, SpriteFont font)
        {
            Notification.graphics = graphics;
            Notification.windowWidth = windowWidth;
            Notification.windowHeight = windowHeight;
            Notification.font = font;
        }

        public static void Show(string text)
        {
            Sound.PlaySound(Sounds.Achievement);
            NotificationInstance newN = new NotificationInstance(graphics, text, windowWidth, windowHeight, font);
            newN.Show();
            notifications.Add(newN);
        }

        public static List<MenuButton> GetButtons()
        {
            List<MenuButton> returnVal = new List<MenuButton>();
            foreach (NotificationInstance n in notifications)
            {
                returnVal.AddRange(n.GetButtons());
            }
            return returnVal;
        }

        public static void Update(GameTime gameTime)
        {
            for (int i = 0; i < notifications.Count; i++)
            {
                notifications[i].Update(gameTime, EnterDirection.Top);
                if (!notifications[i].Active)
                {
                    notifications.RemoveAt(i);
                }
            }
        }
        public static void Draw(SpriteBatch spriteBatch)
        {
            foreach (NotificationInstance n in notifications)
            {
                n.Draw(spriteBatch);
            }
        }
    }

    public enum EnterDirection
    {
        Top,
        Bottom
    }
}
