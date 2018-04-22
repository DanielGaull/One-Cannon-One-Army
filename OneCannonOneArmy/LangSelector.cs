using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OneCannonOneArmy
{
    public class LangSelectorPopup
    {
        #region Fields & Properties

        Texture2D fullImg;
        Rectangle fullRect;

        Texture2D bgImg;
        Rectangle bgRect;

        MenuButton enButton;
        MenuButton esButton;
        MenuButton frButton;
        MenuButton eoButton;
        MenuButton itButton;
        MenuButton deButton;
        MenuButton ukButton;
        MenuButton tlButton;

        string infoText = " Please select your language below.";
        Vector2 infoTextPos = new Vector2();
        SpriteFont font;

        const int WIDTH = 600;
        const int HEIGHT = 150;
        const int SPACING = 5;

        const int BUTTON_WIDTH = 200;

        event Action<Language> onLangSet;

        public bool Active = false;

        #endregion

        #region Constructors

        public LangSelectorPopup(int windowWidth, int windowHeight, SpriteFont font, GraphicsDevice graphics)
        {
            this.font = font;

            fullImg = Utilities.RectImage;
            fullRect = new Rectangle(0, 0, windowWidth, windowHeight);

            bgImg = new Texture2D(graphics, WIDTH, HEIGHT);
            bgImg = DrawHelper.AddBorder(bgImg, 2, Color.Gray, Color.LightGray);
            bgRect = new Rectangle(windowWidth / 2 - WIDTH / 2, windowHeight / 2 - HEIGHT / 2, WIDTH, HEIGHT);

            infoTextPos = new Vector2(bgRect.X + (bgRect.Width / 2 - font.MeasureString(infoText).X / 2), bgRect.Y + SPACING);

            int x = (bgRect.X + bgRect.Width / 2 - BUTTON_WIDTH / 2)/* - (BUTTON_WIDTH + SPACING)*/;
            //int x2 = x + BUTTON_WIDTH + SPACING;
            int y = (int)(infoTextPos.Y + font.MeasureString(infoText).Y + SPACING / 2);

            enButton = new MenuButton(new System.Action(() => SetLang(Language.English)), "English (US)", x, y, true, font, graphics);
            enButton.Width = BUTTON_WIDTH;
            y += enButton.Height + SPACING;
            esButton = new MenuButton(new System.Action(() => SetLang(Language.Spanish)), "Español (MX)", x, y, true, font, graphics);
            esButton.Width = BUTTON_WIDTH;

            y += enButton.Height + SPACING;

            //frButton = new MenuButton(new Action(() => SetLang(Language.French)), "Français", x, y, true, font, graphics);
            //frButton.Width = BUTTON_WIDTH;
            //deButton = new MenuButton(new Action(() => SetLang(Language.German)), "Deutsche", x2, y, true, font, graphics);
            //deButton.Width = BUTTON_WIDTH;

            //y += frButton.Height + SPACING;

            //ukButton = new MenuButton(new Action(() => SetLang(Language.EnglishUK)), "English (UK)", x, y, true, font, graphics);
            //ukButton.Width = BUTTON_WIDTH;
            //itButton = new MenuButton(new Action(() => SetLang(Language.Italian)), "Italiano", x2, y, true, font, graphics);
            //itButton.Width = BUTTON_WIDTH;

            //y += ukButton.Height + SPACING;

            //eoButton = new MenuButton(new Action(() => SetLang(Language.Esperanto)), "Esperanto", x, y, true, font, graphics);
            //eoButton.Width = BUTTON_WIDTH;
            //tlButton = new MenuButton(new Action(() => SetLang(Language.Tagalog)), "Tagalog (Filipino)", x2, y, true, font, graphics);
            //tlButton.Width = BUTTON_WIDTH;
        }

        #endregion

        #region Public Methods

        public void Show()
        {
            Active = true;
        }
        public void Hide()
        {
            Active = false;
        }

        public void Update()
        {
            if (Active)
            {
                enButton.Update();
                esButton.Update();
                //frButton.Update();
                //itButton.Update();
                //eoButton.Update();
                //deButton.Update();
                //ukButton.Update();
                //tlButton.Update();
            }
        }

        public void Draw(SpriteBatch spriteBatch)
        {
            if (Active)
            {
                spriteBatch.Draw(fullImg, fullRect, new Color(150, 150, 150));
                spriteBatch.Draw(bgImg, bgRect, Color.White);
                spriteBatch.DrawString(font, infoText, infoTextPos, Color.Black);

                enButton.Draw(spriteBatch);
                esButton.Draw(spriteBatch);
                //frButton.Draw(spriteBatch);
                //itButton.Draw(spriteBatch);
                //eoButton.Draw(spriteBatch);
                //deButton.Draw(spriteBatch);
                //ukButton.Draw(spriteBatch);
                //tlButton.Draw(spriteBatch);
            }
        }

        public List<MenuButton> GetButtons()
        {
            List<MenuButton> returnVal = new List<MenuButton>()
            {
                enButton,
                esButton,
                //frButton,
                //itButton,
                //eoButton,
                //deButton,
                //ukButton,
                //tlButton,
            };
            return returnVal;
        }

        public void AddLanguageSetHandler(Action<Language> handler)
        {
            onLangSet += handler;
        }

        #endregion

        #region Private Methods

        private void SetLang(Language lang)
        {
            onLangSet?.Invoke(lang);
            Active = false;
        }

        #endregion
    }
}
