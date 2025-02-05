using System;
using System.Collections;
using System.Drawing;
using System.Text;
using QRCoder.Extensions;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.Formats.Png;
using SixLabors.ImageSharp.Processing;
using static QRCoder.QRCodeGenerator;
using static QRCoder.SvgQRCode;
using Color = SixLabors.ImageSharp.Color;
using Size = SixLabors.ImageSharp.Size;

namespace QRCoder;

public class SvgQRCode : AbstractQRCode, IDisposable
{
    /// <summary>
    /// Constructor without params to be used in COM Objects connections
    /// </summary>
    public SvgQRCode() { }
    public SvgQRCode(QRCodeData data) : base(data) { }

    public string GetGraphic(int pixelsPerModule)
    {
        var viewBox = new Size(pixelsPerModule*this.QrCodeData.ModuleMatrix.Count, pixelsPerModule * this.QrCodeData.ModuleMatrix.Count);
        return this.GetGraphic(viewBox, Color.Black, Color.White);
    }
    public string GetGraphic(int pixelsPerModule, Color darkColor, Color lightColor, bool drawQuietZones = true, SizingMode sizingMode = SizingMode.WidthHeightAttribute, SvgLogo logo = null)
    {
        var offset = drawQuietZones ? 0 : 4;
        var edgeSize = this.QrCodeData.ModuleMatrix.Count * pixelsPerModule - (offset * 2 * pixelsPerModule);
        var viewBox = new Size(edgeSize, edgeSize);
        return this.GetGraphic(viewBox, darkColor, lightColor, drawQuietZones, sizingMode, logo);
    }

    public string GetGraphic(int pixelsPerModule, string darkColorHex, string lightColorHex, bool drawQuietZones = true, SizingMode sizingMode = SizingMode.WidthHeightAttribute, SvgLogo logo = null)
    {
        var offset = drawQuietZones ? 0 : 4;
        var edgeSize = this.QrCodeData.ModuleMatrix.Count * pixelsPerModule - (offset * 2 * pixelsPerModule);
        var viewBox = new Size(edgeSize, edgeSize);
        return this.GetGraphic(viewBox, darkColorHex, lightColorHex, drawQuietZones, sizingMode, logo);
    }

    public string GetGraphic(Size viewBox, bool drawQuietZones = true, SizingMode sizingMode = SizingMode.WidthHeightAttribute, SvgLogo logo = null)
    {
        return this.GetGraphic(viewBox, Color.Black, Color.White, drawQuietZones, sizingMode, logo);
    }

    public string GetGraphic(Size viewBox, Color darkColor, Color lightColor, bool drawQuietZones = true, SizingMode sizingMode = SizingMode.WidthHeightAttribute, SvgLogo logo = null)
    {
        return this.GetGraphic(viewBox, darkColor.ToHtmlHex(), lightColor.ToHtmlHex(), drawQuietZones, sizingMode, logo);
    }

    public string GetGraphic(Size viewBox, string darkColorHex, string lightColorHex, bool drawQuietZones = true, SizingMode sizingMode = SizingMode.WidthHeightAttribute, SvgLogo logo = null)
    {
        var offset = drawQuietZones ? 0 : 4;
        var drawableModulesCount = this.QrCodeData.ModuleMatrix.Count - (drawQuietZones ? 0 : offset * 2);
        var pixelsPerModule = Math.Min(viewBox.Width, viewBox.Height) / (double)drawableModulesCount;
        var qrSize = drawableModulesCount * pixelsPerModule;
        var svgSizeAttributes = (sizingMode == SizingMode.WidthHeightAttribute) ? $@"width=""{viewBox.Width}"" height=""{viewBox.Height}""" : $@"viewBox=""0 0 {viewBox.Width} {viewBox.Height}""";

        // Merge horizontal rectangles
        var matrix = new int[drawableModulesCount, drawableModulesCount];
        for (var yi = 0; yi < drawableModulesCount; yi += 1)
        {
            var bitArray = this.QrCodeData.ModuleMatrix[yi+offset];

            var x0 = -1;
            var xL = 0;
            for (var xi = 0; xi < drawableModulesCount; xi += 1)
            {
                matrix[yi, xi] = 0;
                if (bitArray[xi+offset])
                {
                    if(x0 == -1)
                    {
                        x0 = xi;
                    }
                    xL += 1;
                }
                else
                {
                    if(xL > 0)
                    {
                        matrix[yi, x0] = xL;
                        x0 = -1;
                        xL = 0;
                    }
                }
            }

            if (xL > 0)
            {
                matrix[yi, x0] = xL;
            }
        }

        var svgFile = new StringBuilder($@"<svg version=""1.1"" baseProfile=""full"" shape-rendering=""crispEdges"" {svgSizeAttributes} xmlns=""http://www.w3.org/2000/svg"">");
        svgFile.AppendLine($@"<rect x=""0"" y=""0"" width=""{CleanSvgVal(qrSize)}"" height=""{CleanSvgVal(qrSize)}"" fill=""{lightColorHex}"" />");
        for (var yi = 0; yi < drawableModulesCount; yi += 1)
        {
            var y = yi * pixelsPerModule;
            for (var xi = 0; xi < drawableModulesCount; xi += 1)
            {
                var xL = matrix[yi, xi];
                if(xL > 0)
                {
                    // Merge vertical rectangles
                    var yL = 1;
                    for (var y2 = yi + 1; y2 < drawableModulesCount; y2 += 1)
                    {
                        if(matrix[y2, xi] == xL)
                        {
                            matrix[y2, xi] = 0;
                            yL += 1;
                        }
                        else
                        {
                            break;
                        }
                    }

                    // Output SVG rectangles
                    var x = xi * pixelsPerModule;
                    svgFile.AppendLine($@"<rect x=""{CleanSvgVal(x)}"" y=""{CleanSvgVal(y)}"" width=""{CleanSvgVal(xL * pixelsPerModule)}"" height=""{CleanSvgVal(yL * pixelsPerModule)}"" fill=""{darkColorHex}"" />");
                }
            }
        }

        //Render logo, if set
        if (logo != null)
        {
            svgFile.AppendLine($@"<svg width=""100%"" height=""100%"" version=""1.1"" xmlns = ""http://www.w3.org/2000/svg"">");
            svgFile.AppendLine($@"<image x=""{50 - (logo.GetIconSizePercent() / 2)}%"" y=""{50 - (logo.GetIconSizePercent() / 2)}%"" width=""{logo.GetIconSizePercent()}%"" height=""{logo.GetIconSizePercent()}%"" href=""{logo.GetDataUri()}"" />");
            svgFile.AppendLine(@"</svg>");
        }

        svgFile.Append(@"</svg>");
        return svgFile.ToString();
    }

    private string CleanSvgVal(double input)
    {
        //Clean double values for international use/formats
        return input.ToString(System.Globalization.CultureInfo.InvariantCulture);
    }

    public enum SizingMode
    {
        WidthHeightAttribute,
        ViewBoxAttribute
    }

    public class SvgLogo
    {
        private string _logoData;
        private string _mediaType;
        private int _iconSizePercent;

        /// <summary>
        /// Create a logo object to be used in SvgQRCode renderer
        /// </summary>
        /// <param name="iconRasterized">Logo to be rendered as Bitmap/rasterized graphic</param>
        /// <param name="iconSizePercent">Degree of percentage coverage of the QR code by the logo</param>
        public SvgLogo(Image iconRasterized, int iconSizePercent = 15)
        {
            _iconSizePercent = iconSizePercent;
            using (var ms = new System.IO.MemoryStream())
            {

                iconRasterized.Save(ms, new PngEncoder());
                _logoData = Convert.ToBase64String(ms.GetBuffer(), Base64FormattingOptions.None); 
            }
                
            _mediaType = "image/png";
        }

        /// <summary>
        /// Create a logo object to be used in SvgQRCode renderer
        /// </summary>
        /// <param name="iconVectorized">Logo to be rendered as SVG/vectorized graphic/string</param>
        /// <param name="iconSizePercent">Degree of percentage coverage of the QR code by the logo</param>
        public SvgLogo(string iconVectorized, int iconSizePercent = 15)
        {
            _iconSizePercent = iconSizePercent;
            _logoData = Convert.ToBase64String(Encoding.UTF8.GetBytes(iconVectorized), Base64FormattingOptions.None);
            _mediaType = "image/svg+xml";
        }

        public string GetDataUri()
        {
            return $"data:{_mediaType};base64,{_logoData}";
        }

        public int GetIconSizePercent()
        {
            return _iconSizePercent;
        }
    }
}

public static class SvgQRCodeHelper
{
    public static string GetQRCode(string plainText, int pixelsPerModule, string darkColorHex, string lightColorHex, ECCLevel eccLevel, bool forceUtf8 = false, bool utf8BOM = false, EciMode eciMode = EciMode.Default, int requestedVersion = -1, bool drawQuietZones = true, SizingMode sizingMode = SizingMode.WidthHeightAttribute, SvgLogo logo = null)
    {
        using (var qrGenerator = new QRCodeGenerator())
        using (var qrCodeData = qrGenerator.CreateQrCode(plainText, eccLevel, forceUtf8, utf8BOM, eciMode, requestedVersion))
        using (var qrCode = new SvgQRCode(qrCodeData))
            return qrCode.GetGraphic(pixelsPerModule, darkColorHex, lightColorHex, drawQuietZones, sizingMode, logo);
    }
}