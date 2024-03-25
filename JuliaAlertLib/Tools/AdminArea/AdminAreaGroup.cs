using LIB.Tools.AdminArea;
using System.Collections.Generic;

namespace JuliaAlertLib.Tools.AdminArea
{
    public class AdminAreaGroup : LIB.Tools.AdminArea.AdminAreaGroup
    {
        public AdminAreaGroupenum Group { get; set; }

        public AdminAreaGroup Parent { get; set; }

        public static AdminAreaGroup UserManagement = new AdminAreaGroup() { Group = AdminAreaGroupenum.UserManagement, Name = "Управление пользователями", Icon = "users" };
        public static AdminAreaGroup Navigation = new AdminAreaGroup() { Group = AdminAreaGroupenum.Navigation, Name = "Навигация", Icon = "navicon" };
        public static AdminAreaGroup Model = new AdminAreaGroup() { Group = AdminAreaGroupenum.Model, Name = "Модель" };
        public static AdminAreaGroup ModelType = new AdminAreaGroup() { Group = AdminAreaGroupenum.ModelType, Name = "Тип модели" };
        public static AdminAreaGroup Geography = new AdminAreaGroup() { Group = AdminAreaGroupenum.Geography, Name = "География" };
        public static AdminAreaGroup Order = new AdminAreaGroup() { Group = AdminAreaGroupenum.Order, Name = "Заказ" };
        public static AdminAreaGroup Report = new AdminAreaGroup() { Group = AdminAreaGroupenum.Report, Name = "Отчет" };
        public static AdminAreaGroup Settings = new AdminAreaGroup() { Group = AdminAreaGroupenum.Settings, Name = "Настройки" };
        public static AdminAreaGroup Money = new AdminAreaGroup() { Group = AdminAreaGroupenum.Money, Name = "Деньги" };
        public static AdminAreaGroup Stock = new AdminAreaGroup() { Group = AdminAreaGroupenum.Stock, Name = "Склад" };
        public static AdminAreaGroup Production = new AdminAreaGroup() { Group = AdminAreaGroupenum.Production, Name = "Производство" };

        public static Dictionary<AdminAreaGroupenum, AdminAreaGroup> Groups => new Dictionary<AdminAreaGroupenum, AdminAreaGroup>
        {
            { UserManagement.Group, UserManagement },
            { Navigation.Group, Navigation },
            { Model.Group, Model },
            { ModelType.Group, ModelType },
            { Geography.Group, Geography },
            { Order.Group, Order },
            { Report.Group, Report },
            { Settings.Group, Settings },
            { Money.Group, Money },
            { Stock.Group, Stock },
            { Production.Group, Production }
        };
    }
}