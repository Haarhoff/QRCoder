using System;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.Drawing;
using SixLabors.ImageSharp.Drawing.Processing;
using SixLabors.ImageSharp.PixelFormats;
using SixLabors.ImageSharp.Processing;
/*
// pull request raised to extend library used. 
namespace QRCoder
{
  
   public class ArtQRCode : AbstractQRCode, IDisposable
   {
       /// <summary>
       /// Constructor without params to be used in COM Objects connections
       /// </summary>
       public ArtQRCode() { }

       public ArtQRCode(QRCodeData data) : base(data) { }

       public Image GetGraphic(int pixelsPerModule)
       {
           return this.GetGraphic(pixelsPerModule, (pixelsPerModule * 8) / 10, Color.Black, Color.White);
       }

       public Image GetGraphic(Image backgroundImage = null)
       {
           return this.GetGraphic(10, 7, Color.Black, Color.White, backgroundImage: backgroundImage);
       }

       public Image GetGraphic(
           int pixelsPerModule,
           int pixelSize,
           Color darkColor,
           Color lightColor,
           bool drawQuietZones = false,
           Image reticleImage = null,
           Image backgroundImage = null)
       {
           var numModules = this.QrCodeData.ModuleMatrix.Count - (drawQuietZones ? 0 : 8);
           var offset = (drawQuietZones ? 0 : 4);
           var size = numModules * pixelsPerModule;

           var bitmap = Resize(backgroundImage,size) ?? new Image<Argb32>(size, size);

           
               var lightBrush = Brushes.Solid(lightColor);
               
                   var darkBrush = Brushes.Solid(darkColor);
                   
                       // make background transparent if you don't have an image -- not sure this is needed
                       if (backgroundImage == null)
                       {
                           bitmap.Mutate(x => x.Fill(Color.Transparent));
                       }

                       var darkModulePixel = MakeDotPixel(pixelsPerModule, pixelSize, darkBrush);
                       var lightModulePixel = MakeDotPixel(pixelsPerModule, pixelSize, lightBrush);

                       for (var x = 0; x < numModules; x += 1)
                       {
                           for (var y = 0; y < numModules; y += 1)
                           {
                               var rectangleF = new RectangularPolygon(x * pixelsPerModule, y * pixelsPerModule, pixelsPerModule, pixelsPerModule);

                               var pixelIsDark = this.QrCodeData.ModuleMatrix[offset + y][offset + x];
                               var solidBrush = pixelIsDark ? darkBrush : lightBrush;
                               var pixelImage = pixelIsDark ? darkModulePixel : lightModulePixel;

                               if (!IsPartOfReticle(x, y, numModules, offset))
                                   bitmap.Mutate(ctx => ctx.DrawImage(pixelImage, new Point(x * pixelsPerModule, y * pixelsPerModule), 1));
                               else if (reticleImage == null)
                                   bitmap.Mutate(ctx => ctx.Fill(solidBrush, rectangleF));
                           }
                       }

                       if (reticleImage != null)
                       {
                           var reticleSize = 7 * pixelsPerModule;
                           var reticle = reticleImage.Clone(ctx => ctx.Resize(reticleSize, reticleSize));
                           bitmap.Mutate(ctx =>
                           {
                               ctx.DrawImage(reticle, new Point(0, 0), 1);
                               ctx.DrawImage(reticle, new Point(size - reticleSize, 0), 1);
                               ctx.DrawImage(reticle, new Point(0, size - reticleSize), 1);
                           });
                           
                       }
                   
           return bitmap;
       }

       /// <summary>
       /// If the pixelSize is bigger than the pixelsPerModule or may end up filling the Module making a traditional QR code.
       /// </summary>
       /// <param name="pixelsPerModule"></param>
       /// <param name="pixelSize"></param>
       /// <param name="brush"></param>
       /// <returns></returns>
      /* private Image MakeDotPixel(int pixelsPerModule, int pixelSize, SolidBrush brush)
       {
           // draw a dot
           var bitmap = new Image<Argb32>(pixelSize, pixelSize);
           var ellipse = new EllipsePolygon(0, 0, pixelSize);

           var pixelWidth = Math.Min(pixelsPerModule, pixelSize);
           var margin = Math.Max((pixelsPerModule - pixelWidth) / 2, 0);

           // center the dot in the module and crop to stay the right size.
           var cropped = new Bitmap(pixelsPerModule, pixelsPerModule);
           using (var graphics = Graphics.FromImage(cropped))
           {
               graphics.DrawImage(bitmap, new Rectangle(margin, margin, pixelWidth, pixelWidth),
                   new RectangleF(((float)pixelSize - pixelWidth) / 2, ((float)pixelSize - pixelWidth) / 2, pixelWidth, pixelWidth),
                   GraphicsUnit.Pixel);
               graphics.Save();
           }

           return cropped;
       }

       private bool IsPartOfReticle(int x, int y, int numModules, int offset)
       {
           var cornerSize = 11 - offset;
           return
               (x < cornerSize && y < cornerSize) ||
               (x > (numModules - cornerSize - 1) && y < cornerSize) ||
               (x < cornerSize && y > (numModules - cornerSize - 1));
       }

       /// <summary>
       /// Resize to a square bitmap, but maintain the aspect ratio by padding transparently.
       /// </summary>
       /// 
       /// <param name="image"></param>
       /// <param name="newSize"></param>
       /// <returns></returns>
       private Image Resize(Image image, int newSize)
       {
           if (image == null) return null;

           float scale = Math.Min((float)newSize / image.Width, (float)newSize / image.Height);
           var scaledWidth = (int)(image.Width * scale);
           var scaledHeight = (int)(image.Height * scale);
           var offsetX = (newSize - scaledWidth) / 2;
           var offsetY = (newSize - scaledHeight) / 2;

           
           var scaledImage = image.Clone(x =>
           {
               
               x.Resize(scaledWidth, scaledHeight);
           });

           var bm = new Image<Argb32>(newSize, newSize);
           bm.Mutate(x =>
           {
               x.SetGraphicsOptions(new GraphicsOptions
               {
                   Antialias = true,
               });

               x.Fill(Color.Transparent);
               x.DrawImage(scaledImage, new Point(offsetX, offsetY), 1);
           });

           return bm;
       }
   }
} */