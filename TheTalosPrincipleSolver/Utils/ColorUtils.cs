using System;
using System.Collections.Generic;
using System.Windows.Media;

namespace TheTalosPrincipleSolver.Utils
{
	public static class ColorUtils
	{
		private static Color HSBToRGBConversion(float hue, float saturation, float brightness)
		{
			float red, green, blue;

			if (saturation == 0)
			{
				red = green = blue = brightness; // achromatic
			}
			else
			{
				var q = brightness < 0.5
						? brightness * (1 + saturation)
						: brightness + saturation - brightness * saturation;
				var p = 2 * brightness - q;
				red = Hue2Rgb(p, q, hue + 1f / 3);
				green = Hue2Rgb(p, q, hue);
				blue = Hue2Rgb(p, q, hue - 1f / 3);
			}

			return Color.FromRgb((byte)Math.Round(red * 255), (byte)Math.Round(green * 255), (byte)Math.Round(blue * 255));
		}

		private static float Hue2Rgb(float p, float q, float t)
		{
			if (t < 0)
				t += 1;
			if (t > 1)
				t -= 1;
			if (t < 1f / 6)
				return p + (q - p) * 6 * t;
			if (t < 1f / 2)
				return q;
			if (t < 2f / 3)
				return p + (q - p) * (2f / 3 - t) * 6;
			return p;
		}

		public static List<SolidColorBrush> GetBrushes(uint size)
		{
			var res = new List<SolidColorBrush> { Brushes.Black, Brushes.Black };
			for (var i = 0; i < size; ++i)
			{
				var brush = new SolidColorBrush(HSBToRGBConversion(i / (float)size, (i & 1) == 0 ? 1.0f : 0.5f, 0.75f));
				res.Add(brush);
			}
			return res;
		}
	}
}
