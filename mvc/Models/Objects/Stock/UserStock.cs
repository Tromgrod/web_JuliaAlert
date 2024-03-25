using System;
using LIB.AdvancedProperties;
using LIB.BusinessObjects;
using LIB.Tools.BO;
using LIB.Tools.AdminArea;

namespace JuliaAlert.Models.Objects
{
    [Serializable]
    [Bo(Group = AdminAreaGroupenum.Stock
      , ModulesAccess = (long)(Modulesenum.SMI)
      , DisplayName = "Доступы к складам"
      , SingleName = "Доступ к складу"
      , DeleteAccess = (long)BasePermissionenum.SuperAdmin
      , CreateAccess = (long)BasePermissionenum.SuperAdmin
      , LogRevisions = true
      , AllowCopy = false
      , Icon = "user")]
    public class UserStock : ItemBase
    {
        #region Constructors
        public UserStock()
            : base(0) { }

        public UserStock(long id)
            : base(id) { }
        #endregion

        #region Properties
        [Common(DisplayName = "Пользователь"), Template(Mode = Template.ParentDropDown)]
        public User User { get; set; }

        [Common(DisplayName = "Склад"), Template(Mode = Template.ParentDropDown)]
        public Stock Stock { get; set; }
        #endregion
    }
}