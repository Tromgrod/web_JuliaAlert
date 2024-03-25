using System;

namespace LIB.BusinessObjects
{
    /// <summary>
    /// The Permission Enumerator.
    /// </summary>
    [Flags]
    public enum BasePermissionenum : long
    {
        None = 0,
        CPAccess = 1,
        SuperAdmin = 2,
        SMIAccess = 4,
        EditOrder = 8,
        AddOrder = 16,
        MoneyInReportsAccess = 32,
        SalesDashboard = 64,
        ProductionDashboard = 128,
        WorkDashboard = 256,
        MDAccess = 512,
        Production = 1024,
        Sales = 2048,
        FactoryPrice = 4096
    }
}