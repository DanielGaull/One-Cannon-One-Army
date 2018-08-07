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
    public class Tutorial
    {
        #region Fields & Properties

        public bool Playing { get; private set; }

        Texture2D rectImg;
        Rectangle fullRect;
        Rectangle highlightRect;

        string instructions = "";
        Vector2 instructPos = new Vector2();

        Texture2D alienImg;
        Texture2D alienEyeSheet;
        Rectangle alienRect;
        Rectangle alienEyeDrawRect;
        Rectangle alienEyeSourceRect;
        Vector2 alienTravelingTo;

        const int ALIEN_GOTO_ALLOW_RADIUS = 30;

        int windowWidth;
        int windowHeight;

        SpriteFont bigFont;
        SpriteFont mediumFont;
        SpriteFont smallFont;

        int stage = 0;

        const int ALIEN_WIDTH = 150;
        const int ALIEN_HEIGHT = 75;
        const int EYE_SPACING = 30;
        const int EYE_FRAME_SPACING = 84;

        KeyboardState currentKeyboard;
        KeyboardState prevKeyboard;

        bool alienLoaded = false;

        event Action<TutorialAction> onlyAllow;
        event System.Action onComplete;

        bool waitingForAction = false;

        MenuButton skipTutorialButton;
        event System.Action skip;
        
        Rectangle instructRect;

        const int SPACING = 5;

        #endregion

        #region Constructors

        public Tutorial(SpriteFont bigFont, SpriteFont mediumFont, SpriteFont smallFont, Texture2D alienImg, Texture2D eyeSheet,
            int windowWidth, int windowHeight, GraphicsDevice graphics)
        {
            this.bigFont = bigFont;
            this.mediumFont = mediumFont;
            this.smallFont = smallFont;

            this.windowWidth = windowWidth;
            this.windowHeight = windowHeight;

            this.alienImg = alienImg;
            this.alienEyeSheet = eyeSheet;

            alienRect = new Rectangle(0, 0, ALIEN_WIDTH, ALIEN_HEIGHT);
            alienEyeDrawRect = new Rectangle(0, 0, EYE_SPACING * 2, EYE_SPACING / 2);
            alienEyeSourceRect = new Rectangle(0, 0, EYE_FRAME_SPACING, eyeSheet.Height);

            rectImg = Utilities.RectImage;

            fullRect = new Rectangle(0, 0, windowWidth, windowHeight);
            highlightRect = new Rectangle();

            alienTravelingTo = new Vector2(-1, -1);

            skip += new System.Action(() => Playing = false);
            skipTutorialButton = new MenuButton(Skip, Language.Translate("Skip"), 0, 0, true, bigFont, graphics);
            skipTutorialButton.X = windowWidth / 2 - skipTutorialButton.Width / 2;
            skipTutorialButton.Y = (windowHeight / 2 - alienRect.Height / 2) + alienRect.Height;

            instructRect = new Rectangle();
        }

        #endregion

        #region Public Methods

        public void Play(Menu menu)
        {
            Playing = true;
            PrepForStage(0, menu);
        }

        public void Update(Menu menu, GameTime gameTime)
        {
            currentKeyboard = Keyboard.GetState();

            if (currentKeyboard.IsKeyDown(Keys.Space) && !prevKeyboard.IsKeyDown(Keys.Space) && alienLoaded && !waitingForAction)
            {
                if (stage + 1 < TutorialStages.Instructions.Count)
                {
                    stage++;
                    PrepForStage(stage, menu);
                }
                else
                {
                    Playing = false;
                    onComplete?.Invoke();
                }
            }
            //else if (currentKeyboard.IsKeyDown(Keys.Left) && !prevKeyboard.IsKeyDown(Keys.Left) && alienLoaded)
            //{
            //    if (stage - 1 >= 0)
            //    {
            //        stage--;
            //        PrepForStage(stage, menu);
            //    }
            //}

            skipTutorialButton.Active = (stage == 0);
            if (stage == 0)
            {
                // Allow the user to skip
                skipTutorialButton.Update();
            }

            if (alienTravelingTo.X > 0 && alienTravelingTo.Y > 0)
            {
                UpdateAlienLocation(gameTime);
            }

            prevKeyboard = currentKeyboard;
        }

        public void Draw(SpriteBatch spriteBatch)
        {
            spriteBatch.Draw(rectImg, fullRect, Color.Black * 0.5f);
            if (alienLoaded)
            {
                spriteBatch.Draw(rectImg, instructRect, Color.DarkGray);
                spriteBatch.DrawString(mediumFont, instructions, instructPos, Color.White);
                spriteBatch.Draw(rectImg, highlightRect, Color.White * 0.5f);
                if (stage == 0)
                {
                    // Allow user to skip
                    skipTutorialButton.Draw(spriteBatch);
                }
            }
            spriteBatch.Draw(alienImg, alienRect, Color.White);
            spriteBatch.Draw(alienEyeSheet, alienEyeDrawRect, alienEyeSourceRect, Color.White);
        }

        public void ActionCompleted(Menu menu)
        {
            if (stage + 1 < TutorialStages.Instructions.Count)
            {
                stage++;
                PrepForStage(stage, menu);
            }
        }

        public void AddModeChangedHandler(Action<TutorialAction> handler)
        {
            onlyAllow += handler;
        }
        public void AddOnCompleteHandler(System.Action handler)
        {
            onComplete += handler;
        }
        public void AddOnSkipHandler(System.Action handler)
        {
            skip += handler;
        }

        public List<MenuButton> GetButtons()
        {
            return new List<MenuButton>()
            {
                skipTutorialButton
            };
        }

        #endregion

        #region Private Methods

        private void Skip()
        {
            skip?.Invoke();
        }

        private void SetAlienMood(bool mood)
        {
            if (mood)
            {
                MakeHappy();
            }
            else
            {
                MakeNeutral();
            }
        }
        private void MakeNeutral()
        {
            alienEyeSourceRect.X = 0;
        }
        private void MakeHappy()
        {
            alienEyeSourceRect.X = EYE_FRAME_SPACING;
        }

        private void PrepForStage(int stage, Menu menu)
        {
            AlienGoTo(TutorialStages.AlienPositions[stage]);
            SetAlienMood(TutorialStages.IsAlienHappy[stage]);

            SetInstructions(Language.Translate(TutorialStages.Instructions[stage]), 
                TutorialStages.InstructionPositions[stage], ButtonForAction(TutorialStages.ActionsAllowed[stage],
                TutorialStages.Highlights[stage]));

            highlightRect = GetRectangleFor(TutorialStages.Highlights[stage], menu);

            TutorialAction action = TutorialStages.ActionsAllowed[stage];
            onlyAllow?.Invoke(action);
            waitingForAction = (action.Action != TutorialTask.None);
        }

        private void AlienGoTo(Vector2 location)
        {
            if (location.X == -1)
            {
                location.X = windowWidth / 2 - alienRect.Width / 2;
            }
            if (location.Y == -1)
            {
                location.Y = windowHeight / 2 - alienRect.Height / 2;
            }
            alienTravelingTo = location;
            alienLoaded = false;
        }
        private void UpdateAlienLocation(GameTime gameTime)
        {
            if (!(alienRect.X == alienTravelingTo.X && alienRect.Y == alienTravelingTo.Y))
            {
                Vector2 pos = new Vector2(alienRect.X, alienRect.Y);

                if (!((float)Math.Pow((double)(alienRect.X - alienTravelingTo.X), (double)2.0) + Math.Pow(alienRect.Y - alienTravelingTo.Y, 2) < (ALIEN_GOTO_ALLOW_RADIUS ^ 2)))
                // This is the same as the algorithm (x - gotoX)^2 + (y - gotoY)^2 < radius^2
                {
                    Vector2 distance = alienTravelingTo - pos;

                    distance.Normalize();
                    pos += distance * (float)gameTime.ElapsedGameTime.TotalMilliseconds * GameInfo.TUTORIAL_ALIEN_SPD;

                    alienRect.X = (int)pos.X;
                    alienRect.Y = (int)pos.Y;
                }
                else
                {
                    // The alien is close enough to just be set to the location
                    alienRect.X = (int)alienTravelingTo.X;
                    alienRect.Y = (int)alienTravelingTo.Y;
                }
            }
            else
            {
                alienTravelingTo = new Vector2(-1, -1);
                alienLoaded = true;
            }
            RepositionAlienEyes();
        }
        private void RepositionAlienEyes()
        {
            alienEyeDrawRect.X = alienRect.X + alienRect.Width / 2 - (alienEyeDrawRect.Width / 2);
            alienEyeDrawRect.Y = alienRect.Y + (EYE_SPACING / 2);
        }

        private void SetInstructions(string instructions, Vector2 pos, string buttonToContinue)
        {
            if (pos.X == -1)
            {
                pos.X = windowWidth / 2 - mediumFont.MeasureString(instructions).X / 2;
            }
            if (pos.Y == -1)
            {
                pos.Y = windowHeight / 2 - mediumFont.MeasureString(instructions).Y / 2;
            }

            if (buttonToContinue == null)
            {
                this.instructions = instructions + "\n" + Language.Translate("(Press space to continue)");
            }
            else
            {
                this.instructions = instructions + "\n" + string.Format(Language.Translate("(Press {0} to continue)"), 
                    Language.Translate(buttonToContinue));
            }
            instructPos = pos;
            instructRect.X = (int)instructPos.X - SPACING;
            instructRect.Y = (int)instructPos.Y - SPACING;
            instructRect.Width = (int)mediumFont.MeasureString(this.instructions).X + SPACING * 2;
            instructRect.Height = (int)mediumFont.MeasureString(this.instructions).Y + SPACING * 2;
        }

        private Rectangle GetRectangleFor(TutorialActionArea destination, Menu menu)
        {
            switch (destination)
            {
                case TutorialActionArea.Achievements:
                    return menu.AchievementsRect;
                case TutorialActionArea.Back:
                    return menu.BackRect;
                case TutorialActionArea.ColorSliders:
                    return menu.ColorSliderRect;
                case TutorialActionArea.Controls:
                    return menu.ControlsRect;
                case TutorialActionArea.Crafting:
                    return menu.CraftRect;
                case TutorialActionArea.CreateNewUser:
                    return menu.CreateNewUserRect;
                case TutorialActionArea.Gifts:
                    return menu.GiftsRect;
                case TutorialActionArea.Language:
                    return menu.LanguageRect;
                case TutorialActionArea.MainMenu:
                    return menu.MainMenuRect;
                case TutorialActionArea.Organize:
                    return menu.OrganizeRect;
                case TutorialActionArea.Play:
                    return menu.PlayRect;
                case TutorialActionArea.PlayerIcon:
                    return menu.PlayerIconRect;
                case TutorialActionArea.Quit:
                    return menu.QuitRect;
                case TutorialActionArea.Settings:
                    return menu.SettingsRect;
                case TutorialActionArea.Shop:
                    return menu.ShopRect;
                case TutorialActionArea.Resume:
                    return menu.ResumeRect;
                case TutorialActionArea.Start:
                    return menu.StartRect;
                case TutorialActionArea.Stats:
                    return menu.StatsRect;
                case TutorialActionArea.SubmitInfo:
                    return menu.SubmitInfoRect;
                case TutorialActionArea.Upgrade:
                    return menu.UpgradeRect;
                case TutorialActionArea.UsernameBox:
                    return menu.UsernameBoxRect;
                default:
                    return new Rectangle();
            }
        }

        private string ButtonForAction(TutorialAction action, TutorialActionArea highlight)
        {
            if (action.Action == TutorialTask.GameState)
            {
                switch ((GameState)action.Params)
                {
                    case GameState.PickUser:
                        if (highlight == TutorialActionArea.Start)
                        {
                            return "\"Start\"";
                        }
                        else
                        {
                            return "\"Create User\"";
                        }
                    case GameState.NewUser:
                        return "\"Create New User\"";
                    case GameState.MainMenu:
                        if (highlight == TutorialActionArea.None)
                        {
                            return "your user";
                        }
                        else if (highlight == TutorialActionArea.Back)
                        {
                            return "\"Back\"";
                        }
                        else
                        {
                            return "";
                        }
                    case GameState.SelectMission:
                        return "\"Play\"";
                    case GameState.Playing:
                        if (highlight == TutorialActionArea.Resume)
                        {
                            return "\"Resume\"";
                        }
                        else
                        {
                            return "\"0\"";
                        }
                    case GameState.Paused:
                        return "the Escape (Esc.) key";
                    case GameState.Shop:
                        return "\"Shop\"";
                    case GameState.Crafting:
                        return "\"Crafting\"";
                }
            }
            else if (action.Action == TutorialTask.Sweep)
            {
                return "the W key";
            }
            else if (action.Action == TutorialTask.ShootRock)
            {
                return "the Space key";
            }
            else if (action.Action == TutorialTask.BuyMaterial)
            {
                return "\"Buy\"";
            }
            else if (action.Action == TutorialTask.CraftItem)
            {
                return "\"Craft\"";
            }
            return null;
        }

        #endregion
    }

    public static class TutorialStages
    {
        #region Stage Lists

        public static readonly List<string> Instructions = new List<string>()
        {
            "Welcome to One Cannon, One Army!",
            "I will be your guide.",
            "Let's get started!",
            "This is the Start button.\nUse it to begin playing.",
            "And here's the Quit button.\nUse it to exit the game.",
            "This thing is the Controls button.\nIn this menu, you can customize the game's controls.",
            "This is the Language button.\nHere, you can change the game's language.",
            "Now click on the Start button to begin!",
            "Here, you can view your users and create new ones.",
            "Click \"Create New User\" to do so!",
            "On this menu, you can create a new user.",
            "Type your username here.",
            "You can change it later if needed.",
            "Use these sliders to change the color of your avatar.",
            "This is your icon.",
            "You can set your icon to any projectile by clicking the arrows below.",
            "Your icon does not affect the game; it is just cosmetic.",
            "Use the back button to cancel creating a user.",
            "After you've customized your user, click \"Create User\" to continue!",
            "Now, click on your username to sign in!",
            "Now we're on the main menu.",
            "This is the Play button, where you begin your adventure!",
            "Here's the Achievement button, where you can view all achievements.",
            "The Stats button let's you view various statistics about you and your play.",
            "The Shop button is where you can purchase materials...",
            "...and the Crafting menu let's you make projectiles out of them!",
            "Here you can upgrade your cannon.",
            "In the Organize menu you can set your hotbar and view your inventory.",
            "And in the Gifts menu, you can view and open gifts.",
            "The Options menu lets you change your name,\navatar, and icon (among other things).",
            "Now let's make some rocks. To begin, open the shop.",
            "Here in the shop, you can buy one of the eight different materials.",
            "Click on \"Buy 10\" under stone to buy ten stones.",
            "Good job! Now it's time to make some rocks and stop some evil aliens!",
            "Click on \"Crafting\" to make some rocks.",
            "Now click \"Craft\" under rock to use your stone to make a rock!",
            "Great job! You've already been given 50 rocks to start you off, so let's get moving!",
            "Now let's go back to the main menu to fight some aliens!",
            "Now click \"Play\" to begin a mission!",
            "Here, you can play missions.",
            "You cannot attempt a mission until you complete the previous one.",
            "Your goal is to beat level 25.",
            "However, you can still replay previous missions if you desire.",
            "Now, click on mission \"0\" to try out the cannon!",
            "Use Esc. to pause the game.",
            "The Resume button lets you continue your game.",
            "The Main Menu button returns you to the main menu.",
            "The Quit button will save your progress and close the game.",
            "Now click resume to play!",
            "Use the space bar to shoot a rock, and A and D to move around.\nKill the aliens to beat the level!",
            "Press \"W\" or click the button in the bottom right corner to collect all the items the aliens dropped.",
            "This clears the screen and makes it easier to see. However,\nthe game will automatically sweep at the end of the level.",
            "Now press space to finish the\n tutorial and battle the aliens! Good luck!",
        };

        public static readonly List<Vector2> AlienPositions = new List<Vector2>()
        {
            new Vector2(-1, -1),
            new Vector2(-1, -1),
            new Vector2(-1, -1),
            new Vector2(600, 600),
            new Vector2(600, 600),
            new Vector2(-1, 600),
            new Vector2(-1, 600),
            new Vector2(600, 600),
            new Vector2(-1, -1),
            new Vector2(-1, -1),
            new Vector2(10, 10),
            new Vector2(10, 10),
            new Vector2(10, 10),
            new Vector2(10, 10),
            new Vector2(10, 10),
            new Vector2(10, 10),
            new Vector2(10, 10),
            new Vector2(10, 10),
            new Vector2(10, 10),
            new Vector2(-1, 550),
            new Vector2(10, 500),
            new Vector2(10, 500),
            new Vector2(10, 500),
            new Vector2(10, 500),
            new Vector2(10, 500),
            new Vector2(10, 500),
            new Vector2(10, 500),
            new Vector2(10, 500),
            new Vector2(10, 500),
            new Vector2(10, 500),
            new Vector2(10, 500),
            new Vector2(10, 500),
            new Vector2(10, 500),
            new Vector2(10, 500),
            new Vector2(10, 500),
            new Vector2(10, 500),
            new Vector2(10, 500),
            new Vector2(10, 500),
            new Vector2(10, 500),
            new Vector2(-1, 500),
            new Vector2(-1, 500),
            new Vector2(-1, 500),
            new Vector2(-1, 500),
            new Vector2(-1, 500),
            new Vector2(-1, 700),
            new Vector2(-1, 600),
            new Vector2(-1, 600),
            new Vector2(-1, 600),
            new Vector2(-1, 600),
            new Vector2(-1, 700),
            new Vector2(-1, 700),
            new Vector2(-1, 700),
            new Vector2(-1, 700),
        };

        public static readonly List<bool> IsAlienHappy = new List<bool>()
        {
            false,
            false,
            true,
            false,
            false,
            false,
            false,
            true,
            false,
            true,
            false,
            false,
            false,
            false,
            false,
            false,
            false,
            false,
            true,
            true,
            false,
            false,
            false,
            false,
            false,
            true,
            false,
            false,
            false,
            false,
            false,
            false,
            true,
            false,
            true,
            false,
            false,
            false,
            true,
            false,
            false,
            false,
            false,
            true,
            false,
            false,
            false,
            false,
            true,
            false,
            false,
            false,
            false,
        };

        public static readonly List<Vector2> InstructionPositions = new List<Vector2>()
        {
            new Vector2(-1, 550),
            new Vector2(-1, 505),
            new Vector2(-1, 550),
            new Vector2(-1, 725),
            new Vector2(-1, 725),
            new Vector2(-1, 500),
            new Vector2(-1, 500),
            new Vector2(-1, 725),
            new Vector2(-1, 500),
            new Vector2(-1, 500),
            new Vector2(-1, 10),
            new Vector2(-1, 10),
            new Vector2(-1, 10),
            new Vector2(-1, 10),
            new Vector2(-1, 10),
            new Vector2(-1, 10),
            new Vector2(-1, 10),
            new Vector2(-1, 10),
            new Vector2(-1, 10),
            new Vector2(-1, 500),
            new Vector2(-1, 720),
            new Vector2(-1, 720),
            new Vector2(-1, 720),
            new Vector2(-1, 720),
            new Vector2(-1, 720),
            new Vector2(-1, 720),
            new Vector2(-1, 720),
            new Vector2(-1, 720),
            new Vector2(-1, 720),
            new Vector2(-1, 720),
            new Vector2(-1, 720),
            new Vector2(-1, 720),
            new Vector2(-1, 720),
            new Vector2(-1, 720),
            new Vector2(-1, 720),
            new Vector2(-1, 720),
            new Vector2(-1, 720),
            new Vector2(-1, 720),
            new Vector2(-1, 720),
            new Vector2(-1, -1),
            new Vector2(-1, -1),
            new Vector2(-1, -1),
            new Vector2(-1, -1),
            new Vector2(-1, -1),
            new Vector2(-1, 650),
            new Vector2(-1, 700),
            new Vector2(-1, 700),
            new Vector2(-1, 700),
            new Vector2(-1, 700),
            new Vector2(-1, 550),
            new Vector2(-1, 550),
            new Vector2(-1, 550),
            new Vector2(-1, 550),
        };

        public static readonly List<TutorialActionArea> Highlights = new List<TutorialActionArea>()
        {
            TutorialActionArea.None,
            TutorialActionArea.None,
            TutorialActionArea.None,
            TutorialActionArea.Start,
            TutorialActionArea.Quit,
            TutorialActionArea.Controls,
            TutorialActionArea.Language,
            TutorialActionArea.Start,
            TutorialActionArea.None,
            TutorialActionArea.CreateNewUser,
            TutorialActionArea.None,
            TutorialActionArea.UsernameBox,
            TutorialActionArea.UsernameBox,
            TutorialActionArea.ColorSliders,
            TutorialActionArea.PlayerIcon,
            TutorialActionArea.PlayerIcon,
            TutorialActionArea.PlayerIcon,
            TutorialActionArea.Back,
            TutorialActionArea.SubmitInfo,
            TutorialActionArea.None,
            TutorialActionArea.None,
            TutorialActionArea.Play,
            TutorialActionArea.Achievements,
            TutorialActionArea.Stats,
            TutorialActionArea.Shop,
            TutorialActionArea.Crafting,
            TutorialActionArea.Upgrade,
            TutorialActionArea.Organize,
            TutorialActionArea.Gifts,
            TutorialActionArea.Settings,
            TutorialActionArea.Shop,
            TutorialActionArea.None,
            TutorialActionArea.None,
            TutorialActionArea.Back,
            TutorialActionArea.Crafting,
            TutorialActionArea.None,
            TutorialActionArea.None,
            TutorialActionArea.Back,
            TutorialActionArea.Play,
            TutorialActionArea.None,
            TutorialActionArea.None,
            TutorialActionArea.None,
            TutorialActionArea.None,
            TutorialActionArea.None,
            TutorialActionArea.None,
            TutorialActionArea.Resume,
            TutorialActionArea.MainMenu,
            TutorialActionArea.Quit,
            TutorialActionArea.Resume,
            TutorialActionArea.None,
            TutorialActionArea.None,
            TutorialActionArea.None,
            TutorialActionArea.None,
        };

        public static readonly List<TutorialAction> ActionsAllowed = new List<TutorialAction>()
        {
            new TutorialAction(TutorialTask.None, -1),
            new TutorialAction(TutorialTask.None, -1),
            new TutorialAction(TutorialTask.None, -1),
            new TutorialAction(TutorialTask.None, -1),
            new TutorialAction(TutorialTask.None, -1),
            new TutorialAction(TutorialTask.None, -1),
            new TutorialAction(TutorialTask.None, -1),
            new TutorialAction(TutorialTask.GameState, (int)GameState.PickUser),
            new TutorialAction(TutorialTask.None, -1),
            new TutorialAction(TutorialTask.GameState, (int)GameState.NewUser),
            new TutorialAction(TutorialTask.None, -1),
            new TutorialAction(TutorialTask.None, -1),
            new TutorialAction(TutorialTask.None, -1),
            new TutorialAction(TutorialTask.None, -1),
            new TutorialAction(TutorialTask.None, -1),
            new TutorialAction(TutorialTask.None, -1),
            new TutorialAction(TutorialTask.None, -1),
            new TutorialAction(TutorialTask.None, -1),
            new TutorialAction(TutorialTask.GameState, (int)GameState.PickUser),
            new TutorialAction(TutorialTask.GameState, (int)GameState.MainMenu),
            new TutorialAction(TutorialTask.None, -1),
            new TutorialAction(TutorialTask.None, -1),
            new TutorialAction(TutorialTask.None, -1),
            new TutorialAction(TutorialTask.None, -1),
            new TutorialAction(TutorialTask.None, -1),
            new TutorialAction(TutorialTask.None, -1),
            new TutorialAction(TutorialTask.None, -1),
            new TutorialAction(TutorialTask.None, -1),
            new TutorialAction(TutorialTask.None, -1),
            new TutorialAction(TutorialTask.None, -1),
            new TutorialAction(TutorialTask.GameState, (int)GameState.Shop),
            new TutorialAction(TutorialTask.None, -1),
            new TutorialAction(TutorialTask.BuyMaterial, -1),
            new TutorialAction(TutorialTask.GameState, (int)GameState.MainMenu),
            new TutorialAction(TutorialTask.GameState, (int)GameState.Crafting),
            new TutorialAction(TutorialTask.CraftItem, -1),
            new TutorialAction(TutorialTask.None, -1),
            new TutorialAction(TutorialTask.GameState, (int)GameState.MainMenu),
            new TutorialAction(TutorialTask.GameState, (int)GameState.SelectMission),
            new TutorialAction(TutorialTask.None, -1),
            new TutorialAction(TutorialTask.None, -1),
            new TutorialAction(TutorialTask.None, -1),
            new TutorialAction(TutorialTask.None, -1),
            new TutorialAction(TutorialTask.GameState, (int)GameState.Playing),
            new TutorialAction(TutorialTask.GameState, (int)GameState.Paused),
            new TutorialAction(TutorialTask.None, -1),
            new TutorialAction(TutorialTask.None, -1),
            new TutorialAction(TutorialTask.None, -1),
            new TutorialAction(TutorialTask.GameState, (int)GameState.Playing),
            new TutorialAction(TutorialTask.ShootRock, -1),
            new TutorialAction(TutorialTask.Sweep, -1),
            new TutorialAction(TutorialTask.None, -1),
            new TutorialAction(TutorialTask.None, -1),
        };

        #endregion
    }

    public struct TutorialAction
    {
        public TutorialTask Action;
        public int Params;
        public TutorialAction(TutorialTask action, int parameters)
        {
            Action = action;
            Params = parameters;
        }
    }
    public enum TutorialActionArea
    {
        None,
        Start,
        Quit,
        Play,
        Achievements,
        Stats,
        Shop,
        Crafting,
        Upgrade,
        Organize,
        Back,
        CreateNewUser,
        SubmitInfo,
        PlayerIcon,
        ColorSliders,
        Language,
        Controls,
        Settings,
        UsernameBox,
        Resume,
        MainMenu,
        Gifts,
    }
    public enum TutorialTask
    {
        All,
        None,
        GameState,
        ShootRock,
        Sweep,
        BuyMaterial,
        CraftItem,
    }
}