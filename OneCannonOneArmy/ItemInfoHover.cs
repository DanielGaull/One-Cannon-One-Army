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
    public abstract class ItemInfoHover
    {
        #region Fields & Properties

        protected List<NameValue> stats = new List<NameValue>();
        protected List<Vector2> statLocs = new List<Vector2>();

        protected string name = "";
        Vector2 namePos;
        protected string desc = "";
        Vector2 descPos;

        Texture2D bgImg;
        Rectangle bgRect;

        const int SPACING = 6;

        SpriteFont font;

        public bool Active = false;

        int windowWidth;

        const int WIDTH = 400;
        const int HEIGHT = 200;

        #endregion

        #region Constructors

        public ItemInfoHover(GraphicsDevice graphics, SpriteFont font, string name,
            string desc, List<NameValue> stats, int windowWidth)
        {
            this.stats = stats;
            this.name = name;
            this.desc = desc;
            this.font = font;
            this.windowWidth = windowWidth;

            bgImg = new Texture2D(graphics, WIDTH, HEIGHT);
            bgImg = DrawHelper.AddBorder(bgImg, 3, Color.Gray, Color.LightGray);
            bgRect = new Rectangle(0, 0, WIDTH, HEIGHT);

            namePos = new Vector2(bgRect.X + bgRect.Width / 2 - (font.MeasureString(Language.Translate(name)).X / 2),
                bgRect.Y + SPACING);
            descPos = new Vector2(bgRect.X + SPACING, namePos.Y + font.MeasureString(name).Y + SPACING);

            int x1 = bgRect.X + SPACING;
            int x2 = bgRect.X + bgRect.Width / 2;
            int y = (int)descPos.Y + (SPACING * 12);
            bool fCol = true;
            for (int i = 0; i < stats.Count; i++)
            {
                statLocs.Add(new Vector2(fCol ? x1 : x2, y));
                if (!fCol)
                {
                    y += (int)font.MeasureString("A").Y;
                }
                fCol = !fCol;
            }
        }

        #endregion

        #region Public Methods

        public void Update()
        {
            if (Active)
            {
                MouseState mouse = Mouse.GetState();
                if (mouse.X > windowWidth / 2)
                {
                    bgRect.X = mouse.X - bgRect.Width;
                }
                else
                {
                    bgRect.X = mouse.X;
                }
                bgRect.Y = mouse.Y;
                Position();
            }
        }

        public void Draw(SpriteBatch spriteBatch)
        {
            if (Active)
            {
                spriteBatch.Draw(bgImg, bgRect, Color.White);
                spriteBatch.DrawString(font, Language.Translate(name), namePos, Color.Black);
                spriteBatch.DrawString(font, Language.Translate(desc), descPos, Color.Black);

                for (int i = 0; i < stats.Count; i++)
                {
                    spriteBatch.DrawString(font, Language.Translate(stats[i].Name) + 
                        (stats[i].Name != "" ? ": " : "") + stats[i].Value, statLocs[i], Color.Black);
                }
            }
        }

        #endregion

        #region Protected & Private Methods

        protected void Position()
        {
            descPos.X = bgRect.X + SPACING;
            descPos.Y = namePos.Y + font.MeasureString(name).Y + SPACING;

            namePos.X = bgRect.X + bgRect.Width / 2 - (font.MeasureString(Language.Translate(name)).X / 2);
            namePos.Y = bgRect.Y + SPACING;

            int x1 = bgRect.X + SPACING;
            int x2 = bgRect.X + bgRect.Width / 2;
            int y = (int)descPos.Y + (SPACING * 12);
            bool fCol = true;
            for (int i = 0; i < stats.Count; i++)
            {
                statLocs[i] = new Vector2(fCol ? x1 : x2, y);
                if (!fCol)
                {
                    y += (int)font.MeasureString("A").Y;
                }
                fCol = !fCol;
            }
        }

        #endregion
    }

    public class MaterialInfoHover : ItemInfoHover
    {
        public MaterialInfoHover(Material material, SpriteFont font, GraphicsDevice graphics, int windowWidth)
            : base(graphics, font, material.ToString().AddSpaces(), Utilities.GetDescOf(material),
                  new List<NameValue>(), windowWidth)
        {
            stats.Add(new NameValue("Cost", GameInfo.CostOf(material).ToString()));
            stats.Add(new NameValue("Sell Value", GameInfo.SellValueOf(material).ToString()));
            for (int i = 0; i < stats.Count; i++)
            {
                statLocs.Add(new Vector2());
            }
            Position();
        }

        public void ResetMaterial(Material material)
        {
            stats.Clear();
            stats.Add(new NameValue("Cost", GameInfo.CostOf(material).ToString() + "c"));
            stats.Add(new NameValue("Sell Value", GameInfo.SellValueOf(material).ToString() + "c"));

            name = material.ToString().AddSpaces();
            desc = Utilities.GetDescOf(material);

            Position();
        }

        
    }

    public class ProjectileInfoHover : ItemInfoHover
    {
        public ProjectileInfoHover(ProjectileType proj, SpriteFont font, GraphicsDevice graphics, int windowWidth)
            : base(graphics, font, proj.ToString().AddSpaces(), Utilities.GetDescOf(proj),
                  new List<NameValue>(), windowWidth)
        {
            stats.Add(new NameValue("Damage", Utilities.DamageOf(proj).ToString()));
            stats.Add(new NameValue("Speed", Utilities.SpeedOf(proj).ToString()));
            stats.Add(new NameValue("Sell Value", GameInfo.SellValueOf(proj).ToString() + "c"));
            stats.Add(new NameValue("", "")); // Ensures that Effects stat is on a new line
            stats.Add(new NameValue("Effects", Utilities.TextEffectsOf(proj)));
            for (int i = 0; i < stats.Count; i++)
            {
                statLocs.Add(new Vector2());
            }

            Position();
        }

        public void ResetProjectile(ProjectileType proj)
        {
            stats.Clear();
            stats.Add(new NameValue("Damage", Utilities.DamageOf(proj).ToString()));
            stats.Add(new NameValue("Speed", Utilities.SpeedOf(proj).ToString()));
            stats.Add(new NameValue("Sell Value", GameInfo.SellValueOf(proj).ToString() + "c"));
            stats.Add(new NameValue("", ""));
            stats.Add(new NameValue("Effects", Utilities.TextEffectsOf(proj)));

            name = proj.ToString().AddSpaces();
            desc = Utilities.GetDescOf(proj);

            Position();
        }
    }

    public class CannonInfoHover : ItemInfoHover
    {
        public CannonInfoHover(CannonSettings cannon, SpriteFont font, GraphicsDevice graphics, int windowWidth)
            : base(graphics, font, cannon.CannonType.ToString().AddSpaces() + " Cannon",
                Utilities.GetDescOf(cannon.CannonType), new List<NameValue>(), windowWidth)
        {
            stats = new List<NameValue>()
            {
                new NameValue("Damage", cannon.Damage.ToString()),
                new NameValue("Health", cannon.MaxHealth.ToString()),
                new NameValue("Reload Speed", cannon.ReloadSpeed.ToString()),
                new NameValue("Move Speed", cannon.MoveSpeed.ToString()),
                new NameValue("Accuracy", cannon.Accuracy.ToString()),
                new NameValue("Power", cannon.Power.ToString()),
                new NameValue("Defense", cannon.Defense.ToString()),
                new NameValue("Rapid Fire", cannon.RapidFire > 0 ? "Y" : "N"),
            };
            if (cannon.Freezes)
            {
                stats.Add(new NameValue("Effect Added", "Frozen"));
            }
            else
            {
                stats.Add(new NameValue("Effect Added", cannon.EffectAdded.ToString().AddSpaces()));
            }

            for (int i = 0; i < stats.Count; i++)
            {
                statLocs.Add(new Vector2());
            }

            Position();
        }

        public void ResetCannon(CannonSettings cannon)
        {
            name = cannon.CannonType.ToString().AddSpaces() + " Cannon";
            desc = Utilities.GetDescOf(cannon.CannonType);

            stats.Clear();
            stats = new List<NameValue>()
            {
                new NameValue("Damage", cannon.Damage.ToString()),
                new NameValue("Health", cannon.MaxHealth.ToString()),
                new NameValue("Reload Speed", cannon.ReloadSpeed.ToString()),
                new NameValue("Move Speed", cannon.MoveSpeed.ToString()),
                new NameValue("Accuracy", cannon.Accuracy.ToString()),
                new NameValue("Rapid Fire", cannon.RapidFire > 0 ? "Y" : "N"),
            };
            if (cannon.Freezes)
            {
                stats.Add(new NameValue("Effect Added", "Frozen"));
            }
            else
            {
                stats.Add(new NameValue("Effect Added", cannon.EffectAdded.ToString().AddSpaces()));
            }
        }
    }

    public class StatInfoHover : ItemInfoHover
    {
        public StatInfoHover(CannonStats stat, SpriteFont font, GraphicsDevice graphics, int windowWidth)
            : base(graphics, font, stat.ToString().AddSpaces(), Utilities.GetDescOf(stat),
                  new List<NameValue>(), windowWidth)
        {
        }

        public void ResetStat(CannonStats stat)
        {
            name = stat.ToString().AddSpaces();
            desc = Utilities.GetDescOf(stat);
        }
    }

    public struct NameValue
    {
        public string Name;
        public string Value;

        public NameValue(string name, string value)
        {
            Name = name;
            Value = value;
        }

        public override string ToString()
        {
            return Name + ": " + Value;
        }
    }
}
