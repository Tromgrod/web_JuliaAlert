using System;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using BarcodeLib;

namespace LIB.BusinessObjects.Barcode
{
    public class Barcode_CODE128 : Barcode
    {
        public Barcode_CODE128(string code)
        {
            this.InitByCode(code);

            this.InitBarcode(code, TYPE.CODE128);
        }

        public Barcode_CODE128(string code, Type type)
        {
            if (Enum.IsDefined(typeof(Type), (int)type) && type == Type.None)
            {
                this.InitByCode(code);
            }
            else if (string.IsNullOrEmpty(code) is false)
            {
                this.BarType = type;
                code = GetPrefix(type) + code;
            }
            else
                this.Error = true;

            this.InitBarcode(code, TYPE.CODE128);
        }

        private BarcodeLib.Barcode BarcodeItem;

        public Type BarType { get; private set; } = Type.None;

        private void InitBarcode(string code, TYPE type)
        {
            this.Code = code;

            this.BarcodeItem = new BarcodeLib.Barcode()
            {
                EncodedType = type,
                IncludeLabel = true,
                LabelFont = new Font("Arial", 14),
                StandardizeLabel = true
            };
        }

        public override string[] GetProductCodes() => this.Code.Replace(GetPrefix(this.BarType), string.Empty).Split('-');

        private static string GetPrefix(Type type) => (int)type + " ";

        public static bool IsBarcode(string code, Type type = Type.None)
            => string.IsNullOrEmpty(code) is false &&
               code.Contains("-") &&
               (type == Type.None || (int.TryParse(code?.Split(' ')?.First(), out var typeInt) &&
               Enum.IsDefined(typeof(Type), typeInt)));

        private void InitByCode(string code)
        {
            int.TryParse(code?.Split(' ')?.First(), out var type);

            if (Enum.IsDefined(typeof(Type), type))
                this.BarType = (Type)type;
            else
                this.Error = true;
        }

        public override string GenerateBarcode(int width, int height)
        {
            byte[] barcodeData;

            try
            {
                using (var ms = new MemoryStream())
                {
                    this.BarcodeItem.Width = width;
                    this.BarcodeItem.Height = height;

                    var image = this.BarcodeItem.Encode(this.BarcodeItem.EncodedType, this.Code);

                    image.Save(ms, ImageFormat.Png);
                    barcodeData = ms.ToArray();
                }
            }
            catch
            {
                barcodeData = new byte[default];
            }

            return Convert.ToBase64String(barcodeData);
        }

        public enum Type
        {
            None = 0,
            Finding = 1,
            Textile = 2
        }
    }
}