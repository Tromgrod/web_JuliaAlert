// --------------------------------------------------------------------------------------------------------------------
// <copyright file="PredefinedColor.cs" company="GalexStudio">
//   Copyright ©  2018
// </copyright>
// <summary>
//   Defines the PredefinedColor type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace LIB.BusinessObjects
{
    using LIB.AdvancedProperties;
    using LIB.Tools.AdminArea;
    using LIB.Tools.BO;
    using System;

    [Serializable]
    [Bo(Group = AdminAreaGroupenum.Settings,
        ModulesAccess = (long)Modulesenum.SMI,
        DisplayName = "Системные цвета",
        SingleName = "Системный цвет",
        EditAccess = (long)BasePermissionenum.SuperAdmin,
        CreateAccess = (long)BasePermissionenum.SuperAdmin,
        DeleteAccess = (long)BasePermissionenum.SuperAdmin,
        ReadAccess = (long)BasePermissionenum.SuperAdmin,
        RevisionsAccess = (long)BasePermissionenum.SuperAdmin,
        Icon = "paint-brush")]
    public class PredefinedColor : ItemBase
    {
        #region Constructors
        public PredefinedColor()
            : base(0) { }

        public PredefinedColor(long id)
            : base(id) { }

        public PredefinedColor(int id, string name)
            : base(id) => this.Name = name;
        #endregion

        public override string GetAdditionalSelectQuery(AdvancedProperty property)
            => ",[" + property.PropertyName + "].Code" + " AS " + property.PropertyName + "Code,[" + property.PropertyName + "].Color" + " AS " + property.PropertyName + "Color";

        #region PredefinedColor Properties
        [Template(Mode = Template.Name)]
        public string Name { get; set; }

        [Template(Mode = Template.Name)]
        public string Code { get; set; }

        [Template(Mode = Template.ColorPicker), Access(DisplayMode = DisplayMode.Simple | DisplayMode.Advanced)]
        public string Color { get; set; }
        #endregion
    }
}