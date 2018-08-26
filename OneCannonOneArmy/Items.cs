using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace OneCannonOneArmy
{
    public delegate void OnItemCollect(Item item);

    public class Item
    {
        protected Texture2D img;
        protected Rectangle drawRect;

        event OnItemCollect onCollect;

        public bool Flying = false;
        public Vector2 FlyingTo = new Vector2();

        public bool Active = true;

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
            set
            {
                drawRect.Width = value;
            }
        }
        public int Height
        {
            get
            {
                return drawRect.Height;
            }
            set
            {
                drawRect.Height = value;
            }
        }

        public Rectangle Rectangle
        {
            get
            {
                return drawRect;
            }
        }

        public Item(Texture2D img, int x, int y, int width, int height)
        {
            this.img = img;
            drawRect = new Rectangle(x, y, width, height);
        }

        public virtual void Update(GameTime gameTime)
        {
            if (Active)
            {
                if (Flying)
                {
                    if (!(drawRect.X == FlyingTo.X && drawRect.Y == FlyingTo.Y))
                    {
                        Vector2 pos = new Vector2(drawRect.X, drawRect.Y);
                        Vector2 distance = FlyingTo - pos;

                        distance.Normalize();
                        pos += distance * (float)gameTime.ElapsedGameTime.TotalMilliseconds * GameInfo.COIN_SPD;

                        drawRect.X = (int)pos.X;
                        drawRect.Y = (int)pos.Y;
                    }
                    else
                    {
                        Active = false;
                    }
                }
            }
        }

        public virtual void Draw(SpriteBatch spriteBatch)
        {
            spriteBatch.Draw(img, drawRect, Color.White);
        }

        public void AddOnCollectHandler(OnItemCollect handler)
        {
            onCollect += handler;
        }

        public void MoveX(int value)
        {
            drawRect.X += value;
        }
        public void MoveY(int value)
        {
            drawRect.Y += value;
        }

        public void Collect()
        {
            onCollect?.Invoke(this);
        }

        public void StartFlying(Point point)
        {
            FlyingTo = new Vector2(point.X, point.Y);
            Flying = true;
        }
    }

    public class Coin : Item
    {
        #region Fields & Properties
        
        Rectangle sourceRect;
        Timer timer;
        int frame = 0;
        const int FRAMES = 6;
        const int FRAME_SIZE = 50;

        public int Worth = 0;

        #endregion

        #region Constructors

        public Coin(Texture2D img, int x, int y, int width, int height, int worth)
            : base(img, x, y, width, height)
        {
            sourceRect = new Rectangle(0, 0, FRAME_SIZE, FRAME_SIZE);

            Worth = worth;

            timer = new Timer(150, TimerUnits.Milliseconds);
        }

        #endregion

        #region Public Methods

        public override void Update(GameTime gameTime)
        {
            timer.Update(gameTime);

            if (timer.QueryWaitTime(gameTime))
            {
                if (frame + 1 < FRAMES)
                {
                    frame++;
                    sourceRect.X += FRAME_SIZE;
                }
                else
                {
                    frame = 0;
                    sourceRect.X = 0;
                }
            }

            base.Update(gameTime);
        }
        public override void Draw(SpriteBatch spriteBatch)
        {
            spriteBatch.Draw(img, drawRect, sourceRect, Color.White);
        }

        #endregion
    }

    public class MaterialDrop : Item
    {
        public Material Drop;

        public MaterialDrop(Material material, int x, int y, int width, int height)
            : base(Utilities.GetImgOfMaterial(material), x, y, width, height)
        {
            Drop = material;
        }
    }

    public class BadgeDrop : Item
    {
        public Badge Badge;

        public BadgeDrop(Badge badge, int x, int y, int width, int height)
            : base(Utilities.GetImgOfBadge(badge), x, y, width, height)
        {
            Badge = badge;
        }
    }
    public enum Badge
    {
        Red,
        Orange,
        Yellow,
        Green,
        Blue,
        Purple,
    }
}
