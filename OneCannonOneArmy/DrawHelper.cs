using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace OneCannonOneArmy
{
    public static class DrawHelper
    {
        public static Texture2D CreateCircle(int radius, GraphicsDevice graphics, Color color)
        {
            // Code from stackoverflow.com/questions/2983809/how-to-draw-circle-with-specific-color-in-xna
            // This section draws the border

            int outerRadius = radius * 2 + 2; // So circle doesn't go out of bounds
            Texture2D texture = new Texture2D(graphics, outerRadius, outerRadius);

            Color[] data = new Color[outerRadius * outerRadius];

            // Colour the entire texture transparent first.
            for (int i = 0; i < data.Length; i++)
                data[i] = Color.Transparent;

            // Work out the minimum step necessary using trigonometry + sine approximation.
            double angleStep = 1f / radius;

            for (double angle = 0; angle < Math.PI * 2; angle += angleStep)
            {
                // Use the parametric definition of a circle: en.wikipedia.org/wiki/Circle#Cartesian_coordinates
                int x = (int)Math.Round(radius + radius * Math.Cos(angle));
                int y = (int)Math.Round(radius + radius * Math.Sin(angle));

                data[y * outerRadius + x + 1] = color;
            }

            // This section taken from stackoverflow.com/questions/5641579/xna-draw-a-filled-circle, 
            // answer by user1089284
            // Parts edited
            // This section fills the circle
            bool finished = false;
            int firstSkip = 0;
            int lastSkip = 0;
            for (int i = 0; i <= data.Length - 1; i++)
            {
                if (finished == false)
                {
                    //T = transparent W = White;
                    //Find the First Batch of Colors TTTTWWWTTTT The top of the circle
                    if ((data[i] == color) && (firstSkip == 0))
                    {
                        while (data[i + 1] == color)
                        {
                            i++;
                        }
                        firstSkip = 1;
                        i++;
                    }
                    //Now Start Filling                       TTTTTTTTWWTTTTTTTT
                    //circle in Between                       TTTTTTW--->WTTTTTT
                    //transaparent blancks                    TTTTTWW--->WWTTTTT
                    //                                        TTTTTTW--->WTTTTTT
                    //                                        TTTTTTTTWWTTTTTTTT
                    if (firstSkip == 1)
                    {
                        if (data[i] == color && data[i + 1] != color)
                        {
                            i++;
                            while (data[i] != color)
                            {
                                //Loop to check if its the last row of pixels
                                //We need to check this because of the 
                                //int outerRadius = radius * 2 + -->'2'<--;
                                for (int j = 1; j <= outerRadius; j++)
                                {
                                    if (data[i + j] != color)
                                    {
                                        lastSkip++;
                                    }
                                }
                                //If its the last line of pixels, end drawing
                                if (lastSkip == outerRadius)
                                {
                                    finished = true;
                                    break;
                                }
                                else
                                {
                                    data[i] = color;
                                    i++;
                                    lastSkip = 0;
                                }
                            }
                            while (data[i] == color)
                            {
                                i++;
                            }
                            i--;
                        }
                    }
                }
            }

            texture.SetData(data);
            return texture;
        }

        public static Texture2D AddBorder(Texture2D texture, int borderWidth, Color borderColor, Color fillColor)
        {
            // Code taken from stackoverflow.com/questions/13893959/how-to-draw-the-border-of-a-square
            Color[] colors = new Color[texture.Width * texture.Height];

            for (int x = 0; x < texture.Width; x++)
            {
                for (int y = 0; y < texture.Height; y++)
                {
                    bool colored = false;
                    for (int i = 0; i <= borderWidth; i++)
                    {
                        if (x == i || y == i || x == texture.Width - 1 - i || y == texture.Height - 1 - i)
                        {
                            colors[x + y * texture.Width] = borderColor;
                            colored = true;
                            break;
                        }
                    }

                    if (!colored)
                    {
                        colors[x + y * texture.Width] = fillColor;
                    }
                }
            }

            texture.SetData(colors);

            return texture;
        }

        public static Texture2D Fill(Texture2D img, Color fillColor)
        {
            Color[] data = new Color[img.Width * img.Height];
            img.GetData(data);
            for (int i = 0; i < data.Count(); i++)
            {
                data[i] = fillColor;
            }
            img.SetData(data);
            return img;
        }

        public static Texture2D ApplyShade(Texture2D texture, Color shadeColor, GraphicsDevice graphics)
        {
            // Shade the image using additive blending
            Texture2D returnVal = new Texture2D(graphics, texture.Width, texture.Height);
            Color[] currentData = new Color[texture.Width * texture.Height];
            Color[] newData = new Color[texture.Width * texture.Height];
            texture.GetData(currentData);
            Color pixel = new Color(255, 255, 255);

            for (int i = 0; i < currentData.Length; i++)
            {
                if (currentData[i] == Color.Transparent)
                {
                    continue;
                }
                pixel.R = (byte)Utilities.Average(currentData[i].R, shadeColor.R);
                pixel.G = (byte)Utilities.Average(currentData[i].G, shadeColor.G);
                pixel.B = (byte)Utilities.Average(currentData[i].B, shadeColor.B);
                newData[i] = pixel;
            }

            returnVal.SetData(newData);
            return returnVal;
        }

        public static Color GetDominantColor(Texture2D img)
        {
            Color[] data = new Color[img.Width * img.Height];
            img.GetData(data);

            // Find which pixel appears the most
            List<Color> list = data.GroupBy(i => i).OrderByDescending(grp => grp.Count())
                .Select(grp => grp.Key).Distinct().ToList();
            Color most = Color.Black;
            for (int i = 0; i < list.Count; i++)
            {
                if (list[i].A != 0)
                {
                    most = list[i];
                    break;
                }
            }

            return most;
        }
    }
}
