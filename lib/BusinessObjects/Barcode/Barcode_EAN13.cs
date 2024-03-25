using System;
using System.Configuration;
using System.Drawing;
using System.Drawing.Imaging;
using System.Drawing.Text;
using System.IO;
using System.Text;

namespace LIB.BusinessObjects.Barcode
{
    public class Barcode_EAN13 : Barcode
    {
        public Barcode_EAN13(string code, float scale = 2.5f)
        {
            this.InitCode(ref code);

            // This is the nomimal size recommended by the EAN.
            this.Width = 49.29f * scale;
            this.Height = 12f * scale;
            this.FontSize = 7.0f * scale;
        }

        public Barcode_EAN13(string code, (float Width, float Height, float FontSize) config)
        {
            this.InitCode(ref code);

            this.Width = config.Width;
            this.Height = config.Height;
            this.FontSize = config.FontSize;
        }

        private void InitCode(ref string code) 
        {
            if (string.IsNullOrEmpty(code) is false)
            {
                if (code.Length > 12)
                    code = code.Substring(0, 12);


                if (code.Length != 5 && code.Length != 12)
                    code = CountryCode + ManufacturerCode + "00000";

                else if (code.Length == 5)
                    code = CountryCode + ManufacturerCode + code;
            }
            else
            {
                code = CountryCode + ManufacturerCode + "00000";
            }

            this.Code = code;
        }

        private static string CountryCode
        {
            get
            {
                var code = ConfigurationManager.AppSettings.Get("CountryCode");

                if (string.IsNullOrEmpty(code) is false && code.Length == 3)
                    return code;

                return "000";
            }
        }
        private static string ManufacturerCode
        {
            get
            {
                var code = ConfigurationManager.AppSettings.Get("ManufacturerCode");

                if (string.IsNullOrEmpty(code) is false && code.Length == 4)
                    return code;

                return "0000";
            }
        }
        public string ProductCode => this.Code.Substring(7, 5);

        private string ChecksumDigit;

        public static bool IsBarcode(string code) 
            => string.IsNullOrEmpty(code) is false && 
            (code.Length == 5 || (code.Length == 13 && code.StartsWith(CountryCode + ManufacturerCode)));

        public override string[] GetProductCodes() => new[] { this.ProductCode };

        #region Drow Barcode
        private readonly float Width;
        private readonly float Height;
        private readonly float FontSize;

        private static readonly string[] LeftOdd = { "0001101", "0011001", "0010011", "0111101", "0100011", "0110001", "0101111", "0111011", "0110111", "0001011" };
        private static readonly string[] LeftEven = { "0100111", "0110011", "0011011", "0100001", "0011101", "0111001", "0000101", "0010001", "0001001", "0010111" };
        private static readonly string[] Right = { "1110010", "1100110", "1101100", "1000010", "1011100", "1001110", "1010000", "1000100", "1001000", "1110100" };
        private const string QuiteZone = "000000000";
        private const string LeadTail = "101";
        private const string Separator = "01010";

        private void CalculateChecksumDigit()
        {
            string sTemp = CountryCode + ManufacturerCode + this.ProductCode;
            int iSum = 0;

            for (int i = sTemp.Length; i >= 1; i--)
            {
                int iDigit = Convert.ToInt32(sTemp.Substring(i - 1, 1));
                iSum += i % 2 == 0 ? iDigit * 3 : iDigit * 1;
            }
            int iCheckSum = (10 - (iSum % 10)) % 10;
            this.ChecksumDigit = iCheckSum.ToString();
        }

        private string ConvertToDigitPatterns(string[] patterns, string numbers)
        {
            var result = string.Empty;
            foreach (var number in numbers)
            {
                int.TryParse(number.ToString(), out var index);
                result += patterns[index];
            }
            return result;
        }

        private string ConvertLeftPattern(string sLeft)
        {
            switch (sLeft[0])
            {
                case '4':
                    return CountryCode_4(sLeft.Substring(1));
                default:
                    return string.Empty;
            }
        }

        private string CountryCode_4(string sLeft)
        {
            var sReturn = new StringBuilder();
            sReturn.Append(ConvertToDigitPatterns(LeftOdd, sLeft[0].ToString()));
            sReturn.Append(ConvertToDigitPatterns(LeftEven, sLeft[1].ToString()));
            sReturn.Append(ConvertToDigitPatterns(LeftOdd, sLeft[2].ToString()));
            sReturn.Append(ConvertToDigitPatterns(LeftOdd, sLeft[3].ToString()));
            sReturn.Append(ConvertToDigitPatterns(LeftEven, sLeft[4].ToString()));
            sReturn.Append(ConvertToDigitPatterns(LeftEven, sLeft[5].ToString()));
            return sReturn.ToString();
        }

        private void DrawEan13Barcode(Graphics g, Point pt)
        {
            // EAN13 Barcode should be a total of 113 modules wide.
            float lineWidth = this.Width / 113f;

            var gs = g.Save();

            g.PageUnit = GraphicsUnit.Millimeter; // Set the PageUnit to Inch because all of our measurements are in inches
            g.PageScale = 1; // For a true millimeter
            g.TextRenderingHint = TextRenderingHint.AntiAlias;

            var brush = new SolidBrush(Color.Black);

            float xPosition = 0;

            float xStart = pt.X;
            float yStart = pt.Y;

            var font = new Font("Arial", Convert.ToSingle(this.FontSize));

            // Calculate the Check Digit
            this.CalculateChecksumDigit();

            string sTemp = CountryCode + ManufacturerCode + this.ProductCode + this.ChecksumDigit;

            // Convert the left hand numbers
            string sLeftPattern = ConvertLeftPattern(sTemp.Substring(0, 7));

            // Build the UPC Code
            string sTempUPC = QuiteZone + LeadTail + sLeftPattern + Separator + ConvertToDigitPatterns(Right, sTemp.Substring(7)) + LeadTail + QuiteZone;

            float fTextHeight = g.MeasureString(sTempUPC, font).Height;

            // Draw the barcode lines.
            for (int i = 0; i < sTempUPC.Length; i++)
            {
                if (sTempUPC.Substring(i, 1) == "1")
                {
                    if (xStart == pt.X)
                        xStart = xPosition;

                    // Save room for the UPC number below the bar code.
                    if ((i > 12 && i < 55) || (i > 57 && i < 101))
                        g.FillRectangle(brush, xPosition, yStart, lineWidth, this.Height - fTextHeight); // Draw space for the number
                    else
                        g.FillRectangle(brush, xPosition, yStart, lineWidth, this.Height - fTextHeight * 0.66f); // Draw a full line.
                }

                xPosition += lineWidth;
            }

            // Draw the upc numbers below the line.
            xPosition = xStart - g.MeasureString(CountryCode.Substring(0, 1), font).Width;
            float yPosition = yStart + (this.Height - fTextHeight);

            var point = new PointF(xPosition, yPosition);

            // Draw 1st digit of the country code
            DrawStringWhihSpace(g, sTemp.Substring(0, 1), font, brush, ref point, lineWidth * 1.8f);

            point.X += lineWidth * 3;

            // Draw Left Text
            DrawStringWhihSpace(g, sTemp.Substring(1, 6), font, brush, ref point, lineWidth * 1.8f);

            point.X += lineWidth * 4;

            // Draw Right Text
            DrawStringWhihSpace(g, sTemp.Substring(7), font, brush, ref point, lineWidth * 1.8f);

            g.Restore(gs);
        }

        private void DrawStringWhihSpace(Graphics g, string text, Font font, Brush brush, ref PointF point, float space)
        {
            foreach (var @char in text)
            {
                g.DrawString(@char.ToString(), font, brush, point);
                float widthTemp = g.MeasureString(@char.ToString(), font).Width;

                point.X += widthTemp + space;
            }
        }

        private Bitmap CreateBitmap(int Width, int Height)
        {
            var bmp = new Bitmap(Width, Height);

            using (var g = Graphics.FromImage(bmp))
                this.DrawEan13Barcode(g, new Point(0, 0));

            return bmp;
        }
        #endregion

        public override string GenerateBarcode(int width, int height)
        {
            byte[] barcodeData;

            using (var ms = new MemoryStream())
            {
                using (Image image = this.CreateBitmap(width, height))
                    image.Save(ms, ImageFormat.Png);

                barcodeData = ms.ToArray();
            }

            return Convert.ToBase64String(barcodeData);
        }
    }
}