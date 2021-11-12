using SixLabors.ImageSharp;

namespace QRCoder.Extensions;

public static class ColorExtensions
{
    public static string ToHtmlHex(this Color color)
        => $"#{color.ToHex()}";
}