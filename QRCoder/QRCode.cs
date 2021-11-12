using System;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.Drawing;
using SixLabors.ImageSharp.Drawing.Processing;
using SixLabors.ImageSharp.PixelFormats;
using SixLabors.ImageSharp.Processing;
using static QRCoder.QRCodeGenerator;

namespace QRCoder
{
    public class QRCode : AbstractQRCode, IDisposable
    {
        /// <summary>
        /// Constructor without params to be used in COM Objects connections
        /// </summary>
        public QRCode() { }

        public QRCode(QRCodeData data) : base(data) {}

        public Image GetGraphic(int pixelsPerModule)
        {
            return this.GetGraphic(pixelsPerModule, Color.Black, Color.White, true);
        }

        public Image GetGraphic(int pixelsPerModule, string darkColorHtmlHex, string lightColorHtmlHex, bool drawQuietZones = true)
        {
            return GetGraphic(pixelsPerModule, Color.ParseHex(darkColorHtmlHex), Color.ParseHex(lightColorHtmlHex), drawQuietZones);
        }

        public Image GetGraphic(int pixelsPerModule, Color darkColor, Color lightColor, bool drawQuietZones = true)
        {
            var size = (QrCodeData.ModuleMatrix.Count - (drawQuietZones ? 0 : 8)) * pixelsPerModule;
            var offset = drawQuietZones ? 0 : 4 * pixelsPerModule;
            
            var bmp = new Image<Rgba32>(size, size);
            var lightBrush = Brushes.Solid(lightColor);
            var darkBrush = Brushes.Solid(darkColor);

            for (var x = 0; x < size + offset; x = x + pixelsPerModule)
            {
                for (var y = 0; y < size + offset; y = y + pixelsPerModule) {
                    var module = this.QrCodeData.ModuleMatrix[(y + pixelsPerModule) / pixelsPerModule - 1][(x + pixelsPerModule) / pixelsPerModule - 1];

                    if (module)
                    {
                        var rect = new RectangularPolygon(x - offset, y - offset, pixelsPerModule, pixelsPerModule);
                        bmp.Mutate(x => x.Fill(darkBrush, rect));
                    }
                    else
                    {
                        var rect = new RectangularPolygon(x - offset, y - offset, pixelsPerModule, pixelsPerModule);
                        bmp.Mutate(x => x.Fill(lightBrush, rect));
                    }
                }
            }
            return bmp;
        }

        public Image GetGraphic(int pixelsPerModule, Color darkColor, Color lightColor, Image icon=null, int iconSizePercent=15, int iconBorderWidth = 6, bool drawQuietZones = true)
        {
            var size = (this.QrCodeData.ModuleMatrix.Count - (drawQuietZones ? 0 : 8)) * pixelsPerModule;
            var offset = drawQuietZones ? 0 : 4 * pixelsPerModule;

            var bmp = new Image<Argb32>(size, size);
            var lightBrush = Brushes.Solid(lightColor);
            var darkBrush = Brushes.Solid(darkColor);
            
            bmp.Mutate(x => x.Fill(lightColor));

                var drawIconFlag = icon != null && iconSizePercent > 0 && iconSizePercent <= 100;
                float iconX = 0, iconY = 0;
                int iconDestWidth = 0, iconDestHeight = 0;

                if (drawIconFlag)
                {
                    iconDestWidth = (int)(iconSizePercent * bmp.Width / 100f);
                    iconDestHeight = drawIconFlag ? iconDestWidth * icon.Height / icon.Width : 0;
                    iconX = ((float)bmp.Width - iconDestWidth) / 2;
                    iconY = ((float)bmp.Height - iconDestHeight) / 2;

                    var centerDest = new RectangularPolygon(iconX - iconBorderWidth, iconY - iconBorderWidth, iconDestWidth + iconBorderWidth * 2, iconDestHeight + iconBorderWidth * 2);
                }

                for (var x = 0; x < size + offset; x = x + pixelsPerModule)
                {
                    for (var y = 0; y < size + offset; y = y + pixelsPerModule)
                    {
                        var moduleBrush = QrCodeData.ModuleMatrix[(y + pixelsPerModule) / pixelsPerModule - 1][(x + pixelsPerModule) / pixelsPerModule - 1] ? darkBrush : lightBrush;
                        var rect = new RectangularPolygon(x - offset, y - offset, pixelsPerModule, pixelsPerModule);
                        bmp.Mutate(i => i.Fill(moduleBrush, rect));
                    }
                }

                if (drawIconFlag)
                {
                    icon = icon.Clone(x =>
                    {
                        x.Resize(iconDestWidth, iconDestHeight);
                        var corners = CreateRoundedRectanglePath(iconDestWidth, iconDestHeight, iconBorderWidth * 2);
                        x.SetGraphicsOptions(new GraphicsOptions()
                        {
                            Antialias = true,
                            AlphaCompositionMode = PixelAlphaCompositionMode.DestOut
                        });
                        foreach (var c in corners)
                        {
                            x = x.Fill(Color.Red, c);
                        }
                    });
                        
                    bmp.Mutate(i => i.DrawImage(icon, new Point((int)iconX, (int)iconY), 1));
                }
            
            return bmp;
        }

        internal IPathCollection CreateRoundedRectanglePath(int imageWidth, int imageHeight, int cornerRadius)
        {
            var roundedRect = new RectangularPolygon(-0.5f, -0.5f, cornerRadius, cornerRadius);

            var cornerTopLeft =
                roundedRect.Clip(new EllipsePolygon(cornerRadius - 0.5f, cornerRadius - 0.5f, cornerRadius));
            var rightPos = imageWidth - cornerTopLeft.Bounds.Width + 1;
            var bottomPos = imageHeight - cornerTopLeft.Bounds.Height + 1;
            
            var cornerTopRight = cornerTopLeft.RotateDegree(90).Translate(rightPos, 0);
            var cornerBottomLeft = cornerTopLeft.RotateDegree(-90).Translate(0, bottomPos);
            var cornerBottomRight = cornerTopLeft.RotateDegree(180).Translate(rightPos, bottomPos);

            return new PathCollection(cornerTopLeft, cornerBottomLeft, cornerTopRight, cornerBottomRight);
        }
    }

    public static class QRCodeHelper
    {
        public static Image GetQRCode(string plainText, int pixelsPerModule, Color darkColor, Color lightColor, ECCLevel eccLevel, bool forceUtf8 = false, bool utf8BOM = false, EciMode eciMode = EciMode.Default, int requestedVersion = -1, Image icon = null, int iconSizePercent = 15, int iconBorderWidth = 6, bool drawQuietZones = true)
        {
            using (var qrGenerator = new QRCodeGenerator())
            using (var qrCodeData = qrGenerator.CreateQrCode(plainText, eccLevel, forceUtf8, utf8BOM, eciMode, requestedVersion))
            using (var qrCode = new QRCode(qrCodeData))
                return qrCode.GetGraphic(pixelsPerModule, darkColor, lightColor, icon, iconSizePercent, iconBorderWidth, drawQuietZones);
        }
    }
}