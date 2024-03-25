using System.Web.Mvc;
using System.Web;
using Weblib.Controllers;
using JuliaAlert.Models.Objects;
using LIB.Tools.Security;
using LIB.Helpers;
using LIB.Tools.Utils;
using System.Linq;
using LIB.Tools.BO;
using System;

namespace JuliaAlert.Controllers
{
    public class ProductionController : BaseController
    {
        public JsonResult GetTextile()
        {
            if (!Authentication.CheckUser(this.HttpContext))
                return this.Json(new RequestResult() { RedirectURL = Config.GetConfigValue("LoginPage") + "?ReturnUrl=" + HttpUtility.UrlEncode("Account/Manage"), Result = RequestResultType.Reload });

            long.TryParse(Request.Form["TextileId"], out var TextileId);

            if (TextileId > 0)
            {
                var textile = new Textile(TextileId).PopulateById();

                return this.Json(textile);
            }
            else
                return null;
        }

        public JsonResult GetFinding()
        {
            if (!Authentication.CheckUser(this.HttpContext))
                return this.Json(new RequestResult() { RedirectURL = Config.GetConfigValue("LoginPage") + "?ReturnUrl=" + HttpUtility.UrlEncode("Account/Manage"), Result = RequestResultType.Reload });

            long.TryParse(Request.Form["FindingId"], out var FindingId);

            if (FindingId > 0)
            {
                var finding = new Finding(FindingId).PopulateById();

                return this.Json(finding);
            }
            else
                return null;
        }

        public JsonResult GetImplementSupplySpecificProductUnit()
        {
            if (!Authentication.CheckUser(this.HttpContext))
                return this.Json(new RequestResult() { RedirectURL = Config.GetConfigValue("LoginPage") + "?ReturnUrl=" + HttpUtility.UrlEncode("Account/Manage"), Result = RequestResultType.Reload });

            long.TryParse(Request.Form["ImplementSupplySpecificProductUnitId"], out var implementSupplySpecificProductUnitId);

            if (implementSupplySpecificProductUnitId > 0)
            {
                var implementSupplySpecificProductUnit = ImplementSupplySpecificProductUnit.PopulateById(implementSupplySpecificProductUnitId);

                var data = new { implementSupplySpecificProductUnit.Id, implementSupplySpecificProductUnit.Count, implementSupplySpecificProductUnit.DateString };

                return this.Json(data);
            }
            else
                return null;
        }

        public JsonResult GetTailoringSupplySpecificProductUnit()
        {
            if (!Authentication.CheckUser(this.HttpContext))
                return this.Json(new RequestResult() { RedirectURL = Config.GetConfigValue("LoginPage") + "?ReturnUrl=" + HttpUtility.UrlEncode("Account/Manage"), Result = RequestResultType.Reload });

            long.TryParse(Request.Form["TailoringSupplySpecificProductUnitId"], out var tailoringSupplySpecificProductUnitId);

            if (tailoringSupplySpecificProductUnitId > 0)
            {
                var tailoringSupplySpecificProductUnit = TailoringSupplySpecificProductUnit.PopulateById(tailoringSupplySpecificProductUnitId);
                var findingLocationStorageTailoringSupplySpecificProductUnits = FindingLocationStorageTailoringSupplySpecificProductUnit.PopulateByParentIdBase(tailoringSupplySpecificProductUnitId);

                var data = new
                {
                    tailoringSupplySpecificProductUnit.Id,
                    FactoryTailoringId = tailoringSupplySpecificProductUnit.FactoryTailoring.Id,
                    tailoringSupplySpecificProductUnit.TailoringCost,
                    tailoringSupplySpecificProductUnit.Count,
                    tailoringSupplySpecificProductUnit.DateString,
                    FindingLocationStorageTailoringSupplySpecificProductUnits = findingLocationStorageTailoringSupplySpecificProductUnits
                        .Select(flstsspu => new
                        {
                            flstsspu.Id,
                            flstsspu.Consumption,
                            flstsspu.LocationStorage,
                            flstsspu.FindingColor
                        })
                };

                return this.Json(data);
            }
            else
                return null;
        }

        public JsonResult GetSupplySpecificProductUnit()
        {
            if (!Authentication.CheckUser(this.HttpContext))
                return this.Json(new RequestResult() { RedirectURL = Config.GetConfigValue("LoginPage") + "?ReturnUrl=" + HttpUtility.UrlEncode("Account/Manage"), Result = RequestResultType.Reload });

            long.TryParse(Request.Form["SupplySpecificProductUnitId"], out var supplySpecificProductUnitId);

            if (supplySpecificProductUnitId > 0)
            {
                var supplySpecificProductUnit = SupplySpecificProductUnit.PopulateById(supplySpecificProductUnitId);

                var data = new { supplySpecificProductUnit.Id, supplySpecificProductUnit.Count, supplySpecificProductUnit.TotalImplementCount, supplySpecificProductUnit.TotalTailoringCount };

                return this.Json(data);
            }
            else
                return null;
        }

        public ViewResult GetFindingSubspecieOptions()
        {
            long.TryParse(Request.Form["FindingSpecieId"], out var findingSpecieId);

            ViewData["AllowDefault"] = true;

            return View("../Controls/_dropdown_options", FindingSpecie.GetSubspecieList(findingSpecieId).Values.ToList());
        }

        public ViewResult GetFindingColorOptions()
        {
            long.TryParse(Request.Form["FindingId"], out var FindingId);

            ViewData["AllowDefault"] = true;

            return View("../Controls/_dropdown_options", new Finding(FindingId).GetColors().Values.ToList());
        }

        public ViewResult GetTextileColorOptions()
        {
            long.TryParse(Request.Form["TextileId"], out var TextileId);

            ViewData["AllowDefault"] = true;

            return View("../Controls/_dropdown_options", new Textile(TextileId).GetColors().Values.ToList());
        }

        public long SaveLocationStorage()
        {
            if (long.TryParse(Request.Form["FindingLocationStorageTailoringSupplySpecificProductUnit"], out var findingLocationStorageTailoringSupplySpecificProductUnitId) &&
                long.TryParse(Request.Form["TailoringSupplySpecificProductUnit"], out var tailoringSupplySpecificProductUnitId) &&
                long.TryParse(Request.Form["LocationStorage"], out var locationStorageId) &&
                long.TryParse(Request.Form["FindingColor"], out var findingColorId) &&
                decimal.TryParse(Request.Form["Consumption"], out var consumption))
            {
                var findingLocationStorageTailoringSupplySpecificProductUnit = new FindingLocationStorageTailoringSupplySpecificProductUnit(findingLocationStorageTailoringSupplySpecificProductUnitId)
                {
                    TailoringSupplySpecificProductUnit = new TailoringSupplySpecificProductUnit(tailoringSupplySpecificProductUnitId),
                    LocationStorage = new LocationStorage(locationStorageId),
                    FindingColor = new FindingColor(findingColorId),
                    Consumption = consumption
                };

                findingLocationStorageTailoringSupplySpecificProductUnit.SaveForm();

                return findingLocationStorageTailoringSupplySpecificProductUnit.Id;
            }

            return default;
        }

        public int UpdateInventoryCurrentCount()
        {
            int oldCurrentCount = 0;

            if (long.TryParse(Request.Form["SpecificProductId"], out var specificProductId) &&
                int.TryParse(Request.Form["CurrentCount"], out var currentCount) &&
                long.TryParse(Request.Form["InventoryId"], out var inventoryId) &&
                long.TryParse(Request.Form["StockId"], out var stockId))
            {
                var stock = new Stock(stockId);
                var specificProduct = new SpecificProduct(specificProductId);
                var inventory = Inventory.PopulateById(inventoryId);

                var specificProductStock = SpecificProductStock.GetBySpecificProduct(specificProduct, stock);

                if (specificProductStock.Id <= 0)
                {
                    specificProductStock = new SpecificProductStock()
                    {
                        SpecificProduct = specificProduct,
                        Stock = stock
                    };

                    specificProductStock.Insert(specificProductStock);
                }

                var inventoryUnit = InventoryUnit.PopulateBySpecificProductStockIdAndInventoryId(specificProductStock, inventory);

                if (inventoryUnit.Id > 0)
                {
                    oldCurrentCount = inventoryUnit.CurrentCount;

                    inventoryUnit.CurrentCount = currentCount;

                    inventoryUnit.UpdateProperties(nameof(inventoryUnit.CurrentCount), currentCount);
                }
                else
                {
                    var specificProductStockOld = SpecificProductStockHistory.PopulateBySpecificProductStockIdAndDate(specificProductStock, inventory.Date);

                    inventoryUnit = new InventoryUnit
                    {
                        Inventory = inventory,
                        SpecificProductStock = specificProductStock,
                        CountInStock = specificProductStockOld.CurrentCount,
                        CurrentCount = currentCount
                    };

                    inventoryUnit.Insert(inventoryUnit);
                }
            }

            return oldCurrentCount;
        }

        public void UpdateInventoryList()
        {
            var lastInventory = ItemBase.GetLast<Inventory>();

            if (lastInventory.Date.Date != DateTime.Today.Date)
            {
                var inventory = new Inventory { Date = DateTime.Today };

                inventory.Insert(inventory);
            }
        }
    }
}