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
    public class MenuButton
    {
        #region Fields & Properties

        Texture2D bgImage;
        Texture2D inactiveImg;
        Texture2D activeImg;
        public Rectangle DrawRectangle;
        Texture2D buttonImage;
        Rectangle buttonRectangle;

        GraphicsDevice graphics;

        public ButtonType ButtonType;

        const int TEXT_SPACING_WIDTH = 5;
        const int TEXT_SPACING_HEIGHT = 5;

        public bool CanHold { get; set; }
        Timer holdPauseTimer = new Timer(0.05f, TimerUnits.Seconds);
        bool holdPausing = false;
        Timer holdTimer = new Timer(0.5f, TimerUnits.Seconds);
        bool waitingToStartHold = true;

        MouseState mouse;

        Color drawColor = Color.DarkGray;
        SpriteFont font;
        public SpriteFont Font
        {
            get
            {
                return font;
            }
        }

        string text;
        Vector2 textPos;
        public int TextX
        {
            get
            {
                return (int)textPos.X;
            }
            set
            {
                textPos.X = value;
            }
        }
        public int TextY
        {
            get
            {
                return (int)textPos.Y;
            }
            set
            {
                textPos.Y = value;
            }
        }

        System.Action onButtonClick;
        Action<MenuButton> clickWThis;
        bool provideThisAsArg = false;

        const int SPACING_Y = 10;
        const int SPACING_X = 10;

        bool clickStarted = false;
        bool buttonReleased = true;

        bool hasBackground = true;

        bool active = true;

        public int X
        {
            get
            {
                return DrawRectangle.X;
            }
            set
            {
                DrawRectangle.X = value;
                if (font != null)
                {
                    UpdateTextPosition();
                }
            }
        }
        public int Y
        {
            get
            {
                return DrawRectangle.Y;
            }
            set
            {
                DrawRectangle.Y = value;
                if (font != null)
                {
                    UpdateTextPosition();
                }
            }
        }
        public int Width
        {
            get
            {
                return DrawRectangle.Width;
            }
            set
            {
                DrawRectangle.Width = value;
                if (graphics != null)
                {
                    GenerateBackgroundImage();
                }
                if (font != null)
                {
                    UpdateTextPosition();
                }
            }
        }
        public int Height
        {
            get
            {
                return DrawRectangle.Height;
            }
            set
            {
                DrawRectangle.Height = value;

                if (graphics != null)
                {
                    GenerateBackgroundImage();
                }
                if (font != null)
                {
                    UpdateTextPosition();
                }
            }
        }
        public int ImgWidth
        {
            get
            {
                return buttonRectangle.Width;
            }
            set
            {
                buttonRectangle.Width = value;
                DrawRectangle.Width = value + SPACING_X;
                if (graphics != null)
                {
                    GenerateBackgroundImage();
                }
            }
        }
        public int ImgHeight
        {
            get
            {
                return buttonRectangle.Height;
            }
            set
            {
                buttonRectangle.Height = value;
                DrawRectangle.Height = value + SPACING_Y;
                if (graphics != null)
                {
                    GenerateBackgroundImage();
                }
            }
        }

        public bool Active
        {
            get
            {
                return active;
            }
            set
            {
                active = value;
                bgImage = value ? activeImg : inactiveImg;
            }
        }

        public string Text
        {
            get
            {
                return text;
            }
            set
            {
                text = value;
                if (ButtonType == ButtonType.Text)
                {
                    int prevWidth = DrawRectangle.Width;
                    DrawRectangle.Width = (int)font.MeasureString(text).X + SPACING_X;
                    DrawRectangle.Height = (int)font.MeasureString(text).Y + SPACING_Y;
                    if (prevWidth != DrawRectangle.Width && hasBackground)
                    {
                        GenerateBackgroundImage();
                    }
                }
            }
        }

        public bool Hovered = false;

        public int DefaultWidth
        {
            get
            {
                return font != null ? ((int)font.MeasureString(text).X + SPACING_X) : 0;
            }
        }
        public int DefaultHeight
        {
            get
            {
                return font != null ? ((int)font.MeasureString(text).Y + SPACING_Y) : 0;
            }
        }

        const int INACTIVE_COLOR_DEPLETION = 50;

        #endregion

        #region Constructors

        public MenuButton(System.Action onButtonClick, string buttonText, int x, int y, bool hasBackground, SpriteFont font,
            GraphicsDevice graphics)
        {
            active = true;
            this.onButtonClick = onButtonClick;
            text = buttonText;
            this.graphics = graphics;

            if (font != null)
            {
                DrawRectangle = new Rectangle(x, y, ((int)font.MeasureString(buttonText).X + SPACING_X),
                    ((int)font.MeasureString(buttonText).Y + SPACING_Y));
                textPos = new Vector2(DrawRectangle.X + TEXT_SPACING_WIDTH, DrawRectangle.Y + TEXT_SPACING_HEIGHT);
            }
            else
            {
                DrawRectangle = new Rectangle(x, y, 1, 1);
            }

            if (hasBackground)
            {
                GenerateBackgroundImage();
            }

            this.font = font;

            ButtonType = ButtonType.Text;

            this.hasBackground = hasBackground;
        }
        public MenuButton(System.Action onButtonClick, int x, int y, bool hasBackground, GraphicsDevice graphics,
            Texture2D interiorImg)
        {
            this.onButtonClick = onButtonClick;
            this.graphics = graphics;

            DrawRectangle = new Rectangle(x, y, interiorImg.Width + SPACING_X, interiorImg.Height + SPACING_Y);

            if (hasBackground)
            {
                GenerateBackgroundImage();
            }

            ButtonType = ButtonType.Image;

            buttonImage = interiorImg;
            buttonRectangle = new Rectangle(0, 0, interiorImg.Width, interiorImg.Height);

            this.hasBackground = hasBackground;
        }

        public MenuButton(Action<MenuButton> onButtonClick, string text, int x, int y, bool hasBackground, SpriteFont font,
            GraphicsDevice graphics)
            : this((System.Action)null, text, x, y, hasBackground, font, graphics)
        {
            provideThisAsArg = true;
            clickWThis = onButtonClick;
        }

        #endregion

        #region Public Methods

        public void Update(GameTime gameTime)
        {
            Hovered = (DrawRectangle.Intersects(new Rectangle(Mouse.GetState().X, Mouse.GetState().Y, 1, 1)));
            if (active)
            {
                if (Hovered)
                {
                    drawColor = Color.Gray;
                }
                else
                {
                    drawColor = Color.DarkGray;
                }
                if (Hovered && Mouse.GetState().LeftButton == ButtonState.Pressed)
                {
                    clickStarted = true;
                    buttonReleased = false;
                }
                else if (!(mouse.LeftButton == ButtonState.Pressed))
                {
                    buttonReleased = true;
                }

                if (clickStarted && buttonReleased)
                {
                    if (provideThisAsArg)
                    {
                        clickWThis?.Invoke(this);
                    }
                    else
                    {
                        Sound.PlaySound(Sounds.Click);
                        onButtonClick?.Invoke();
                    }
                    clickStarted = false;
                    buttonReleased = true;
                }
                if (CanHold && Hovered && Mouse.GetState().LeftButton == ButtonState.Pressed)
                {
                    if (waitingToStartHold)
                    {
                        holdTimer.Update(gameTime);
                        if (holdTimer.QueryWaitTime(gameTime))
                        {
                            waitingToStartHold = false;
                        }
                    }
                    else
                    {
                        if (holdPausing)
                        {
                            holdPauseTimer.Update(gameTime);
                            if (holdPauseTimer.QueryWaitTime(gameTime))
                            {
                                holdPausing = false;
                            }
                        }
                        else
                        {
                            ClickWithSound();
                            holdPausing = true;
                        }
                    }
                }
                else if (CanHold)
                {
                    waitingToStartHold = true;
                    holdTimer.Reset();
                }
            }
            else
            {
                drawColor = Color.DarkGray;
            }
        }

        public void Draw(SpriteBatch spriteBatch)
        {
            if (hasBackground)
            {
                spriteBatch.Draw(bgImage, DrawRectangle, drawColor);
            }
            Color contentColor = active ? Color.LightGray : Color.Gray;
            switch (ButtonType)
            {
                case ButtonType.Text:
                    if (font != null)
                    {
                        spriteBatch.DrawString(font, text, textPos, contentColor);
                    }
                    break;
                case ButtonType.Image:
                    buttonRectangle.X = DrawRectangle.X + DrawRectangle.Width / 2 - buttonRectangle.Width / 2;
                    buttonRectangle.Y = DrawRectangle.Y + DrawRectangle.Height / 2 - buttonRectangle.Height / 2;
                    spriteBatch.Draw(buttonImage, buttonRectangle, contentColor);
                    break;
            }
        }
        public void Draw(SpriteBatch spriteBatch, Color contentColor)
        {
            if (hasBackground)
            {
                spriteBatch.Draw(bgImage, DrawRectangle, drawColor);
            }
            if (!active)
            {
                contentColor.R -= INACTIVE_COLOR_DEPLETION;
                contentColor.G -= INACTIVE_COLOR_DEPLETION;
                contentColor.B -= INACTIVE_COLOR_DEPLETION;
            }
            switch (ButtonType)
            {
                case ButtonType.Text:
                    spriteBatch.DrawString(font, text, textPos, contentColor);
                    break;
                case ButtonType.Image:
                    buttonRectangle.X = DrawRectangle.X + TEXT_SPACING_WIDTH;
                    buttonRectangle.Y = DrawRectangle.Y + TEXT_SPACING_HEIGHT;
                    spriteBatch.Draw(buttonImage, buttonRectangle, contentColor);
                    break;
            }
        }
        public void Draw(SpriteBatch spriteBatch, Color? contentColor, SpriteEffects interiorImgEffect)
        {
            Color color = active ? Color.LightGray : Color.Gray;
            if (contentColor != null)
            {
                color = (Color)contentColor;
                if (!active)
                {
                    color.R -= INACTIVE_COLOR_DEPLETION;
                    color.G -= INACTIVE_COLOR_DEPLETION;
                    color.B -= INACTIVE_COLOR_DEPLETION;
                }
            }
            if (hasBackground)
            {
                spriteBatch.Draw(bgImage, DrawRectangle, drawColor);
            }
            switch (ButtonType)
            {
                case ButtonType.Text:
                    spriteBatch.DrawString(font, text, textPos, color);
                    break;
                case ButtonType.Image:
                    buttonRectangle.X = DrawRectangle.X + TEXT_SPACING_WIDTH;
                    buttonRectangle.Y = DrawRectangle.Y + TEXT_SPACING_HEIGHT;
                    spriteBatch.Draw(buttonImage, buttonRectangle, null, color, 0.0f, new Vector2(), interiorImgEffect,
                        1.0f);
                    break;
            }
        }

        public void Click()
        {
            if (provideThisAsArg)
            {
                clickWThis?.Invoke(this);
            }
            else
            {
                onButtonClick?.Invoke();
            }
        }
        public void ClickWithSound()
        {
            Sound.PlaySound(Sounds.Click);
            if (provideThisAsArg)
            {
                clickWThis?.Invoke(this);
            }
            else
            {
                onButtonClick?.Invoke();
            }
        }

        #endregion

        #region Private Methods

        private void GenerateBackgroundImage()
        {
            activeImg = DrawHelper.AddBorder(new Texture2D(graphics, DrawRectangle.Width, DrawRectangle.Height), 
                2, Color.DarkGray, Color.White);
            inactiveImg = DrawHelper.AddBorder(new Texture2D(graphics, DrawRectangle.Width, DrawRectangle.Height),
                2, new Color(100, 100, 100), new Color(150, 150, 150));
            bgImage = active ? activeImg : inactiveImg;
        }

        private void UpdateTextPosition()
        {
            textPos.X = DrawRectangle.X + DrawRectangle.Width / 2 - font.MeasureString(text).X / 2;
            textPos.Y = DrawRectangle.Y + DrawRectangle.Height / 2 - font.MeasureString(text).Y / 2;
        }

        #endregion
    }

    public class ToggleButton
    {
        #region Fields & Properties

        MenuButton toggleButton;

        Texture2D falseImg;
        Rectangle falseRect;
        public bool Toggled;

        const int SPACING = 4;

        public int X
        {
            get
            {
                return toggleButton.X;
            }
            set
            {
                toggleButton.X = value;
                falseRect.X = value;
            }
        }
        public int Y
        {
            get
            {
                return toggleButton.Y;
            }
            set
            {
                toggleButton.Y = value;
                falseRect.Y = value;
            }
        }
        public int Width
        {
            get
            {
                return toggleButton.Width;
            }
        }
        public int Height
        {
            get
            {
                return toggleButton.Height;
            }
        }

        event Action<bool> toggled;

        public MenuButton Button
        {
            get
            {
                return toggleButton;
            }
        }

        public bool Active
        {
            get
            {
                return toggleButton.Active;
            }
            set
            {
                toggleButton.Active = value;
            }
        }

        #endregion

        #region Constructors

        public ToggleButton(Texture2D insideImg, Texture2D falseImg, GraphicsDevice graphics, int x, int y, int width, int height)
        {
            this.falseImg = falseImg;
            toggleButton = new MenuButton(Toggle, x, y, true,
                graphics, insideImg);
            toggleButton.ImgWidth = width - SPACING * 3;
            toggleButton.ImgHeight = height - SPACING * 3;
            toggleButton.Width = width;
            toggleButton.Height = height;
            falseRect = new Rectangle(x, y, toggleButton.Width, toggleButton.Height);
        }

        #endregion

        #region Public Methods

        public void Update(GameTime gameTime)
        {
            toggleButton.Update(gameTime);
        }

        public void Draw(SpriteBatch spriteBatch)
        {
            toggleButton.Draw(spriteBatch);
            if (Toggled)
            {
                spriteBatch.Draw(falseImg, falseRect, Color.White);
            }
        }

        public void AddOnToggleHandler(Action<bool> handler)
        {
            toggled += handler;
        }

        public void ClickWithSound()
        {
            if (Active)
            {
                toggleButton.ClickWithSound();
            }
        }
        public void Click()
        {
            if (Active)
            {
                toggleButton.Click();
            }
        }

        #endregion

        #region Private Methods

        private void Toggle()
        {
            Toggled = !Toggled;
            toggled?.Invoke(Toggled);
        }

        #endregion
    }

    #region Enums

    public enum ButtonType
    {
        Text,
        Image
    }

    #endregion
}