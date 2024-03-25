using LIB.Tools.BO;

namespace LIB.BusinessObjects.Barcode
{
    public abstract class Barcode
    {
        public bool Error { get; protected set; }

        public string Code { get; protected set; }

        public abstract string[] GetProductCodes();

        public abstract string GenerateBarcode(int width, int height);

        public T GetObject<T>() where T : ItemBase, new()
        {
            var item = new T();

            item.SetByFullCode(this.GetProductCodes());

            return item;
        }
    }
}