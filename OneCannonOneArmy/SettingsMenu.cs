using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OneCannonOneArmy
{
    public delegate void SubmitUserInfo(string name, float r, float g, float b, string assetName, GraphicsSettings graphics, 
        VolumeSettings volumeSettings);

    public class SettingsMenu
    {
        #region Fields & Properties

        Textbox usernameBox;
        string usernamePre = Language.Translate("Username") + ": ";
        Vector2 usernamePrePos = new Vector2();

        List<Texture2D> projImgs;
        int projIndex = 0;
        Rectangle projRect;
        const int PROJ_SIZE = 100;
        MenuButton nextProj;
        MenuButton prevProj;

        ValueSlider rSlider;
        ValueSlider bSlider;
        ValueSlider gSlider;

        Texture2D alienImg;
        Rectangle alienRect;
        const int ALIEN_WIDTH = 100;
        const int ALIEN_HEIGHT = 50;
        const int EYE_SPACING = 5;
        Texture2D alienEyeImg;
        Rectangle alienEyeRect;
        Color alienColor;

        MenuButton submit;
        event SubmitUserInfo submitInfo;

        MenuButton replayTutorial;
        event System.Action replayTutorialClicked;

        public const int SPACING = 25;
        const int BUTTON_SPACING = 10;
        const int BUTTON_IMG_SIZE = 50;
        const int BUTTON_SIZE = 60;

        SpriteFont bigFont;

        MenuButton graphicsButton;
        MenuButton profileButton;
        MenuButton soundButton;

        SettingsState state;

        ValueSlider brightnessSlider;

        ValueSlider musicSlider;
        ValueSlider systemSlider;
        ValueSlider sfxSlider;
        ToggleButton muteToggle;
        const int MUTE_TOGGLE_SIZE = 50;

        MenuButton dirXInstallButton;

        public GraphicsSettings GraphicsSettings
        {
            get
            {
                return new GraphicsSettings(brightnessSlider.Value / 100.0f);
            }
        }
        public VolumeSettings VolumeSettings
        {
            get
            {
                return new VolumeSettings(systemSlider.Value, musicSlider.Value, sfxSlider.Value, muteToggle.Toggled);
            }
        }

        #endregion

        #region Constructors

        public SettingsMenu(int windowWidth, int windowHeight, GraphicsDevice graphics, int usernameWidth, int usernameHeight,
            SpriteFont bigFont, SpriteFont mediumFont, SpriteFont smallFont, int sliderWidth, int sliderHeight,
            List<Texture2D> projImgs, Texture2D arrowImg, Texture2D graphicsImg, Texture2D profileImg, Texture2D soundImg,
            Texture2D crossImg)
        {
            this.bigFont = bigFont;
            this.projImgs = projImgs;

            state = SettingsState.Profile;

            usernameBox = new Textbox(windowWidth / 2 - usernameWidth / 2, Utilities.MENU_Y_OFFSET, usernameWidth, usernameHeight,
                bigFont, false, graphics);
            usernameBox.Drawn = true;

            alienImg = Utilities.AlienImg;
            alienEyeImg = Utilities.AlienEyeImg;
            alienRect = new Rectangle(windowWidth / 2 - ALIEN_WIDTH / 2, usernameBox.Y + usernameBox.Height + SPACING,
                ALIEN_WIDTH, ALIEN_HEIGHT);
            alienEyeRect = new Rectangle(0, alienRect.Y + (EYE_SPACING * 4), EYE_SPACING * 5, EYE_SPACING);
            alienEyeRect.X = alienRect.X + (alienRect.Width / 2 - (alienEyeRect.Width / 2));

            rSlider = new ValueSlider(0, Utilities.RectImage, windowWidth / 2 - sliderWidth / 2, alienRect.Bottom + SPACING,
                sliderWidth, sliderHeight, smallFont, Language.Translate("Red"), 255.0f);
            rSlider.AddValueChangedHandler(ColorChanged);
            gSlider = new ValueSlider(0, Utilities.RectImage, rSlider.X, rSlider.Y + rSlider.Height + SPACING,
                sliderWidth, sliderHeight, smallFont, Language.Translate("Green"), 255.0f);
            gSlider.AddValueChangedHandler(ColorChanged);
            bSlider = new ValueSlider(0, Utilities.RectImage, gSlider.X, gSlider.Y + gSlider.Height + SPACING,
                sliderWidth, sliderHeight, smallFont, Language.Translate("Blue"), 255.0f);
            bSlider.AddValueChangedHandler(ColorChanged);

            projRect = new Rectangle(windowWidth / 2 - PROJ_SIZE / 2,
                bSlider.Y + bSlider.Height + SPACING, PROJ_SIZE, PROJ_SIZE);
            prevProj = new MenuButton(PrevProj, 0, 0,
                true, graphics, arrowImg);
            prevProj.X = projRect.X - prevProj.Width - SPACING;
            prevProj.Y = projRect.Y + projRect.Height / 2 - prevProj.Height / 2;
            nextProj = new MenuButton(NextProj, projRect.Right + SPACING, prevProj.Y, true, graphics, arrowImg);

            replayTutorial = new MenuButton(ReplayTutorial, Language.Translate("Replay Tutorial"), 0, 0, true, mediumFont, graphics);
            replayTutorial.X = windowWidth - replayTutorial.Width - SPACING;
            replayTutorial.Y = windowHeight - replayTutorial.Height - SPACING;

            submit = new MenuButton(new System.Action(() => submitInfo(usernameBox.Content, rSlider.Value, gSlider.Value, bSlider.Value,
                projImgs[projIndex].Name, new GraphicsSettings(brightnessSlider.Value / 100.0f), 
                new VolumeSettings(systemSlider.Value, musicSlider.Value, sfxSlider.Value, muteToggle.Toggled))),
                Language.Translate("Submit"), 0, 0, true, bigFont, graphics);
            submit.X = windowWidth / 2 - submit.Width / 2;
            submit.Y = windowHeight - submit.Height - SPACING;

            usernamePrePos.X = usernameBox.X - bigFont.MeasureString(usernamePre).X;
            usernamePrePos.Y = usernameBox.Y;

            graphicsButton = new MenuButton(new System.Action(() => state = SettingsState.Graphics), BUTTON_SPACING * 2, 0, true, graphics, graphicsImg);

            profileButton = new MenuButton(new System.Action(() => state = SettingsState.Profile), graphicsButton.X, 0, true, graphics, profileImg);

            soundButton = new MenuButton(new System.Action(() => state = SettingsState.Sound), graphicsButton.X, 0, true, graphics, soundImg);

            graphicsButton.ImgWidth = graphicsButton.ImgHeight = profileButton.ImgWidth = profileButton.ImgHeight =
                soundButton.ImgWidth = soundButton.ImgHeight = BUTTON_IMG_SIZE;
            graphicsButton.Width = graphicsButton.Height = profileButton.Width = profileButton.Height =
                soundButton.Width = soundButton.Height = BUTTON_SIZE;

            graphicsButton.Y = windowHeight / 2 - graphicsButton.Height / 2;
            profileButton.Y = graphicsButton.Y - profileButton.Height - BUTTON_SPACING;
            soundButton.Y = graphicsButton.Y + graphicsButton.Height + BUTTON_SPACING;

            brightnessSlider = new ValueSlider(0, Utilities.RectImage, rSlider.X, rSlider.Y,
                rSlider.Width, rSlider.Height, smallFont, Language.Translate("Brightness"));

            systemSlider = new ValueSlider(0, Utilities.RectImage, rSlider.X, rSlider.Y, rSlider.Width, rSlider.Height,
                smallFont, Language.Translate("System"));
            musicSlider = new ValueSlider(0, Utilities.RectImage, gSlider.X, gSlider.Y, gSlider.Width, gSlider.Height,
                smallFont, Language.Translate("Music"));
            sfxSlider = new ValueSlider(0, Utilities.RectImage, bSlider.X, bSlider.Y, bSlider.Width, bSlider.Height,
                smallFont, Language.Translate("Sound Effects"));
            muteToggle = new ToggleButton(soundImg, crossImg, graphics, sfxSlider.X, sfxSlider.Y + sfxSlider.Height + SPACING * 2,
                MUTE_TOGGLE_SIZE, MUTE_TOGGLE_SIZE);

            dirXInstallButton = new MenuButton(DirectXInstallClick, Language.Translate("Sound Issues?"), 
                muteToggle.X, muteToggle.Y + muteToggle.Height + SPACING, true, bigFont, graphics);
        }

        #endregion

        #region Public Methods

        public void Initialize(User user)
        {
            usernameBox.Content = user.Username;

            rSlider.Value = user.AvatarR;
            gSlider.Value = user.AvatarG;
            bSlider.Value = user.AvatarB;
            alienColor = new Color(rSlider.Value, gSlider.Value, bSlider.Value, 255);

            brightnessSlider.Value = user.GraphicsSettings.Brightness * 100.0f;

            systemSlider.Value = user.VolumeSettings.SystemVolume;
            musicSlider.Value = user.VolumeSettings.MusicVolume;
            sfxSlider.Value = user.VolumeSettings.SfxVolume;
            muteToggle.Toggled = user.VolumeSettings.Muted;

            // Set index, but ensure that index isn't -1
            projIndex = Math.Max(projImgs.IndexOf(projImgs.Find(x => x.Name == user.ProjectileAsset)), 0);
        }

        public void Update(GameTime gameTime)
        {
            switch (state)
            {
                case SettingsState.Profile:

                    usernameBox.Update(gameTime);
                    rSlider.Update();
                    gSlider.Update();
                    bSlider.Update();

                    nextProj.Update(gameTime);
                    prevProj.Update(gameTime);

                    break;

                case SettingsState.Graphics:

                    brightnessSlider.Update();

                    break;

                case SettingsState.Sound:

                    systemSlider.Update();
                    musicSlider.Update();
                    sfxSlider.Update();
                    muteToggle.Update(gameTime);
                    dirXInstallButton.Update(gameTime);

                    break;
            }

            replayTutorial.Update(gameTime);
            submit.Update(gameTime);

            profileButton.Active = state != SettingsState.Profile;
            graphicsButton.Active = state != SettingsState.Graphics;
            soundButton.Active = state != SettingsState.Sound;
            profileButton.Update(gameTime);
            graphicsButton.Update(gameTime);
            soundButton.Update(gameTime);
        }

        public void Draw(SpriteBatch spriteBatch)
        {
            switch (state)
            {
                case SettingsState.Profile:

                    spriteBatch.DrawString(bigFont, usernamePre, usernamePrePos, Textbox.ColorTheme);
                    usernameBox.Draw(spriteBatch);

                    rSlider.Draw(spriteBatch);
                    gSlider.Draw(spriteBatch);
                    bSlider.Draw(spriteBatch);

                    spriteBatch.Draw(alienImg, alienRect, alienColor);
                    spriteBatch.Draw(alienEyeImg, alienEyeRect, Color.White);

                    spriteBatch.Draw(projImgs[projIndex], projRect, Color.White);
                    nextProj.Draw(spriteBatch);
                    prevProj.Draw(spriteBatch, null, SpriteEffects.FlipHorizontally);

                    break;

                case SettingsState.Graphics:

                    brightnessSlider.Draw(spriteBatch);

                    break;

                case SettingsState.Sound:

                    systemSlider.Draw(spriteBatch);
                    musicSlider.Draw(spriteBatch);
                    sfxSlider.Draw(spriteBatch);
                    muteToggle.Draw(spriteBatch);
                    dirXInstallButton.Draw(spriteBatch);

                    break;
            }

            profileButton.Draw(spriteBatch);
            graphicsButton.Draw(spriteBatch);
            soundButton.Draw(spriteBatch);

            submit.Draw(spriteBatch);
            replayTutorial.Draw(spriteBatch);
        }

        public void AddOnReplayTutorialClickHandler(System.Action handler)
        {
            replayTutorialClicked += handler;
        }
        public void AddSubmitInfoHander(SubmitUserInfo handler)
        {
            submitInfo += handler;
        }

        public void Reset(User user)
        {
            usernameBox.Content = user.Username;
            rSlider.Value = user.AvatarR;
            gSlider.Value = user.AvatarG;
            bSlider.Value = user.AvatarB;
            projIndex = projImgs.FindIndex(x => x.Name == user.ProjectileAsset);
            alienColor = new Color(user.AvatarR, user.AvatarG, user.AvatarB);
        }

        public void LangChanged()
        {
            rSlider.ValueName = Language.Translate("Red");
            gSlider.ValueName = Language.Translate("Green");
            bSlider.ValueName = Language.Translate("Blue");

            submit.Text = Language.Translate("Submit");
            replayTutorial.Text = Language.Translate("Replay Tutorial");

            usernamePre = Language.Translate("Username") + ": ";
            usernamePrePos.X = usernameBox.X - bigFont.MeasureString(usernamePre).X;

            brightnessSlider.ValueName = Language.Translate("Brightness");

            systemSlider.ValueName = Language.Translate("System");
            musicSlider.ValueName = Language.Translate("Music");
            sfxSlider.ValueName = Language.Translate("Sound Effects");
            dirXInstallButton.Text = Language.Translate("Sound Issues?");
        }

        public List<MenuButton> GetButtons()
        {
            List<MenuButton> returnVal = new List<MenuButton>()
            {
                submit,
                replayTutorial,
                profileButton,
                graphicsButton,
                soundButton,
            };

            switch (state)
            {
                case SettingsState.Profile:
                    returnVal.Add(nextProj);
                    returnVal.Add(prevProj);
                    break;
                case SettingsState.Sound:
                    returnVal.Add(muteToggle.Button);
                    returnVal.Add(dirXInstallButton);
                    break;
            }

            return returnVal;
        }
        public List<Textbox> GetTextboxes()
        {
            if (state == SettingsState.Profile)
            {
                return new List<Textbox>()
                {
                    usernameBox
                };
            }
            else
            {
                return new List<Textbox>();
            }
        }
        public List<ValueSlider> GetSliders()
        {
            switch (state)
            {
                case SettingsState.Profile:
                    return new List<ValueSlider>()
                    {
                        rSlider, gSlider, bSlider
                    };
                case SettingsState.Graphics:
                    return new List<ValueSlider>()
                    {
                        brightnessSlider
                    };
                case SettingsState.Sound:
                    return new List<ValueSlider>()
                    {
                        systemSlider, musicSlider, sfxSlider
                    };
            }
            return new List<ValueSlider>();
        }

        #endregion

        #region Private Methods

        private void ReplayTutorial()
        {
            Popup.Show(Language.Translate(
                "Are you sure? The program will restart and the tutorial and story will be replayed."),
                true, new System.Action(() => replayTutorialClicked?.Invoke()), false);
        }

        private void ColorChanged()
        {
            alienColor.R = (byte)rSlider.Value;
            alienColor.G = (byte)gSlider.Value;
            alienColor.B = (byte)bSlider.Value;
        }

        private void NextProj()
        {
            if (projIndex < projImgs.Count - 1)
            {
                projIndex++;
            }
            else
            {
                projIndex = 0;
            }
        }
        private void PrevProj()
        {
            if (projIndex > 0)
            {
                projIndex--;
            }
            else
            {
                projIndex = projImgs.Count - 1;
            }
        }

        private void DirectXInstallClick()
        {
            Popup.Show(Language.Translate("Many sound issues can be resolved by reinstalling DirectX. Click \"Okay\" to " +
                "open the install\npage in your browser. Be sure to sure to close all programs and " + 
                "restart your computer after the\ninstall is complete."), 
                true, OpenDirectXInstall, false);
        }
        private void OpenDirectXInstall()
        {
            System.Diagnostics.Process.Start("https://www.microsoft.com/en-us/download/details.aspx?displaylang=en&id=35");
        }

        #endregion
    }

    public enum SettingsState
    {
        Profile,
        Graphics,
        Sound
    }
}