using ColorPicker.Settings;
using System;
using System.Drawing;

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
                case ColorFormat.rgb:
                    return ColorToRgb(c);
                case ColorFormat.hsl:
                    return ColorToHsl(c);
                case ColorFormat.hwb:
                    return ColorToHwb(c);
                case ColorFormat.hsv:
                    return ColorToHsv(c);
                case ColorFormat.lab:
                    return ColorToLab(c);
                case ColorFormat.xyz:
                    return ColorToXyzString(c);
                case ColorFormat.vec4:
                    return ColorToVec4(c);
                case ColorFormat.cmyk:
                    return ColorToCmyk(c);
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
            double r = c.R / 255.0;
            double g = c.G / 255.0;
            double b = c.B / 255.0;

            double max = Math.Max(r, Math.Max(g, b));
            double min = Math.Min(r, Math.Min(g, b));
            double delta = max - min;

            double h = 0;
            if (delta != 0)
            {
                if (max == r)
                {
                    h = (g - b) / delta + (g < b ? 6 : 0);
                }
                else if (max == g)
                {
                    h = (b - r) / delta + 2;
                }
                else
                {
                    h = (r - g) / delta + 4;
                }
                h /= 6;
            }

            double l = (max + min) / 2;
            double s = (delta == 0) ? 0 : delta / (1 - Math.Abs(2 * l - 1));

            h = Math.Round(h * 360);
            s = Math.Round(s * 100);
            l = Math.Round(l * 100);

            return $"hsl({h}, {s}%, {l}%)";
        }

        private static string ColorToHsv(Color c)
        {
            double r = c.R / 255.0;
            double g = c.G / 255.0;
            double b = c.B / 255.0;

            double max = Math.Max(r, Math.Max(g, b));
            double min = Math.Min(r, Math.Min(g, b));
            double delta = max - min;

            double h = 0;
            if (delta != 0)
            {
                if (max == r)
                {
                    h = (g - b) / delta + (g < b ? 6 : 0);
                }
                else if (max == g)
                {
                    h = (b - r) / delta + 2;
                }
                else
                {
                    h = (r - g) / delta + 4;
                }
                h /= 6;
            }

            double s = max == 0 ? 0 : delta / max;
            double v = max;

            h = Math.Round(h * 360);
            s = Math.Round(s * 100);
            v = Math.Round(v * 100);

            return $"hsv({h}, {s}%, {v}%)";
        }

        private static string ColorToVec4(Color c)
        {
            return $"vec4({(c.R / 255f):F3}, {(c.G / 255f):F3}, {(c.B / 255f):F3}, {(c.A / 255f):F3})";
        }

        private static string ColorToRgb565(Color c)
        {
            ushort r = (ushort)(c.R >> 3);
            ushort g = (ushort)(c.G >> 2);
            ushort b = (ushort)(c.B >> 3);
            ushort rgb565 = (ushort)((r << 11) | (g << 5) | b);
            return $"#{rgb565:X4}";
        }

        private static string ColorToDecimal(Color c, bool isLittleEndian)
        {
            int red = c.R;
            int green = c.G;
            int blue = c.B;

            if (isLittleEndian)
            {
                (blue, red) = (red, blue);
            }

            long result = (red * 65536L) + (green * 256) + blue;
            return result.ToString();
        }

        private static string ColorToCmyk(Color c)
        {
            double r = c.R / 255.0;
            double g = c.G / 255.0;
            double b = c.B / 255.0;

            double k = 1 - Math.Max(r, Math.Max(g, b));
            double cyan = (1 - r - k) / (1 - k);
            double magenta = (1 - g - k) / (1 - k);
            double yellow = (1 - b - k) / (1 - k);

            cyan = Math.Round(cyan * 100);
            magenta = Math.Round(magenta * 100);
            yellow = Math.Round(yellow * 100);
            k = Math.Round(k * 100);

            return $"cmyk({cyan}%, {magenta}%, {yellow}%, {k}%)";
        }

        private static (double x, double y, double z) ColorToXyz(Color c)
        {
            double r = c.R / 255.0;
            double g = c.G / 255.0;
            double b = c.B / 255.0;

            // Convert to linear RGB
            r = (r > 0.04045) ? Math.Pow((r + 0.055) / 1.055, 2.4) : r / 12.92;
            g = (g > 0.04045) ? Math.Pow((g + 0.055) / 1.055, 2.4) : g / 12.92;
            b = (b > 0.04045) ? Math.Pow((b + 0.055) / 1.055, 2.4) : b / 12.92;

            // Convert to XYZ
            double x = r * 0.4124564 + g * 0.3575761 + b * 0.1804375;
            double y = r * 0.2126729 + g * 0.7151522 + b * 0.0721750;
            double z = r * 0.0193339 + g * 0.1191920 + b * 0.9503041;

            // Normalize for D65 illuminant
            x /= 0.95047;
            y /= 1.00000;
            z /= 1.08883;

            x = Math.Round(x * 100);
            y = Math.Round(y * 100);
            z = Math.Round(z * 100);

            return (x, y, z);
        }

        private static string ColorToXyzString(Color c)
        {
            var (x, y, z) = ColorToXyz(c);
            return $"xyz({x}, {y}, {z})";
        }

        private static string ColorToLab(Color c)
        {
            // Use ColorToXyz to get XYZ values
            var (x, y, z) = ColorToXyz(c);

            x = x / 100.0;
            y = y / 100.0;
            z = z / 100.0;

            // Convert XYZ to Lab
            x = (x > 0.008856) ? Math.Pow(x, 1.0 / 3) : (7.787 * x) + (16.0 / 116);
            y = (y > 0.008856) ? Math.Pow(y, 1.0 / 3) : (7.787 * y) + (16.0 / 116);
            z = (z > 0.008856) ? Math.Pow(z, 1.0 / 3) : (7.787 * z) + (16.0 / 116);

            double l = (116 * y) - 16;
            double a = 500 * (x - y);
            double b = 200 * (y - z);

            l = Math.Round(l);
            a = Math.Round(a);
            b = Math.Round(b);

            return $"lab({l}, {a}, {b})";
        }

        private static string ColorToHwb(Color c)
        {
            double r = c.R / 255.0;
            double g = c.G / 255.0;
            double b = c.B / 255.0;

            double max = Math.Max(r, Math.Max(g, b));
            double min = Math.Min(r, Math.Min(g, b));
            double delta = max - min;

            double h = 0;
            if (delta != 0)
            {
                if (max == r)
                {
                    h = (g - b) / delta + (g < b ? 6 : 0);
                }
                else if (max == g)
                {
                    h = (b - r) / delta + 2;
                }
                else
                {
                    h = (r - g) / delta + 4;
                }
                h /= 6;
            }

            double w = min;
            double b_value = 1 - max;

            h = Math.Round(h * 360);
            w = Math.Round(w * 100);
            b_value = Math.Round(b_value * 100);

            return $"hwb({h}, {w}%, {b_value}%)";
        }
    }
}
