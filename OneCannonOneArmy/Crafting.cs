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
    public delegate void OnCraft(List<Material> materials, List<int> materialCounts, Projectile output);

    [Serializable]
    public struct MaterialInventory
    {
        public List<Material> Items;
        public List<int> Counts;

        public MaterialInventory(List<Material> items, List<int> counts)
        {
            Items = items;
            Counts = counts;
        }

        public void AddItem(Material item, int count)
        {
            if (!Items.Contains(item))
            {
                Items.Add(item);
                Counts.Add(count);
            }
            else
            {
                int i = Items.IndexOf(item);
                Counts[i] += count;
            }
        }
        public int GetItemCount(Material item)
        {
            if (Items.Contains(item))
            {
                return Counts[Items.IndexOf(item)];
            }
            return 0;
        }
        public bool ContainsItem(Material item)
        {
            return Items.Contains(item);
        }
        public void RemoveItem(Material item, int amount)
        {
            if (Items.Contains(item))
            {
                int i = Items.IndexOf(item);
                Counts[i] -= amount;
                if (Counts[i] <= 0)
                {
                    Counts.RemoveAt(i);
                    Items.RemoveAt(i);
                }
            }
        }
    }

    public struct CraftingRecipe
    {
        public ProjectileType Result;
        public List<Material> Materials;
        public List<int> MaterialAmounts;

        public CraftingRecipe(List<Material> materials, List<int> amounts, ProjectileType result)
        {
            Result = result;
            Materials = materials;
            MaterialAmounts = amounts;
        }

        #region Recipes

        public static CraftingRecipe Rock
        {
            get
            {
                List<Material> materials = new List<Material>()
                {
                    Material.Stone
                };
                List<int> counts = new List<int>()
                {
                    2
                };
                return new CraftingRecipe(materials, counts, ProjectileType.Rock);
            }
        }
        public static CraftingRecipe Cannonball
        {
            get
            {
                List<Material> materials = new List<Material>()
                {
                    Material.Metal
                };
                List<int> counts = new List<int>()
                {
                    5
                };
                return new CraftingRecipe(materials, counts, ProjectileType.Cannonball);
            }
        }
        public static CraftingRecipe Fireball
        {
            get
            {
                List<Material> materials = new List<Material>()
                {
                    Material.EssenceOfFire,
                };
                List<int> counts = new List<int>()
                {
                    5,
                };
                return new CraftingRecipe(materials, counts, ProjectileType.Fireball);
            }
        }
        public static CraftingRecipe Bomb
        {
            get
            {
                List<Material> materials = new List<Material>()
                {
                    Material.Metal,
                    Material.Gunpowder
                };
                List<int> counts = new List<int>()
                {
                    5,
                    2
                };
                return new CraftingRecipe(materials, counts, ProjectileType.Bomb);
            }
        }
        public static CraftingRecipe Dart
        {
            get
            {
                List<Material> materials = new List<Material>()
                {
                    Material.Stone,
                    Material.Metal
                };
                List<int> counts = new List<int>()
                {
                    2,
                    2
                };
                return new CraftingRecipe(materials, counts, ProjectileType.Dart);
            }
        }
        public static CraftingRecipe PoisonDart
        {
            get
            {
                List<Material> materials = new List<Material>()
                {
                    Material.Stone,
                    Material.Metal,
                    Material.PlantMatter
                };
                List<int> counts = new List<int>()
                {
                    2,
                    2,
                    2
                };
                return new CraftingRecipe(materials, counts, ProjectileType.PoisonDart);
            }
        }
        public static CraftingRecipe Laser
        {
            get
            {
                List<Material> materials = new List<Material>()
                {
                    Material.Plasma,
                    Material.ChaosEnergy
                };
                List<int> counts = new List<int>()
                {
                    7,
                    2
                };
                return new CraftingRecipe(materials, counts, ProjectileType.Laser);
            }
        }
        public static CraftingRecipe Hex
        {
            get
            {
                List<Material> materials = new List<Material>()
                {
                    Material.ChaosEnergy
                };
                List<int> counts = new List<int>()
                {
                    5
                };
                return new CraftingRecipe(materials, counts, ProjectileType.Hex);
            }
        }
        public static CraftingRecipe LightningBolt
        {
            get
            {
                List<Material> materials = new List<Material>()
                {
                    Material.EssenceOfFire,
                    Material.Gunpowder,
                };
                List<int> counts = new List<int>()
                {
                    5,
                    4,
                };
                return new CraftingRecipe(materials, counts, ProjectileType.LightningBolt);
            }
        }
        public static CraftingRecipe FrozenBlast
        {
            get
            {
                List<Material> materials = new List<Material>()
                {
                    Material.Ice,
                };
                List<int> counts = new List<int>()
                {
                    6,
                };
                return new CraftingRecipe(materials, counts, ProjectileType.FrostHex);
            }
        }
        public static CraftingRecipe Meteor
        {
            get
            {
                List<Material> materials = new List<Material>()
                {
                    Material.Stone,
                    Material.Metal,
                    Material.EssenceOfFire
                };
                List<int> counts = new List<int>()
                {
                    7,
                    1,
                    2
                };
                return new CraftingRecipe(materials, counts, ProjectileType.Meteor);
            }
        }
        public static CraftingRecipe Hammer
        {
            get
            {
                List<Material> materials = new List<Material>()
                {
                    Material.Metal
                };
                List<int> counts = new List<int>()
                {
                    6
                };
                return new CraftingRecipe(materials, counts, ProjectileType.Hammer);
            }
        }
        public static CraftingRecipe Rocket
        {
            get
            {
                List<Material> materials = new List<Material>()
                {
                    Material.Metal,
                    Material.Gunpowder,
                };
                List<int> counts = new List<int>()
                {
                    5,
                    2
                };
                return new CraftingRecipe(materials, counts, ProjectileType.Rocket);
            }
        }
        public static CraftingRecipe PoisonRocket
        {
            get
            {
                List<Material> materials = new List<Material>()
                {
                    Material.Metal,
                    Material.Gunpowder,
                    Material.PlantMatter
                };
                List<int> counts = new List<int>()
                {
                    5,
                    2,
                    1
                };
                return new CraftingRecipe(materials, counts, ProjectileType.PoisonRocket);
            }
        }
        public static CraftingRecipe FireRocket
        {
            get
            {
                List<Material> materials = new List<Material>()
                {
                    Material.Metal,
                    Material.Gunpowder,
                    Material.EssenceOfFire
                };
                List<int> counts = new List<int>()
                {
                    5,
                    2,
                    1
                };
                return new CraftingRecipe(materials, counts, ProjectileType.FireRocket);
            }
        }
        public static CraftingRecipe FrozenRocket
        {
            get
            {
                List<Material> materials = new List<Material>()
                {
                    Material.Metal,
                    Material.Gunpowder,
                    Material.Ice
                };
                List<int> counts = new List<int>()
                {
                    5,
                    2,
                    1
                };
                return new CraftingRecipe(materials, counts, ProjectileType.FrozenRocket);
            }
        }
        public static CraftingRecipe PlasmaRocket
        {
            get
            {
                List<Material> materials = new List<Material>()
                {
                    Material.Metal,
                    Material.Gunpowder,
                    Material.Plasma
                };
                List<int> counts = new List<int>()
                {
                    5,
                    2,
                    1
                };
                return new CraftingRecipe(materials, counts, ProjectileType.PlasmaRocket);
            }
        }
        public static CraftingRecipe OmegaRocket
        {
            get
            {
                List<Material> materials = new List<Material>()
                {
                    Material.Metal,
                    Material.Gunpowder,
                    Material.PlantMatter,
                    Material.EssenceOfFire,
                    Material.Ice,
                    Material.Plasma
                };
                List<int> counts = new List<int>()
                {
                    5,
                    2,
                    1,
                    1,
                    1,
                    1
                };
                return new CraftingRecipe(materials, counts, ProjectileType.OmegaRocket);
            }
        }
        public static CraftingRecipe Snowball
        {
            get
            {
                List<Material> materials = new List<Material>()
                {
                    Material.Ice
                };
                List<int> counts = new List<int>()
                {
                    5
                };
                return new CraftingRecipe(materials, counts, ProjectileType.Snowball);
            }
        }
        public static CraftingRecipe Shuriken
        {
            get
            {
                List<Material> materials = new List<Material>()
                {
                    Material.Metal
                };
                List<int> counts = new List<int>()
                {
                    4
                };
                return new CraftingRecipe(materials, counts, ProjectileType.Shuriken);
            }
        }
        public static CraftingRecipe Bone
        {
            get
            {
                List<Material> materials = new List<Material>()
                {
                    Material.Stone
                };
                List<int> counts = new List<int>()
                {
                    5
                };
                return new CraftingRecipe(materials, counts, ProjectileType.Bone);
            }
        }
        public static CraftingRecipe IceShard
        {
            get
            {
                List<Material> materials = new List<Material>()
                {
                    Material.Ice
                };
                List<int> counts = new List<int>()
                {
                    7,
                };
                return new CraftingRecipe(materials, counts, ProjectileType.IceShard);
            }
        }
        public static CraftingRecipe AbsorbHex
        {
            get
            {
                List<Material> materials = new List<Material>()
                {
                    Material.PlantMatter
                };
                List<int> counts = new List<int>()
                {
                    3,
                };
                return new CraftingRecipe(materials, counts, ProjectileType.AbsorbHex);
            }
        }

        #endregion

        public static CraftingRecipe GetRecipeForProj(ProjectileType p)
        {
            switch (p)
            {
                case ProjectileType.Rock:
                    return Rock;
                case ProjectileType.Cannonball:
                    return Cannonball;
                case ProjectileType.Fireball:
                    return Fireball;
                case ProjectileType.Dart:
                    return Dart;
                case ProjectileType.PoisonDart:
                    return PoisonDart;
                case ProjectileType.Bomb:
                    return Bomb;
                case ProjectileType.LightningBolt:
                    return LightningBolt;
                case ProjectileType.Hex:
                    return Hex;
                case ProjectileType.Laser:
                    return Laser;
                case ProjectileType.FrostHex:
                    return FrozenBlast;
                case ProjectileType.Meteor:
                    return Meteor;
                case ProjectileType.Hammer:
                    return Hammer;
                case ProjectileType.Rocket:
                    return Rocket;
                case ProjectileType.FireRocket:
                    return FireRocket;
                case ProjectileType.PoisonRocket:
                    return PoisonRocket;
                case ProjectileType.FrozenRocket:
                    return FrozenRocket;
                case ProjectileType.PlasmaRocket:
                    return PlasmaRocket;
                case ProjectileType.OmegaRocket:
                    return OmegaRocket;
                case ProjectileType.Snowball:
                    return Snowball;
                case ProjectileType.Shuriken:
                    return Shuriken;
                case ProjectileType.Bone:
                    return Bone;
                case ProjectileType.IceShard:
                    return IceShard;
                case ProjectileType.AbsorbHex:
                    return AbsorbHex;
            }
            return new CraftingRecipe(new List<Material>(), new List<int>(), ProjectileType.None);
        }
    }

    public static class Crafting
    {
        public static bool CanCraft(MaterialInventory inventory, CraftingRecipe craftingRecipe)
        {
            bool can = true;
            MaterialInventory recipe = new MaterialInventory(craftingRecipe.Materials,
                craftingRecipe.MaterialAmounts);
            for (int i = 0; i < craftingRecipe.Materials.Count; i++)
            {
                if (!(inventory.GetItemCount(craftingRecipe.Materials[i]) >=
                    recipe.GetItemCount(craftingRecipe.Materials[i])))
                {
                    // We don't have enough of a certain material, and so we set the return value to false
                    can = false;
                }
            }
            return can;
        }
    }

    public class CraftingInterface
    {
        #region Fields & Properties

        Texture2D bgImg;
        Rectangle bgRect;

        List<Material> materials = new List<Material>();
        List<Texture2D> materialImgs = new List<Texture2D>();
        List<Rectangle> materialRects = new List<Rectangle>();
        List<int> materialCounts = new List<int>();
        List<Vector2> materialCountLocations = new List<Vector2>();

        MenuButton craftButton;

        public Projectile Output;

        SpriteFont font;

        const int MATERIAL_SIZE = 25;
        const int SPACING = 4;

        OnCraft onCraft;

        public bool HoveringOnItem = false;

        public int X
        {
            get
            {
                return bgRect.X;
            }
            set
            {
                bgRect.X = value;
                Output.X = bgRect.X + (bgRect.Width / 2 - (Output.Width / 2));
                UpdateMaterialPositions();
                craftButton.X = bgRect.X + (bgRect.Width / 2 - (craftButton.Width / 2));
                youHavePos.X = bgRect.X + (bgRect.Width / 2 - (font.MeasureString(youHave).X / 2));
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
                Output.Y = bgRect.Y + SPACING;
                UpdateMaterialPositions();
                craftButton.Y = (bgRect.Y + bgRect.Height) - (craftButton.Height + SPACING);
                youHavePos.Y = craftButton.Y - (font.MeasureString(youHave).Y);
            }
        }
        public int Width
        {
            get
            {
                return bgRect.Width;
            }
        }
        public int Height
        {
            get
            {
                return bgRect.Height;
            }
        }

        public bool Visible = true;

        GraphicsDevice graphics;

        string youHave;
        Vector2 youHavePos;

        #endregion

        #region Constructors

        public CraftingInterface(CraftingRecipe recipe, Projectile output, SpriteFont font, int x, int y,
            int width, int height, OnCraft onCraft, GraphicsDevice graphics)
        {
            Output = output;
            this.font = font;
            this.onCraft = onCraft;
            this.graphics = graphics;

            bgImg = new Texture2D(graphics, width, height);
            bgImg = DrawHelper.AddBorder(bgImg, 3, Color.Gray, Color.LightGray);
            bgRect = new Rectangle(x, y, width, height);

            Output.X = bgRect.X + (bgRect.Width / 2 - (output.Width / 2));
            Output.Y = bgRect.Y + SPACING;
            if (Output.Height > Utilities.LightningHeight * 2)
            {
                Output.Height = Utilities.LightningHeight * 2;
            }

            materials = recipe.Materials;
            materialCounts = recipe.MaterialAmounts;

            int lastXMaterials = bgRect.X + SPACING;
            for (int i = 0; i < recipe.Materials.Count; i++)
            {
                materialImgs.Add(Utilities.GetImgOfMaterial(recipe.Materials[i]));
                materialRects.Add(new Rectangle(lastXMaterials, output.Y + output.Height + SPACING,
                    MATERIAL_SIZE, MATERIAL_SIZE));
                materialCountLocations.Add(new Vector2(materialRects[i].Right - SPACING,
                    materialRects[i].Bottom - SPACING));
                lastXMaterials += materialRects[i].Width + (SPACING * 3);
            }

            craftButton = new MenuButton(Craft, Language.Translate("Craft"), 0, 0, true, font, graphics);
            Reposition();

            youHave = Language.Translate("You Have") + ": 0";
            youHavePos = new Vector2(bgRect.X + (bgRect.Width / 2 - (font.MeasureString(youHave).X / 2)),
                craftButton.Y - (font.MeasureString(youHave).Y));
        }

        #endregion

        #region Public Methods

        public List<MenuButton> GetButtons()
        {
            return new List<MenuButton>() { craftButton };
        }

        public CraftingInterface Clone()
        {
            CraftingInterface clone = new CraftingInterface(new CraftingRecipe(materials, materialCounts, Output.Type), Output,
                font, X, Y, Width, Height, onCraft, graphics);
            clone.Visible = this.Visible;
            return clone;
        }

        public void Update(User user, List<Projectile> projectiles)
        {
            craftButton.Active = Crafting.CanCraft(user.MaterialInventory,
                new CraftingRecipe(materials, materialCounts, Output.Type));
            craftButton.Update();

            youHave = Language.Translate("You Have") + ": " + GameInfo.ProjListToTypes(projectiles).CountOf(Output.Type);
            youHavePos.X = bgRect.X + (bgRect.Width / 2 - (font.MeasureString(youHave).X / 2));

            HoveringOnItem = Output.Rectangle.Intersects(new Rectangle(Mouse.GetState().X, Mouse.GetState().Y, 1, 1));
        }

        public void Draw(SpriteBatch spriteBatch, User user)
        {
            if (Visible)
            {
                spriteBatch.Draw(bgImg, bgRect, Color.DarkGray);
                Output.Draw(spriteBatch, false);
                craftButton.Draw(spriteBatch);
                for (int i = 0; i < materialImgs.Count; i++)
                {
                    spriteBatch.Draw(materialImgs[i], materialRects[i], Color.White);

                    Color countColor = Color.White;
                    if (user.AmountOfMaterial(materials[i]) < materialCounts[i])
                    {
                        countColor = Color.Red;
                    }
                    spriteBatch.DrawString(font, "x" + materialCounts[i], materialCountLocations[i], countColor);
                }
                spriteBatch.DrawString(font, youHave, youHavePos, Color.LightGray);
            }
        }

        public void LangChanged()
        {
            craftButton.Text = Language.Translate("Craft");
            Reposition();
        }

        #endregion

        #region Private Methods

        private void Reposition()
        {
            craftButton.X = bgRect.X + (bgRect.Width / 2 - (craftButton.Width / 2));
            craftButton.Y = (bgRect.Y + bgRect.Height) - (craftButton.Height + SPACING);
        }

        private void Craft()
        {
            onCraft(materials, materialCounts, Output);
        }

        private void UpdateMaterialPositions()
        {
            int lastXMaterials = bgRect.X + SPACING;
            for (int i = 0; i < materialRects.Count; i++)
            {
                materialRects[i] = new Rectangle(lastXMaterials, Output.Y + Output.Height + SPACING,
                    MATERIAL_SIZE, MATERIAL_SIZE);
                materialCountLocations[i] = new Vector2(materialRects[i].X,
                    materialRects[i].Bottom - SPACING);
                lastXMaterials += materialRects[i].Width + (SPACING * 3);
            }
        }

        #endregion
    }

    public class CraftingMenu
    {
        #region Fields & Properties

        List<CraftingInterface> interfaces = new List<CraftingInterface>();
        List<List<CraftingInterface>> pages = new List<List<CraftingInterface>>();
        int page = 0;

        ProjectileInfoHover infoHover;

        MenuButton nextButton;
        MenuButton prevButton;

        const int SPACING = 10;
        const int Y_OFFSET = Utilities.MENU_Y_OFFSET;
        int X_OFFSET;

        const int CI_WIDTH = 500;
        const int CI_HEIGHT = 175;

        event OnCraft itemCrafted;

        const int ITEMS_PER_PAGE = 3;

        const int SLIDE_SPEED = 100;
        bool slidingOver;
        int slideOffset;
        int slideSpeed;
        int transitionPage;

        int windowWidth;
        int windowHeight;

        SpriteFont font;
        GraphicsDevice graphics;

        #endregion

        #region Constructors

        public CraftingMenu(GraphicsDevice graphics, SpriteFont mediumFont, SpriteFont smallFont, Texture2D arrowImg, int windowWidth,
            int windowHeight, List<Projectile> projectiles)
        {
            this.windowWidth = windowWidth;
            this.windowHeight = windowHeight;
            font = mediumFont;
            this.graphics = graphics;

            nextButton = new MenuButton(NextPage, 0, 0, true, graphics, arrowImg);
            nextButton.X = windowWidth - nextButton.Width - SPACING;
            nextButton.Y = windowHeight - nextButton.Height - SPACING;
            prevButton = new MenuButton(PrevPage, SPACING, nextButton.Y, true, graphics, arrowImg);

            for (int i = 0; i < projectiles.Count; i++)
            {
                interfaces.Add(new CraftingInterface(CraftingRecipe.GetRecipeForProj(projectiles[i].Type),
                    projectiles[i], mediumFont, 0, 0, CI_WIDTH, CI_HEIGHT,
                    OnCraft, graphics));
            }
            UpdatePagesToInterfaces();

            X_OFFSET = windowWidth / 2 - (interfaces[0].Width / 2);

            infoHover = new ProjectileInfoHover(ProjectileType.None, smallFont, graphics, windowWidth);
        }

        #endregion

        #region Public Methods

        public void AddOnCraftHandler(OnCraft handler)
        {
            itemCrafted += handler;
        }

        public void Initialize(User user)
        {
            for (int i = 0; i < interfaces.Count; i++)
            {
                interfaces[i].Visible = GameInfo.CanSee(user, interfaces[i].Output.Type);
            }
            UpdatePagesToInterfaces();
            UpdatePositions(0, user, 0, null);
        }

        public void Update(User user, List<Projectile> projectiles)
        {
            if (slideOffset + X_OFFSET >= windowWidth || slideOffset + (X_OFFSET * -1) <= windowWidth * -1)
            {
                slideOffset = 0;
                slidingOver = false;
                page = transitionPage;
            }

            bool active = false;
            for (int i = 0; i < pages[page].Count; i++)
            {
                if (pages[page][i].HoveringOnItem && pages[page][i].Visible)
                {
                    active = true;
                    infoHover.ResetProjectile(pages[page][i].Output.Type);
                    infoHover.Update();
                }
            }
            infoHover.Active = active;

            if (page <= pages.Count - 1)
            {
                if (slidingOver)
                {
                    int offset = 0;
                    if (slideSpeed < 0)
                    {
                        offset = interfaces[0].Width + X_OFFSET;
                    }
                    else
                    {
                        offset = (interfaces[0].Width + X_OFFSET) * -1;
                    }
                    UpdatePositions(transitionPage, user, offset, projectiles);
                }

                UpdatePositions(page, user, 0, projectiles);
            }

            for (int i = 0; i <= pages.Count - 1; i++)
            {
                if (pages[i].Count <= 0)
                {
                    pages.RemoveAt(i);
                }
            }

            if (pages.Count <= 1)
            {
                // Both buttons should be disabled, as there is only 1 page
                // This must be tested for first, otherwise, since the page always starts equal to 0, the below criteria will be met
                // and the game will act like there are 2 pages when there is only 1
                nextButton.Active = false;
                prevButton.Active = false;
                // Since there is only one page, we have to set the page integer to 0
                page = 0;
            }
            else if (page == 0)
            {
                // There is another page, but we are on the first. Therefore, we cannot go backwards, so we'll disable the backwards button
                nextButton.Active = true;
                prevButton.Active = false;
            }
            else if (page + 1 == pages.Count)
            {
                // We are on the last page, so the forward button must be disabled
                nextButton.Active = false;
                prevButton.Active = true;
            }
            else
            {
                // We must be somewhere in the middle, so both buttons should be enabled
                nextButton.Active = true;
                prevButton.Active = true;
            }

            if (pages.Count > page + 1)
            {
                // If the next page doesn't contain any unlocked projectiles
                if (!pages[page + 1].Exists(x => x.Visible))
                {
                    nextButton.Active = false;
                }
            }

            if (slidingOver)
            {
                slideOffset += slideSpeed;
            }

            nextButton.Update();
            prevButton.Update();
        }

        public void Draw(SpriteBatch spriteBatch, User user)
        {
            nextButton.Draw(spriteBatch);
            prevButton.Draw(spriteBatch, null, SpriteEffects.FlipHorizontally);

            foreach (CraftingInterface c in pages[page])
            {
                c.Draw(spriteBatch, user);
            }

            if (slidingOver)
            {
                for (int i = 0; i <= pages[transitionPage].Count - 1; i++)
                {
                    pages[transitionPage][i].Draw(spriteBatch, user);
                }
            }

            if (infoHover.Active)
            {
                infoHover.Draw(spriteBatch);
            }
        }

        public List<MenuButton> GetButtons()
        {
            List<MenuButton> returnVal = new List<MenuButton>();

            foreach (CraftingInterface c in pages[page])
            {
                returnVal.AddRange(c.GetButtons());
            }

            returnVal.Add(nextButton);
            returnVal.Add(prevButton);

            return returnVal;
        }

        public void LangChanged()
        {
            for (int i = 0; i < interfaces.Count; i++)
            {
                interfaces[i].LangChanged();
            }
            UpdatePagesToInterfaces();
        }

        public void AddItem(CraftingRecipe item, Projectile output)
        {
            interfaces.Add(new CraftingInterface(item, output, font, 0, 0, CI_WIDTH, CI_HEIGHT,
                OnCraft, graphics));
            UpdatePagesToInterfaces();
        }

        #endregion

        #region Private Methods

        private void NextPage()
        {
            transitionPage = page + 1;
            slidingOver = true;
            slideSpeed = SLIDE_SPEED * -1;
        }
        private void PrevPage()
        {
            transitionPage = page - 1;
            slidingOver = true;
            slideSpeed = SLIDE_SPEED;
        }

        private void OnCraft(List<Material> materials, List<int> counts, Projectile output)
        {
            itemCrafted(materials, counts, output);
        }

        private void UpdatePositions(int page, User user, int xOffset, List<Projectile> projectiles)
        {
            int lastY = Y_OFFSET;
            foreach (CraftingInterface c in pages[page])
            {
                c.X = xOffset + slideOffset + X_OFFSET;
                c.Y = lastY;
                lastY += SPACING + c.Height;
                c.Visible = GameInfo.CanSee(user, c.Output.Type);
                if (projectiles != null)
                {
                    c.Update(user, projectiles);
                }
            }
        }

        private void AddItem(CraftingInterface interfaceToAdd)
        {
            // This gets the currently used page and adds to it. 
            // Unfortunately, we need to add extra code to check if the latest page is full.
            if (pages.Count > 0)
            {
                for (int i = 0; i < pages.Count; i++)
                {
                    if (pages[i].Count < ITEMS_PER_PAGE)
                    {
                        pages[i].Add(interfaceToAdd);
                        break;
                    }
                    else
                    {
                        pages.Add(new List<CraftingInterface>());
                        continue;
                    }
                }
            }
            else // pages.Count <= 0
            {
                pages.Add(new List<CraftingInterface>());
                pages[0].Add(interfaceToAdd);
            }
        }

        private void UpdatePagesToInterfaces()
        {
            pages.Clear();

            for (int i = 0; i < interfaces.Count; i++)
            {
                AddItem(interfaces[i].Clone());
            }
        }

        #endregion
    }

    public enum Material
    {
        Stone,
        Metal,
        PlantMatter,
        Gunpowder,
        Ice,
        EssenceOfFire,
        Plasma,
        ChaosEnergy,
    }
}