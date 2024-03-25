using System.Web.Mvc;
using System.Linq;
using System.Collections.Generic;
using Weblib.Controllers;
using JuliaAlert.Models.Objects;
using LIB.Tools.Security;
using LIB.BusinessObjects.Barcode;

namespace JuliaAlert.Controllers
{
    public class ProductController : BaseController
    {
        public JsonResult GetUniqueProduct()
        {
            long.TryParse(Request.Form["UniqueProductId"], out long UniqueProductId);

            var UniqueProduct = new UniqueProduct(UniqueProductId).PopulateById();

            decimal? Price = default;

            if (long.TryParse(Request.Form["SalesChannelId"], out long SalesChannelId))
            {
                Price = ProductPrice.PopulateByProduct(UniqueProduct.Product).FirstOrDefault(pp => pp.SalesChannel.Id == SalesChannelId)?.Price;
            }

            Dictionary<string, object> Data = new Dictionary<string, object>
            {
                { "ColorProduct", UniqueProduct.ColorProduct.Id },
                { "ColorProductName", UniqueProduct.ColorProduct.Name },
                { "Decor", UniqueProduct.Decor.Id },
                { "DecorName", UniqueProduct.Decor.Name },
                { "Code", UniqueProduct.GetCode() },
                { "Product", UniqueProduct.Product.Id },
                { "LastFactoryPrice", UniqueProduct.LastFactoryPrice.ToString("F") },
                { "LastTailoringCost", UniqueProduct.LastTailoringCost },
                { "LastFactoryTailoring", UniqueProduct.LastFactoryTailoring?.Id },
                { "LastCutCost", UniqueProduct.LastCutCost },
                { "LastFactoryCut", UniqueProduct.LastFactoryCut?.Id },
                { "LastSupplySpecificProductUnit", UniqueProduct.LastSupplySpecificProductUnit?.Id },
                { "Price", Price }
            };

            return this.Json(Data);
        }

        public void UpdateEnabledUniqueModel()
        {
            if (!Authentication.CheckUser(this.HttpContext))
                return;

            var UniqueProductIdStr = Request.Form["UniqueProductId"];
            var EnabledStr = Request.Form["Enabled"];

            if (long.TryParse(UniqueProductIdStr, out var UniqueProductId) && byte.TryParse(EnabledStr, out var Enabled))
            {
                var UniqueProduct = new UniqueProduct(UniqueProductId).PopulateById();

                UniqueProduct.UpdateProperties(nameof(UniqueProduct.Enabled), Enabled);
            }
        }

        public ViewResult GetTypeProductOptions()
        {
            long.TryParse(Request.Form["GroupProductId"], out var GroupProductId);

            ViewData["AllowDefault"] = true;

            return View("../Controls/_dropdown_options", TypeProduct.PopulateByGroupProduct(GroupProductId).Values.ToList());
        }

        public ViewResult GetUniqueProductSizeOptions()
        {
            long.TryParse(Request.Form["UniqueProductId"], out var UniqueProductId);

            ViewData["AllowDefault"] = true;

            return View("../Controls/_dropdown_options", new UniqueProduct(UniqueProductId).GetSizes().Values.ToList());
        }

        public string GetSpecificProductBarcode()
        {
            long.TryParse(Request.Form["UniqueProductId"], out var uniqueProductId);
            long.TryParse(Request.Form["ProductSizeId"], out var productSizeId);
            int.TryParse(Request.Form["WidthBarcode"], out var widthBarcode);
            int.TryParse(Request.Form["HeightBarcode"], out var heightBarcode);

            var specificProduct = SpecificProduct.GetByUniqueProductAndSize(uniqueProductId, productSizeId);

            var barcode = new Barcode_EAN13(specificProduct.ProductCode.ToString("00000"));

            return barcode.GenerateBarcode(widthBarcode, heightBarcode);
        }

        public ViewResult GetProductPrices()
        {
            long.TryParse(Request.Form["ProductId"], out var productId);

            var product = new Product(productId).PopulateOne();

            return View("ProductPrices", product);
        }
    }
}