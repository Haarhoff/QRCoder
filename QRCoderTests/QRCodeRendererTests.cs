using System;
using Xunit;
using QRCoder;
using Shouldly;
using QRCoderTests.XUnitExtenstions;
using System.IO;
using System.Security.Cryptography;
using SixLabors.ImageSharp.Formats.Png;


namespace QRCoderTests
{

    public class QRCodeRendererTests
    {


        [Fact]
        [Category("QRRenderer/QRCode")]
        public void can_create_standard_qrcode_graphic()
        {
            var gen = new QRCodeGenerator();
            var data = gen.CreateQrCode("This is a quick test! 123#?", QRCodeGenerator.ECCLevel.H);
            var bmp = new QRCode(data).GetGraphic(10);
            
            using var ms = new MemoryStream();
            bmp.Save(ms, new PngEncoder());
            var imgBytes = ms.ToArray();
            var md5 = MD5.Create();
            var hash = md5.ComputeHash(imgBytes);
            var result = BitConverter.ToString(hash).Replace("-", "").ToLower();

            result.ShouldBe("c0f8af4256eddc7e566983e539cce389");
        }

/*

#if !NETCOREAPP1_1 && !NETCOREAPP2_0

        private string GetAssemblyPath()
        {
            return
#if NET5_0
                AppDomain.CurrentDomain.BaseDirectory;
#else
                Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().CodeBase).Replace("file:\\", "");
#endif
        }

        [Fact]
        [Category("QRRenderer/QRCode")]
        public void can_create_qrcode_with_transparent_logo_graphic()
        {        
            //Create QR code
            var gen = new QRCodeGenerator();
            var data = gen.CreateQrCode("This is a quick test! 123#?", QRCodeGenerator.ECCLevel.H);

            var bmp = new QRCode(data).GetGraphic(10, Color.Black, Color.Transparent, icon: Image.Load(GetAssemblyPath() + "\\assets\\noun_software engineer_2909346.png"));
            //Used logo is licensed under public domain. Ref.: https://thenounproject.com/Iconathon1/collection/redefining-women/?i=2909346

            var imgBytes = PixelsToAveragedByteArray(bmp);
            var md5 = new MD5CryptoServiceProvider();
            var hash = md5.ComputeHash(imgBytes);
            var result = BitConverter.ToString(hash).Replace("-", "").ToLower();

            result.ShouldBe("33c250bf306b7cbbd3dd71b6029b8784");
        }

        [Fact]
        [Category("QRRenderer/QRCode")]
        public void can_create_qrcode_with_non_transparent_logo_graphic()
        {
            //Create QR code
            var gen = new QRCodeGenerator();
            var data = gen.CreateQrCode("This is a quick test! 123#?", QRCodeGenerator.ECCLevel.H);
            var bmp = new QRCode(data).GetGraphic(10, Color.Black, Color.White, icon: Image.Load(GetAssemblyPath() + "\\assets\\noun_software engineer_2909346.png"));
            //Used logo is licensed under public domain. Ref.: https://thenounproject.com/Iconathon1/collection/redefining-women/?i=2909346

            var imgBytes = PixelsToAveragedByteArray(bmp);
            var md5 = new MD5CryptoServiceProvider();
            var hash = md5.ComputeHash(imgBytes);
            var result = BitConverter.ToString(hash).Replace("-", "").ToLower();

            result.ShouldBe("33c250bf306b7cbbd3dd71b6029b8784");
        }


        private static byte[] PixelsToAveragedByteArray(Image bmp)
        {
            //Re-color
            var bmpTmp = new Image<L8>(bmp.Width, bmp.Height);
            bmpTmp.Mutate(x => x.DrawImage(bmp, new Point());
            
            //Downscale
            var bmpSmall = new Bitmap(bmpTmp, new Size(16, 16));

            var bytes = new System.Collections.Generic.List<byte>();
            for (int x = 0; x < bmpSmall.Width; x++)
            {
                for (int y = 0; y < bmpSmall.Height; y++)
                {
                    bytes.AddRange(new byte[] { bmpSmall.GetPixel(x, y).R, bmpSmall.GetPixel(x, y).G, bmpSmall.GetPixel(x, y).B });
                }
            }
            return bytes.ToArray();
        }

#endif */
    }
}