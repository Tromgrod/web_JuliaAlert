// --------------------------------------------------------------------------------------------------------------------
// <copyright file="Permission.cs" company="GalexStudio">
//   Copyright ©  2022
// </copyright>
// <summary>
//   Defines the Permission type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace LIB.BusinessObjects
{
    using LIB.Tools.AdminArea;
    using LIB.AdvancedProperties;
    using LIB.Tools.BO;
    using System.Collections.Generic;

    [Bo(Group = AdminAreaGroupenum.UserManagement
      , ModulesAccess = (long)Modulesenum.SMI
      , DisplayName = "Уровни доступа"
      , SingleName = "Уровень доступа"
      , EditAccess = (long)BasePermissionenum.SuperAdmin
      , CreateAccess = (long)BasePermissionenum.SuperAdmin
      , DeleteAccess = (long)BasePermissionenum.SuperAdmin
      , ReadAccess = (long)BasePermissionenum.SuperAdmin
      , LogRevisions = false
      , RevisionsAccess = (long)BasePermissionenum.SuperAdmin
      , Icon ="laptop")]
    public class Permission : ItemBase
    {
        #region Static Permission
        public static readonly Permission None = new Permission(0, 0);

        public static readonly Permission CPAccess = new Permission(1, (long)BasePermissionenum.CPAccess) { Name = "Допуск к CP" };

        public static readonly Permission SuperAdmin = new Permission(2, (long)BasePermissionenum.SuperAdmin) { Name = "Права администратора" };

        public static readonly Permission SMIAccess = new Permission(3, (long)BasePermissionenum.SMIAccess) { Name = "Допуск к SMI" };

        public static readonly Permission EditOrderAccess = new Permission(4, (long)BasePermissionenum.EditOrder) { Name = "Допуск к редактированию заказов" };

        public static readonly Permission AddOrderAccess = new Permission(5, (long)BasePermissionenum.AddOrder) { Name = "Допуск к созданию заказов" };

        public static readonly Permission MoneyInReportsAccess = new Permission(6, (long)BasePermissionenum.MoneyInReportsAccess) { Name = "Допуск к деньгам в отчетах" };

        public static readonly Permission SalesDashboardAccess = new Permission(7, (long)BasePermissionenum.SalesDashboard) { Name = "Dashboard продаж" };

        public static readonly Permission PoductionDashboardAccess = new Permission(8, (long)BasePermissionenum.ProductionDashboard) { Name = "Dashboard производства" };

        public static readonly Permission WorkDashboardAccess = new Permission(9, (long)BasePermissionenum.WorkDashboard) { Name = "Dashboard работника" };

        public static readonly Permission MDAccess = new Permission(10, (long)BasePermissionenum.MDAccess) { Name = "Пропуск к секции молдовы" };

        public static readonly Permission ProductionAccess = new Permission(11, (long)BasePermissionenum.Production) { Name = "Пропуск к производству" };

        public static readonly Permission SalesAccess = new Permission(12, (long)BasePermissionenum.Sales) { Name = "Пропуск к продажам" };

        public static readonly Permission FactoryPriceVisible = new Permission(13, (long)BasePermissionenum.FactoryPrice) { Name = "Показ цены фабрики" };
        #endregion

        public override string GetName() => this.Name;

        public static Dictionary<long, ItemBase> LoadPermissions(long Permissions)
        {
            var Perms = new Dictionary<long, ItemBase>();

            if (Tools.Security.Permissions.HasPermissions(Permissions, CPAccess.Value))
                Perms.Add(CPAccess.Id, CPAccess);

            if (Tools.Security.Permissions.HasPermissions(Permissions, SuperAdmin.Value))
                Perms.Add(SuperAdmin.Id, SuperAdmin);  
            
            if (Tools.Security.Permissions.HasPermissions(Permissions, SMIAccess.Value))
                Perms.Add(SMIAccess.Id, SMIAccess);

            if (Tools.Security.Permissions.HasPermissions(Permissions, EditOrderAccess.Value))
                Perms.Add(EditOrderAccess.Id, EditOrderAccess);

            if (Tools.Security.Permissions.HasPermissions(Permissions, AddOrderAccess.Value))
                Perms.Add(AddOrderAccess.Id, AddOrderAccess);

            if (Tools.Security.Permissions.HasPermissions(Permissions, MoneyInReportsAccess.Value))
                Perms.Add(MoneyInReportsAccess.Id, MoneyInReportsAccess);

            if (Tools.Security.Permissions.HasPermissions(Permissions, SalesDashboardAccess.Value))
                Perms.Add(SalesDashboardAccess.Id, SalesDashboardAccess);

            if (Tools.Security.Permissions.HasPermissions(Permissions, PoductionDashboardAccess.Value))
                Perms.Add(PoductionDashboardAccess.Id, PoductionDashboardAccess);

            if (Tools.Security.Permissions.HasPermissions(Permissions, WorkDashboardAccess.Value))
                Perms.Add(WorkDashboardAccess.Id, WorkDashboardAccess);

            if (Tools.Security.Permissions.HasPermissions(Permissions, MDAccess.Value))
                Perms.Add(MDAccess.Id, MDAccess);

            if (Tools.Security.Permissions.HasPermissions(Permissions, ProductionAccess.Value))
                Perms.Add(ProductionAccess.Id, ProductionAccess);

            if (Tools.Security.Permissions.HasPermissions(Permissions, SalesAccess.Value))
                Perms.Add(SalesAccess.Id, SalesAccess);

            if (Tools.Security.Permissions.HasPermissions(Permissions, FactoryPriceVisible.Value))
                Perms.Add(FactoryPriceVisible.Id, FactoryPriceVisible);

            return Perms;
        }

        public static Permission LoadPermission(long id)
        {
            if (CPAccess.Id == id)
                return CPAccess;

            if (SuperAdmin.Id == id)
                return SuperAdmin;

            if (SMIAccess.Id == id)
                return SMIAccess;

            if (EditOrderAccess.Id == id)
                return EditOrderAccess;

            if (AddOrderAccess.Id == id)
                return AddOrderAccess;

            if (MoneyInReportsAccess.Id == id)
                return MoneyInReportsAccess;

            if (SalesDashboardAccess.Id == id)
                return SalesDashboardAccess;

            if (PoductionDashboardAccess.Id == id)
                return PoductionDashboardAccess;

            if (WorkDashboardAccess.Id == id)
                return WorkDashboardAccess;

            if (MDAccess.Id == id)
                return MDAccess;

            if (ProductionAccess.Id == id)
                return ProductionAccess;

            if (SalesAccess.Id == id)
                return SalesAccess;

            if (FactoryPriceVisible.Id == id)
                return FactoryPriceVisible;

            return None;
        }

        #region Constructors
        public Permission()
            : base(0) { }

        public Permission(long id)
            : base(id) { }

        public Permission(long id, long value)
            : base(id) => this.Value = value;
        #endregion

        #region Properties
        [Template(Mode = Template.Name)]
        [Validation(ValidationType = ValidationTypes.Required),
         Access(DisplayMode = DisplayMode.Search | DisplayMode.Simple | DisplayMode.Advanced, EditableFor = (long)BasePermissionenum.SuperAdmin)]
        public string Name { get; set; }

        [Template(Mode = Template.Number),
        Access(EditableFor = (long)BasePermissionenum.SuperAdmin, VisibleFor = (long)BasePermissionenum.SuperAdmin)]
        public long Value { get; set; }
        #endregion
    }
}