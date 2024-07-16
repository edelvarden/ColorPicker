using ColorPicker.Settings;
using System;
using System.Drawing;
using System.Globalization;

namespace ColorPicker.Helpers
{
    public static class ColorFormatHelper
    {
        public static string ColorToString(Color c, ColorFormat format)
        {
            switch (format)
            {
                case ColorFormat.hex:
                    return ColorToHex(c);
                case ColorFormat.hsl:
                    return ColorToHsl(c);
                case ColorFormat.hsv:
                    return ColorToHsv(c);
                case ColorFormat.rgb:
                    return ColorToRgb(c);
                case ColorFormat.vec4:
                    return ColorToVec4(c);
                case ColorFormat.rgb565:
                    return ColorToRgb565(c);
                case ColorFormat.decimalLE:
                    return ColorToDecimal(c, true);
                case ColorFormat.decimalBE:
                    return ColorToDecimal(c, false);
                default:
                    return string.Empty;
            }
        }

        private static string ColorToDecimal(Color c, bool isLittleEndian)
        {
            int red = c.R;
            int green = c.G;
            int blue = c.B;

            if (isLittleEndian)
            {
                int temp = red;
                red = blue;
                blue = temp;
            }

            long result = (red * 65536L) + (green * 256) + blue;
            return result.ToString();
        }

        private static string ColorToRgb565(Color c)
        {
            ushort r = (ushort)(c.R >> 3);
            ushort g = (ushort)(c.G >> 2);
            ushort b = (ushort)(c.B >> 3);
            ushort rgb565 = (ushort)((r << 11) | (g << 5) | b);
            return $"#{rgb565:X4}";
        }

        private static string ColorToHex(Color c)
        {
            return $"{c.R:X2}{c.G:X2}{c.B:X2}".ToLower();
        }

        private static string ColorToRgb(Color c)
        {
            return $"rgb({c.R}, {c.G}, {c.B})";
        }

        private static string ColorToHsl(Color c)
        {
            int h = (int)Math.Round(c.GetHue());
            double s = Math.Round(c.GetSaturation() * 100, 1);
            double l = Math.Round(c.GetBrightness() * 100, 1);
            return $"hsl({h}, {s}%, {l}%)";
        }

        private static string ColorToHsv(Color c)
        {
            int h = (int)Math.Round(c.GetHue());
            double s = Math.Round(c.GetSaturation() * 100, 1);
            double v = Math.Round(c.GetBrightness() * 100, 1);
            return $"hsv({h}, {s}%, {v}%)";
        }

        private static string ColorToVec4(Color c)
        {
            return $"vec4({c.R / 255f:F3}, {c.G / 255f:F3}, {c.B / 255f:F3}, 1.0)";
        }
    }
}
