using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OneCannonOneArmy
{
    public class AlienLaserCannon
    {
        #region Fields & Properties

        Texture2D img;
        Rectangle drawRect;

        public int X
        {
            get
            {
                return drawRect.X;
            }
        }
        public int Y
        {
            get
            {
                return drawRect.Y;
            }
        }

        Timer fireTimer;
        Timer callFailTimer;
        Timer playNextSoundTimer;

        Texture2D laserImg;
        Rectangle laserRect;

        public int Health;
        FillBar healthBar;
        const int HEALTH_BAR_WIDTH = 50;
        const int HEALTH_BAR_HEIGHT = 10;

        public bool Active = false;

        const int OFFSET = 2;
        bool shakeDir;
        int xOffset;
        int yOffset;

        public const int INIT_HEALTH = 150;
        const int LASER_WIDTH = 15;
        const int LASER_SPD = 30;

        public bool Firing = false;

        const int SLIDE_SPD = 4;
        bool slidingIn = false;
        int topOffset;

        event System.Action onDeath;
        event System.Action onLaserHit;

        int windowHeight;

        const int SPACING = 10;

        int seqIndex = 0;
        List<Direction> travelSequence = new List<Direction>()
        {
            Direction.Right, Direction.Right,
            Direction.Up, Direction.Up,
            Direction.Left, Direction.Left,
            Direction.Down, Direction.Down,
        };

        #endregion

        #region Constructors

        public AlienLaserCannon(Texture2D img, int x, int y, int width, int height)
        {
            this.img = img;
            drawRect = new Rectangle(x, y, width, height);
            fireTimer = new Timer(9.0f, TimerUnits.Seconds);
            Health = INIT_HEALTH;
            laserImg = Utilities.LaserImg;
            laserRect = new Rectangle(0, 0, LASER_WIDTH, 0);
            healthBar = new FillBar(Health, 0, 0, HEALTH_BAR_WIDTH, HEALTH_BAR_HEIGHT, INIT_HEALTH, null);

            callFailTimer = new Timer(2, TimerUnits.Seconds);
            playNextSoundTimer = new Timer(5, TimerUnits.Seconds);
        }

        #endregion

        #region Public Methods

        public void Spawn(int windowWidth, int windowHeight, int topOffset)
        {
            this.windowHeight = windowHeight;
            this.topOffset = topOffset;
            drawRect.X = windowWidth / 2 - (drawRect.Width / 2);
            laserRect.X = drawRect.X + (drawRect.Width / 2 - (laserRect.Width / 2)) + OFFSET * 4;
            laserRect.Y = drawRect.Bottom;
            healthBar.X = drawRect.X + drawRect.Width / 2 - (healthBar.Width / 2);
            healthBar.Y = drawRect.Y;
            Active = true;
            slidingIn = true;
        }

        public void Update(GameTime gameTime, ref List<Projectile> projectiles)
        {
            if (slidingIn)
            {
                if (drawRect.Y < topOffset)
                {
                    drawRect.Y += SLIDE_SPD;
                    healthBar.Y += SLIDE_SPD;
                }
                else
                {
                    Sound.PlaySound(Sounds.MetalShaking);
                    slidingIn = false;
                }
            }
            else
            {
                if (Firing && laserRect.Height > windowHeight)
                {
                    callFailTimer.Update(gameTime);
                    if (callFailTimer.QueryWaitTime(gameTime))
                    {
                        onLaserHit?.Invoke();
                    }
                }

                playNextSoundTimer.Update(gameTime);
                if (playNextSoundTimer.QueryWaitTime(gameTime))
                {
                    Sound.PlaySound(Sounds.LaserCharge);
                }

                fireTimer.Update(gameTime);

                if (fireTimer.QueryWaitTime(gameTime))
                {
                    Firing = true;
                    Sound.PlaySound(Sounds.Laser);
                    Sound.PlaySound(Sounds.LaserSustained);
                }
                if (Firing && !(laserRect.Height > windowHeight))
                {
                    laserRect.Height += LASER_SPD;
                    xOffset = yOffset = 0;
                }
                else if (!Firing)
                {
                    #region Previous Animation Code
                    // Now we can check for and handle moving
                    //if (shakeDir)
                    //{
                    //    xOffset -= OFFSET;
                    //    if (xOffset < -MAX_OFFSET)
                    //    {
                    //        xOffset = -MAX_OFFSET;
                    //        shakeDir = !shakeDir;
                    //    }
                    //}
                    //else
                    //{
                    //    xOffset += OFFSET;
                    //    if (xOffset > MAX_OFFSET)
                    //    {
                    //        xOffset = MAX_OFFSET;
                    //        shakeDir = !shakeDir;
                    //    }
                    //}
                    //if (Utilities.Rand.Next(2) == 0)
                    //{
                    //    yOffset = xOffset;
                    //}
                    //else
                    //{
                    //    yOffset = -xOffset;
                    //}
                    #endregion

                    switch (travelSequence[seqIndex])
                    {
                        case Direction.Up:
                            yOffset -= OFFSET;
                            break;
                        case Direction.Down:
                            yOffset += OFFSET;
                            break;
                        case Direction.Left:
                            xOffset += OFFSET;
                            break;
                        case Direction.Right:
                            xOffset -= OFFSET;
                            break;
                    }
                    if (seqIndex + 1 < travelSequence.Count)
                    {
                        seqIndex++;
                    }
                    else
                    {
                        seqIndex = 0;
                    }
                }

                // Handle collisions
                for (int i = 0; i < projectiles.Count; i++)
                {
                    if (projectiles[i].Intersects(drawRect) && Health > 0 &&
                        projectiles[i].Flying)
                    {
                        // We've been hit by a flying projectile
                        Health -= projectiles[i].Damage;
                        healthBar.Value = Health;

                        if (projectiles[i].DestroyOnHit)
                        {
                            projectiles[i].Active = false;
                            projectiles.RemoveAt(i);
                        }
                        else
                        {
                            projectiles[i].OnHit();
                        }
                    }
                }

                if (Health <= 0)
                {
                    Active = false;
                    onDeath?.Invoke();
                }
            }
        }

        public void Draw(SpriteBatch spriteBatch)
        {
            if (Firing)
            {
                spriteBatch.Draw(laserImg, laserRect, Color.DarkRed);
            }
            spriteBatch.Draw(img, new Rectangle(drawRect.X + xOffset, drawRect.Y + yOffset, drawRect.Width, drawRect.Height),
                Color.White);
            healthBar.Draw(spriteBatch, GameInfo.GetColorForAmount(Health, INIT_HEALTH), Color.White);
        }

        public void Reset()
        {
            Firing = false;
            Active = false;
            Health = INIT_HEALTH;
            healthBar.Value = INIT_HEALTH;
            laserRect.Height = 0;
        }

        public void AddOnDeathHandler(System.Action handler)
        {
            onDeath += handler;
        }
        public void AddLaserHitHandler(System.Action handler)
        {
            onLaserHit += handler;
        }

        #endregion
    }

    public class Cage
    {
        #region Fields & Properties

        Texture2D img;
        Rectangle drawRect;

        public int X
        {
            get
            {
                return drawRect.X;
            }
            set
            {
                drawRect.X = value;
            }
        }
        public int Y
        {
            get
            {
                return drawRect.Y;
            }
            set
            {
                drawRect.Y = value;
            }
        }
        public int Width
        {
            get
            {
                return drawRect.Width;
            }
        }
        public int Height
        {
            get
            {
                return drawRect.Height;
            }
        }

        public float Health = INIT_HEALTH;
        public const float INIT_HEALTH = 100;
        FillBar healthBar;
        const int SPACING = 8;
        const int HEALTHBAR_HEIGHT = 10;

        public bool Active = true;

        event Action<Cage> onDeath;

        #endregion

        #region Constructors

        public Cage(int x, int y, int width, int height)
        {
            img = Utilities.CageImg;
            drawRect = new Rectangle(x, y, width, height);
            healthBar = new FillBar((int)Health, x + SPACING / 2, y, width - SPACING, HEALTHBAR_HEIGHT, INIT_HEALTH, null);
        }

        #endregion

        #region Public Methods

        public void Update(ref List<Projectile> projectiles)
        {
            for (int i = 0; i < projectiles.Count; i++)
            {
                if (projectiles[i].Intersects(drawRect) && Health > 0 &&
                    projectiles[i].Flying)
                {
                    // We've been hit by a flying projectile
                    Health -= projectiles[i].Damage;
                    healthBar.Value = Health;
                    Sound.PlaySound(Sounds.CageHit);

                    if (projectiles[i].DestroyOnHit)
                    {
                        projectiles[i].Active = false;
                        projectiles.RemoveAt(i);
                    }
                    else
                    {
                        projectiles[i].OnHit();
                    }
                }
            }

            if (Health <= 0)
            {
                onDeath?.Invoke(this);
            }
        }

        public void Draw(SpriteBatch spriteBatch)
        {
            spriteBatch.Draw(img, drawRect, Color.White);
            healthBar.Draw(spriteBatch, Color.Lime, Color.Transparent);
        }

        public void AddOnDeathHandler(Action<Cage> handler)
        {
            onDeath += handler;
        }

        #endregion
    }
}
