// --------------------------------------------------------------------------------------------------------------------
// <copyright file="DisplayMode.cs" company="JuliaAlert">
//   Copyright ©  2013
// </copyright>
// <summary>
//   The DisplayMode.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace JuliaAlertLib.BusinessObjects
{
    using LIB.AdvancedProperties;
    using LIB.BusinessObjects;
    using LIB.Tools.BO;
    using System.Collections.Generic;
    using System.Data;

    public class DisplayMode : ItemBase
    {
        public static DisplayMode Simple = new DisplayMode(1);
        public static DisplayMode Advanced = new DisplayMode(2);
        public static DisplayMode Search = new DisplayMode(3);
        public static DisplayMode AdvancedEdit = new DisplayMode(4);
        public static DisplayMode Print = new DisplayMode(5);
        public static DisplayMode PrintSearch = new DisplayMode(6);
        public static DisplayMode CSV = new DisplayMode(7);
        public static DisplayMode Excell = new DisplayMode(8);

        #region Constructors
        public DisplayMode()
            : base(0) { }

        public DisplayMode(long id)
            : base(id) { }
        #endregion

        #region Properties
        [Template(Mode = Template.Name)]
        public string Name { get; set; }

        [Template(Mode = Template.Number),
        Access(EditableFor = (long)BasePermissionenum.SuperAdmin, VisibleFor = (long)BasePermissionenum.SuperAdmin)]
        public long Value { get; set; }
        #endregion
        
        public static Dictionary<long, ItemBase> FromDataTable(DataRow[] dt, DataSet ds)
        {
            var displayModes = new Dictionary<long, ItemBase>();
            foreach (var dr in dt)
            {
                var obj = (new DisplayMode()).FromDataRow(dr);
                if (!displayModes.ContainsKey(obj.Id))
                {
                    displayModes.Add(obj.Id, obj);
                }
            }

            return displayModes;
        }
    }
}