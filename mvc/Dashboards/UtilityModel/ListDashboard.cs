using System;
using System.Collections.Generic;
using JuliaAlert.Models.Objects;
using LIB.BusinessObjects;
using LIB.Tools.BO;

namespace JuliaAlert.Dashboards.UtilityModel
{
    public struct ListDashboard
    {
        public readonly static ListDashboard OrderList = new ListDashboard("Список заказов", "Order/OrderList", Order.LoadList);
        public readonly static ListDashboard OrderLocalList = new ListDashboard("Список заказов MD", "Order/OrderList_Local", Order.LoadLocalList);
        public readonly static ListDashboard UserList = new ListDashboard("Список пользователей", "User/UserList", User.LoadList);
        public readonly static ListDashboard SupplyFindingList = new ListDashboard("Список приходов фурнитуры", "Supply/SupplyFindingList", SupplyFinding.LoadList);
        public readonly static ListDashboard SupplyTextileList = new ListDashboard("Список приходов ткани", "Supply/SupplyTextileList", SupplyTextile.LoadList);
        public readonly static ListDashboard SupplyModelList = new ListDashboard("Список приходов модели", "Supply/SupplyModelList", SupplySpecificProduct.LoadList);
        public readonly static ListDashboard SpecificProductMainStockList = new ListDashboard("Главный склад моделей", "Stock/SpecificProductMainStockList", SpecificProductStock.LoadMainStockList);
        public readonly static ListDashboard SpecificProductSubsidiaryStockList = new ListDashboard("Склады моделей", "Stock/SpecificProductSubsidiaryStockList", SpecificProductStock.LoadSubsidiaryStockList);

        private ListDashboard(string Title, string ViewName, Func<List<ItemBase>> Func)
        {
            this.Title = Title;
            this.ViewName = ViewName;
            this.Func = Func;
        }

        public readonly string Title;
        public readonly string ViewName;
        private readonly Func<List<ItemBase>> Func;

        public List<ItemBase> Data => Func?.Invoke();
    }
}