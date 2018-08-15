using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Content;

namespace OneCannonOneArmy
{
    public class UserInterface
    {
        #region Fields & Properties

        Texture2D bgImg;
        Rectangle bgRect;
        Texture2D alienImg;
        Rectangle alienRect;
        Color alienColor;
        Texture2D alienEyeImg;
        Rectangle alienEyeRect;
        Texture2D projImg;
        Rectangle projRect;

        Texture2D lifeImg;
        List<Rectangle> lifeRects = new List<Rectangle>();
        List<Color> lifeColors = new List<Color>();

        MenuButton nameButton;
        MenuButton delButton;

        public User User;

        Action<User> userClick;
        Action<User> delClick;

        const int SPACING = 10;
        const int EYE_SPACING = 5;

        const int PROJECTILE_SIZE = 30;

        const int ALIEN_WIDTH = 100;
        const int ALIEN_HEIGHT = 50;
        const int LIFE_SIZE = 50;

        Color nameShadeColor;
        Color delShadeColor;

        ContentManager content;

        public int X
        {
            get
            {
                return bgRect.X;
            }
            set
            {
                bgRect.X = value;
                alienRect.X = bgRect.X + SPACING;
                alienEyeRect.X = alienRect.X + (alienRect.Width / 2 - (alienEyeRect.Width / 2));
                delButton.X = bgRect.Right - delButton.Width - SPACING;
                projRect.X = alienRect.Right - PROJECTILE_SIZE;
                PositionUsernameButton();
                PositionLives();
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
                alienRect.Y = bgRect.Y + (bgRect.Height / 2 - (ALIEN_HEIGHT / 2));
                alienEyeRect.Y = alienRect.Y + (EYE_SPACING * 4);
                delButton.Y = bgRect.Y + (bgRect.Height / 2 - (delButton.Height / 2));
                projRect.Y = alienRect.Bottom - PROJECTILE_SIZE / 2;
                PositionUsernameButton();
                PositionLives();
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

        public UserInterface(User user, Texture2D alienImg, Texture2D alienEyeImg,
            GraphicsDevice graphics, Action<User> playClicked, Action<User> delClicked, 
            SpriteFont font, Texture2D delIcon, int x, int y, int width, int height, 
            ContentManager content, Texture2D lifeImg)
        {
            User = user;
            this.content = content;

            userClick = playClicked;
            delClick = delClicked;

            bgImg = new Texture2D(graphics, width, height);
            bgImg = DrawHelper.AddBorder(bgImg, 3, Color.Gray, Color.LightGray);
            bgRect = new Rectangle(x, y, width, height);
            this.alienImg = alienImg;
            alienRect = new Rectangle(bgRect.X + SPACING, bgRect.Y + (bgRect.Height / 2 - (ALIEN_HEIGHT / 2)), 
                ALIEN_WIDTH, ALIEN_HEIGHT);
            this.alienEyeImg = alienEyeImg;
            alienEyeRect = new Rectangle(0, alienRect.Y + (EYE_SPACING * 4),
                EYE_SPACING * 5, EYE_SPACING);
            alienEyeRect.X = alienRect.X + (alienRect.Width / 2 - (alienEyeRect.Width / 2));
            alienColor = new Color(user.AvatarR, user.AvatarG, user.AvatarB);
            projImg = content.Load<Texture2D>(user.ProjectileAsset);
            projRect = new Rectangle(alienRect.Right - PROJECTILE_SIZE, alienRect.Bottom - PROJECTILE_SIZE / 2,
                PROJECTILE_SIZE, PROJECTILE_SIZE);

            delButton = new MenuButton(OnDelete, 0, 0, false, null, delIcon);
            delButton.X = bgRect.Right - delButton.Width - SPACING;
            delButton.Y = bgRect.Y + (bgRect.Height / 2 - (delButton.Height / 2));
            delButton.ImgWidth = (int)(delButton.Width * 0.25f);
            delButton.ImgHeight = (int)(delButton.Height * 0.3025f);

            nameButton = new MenuButton(OnUsernameClick, user.Username, 0, 
                0, false, font, null);
            PositionUsernameButton();

            this.lifeImg = lifeImg;
            int lifeX = alienRect.Right + SPACING;
            int lifeY = bgRect.Y + (bgRect.Height / 2 - (LIFE_SIZE / 2));
            for (int i = 0; i < GameInfo.MAX_LIVES; i++)
            {
                lifeRects.Add(new Rectangle(lifeX, lifeY, LIFE_SIZE, LIFE_SIZE));
                lifeX += LIFE_SIZE + SPACING;
                if (i < user.Lives)
                {
                    // The user has more lives than "i"
                    lifeColors.Add(new Color(220, 0, 0));
                }
                else
                {
                    lifeColors.Add(Color.LightGray);
                }
            }
        }

        #endregion

        #region Public Methods

        public void Update()
        {
            nameButton.Update();
            nameShadeColor = nameButton.Hovered ? Color.DarkGray : Color.LightGray;
            delButton.Update();
            delShadeColor = delButton.Hovered ? Color.DarkGray : Color.LightGray;
        }

        public void Draw(SpriteBatch spriteBatch)
        {
            spriteBatch.Draw(bgImg, bgRect, Color.DarkGray);

            nameButton.Draw(spriteBatch, nameShadeColor);
            delButton.Draw(spriteBatch, delShadeColor);

            spriteBatch.Draw(alienImg, alienRect, alienColor);
            spriteBatch.Draw(alienEyeImg, alienEyeRect, Color.White);
            spriteBatch.Draw(projImg, projRect, Color.White);

            for (int i = 0; i < lifeRects.Count; i++)
            {
                spriteBatch.Draw(lifeImg, lifeRects[i], lifeColors[i]);
            }
        }

        public void UpdateInfo(User user)
        {
            nameButton.Text = user.Username;
            nameButton.X = bgRect.X;
            nameButton.Y = bgRect.Y;
            nameButton.Width = bgRect.Width - delButton.Width - SPACING;
            nameButton.Height = bgRect.Height;
            nameButton.TextX = bgRect.Right - delButton.Width - nameButton.Width - SPACING;
            nameButton.TextY = bgRect.Y + (bgRect.Height / 2 - (nameButton.Height / 2));

            alienColor = new Color(user.AvatarR, user.AvatarG, user.AvatarB, 255);

            projImg = content.Load<Texture2D>(user.ProjectileAsset);

            for (int i = 0; i < lifeColors.Count; i++)
            {
                if (i < user.Lives)
                {
                    // The user has more lives than "i"
                    lifeColors[i] = new Color(220, 0, 0);
                }
                else
                {
                    lifeColors[i] = Color.LightGray;
                }
            }
        }

        public List<MenuButton> GetButtons()
        {
            List<MenuButton> buttons = new List<MenuButton>()
            {
                nameButton,
                delButton
            };
            return buttons;
        }

        #endregion

        #region Private Methods

        private void OnUsernameClick()
        {
            userClick?.Invoke(User);
        }
        private void OnDelete()
        {
            delClick?.Invoke(User);
        }

        private void PositionUsernameButton()
        {
            nameButton.X = bgRect.X;
            nameButton.Y = bgRect.Y;
            nameButton.Width = bgRect.Width - delButton.Width - SPACING;
            nameButton.Height = bgRect.Height;
            nameButton.TextX = (int)(bgRect.Right - delButton.Width - nameButton.Font.MeasureString(nameButton.Text).X - SPACING);
            nameButton.TextY = (int)(bgRect.Y + (bgRect.Height / 2 - (nameButton.Font.MeasureString(nameButton.Text).Y / 2)));
        }
        private void PositionLives()
        {
            int lifeX = alienRect.Right + SPACING;
            int lifeY = bgRect.Y + (bgRect.Height / 2 - (LIFE_SIZE / 2));
            for (int i = 0; i < GameInfo.MAX_LIVES; i++)
            {
                lifeRects[i] = new Rectangle(lifeX, lifeY, LIFE_SIZE, LIFE_SIZE);
                lifeX += LIFE_SIZE + SPACING;
            }
        }

        #endregion
    }
}
