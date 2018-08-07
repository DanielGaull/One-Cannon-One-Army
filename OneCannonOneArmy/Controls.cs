using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;

namespace OneCannonOneArmy
{
    public static class Controls
    {
        static KeyControls controls = new KeyControls();
        public static Keys MoveLeftKey
        {
            get
            {
                return (Keys)controls.LeftKey;
            }
            set
            {
                controls.LeftKey = (int)value;
                controls.Save();
            }
        }
        public static Keys MoveRightKey
        {
            get
            {
                return (Keys)controls.RightKey;
            }
            set
            {
                controls.RightKey = (int)value;
                controls.Save();
            }
        }
        public static Keys LaunchKey
        {
            get
            {
                return (Keys)controls.FireKey;
            }
            set
            {
                controls.FireKey = (int)value;
                controls.Save();
            }
        }

        public static Keys SweepKey
        {
            get
            {
                return (Keys)controls.SweepKey;
            }
            set
            {
                controls.SweepKey = (int)value;
                controls.Save();
            }
        }
        public static Keys ToggleRapidFireKey
        {
            get
            {
                return (Keys)controls.ToggleRapidFireKey;
            }
            set
            {
                controls.ToggleRapidFireKey = (int)value;
                controls.Save();
            }
        }

        public static Keys PauseKey
        {
            get
            {
                return (Keys)controls.PauseKey;
            }
            set
            {
                controls.PauseKey = (int)value;
                controls.Save();
            }
        }

        public static Keys FullscreenKey
        {
            get
            {
                return (Keys)controls.FullscreenKey;
            }
            set
            {
                controls.FullscreenKey = (int)value;
                controls.Save();
            }
        }
        public static Keys ScreenshotKey
        {
            get
            {
                return (Keys)controls.ScreenshotKey;
            }
            set
            {
                controls.ScreenshotKey = (int)value;
                controls.Save();
            }
        }
        
        public static Keys HotbarKey1
        {
            get
            {
                return (Keys)controls.HotbarKey1;
            }
            set
            {
                controls.HotbarKey1 = (int)value;
                controls.Save();
            }
        }
        public static Keys HotbarKey2
        {
            get
            {
                return (Keys)controls.HotbarKey2;
            }
            set
            {
                controls.HotbarKey2 = (int)value;
                controls.Save();
            }
        }
        public static Keys HotbarKey3
        {
            get
            {
                return (Keys)controls.HotbarKey3;
            }
            set
            {
                controls.HotbarKey3 = (int)value;
                controls.Save();
            }
        }
        public static Keys HotbarKey4
        {
            get
            {
                return (Keys)controls.HotbarKey4;
            }
            set
            {
                controls.HotbarKey4 = (int)value;
                controls.Save();
            }
        }
        public static Keys HotbarKey5
        {
            get
            {
                return (Keys)controls.HotbarKey5;
            }
            set
            {
                controls.HotbarKey5 = (int)value;
                controls.Save();
            }
        }

        public static bool CtrlPressed()
        {
            KeyboardState state = Keyboard.GetState();
            return (state.IsKeyDown(Keys.RightControl) || state.IsKeyDown(Keys.LeftControl));
        }
        public static bool CtrlPressed(KeyboardState state)
        {
            return (state.IsKeyDown(Keys.RightControl) || state.IsKeyDown(Keys.LeftControl));
        }

        public static void SetControlFor(Control control, Keys value)
        {
            switch (control)
            {
                case Control.MoveLeft:
                    MoveLeftKey = value;
                    break;
                case Control.MoveRight:
                    MoveRightKey = value;
                    break;
                case Control.Launch:
                    LaunchKey = value;
                    break;
                case Control.Sweep:
                    SweepKey = value;
                    break;
                case Control.Pause:
                    PauseKey = value;
                    break;
                case Control.Fullscreen:
                    FullscreenKey = value;
                    break;
                case Control.ToggleRapidFire:
                    ToggleRapidFireKey = value;
                    break;
                case Control.Hotbar1:
                    HotbarKey1 = value;
                    break;
                case Control.Hotbar2:
                    HotbarKey2 = value;
                    break;
                case Control.Hotbar3:
                    HotbarKey3 = value;
                    break;
                case Control.Hotbar4:
                    HotbarKey4 = value;
                    break;
                case Control.Hotbar5:
                    HotbarKey5 = value;
                    break;
                    //case Control.Screenshot:
                    //    ScreenshotKey = value;
                    //    break;
            }
        }
        public static Keys GetControlFor(Control control)
        {
            switch (control)
            {
                case Control.MoveLeft:
                    return MoveLeftKey;
                case Control.MoveRight:
                    return MoveRightKey;
                case Control.Launch:
                    return LaunchKey;
                case Control.Sweep:
                    return SweepKey;
                case Control.Pause:
                    return PauseKey;
                case Control.Fullscreen:
                    return FullscreenKey;
                case Control.ToggleRapidFire:
                    return ToggleRapidFireKey;
                case Control.Hotbar1:
                    return HotbarKey1;
                case Control.Hotbar2:
                    return HotbarKey2;
                case Control.Hotbar3:
                    return HotbarKey3;
                case Control.Hotbar4:
                    return HotbarKey4;
                case Control.Hotbar5:
                    return HotbarKey5;
                    //case Control.Screenshot:
                    //    return ScreenshotKey;
            }
            return Keys.None;
        }
    }

    public class ControlMenu
    {
        #region Fields

        //List<string> controlNames = new List<string>();
        //List<Vector2> controlNamePositions = new List<Vector2>();

        //List<MenuButton> controlSelectors = new List<MenuButton>();
        //Dictionary<Control, Keys> values = new Dictionary<Control, Keys>();

        //List<string> keyNames = new List<string>();
        //List<Vector2> keyNamePositions = new List<Vector2>();

        //Texture2D rectImg;
        //List<Rectangle> rects = new List<Rectangle>();

        //SpriteFont bigFont;
        //SpriteFont smallFont;

        const int SPACING = 5;
        const int ROWS = 2;
        const int Y_OFFSET = 170;
        int X_OFFSET;

        //Control waitingFor;
        //bool isWaitingForKey = false;
        //KeyboardState keyboard;

        Texture2D selectedControlImg;
        Rectangle selectedControlRect;

        List<ControlInterface> interfaces = new List<ControlInterface>();

        #endregion

        #region Constructors

        public ControlMenu(GraphicsDevice graphics, int windowWidth, int windowHeight, SpriteFont bigFont, SpriteFont smallFont)
        {
            //this.bigFont = bigFont;
            //this.smallFont = smallFont;

            //rectImg = new Texture2D(graphics, windowWidth / 3, (int)bigFont.MeasureString("A").Y + SPACING * 2);
            //rectImg = DrawHelper.AddBorder(rectImg, 2, Color.Black, Color.Transparent);

            int width = windowWidth / 3;
            int height = (int)bigFont.MeasureString("A").Y + SPACING * 2;
            X_OFFSET = windowWidth / 2 - (width * 2 + SPACING) / 2;

            //InitializeDictionary();

            //for (int i = 0; i < values.Count; i++)
            //{
            //controlNames.Add(LanguageTranslator.Translate(((Control)i).ToString().AddSpaces()));
            //controlNamePositions.Add(new Vector2());
            //keyNames.Add(values[(Control)i].ToString().AddSpaces());
            //keyNamePositions.Add(new Vector2());
            //rects.Add(new Rectangle(0, 0, rectImg.Width, rectImg.Height));
            //controlSelectors.Add(new MenuButton(() => SetControlFor((Control)i), "", 0, 0, false, null, null));
            //controlSelectors[i].Width = rects[i].Width;
            //controlSelectors[i].Height = rects[i].Height;
            //}
            for (int i = 0; i < Enum.GetNames(typeof(Control)).Count(); i++)
            {
                interfaces.Add(new ControlInterface(bigFont, smallFont, 0, 0, width, height, graphics, (Control)i));
                interfaces[i].AddKeyChangedHandler(KeyChanged);
                interfaces[i].AddWhenSelectedHandler(WhenSelected);
            }
            Position();

            selectedControlImg = Utilities.RectImage;
            selectedControlRect = new Rectangle(0, 0, width, height);
        }

        #endregion

        #region Public Methods

        public void Update()
        {
            //keyboard = Keyboard.GetState();
            //if (isWaitingForKey)
            //{
            //    if (keyboard.GetPressedKeys().Count() > 0)
            //    {
            //        Controls.SetControlFor(waitingFor, keyboard.GetPressedKeys()[0]);
            //    }
            //    selectedControlRect.X = rects[(int)waitingFor].X;
            //    selectedControlRect.Y = rects[(int)waitingFor].Y;
            //}

            //for (int i = 0; i < controlSelectors.Count; i++)
            //{
            //    controlSelectors[i].Update();
            //}

            for (int i = 0; i < interfaces.Count; i++)
            {
                interfaces[i].Update();
            }
        }

        public void Draw(SpriteBatch spriteBatch)
        {
            //for (int i = 0; i < values.Count; i++)
            //{
            //    if (isWaitingForKey)
            //    {
            //        spriteBatch.Draw(selectedControlImg, selectedControlRect, Color.White);
            //    }
            //    spriteBatch.Draw(rectImg, rects[i], Color.White);
            //    spriteBatch.DrawString(smallFont, controlNames[i], controlNamePositions[i], Color.Black);
            //    spriteBatch.DrawString(bigFont, keyNames[i], keyNamePositions[i], Color.Black);
            //}

            for (int i = 0; i < interfaces.Count; i++)
            {
                interfaces[i].Draw(spriteBatch);
            }
        }

        public List<MenuButton> GetButtons()
        {
            List<MenuButton> returnVal = new List<MenuButton>();

            for (int i = 0; i < interfaces.Count; i++)
            {
                returnVal.AddRange(interfaces[i].GetButtons());
            }

            return returnVal;
        }

        #endregion

        #region Private Methods

        //private void InitializeDictionary()
        //{
        //    values.Clear();
        //    values.Add(Control.MoveRight, Controls.MoveRightKey);
        //    values.Add(Control.MoveLeft, Controls.MoveLeftKey);
        //    values.Add(Control.Launch, Controls.LaunchKey);
        //    values.Add(Control.Sweep, Controls.SweepKey);
        //    values.Add(Control.Pause, Controls.PauseKey);
        //    values.Add(Control.Fullscreen, Controls.FullscreenKey);
        //    //values.Add(Control.Screenshot, Controls.ScreenshotKey);
        //}

        private void Position()
        {
            int x = X_OFFSET, y = Y_OFFSET, col = 0, itemsInCurrentCol = 0;
            for (int i = 0; i < interfaces.Count; i++)
            {
                if (itemsInCurrentCol >= (int)Math.Ceiling(interfaces.Count / (double)ROWS))
                {
                    itemsInCurrentCol = 0;
                    col++;
                    x += interfaces[0].Width + SPACING;
                    y = Y_OFFSET;
                }
                interfaces[i].X = x;
                interfaces[i].Y = y;

                itemsInCurrentCol++;
                y += interfaces[i].Height + SPACING;
            }
            //for (int i = 0; i < values.Count; i++)
            //{
            //    if (itemsInCurrentCol >= MAX_CONTROLS_IN_COL)
            //    {
            //        itemsInCurrentCol = 0;
            //        col++;
            //        x += rects[0].Width + SPACING;
            //        y = Y_OFFSET;
            //    }
            //    rects[i] = new Rectangle(x, y, rects[i].Width, rects[i].Height);
            //    controlNamePositions[i] = new Vector2(rects[i].X + SPACING, rects[i].Y + SPACING / 2);
            //    keyNamePositions[i] = new Vector2(controlNamePositions[i].X, 
            //        controlNamePositions[i].Y + smallFont.MeasureString(controlNames[i]).Y);
            //    controlSelectors[i].X = rects[i].X;
            //    controlSelectors[i].Y = rects[i].Y;

            //    itemsInCurrentCol++;
            //    y += rects[i].Height + SPACING;
            //}

        }

        private void KeyChanged(Control control)
        {
            // Get the index of the interface that called the event
            int index = 0;
            for (int i = 0; i < interfaces.Count; i++)
            {
                if (interfaces[i].Control == control)
                {
                    index = i;
                    break;
                }
            }

            Keys newKey = interfaces[index].Key;
            for (int i = 0; i < interfaces.Count; i++)
            {
                if (interfaces[i].Key == newKey && i != index)
                {
                    interfaces[index].RevertToPrevious();
                    break;
                }
            }
        }
        private void WhenSelected(Control control)
        {
            for (int i = 0; i < interfaces.Count; i++)
            {
                if (interfaces[i].Selected && interfaces[i].Control != control)
                {
                    interfaces[i].Selected = false;
                    break;
                }
            }
        }

        #endregion
    }
    public class ControlInterface
    {
        #region Fields & Properties

        public Control Control;
        public Keys Key;
        Keys prevKey;

        Texture2D borderImg;
        Rectangle borderRect;

        MenuButton selectButton;

        Vector2 controlNamePos;
        Vector2 keyNamePos;

        SpriteFont smallFont;
        SpriteFont bigFont;

        public bool Selected = false;

        public int X
        {
            get
            {
                return borderRect.X;
            }
            set
            {
                borderRect.X = value;
                Position();
            }
        }
        public int Y
        {
            get
            {
                return borderRect.Y;
            }
            set
            {
                borderRect.Y = value;
                Position();
            }
        }
        public int Width
        {
            get
            {
                return borderRect.Width;
            }
        }
        public int Height
        {
            get
            {
                return borderRect.Height;
            }
        }

        const int SPACING = 5;

        KeyboardState keyboard;

        Texture2D selectedImg;
        Rectangle selectedRect;

        event Action<Control> keyChanged;
        event Action<Control> whenSelected;

        #endregion

        #region Constructors

        public ControlInterface(SpriteFont bigFont, SpriteFont smallFont, int x, int y, int width, int height, GraphicsDevice graphics,
            Control control)
        {
            this.bigFont = bigFont;
            this.smallFont = smallFont;

            Control = control;
            Key = Controls.GetControlFor(control);

            borderImg = new Texture2D(graphics, width, height);
            borderImg = DrawHelper.AddBorder(borderImg, 2, Color.Black, Color.Transparent);
            borderRect = new Rectangle(x, y, width, height);

            selectButton = new MenuButton(Select, "", 0, 0, false, null, null);
            selectButton.Width = width;
            selectButton.Height = height;

            controlNamePos = new Vector2();
            keyNamePos = new Vector2();

            selectedImg = Utilities.RectImage;
            selectedRect = new Rectangle(x, y, width, height);

            Position();
        }

        #endregion

        #region Public Methods

        public void Update()
        {
            selectButton.Update();

            keyboard = Keyboard.GetState();
            if (Selected)
            {
                if (keyboard.GetPressedKeys().Count() > 0)
                {
                    prevKey = Key;
                    Key = keyboard.GetPressedKeys()[0];
                    Controls.SetControlFor(Control, Key);
                    keyChanged?.Invoke(Control);
                }
            }
        }

        public void Draw(SpriteBatch spriteBatch)
        {
            if (Selected)
            {
                spriteBatch.Draw(selectedImg, selectedRect, Color.White);
            }
            spriteBatch.Draw(borderImg, borderRect, Color.White);
            spriteBatch.DrawString(smallFont, Language.Translate(Control.ToString().AddSpaces()), controlNamePos, Color.Black);
            spriteBatch.DrawString(bigFont, Key.ToString(), keyNamePos, Color.Black);
        }

        public void RevertToPrevious()
        {
            Key = prevKey;
            Controls.SetControlFor(Control, Key);
        }

        public List<MenuButton> GetButtons()
        {
            return new List<MenuButton>()
            {
                selectButton
            };
        }

        public void AddKeyChangedHandler(Action<Control> handler)
        {
            keyChanged += handler;
        }
        public void AddWhenSelectedHandler(Action<Control> handler)
        {
            whenSelected += handler;
        }

        #endregion

        #region Private Methods

        private void Position()
        {
            selectButton.X = borderRect.X;
            selectButton.Y = borderRect.Y;

            selectedRect.X = borderRect.X;
            selectedRect.Y = borderRect.Y;

            controlNamePos.X = borderRect.X + SPACING;
            controlNamePos.Y = borderRect.Y + SPACING;

            keyNamePos.X = controlNamePos.X;
            keyNamePos.Y = borderRect.Bottom - bigFont.MeasureString(Key.ToString()).Y;
        }

        private void Select()
        {
            Selected = !Selected;
            if (Selected)
            {
                whenSelected?.Invoke(Control);
            }
        }

        #endregion
    }

    public enum Control
    {
        MoveRight,
        MoveLeft,
        Launch,
        Sweep,
        Pause,
        Fullscreen,
        ToggleRapidFire,
        Hotbar1,
        Hotbar2,
        Hotbar3,
        Hotbar4,
        Hotbar5,
        //Screenshot
    }
}
