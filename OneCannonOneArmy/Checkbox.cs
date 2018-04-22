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
    public class Checkbox
    {
        #region Fields & Properties

        public string Text = "";

        Texture2D image;
        Texture2D checkImage;
        public Rectangle DrawRectangle;
        Rectangle checkRectangle;

        SpriteFont font;

        MouseState mouse;

        const int SPACING_X = 10;

        const int SIZE = 20;

        bool selected = false;

        bool clickStarted = false;
        bool buttonReleased = true;

        public bool IsChecked { get; set; }

        public int X
        {
            get
            {
                return DrawRectangle.X;
            }
            set
            {
                DrawRectangle.X = value;
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
            }
        }
        public int Width
        {
            get
            {
                return DrawRectangle.Width + (int)font.MeasureString(Text).X;
            }
        }
        public int Height
        {
            get
            {
                return DrawRectangle.Height + (int)font.MeasureString(Text).Y;
            }
        }

        event System.Action onCheck;

        #endregion

        #region Constructors

        public Checkbox(string message, SpriteFont font, GraphicsDevice graphics, Texture2D checkImage)
        {
            Text = message;

            this.font = font;
            image = new Texture2D(graphics, SIZE, SIZE);
            Color[] data = new Color[SIZE * SIZE];
            for (int i = 0; i < SIZE * SIZE; i++)
            {
                data[i] = Color.White;
            }
            image.SetData(data);
            image = DrawHelper.AddBorder(image, 2, Color.Gray, Color.White);

            this.checkImage = checkImage;

            DrawRectangle = new Rectangle(0, 0, SIZE, SIZE);
            checkRectangle = new Rectangle(0, 0, SIZE, SIZE);
        }
        public Checkbox(string message, bool isChecked, ContentManager content, SpriteFont font, GraphicsDevice graphics, 
            Texture2D checkImage)
            : this(message, font, graphics, checkImage)
        {
            IsChecked = isChecked;
        }

        #endregion

        #region Public Methods

        public void Update()
        {
            mouse = Mouse.GetState();

            //click processing to make a realistic checkbox
            selected = (DrawRectangle.Intersects(new Rectangle(Mouse.GetState().X, Mouse.GetState().Y, 1, 1)));
            if (selected && mouse.LeftButton == ButtonState.Pressed)
            {
                clickStarted = true;
                buttonReleased = false;
            }
            else if (!(mouse.LeftButton == ButtonState.Pressed) && selected)
            {
                buttonReleased = true;
            }

            if (clickStarted && buttonReleased)
            {
                IsChecked = !IsChecked;
                clickStarted = false;
                buttonReleased = true;
                if (IsChecked == true)
                {
                    onCheck?.Invoke();
                }
            }

            checkRectangle.X = DrawRectangle.Center.X - SPACING_X;
            checkRectangle.Y = DrawRectangle.Y - (SPACING_X / 2);
        }
        public void Draw(SpriteBatch spriteBatch, Color textColor)
        {
            spriteBatch.Draw(image, DrawRectangle, Color.White);
            spriteBatch.DrawString(font, Text, new Vector2(DrawRectangle.Right + SPACING_X, DrawRectangle.Y), textColor);
            if (IsChecked)
            {
                spriteBatch.Draw(checkImage, checkRectangle, Color.White);
            }

        }

        public void AddOnCheckHandler(System.Action handler)
        {
            onCheck += handler;
        }

        #endregion
    }
}
