// --------------------------------------------------------------------------------------------------------------------
// <copyright file="MenuItem.cs" company="JuliaAlert">
//   Copyright ©  2013
// </copyright>
// <summary>
//   The MenuItem.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace JuliaAlertLib.BusinessObjects
{
    using System;
    using LIB.AdvancedProperties;
    using LIB.BusinessObjects;
    using LIB.Tools.BO;
    using System.Collections.Generic;
    using System.Data;

    [Serializable]
    [Bo(ModulesAccess = (long)Modulesenum.SMI
      , DisplayName = "Optii meniu"
      , SingleName = "Optii meniu"
      , LogRevisions = true)]
    public class MenuItem : ItemBase
    {
        #region Constructors
        /// <summary>
        /// Initializes a new instance of the <see cref="MenuItemItem"/> class.
        /// </summary>
        public MenuItem()
            : base(0)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="MenuItemItem"/> class.
        /// </summary>
        /// <param name="id">
        /// The id.
        /// </param>
        public MenuItem(long id)
            : base(id)
        {
        }
        #endregion

        #region Properties

        [Common(Order = 0), Template(Mode = Template.Name)]
        public string Name { get; set; }

        [Common(Order = 1), Template(Mode = Template.SearchDropDown)]
        public MenuType MenuType { get; set; }

        [Common(Order = 2), Template(Mode = Template.SearchDropDown), LookUp(DefaultValue = true)]
        public Page Page { get; set; }

        [Common(Order = 3), Template(Mode = Template.PermissionsSelector), Access(DisplayMode = LIB.AdvancedProperties.DisplayMode.Advanced)]
        public long Permission { get; set; }

        [Common(Order = 2), Template(Mode = Template.String)]
        public string Object { get; set; }

        [Common(Order = 2), Template(Mode = Template.String)]
        public string Namespace { get; set; }

        [Common(Order = 4), Template(Mode = Template.CheckBox)]
        public bool Visible { get; set; }

        [Common(Order = 5), Template(Mode = Template.Number)]
        public int SortOrder { get; set; }

        [Common(Order = 6), Template(Mode = Template.ParentDropDown)]
        public MenuGroup MenuGroup { get; set; }

        #endregion
        
        #region Populate Methods
        public static Dictionary<long, ItemBase> FromDataTable(DataRow[] dt)
        {
            var MenuItems = new Dictionary<long, ItemBase>();
            foreach (var dr in dt)
            {
                var obj = (new MenuItem()).FromDataRow(dr);

                if (!MenuItems.ContainsKey(obj.Id))
                    MenuItems.Add(obj.Id, obj);
            }

            return MenuItems;
        }
        #endregion
    }
}