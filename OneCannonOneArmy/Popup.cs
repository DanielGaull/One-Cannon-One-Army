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
    public class PopupInstance
    {
        #region Fields & Properties

        string text = "";
        Vector2 textPosition;

        bool hasCheckbox = false;
        
        Texture2D image;
        Rectangle drawRectangle;
        MenuButton okButton;
        MenuButton cancelButton;
        Checkbox queryCheckbox;
        
        Rectangle transRect;
        Color transColor;

        const int SPACING_Y = 60;
        const int SPACING_X = 40;
        const int SPACING_SMALL = 10;
        const int BACKGROUND_WIDTH = 600;
        const int BACKGROUND_HEIGHT = 250;
        string BUTTON1 = LanguageTranslator.Translate("Okay");
        string BUTTON2 = LanguageTranslator.Translate("Cancel");
        const byte ALPHA = 100;

        System.Action whenCancelButtonClicked;
        System.Action whenOkButtonClicked;

        SpriteFont font;

        bool active = false;

        bool hasCancelButton = true;

        public bool Active
        {
            get
            {
                return active;
            }
            set
            {
                active = value;
            }
        }
        public bool CheckboxChecked
        {
            get
            {
                return queryCheckbox.IsChecked;
            }
            set
            {
                queryCheckbox.IsChecked = value;
            }
        }

        public int X
        {
            get
            {
                return drawRectangle.X;
            }
            set
            {
                drawRectangle.X = value;
            }
        }
        public int Y
        {
            get
            {
                return drawRectangle.Y;
            }
            set
            {
                drawRectangle.Y = value;
            }
        }
        public int Width
        {
            get
            {
                return drawRectangle.Width;
            }
        }
        public int Height
        {
            get
            {
                return drawRectangle.Height;
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
            }
        }

        #endregion

        #region Constructors

        public PopupInstance(System.Action whenOkButtonClicked, Texture2D checkSprite, SpriteFont font, 
            GraphicsDevice graphics, int windowWidth, int windowHeight, bool hasCancelButton, SpriteFont bigFont)
        {
            this.hasCancelButton = hasCancelButton;

            image = new Texture2D(graphics, BACKGROUND_WIDTH, BACKGROUND_HEIGHT);
            Color[] data = new Color[BACKGROUND_WIDTH * BACKGROUND_HEIGHT];
            for (int i = 0; i < BACKGROUND_WIDTH * BACKGROUND_HEIGHT; i++)
            {
                data[i] = Color.White;
            }
            image = DrawHelper.AddBorder(image, 5, Color.Gray, Color.DarkGray);
            drawRectangle = new Rectangle(0, 0, BACKGROUND_WIDTH, BACKGROUND_HEIGHT);

            whenCancelButtonClicked = new System.Action(HidePopup);
            this.whenOkButtonClicked = whenOkButtonClicked;

            queryCheckbox = new Checkbox(LanguageTranslator.Translate("Do not show again"), font, graphics, checkSprite);

            this.font = font;

            okButton = new MenuButton(new System.Action(OnButtonClick), BUTTON1, 0, 0, true, bigFont, graphics);
            if (hasCancelButton)
            {
                cancelButton = new MenuButton(whenCancelButtonClicked, BUTTON2, 0, 0, true, bigFont, graphics);
                int maxWidth = Math.Max(cancelButton.Width, okButton.Width);
                okButton.Width = cancelButton.Width = maxWidth;
            }

            textPosition = new Vector2(0, 0);
            
            transRect = new Rectangle(0, 0, windowWidth, windowHeight);
            transColor = Color.Black;
            transColor.A = ALPHA;
        }

        public PopupInstance(SpriteFont smallFont, int windowWidth, int windowHeight, 
            bool hasCancelButton, SpriteFont bigFont, GraphicsDevice graphics)
        {
            this.hasCancelButton = hasCancelButton;

            image = new Texture2D(graphics, BACKGROUND_WIDTH, BACKGROUND_HEIGHT);
            Color[] data = new Color[BACKGROUND_WIDTH * BACKGROUND_HEIGHT];
            for (int i = 0; i < BACKGROUND_WIDTH * BACKGROUND_HEIGHT; i++)
            {
                data[i] = Color.White;
            }
            image = DrawHelper.AddBorder(image, 5, Color.Gray, Color.DarkGray);
            drawRectangle = new Rectangle(0, 0, BACKGROUND_WIDTH, BACKGROUND_HEIGHT);

            whenCancelButtonClicked = new System.Action(HidePopup);

            font = smallFont;

            okButton = new MenuButton(new System.Action(HidePopup), BUTTON1, 0, 0, true, bigFont, graphics);
            if (hasCancelButton)
            {
                cancelButton = new MenuButton(whenCancelButtonClicked, BUTTON2, 0, 0, true, bigFont, graphics);
                int maxWidth = Math.Max(cancelButton.Width, okButton.Width);
                okButton.Width = cancelButton.Width = maxWidth;
            }

            textPosition = new Vector2(0, 0);
            
            transRect = new Rectangle(0, 0, windowWidth, windowHeight);
            transColor = Color.Black;
            transColor.A = ALPHA;
        }
        public PopupInstance(SpriteFont smallFont, int windowWidth, int windowHeight, bool hasCancelButton, SpriteFont bigFont,
            System.Action whenOkButtonClicked, GraphicsDevice graphics)
        {
            this.hasCancelButton = hasCancelButton;

            image = new Texture2D(graphics, BACKGROUND_WIDTH, BACKGROUND_HEIGHT);
            Color[] data = new Color[BACKGROUND_WIDTH * BACKGROUND_HEIGHT];
            for (int i = 0; i < BACKGROUND_WIDTH * BACKGROUND_HEIGHT; i++)
            {
                data[i] = Color.White;
            }
            image = DrawHelper.AddBorder(image, 5, Color.Gray, Color.DarkGray);
            drawRectangle = new Rectangle(0, 0, BACKGROUND_WIDTH, BACKGROUND_HEIGHT);

            whenCancelButtonClicked = new System.Action(HidePopup);
            this.whenOkButtonClicked = whenOkButtonClicked;

            font = smallFont;

            okButton = new MenuButton(new System.Action(OnButtonClick), BUTTON1, 0, 0, true, bigFont, graphics);
            if (hasCancelButton)
            {
                cancelButton = new MenuButton(whenCancelButtonClicked, BUTTON2, 0, 0, true, bigFont, graphics);
                int maxWidth = Math.Max(cancelButton.Width, okButton.Width);
                okButton.Width = cancelButton.Width = maxWidth;
            }

            textPosition = new Vector2(0, 0);
            
            transRect = new Rectangle(0, 0, windowWidth, windowHeight);
            transColor = Color.Black;
            transColor.A = ALPHA;
        }
        #endregion

        #region Public Methods

        public void ShowPopup(string text, bool shouldQueryShowPopupAgain, int x, int y)
        {
            Sound.PlaySound(Sounds.Notification);
            this.text = text;
            active = true;
            drawRectangle.X = x;
            drawRectangle.Y = y;
            hasCheckbox = shouldQueryShowPopupAgain;
        }
        public void HidePopup()
        {
            active = false;
        }

        public List<MenuButton> GetButtons()
        {
            List<MenuButton> returnVal = new List<MenuButton>();
            returnVal.Add(okButton);
            if (hasCancelButton)
            {
                returnVal.Add(cancelButton);
            }

            return returnVal;
        }

        public void Update()
        {
            okButton.Text = LanguageTranslator.Translate("Okay");
            if (hasCancelButton)
            {
                cancelButton.Text = LanguageTranslator.Translate("Cancel");
            }
            if (active && hasCheckbox)
            {
                queryCheckbox.Text = LanguageTranslator.Translate("Do not show again.");
            }

            if (active)
            {
                okButton.Update();
                if (hasCancelButton)
                {
                    cancelButton.Update();
                }
                if (hasCheckbox)
                {
                    queryCheckbox.Update();
                }
            }

            // set button positions so they are correctly aligned with the popup background
            if (hasCancelButton)
            {
                okButton.X = drawRectangle.X + SPACING_X;
                okButton.Y = drawRectangle.Bottom - okButton.Height - SPACING_SMALL;
                cancelButton.X = drawRectangle.Right - (SPACING_X + cancelButton.Width);
                cancelButton.Y = drawRectangle.Bottom - cancelButton.Height - SPACING_SMALL;
            }
            else
            {
                // No cancel button, center Ok Button
                okButton.X = drawRectangle.X + (drawRectangle.Width / 2 - (okButton.Width / 2));
                okButton.Y = drawRectangle.Bottom - okButton.Height - SPACING_SMALL;
            }

            // Set the checkbox and text positions so they are correctly aligned with the popup background
            if (hasCheckbox)
            {
                queryCheckbox.X = okButton.DrawRectangle.Left;
                queryCheckbox.Y = okButton.DrawRectangle.Top - SPACING_Y;
            }
            textPosition.X = drawRectangle.X + SPACING_X;
            textPosition.Y = (float)drawRectangle.Y + SPACING_Y;
        }

        public void Draw(SpriteBatch spriteBatch)
        {
            if (active)
            {
                spriteBatch.Draw(image, transRect, transColor);

                spriteBatch.Draw(image, drawRectangle, Color.White);

                spriteBatch.DrawString(font, text, textPosition, Color.White);

                okButton.Draw(spriteBatch);

                if (hasCancelButton)
                {
                    cancelButton.Draw(spriteBatch);
                }

                if (hasCheckbox)
                {
                    queryCheckbox.Draw(spriteBatch, Color.White);
                }
            }
        }

        #endregion

        #region Private Methods

        private void OnButtonClick()
        {
            HidePopup();
            whenOkButtonClicked();
        }

        #endregion
    }

    public static class Popup
    {
        #region Fields

        static bool allowNew = true;

        static List<PopupInstance> popups = new List<PopupInstance>();

        static int windowWidth;
        static int windowHeight;
        static SpriteFont smallFont;
        static SpriteFont bigFont;
        static GraphicsDevice graphics;
        static Texture2D checkImg;

        public static bool HasActivePopups
        {
            get
            {
                bool value = false;
                foreach (PopupInstance pi in popups)
                {
                    if (pi.Active)
                    {
                        value = true;
                    }
                }
                return value;
            }
        }

        #endregion

        #region Public Methods
        
        public static void BlockAllPopups()
        {
            allowNew = false;
        }
        public static void RemoveBlock()
        {
            allowNew = true;
        }

        public static List<MenuButton> GetButtons()
        {
            List<MenuButton> returnVal = new List<MenuButton>();

            for (int i = 0; i <= popups.Count - 1; i++)
            {
                returnVal.AddRange(popups[i].GetButtons());
            }

            return returnVal;
        }

        public static void Initialize(int windowWidth, int windowHeight, SpriteFont smallFont, SpriteFont bigFont, 
            GraphicsDevice graphics, Texture2D checkImg)
        {
            Popup.windowWidth = windowWidth;
            Popup.windowHeight = windowHeight;
            Popup.smallFont = smallFont;
            Popup.bigFont = bigFont;
            Popup.graphics = graphics;
            Popup.checkImg = checkImg;
        }

        public static void Update()
        {
            for (int i = 0; i <= popups.Count - 1; i++)
            {
                if (!popups[i].Active)
                {
                    popups.RemoveAt(i);
                }
                else
                {
                    popups[i].Update();
                }
            }
        }

        public static void Draw(SpriteBatch spriteBatch)
        {
            for (int i = 0; i <= popups.Count - 1; i++)
            {
                popups[i].Draw(spriteBatch);
            }
        }

        public static void Show(string text, bool hasCancelButton, bool shouldQueryShowAgain)
        {
            if (allowNew)
            {
                PopupInstance newP;
                if (shouldQueryShowAgain)
                {
                    newP = new PopupInstance(null, checkImg, smallFont, graphics, windowWidth,
                        windowHeight, hasCancelButton, bigFont);
                }
                else
                {
                    newP = new PopupInstance(smallFont, windowWidth, windowHeight, hasCancelButton,
                        bigFont, graphics);
                }

                newP.ShowPopup(text, shouldQueryShowAgain, 0, 0);
                newP.X = windowWidth / 2 - (newP.Width / 2);
                newP.Y = windowHeight / 2 - (newP.Height / 2);

                popups.Add(newP);
            }
        }
        public static void Show(string text, bool hasCancelButton, System.Action ok, bool shouldQueryShowAgain)
        {
            if (allowNew)
            {
                PopupInstance newP;
                if (shouldQueryShowAgain)
                {
                    newP = new PopupInstance(ok, checkImg, smallFont, graphics, windowWidth,
                        windowHeight, hasCancelButton, bigFont);
                }
                else
                {
                    newP = new PopupInstance(smallFont, windowWidth, windowHeight, hasCancelButton,
                        bigFont, ok, graphics);
                }

                newP.ShowPopup(text, shouldQueryShowAgain, 0, 0);
                newP.X = windowWidth / 2 - (newP.Width / 2);
                newP.Y = windowHeight / 2 - (newP.Height / 2);

                popups.Add(newP);
            }
        }

        #endregion
    }
}

