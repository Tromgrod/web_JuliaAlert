// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ColorHelper.cs" company="GalexStudio">
//   Copyright 2013
// </copyright>
// <summary>
//   Defines the ColorHelper.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

using System;
using System.Drawing;
namespace LIB.Helpers
{
    /// <summary>
    /// The resource register.
    /// </summary>
    public class ColorHelper
    {
        public static Color generateRandomColor(Color mix,int seed)
        {
            Random random = new Random(seed);
            int red = random.Next(256);
            int green = random.Next(256);
            int blue = random.Next(256);

            // mix the color
            if (mix.ToArgb()!=0)
            {
                red = (red + mix.R) / 2;
                green = (green + mix.G) / 2;
                blue = (blue + mix.B) / 2;
            }

            var color = Color.FromArgb(red, green, blue);    
            return color;
        }
        public static String HexConverter(System.Drawing.Color c)
        {
            return "#" + c.R.ToString("X2") + c.G.ToString("X2") + c.B.ToString("X2");
        }
    }
}
