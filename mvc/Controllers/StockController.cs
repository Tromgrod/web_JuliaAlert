using Weblib.Controllers;
using JuliaAlert.Models.Objects;

namespace JuliaAlert.Controllers
{
    public class StockController : BaseController
    {
        public int GetStockCurrentCount()
        {
            long.TryParse(Request.Form["StockId"], out var stockId);
            long.TryParse(Request.Form["UniqueProductId"], out var uniqueProductId);
            long.TryParse(Request.Form["ProductSizeId"], out var productSizeId);

            var specificProduct = SpecificProduct.GetByUniqueProductAndSize(uniqueProductId, productSizeId);

            return SpecificProductStock.GetBySpecificProduct(specificProduct, new Stock(stockId)).CurrentCount;
        }
    }
}