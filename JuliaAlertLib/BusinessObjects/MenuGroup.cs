// --------------------------------------------------------------------------------------------------------------------
// <copyright file="MenuGroup.cs" company="JuliaAlert">
//   Copyright ©  2013
// </copyright>
// <summary>
//   The MenuGroup.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace JuliaAlertLib.BusinessObjects
{
    using System;
    using System.Data;
    using System.Data.SqlClient;
    using System.Collections.Generic;
    using LIB.AdvancedProperties;
    using LIB.Tools.BO;
    using LIB.Tools.Utils;
    using LIB.Tools.AdminArea;
    using LIB.BusinessObjects;

    [Serializable]
    [Bo(Group = AdminAreaGroupenum.Navigation
      , ModulesAccess = (long)(Modulesenum.SMI)
      , DisplayName = "Группы меню"
      , SingleName = "Группа меню"
      , LogRevisions = true
      , Icon = "list-alt")]
    public class MenuGroup : ItemBase
    {
        #region Constructors
        /// <summary>
        /// Initializes a new instance of the <see cref="MenuGroup"/> class.
        /// </summary>
        public MenuGroup()
            : base(0)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="MenuGroup"/> class.
        /// </summary>
        /// <param name="id">
        /// The id.
        /// </param>
        public MenuGroup(long id)
            : base(id)
        {
        }
        #endregion
        
        #region Properties

        [Common(Order = 0, DisplayName = "Denumire"), Template(Mode = Template.Name)]
        public string Name { get; set; }
        
        [Common(Order = 1, DisplayName = "Tip meniu"), Template(Mode = Template.SearchDropDown)]
        public MenuType MenuType { get; set; }

        [Common(Order = 2, DisplayName = "Permisiune"), Template(Mode = Template.PermissionsSelector), Access(DisplayMode = LIB.AdvancedProperties.DisplayMode.Advanced)]
        public long Permission { get; set; }

        [Common(Order = 3), Template(Mode = Template.LinkItems), LinkItem(LinkType = typeof(MenuItem)), Access(DisplayMode = LIB.AdvancedProperties.DisplayMode.Advanced| LIB.AdvancedProperties.DisplayMode.Simple)]
        public Dictionary<long, ItemBase> MenuItems { get; set; }

        [Common(Order = 4, DisplayName = "Vizibil"), Template(Mode = Template.CheckBox)]
        public bool Visible { get; set; }

        [Common(Order = 5, DisplayName = "Nr. sort."), Template(Mode = Template.Number)]
        public int SortOrder { get; set; }


        #endregion

        #region Populate Methods
        public static Dictionary<long, MenuGroup> Populate(LIB.BusinessObjects.User usr)
        {
            var selectCommand = new SqlCommand("MenueGroup_Populate", DataBase.ConnectionFromContext()) { CommandType = CommandType.StoredProcedure };

            var ds = new DataSet();
            var da = new SqlDataAdapter { SelectCommand = selectCommand };
            da.Fill(ds);

            ds.Tables[0].TableName = "MenueGroup";
            ds.Tables[1].TableName = "Menues";

            ds.Relations.Add(
            ds.Tables["MenueGroup"].Columns["MenuGroupId"],
            ds.Tables["Menues"].Columns["MenuGroupId"]);

            ds.Relations[0].Nested = true;

            ds.Relations[0].RelationName = "MenueGroup_Menues";

            var menuGroups = new Dictionary<long, MenuGroup>();

            if (usr == null || usr.Role == null)
                return menuGroups;

            foreach (DataRow dr in ds.Tables["MenueGroup"].Rows)
            {
                var obj = (MenuGroup)new MenuGroup().FromDataRow(dr);
                obj.MenuItems = new Dictionary<long, ItemBase>();
                var datarowsMenues = dr.GetChildRows(ds.Relations["MenueGroup_Menues"]);
                obj.MenuItems = MenuItem.FromDataTable(datarowsMenues);
                if (obj.MenuItems != null && obj.MenuItems.Count > 0)
                    menuGroups.Add(obj.Id, obj);
            }

            return menuGroups;
        }
        #endregion
    }
}