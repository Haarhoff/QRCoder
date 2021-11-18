using System;
using Xunit;
using QRCoder;
using Shouldly;
using QRCoderTests.XUnitExtenstions;
using System.IO;
using System.Security.Cryptography;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.Formats.Png;
using SixLabors.ImageSharp.PixelFormats;
using SixLabors.ImageSharp.Processing;
using SixLabors.ImageSharp.Processing.Processors.Transforms;

namespace QRCoderTests;

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

    private string GetAssemblyPath()
    {
        return AppDomain.CurrentDomain.BaseDirectory;
    }

    [Fact]
    [Category("QRRenderer/QRCode")]
    public void can_create_qrcode_with_transparent_logo_graphic()
    {        
        //Create QR code
        var gen = new QRCodeGenerator();
        var data = gen.CreateQrCode("This is a quick test! 123#?", QRCodeGenerator.ECCLevel.H);

        var bmp = new QRCode(data).GetGraphic(10, Color.Black, Color.Transparent, icon: Image.Load(GetAssemblyPath() + "/assets/noun_software engineer_2909346.png"));
        //Used logo is licensed under public domain. Ref.: https://thenounproject.com/Iconathon1/collection/redefining-women/?i=2909346
        
        var imgBytes = PixelsToAveragedByteArray(bmp);
        var md5 = MD5.Create();
        var hash = md5.ComputeHash(imgBytes);
        var result = BitConverter.ToString(hash).Replace("-", "").ToLower();

        result.ShouldBe("ce715c4d2b8d8e7cb750b1c3414516fd");
    }

        [Fact]
        [Category("QRRenderer/QRCode")]
        public void can_create_qrcode_with_non_transparent_logo_graphic()
        {
            //Create QR code
            var gen = new QRCodeGenerator();
            var data = gen.CreateQrCode("This is a quick test! 123#?", QRCodeGenerator.ECCLevel.H);
            var bmp = new QRCode(data).GetGraphic(10, Color.Black, Color.White, icon: Image.Load(GetAssemblyPath() + "/assets/noun_software engineer_2909346.png"));
            //Used logo is licensed under public domain. Ref.: https://thenounproject.com/Iconathon1/collection/redefining-women/?i=2909346
            bmp.SaveAsPng("/Users/hh_21/Desktop/test.png");
            var imgBytes = PixelsToAveragedByteArray(bmp);
            var md5 = MD5.Create();
            var hash = md5.ComputeHash(imgBytes);
            var result = BitConverter.ToString(hash).Replace("-", "").ToLower();

            result.ShouldBe("45ac15403c89ade87b3110b3e1aa443f");
        }
        
    private static byte[] PixelsToAveragedByteArray(Image bmp)
    {
        //Re-color
        var bmpTmp = new Image<Rgba32>(bmp.Width, bmp.Height);
        bmpTmp.Mutate(x => x.DrawImage(bmp, new Point(0,0), 1));
            
        //Downscale
        var bmpSmall = bmpTmp.Clone(x => x.Resize(16, 16, new BicubicResampler()));

        var bytes = new System.Collections.Generic.List<byte>();
        for (var x = 0; x < bmpSmall.Width; x++)
        {
            for (var y = 0; y < bmpSmall.Height; y++)
            {
                bytes.AddRange(new [] { bmpSmall[x,y].R, bmpSmall[x, y].G, bmpSmall[x, y].B });
            }
        }
        return bytes.ToArray();
    }
}