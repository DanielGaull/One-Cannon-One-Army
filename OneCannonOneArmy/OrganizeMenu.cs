using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework.Input;

namespace OneCannonOneArmy
{
    public class OrganizeMenu
    {
        #region Fields & Properties

        MenuButton materialsButton;
        MenuButton projsButton;
        MenuButton hotbarButton;
        MenuButton collectionButton;

        SpriteFont countFont;

        List<Material> materials = new List<Material>();
        List<Texture2D> materialImgs = new List<Texture2D>();
        List<Rectangle> materialRects = new List<Rectangle>();
        List<int> materialCounts = new List<int>();
        List<Vector2> materialCountPositions = new List<Vector2>();
        MaterialInfoHover materialInfoHover;
        const int MATERIAL_ROWS = 2;

        List<Badge> badges = new List<Badge>();
        List<Texture2D> badgeImgs = new List<Texture2D>();
        List<Rectangle> badgeRects = new List<Rectangle>();
        List<int> badgeCounts = new List<int>();
        List<Vector2> badgeCountPositions = new List<Vector2>();

        const int ITEM_SIZE = 100;
        const int TROPHY_WIDTH = 75;

        List<ProjectileType> projectiles = new List<ProjectileType>();
        List<Texture2D> projImgs = new List<Texture2D>();
        List<Rectangle> projRects = new List<Rectangle>();
        List<int> projCounts = new List<int>();
        List<Vector2> projCountPositions = new List<Vector2>();
        ProjectileInfoHover projInfoHover;
        // We'll have to implement scrolling for projectiles
        Rectangle topScrollRect;
        Rectangle bottomScrollRect;
        Rectangle scrollBarRect;
        bool needsScroll = false;
        float scrollOffset = 0.0f;
        int WANTED_MAX_AREA_HEIGHT;
        int ON_SCREEN_MAX_AREA_HEIGHT;
        const float MAX_AREA_PERCENT = 0.8f;
        const int MIN_SCROLLBAR_HEIGHT = 20;
        // Measures the number of pixels actually scrolled per number of 
        // pixels the scrollbar is dragged
        // Generally 1 unless the content area is greater than the maximum area,
        // thus making the scroll bar the minimum height
        float scrollPerPixel = 1.0f;
        float prevScrollVal;
        const int SCROLL_DIVISION = 10;

        List<HotbarSlotInteractive> hotbarSlots = new List<HotbarSlotInteractive>();
        List<DraggableProjectile> projDraggables = new List<DraggableProjectile>();

        OrganizeState state = OrganizeState.Materials;

        const int SPACING = 14;
        int X_OFFSET;
        const int Y_OFFSET = Utilities.MENU_Y_OFFSET - SPACING;

        Texture2D gridImg;
        List<Rectangle> grid = new List<Rectangle>();
        const int GRID_ROWS = 4;
        const int ITEMS_PER_ROW = 7;
        const int GRID_BORDER_WIDTH = 1;

        const int BUTTON_SIZE = 60;
        const int BUTTON_INT_SPACING = 6;

        RClickDropdown dropdown;
        GraphicsDevice graphics;

        SpriteFont mediumFont;

        Action<Material> mSell;
        Action<ProjectileType> pSell;
        Action<Badge> bSell;
        Material activeMaterial;
        ProjectileType activeProj;
        Badge activeBadge;

        #endregion

        #region Constructors

        public OrganizeMenu(SpriteFont smallFont, SpriteFont bigFont, SpriteFont mediumFont, GraphicsDevice graphics,
            int windowWidth, int windowHeight, Action<Material> mSell, Action<ProjectileType> pSell, Action<Badge> bSell)
        {
            this.graphics = graphics;
            this.mSell = mSell;
            this.pSell = pSell;
            this.bSell = bSell;
            this.mediumFont = mediumFont;

            WANTED_MAX_AREA_HEIGHT = (int)(windowWidth * MAX_AREA_PERCENT);

            countFont = smallFont;

            X_OFFSET = windowWidth / 2 - (((ITEM_SIZE + SPACING) * ITEMS_PER_ROW) / 2);

            gridImg = new Texture2D(graphics, ITEM_SIZE + SPACING, ITEM_SIZE + SPACING);
            gridImg = DrawHelper.AddBorder(gridImg, GRID_BORDER_WIDTH, Color.Black, Color.Transparent);

            //hotbarBorderImg = new Texture2D(graphics, ITEM_SIZE + SPACING, ITEM_SIZE + SPACING);
            //hotbarBorderImg = DrawHelper.AddBorder(hotbarBorderImg, 3, Color.Black, Color.Transparent);
            //int lastX = windowWidth / 2 - (((hotbarBorderImg.Width + SPACING) * GameInfo.HOTBAR_SLOTS) / 2);
            //for (int i = 0; i < GameInfo.HOTBAR_SLOTS; i++)
            //{
            //    hotbarRects.Add(new Rectangle(lastX, Y_OFFSET, hotbarBorderImg.Width, hotbarBorderImg.Height));
            //    lastX += hotbarRects[i].Width + SPACING;
            //    hotbarFullVals.Add(false);
            //}

            int size = ITEM_SIZE + SPACING;
            int lastX = windowWidth / 2 - (((size + SPACING) * GameInfo.HOTBAR_SLOTS) / 2);
            for (int i = 0; i < GameInfo.HOTBAR_SLOTS; i++)
            {
                hotbarSlots.Add(new HotbarSlotInteractive(graphics, lastX, Y_OFFSET, size, size));
                lastX += hotbarSlots[i].Width + SPACING;
            }

            projsButton = new MenuButton(new System.Action(() => state = OrganizeState.Projectiles), 0, 0, true,
                graphics, Utilities.RockImg);
            projsButton.ImgWidth = projsButton.ImgHeight = BUTTON_SIZE;
            projsButton.X = windowWidth / 2 - projsButton.Width - SPACING / 2;
            projsButton.Y = windowHeight - projsButton.Height - SPACING / 4;

            materialsButton = new MenuButton(new System.Action(() => state = OrganizeState.Materials), 0, projsButton.Y,
                true, graphics, Utilities.MetalImg);
            materialsButton.ImgWidth = materialsButton.ImgHeight = BUTTON_SIZE;
            materialsButton.X = projsButton.X - materialsButton.Width - SPACING;

            collectionButton = new MenuButton(new System.Action(() => state = OrganizeState.Collection),
                0, projsButton.Y, true, graphics, Utilities.BadgeIcon);
            collectionButton.ImgWidth = (int)(((float)TROPHY_WIDTH / ITEM_SIZE) * BUTTON_SIZE);
            collectionButton.ImgHeight = BUTTON_SIZE;
            collectionButton.Width = collectionButton.Height;
            collectionButton.X = windowWidth / 2 + SPACING / 2;

            hotbarButton = new MenuButton(new System.Action(() => state = OrganizeState.Hotbar), 0, projsButton.Y,
                true, graphics, Utilities.HotbarIcon);
            hotbarButton.ImgWidth = hotbarButton.ImgHeight = BUTTON_SIZE;
            hotbarButton.X = collectionButton.X + collectionButton.Width + SPACING;

            topScrollRect = new Rectangle(0, 0, windowWidth, Y_OFFSET);
            bottomScrollRect = new Rectangle(0, 0, windowWidth, Y_OFFSET);
            bottomScrollRect.Y = windowHeight - bottomScrollRect.Height;
            scrollBarRect = new Rectangle(0, topScrollRect.Bottom, SPACING, 0);
            scrollBarRect.X = windowWidth - scrollBarRect.Width;
            ON_SCREEN_MAX_AREA_HEIGHT = windowHeight - topScrollRect.Height - bottomScrollRect.Height;

            materialInfoHover = new MaterialInfoHover(Material.Stone, smallFont, graphics, windowWidth);
            projInfoHover = new ProjectileInfoHover(ProjectileType.None, smallFont, graphics, windowWidth);
        }

        #endregion

        #region Public Methods

        public void Reset(User user)
        {
            for (int i = 0; i < hotbarSlots.Count; i++)
            {
                hotbarSlots[i].Content = user.Hotbar[i];
            }
        }

        public void Initialize(List<ProjectileType> projectiles, MaterialInventory materials,
            List<ProjectileType> hotbar, List<Badge> badges)
        {
            state = OrganizeState.Materials;

            // Clear out our lists to make sure we start fresh
            this.materials.Clear();
            materialImgs.Clear();
            materialRects.Clear();
            materialCounts.Clear();
            materialCountPositions.Clear();
            this.projectiles.Clear();
            projImgs.Clear();
            projRects.Clear();
            projCounts.Clear();
            projCountPositions.Clear();
            projDraggables.Clear();
            this.badges.Clear();
            badgeImgs.Clear();
            badgeRects.Clear();
            badgeCounts.Clear();
            badgeCountPositions.Clear();
            grid.Clear();

            // Set the hotbar contents to the user's hotbar
            for (int i = 0; i < hotbar.Count; i++)
            {
                hotbarSlots[i].Content = hotbar[i];
            }

            // First initialize the material inventory values
            for (int i = 0; i < materials.Items.Count; i++)
            {
                this.materials.Add(materials.Items[i]);
                materialImgs.Add(Utilities.GetImgOfMaterial(materials.Items[i]));
                materialRects.Add(new Rectangle(0, 0, ITEM_SIZE, ITEM_SIZE));
                materialCounts.Add(materials.Counts[i]);
                materialCountPositions.Add(new Vector2());
            }

            // Then initialize badge inventory values
            var uniqueBadges = badges.Unique();
            for (int i = 0; i < uniqueBadges.Count; i++)
            {
                this.badges.Add(uniqueBadges[i]);
                badgeImgs.Add(Utilities.GetImgOfBadge(uniqueBadges[i]));
                badgeRects.Add(new Rectangle(0, 0, TROPHY_WIDTH, ITEM_SIZE));
                badgeCounts.Add(badges.CountOf(uniqueBadges[i]));
                badgeCountPositions.Add(new Vector2());
            }

            // Now initialize projectile inventory values
            var uniqueProjs = projectiles.Unique();
            for (int i = 0; i < uniqueProjs.Count; i++)
            {
                this.projectiles.Add(uniqueProjs[i]);
                projImgs.Add(Utilities.GetIconOf(uniqueProjs[i]));
                projRects.Add(new Rectangle(0, 0, ITEM_SIZE, ITEM_SIZE));
                projCounts.Add(projectiles.CountOf(uniqueProjs[i]));
                projCountPositions.Add(new Vector2());
            }

            PositionMaterials();
            PositionProjectiles();
            PositionBadges();

            // With everything positioned, now initialize hotbar values
            for (int i = 0; i < uniqueProjs.Count; i++)
            {
                DraggableProjectile newDraggable = new DraggableProjectile(projImgs[i], 0,
                    0, projRects[i].Width, projRects[i].Height, uniqueProjs[i],
                    projectiles.CountOf(uniqueProjs[i]), countFont);
                newDraggable.AddOnMouseReleaseHandler(CheckDraggableForGridSnap);
                newDraggable.AddSnapToHotbarHandler(PositionDraggables);
                projDraggables.Add(newDraggable);
            }
            for (int i = 0; i < projDraggables.Count; i++)
            {
                for (int j = 0; j < hotbar.Count; j++)
                {
                    if (projDraggables[i].Content == hotbar[j])
                    {
                        projDraggables[i].SnapTo(hotbarSlots[j]);
                        SetSnapPoints(i, true);
                    }
                }
            }
            PositionDraggables();

            // Add grid
            grid.AddRange(Enumerable.Repeat(new Rectangle(0, 0, ITEM_SIZE + SPACING, ITEM_SIZE + SPACING),
                GRID_ROWS * ITEMS_PER_ROW));
            int lastX = X_OFFSET - (SPACING / 2);
            int lastY = Y_OFFSET + (int)scrollOffset + hotbarSlots[0].Height + SPACING;
            int itemsInRow = 0;
            int row = 0;
            for (int i = 0; i < grid.Count; i++)
            {
                if (itemsInRow >= ITEMS_PER_ROW)
                {
                    row++;
                    itemsInRow = 0;
                    lastY += grid[i].Height - GRID_BORDER_WIDTH;
                    lastX = X_OFFSET - (SPACING / 2);
                }

                grid[i] = new Rectangle(lastX, lastY, grid[i].Width, grid[i].Height);
                lastX += grid[i].Width - GRID_BORDER_WIDTH;
                itemsInRow++;
            }
        }

        public void Update(User user, GameTime gameTime)
        {
            materialsButton.Update(gameTime);
            projsButton.Update(gameTime);
            hotbarButton.Update(gameTime);
            collectionButton.Update(gameTime);

            if (dropdown?.Active == true)
            {
                dropdown.Update(gameTime);
            }

            bool makeActive = false;
            Rectangle mouseRect;
            switch (state)
            {
                case OrganizeState.Materials:

                    mouseRect = new Rectangle(Mouse.GetState().X, Mouse.GetState().Y, 1, 1);
                    makeActive = false;
                    for (int i = 0; i < materialRects.Count; i++)
                    {
                        if (materialRects[i].Intersects(mouseRect))
                        {
                            materialInfoHover.ResetMaterial(materials[i]);
                            makeActive = true;
                            if (Mouse.GetState().RightButton == ButtonState.Pressed)
                            {
                                activeMaterial = materials[i];
                                dropdown = new RClickDropdown(
                                    new List<string> {
                                        Language.Translate("Sell") + " (" + GameInfo.SellValueOf(materials[i]).ToString()
                                            + "c)", Language.Translate("Sell All") + " (" +
                                            GameInfo.SellValueOf(materials[i]) * materialCounts[i] + "c)",
                                    },
                                    new List<System.Action> {
                                        new System.Action(() => mSell(activeMaterial)),
                                        new System.Action(() => MSellAll(activeMaterial)),
                                    },
                                    mediumFont);
                                dropdown.Show(mouseRect.X, mouseRect.Y + 2);
                            }
                            break;
                        }
                    }
                    materialInfoHover.Active = makeActive;
                    if (materialInfoHover.Active)
                    {
                        materialInfoHover.Update();
                    }

                    break;
                case OrganizeState.Collection:
                    mouseRect = new Rectangle(Mouse.GetState().X, Mouse.GetState().Y, 1, 1);
                    makeActive = false;
                    for (int i = 0; i < badgeRects.Count; i++)
                    {
                        if (badgeRects[i].Intersects(mouseRect) && Mouse.GetState().RightButton == ButtonState.Pressed)
                        {
                            activeBadge = badges[i];
                            dropdown = new RClickDropdown(
                                new List<string> {
                                    Language.Translate("Sell") + " (" + GameInfo.SellValueOf(badges[i]).ToString() + "c)",
                                    Language.Translate("Sell All") + " (" +
                                            GameInfo.SellValueOf(badges[i]) * badgeCounts[i] + "c)",},
                                new List<System.Action> {
                                    new System.Action(() => bSell(activeBadge)),
                                    new System.Action(() => BSellAll(activeBadge)),
                                },
                                mediumFont);
                            dropdown.Show(mouseRect.X, mouseRect.Y + 2);
                            break;
                        }
                    }
                    break;
                case OrganizeState.Projectiles:

                    // Scroll by dragging scroll bar
                    // Code not needed as there aren't enough projectiles (in 0.5.0)

                    //MouseState mouse = Mouse.GetState();
                    //if (scrollBarRect.Intersects(new Rectangle(mouse.X, mouse.Y, 1, 1)) && 
                    //    mouse.LeftButton == ButtonState.Pressed)
                    //{
                    //    scrollOffset = mouse.Y - (scrollBarRect.Height / 2);
                    //}

                    //// Scroll by using scroll wheel
                    //if (mouse.ScrollWheelValue != prevScrollVal)
                    //{
                    //    // Scroll value has changed since last update
                    //    scrollOffset += (mouse.ScrollWheelValue - prevScrollVal) / SCROLL_DIVISION;
                    //}
                    //prevScrollVal = mouse.ScrollWheelValue;
                    //PositionProjectiles();

                    mouseRect = new Rectangle(Mouse.GetState().X, Mouse.GetState().Y, 1, 1);
                    for (int i = 0; i < projRects.Count; i++)
                    {
                        if (projRects[i].Intersects(mouseRect))
                        {
                            projInfoHover.ResetProjectile(projectiles[i]);
                            makeActive = true;
                            if (Mouse.GetState().RightButton == ButtonState.Pressed)
                            {
                                activeProj = projectiles[i];
                                dropdown = new RClickDropdown(
                                    new List<string> {
                                        Language.Translate("Sell") + " (" + GameInfo.SellValueOf(projectiles[i]).ToString() + "c)",
                                        Language.Translate("Sell All") + " (" +
                                            GameInfo.SellValueOf(projectiles[i]) * projCounts[i] + "c)",
                                    },
                                    new List<System.Action> {
                                        new System.Action(() => PSell(activeProj)),
                                        new System.Action(() => PSellAll(activeProj)),
                                    },
                                    mediumFont);
                                dropdown.Show(mouseRect.X, mouseRect.Y + 2);
                            }
                            break;
                        }
                    }
                    projInfoHover.Active = makeActive;
                    if (projInfoHover.Active)
                    {
                        projInfoHover.Update();
                    }

                    break;
                case OrganizeState.Hotbar:
                    // Don't use the line of code below, but keep the yOffset value in mind
                    //PositionProjectiles(hotbarRects[0].Height + SPACING);

                    List<ProjectileType> currentHotbar = new List<ProjectileType>();
                    for (int i = 0; i < hotbarSlots.Count; i++)
                    {
                        currentHotbar.Add(hotbarSlots[i].Content);
                    }
                    if (user.Hotbar != currentHotbar)
                    {
                        user.SetHotbar(currentHotbar);
                    }

                    for (int i = 0; i < projDraggables.Count; i++)
                    {
                        projDraggables[i].Update();
                    }
                    break;
            }
        }

        public void Draw(SpriteBatch spriteBatch)
        {
            materialsButton.Draw(spriteBatch);
            projsButton.Draw(spriteBatch);
            hotbarButton.Draw(spriteBatch);
            collectionButton.Draw(spriteBatch);

            switch (state)
            {
                case OrganizeState.Materials:

                    for (int i = 0; i < materialImgs.Count; i++)
                    {
                        spriteBatch.Draw(materialImgs[i], materialRects[i], Color.White);
                        spriteBatch.DrawString(countFont, "x" + materialCounts[i].ToString(),
                            materialCountPositions[i], Color.Black);
                    }
                    if (materialInfoHover.Active)
                    {
                        materialInfoHover.Draw(spriteBatch);
                    }

                    break;
                case OrganizeState.Collection:

                    for (int i = 0; i < badgeImgs.Count; i++)
                    {
                        spriteBatch.Draw(badgeImgs[i], badgeRects[i], Color.White);
                        spriteBatch.DrawString(countFont, "x" + badgeCounts[i].ToString(),
                            badgeCountPositions[i], Color.Black);
                    }

                    break;
                case OrganizeState.Projectiles:

                    for (int i = 0; i < projImgs.Count; i++)
                    {
                        spriteBatch.Draw(projImgs[i], projRects[i], Color.White);
                        spriteBatch.DrawString(countFont, "x" + projCounts[i].ToString(),
                            projCountPositions[i], Color.Black);
                    }
                    if (projInfoHover.Active)
                    {
                        projInfoHover.Draw(spriteBatch);
                    }

                    //if (needsScroll)
                    //{
                    //    spriteBatch.Draw(Utilities.RectImage, topScrollRect, Color.Gray);
                    //    spriteBatch.Draw(Utilities.RectImage, bottomScrollRect, Color.Gray);
                    //    spriteBatch.Draw(Utilities.RectImage, scrollBarRect, Color.DarkGray);
                    //}

                    break;
                case OrganizeState.Hotbar:

                    for (int i = 0; i < grid.Count; i++)
                    {
                        spriteBatch.Draw(gridImg, grid[i], Color.White);
                    }

                    for (int i = 0; i < hotbarSlots.Count; i++)
                    {
                        hotbarSlots[i].Draw(spriteBatch);
                    }

                    // Make sure to draw the draggables on top
                    for (int i = 0; i < projDraggables.Count; i++)
                    {
                        projDraggables[i].Draw(spriteBatch, Color.White);
                    }

                    break;
            }

            if (dropdown?.Active == true)
            {
                dropdown.Draw(spriteBatch);
            }
        }

        public List<MenuButton> GetButtons()
        {
            List<MenuButton> returnVal = new List<MenuButton>();

            returnVal.Add(materialsButton);
            returnVal.Add(projsButton);
            returnVal.Add(hotbarButton);
            returnVal.Add(collectionButton);
            if (dropdown?.Active == true)
            {
                returnVal.AddRange(dropdown.GetButtons());
            }

            return returnVal;
        }

        public void AddMaterial(Material material, int count)
        {
            if (materials.Contains(material))
            {
                int index = materials.IndexOf(material);
                materialCounts[index] += count;
            }
            else
            {
                materials.Add(material);
                materialImgs.Add(Utilities.GetImgOfMaterial(material));
                materialRects.Add(new Rectangle(0, 0, ITEM_SIZE, ITEM_SIZE));
                materialCounts.Add(count);
                materialCountPositions.Add(new Vector2());

                PositionMaterials();
            }
        }
        public void AddProjectile(ProjectileType projectile, int count)
        {
            if (projectiles.Contains(projectile))
            {
                int index = projectiles.IndexOf(projectile);
                projCounts[index] += count;
                projDraggables[index].Count += count;
            }
            else
            {
                projectiles.Add(projectile);
                projImgs.Add(Utilities.GetIconOf(projectile));
                projRects.Add(new Rectangle(0, 0, ITEM_SIZE, ITEM_SIZE));
                projCounts.Add(count);
                projCountPositions.Add(new Vector2());

                PositionProjectiles();

                //projDraggables.Add(new DraggableProjectile(Utilities.GetIconOf(projectile), projRects[projRects.Count - 1], 
                //    projectiles[projectiles.Count - 1]));
                DraggableProjectile newDraggable = new DraggableProjectile(Utilities.GetIconOf(projectile), 0,
                    0, projRects[projRects.Count - 1].Width,
                    projRects[projRects.Count - 1].Height, projectiles[projectiles.Count - 1],
                    projectiles.CountOf(projectiles[projectiles.Count - 1]), countFont);
                newDraggable.AddOnMouseReleaseHandler(CheckDraggableForGridSnap);
                newDraggable.AddSnapToHotbarHandler(PositionDraggables);
                projDraggables.Add(newDraggable);
                PositionDraggables();
            }
        }
        public void AddBadge(Badge badge, int count)
        {
            if (badges.Contains(badge))
            {
                int index = badges.IndexOf(badge);
                badgeCounts[index] += count;
            }
            else
            {
                badges.Add(badge);
                badgeImgs.Add(Utilities.GetImgOfBadge(badge));
                badgeRects.Add(new Rectangle(0, 0, ITEM_SIZE, ITEM_SIZE));
                badgeCounts.Add(count);
                badgeCountPositions.Add(new Vector2());

                PositionBadges();
            }
        }
        public void RemoveBadge(Badge badge, int count)
        {
            if (badges.Contains(badge))
            {
                int index = badges.IndexOf(badge);

                if (badgeCounts[index] > 1)
                {
                    badgeCounts[index] -= count;
                }
                else
                {
                    badges.RemoveAt(index);
                    badgeImgs.RemoveAt(index);
                    badgeRects.RemoveAt(index);
                    badgeCounts.RemoveAt(index);
                    badgeCountPositions.RemoveAt(index);

                    PositionBadges();
                }
            }
        }
        public void RemoveMaterial(Material material, int count)
        {
            if (materials.Contains(material))
            {
                int index = materials.IndexOf(material);
                if (materialCounts[index] - count > 0)
                {
                    // We can simply decrement the count
                    materialCounts[index] -= count;
                }
                else
                {
                    // We only have one of the specified item
                    // We have to remove all variables associated with this material
                    materials.RemoveAt(index);
                    materialImgs.RemoveAt(index);
                    materialRects.RemoveAt(index);
                    materialCounts.RemoveAt(index);
                    materialCountPositions.RemoveAt(index);
                    PositionMaterials();
                }
            }
        }
        public void RemoveProjectile(ProjectileType proj, int count)
        {
            if (projectiles.Contains(proj))
            {
                int index = projectiles.IndexOf(proj);
                if (projCounts[index] > 1)
                {
                    // We can simply decrement the count
                    projCounts[index] -= count;
                    projDraggables[index].Count -= count;
                }
                else
                {
                    // We only have one of the specified item
                    // We have to remove all variables associated with this projectile
                    projectiles.RemoveAt(index);
                    projImgs.RemoveAt(index);
                    projRects.RemoveAt(index);
                    projCounts.RemoveAt(index);
                    projCountPositions.RemoveAt(index);
                    PositionProjectiles();

                    projDraggables.RemoveAt(index);

                    for (int i = 0; i < hotbarSlots.Count; i++)
                    {
                        if (hotbarSlots[i].Content == proj)
                        {
                            // The hotbar slot will say its full when it is in fact empty
                            // Make it empty
                            hotbarSlots[i].Content = ProjectileType.None;
                        }
                    }
                }
            }
        }

        public List<DraggableProjectile> GetDraggables()
        {
            if (state == OrganizeState.Hotbar)
            {
                return projDraggables;
            }
            else
            {
                return new List<DraggableProjectile>();
            }
        }

        #endregion

        #region Private Methods

        private void MSellAll(Material m)
        {
            int count = materialCounts[materials.IndexOf(m)];
            for (int i = 0; i < count; i++)
            {
                mSell(m);
            }
        }
        private void PSell(ProjectileType p)
        {
            pSell(p);
            PositionDraggables();
        }
        private void PSellAll(ProjectileType p)
        {
            int count = projCounts[projectiles.IndexOf(p)];
            for (int i = 0; i < count; i++)
            {
                PSell(p);
            }
        }
        private void BSellAll(Badge b)
        {
            int count = badgeCounts[badges.IndexOf(b)];
            for (int i = 0; i < count; i++)
            {
                bSell(b);
            }
        }

        private void SetSnapPoints(int index, bool includeCurrent)
        {
            //List<Vector2> points = new List<Vector2>();
            //List<Rectangle> rects = new List<Rectangle>();
            //for (int i = 0; i < hotbarRects.Count; i++)
            //{
            //    if (!hotbarFullVals[i])
            //    {
            //        // We have an empty hotbar slot
            //        rects.Add(hotbarRects[i]);
            //    }
            //}

            //projDraggables[projDraggables.Count - 1].SetSnapPoints(
            //        GameInfo.RectsToVectors(rects).AddToAll(SPACING / 2, SPACING / 2), true);

            projDraggables[index].SetSnapPoints(hotbarSlots, includeCurrent);
        }

        private bool CheckDraggableForGridSnap(DraggableProjectile drag)
        {
            // Check if a draggable is below the grid, and is currently in a hotbar slot
            MouseState mouse = Mouse.GetState();
            if (drag.Y >= grid[0].Y && drag.CurrentSlot != null)
            {
                // The draggable is below the grid
                drag.CurrentSlot.SetContent(ProjectileType.None);
                drag.CurrentSlot = null;
                PositionDraggables();
                return true;
            }
            return false;
        }

        private void PositionMaterials()
        {
            int itemsInCurrentRow = 0;
            int lastX = X_OFFSET;
            int lastY = Y_OFFSET;
            for (int i = 0; i < materialRects.Count; i++)
            {
                if (itemsInCurrentRow >= ITEMS_PER_ROW)
                {
                    itemsInCurrentRow = 0;
                    lastX = X_OFFSET;
                    lastY += materialRects[i].Height + SPACING;
                }

                materialRects[i] = new Rectangle(lastX, lastY, materialRects[i].Width, materialRects[i].Height);
                lastX += materialRects[i].Width + SPACING;

                materialCountPositions[i] = new Vector2(materialRects[i].Right - (SPACING / 2),
                    materialRects[i].Bottom - (SPACING / 2));

                itemsInCurrentRow++;
            }
        }
        private void PositionProjectiles()
        {
            int itemsInCurrentRow = 0;
            int lastX = X_OFFSET;
            int lastY = Y_OFFSET + (int)scrollOffset;
            for (int i = 0; i < projRects.Count; i++)
            {
                if (itemsInCurrentRow >= ITEMS_PER_ROW)
                {
                    itemsInCurrentRow = 0;
                    lastX = X_OFFSET;
                    lastY += projRects[i].Height + SPACING;
                    //if (lastY + projRects[i].Height >=  bottomScrollRect.Top && !needsScroll)
                    //{
                    //    // The current projectile is not completely visible
                    //    // and we don't already have the scroll values ready
                    //    CalculateScrollValues();
                    //}
                }

                projRects[i] = new Rectangle(lastX, lastY, projRects[i].Width, projRects[i].Height);
                lastX += projRects[i].Width + SPACING;

                projCountPositions[i] = new Vector2(projRects[i].Right - (SPACING / 2),
                    projRects[i].Bottom - (SPACING / 2));

                itemsInCurrentRow++;
            }
        }
        private void PositionBadges()
        {
            int itemsInCurrentRow = 0;
            int lastX = X_OFFSET;
            int lastY = Y_OFFSET;
            for (int i = 0; i < badgeRects.Count; i++)
            {
                if (itemsInCurrentRow >= ITEMS_PER_ROW)
                {
                    itemsInCurrentRow = 0;
                    lastX = X_OFFSET;
                    lastY += badgeRects[i].Height + SPACING;
                }

                badgeRects[i] = new Rectangle(lastX, lastY, badgeRects[i].Width, badgeRects[i].Height);
                lastX += badgeRects[i].Width + SPACING;

                badgeCountPositions[i] = new Vector2(badgeRects[i].Right - (SPACING / 2),
                    badgeRects[i].Bottom - (SPACING / 2));

                itemsInCurrentRow++;
            }
        }
        private void PositionDraggables()
        {
            int itemsInCurrentRow = 0;
            int lastX = X_OFFSET;
            int lastY = Y_OFFSET + hotbarSlots[0].Height + SPACING;
            for (int i = 0; i < projDraggables.Count; i++)
            {
                // Don't reposition any draggables currently in a hotbar slot
                if (hotbarSlots.Where(x => x.Content == projDraggables[i].Content).Count() <= 0)
                {
                    if (itemsInCurrentRow >= ITEMS_PER_ROW)
                    {
                        itemsInCurrentRow = 0;
                        lastX = X_OFFSET;
                        lastY += projDraggables[i].Height + SPACING;
                        //if (lastY + projRects[i].Height >=  bottomScrollRect.Top && !needsScroll)
                        //{
                        //    // The current projectile is not completely visible
                        //    // and we don't already have the scroll values ready
                        //    CalculateScrollValues();
                        //}
                    }

                    projDraggables[i].X = lastX;
                    projDraggables[i].Y = lastY;
                    lastX += projDraggables[i].Width + SPACING;

                    SetSnapPoints(i, true);

                    itemsInCurrentRow++;
                }
            }
        }

        private void CalculateScrollValues()
        {
            float contentArea = (projRects[0].Height + SPACING) * projRects.Count;
            if (contentArea + topScrollRect.Bottom >= bottomScrollRect.Top)
            {
                // The content area is too large and we need to add a scroll bar
                needsScroll = true;

                if (contentArea > WANTED_MAX_AREA_HEIGHT)
                {
                    // The content area is larger than the area allowed
                    scrollPerPixel = contentArea / WANTED_MAX_AREA_HEIGHT;
                    scrollBarRect.Height = MIN_SCROLLBAR_HEIGHT;
                }
                else
                {
                    scrollPerPixel = 1;
                    scrollBarRect.Height = (int)(ON_SCREEN_MAX_AREA_HEIGHT * (ON_SCREEN_MAX_AREA_HEIGHT / contentArea));
                }
            }
            else
            {
                needsScroll = false;
            }
        }

        #endregion
    }

    public class HotbarSlotInteractive
    {
        #region Fields

        // Set to none if empty
        public ProjectileType Content;

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

        #endregion

        #region Constructors

        public HotbarSlotInteractive(GraphicsDevice graphics, int x, int y, int width, int height)
        {
            img = new Texture2D(graphics, width, height);
            img = DrawHelper.AddBorder(img, 2, Color.Black, Color.Transparent);

            drawRect = new Rectangle(x, y, width, height);
        }

        #endregion

        #region Public Methods

        public void Draw(SpriteBatch spriteBatch)
        {
            spriteBatch.Draw(img, drawRect, Color.White);
        }

        public void SetContent(ProjectileType proj)
        {
            Content = proj;
        }

        #endregion
    }

    public delegate bool DraggableEvent(DraggableProjectile obj);
    public class DraggableProjectile
    {
        #region Fields & Properties

        Texture2D img;
        public Rectangle DrawRectangle;

        public int Count;
        Vector2 countPos = new Vector2();
        SpriteFont font;

        static bool draggingAny = false;

        public int X
        {
            get
            {
                return DrawRectangle.X;
            }
            set
            {
                DrawRectangle.X = value;
            }
        }
        public int Y
        {
            get
            {
                return DrawRectangle.Y;
            }
            set
            {
                DrawRectangle.Y = value;
            }
        }
        public int Width
        {
            get
            {
                return DrawRectangle.Width;
            }
            set
            {
                DrawRectangle.Width = value;
            }
        }
        public int Height
        {
            get
            {
                return DrawRectangle.Height;
            }
            set
            {
                DrawRectangle.Height = value;
            }
        }

        public bool DraggingThis = false;
        public ProjectileType Content;
        public List<HotbarSlotInteractive> SnapPoints = new List<HotbarSlotInteractive>();
        // This will prevent the snapping method from snapping the draggable to 0, 0 unless we tell it to
        // Before, the current snap point was initially (0, 0), and this caused a bug unless it was set to something else
        // This prevented the draggable from only being able to snap to hotbar slots
        Vector2 currentSnapPoint = new Vector2(-1, -1);

        public HotbarSlotInteractive CurrentSlot;

        const int SPACING = 15;

        event DraggableEvent whenDraggingEnded;
        event System.Action whenSnappedToHotbar;

        #endregion

        #region Constructors

        public DraggableProjectile(Texture2D img, int x, int y, int width, int height, ProjectileType content, int count, SpriteFont font)
        {
            this.img = img;
            this.Count = count;
            this.font = font;
            Content = content;
            DrawRectangle = new Rectangle(x, y, width, height);
            RepositionCount();
        }

        public DraggableProjectile(Texture2D img, Rectangle rect, ProjectileType content, int count, SpriteFont font)
        {
            this.img = img;
            this.Count = count;
            this.font = font;
            Content = content;
            DrawRectangle = rect;
            RepositionCount();
        }

        #endregion

        #region Public Methods

        public void Update()
        {
            MouseState mouse = Mouse.GetState();
            bool isSelected = DrawRectangle.Intersects(new Rectangle(mouse.X, mouse.Y, 1, 1));
            if (isSelected && mouse.LeftButton == ButtonState.Pressed && (!draggingAny || DraggingThis))
            {
                DraggingThis = true;
                draggingAny = true;
                DrawRectangle.X = mouse.X - DrawRectangle.Width / 2;
                DrawRectangle.Y = mouse.Y - DrawRectangle.Height / 2;
            }
            else
            {
                if (DraggingThis)
                {
                    if (whenDraggingEnded?.Invoke(this) == false)
                    {
                        // Only execute the below code if the dragging method failed

                        // We were previously dragging, but now we aren't. Therefore, we
                        // must find the closest snap point to go to
                        Vector2 currentPos = new Vector2(DrawRectangle.X, DrawRectangle.Y);
                        HotbarSlotInteractive min = SnapPoints.FindMinDistance(currentPos);
                        if (Vector2.Distance(new Vector2(min.X, min.Y), currentPos) >=
                            Vector2.Distance(currentSnapPoint, currentPos) && currentSnapPoint.X > -1)
                        {
                            DrawRectangle.X = (int)currentSnapPoint.X;
                            DrawRectangle.Y = (int)currentSnapPoint.Y;
                            if (CurrentSlot != null)
                            {
                                CurrentSlot.SetContent(ProjectileType.None);
                                CurrentSlot = null;
                            }
                        }
                        else
                        {
                            if (min.Content == ProjectileType.None || min.Content == this.Content)
                            {
                                if (min != CurrentSlot)
                                {
                                    CurrentSlot?.SetContent(ProjectileType.None);
                                    CurrentSlot = min;
                                }
                                DrawRectangle.X = CurrentSlot.X + SPACING / 2;
                                DrawRectangle.Y = CurrentSlot.Y + SPACING / 2;
                                CurrentSlot.SetContent(Content);
                                whenSnappedToHotbar?.Invoke();
                                // Don't allow draggables to snap back to the "lone position" (where there's no hotbar slot)
                                // once they're in a hotbar slot
                                currentSnapPoint.X = currentSnapPoint.Y = -1;
                            }
                            else
                            {
                                DrawRectangle.X = (int)currentSnapPoint.X;
                                DrawRectangle.Y = (int)currentSnapPoint.Y;
                                if (CurrentSlot != null)
                                {
                                    CurrentSlot.SetContent(ProjectileType.None);
                                    CurrentSlot = null;
                                }
                            }
                        }
                    }
                    draggingAny = false;
                }

                DraggingThis = false;
            }
            RepositionCount();
        }

        public void Draw(SpriteBatch spriteBatch, Color color)
        {
            spriteBatch.Draw(img, DrawRectangle, color);
            spriteBatch.DrawString(font, "x" + Count.ToString(), countPos, Color.Black);
        }

        public void SetSnapPoints(List<HotbarSlotInteractive> points, bool includeCurrent)
        {
            SnapPoints = points;
            if (includeCurrent)
            {
                currentSnapPoint = new Vector2(DrawRectangle.X, DrawRectangle.Y);
            }
        }

        public void SnapTo(HotbarSlotInteractive point)
        {
            if (point.Content == ProjectileType.None || point.Content == this.Content)
            {
                if (point != CurrentSlot && CurrentSlot != null)
                {
                    CurrentSlot.SetContent(ProjectileType.None);
                    CurrentSlot = null;
                }
                DrawRectangle.X = point.X + SPACING / 2;
                DrawRectangle.Y = point.Y + SPACING / 2;
                point.SetContent(Content);
                CurrentSlot = point;
            }
        }

        public void AddOnMouseReleaseHandler(DraggableEvent handler)
        {
            whenDraggingEnded += handler;
        }
        public void AddSnapToHotbarHandler(System.Action handler)
        {
            whenSnappedToHotbar += handler;
        }

        #endregion

        #region Private Methods

        private void RepositionCount()
        {
            countPos.X = DrawRectangle.Right - font.MeasureString(Count.ToString()).X - SPACING / 3;
            countPos.Y = DrawRectangle.Bottom - font.MeasureString(Count.ToString()).Y + SPACING / 3;
        }

        #endregion
    }

    public class RClickDropdown
    {
        #region Fields & Properties

        List<DropdownItem> items = new List<DropdownItem>();

        public bool Active = false;

        Vector2 pos = new Vector2();
        Rectangle rect = new Rectangle();
        public int X
        {
            get
            {
                return (int)pos.X;
            }
            set
            {
                pos.X = value;
                Position();
            }
        }
        public int Y
        {
            get
            {
                return (int)pos.Y;
            }
            set
            {
                pos.Y = value;
                Position();
            }
        }

        #endregion

        #region Constructors

        public RClickDropdown(List<string> values, List<System.Action> actions, SpriteFont font)
        {
            items = DropdownItem.DropdownsFrom(values, actions, font);
            for (int i = 0; i < items.Count; i++)
            {
                items[i].AddOnClickHandler(Close);
            }
        }

        #endregion

        #region Public Methods

        public void Show(int x, int y)
        {
            X = x;
            Y = y;
            Active = true;
        }
        public void Close()
        {
            Active = false;
        }

        public void Update(GameTime gameTime)
        {
            if (Active)
            {
                for (int i = 0; i < items.Count; i++)
                {
                    items[i].Update(gameTime);
                }

                MouseState mouse = Mouse.GetState();
                Rectangle mouseRect = new Rectangle(mouse.X, mouse.Y, 1, 1);
                if (mouse.LeftButton == ButtonState.Pressed && !mouseRect.Intersects(rect))
                {
                    // If you click the screen somewhere that is NOT the dropdown,
                    // then close.
                    Close();
                }
            }
        }

        public void Draw(SpriteBatch spriteBatch)
        {
            if (Active)
            {
                for (int i = 0; i < items.Count; i++)
                {
                    items[i].Draw(spriteBatch);
                }
            }
        }

        public List<MenuButton> GetButtons()
        {
            List<MenuButton> returnVal = new List<MenuButton>();
            for (int i = 0; i < items.Count; i++)
            {
                returnVal.AddRange(items[i].GetButtons());
            }
            return returnVal;
        }

        #endregion

        #region Private Methods

        private void Position()
        {
            int y = (int)pos.Y;
            for (int i = 0; i < items.Count; i++)
            {
                items[i].X = (int)pos.X;
                items[i].Y = y;
                y += DropdownItem.HEIGHT;
            }
            rect = new Rectangle((int)pos.X, (int)pos.Y, DropdownItem.WIDTH,
                DropdownItem.HEIGHT * items.Count);
        }

        #endregion
    }
    public class DropdownItem
    {
        #region Fields & Properties

        Texture2D bgImg;
        Rectangle bgRect;
        Color bgColor = Color.Blue;
        SpriteFont font;
        string text;
        Vector2 textPos;
        MenuButton detect;

        event System.Action onClick;

        public const int WIDTH = 125;
        public const int HEIGHT = 35;

        public int X
        {
            get
            {
                return bgRect.X;
            }
            set
            {
                bgRect.X = value;
                detect.X = bgRect.X;
                textPos.X = bgRect.X + (bgRect.Width / 2 - font.MeasureString(text).X / 2);
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
                detect.Y = bgRect.Y;
                textPos.Y = bgRect.Y + (bgRect.Height / 2 - font.MeasureString(text).Y / 2);
            }
        }

        #endregion

        #region Constructors

        public DropdownItem(string text, System.Action onClick, SpriteFont font)
        {
            this.text = text;
            this.font = font;

            this.onClick += onClick;

            bgImg = Utilities.RectImage;
            bgRect = new Rectangle(0, 0, WIDTH, HEIGHT);

            detect = new MenuButton(OnClick, null, bgRect.X, bgRect.Y, false, null, null);
            detect.Width = bgRect.Width;
            detect.Height = bgRect.Height;

            textPos = new Vector2(bgRect.X + (bgRect.Width / 2 - font.MeasureString(text).X / 2),
                bgRect.Y + (bgRect.Height / 2 - font.MeasureString(text).Y / 2));
        }

        #endregion

        #region Public Methods

        public void Update(GameTime gameTime)
        {
            detect.Update(gameTime);

            bgColor = detect.Hovered ? Color.LightBlue : Color.Blue;
        }

        public void Draw(SpriteBatch spriteBatch)
        {
            spriteBatch.Draw(bgImg, bgRect, bgColor);
            spriteBatch.DrawString(font, text, textPos, Color.White);
        }

        public static List<DropdownItem> DropdownsFrom(List<string> values, List<System.Action> actions, SpriteFont font)
        {
            List<DropdownItem> returnVal = new List<DropdownItem>();
            for (int i = 0; i < values.Count; i++)
            {
                returnVal.Add(new DropdownItem(values[i], actions[i], font));
            }
            return returnVal;
        }

        public void AddOnClickHandler(System.Action handler)
        {
            onClick += handler;
        }

        public List<MenuButton> GetButtons()
        {
            return new List<MenuButton>() { detect };
        }

        #endregion

        #region Private Methods

        private void OnClick()
        {
            onClick?.Invoke();
        }

        #endregion
    }

    public enum OrganizeState
    {
        Materials,
        Projectiles,
        Hotbar,
        Collection
    }
}
