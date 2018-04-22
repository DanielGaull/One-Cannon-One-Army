using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Audio;

namespace OneCannonOneArmy
{
    /// <summary>
    /// Used to play the opening animation of Purasu
    /// </summary>
    public class StudioIntro
    {
        #region Fields

        List<Texture2D> imgs = new List<Texture2D>();
        List<Rectangle> rects = new List<Rectangle>();

        Texture2D outlineImg;
        Texture2D orangeImg;
        Rectangle purasuRect;

        Texture2D bgImg;

        float rectSize = 750.0f;
        const float C = -0.57f;
        const float M = 11.4f;
        const int INIT_PURASURECT_SIZE = 7175;

        public bool Playing = false;

        int windowWidth;
        int windowHeight;

        float alpha = 0.0f;
        bool showOrangeImg = false;

        const int END_TIME = 5;
        Timer timer;
        bool started = false;
        bool waitEnd = false;

        string introCue;

        event System.Action onFinish;

        bool playFull = true;
        bool fadingIn = false;

        #region Possibilities
        readonly List<string> possibleCodeLines = new List<string>()
        {
            "string test = \"\";",
            "Console.WriteLine(\"hello world\");",
            "(((int)(System.Math.Pow(2, i)) & int.Parse(foo[0])) != 0",
            "System.Linq.Enumerable.ToArray(System.Linq.Enumerable.Select(foo,",
            "int a = Convert.ToInt16(Console.ReadLine());",
            "int a, b;",
            "Img bgImg;",
            "foreach (string item in args)\n{\n    message += item;\n}",
            "if (args.Length < 1)\n{\n    message = \"Welcome!\";\n}",
            "System.IO.StreamReader objReader = new System.IO.StreamReader(FILE_NAME);",
            "tbox1.Text = textLine;",
            "objReader.Close();",
            "storageAccount.AccountType = AccountType.StandardLRS;",
            "// Add a storage provider",
            "Color c = new Color(0, 0, 255);",
            "001010101010101010101010010110010",
            "101001010000101010101100101010101",
            "using Microsoft.Xna.Framwork;",
        };
        readonly List<Color> possibleCodeColors = new List<Color>()
        {
            Color.Blue,
            Color.DarkRed,
            Color.Lime,
            Color.Cyan,
            Color.Yellow,
            new Color(214, 157, 133),
        };
        #endregion

        List<string> codeLines = new List<string>();
        List<Vector2> codeLocs = new List<Vector2>();
        List<Color> codeColors = new List<Color>();
        Timer codeTimer = new Timer(0.05f, TimerUnits.Seconds);
        SpriteFont font;
        const int CODE_SPD = 60;

        #endregion

        #region Constructors

        public StudioIntro(List<Texture2D> gameImgs, Texture2D outlineImg, Texture2D orangeImg, int windowWidth, int windowHeight, SpriteFont font)
        {
            this.font = font;

            this.windowWidth = windowWidth;
            this.windowHeight = windowHeight;
            imgs = gameImgs;

            this.outlineImg = outlineImg;
            this.orangeImg = orangeImg;

            this.introCue = introCue;

            playFull = !(new UniversalSettings().ViewedFullIntro);

            if (playFull)
            {
                purasuRect = new Rectangle(windowWidth / 2 - (INIT_PURASURECT_SIZE / 2), windowHeight / 2 - (INIT_PURASURECT_SIZE / 2),
                    INIT_PURASURECT_SIZE, INIT_PURASURECT_SIZE);

                rects = new List<Rectangle>()
                    {
                        new Rectangle((int)(rectSize * 0), 0, (int)rectSize, (int)rectSize),
                        new Rectangle((int)(rectSize * 1), 0, (int)rectSize, (int)rectSize),
                        new Rectangle((int)(rectSize * 2), 0, (int)rectSize, (int)rectSize),
                        new Rectangle((int)(rectSize * 3), 0, (int)rectSize, (int)rectSize),

                        new Rectangle((int)(rectSize * 0), (int)rectSize, (int)rectSize, (int)rectSize),
                        new Rectangle((int)(rectSize * 1), (int)rectSize, (int)rectSize, (int)rectSize),
                        new Rectangle((int)(rectSize * 2), (int)rectSize, (int)rectSize, (int)rectSize),
                        new Rectangle((int)(rectSize * 3), (int)rectSize, (int)rectSize, (int)rectSize),

                        new Rectangle((int)(rectSize * 0), (int)(rectSize * 2), (int)rectSize, (int)rectSize),
                        new Rectangle((int)(rectSize * 1), (int)(rectSize * 2), (int)rectSize, (int)rectSize),
                        new Rectangle((int)(rectSize * 2), (int)(rectSize * 2), (int)rectSize, (int)rectSize),
                        new Rectangle((int)(rectSize * 3), (int)(rectSize * 2), (int)rectSize, (int)rectSize),

                        new Rectangle((int)(rectSize * 0), (int)(rectSize * 3), (int)rectSize, (int)rectSize),
                        new Rectangle((int)(rectSize * 1), (int)(rectSize * 3), (int)rectSize, (int)rectSize),
                        new Rectangle((int)(rectSize * 2), (int)(rectSize * 3), (int)rectSize, (int)rectSize),
                        new Rectangle((int)(rectSize * 3), (int)(rectSize * 3), (int)rectSize, (int)rectSize),

                        new Rectangle((int)(rectSize * 0), (int)(rectSize * 4), (int)rectSize, (int)rectSize),
                        new Rectangle((int)(rectSize * 1), (int)(rectSize * 4), (int)rectSize, (int)rectSize),
                        new Rectangle((int)(rectSize * 2), (int)(rectSize * 4), (int)rectSize, (int)rectSize),
                        new Rectangle((int)(rectSize * 3), (int)(rectSize * 4), (int)rectSize, (int)rectSize),

                        new Rectangle((int)(rectSize * 0), (int)(rectSize * 5), (int)rectSize, (int)rectSize),
                        new Rectangle((int)(rectSize * 1), (int)(rectSize * 5), (int)rectSize, (int)rectSize),
                        new Rectangle((int)(rectSize * 2), (int)(rectSize * 5), (int)rectSize, (int)rectSize),
                        new Rectangle((int)(rectSize * 3), (int)(rectSize * 5), (int)rectSize, (int)rectSize),
                    };
            }
            else
            {
                purasuRect = new Rectangle(0, 0, windowWidth, windowHeight);
                bgImg = Utilities.RectImage;
            }

            timer = new Timer(0.5f, TimerUnits.Seconds);
        }

        #endregion

        #region Public Methods

        public void Update(GameTime gameTime)
        {
            if (Playing)
            {
                if (!started || waitEnd)
                {
                    timer.Update(gameTime);
                }
                if (playFull)
                {
                    codeTimer.Update(gameTime);
                    if (codeTimer.QueryWaitTime(gameTime))
                    {
                        codeLines.Add(possibleCodeLines.Random());
                        codeColors.Add(possibleCodeColors.Random());
                        codeLocs.Add(new Vector2(0 - font.MeasureString(codeLines[codeLines.Count - 1]).X,
                            Utilities.Rand.Next(windowHeight)));
                    }
                    for (int i = 0; i < codeLocs.Count; i++)
                    {
                        codeLocs[i] = new Vector2(codeLocs[i].X + CODE_SPD, codeLocs[i].Y);
                        if (codeLocs[i].X >= windowWidth)
                        {
                            codeLocs.RemoveAt(i);
                            codeLines.RemoveAt(i);
                            codeColors.RemoveAt(i);
                        }
                    }

                    if (purasuRect.Width >= windowWidth && started)
                    {
                        rectSize += C;
                        purasuRect.Width += (int)(C * M);
                        purasuRect.Height += (int)(C * M);

                        purasuRect.X = windowWidth / 2 - (purasuRect.Width / 2);
                        purasuRect.Y = windowHeight / 2 - (purasuRect.Height / 2);

                        rects = new List<Rectangle>()
                    {
                        new Rectangle((int)(rectSize * 0), 0, (int)rectSize, (int)rectSize),
                        new Rectangle((int)(rectSize * 1), 0, (int)rectSize, (int)rectSize),
                        new Rectangle((int)(rectSize * 2), 0, (int)rectSize, (int)rectSize),
                        new Rectangle((int)(rectSize * 3), 0, (int)rectSize, (int)rectSize),

                        new Rectangle((int)(rectSize * 0), (int)rectSize, (int)rectSize, (int)rectSize),
                        new Rectangle((int)(rectSize * 1), (int)rectSize, (int)rectSize, (int)rectSize),
                        new Rectangle((int)(rectSize * 2), (int)rectSize, (int)rectSize, (int)rectSize),
                        new Rectangle((int)(rectSize * 3), (int)rectSize, (int)rectSize, (int)rectSize),

                        new Rectangle((int)(rectSize * 0), (int)(rectSize * 2), (int)rectSize, (int)rectSize),
                        new Rectangle((int)(rectSize * 1), (int)(rectSize * 2), (int)rectSize, (int)rectSize),
                        new Rectangle((int)(rectSize * 2), (int)(rectSize * 2), (int)rectSize, (int)rectSize),
                        new Rectangle((int)(rectSize * 3), (int)(rectSize * 2), (int)rectSize, (int)rectSize),

                        new Rectangle((int)(rectSize * 0), (int)(rectSize * 3), (int)rectSize, (int)rectSize),
                        new Rectangle((int)(rectSize * 1), (int)(rectSize * 3), (int)rectSize, (int)rectSize),
                        new Rectangle((int)(rectSize * 2), (int)(rectSize * 3), (int)rectSize, (int)rectSize),
                        new Rectangle((int)(rectSize * 3), (int)(rectSize * 3), (int)rectSize, (int)rectSize),

                        new Rectangle((int)(rectSize * 0), (int)(rectSize * 4), (int)rectSize, (int)rectSize),
                        new Rectangle((int)(rectSize * 1), (int)(rectSize * 4), (int)rectSize, (int)rectSize),
                        new Rectangle((int)(rectSize * 2), (int)(rectSize * 4), (int)rectSize, (int)rectSize),
                        new Rectangle((int)(rectSize * 3), (int)(rectSize * 4), (int)rectSize, (int)rectSize),

                        new Rectangle((int)(rectSize * 0), (int)(rectSize * 5), (int)rectSize, (int)rectSize),
                        new Rectangle((int)(rectSize * 1), (int)(rectSize * 5), (int)rectSize, (int)rectSize),
                        new Rectangle((int)(rectSize * 2), (int)(rectSize * 5), (int)rectSize, (int)rectSize),
                        new Rectangle((int)(rectSize * 3), (int)(rectSize * 5), (int)rectSize, (int)rectSize),
                    };
                    }

                    if (alpha >= 1.0f)
                    {
                        waitEnd = true;
                        timer.WaitTime = END_TIME;
                    }
                    if (showOrangeImg && alpha < 1.0f)
                    {
                        alpha += 0.005f;
                    }
                    if (purasuRect.Width <= windowWidth)
                    {
                        showOrangeImg = true;
                    }

                    if (!started)
                    {
                        if (timer.QueryWaitTime(gameTime))
                        {
                            Sound.PlaySound(Sounds.Intro);
                            started = true;
                            timer.WaitTime = 4.0f;
                        }
                    }
                    if (waitEnd)
                    {
                        if (timer.QueryWaitTime(gameTime))
                        {
                            waitEnd = false;
                            Playing = false;
                            onFinish?.Invoke();
                        }
                    }
                }
                else
                {
                    // Just fade in and out
                    if (fadingIn)
                    {
                        if (alpha < 1.0f)
                        {
                            alpha += 0.005f;
                        }
                        else
                        {
                            fadingIn = false;
                        }
                    }
                    else
                    {
                        if (alpha > 0.0f)
                        {
                            alpha -= 0.005f;
                        }
                        else
                        {
                            Stop();
                        }
                    }
                }
            }
        }
        public void Draw(SpriteBatch spriteBatch)
        {
            if (playFull)
            {
                for (int i = 0; i < imgs.Count; i++)
                {
                    spriteBatch.Draw(imgs[i], rects[i], Color.White);
                }

                for (int i = 0; i < codeLines.Count; i++)
                {
                    spriteBatch.DrawString(font, codeLines[i], codeLocs[i], codeColors[i]);
                }

                spriteBatch.Draw(outlineImg, purasuRect, Color.White);
            }

            if (!playFull)
            {
                spriteBatch.Draw(bgImg, purasuRect, Color.Black);
            }

            if (showOrangeImg || !playFull)
            {
                spriteBatch.Draw(orangeImg, purasuRect, Color.White * alpha);
            }
        }

        public void Play()
        {
            Playing = true;
            started = false;
            if (!playFull)
            {
                fadingIn = true;
                alpha = 0.0f;
            }
        }
        public void Stop()
        {
            Playing = false;
            started = false;
            waitEnd = false;
            onFinish?.Invoke();
        }

        public void AddOnFinishHandler(System.Action handler)
        {
            onFinish += handler;
        }

        #endregion
    }
}
