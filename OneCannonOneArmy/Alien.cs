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
    public delegate void OnAlienHit(Alien alien);

    public abstract class Alien
    {
        #region Fields & Properties

        protected Texture2D img;
        protected Rectangle drawRect;
        protected Texture2D eyeImg;
        protected Rectangle eyeRect;
        protected float y;
        protected float eyeY;
        public Aliens Type;
        Timer eyeMoveTimer;
        int eyeDirectionIndex;
        List<Direction> eyeSequence = new List<Direction>()
        {
            Direction.Right,
            Direction.Right,
            Direction.Left,
            Direction.Left,
        };

        protected float speed = 1.0f;
        public int Worth = 0;

        protected FillBar healthBar;
        const int HEALTHBAR_HEIGHT = 5;
        public bool ShowHealthbar = true;

        public float Health = 0;
        public float Defense = 0;
        public float FireDefense = 0;
        public float PoisonDefense = 0;
        public float PlasmaDefense = 0;

        const int SPACING = 2;
        const int EYE_SPACING = 10;

        Color currentColor;
        Color origColor;
        Color hurtColor;

        event OnAlienHit onHit;
        event OnAlienHit onDeath;

        float alpha = 0.5f;

        public bool Dead = false;

        protected bool isMechAlien = false;
        public bool IsMechAlien
        {
            get
            {
                return isMechAlien;
            }
            set
            {
                if (value == true && isMechAlien == false)
                {
                    // Add some coins to the drops
                    for (int i = 0; i < GameInfo.MECH_WORTH; i++)
                    {
                        Coin newC = new Coin(Utilities.CoinImage, 0, 0, COIN_SIZE, COIN_SIZE, 1);
                        newC.AddOnCollectHandler(itemCollected);
                        Drops.Add(newC);
                    }
                }
                else if (value == false && isMechAlien == true)
                {
                    // Remove coins that were previously available
                    for (int i = 0; i < GameInfo.MECH_WORTH; i++)
                    {
                        Drops.Remove(Drops.Where(x => x is Coin).First());
                    }
                }
                isMechAlien = value;
            }
        }
        protected Texture2D mechImg;
        protected Rectangle mechRect;
        protected float mechY;
        const float MECH_RATIO = 0.7f;

        public int X
        {
            get
            {
                return drawRect.X;
            }
            set
            {
                drawRect.X = value;
                eyeRect.X = drawRect.X + (int)(EYE_SPACING * 1.6f);
                shieldRect.X = drawRect.X + (drawRect.Width / 2 - (shieldRect.Width / 2));
                mechRect.X = drawRect.Right - (mechRect.Width / 2);
                healthBar.X = drawRect.X;
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
                eyeRect.Y = drawRect.Y + EYE_SPACING;
                shieldRect.Y = drawRect.Bottom;
                mechRect.Y = drawRect.Bottom - (mechRect.Height / 2);
                y = drawRect.Y;
                eyeY = eyeRect.Y;
                mechY = mechRect.Y;
            }
        }

        HashSet<StatusEffect> effects = new HashSet<StatusEffect>();

        Timer effTimer;

        bool flashingRed = false;
        bool escapingRed = false;
        const float CHANGE_COUNT = 15.0f;
        int colorCount = 0;

        public List<Item> Drops = new List<Item>();
        const int COIN_SIZE = 15;

        public bool Active = true;

        public const int INIT_SHIELD_HEALTH = 5;
        public bool HasShield = false;
        public int ShieldHealth = INIT_SHIELD_HEALTH;
        protected Rectangle shieldRect;
        const int SHIELD_SIZE = 24;

        protected bool canBeFrozen = true;
        public bool Frozen = false;
        protected Timer frozenTimer = new Timer(5.0f, TimerUnits.Seconds);
        protected const float NORM_SPD = 1.0f;
        protected const float FREEZE_SPD = 0.5f;

        const int DROP_SIZE = 20;

        OnItemCollect itemCollected;

        #endregion

        #region Constructors

        public Alien(Texture2D img, Texture2D eyeImg, int x, int y, int width, int height, int worth, int health,
            Color color, OnItemCollect itemCollected)
        {
            this.itemCollected = itemCollected;

            drawRect = new Rectangle(x, y, width, height);
            eyeRect = new Rectangle(drawRect.X + (int)(EYE_SPACING * 1.6f), drawRect.Y + EYE_SPACING, SPACING * 8,
                SPACING * 2);

            this.y = y;
            eyeY = eyeRect.Y;

            this.img = img;
            this.eyeImg = eyeImg;
            Worth = worth;
            Health = health;
            origColor = color;
            Color red = new Color(255, 0, 0);
            color.R = (byte)((color.R + red.R) / 2.0f);
            color.G = (byte)((color.G + red.G) / 2.0f);
            color.B = (byte)((color.B + red.B) / 2.0f);
            hurtColor = color;
            currentColor = origColor;
            effTimer = new Timer(2, TimerUnits.Seconds);

            shieldRect = new Rectangle(0, 0, SHIELD_SIZE, SHIELD_SIZE);
            shieldRect.X = drawRect.Right - shieldRect.Width / 2;
            shieldRect.Y = drawRect.Bottom - shieldRect.Height / 2;

            for (int i = 0; i < worth; i++)
            {
                Coin newC = new Coin(Utilities.CoinImage, 0, 0, COIN_SIZE, COIN_SIZE, 1);
                newC.AddOnCollectHandler(itemCollected);
                Drops.Add(newC);
            }

            eyeMoveTimer = new Timer(200, TimerUnits.Milliseconds);

            healthBar = new FillBar(health, drawRect.X, 0, drawRect.Width, HEALTHBAR_HEIGHT, health, null);
            healthBar.Y = drawRect.Y - healthBar.Height - SPACING;

            mechImg = Utilities.MechImg;
            mechRect = new Rectangle();
            mechRect.Width = (int)(MECH_RATIO * height);
            mechRect.Height = mechRect.Width;
            mechRect.X = drawRect.Right - (mechRect.Width / 2);
            mechRect.Y = drawRect.Bottom - (mechRect.Height / 2);
            mechY = mechRect.Y;
        }

        #endregion

        #region Public Methods

        public virtual void Update(ref List<Projectile> playerProjectiles, GameTime gameTime,
            ref List<Projectile> alienProjectiles)
        {
            if (Health > 0)
            {
                eyeMoveTimer.Update(gameTime);
                y += speed;
                eyeY += speed;
                drawRect.Y = (int)y;
                eyeRect.Y = (int)eyeY;
                if (isMechAlien)
                {
                    mechY += speed;
                    mechRect.Y = (int)mechY;
                }
                if (HasShield)
                {
                    shieldRect.Y = drawRect.Bottom - shieldRect.Height / 2;
                }

                // Move eyes, giving alien a dynamic feel
                if (eyeMoveTimer.QueryWaitTime(gameTime))
                {
                    switch (eyeSequence[eyeDirectionIndex])
                    {
                        case Direction.Up:
                            eyeRect.Y--;
                            break;
                        case Direction.Down:
                            eyeRect.Y++;
                            break;
                        case Direction.Left:
                            eyeRect.X--;
                            break;
                        case Direction.Right:
                            eyeRect.X++;
                            break;
                    }
                    if (eyeDirectionIndex >= eyeSequence.Count - 1)
                    {
                        // We put the count - 1 because the index can be less than the count,
                        // but then we add to it and an exception will be thrown
                        eyeDirectionIndex = 0;
                    }
                    else
                    {
                        eyeDirectionIndex++;
                    }
                }
            }
            else
            {
                drawRect.Y -= 1;
                eyeRect.Y -= 1;
                alpha -= 0.009f;
                if (alpha <= 0.0f)
                {
                    Active = false;
                }
            }

            if (Frozen)
            {
                frozenTimer.Update(gameTime);
                if (frozenTimer.QueryWaitTime(gameTime))
                {
                    Frozen = false;
                    speed = NORM_SPD;
                }
            }

            for (int i = 0; i < playerProjectiles.Count; i++)
            {
                if ((playerProjectiles[i].Intersects(drawRect) || (HasShield && playerProjectiles[i].Intersects(shieldRect)))
                    && Health > 0 && playerProjectiles[i].Flying)
                // If we don't check if projectile isn't flying, then aliens can be killed by touching a loaded projectile
                {
                    if (HasShield)
                    {
                        ShieldHealth -= playerProjectiles[i].Damage;
                        if (ShieldHealth <= 0)
                        {
                            Sound.PlaySound(Sounds.ShieldBreak);
                            HasShield = false;
                        }
                    }
                    else
                    {
                        Health -= playerProjectiles[i].Damage - (Defense * playerProjectiles[i].Damage);
                        foreach (StatusEffect e in playerProjectiles[i].Effects)
                        {
                            effects.Add(e);
                            // Gives the user a visual when something like
                            // a fireball hits, and instantly deals the effect's damage
                        }

                        flashingRed = true;

                        if (playerProjectiles[i].Freezes && canBeFrozen)
                        {
                            Frozen = true;
                            speed = FREEZE_SPD;
                        }
                    }

                    if (Health <= 0)
                    {
                        OnDeath();
                    }
                    else
                    {
                        Sound.PlaySound(Sounds.AlienHit);
                    }

                    playerProjectiles[i].OnHit();

                    onHit?.Invoke(this);
                }
            }

            healthBar.Value = Health;
            healthBar.Y = drawRect.Y - healthBar.Height - SPACING;

            if (flashingRed)
            {
                if (currentColor != hurtColor)
                {
                    currentColor.R = (byte)MathHelper.Lerp(origColor.R, hurtColor.R, colorCount / CHANGE_COUNT);
                    currentColor.G = (byte)MathHelper.Lerp(origColor.G, hurtColor.G, colorCount / CHANGE_COUNT);
                    currentColor.B = (byte)MathHelper.Lerp(origColor.B, hurtColor.B, colorCount / CHANGE_COUNT);
                    colorCount++;
                }
                else
                {
                    escapingRed = true;
                    flashingRed = false;
                    colorCount = 0;
                }
            }
            else if (escapingRed)
            {
                if (currentColor != origColor)
                {
                    currentColor.R = (byte)MathHelper.Lerp(hurtColor.R, origColor.R, colorCount / CHANGE_COUNT);
                    currentColor.G = (byte)MathHelper.Lerp(hurtColor.G, origColor.G, colorCount / CHANGE_COUNT);
                    currentColor.B = (byte)MathHelper.Lerp(hurtColor.B, origColor.B, colorCount / CHANGE_COUNT);
                    colorCount++;
                }
                else
                {
                    escapingRed = false;
                    colorCount = 0;
                }
            }

            if (effects.Count > 0)
            {
                effTimer.Update(gameTime);
                if (effTimer.QueryWaitTime(gameTime))
                {
                    foreach (StatusEffect e in effects)
                    {
                        ApplyEffect(e);
                    }
                }
            }
        }

        public virtual void Draw(SpriteBatch spriteBatch)
        {
            if (Health > 0)
            {
                spriteBatch.Draw(img, drawRect, currentColor);
                spriteBatch.Draw(eyeImg, eyeRect, Color.White);
                if (HasShield)
                {
                    spriteBatch.Draw(Utilities.AlienShieldImg, shieldRect, Color.White);
                }
                if (ShowHealthbar)
                {
                    healthBar.Draw(spriteBatch, GameInfo.GetColorForAmount((int)Health, (int)healthBar.MaxValue),
                        Color.Transparent);
                }
                if (isMechAlien)
                {
                    spriteBatch.Draw(mechImg, mechRect, Color.White);
                }
            }
            else
            {
                spriteBatch.Draw(img, drawRect, Color.White * alpha);
                spriteBatch.Draw(eyeImg, eyeRect, Color.White * alpha);
            }
        }

        public virtual void AddOnHitHandler(OnAlienHit action)
        {
            onHit += action;
        }
        public virtual void AddOnDeathHandler(OnAlienHit action)
        {
            onDeath += action;
        }

        public void ApplyDamage(float damage, bool useShield)
        {
            if (useShield && HasShield)
            {
                ShieldHealth -= (int)damage;
                if (ShieldHealth <= 0)
                {
                    HasShield = false;
                    Sound.PlaySound(Sounds.ShieldBreak);
                }
            }
            else
            {
                Health -= damage;
                if (Health <= 0)
                {
                    OnDeath();
                }
            }
        }

        #endregion

        #region Protected & Private Methods

        protected virtual void ApplyEffect(StatusEffect effect)
        {
            if (!Dead)
            {
                if (effect == StatusEffect.Fire && FireDefense > 0)
                {
                    Health -= effect.Damage - (FireDefense * effect.Damage);
                }
                else if (effect == StatusEffect.Poison && PoisonDefense > 0)
                {
                    Health -= effect.Damage - (PoisonDefense * effect.Damage);
                }
                else if (effect == StatusEffect.Plasma && PlasmaDefense > 0)
                {
                    Health -= effect.Damage - (PlasmaDefense * effect.Damage);
                }
                else
                {
                    Health -= effect.Damage;
                }
                if (!flashingRed)
                {
                    flashingRed = true;
                }
                if (Health <= 0)
                {
                    OnDeath();
                }
            }
        }

        protected void OnDeath()
        {
            if (!Dead)
            {
                int randomNumX = 0;
                int randomNumY = 0;
                for (int i = 0; i < Drops.Count; i++)
                {
                    randomNumX = Utilities.Rand.Next((COIN_SIZE + SPACING) * -1, COIN_SIZE + SPACING);
                    randomNumY = Utilities.Rand.Next((COIN_SIZE + SPACING) * -1, COIN_SIZE + SPACING);
                    Drops[i].X = drawRect.X + randomNumX;
                    Drops[i].Y = drawRect.Y + randomNumY;
                }
                onDeath?.Invoke(this);
                Sound.PlaySound(Sounds.AlienDeath);
                Dead = true;
            }
        }

        protected void AddPotentialDrop(Material drop, int percent)
        {
            int num = Utilities.Rand.Next(100);
            if (num <= percent)
            {
                MaterialDrop newM = new MaterialDrop(drop, 0, 0, DROP_SIZE, DROP_SIZE);
                newM.AddOnCollectHandler(itemCollected);
                Drops.Add(newM);
            }
        }
        protected void AddPotentialDrop(Badge badge, int percent)
        {
            int num = Utilities.Rand.Next(100);
            if (num <= percent)
            {
                BadgeDrop newB = new BadgeDrop(badge, 0, 0, DROP_SIZE, DROP_SIZE);
                newB.AddOnCollectHandler(itemCollected);
                Drops.Add(newB);
            }
        }

        #endregion
    }

    public class NormalAlien : Alien
    {
        #region Constructors

        public NormalAlien(Texture2D img, Texture2D eyeImg, int x, int y, int width, int height,
            OnItemCollect itemCollected)
            : base(img, eyeImg, x, y, width, height, GameInfo.NORMAL_ALIEN_WORTH, GameInfo.NORMAL_ALIEN_HEALTH,
                  Color.Lime, itemCollected)
        {
            Type = Aliens.Normal;
            AddPotentialDrop(Material.Stone, 10);
            AddPotentialDrop(Badge.Green, 5);
        }

        #endregion
    }

    public class DefenseAlien : Alien
    {
        public DefenseAlien(Texture2D img, Texture2D eyeImg, int x, int y, int width, int height,
            OnItemCollect itemCollected)
            : base(img, eyeImg, x, y, width, height, GameInfo.DEFENSE_ALIEN_WORTH, GameInfo.DEFENSE_ALIEN_HEALTH,
                  Color.DeepSkyBlue, itemCollected)
        {
            Defense = GameInfo.NORMAL_DEFENSE;

            AddPotentialDrop(Material.Metal, 25);
            AddPotentialDrop(Badge.Blue, 10);

            Type = Aliens.Defense;
        }
    }
    public class LDefenseAlien : Alien
    {
        public LDefenseAlien(Texture2D img, Texture2D eyeImg, int x, int y, int width, int height,
            OnItemCollect itemCollected)
            : base(img, eyeImg, x, y, width, height, GameInfo.LDEFENSE_ALIEN_WORTH, GameInfo.LDEFENSE_ALIEN_HEALTH,
                  Color.SkyBlue, itemCollected)
        {
            Defense = GameInfo.LIGHT_DEFENSE;

            AddPotentialDrop(Material.Metal, 20);
            AddPotentialDrop(Badge.Blue, 5);

            Type = Aliens.LDefense;
        }
    }
    public class HDefenseAlien : Alien
    {
        public HDefenseAlien(Texture2D img, Texture2D eyeImg, int x, int y, int width, int height,
            OnItemCollect itemCollected)
            : base(img, eyeImg, x, y, width, height, GameInfo.HDEFENSE_ALIEN_WORTH, GameInfo.HDEFENSE_ALIEN_HEALTH,
                  new Color(0, 120, 255), itemCollected)
        {
            Defense = GameInfo.HEAVY_DEFENSE;

            AddPotentialDrop(Material.Metal, 30);
            AddPotentialDrop(Badge.Blue, 15);

            Type = Aliens.HDefense;
        }
    }

    public class FireDefenseAlien : Alien
    {
        public FireDefenseAlien(Texture2D img, Texture2D eyeImg, int x, int y, int width, int height,
            OnItemCollect itemCollected)
            : base(img, eyeImg, x, y, width, height, GameInfo.FIRE_DEFENSE_ALIEN_WORTH, GameInfo.FIRE_DEFENSE_ALIEN_HEALTH,
                  Color.Orange, itemCollected)
        {
            FireDefense = GameInfo.NORMAL_DEFENSE;
            Defense = GameInfo.NORMAL_DEFENSE;

            AddPotentialDrop(Material.EssenceOfFire, 25);
            AddPotentialDrop(Badge.Orange, 10);

            Type = Aliens.FireDefense;
        }
    }
    public class LFireDefenseAlien : Alien
    {
        public LFireDefenseAlien(Texture2D img, Texture2D eyeImg, int x, int y, int width, int height,
            OnItemCollect itemCollected)
            : base(img, eyeImg, x, y, width, height, GameInfo.LFIRE_DEFENSE_ALIEN_WORTH, GameInfo.LFIRE_DEFENSE_ALIEN_HEALTH,
                  new Color(255, 213, 139), itemCollected)
        {
            FireDefense = GameInfo.LIGHT_DEFENSE;
            Defense = GameInfo.LIGHT_DEFENSE;

            AddPotentialDrop(Material.EssenceOfFire, 20);
            AddPotentialDrop(Badge.Orange, 5);

            Type = Aliens.LFireDefense;
        }
    }
    public class HFireDefenseAlien : Alien
    {
        public HFireDefenseAlien(Texture2D img, Texture2D eyeImg, int x, int y, int width, int height,
            OnItemCollect itemCollected)
            : base(img, eyeImg, x, y, width, height, GameInfo.HFIRE_DEFENSE_ALIEN_WORTH, GameInfo.HFIRE_DEFENSE_ALIEN_HEALTH,
                  Color.DarkOrange, itemCollected)
        {
            FireDefense = GameInfo.HEAVY_DEFENSE;
            Defense = GameInfo.HEAVY_DEFENSE;

            AddPotentialDrop(Material.EssenceOfFire, 30);
            AddPotentialDrop(Badge.Orange, 15);

            Type = Aliens.HFireDefense;
        }
    }

    public class ChaosAlien : ProjectileAlien
    {
        #region Constructors

        public ChaosAlien(Texture2D img, Texture2D eyeImg, int x, int y, int width, int height,
            OnItemCollect itemCollected)
            : base(img, eyeImg, x, y, width, height, GameInfo.CHAOS_ALIEN_WORTH, GameInfo.CHAOS_ALIEN_HEALTH,
                  Color.MediumPurple, itemCollected, ProjectileType.Hex, 5)
        {
            Defense = GameInfo.HEAVY_DEFENSE;
            FireDefense = GameInfo.NORMAL_DEFENSE;
            PoisonDefense = GameInfo.NORMAL_DEFENSE;
            PlasmaDefense = GameInfo.LIGHT_DEFENSE;

            for (int i = 0; i < 2; i++)
            {
                AddPotentialDrop(Material.ChaosEnergy, 50);
            }

            Type = Aliens.Chaos;
        }

        #endregion
    }

    public abstract class ProjectileAlien : Alien
    {
        #region Fields

        Timer launchTimer;
        protected ProjectileType projectileFired;

        #endregion

        #region Constructors

        public ProjectileAlien(Texture2D img, Texture2D eyeImg, int x, int y, int width, int height, int worth,
            int health, Color color, OnItemCollect itemCollected, ProjectileType projectileFired, float secondsRecharge)
            : base(img, eyeImg, x, y, width, height, worth, health, color, itemCollected)
        {
            this.projectileFired = projectileFired;
            launchTimer = new Timer(secondsRecharge, TimerUnits.Seconds);
        }

        #endregion

        #region Public Methods

        public override void Update(ref List<Projectile> playerProjectiles, GameTime gameTime,
            ref List<Projectile> alienProjectiles)
        {
            base.Update(ref playerProjectiles, gameTime, ref alienProjectiles);
            launchTimer.Update(gameTime);
            if (launchTimer.QueryWaitTime(gameTime) && !Dead && drawRect.Y > 0 && !Frozen)
            {
                Projectile newP = GameInfo.CreateProj(projectileFired);
                newP.X = drawRect.X + (drawRect.Width / 2);
                newP.Y = drawRect.Y;
                Sound.PlaySound(newP.SoundWhenFired);
                alienProjectiles.Add(newP);
            }
        }

        #endregion
    }

    public class BossAlien : ProjectileAlien
    {
        Timer sideToSideTimer;
        bool movingSideToSide = false;
        int sideOffset;
        const int MAX_SIDE_OFFSET = 100;
        const int SIDE_SPD = 2;
        int sideDirection = 1;

        public BossAlien(Texture2D img, Texture2D eyeImg, int x, int y, int width, int height,
            OnItemCollect itemCollected)
            : base(img, eyeImg, x, y, width, height, GameInfo.BOSS_ALIEN_WORTH, GameInfo.BOSS_ALIEN_HEALTH,
                  Color.DarkRed, itemCollected, ProjectileType.Laser, 9)
        {
            speed = 0.1f;
            canBeFrozen = false;
            Defense = GameInfo.BOSS_DEFENSE;

            sideToSideTimer = new Timer(20.0f, TimerUnits.Seconds);

            AddPotentialDrop(Material.ChaosEnergy, 75);
            AddPotentialDrop(Material.Metal, 65);
            AddPotentialDrop(Material.Stone, 65);
            AddPotentialDrop(Material.Plasma, 90);
            AddPotentialDrop(Badge.Red, 100);

            Type = Aliens.Boss;
        }

        public override void Update(ref List<Projectile> playerProjectiles, GameTime gameTime,
            ref List<Projectile> alienProjectiles)
        {
            base.Update(ref playerProjectiles, gameTime, ref alienProjectiles);

            if (movingSideToSide)
            {
                sideOffset += SIDE_SPD * sideDirection;
                drawRect.X += SIDE_SPD * sideDirection;
                eyeRect.X += SIDE_SPD * sideDirection;
                healthBar.X += SIDE_SPD * sideDirection;
                if (HasShield)
                {
                    shieldRect.X += SIDE_SPD * sideDirection;
                }
                if (isMechAlien)
                {
                    mechRect.X += SIDE_SPD * sideDirection;
                }

                if (Math.Abs(sideOffset) >= MAX_SIDE_OFFSET)
                {
                    sideDirection *= -1;
                }
            }
            else
            {
                sideToSideTimer.Update(gameTime);
                if (sideToSideTimer.QueryWaitTime(gameTime))
                {
                    movingSideToSide = true;
                }
            }
        }

        protected override void ApplyEffect(StatusEffect e)
        {
            // Do nothing, as the boss is immune to status effects
        }
    }

    public class PoisonResistantAlien : Alien
    {
        public PoisonResistantAlien(Texture2D img, Texture2D eyeImg, int x, int y, int width, int height,
            OnItemCollect itemCollected)
            : base(img, eyeImg, x, y, width, height, GameInfo.POISON_RESIST_ALIEN_WORTH,
                  GameInfo.POISON_RESIST_ALIEN_HEALTH, new Color(255, 255, 45), itemCollected)
        {
            PoisonDefense = GameInfo.NORMAL_DEFENSE;
            Defense = GameInfo.NORMAL_DEFENSE;

            AddPotentialDrop(Material.PlantMatter, 25);
            AddPotentialDrop(Badge.Yellow, 10);

            Type = Aliens.PoisonResistant;
        }
    }
    public class LPoisonResistantAlien : Alien
    {
        public LPoisonResistantAlien(Texture2D img, Texture2D eyeImg, int x, int y, int width, int height,
            OnItemCollect itemCollected)
            : base(img, eyeImg, x, y, width, height, GameInfo.LPOISON_RESIST_ALIEN_WORTH,
                  GameInfo.LPOISON_RESIST_ALIEN_HEALTH, new Color(255, 255, 135), itemCollected)
        {
            PoisonDefense = GameInfo.LIGHT_DEFENSE;
            Defense = GameInfo.LIGHT_DEFENSE;

            AddPotentialDrop(Material.PlantMatter, 20);
            AddPotentialDrop(Badge.Yellow, 5);

            Type = Aliens.LPoisonResistant;
        }
    }
    public class HPoisonResistantAlien : Alien
    {
        public HPoisonResistantAlien(Texture2D img, Texture2D eyeImg, int x, int y, int width, int height,
            OnItemCollect itemCollected)
            : base(img, eyeImg, x, y, width, height, GameInfo.HPOISON_RESIST_ALIEN_WORTH,
                  GameInfo.HPOISON_RESIST_ALIEN_HEALTH, new Color(255, 227, 0), itemCollected)
        {
            PoisonDefense = GameInfo.HEAVY_DEFENSE;
            Defense = GameInfo.HEAVY_DEFENSE;

            AddPotentialDrop(Material.PlantMatter, 30);
            AddPotentialDrop(Badge.Yellow, 15);

            Type = Aliens.HPoisonResistant;
        }
    }

    public class FreezeProofAlien : Alien
    {
        public FreezeProofAlien(Texture2D img, Texture2D eyeImg, int x, int y, int width, int height,
            OnItemCollect itemCollected)
            : base(img, eyeImg, x, y, width, height, GameInfo.FREEZE_PROOF_ALIEN_WORTH,
                  GameInfo.FREEZE_PROOF_ALIEN_HEALTH, new Color(0, 255, 255), itemCollected)
        {
            canBeFrozen = false;
            Defense = GameInfo.NORMAL_DEFENSE;

            AddPotentialDrop(Material.Ice, 30);

            Type = Aliens.FreezeProof;
        }
    }

    public class PlasmaResistantAlien : Alien
    {
        public PlasmaResistantAlien(Texture2D img, Texture2D eyeImg, int x, int y, int width, int height,
            OnItemCollect itemCollected)
            : base(img, eyeImg, x, y, width, height, GameInfo.PLASMA_RESIST_ALIEN_WORTH,
                  GameInfo.PLASMA_RESIST_ALIEN_HEALTH, new Color(100, 100, 255), itemCollected)
        {
            PlasmaDefense = GameInfo.NORMAL_DEFENSE;
            Defense = GameInfo.NORMAL_DEFENSE;

            AddPotentialDrop(Material.Plasma, 25);
            AddPotentialDrop(Badge.Purple, 10);

            Type = Aliens.PlasmaResistant;
        }
    }
    public class LPlasmaResistantAlien : Alien
    {
        public LPlasmaResistantAlien(Texture2D img, Texture2D eyeImg, int x, int y, int width, int height,
            OnItemCollect itemCollected)
            : base(img, eyeImg, x, y, width, height, GameInfo.LPLASMA_RESIST_ALIEN_WORTH,
                  GameInfo.LPLASMA_RESIST_ALIEN_HEALTH, new Color(127, 127, 255), itemCollected)
        {
            PlasmaDefense = GameInfo.LIGHT_DEFENSE;
            Defense = GameInfo.LIGHT_DEFENSE;

            AddPotentialDrop(Material.Plasma, 20);
            AddPotentialDrop(Badge.Purple, 5);

            Type = Aliens.LPlasmaResistant;
        }
    }
    public class HPlasmaResistantAlien : Alien
    {
        public HPlasmaResistantAlien(Texture2D img, Texture2D eyeImg, int x, int y, int width, int height,
            OnItemCollect itemCollected)
            : base(img, eyeImg, x, y, width, height, GameInfo.HPLASMA_RESIST_ALIEN_WORTH,
                  GameInfo.HPLASMA_RESIST_ALIEN_HEALTH, new Color(70, 70, 255), itemCollected)
        {
            PlasmaDefense = GameInfo.HEAVY_DEFENSE;
            Defense = GameInfo.HEAVY_DEFENSE;

            AddPotentialDrop(Material.Plasma, 30);
            AddPotentialDrop(Badge.Purple, 15);

            Type = Aliens.HPlasmaResistant;
        }
    }

    public class OmegaAlien : ProjectileAlien
    {
        public OmegaAlien(Texture2D img, Texture2D eyeImg, int x, int y, int width, int height,
            OnItemCollect itemCollected)
            : base(img, eyeImg, x, y, width, height, GameInfo.OMEGA_ALIEN_WORTH,
                  GameInfo.OMEGA_ALIEN_HEALTH, Color.Gold, itemCollected, ProjectileType.Meteor, 6)
        {
            Defense = GameInfo.OMEGA_DEFENSE;
            
            AddPotentialDrop(Material.Plasma, 20);
            AddPotentialDrop(Material.PlantMatter, 20);
            AddPotentialDrop(Material.EssenceOfFire, 20);
            AddPotentialDrop(Material.Ice, 20);
            AddPotentialDrop(Material.ChaosEnergy, 10);

            Type = Aliens.Omega;
        }

        protected override void ApplyEffect(StatusEffect e)
        {
            // Do nothing, as the Omega Alien is immune to all effects
        }
    }

    public class NinjaAlien : ProjectileAlien
    {
        public NinjaAlien(Texture2D img, Texture2D eyeImg, int x, int y, int width, int height,
            OnItemCollect itemCollected)
            : base(img, eyeImg, x, y, width, height, GameInfo.NINJA_ALIEN_WORTH, GameInfo.NINJA_ALIEN_HEALTH,
                  Color.White, itemCollected, ProjectileType.Shuriken, 3.5f)
        {
            Defense = GameInfo.NORMAL_DEFENSE;

            AddPotentialDrop(Material.Metal, 10);
            Type = Aliens.Ninja;
        }
    }

    public enum Aliens
    {
        Normal,
        Defense,
        LDefense,
        HDefense,
        FireDefense,
        LFireDefense,
        HFireDefense,
        Chaos,
        Boss,
        PoisonResistant,
        LPoisonResistant,
        HPoisonResistant,
        FreezeProof,
        PlasmaResistant,
        LPlasmaResistant,
        HPlasmaResistant,
        Omega,
        Ninja,
    }
    public enum BasicAlienTypes
    {
        Normal,
        Defense,
        FireDefense,
        Chaos,
        Boss,
        PoisonResistant,
        FreezeProof,
        PlasmaResistant,
        Omega,
        Ninja,
    }

    public struct AlienType
    {
        public Aliens Type;
        public bool HasShield;

        public AlienType(Aliens type, bool hasShield)
        {
            Type = type;
            HasShield = hasShield;
        }
    }
}
