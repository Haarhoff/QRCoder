using System;
using Xunit;
using QRCoder;
using Shouldly;
using QRCoderTests.XUnitExtenstions;
using System.IO;
using System.Security.Cryptography;
using SixLabors.ImageSharp;


namespace QRCoderTests
{

    public class SvgQRCodeRendererTests
    {

        private string GetAssemblyPath()
        {
            return AppDomain.CurrentDomain.BaseDirectory;

        }

        [Fact]
        [Category("QRRenderer/SvgQRCode")]
        public void can_render_svg_qrcode()
        {        
            //Create QR code
            var gen = new QRCodeGenerator();
            var data = gen.CreateQrCode("This is a quick test! 123#?", QRCodeGenerator.ECCLevel.H);
            var svg = new SvgQRCode(data).GetGraphic(10, Color.Red, Color.White);

            var md5 = MD5.Create();
            var hash = md5.ComputeHash(System.Text.Encoding.UTF8.GetBytes(svg));
            var result = BitConverter.ToString(hash).Replace("-", "").ToLower();
            
            result.ShouldBe("879b36fdcb31c4b9e631ce427251b1dc");
        }

        [Fact]
        [Category("QRRenderer/SvgQRCode")]
        public void can_render_svg_qrcode_without_quietzones()
        {
            //Create QR code
            var gen = new QRCodeGenerator();
            var data = gen.CreateQrCode("This is a quick test! 123#?", QRCodeGenerator.ECCLevel.H);
            var svg = new SvgQRCode(data).GetGraphic(10, Color.Red, Color.White, false);
            File.WriteAllText("/Users/hh_21/Desktop/test2.svg", svg);
            var md5 = new MD5CryptoServiceProvider();
            var hash = md5.ComputeHash(System.Text.Encoding.UTF8.GetBytes(svg));
            var result = BitConverter.ToString(hash).Replace("-", "").ToLower();

            result.ShouldBe("aa1f74ebca35ffebad0c60a1297792e4");
        }

        [Fact]
        [Category("QRRenderer/SvgQRCode")]
        public void can_render_svg_qrcode_with_png_logo()
        {
            //Create QR code
            var gen = new QRCodeGenerator();
            var data = gen.CreateQrCode("This is a quick test! 123#?", QRCodeGenerator.ECCLevel.H);

            //Used logo is licensed under public domain. Ref.: https://thenounproject.com/Iconathon1/collection/redefining-women/?i=2909346
            var logoBitmap = Image.Load(GetAssemblyPath() + "assets/noun_software engineer_2909346.png");
            var logoObj = new SvgQRCode.SvgLogo(logoBitmap, 15);

            var svg = new SvgQRCode(data).GetGraphic(10, Color.DarkGray, Color.White, logo: logoObj);
            
            var md5 = MD5.Create();
            var hash = md5.ComputeHash(System.Text.Encoding.UTF8.GetBytes(svg));
            var result = BitConverter.ToString(hash).Replace("-", "").ToLower();

            result.ShouldBe("d91bf0aed6ac33d7aacc7f8a55161d5d");
        }

        [Fact]
        [Category("QRRenderer/SvgQRCode")]
        public void can_render_svg_qrcode_with_svg_logo()
        {
            //Create QR code
            var gen = new QRCodeGenerator();
            var data = gen.CreateQrCode("This is a quick test! 123#?", QRCodeGenerator.ECCLevel.H);

            //Used logo is licensed under public domain. Ref.: https://thenounproject.com/Iconathon1/collection/redefining-women/?i=2909361
            var logoSvg = File.ReadAllText(GetAssemblyPath() + "assets/noun_Scientist_2909361.svg");
            var logoObj = new SvgQRCode.SvgLogo(logoSvg, 30);

            var svg = new SvgQRCode(data).GetGraphic(10, Color.DarkGray, Color.White, logo: logoObj);
            
            var md5 = MD5.Create();
            var hash = md5.ComputeHash(System.Text.Encoding.UTF8.GetBytes(svg));
            var result = BitConverter.ToString(hash).Replace("-", "").ToLower();

            result.ShouldBe("8deda066b9b703c2c3fccce2df2a909e");
        }
    }
}



