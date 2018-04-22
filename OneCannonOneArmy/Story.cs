using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OneCannonOneArmy
{
    public class Story
    {
        const float SCROLL_SPD = 0.5f;

        List<string> lines = new List<string>();
        List<Vector2> locations = new List<Vector2>();

        SpriteFont font;

        int windowWidth;
        int windowHeight;

        public bool Scrolling = false;

        public Story(int windowWidth, int windowHeight, SpriteFont font)
        {
            this.windowWidth = windowWidth;
            this.windowHeight = windowHeight;
            this.font = font;
        }

        public void StartStory(int location)
        {
            lines.Clear();
            locations.Clear();

            List<string> story = new List<string>();
            switch (location)
            {
                case 0:
                    story = GameInfo.STORY1;
                    break;
                case 1:
                    story = GameInfo.STORY2;
                    break;
            }
            for (int i = 0; i < story.Count; i++)
            {
                lines.Add(LanguageTranslator.Translate(story[i]));
                float x = windowWidth / 2 - (font.MeasureString(lines[i]).X / 2);
                float y = i * font.MeasureString(lines[i]).Y + windowHeight;
                locations.Add(new Vector2(x, y));
            }

            Scrolling = true;
        }

        public void Update()
        {
            Sound.CheckAndPlaySong(Music.StoryMusic);

            for (int i = 0; i < locations.Count; i++)
            {
                locations[i] = new Vector2(locations[i].X, locations[i].Y - SCROLL_SPD);
            }
            
            if (locations[locations.Count - 1].Y <= 0)
            {
                Scrolling = false;
            }
        }
        public void Draw(SpriteBatch spriteBatch)
        {
            for (int i = 0; i < lines.Count; i++)
            {
                spriteBatch.DrawString(font, lines[i], locations[i], Color.Black);
            }
        }
    }
}
