using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace OneCannonOneArmy
{
    [Serializable]
    public abstract class Projectile
    {
        #region Fields & Properties

        protected Texture2D image;
        protected Rectangle drawRect;

        public Sounds SoundWhenFired = Sounds.CannonFire;

        public ProjectileType Type;

        public int Damage;
        public bool Freezes;
        public int HealingPower;

        public int Speed = 5;
        public bool Flying = false;
        public bool Launched = false;
        
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

        public List<StatusEffect> Effects = new List<StatusEffect>();

        public bool DestroyOnHit = true;

        public bool Active = true;

        public Rectangle Rectangle
        {
            get
            {
                return drawRect;
            }
        }

        #endregion

        #region Constructors

        public Projectile(Texture2D image, int x, int y, int width, int height, int damage)
        {
            this.image = image;
            Damage = damage;
            drawRect = new Rectangle(x, y, width, height);
            DestroyOnHit = true;
        }

        #endregion

        #region Public Methods

        public virtual void Update(GameTime gameTime)
        {
            if (Flying)
            {
                drawRect.Y -= Speed;
            }
        }

        public virtual void Draw(SpriteBatch spriteBatch, bool withOrigin)
        {
            Vector2 origin = new Vector2();
            if (withOrigin)
            {
                origin = new Vector2(image.Width / 2, image.Height / 2);
            }
            spriteBatch.Draw(image, drawRect, null, Color.White, 0.0f, origin, SpriteEffects.None, 1.0f);
        }

        public virtual void Launch()
        {
            Flying = true;
            Launched = true;
        }

        public virtual bool Intersects(Rectangle rect)
        {
            Rectangle collisionRect = new Rectangle(drawRect.X - (drawRect.Width / 2), drawRect.Y - (drawRect.Height / 2),
                drawRect.Width, drawRect.Height);
            return collisionRect.Intersects(rect);
        }

        public virtual void Move(int pixels, Direction dir)
        {
            switch (dir)
            {
                case Direction.Up:
                    drawRect.Y -= pixels;
                    break;
                case Direction.Down:
                    drawRect.Y += pixels;
                    break;
                case Direction.Left:
                    drawRect.X -= pixels;
                    break;
                case Direction.Right:
                    drawRect.X += pixels;
                    break;
            }
        }

        public virtual void SetPosition(int x, int y)
        {
            drawRect.X = x;
            drawRect.Y = y;
        }

        public virtual void OnHit()
        {
        }

        #endregion
    }

    public delegate void OnExplosion(Vector2 location, float damage, float radius);
    public abstract class ExplosiveProjectile : Projectile
    {
        #region Fields & Properties

        Animation explosion;

        public bool Exploding = false;

        const int ROW_SIZE = 65;
        const int COLUMN_SIZE = 64;
        const int FRAMES_PER_ROW = 5;
        const int ROWS = 5;

        event OnExplosion onExploding;
        protected float areaDamage;

        #endregion

        #region Constructors

        public ExplosiveProjectile(Texture2D img, int x, int y, int width, int height, int damage)
            : base(img, x, y, width, height, damage)
        {
            explosion = new Animation(Utilities.ExplosionSheet, ROWS, FRAMES_PER_ROW, 2, COLUMN_SIZE, ROW_SIZE,
               () => Active = Exploding = false);
            DestroyOnHit = false;

            areaDamage = GameInfo.AREA_DMG_MULTIPLIER * damage;

            AddExplosionHandler(Utilities.OnExplosion);
        }

        #endregion

        #region Public Methods

        public override void Update(GameTime gameTime)
        {
            if (Exploding)
            {
                explosion.Update(gameTime);
            }

            base.Update(gameTime);
        }

        public override void Draw(SpriteBatch spriteBatch, bool withOrigin)
        {
            if (!Exploding)
            {
                base.Draw(spriteBatch, withOrigin);
            }
            else
            {
                explosion.Draw(spriteBatch, drawRect, Color.White);
            }
        }

        public void AddExplosionHandler(OnExplosion handler)
        {
            onExploding += handler;
        }

        public override void OnHit()
        {
            Exploding = true;
            Flying = false;
            explosion.Play();
            onExploding?.Invoke(new Vector2(drawRect.Center.X, drawRect.Center.Y), areaDamage, GameInfo.AREA_OF_EFFECT);
            Sound.PlaySound(Sounds.Explosion);
            drawRect.X -= drawRect.Width / 2;
            drawRect.Y -= drawRect.Height / 2;
        }

        #endregion
    }

    public abstract class SpinningProjectile : Projectile
    {
        #region Fields & Properties

        float rotation = 0.0f;
        public float RotationSpeed = 0.05f; // 1.0 -> flying; 0.05 -> in-cannon

        #endregion

        #region Constructors

        public SpinningProjectile(Texture2D img, int x, int y, int width, int height, int damage)
            : base(img, x, y, width, height, damage)
        {
        }

        #endregion

        #region Public Methods

        public override void Update(GameTime gameTime)
        {
            if (Flying)
            {
                RotationSpeed = 1.0f;
            }
            else
            {
                RotationSpeed = 0.05f;
            }
            rotation += RotationSpeed;
            base.Update(gameTime);
        }
        public override void Draw(SpriteBatch spriteBatch, bool withOrigin)
        {
            Vector2 origin = new Vector2();
            if (withOrigin)
            {
                origin.X = image.Width / 2;
                origin.Y = image.Height / 2;
            }
            spriteBatch.Draw(image, drawRect, null, Color.White, rotation, origin, SpriteEffects.None, 1.0f);
        }

        #endregion
    }

    public class Rock : SpinningProjectile
    {
        public Rock(int x, int y, int width, int height)
            : base(Utilities.RockImg, x, y, width, height, GameInfo.ROCK_DMG)
        {
            Speed = GameInfo.ROCK_SPD;
            Type = ProjectileType.Rock;
        }
    }

    public class Cannonball : Projectile
    {
        #region Constructors

        public Cannonball(int x, int y, int width, int height)
            : base(Utilities.CannonballImg, x, y, width, height, GameInfo.CANNONBALL_DMG)
        {
            Speed = GameInfo.CANNONBALL_SPD;
            Type = ProjectileType.Cannonball;
        }

        #endregion
    }

    public class Fireball : Projectile
    {
        int row = 0;
        int column = 0;
        Rectangle sourceRect;
        const int FRAME_SIZE = 100;
        const int ROWS = 2;
        const int COLUMNS = 5;
        Timer timer;

        public Fireball(int x, int y, int width, int height)
            : base(Utilities.FireballImg, x, y, width, height, GameInfo.FIREBALL_DMG)
        {
            Speed = GameInfo.FIREBALL_SPD;
            Effects.Add(StatusEffect.Fire);
            sourceRect = new Rectangle(0, 0, FRAME_SIZE, FRAME_SIZE);
            timer = new Timer(50, TimerUnits.Milliseconds);
            Type = ProjectileType.Fireball;
            SoundWhenFired = Sounds.SpellLaunch;
        }

        public override void Update(GameTime gameTime)
        {
            timer.Update(gameTime);

            if (timer.QueryWaitTime(gameTime))
            {
                if (column < COLUMNS - 1)
                {
                    // We are NOT on the last frame of the row
                    sourceRect.X += FRAME_SIZE;
                    column++;
                }
                else
                {
                    // We are on the last frame of the row
                    if (row >= ROWS - 1 && column >= COLUMNS - 1)
                    {
                        // We are displaying the last image of the sprite sheet,
                        // and must loop back to frame 1
                        row = 0;
                        column = 0;
                        sourceRect.X = 0;
                        sourceRect.Y = 0;
                    }
                    else
                    {
                        // Go to the first image of the next row
                        column = 0;
                        row++;
                        sourceRect.X = 0;
                        sourceRect.Y += FRAME_SIZE;
                    }
                }
            }

            base.Update(gameTime);
        }

        public override void Draw(SpriteBatch spriteBatch, bool withOrigin)
        {
            Vector2 origin = new Vector2();
            if (withOrigin)
            {
                origin = new Vector2(FRAME_SIZE / 2);
            }
            spriteBatch.Draw(image, drawRect, sourceRect, Color.White, 0.0f, origin, SpriteEffects.None, 0.0f);
        }
    }

    public class Bomb : ExplosiveProjectile
    {
        public Bomb(int x, int y, int width, int height)
            : base(Utilities.BombImg, x, y, width, height, GameInfo.BOMB_DMG)
        {
            Speed = GameInfo.BOMB_SPD;
            Type = ProjectileType.Bomb;
        }
    }

    public class Dart : Projectile
    {
        public Dart(int x, int y, int width, int height)
            : base(Utilities.DartImg, x, y, width, height, GameInfo.DART_DMG)
        {
            Speed = GameInfo.DART_SPD;
            Type = ProjectileType.Dart;
            SoundWhenFired = Sounds.Whoosh;
        }
    }
    public class PoisonDart : Dart
    {
        public PoisonDart(int x, int y, int width, int height)
            : base(x, y, width, height)
        {
            Effects.Add(StatusEffect.Poison);
            image = Utilities.PoisonDartImg;
            Type = ProjectileType.PoisonDart;
        }
    }

    public class Laser : Projectile
    {
        public Laser(int x, int y, int width, int height)
            : base(Utilities.LaserImg, x, y, width, height, GameInfo.LASER_DMG)
        {
            Speed = GameInfo.LASER_SPD;
            Effects.Add(StatusEffect.Plasma);
            Type = ProjectileType.Laser;
            SoundWhenFired = Sounds.Laser;
        }
    }

    public class LightningBolt : Projectile
    {
        #region Fields

        Animation lightning;
        Texture2D batteryImg;

        #endregion

        #region Constructors

        public LightningBolt(int x, int y, int width, int height)
            : base(Utilities.LightningBoltImg, x, y, width, height, GameInfo.LIGHTNING_DMG)
        {
            batteryImg = Utilities.BatteryImg;
            lightning = new Animation(Utilities.LightningBoltImg, Utilities.LIGHTNING_ROWS, Utilities.LIGHTNING_FRAMES_PER_ROW,
                Utilities.LIGHTNING_TIME, Utilities.LIGHTNING_FRAME_WIDTH, Utilities.LIGHTNING_FRAME_HEIGHT, null);
            Speed = GameInfo.LIGHTNING_SPD;
            Effects.Add(StatusEffect.Fire);
            Type = ProjectileType.LightningBolt;
            SoundWhenFired = Sounds.Zap;
        }

        #endregion

        #region Public Methods

        public override void Update(GameTime gameTime)
        {
            if (Flying)
            {
                lightning.Update(gameTime);
                drawRect.Y -= Speed;
                drawRect.Height += Speed;
            }
        }

        public override void Draw(SpriteBatch spriteBatch, bool withOrigin)
        {
            if (Flying)
            {
                lightning.Draw(spriteBatch, drawRect, Color.Yellow);
            }
            else
            {
                Vector2 origin = new Vector2();
                if (withOrigin)
                {
                    origin.X = batteryImg.Width / 2;
                    origin.Y = batteryImg.Height / 2;
                }
                spriteBatch.Draw(batteryImg, drawRect, null, Color.White, 0.0f, origin, SpriteEffects.None, 1.0f);
            }
        }

        public override void Launch()
        {
            base.Launch();
            drawRect.Width = 10;
        }

        #endregion
    }

    public class Hex : Projectile
    {
        public Hex(int x, int y, int width, int height)
            : base(Utilities.ChaosImg, x, y, width, height, GameInfo.CHAOS_DMG)
        {
            Speed = GameInfo.CHAOS_SPD;
            Type = ProjectileType.Hex;
            Effects.Add(StatusEffect.Curse);
            SoundWhenFired = Sounds.SpellLaunch;
        }
    }

    public class FrozenBlast : Projectile
    {
        Animation frozenBlast;

        public FrozenBlast(int x, int y, int width, int height)
            : base(Utilities.FrozenBlastImg, x, y, width, height, GameInfo.FROZENBLAST_DMG)
        {
            frozenBlast = new Animation(Utilities.FrozenBlastImg, 2, 5, 75, 100, 100, CheckAndPlay);
            frozenBlast.Play();
            Speed = GameInfo.FROZENBLAST_SPD;
            Type = ProjectileType.FrostHex;
            Freezes = true;
            SoundWhenFired = Sounds.SpellLaunch;
        }

        public override void Update(GameTime gameTime)
        {
            frozenBlast.Update(gameTime);
            base.Update(gameTime);
        }

        public override void Draw(SpriteBatch spriteBatch, bool withOrigin)
        {
            Vector2 origin = new Vector2();
            if (withOrigin)
            {
                origin = new Vector2(frozenBlast.FrameWidth / 2, frozenBlast.FrameHeight / 2);
            }
            frozenBlast.Draw(spriteBatch, drawRect, Color.White, origin, SpriteEffects.None);
        }

        private void CheckAndPlay()
        {
            if (Active && !frozenBlast.Playing)
            {
                frozenBlast.Play();
            }
        }
    }

    public class Meteor : SpinningProjectile
    {
        public Meteor(int x, int y, int width, int height)
            : base(Utilities.MeteorImg, x, y, width, height, GameInfo.METEOR_DMG)
        {
            Speed = GameInfo.METEOR_SPD;
            Type = ProjectileType.Meteor;
            Effects.Add(StatusEffect.Fire);
        }
    }

    public class Hammer : SpinningProjectile
    {
        public Hammer(int x, int y, int width, int height)
            : base(Utilities.HammerImg, x, y, width, height, GameInfo.HAMMER_DMG)
        {
            Speed = GameInfo.HAMMER_SPD;
            Type = ProjectileType.Hammer;
            SoundWhenFired = Sounds.Whoosh;
        }
    }

    public class Snowball : Projectile
    {
        public Snowball(int x, int y, int width, int height)
            : base(Utilities.SnowballImg, x, y, width, height, GameInfo.SNOWBALL_DMG)
        {
            Speed = GameInfo.SNOWBALL_SPD;
            Type = ProjectileType.Snowball;
            Freezes = true;
            SoundWhenFired = Sounds.SpellLaunch;
        }
    }

    public class Rocket : ExplosiveProjectile
    {
        public Rocket(int x, int y, int width, int height)
            : base(Utilities.RocketImg, x, y, width, height, GameInfo.ROCKET_DMG)
        {
            Speed = GameInfo.ROCKET_SPD;
            Type = ProjectileType.Rocket;
        }
    }
    public class FireRocket : ExplosiveProjectile
    {
        public FireRocket(int x, int y, int width, int height)
            : base(Utilities.FireRktImg, x, y, width, height, GameInfo.ROCKET_DMG)
        {
            Speed = GameInfo.ROCKET_SPD;
            Type = ProjectileType.FireRocket;
            Effects.Add(StatusEffect.Fire);
        }
    }
    public class PoisonRocket : ExplosiveProjectile
    {
        public PoisonRocket(int x, int y, int width, int height)
            : base(Utilities.PoisonRktImg, x, y, width, height, GameInfo.ROCKET_DMG)
        {
            Speed = GameInfo.ROCKET_SPD;
            Type = ProjectileType.PoisonRocket;
            Effects.Add(StatusEffect.Poison);
        }
    }
    public class PlasmaRocket : ExplosiveProjectile
    {
        public PlasmaRocket(int x, int y, int width, int height)
            : base(Utilities.PlasmaRktImg, x, y, width, height, GameInfo.ROCKET_DMG)
        {
            Speed = GameInfo.ROCKET_SPD;
            Type = ProjectileType.PlasmaRocket;
            Effects.Add(StatusEffect.Plasma);
        }
    }
    public class FrozenRocket : ExplosiveProjectile
    {
        public FrozenRocket(int x, int y, int width, int height)
            : base(Utilities.FrozenRktImg, x, y, width, height, GameInfo.ROCKET_DMG)
        {
            Speed = GameInfo.ROCKET_SPD;
            Type = ProjectileType.FrozenRocket;
            Freezes = true;
        }
    }
    public class OmegaRocket : ExplosiveProjectile
    {
        public OmegaRocket(int x, int y, int width, int height)
            : base(Utilities.OmegaRktImg, x, y, width, height, GameInfo.ROCKET_DMG)
        {
            Speed = GameInfo.ROCKET_SPD;
            Type = ProjectileType.OmegaRocket;
            Freezes = true;
            Effects.Add(StatusEffect.Fire);
            Effects.Add(StatusEffect.Poison);
            Effects.Add(StatusEffect.Plasma);
        }
    }

    public class Bone : SpinningProjectile
    {
        public Bone(int x, int y, int width, int height)
            : base(Utilities.BoneImg, x, y, width, height, GameInfo.BONE_DMG)
        {
            Speed = GameInfo.BONE_SPD;
            Type = ProjectileType.Bone;
        }
    }

    public class Shuriken : SpinningProjectile
    {
        public Shuriken(int x, int y, int width, int height)
            : base(Utilities.ShurikenImg, x, y, width, height, GameInfo.SHURIKEN_DMG)
        {
            Speed = GameInfo.SHURIKEN_SPD;
            Type = ProjectileType.Shuriken;
            SoundWhenFired = Sounds.Whoosh;
        }
    }

    public class IceShard : Projectile
    {
        public IceShard(int x, int y, int width, int height)
            : base(Utilities.IceShardImg, x, y, width, height, GameInfo.ICESHARD_DMG)
        {
            Speed = GameInfo.ICESHARD_SPD;
            Type = ProjectileType.IceShard;
            Freezes = true;
            SoundWhenFired = Sounds.Whoosh;
        }
    }

    public class AbsorbHex : Projectile
    {
        public AbsorbHex(int x, int y, int width, int height)
            : base(Utilities.AbsorbHexImg, x, y, width, height, GameInfo.ABSORBHEX_DMG)
        {
            Speed = GameInfo.ICESHARD_SPD;
            Type = ProjectileType.AbsorbHex;
            HealingPower = GameInfo.ABSORBHEX_HEAL;
            SoundWhenFired = Sounds.SpellLaunch;
        }
    }

    #region Unused
    //public class AlienMinion : Projectile
    //{
    //    #region Fields

    //    Texture2D eyeImg;
    //    Rectangle eyeRect;

    //    const int SPACING = 2;
    //    const int EYE_SPACING = 10;

    //    #endregion

    //    #region Constructors

    //    public AlienMinion(int x, int y, int width, int height)
    //        : base(Utilities.AlienImg, x, y, width, height, GameInfo.NORMAL_ALIEN_WORTH)
    //    {
    //        ProjType = ProjectileType.AlienMinion;

    //        eyeImg = Utilities.AlienEyeImg;
    //        eyeRect = new Rectangle(drawRect.X + (int)(EYE_SPACING * 1.6f), drawRect.Y + EYE_SPACING, 
    //            SPACING * 8, SPACING * 2);
    //    }

    //    #endregion

    //    #region Public Methods

    //    public override void Update(GameTime gameTime)
    //    {
    //        base.Update(gameTime);
    //        if (Flying)
    //        {
    //            eyeRect.Y -= Speed;
    //        }
    //    }

    //    public override void Draw(SpriteBatch spriteBatch, bool withOrigin)
    //    {
    //        Vector2 origin = new Vector2();
    //        if (withOrigin)
    //        {
    //            origin = new Vector2(image.Width / 2, image.Height / 2);
    //        }
    //        spriteBatch.Draw(image, drawRect, null, Color.Lime, 0.0f, origin, SpriteEffects.None, 1.0f);
    //        spriteBatch.Draw(eyeImg, eyeRect, Color.White);
    //    }

    //    #endregion
    //}
    #endregion

    #region Enumerations

    public enum Direction
    {
        Up,
        Down,
        Left,
        Right
    }

    public enum ProjectileType
    {
        // 0.5.0
        // First Group
        None,
        Rock,
        Cannonball,
        Fireball,
        Bomb,
        Dart,
        PoisonDart,
        Laser,
        Hex,
        LightningBolt,
        // Second Group
        FrostHex,
        // Third Group
        Meteor,
        Hammer,
        // Fourth Group
        Rocket,
        FireRocket,
        PoisonRocket,
        FrozenRocket,
        PlasmaRocket,
        OmegaRocket,
        Snowball,
        // Fifth Group
        Shuriken,
        Bone,
        IceShard,

        // 1.1
        AbsorbHex,
    }

    #endregion
}